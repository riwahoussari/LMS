using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Enums;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _uow;
        private readonly AppDbContext _context;

        public AnalyticsService(IUnitOfWork uow, AppDbContext context)
        {
            _uow = uow;
            _context = context;
        }

        // USERS
        public async Task<UserAnalyticsDto> GetUserAnalyticsAsync()
        {
            // Identity users live in auth schema but exposed via DbSet<AppUser> Users
            var total = await _uow.Users.CountAsync(u => true);

            // roles are IdentityRole entries; AppUser.RoleId points to Role.Id (string) in your model.
            var totalAdmins = await _uow.Users.CountAsync(u => u.Role != null && u.Role.Name == "admin");
            var totalTutors = await _uow.Users.CountAsync(u => u.Role != null && u.Role.Name == "tutor");
            var totalStudents = await _uow.Users.CountAsync(u => u.Role != null && u.Role.Name == "student");

            var since = DateTime.UtcNow.Date.AddDays(-29); // last 30 days inclusive
            var growth = await _context.Users
                .Where(u => u.CreatedAt >= since)
                .GroupBy(u => u.CreatedAt.Date)
                .Select(g => new UserGrowthPoint { Date = g.Key, Count = g.Count() })
                .OrderBy(p => p.Date)
                .ToArrayAsync();

            return new UserAnalyticsDto
            {
                TotalUsers = total,
                TotalAdmins = totalAdmins,
                TotalTutors = totalTutors,
                TotalStudents = totalStudents,
                GrowthLast30Days = growth
            };
        }

        // COURSES
        public async Task<CourseAnalyticsDto> GetCourseAnalyticsAsync()
        {
            var total = await _uow.Courses.CountAsync(c => true);
            var published = await _uow.Courses.CountAsync(c => c.Status == CourseStatus.Published);
            var draft = await _uow.Courses.CountAsync(c => c.Status == CourseStatus.Draft);
            var archived = await _uow.Courses.CountAsync(c => c.Status == CourseStatus.Archived);

            // average capacity utilization: for courses with MaxCapacity > 0, avg(enrolled / maxCapacity)
            var coursesWithCapacity = await _context.Courses
                .Where(c => c.MaxCapacity != null && c.MaxCapacity > 0)
                .Select(c => new {
                    c.Id,
                    c.MaxCapacity,
                    Enrolled = _context.Enrollments.Count(e => e.CourseId == c.Id && (e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Passed || e.Status == EnrollmentStatus.Failed))
                }).ToListAsync();

            double avgUtilPct = 0;
            if (coursesWithCapacity.Count > 0)
            {
                avgUtilPct = coursesWithCapacity
                    .Select(x => (double)x.Enrolled / Math.Max(1, x.MaxCapacity!.Value))
                    .Average() * 100.0;
            }

            // global completion rate = passed / (active+passed+failed) across enrollments
            var totalCompleted = await _context.Enrollments.CountAsync(e => e.Status == EnrollmentStatus.Passed);
            var totalRelevant = await _context.Enrollments.CountAsync(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Passed || e.Status == EnrollmentStatus.Failed);
            double completionRate = totalRelevant == 0 ? 0 : (double)totalCompleted / totalRelevant * 100.0;

            // top 10 courses by enrollment count
            var topCourses = await _context.Courses
                .Select(c => new CourseEnrollmentPoint
                {
                    CourseId = c.Id,
                    Title = c.Title,
                    Enrollments = _context.Enrollments.Count(e => e.CourseId == c.Id)
                })
                .OrderByDescending(x => x.Enrollments)
                .Take(10)
                .ToListAsync();

            return new CourseAnalyticsDto
            {
                TotalCourses = total,
                Published = published,
                Draft = draft,
                Archived = archived,
                AvgCapacityUtilizationPct = Math.Round(avgUtilPct, 2),
                GlobalCompletionRatePct = Math.Round(completionRate, 2),
                TopCoursesByEnrollment = topCourses
            };
        }

        // TUTORS
        public async Task<TutorAnalyticsDto> GetTutorAnalyticsAsync()
        {
            var totalTutors = await _context.TutorProfiles.CountAsync();

            // top tutors by enrollments: join tutor profiles -> course_tutor -> courses -> enrollments
            var tutorStats = await _context.TutorProfiles
                .Select(tp => new {
                    TutorId = tp.Id,
                    TutorName = tp.User != null ? tp.User.FirstName + " " + tp.User.LastName : "",
                    CoursesCount = tp.Courses.Count(),
                    TotalEnrollments = tp.Courses.Sum(c => _context.Enrollments.Count(e => e.CourseId == c.Id))
                })
                .OrderByDescending(x => x.TotalEnrollments)
                .Take(10)
                .ToListAsync();

            var items = tutorStats.Select(x => new TutorWorkloadItem
            {
                TutorId = x.TutorId.ToString(),
                TutorName = x.TutorName,
                CoursesCount = x.CoursesCount,
                TotalEnrollments = x.TotalEnrollments
            });

            return new TutorAnalyticsDto
            {
                TotalTutors = totalTutors,
                TopTutorsByEnrollments = items
            };
        }

        // STUDENTS
        public async Task<StudentAnalyticsDto> GetStudentAnalyticsAsync()
        {
            var totalStudents = await _context.StudentProfiles.CountAsync();

            // average active enrollments per student
            var avgActive = 0.0;
            if (totalStudents > 0)
            {
                avgActive = await _context.StudentProfiles
                    .Select(sp => _context.Enrollments.Count(e => e.StudentId == sp.Id && e.Status == EnrollmentStatus.Active))
                    .AverageAsync();
            }

            var totalDropped = await _context.Enrollments.CountAsync(e => e.Status == EnrollmentStatus.Dropped);
            var totalEnroll = await _context.Enrollments.CountAsync();
            double dropoutRate = totalEnroll == 0 ? 0 : (double)totalDropped / totalEnroll * 100.0;

            var totalPassed = await _context.Enrollments.CountAsync(e => e.Status == EnrollmentStatus.Passed);
            double passRate = totalEnroll == 0 ? 0 : (double)totalPassed / totalEnroll * 100.0;

            return new StudentAnalyticsDto
            {
                TotalStudents = totalStudents,
                AvgActiveEnrollmentsPerStudent = Math.Round(avgActive, 2),
                DropoutRatePct = Math.Round(dropoutRate, 2),
                PassRatePct = Math.Round(passRate, 2)
            };
        }

        

        public async Task<AnalyticsSummaryDto> GetSummaryAsync()
        {
            var users = await GetUserAnalyticsAsync();
            var courses = await GetCourseAnalyticsAsync();
            var tutors = await GetTutorAnalyticsAsync();
            var students = await GetStudentAnalyticsAsync();

            return new AnalyticsSummaryDto
            {
                Users = users,
                Courses = courses,
                Tutors = tutors,
                Students = students,
            };
        }
    }

}
