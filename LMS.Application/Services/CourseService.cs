using AutoMapper;
using LMS.Application.DTOs;
using LMS.Application.Interfaces;
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

        public CourseService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<CourseResponseDto> CreateCourseAsync(string tutorId, CreateCourseDto dto)
        {
            var course = new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                MaxCapacity = dto.MaxCapacity,
            };

            // Find and Assign Tutor
            var tutor = await _uow.Users.FindSingleWithProfileAsync(u => u.Id == tutorId);

            if (tutor == null || tutor.TutorProfile == null)
            {
                throw new Exception("Tutor Profile not found");
            }

            course.TutorProfiles.Add(tutor.TutorProfile);

            // Find and Assign category
            if (!Guid.TryParse(dto.CategoryId, out Guid categoryGuid))
            {
                throw new Exception("Invalid category id");
            }

            var category = await _uow.Category.FindSingleAsync(c => c.Id == categoryGuid);

            if (category == null)
            {
                throw new Exception("Category not found");
            }

            course.Category = category;

            // Find and Assign Tags
            foreach (string tagId in dto.TagIds)
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

            // Find and Assign prerequisites
            foreach (string courseId in dto.PrerequisiteIds)
            {
                if (Guid.TryParse(courseId, out Guid courseGuid))
                {
                    var preCourse = await _uow.Courses.FindSingleAsync(c => c.Id == courseGuid);

                    if (preCourse != null)
                    {
                        var prerequisite = new Prerequisite
                        {
                            PrerequisiteCourse = preCourse,
                            TargetCourse = course,
                        };

                        await _uow.Prerequisites.AddAsync(prerequisite);

                        course.Prerequisites.Add(prerequisite);
                    }
                }

            }

            // Create and Assign Schedule
            var schedule = new Schedule
            {
                StartDate =dto.Schedule.StartDate,
                EndDate = dto.Schedule.EndDate
            };

            foreach (ScheduleSessionDto sessionDto in dto.Schedule.Sessions)
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

            // Create and save course
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
    }
}
