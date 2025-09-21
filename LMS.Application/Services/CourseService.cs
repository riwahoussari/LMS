using AutoMapper;
using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Application.Strategies.Sorting;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Infrastructure;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly SortStrategyFactory<Course> _sorter;

        public CourseService(IUnitOfWork uow, IMapper mapper, SortStrategyFactory<Course> sorter)
        {
            _uow = uow;
            _mapper = mapper;
            _sorter = sorter;
        }

        
        // CREATE
        public async Task<CourseResponseDto> CreateAsync(string tutorId, CreateCourseDto dto)
        {
            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                MaxCapacity = dto.MaxCapacity,
            };

            await FindAndAssignTutor(tutorId, course);
            await FindAndAssignCategory(dto.CategoryId, course);
            await FindAndAssignTags(dto.TagIds, course);
            await FindAndAssignPrerequisites(dto.PrerequisiteIds, course);
            await CreateAndAssignSchedule(dto.Schedule, course);
            
            await _uow.Courses.AddAsync(course);

            try
            {
                await _uow.CompleteAsync();
            }
            catch (DbUpdateException ex)
            {
                // Check if it's a unique constraint violation
                if (ex.InnerException != null)
                {
                    if (ex.InnerException is PostgresException pgEx)
                    {
                        switch (pgEx.SqlState)
                        {
                            case "23505": // Unique constraint violation
                                throw new InvalidOperationException("A course with this title already exists.");
                            case "23503": // Foreign key constraint violation (in case category doesn't exist)
                                throw new InvalidOperationException("The specified category does not exist.");
                            default:
                                throw; // Re-throw other PostgreSQL exceptions
                        }
                    }
                }
                // Re-throw if it's not a PostgreSQL exception
                throw;
            }

            return _mapper.Map<CourseResponseDto>(course);
        }

        // READ
        public async Task<IEnumerable<CourseResponseDto>> GetAllAsync(GetCoursesQueryDto dto)
        {
            IQueryable<Course> query = _uow.Courses.Query();

            query = CourseFilter.Apply(query, dto);

            if (dto.SortBy != null)
            {
                query = _sorter.Apply(query, dto.SortBy, dto.SortAsc != false);
            }

            var courses = await query.ToListAsync();
            return _mapper.Map<IEnumerable<CourseResponseDto>>(courses);
        }

        public async Task<CourseResponseDto?> GetByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("Invalid Course id");
            }

            var course = await _uow.Courses.FindSingleAsync(c => c.Id == guid);

            return course == null ? null : _mapper.Map<CourseResponseDto>(course);
        }

        // UPDATE
        public async Task<CourseResponseDto> Publish(string id)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("Invalid Course id");
            }

            var course = await _uow.Courses.FindSingleAsync(c => c.Id == guid);

            if (course == null) throw new KeyNotFoundException("Course not found.");

            course.Status = CourseStatus.Published;
            await _uow.CompleteAsync();

            return _mapper.Map<CourseResponseDto>(course);
        }

        public async Task<CourseResponseDto> Update(string id, string tutorId, UpdateCourseDto dto)
        {
            // Find Course
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("Invalid Course id");
            }

            var course = await _uow.Courses.FindSingleAsync(c => c.Id == guid);

            if (course == null) throw new KeyNotFoundException("Course not found.");

            // ensure it is an assigned tutor
            bool isAssignedTutor = course.TutorProfiles.Any(tp => tp.UserId == tutorId);
            if (!isAssignedTutor) throw new UnauthorizedAccessException("Forbidden");


            switch (course.Status)
            {
                // Full Update 
                case CourseStatus.Draft:
                    course.Title = string.IsNullOrEmpty(dto.Title) ? course.Title : dto.Title;
                    course.Description = string.IsNullOrEmpty(dto.Description) ? course.Description : dto.Description;
                    course.MaxCapacity = dto.MaxCapacity == null ? course.MaxCapacity : dto.MaxCapacity;
                    
                    if (!string.IsNullOrEmpty(dto.CategoryId))
                        await FindAndAssignCategory(dto.CategoryId, course);

                    if (dto.TagIds != null)
                    {
                        course.Tags = new List<Tag>();
                        await FindAndAssignTags(dto.TagIds, course);
                    }

                    if (dto.Schedule != null)
                        await CreateAndAssignSchedule(dto.Schedule, course);

                    if (dto.PrerequisiteIds != null)
                    {
                        foreach (Prerequisite prerequisite in course.Prerequisites)
                        {
                            _uow.Prerequisites.Remove(prerequisite);
                        }
                        course.Prerequisites = new List<Prerequisite>();
                        await FindAndAssignPrerequisites(dto.PrerequisiteIds, course);
                    }

                    break;

                // Partial Update
                case CourseStatus.Published:
                    course.Description = string.IsNullOrEmpty(dto.Description) ? course.Description : dto.Description;

                    if (dto.MaxCapacity != null)
                    {
                        int enrollmentCount = course.Enrollments.Where(e => e.Status == EnrollmentStatus.Active).Count();
                        if (dto.MaxCapacity < enrollmentCount) 
                            throw new Exception($"New max capacity ({dto.MaxCapacity}) cannot be less than the current number of active enrollments ({enrollmentCount}).");
                    }

                    if (dto.TagIds != null)
                    {
                        course.Tags = new List<Tag>();
                        await FindAndAssignTags(dto.TagIds, course);
                    }

                    break;

                // No Update 
                case CourseStatus.Archived:
                    throw new Exception("Cannot update an Archived course.");

                default:
                    break;
            }

            try
            {
                await _uow.CompleteAsync();
            }
            catch (DbUpdateException ex)
            {
                // Check if it's a unique constraint violation
                if (ex.InnerException != null)
                {
                    if (ex.InnerException is PostgresException pgEx)
                    {
                        switch (pgEx.SqlState)
                        {
                            case "23505": // Unique constraint violation
                                throw new InvalidOperationException("A course with this title already exists.");
                            case "23503": // Foreign key constraint violation (in case category doesn't exist)
                                throw new InvalidOperationException("The specified category does not exist.");
                            default:
                                throw; // Re-throw other PostgreSQL exceptions
                        }
                    }
                }
                // Re-throw if it's not a PostgreSQL exception
                throw;
            }
            return _mapper.Map<CourseResponseDto>(course);
        }

        // DELETE
        public async Task<CourseResponseDto> Archive(string id, string? tutorId)
        {
           
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("Invalid Course id");
            }

            var course = await _uow.Courses.FindSingleAsync(c => c.Id == guid);

            if (course == null) throw new KeyNotFoundException("Course not found.");

            // if tutorId is passed to the function ensure it is an assigned tutor
            if (!string.IsNullOrEmpty(tutorId))
            {
                bool isAssignedTutor = course.TutorProfiles.Any(tp => tp.UserId == tutorId);
                if (!isAssignedTutor) throw new UnauthorizedAccessException("Forbidden");
            }

            course.Status = CourseStatus.Archived;
            await _uow.CompleteAsync();

            return _mapper.Map<CourseResponseDto>(course);
        }

        // TUTORS
        public async Task<CourseResponseDto> AssignTutor(string id, string tutorId, string requesterId)
        {
            // Find course
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("Invalid Course id");
            }

            var course = await _uow.Courses.FindSingleAsync(c => c.Id == guid);

            if (course == null)
                throw new KeyNotFoundException("Course not found");

            // make sure requester is an assigned tutor
            if (!course.TutorProfiles.Any(tp => tp.User.Id == requesterId))
                throw new UnauthorizedAccessException("Forbidden");

            // unassign tutor
            await FindAndAssignTutor(tutorId, course);
            await _uow.CompleteAsync();
            return _mapper.Map<CourseResponseDto>(course);
        }

        public async Task<CourseResponseDto> UnassignTutor(string id, string tutorId, string requesterId)
        {
            // Find course
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("Invalid Course id");
            }

            var course = await _uow.Courses.FindSingleAsync(c => c.Id == guid);

            if (course == null)
                throw new KeyNotFoundException("Course not found");

            // make sure requester is an assigned tutor
            if (!course.TutorProfiles.Any(tp => tp.User.Id == requesterId))
                throw new UnauthorizedAccessException("Forbidden");

            // unassign tutor
            var toBeRemovedTutor = course.TutorProfiles.FirstOrDefault(tp => tp.User.Id == tutorId);

            if (toBeRemovedTutor == null)
                throw new KeyNotFoundException("Tutor Not Found");

            if (course.TutorProfiles.Count() == 1)
                throw new Exception("Cannot Unassign the only tutor. A course must always have at least one assigned tutor.");

            course.TutorProfiles.Remove(toBeRemovedTutor);

            await _uow.CompleteAsync();

            return _mapper.Map<CourseResponseDto>(course);
        }

        // -------------- HELPERS ------------------
        internal class CourseFilter
        {
            public static IQueryable<Course> Apply(IQueryable<Course> query, GetCoursesQueryDto dto)
            {
                if (!string.IsNullOrWhiteSpace(dto.Title))
                    query = query.Where(c => c.Title != null &&
                                             c.Title.ToLower().Contains(dto.Title.ToLower()));

                if (dto.Status != null)
                    query = query.Where(c => c.Status == dto.Status);

                if (!string.IsNullOrWhiteSpace(dto.CategoryId))
                    if (Guid.TryParse(dto.CategoryId, out Guid categoryGuid))
                        query = query.Where(c => c.CategoryId == categoryGuid);

                if (!string.IsNullOrWhiteSpace(dto.TutorProfileId))
                    if (Guid.TryParse(dto.TutorProfileId, out Guid tutorProfileGuid))
                        query = query.Where(c => c.TutorProfiles.Any(tp => tp.Id == tutorProfileGuid));

                if (dto.TagIds != null && dto.TagIds.Any())
                {
                    var tagGuids = dto.TagIds.Where(tagId => Guid.TryParse(tagId, out _))
                                          .Select(Guid.Parse)
                                          .ToList();

                    if (tagGuids.Any())
                    {
                        query = query.Where(c => tagGuids.All(tagId => c.Tags.Any(t => t.Id == tagId)));
                    }
                }

                return query;
            }
        }

        private async Task FindAndAssignTutor(string tutorId, Course course)
        {
            var tutor = await _uow.Users.FindSingleWithProfileAsync(u => u.Id == tutorId);

            if (tutor == null || tutor.TutorProfile == null)
            {
                throw new Exception("Tutor Profile not found");
            }

            if (course.TutorProfiles.Any(tp => tp.Id == tutor.TutorProfile.Id))
                throw new Exception("Tutor already assigned to this course");

            course.TutorProfiles.Add(tutor.TutorProfile);
        }

        private async Task FindAndAssignCategory(string categoryId, Course course)
        {
            // Find and Assign category
            if (!Guid.TryParse(categoryId, out Guid categoryGuid))
            {
                throw new Exception("Invalid category id");
            }

            var category = await _uow.Category.FindSingleAsync(c => c.Id == categoryGuid);

            if (category == null)
            {
                throw new Exception("Category not found");
            }

            course.Category = category;
        }

        private async Task FindAndAssignTags(string[] tagIds, Course course)
        {
            foreach (string tagId in tagIds.Distinct())
            {
                if (Guid.TryParse(tagId, out Guid tagGuid))
                {
                    var tag = await _uow.Tag.FindSingleAsync(t => t.Id == tagGuid);

                    if (tag != null)
                    {
                        course.Tags.Add(tag);
                    }
                }
            }
        }

        private async Task FindAndAssignPrerequisites(string[] prerequisiteIds, Course course)
        {
            foreach (string prerequisiteId in prerequisiteIds.Distinct())
            {
                if (Guid.TryParse(prerequisiteId, out Guid prerequisiteGuid))
                {
                    var preCourse = await _uow.Courses.FindSingleAsync(c => c.Id == prerequisiteGuid);
                    if (preCourse != null)
                    {
                        // Check for circular dependency
                        if (course.Id == prerequisiteGuid || await WouldCreateCircularDependency(course.Id, prerequisiteGuid))
                        {
                            continue; // Skip this prerequisite to avoid cycle
                        }

                        var prerequisite = new Prerequisite
                        {
                            PrerequisiteCourse = preCourse,
                            TargetCourse = course,
                        };
                        
                        await _uow.Prerequisites.AddAsync(prerequisite);
                    }
                }
            }

        }

        private async Task<bool> WouldCreateCircularDependency(Guid targetCourseId, Guid prerequisiteCourseId)
        {
            // If prerequisite course has target course as a prerequisite (directly or indirectly)
            return await HasPrerequisite(prerequisiteCourseId, targetCourseId, new HashSet<Guid>());
        }

        private async Task<bool> HasPrerequisite(Guid courseId, Guid searchForId, HashSet<Guid> visited)
        {
            // Prevent infinite loops in case of existing cycles
            if (visited.Contains(courseId))
                return false;

            visited.Add(courseId);

            // Get all prerequisites for this course
            var prerequisites = await _uow.Prerequisites
                .FindAsync(p => p.TargetCourse.Id == courseId);

            foreach (var prerequisite in prerequisites)
            {
                var prereqCourseId = prerequisite.PrerequisiteCourse.Id;

                // Direct match - would create cycle
                if (prereqCourseId == searchForId)
                    return true;

                // Check indirect prerequisites recursively
                if (await HasPrerequisite(prereqCourseId, searchForId, visited))
                    return true;
            }

            return false;
        }


        private async Task CreateAndAssignSchedule(ScheduleDto scheduleDto, Course course)
        {
            var schedule = new Schedule
            {
                StartDate = scheduleDto.StartDate,
                EndDate = scheduleDto.EndDate
            };

            foreach (ScheduleSessionDto sessionDto in scheduleDto.Sessions)
            {
                var session = new ScheduleSession
                {
                    DayOfWeek = (DayOfWeekEnum)sessionDto.DayOfWeek,
                    StartTime = TimeSpan.Parse(sessionDto.StartTime),
                    EndTime = TimeSpan.Parse(sessionDto.EndTime),
                    Location = sessionDto.Location,
                };

                await _uow.ScheduleSessions.AddAsync(session);
                schedule.Sessions.Add(session);
            }

            await _uow.Schedules.AddAsync(schedule);
            course.Schedule = schedule;
        }

        
    }

}
