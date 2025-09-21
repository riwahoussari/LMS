using LMS.Domain.Interfaces;
using LMS.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        // Enrollments
        public IEnrollmentRepository Enrollments { get; }

        // Schedules
        public IScheduleRepository Schedules { get; }
        public IScheduleSessionRepository ScheduleSessions { get; }

        // Courses
        public ICategoryRepository Category { get; }
        public ITagRepository Tag { get; }
        public ICourseRepository Courses { get; }
        public IPrerequisiteRepository Prerequisites { get; }
        
        // Auth - Users
        public IRefreshTokenRepository RefreshTokens { get; }
        public IUserRepository Users { get; }
        public ITutorProfileRepository TutorProfiles { get; }
        public IStudentProfileRepository StudentProfiles { get; }


        public UnitOfWork(AppDbContext context,
                          IEnrollmentRepository enrollments,
                          IScheduleRepository schedules,
                          IScheduleSessionRepository scheduleSessions,
                          ICategoryRepository category,
                          ITagRepository tag,
                          IRefreshTokenRepository refreshTokens,
                          ICourseRepository courses,
                          IPrerequisiteRepository prerequisites,
                          IUserRepository users,
                          ITutorProfileRepository tutorProfiles,
                          IStudentProfileRepository studentProfiles)
        {
            _context = context;
            Enrollments = enrollments;
            Schedules = schedules;
            ScheduleSessions = scheduleSessions;
            Category = category;
            Tag = tag;
            Courses = courses;
            Prerequisites = prerequisites;
            RefreshTokens = refreshTokens;
            Users = users;
            TutorProfiles = tutorProfiles;
            StudentProfiles = studentProfiles;
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }

}
