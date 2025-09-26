using AutoMapper;
using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public EnrollmentService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        // CREATE
        public async Task<EnrollmentResponseDto> EnrollAsync(string studentUserId, string courseId)
        {
            // Find User
            var studentProfile = await _uow.StudentProfiles.FindSingleAsync(sp => sp.User.Id == studentUserId);
            if (studentProfile == null) throw new KeyNotFoundException("Student not found");

            // Find Course
            if (!Guid.TryParse(courseId, out Guid guid))
            {
                throw new Exception("Invalid Course id");
            }

            var course = await _uow.Courses.FindSingleAsync(c => c.Id == guid);

            if (course == null || course.Status != CourseStatus.Published)
                throw new KeyNotFoundException("Course not found");

            // Check if User is already enrolled
            if (await _uow.Enrollments.FindFirstAsync(e => e.CourseId == course.Id && e.StudentId == studentProfile.Id) != null)
                throw new Exception("You are already enrolled to this course");
            
            // Check if Course has ended
            if (DateHasPassed(course.Schedule.EndDate))
                throw new Exception("Course ended");

            // Check Max Capacity
            if (course.MaxCapacity > 0 && course.Enrollments.Count() == course.MaxCapacity)
                throw new Exception("No spots left. Course reached the max capacity");

            // Check if user completed all prerequisites
            foreach (var prerequisite in course.Prerequisites)
            {
                if (await StudentCompletedCourse(studentProfile.Id, prerequisite.PrerequisiteCourseId) == false)
                    throw new Exception("You must complete all prerequisite courses.");
            }

            // Enroll
            var enrollment = new Enrollment
            {
                Course = course,
                Student = studentProfile,
                Status = EnrollmentStatus.Pending,
            };

            await _uow.Enrollments.AddAsync(enrollment);
            
            await _uow.CompleteAsync();
            return _mapper.Map<EnrollmentResponseDto>(enrollment);
        }

        // READ
        public async Task<IEnumerable<EnrollmentResponseDto>> GetAllAsync()
        {
            // Get enrollments
            var enrollments = await _uow.Enrollments.GetAllAsync();

            return _mapper.Map<IEnumerable<EnrollmentResponseDto>>(enrollments);
        }

        public async Task<IEnumerable<EnrollmentResponseDto>> GetByCourseAsync(string courseId)
        {
            if (!Guid.TryParse(courseId, out Guid guid))
            {
                throw new Exception("Invalid course id");
            }
            var enrollments = await _uow.Enrollments.FindAsync(e => e.CourseId == guid);

            return _mapper.Map<IEnumerable<EnrollmentResponseDto>>(enrollments);
        }

        public async Task<IEnumerable<EnrollmentResponseDto>> GetByStudentAsync(string studentProfileId, EnrollmentStatus? statusFilter)
        {
            // Find student
            if (!Guid.TryParse(studentProfileId, out Guid guid))
            {
                throw new Exception("Invalid student profile id");
            }

            var student = await _uow.StudentProfiles.FindSingleAsync(sp => sp.Id == guid);

            if (student == null) throw new KeyNotFoundException("Student not found");

            // Get enrollments
           
            var enrollments = await _uow.Enrollments.FindAsync(e => 
                e.StudentId == guid && 
                statusFilter == null ? true : e.Status == statusFilter);

            return _mapper.Map<IEnumerable<EnrollmentResponseDto>>(enrollments);
        }

        public async Task<IEnumerable<EnrollmentResponseDto>> GetMyEnrollmentsAsync(string studentId, EnrollmentStatus? statusFilter)
        {
            // Find student
            var student = await _uow.Users.FindSingleWithProfileAsync(u => u.Id == studentId);

            if (student == null) throw new KeyNotFoundException("Student not found");
            
            // Get enrollments
            if (student.StudentProfile == null) throw new KeyNotFoundException("Student profile not found");
            
            var enrollments = await _uow.Enrollments.FindAsync(e =>
                e.StudentId == student.StudentProfile.Id &&
                (statusFilter == null ? true : e.Status == statusFilter));

            return _mapper.Map<IEnumerable<EnrollmentResponseDto>>(enrollments);
        }

        public async Task<EnrollmentResponseDto> GetOneAsync(string courseId, string studentProfileId, string requesterId)
        {
            // Authorize user
            var requester = await _uow.Users.FindSingleWithProfileAsync(u => u.Id == requesterId);
            if (requester == null) throw new UnauthorizedAccessException("Unauthorized. Unable to find user.");

            // Find Student profile
            if (!Guid.TryParse(studentProfileId, out Guid studentProfileGuid))
            {
                throw new Exception("Invalid student profile id");
            }
            var studentProfile = await _uow.StudentProfiles.FindSingleAsync(sp => sp.Id == studentProfileGuid);
            if (studentProfile == null) throw new KeyNotFoundException("Student Profile not found");

            // students can only see their own enrollments
            if (requester.Role.Name == RoleConstants.Student && requester.Id != studentProfile.User.Id)
                throw new UnauthorizedAccessException("Students cannot see other students' enrollments");

            // Find course
            if (!Guid.TryParse(courseId, out Guid courseGuid))
            {
                throw new Exception("Invalid course id");
            }
            var course = await _uow.Courses.FindSingleAsync(c => c.Id == courseGuid);
            if (course == null) throw new KeyNotFoundException("Course not found");

            // tutors can only see enrollments to courses they're assigned to
            if (requester.Role.Name == RoleConstants.Tutor && !course.TutorProfiles.Any(tp => tp.User.Id == requesterId))
                throw new UnauthorizedAccessException("Tutors can only see enrollments to courses they're assigned to.");

            // Get enrollement
            var enrollment = await _uow.Enrollments.FindSingleAsync(e => e.CourseId == courseGuid && e.StudentId == studentProfileGuid);
            if (enrollment == null) throw new KeyNotFoundException("Enrollment not found");

            return _mapper.Map<EnrollmentResponseDto>(enrollment);
        }

        // UPDATE
        public async Task<EnrollmentResponseDto> UpdateAsync(string courseId, string studentProfileId, string requesterId, EnrollmentStatus status)
        {
            // Authorize user
            var requester = await _uow.Users.FindSingleWithProfileAsync(u => u.Id == requesterId);
            if (requester == null) throw new UnauthorizedAccessException("Unauthorized. Unable to find user.");

            // Find Student profile
            if (!Guid.TryParse(studentProfileId, out Guid studentProfileGuid))
            {
                throw new Exception("Invalid student profile id");
            }
            var studentProfile = await _uow.StudentProfiles.FindSingleAsync(sp => sp.Id == studentProfileGuid);
            if (studentProfile == null) throw new KeyNotFoundException("Student Profile not found");

            // students can only update their own enrollments
            bool isStudent = requester.Role.Name == RoleConstants.Student;
            if (isStudent && requester.Id != studentProfile.User.Id)
                throw new UnauthorizedAccessException("Students cannot update other students' enrollments");

            // Find course
            if (!Guid.TryParse(courseId, out Guid courseGuid))
            {
                throw new Exception("Invalid course id");
            }
            var course = await _uow.Courses.FindSingleAsync(c => c.Id == courseGuid);
            if (course == null || (isStudent && course.Status != CourseStatus.Published)) throw new KeyNotFoundException("Course not found");

            // tutors can only update enrollments to courses they're assigned to
            var isTutor = requester.Role.Name == RoleConstants.Tutor;
            if (isTutor && !course.TutorProfiles.Any(tp => tp.User.Id == requesterId))
                throw new UnauthorizedAccessException("Tutors can only update enrollments to courses they're assigned to.");

            // Get enrollement
            var enrollment = await _uow.Enrollments.FindSingleAsync(e => e.CourseId == courseGuid && e.StudentId == studentProfileGuid);
            if (enrollment == null) throw new KeyNotFoundException("Enrollment not found");

            // Update Enrollement Status
            if (isTutor)
            {
                switch (enrollment.Status)
                {
                    case EnrollmentStatus.Pending:
                        if (status != EnrollmentStatus.Active && status != EnrollmentStatus.Suspended)
                            throw new Exception($"Cannot go from 'Pending' status to {status} status");

                        if (DateHasPassed(course.Schedule.EndDate))
                            throw new Exception($"Cannot go to {status} status since the course has ended");

                        enrollment.Status = status;
                        break;

                    case EnrollmentStatus.Active:
                        if (status != EnrollmentStatus.Passed && status != EnrollmentStatus.Failed && status != EnrollmentStatus.Suspended)
                            throw new Exception($"Cannot go from 'Active' status to {status} status");

                        if ((status  == EnrollmentStatus.Passed || status == EnrollmentStatus.Failed) && !DateHasPassed(course.Schedule.EndDate))
                            throw new Exception($"Cannot go to {status} status since the course has not ended yet");

                        else if (status == EnrollmentStatus.Suspended && DateHasPassed(course.Schedule.EndDate))
                            throw new Exception($"Cannot go to {status} status since the course has ended");

                            enrollment.Status = status;
                        break;

                    case EnrollmentStatus.Suspended:
                        if (status != EnrollmentStatus.Pending && status != EnrollmentStatus.Active)
                            throw new Exception($"Cannot go from 'Suspended' status to {status} status");

                        if (status == EnrollmentStatus.Active && DateHasPassed(course.Schedule.EndDate))
                            throw new Exception($"Cannot go to {status} status since the course has ended");

                        enrollment.Status = status;
                        break;

                    default:
                        throw new Exception($"Cannot go from {enrollment.Status} status to {status} status");
                }
            }
            if (isStudent)
            {
                switch (enrollment.Status)
                {
                    case EnrollmentStatus.Pending:
                        if (status != EnrollmentStatus.Dropped)
                            throw new Exception($"Cannot go from 'Pending' status to {status} status");
                        enrollment.Status = status;
                        break;

                    case EnrollmentStatus.Active:
                        if (status != EnrollmentStatus.Dropped)
                            throw new Exception($"Cannot go from 'Active' status to {status} status");
                        enrollment.Status = status;
                        break;

                    case EnrollmentStatus.Dropped:
                        if (status != EnrollmentStatus.Pending)
                            throw new Exception($"Cannot go from 'Dropped' status to {status} status");

                        if (DateHasPassed(course.Schedule.EndDate))
                            throw new Exception($"Cannot go to {status} status since the course has ended");

                        enrollment.Status = status;
                        break;

                    default:
                        throw new Exception($"Cannot go from {enrollment.Status} status to {status} status");
                }
            }

            _uow.Enrollments.Update(enrollment);
            await _uow.CompleteAsync();

            return _mapper.Map<EnrollmentResponseDto>(enrollment);
        }


        // --------------- HELPERS -------------------
        private async Task<bool> StudentCompletedCourse(Guid studentProfileId, Guid courseId)
        {
            var enrollment = await _uow.Enrollments.FindSingleAsync(e => 
            e.StudentId == studentProfileId && 
            e.CourseId == courseId && 
            e.Status == EnrollmentStatus.Passed);

            return enrollment != null;
        }

        private bool DateHasPassed(string date)
        {
            if (DateTime.TryParseExact(date, "yyyy-MM-dd", null, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate.Date < DateTime.Now.Date;
            }
            return false;
        }

        
    }

}
