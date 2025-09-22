using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Application.Services;
using LMS.Application.Validators;
using LMS.Domain.Enums;
using LMS.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService, IEnrollmentService enrollmentService)
        {
            _courseService = courseService;
        }

        // CREATE
        [Authorize(Roles = RoleConstants.Tutor)]
        [HttpPost]
        public async Task<IActionResult> CreateCourse(CreateCourseDto dto)
        {
            // get user id
            var tutorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (tutorId == null) return BadRequest();

            // validate data
            var errors = await ValidateAsync<CreateCourseDto>(dto, typeof(CreateCourseDtoValidator));
            if (errors.Any())
            {
                return BadRequest(new { Errors = errors});
            }

            // create course
            try
            {
                var course = await _courseService.CreateAsync(tutorId, dto);
                return Ok(course);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        // READ
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCourses([FromQuery] GetCoursesQueryDto query)
        {
            // data validation
            var errors = await ValidateAsync<GetCoursesQueryDto>(query,
                typeof(GetCoursesQueryDtoValidator));

            if (errors.Any())
                return BadRequest(new { Errors = errors });

            // if not admin only => see published courses
            if (!User.IsInRole(RoleConstants.Admin))
            {
                // if query filter is empty => set it to published
                if (query.Status == null)
                    query.Status = CourseStatus.Published;

                // if query filter is set to a unallowed value => return bad request
                else if (query.Status != CourseStatus.Published)
                    return BadRequest("You can only see Published courses. Leave status feild empty or set it to 'Published'");
            }

            // get courses
            var courses = await _courseService.GetAllAsync(query);

            return Ok(courses);
        }


        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(string id)
        {
            CourseResponseDto? course;
            try
            {
                course = await _courseService.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            if (course == null) return NotFound("Course not found.");

            // only admins and assigned tutors can see a non-published course
            if (course.Status != CourseStatus.Published)
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                bool isAdmin = User.IsInRole(RoleConstants.Admin);
                bool isAssignedTutor = course.TutorProfiles.Any(tp => tp.UserId == currentUserId);

                if (!isAdmin && !isAssignedTutor)
                {
                    return NotFound("Course not found"); // generic message to avoid leaking info
                }
            }

            return Ok(course);
        }


        // UPDATE
        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPatch("{id}/publish")]
        public async Task<IActionResult> PublishCourse(string id)
        {
            try
            {
                var course = await _courseService.Publish(id);
                return Ok(course);
            }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException) return NotFound(ex.Message);
                return BadRequest(ex.Message);
            }

        }

        [Authorize(Roles = RoleConstants.Tutor)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(string id, UpdateCourseDto dto)
        {
            // data validation
            var errors = await ValidateAsync<UpdateCourseDto>(dto,
                typeof(UpdateCourseDtoValidator));

            if (errors.Any())
                return BadRequest(new { Errors = errors });

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null) return BadRequest("Unable to identify the current user");

            try
            {
                var course = await _courseService.Update(id: id, tutorId: currentUserId, dto: dto);
                return Ok(course);
            }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException) return NotFound(ex.Message);
                if (ex is UnauthorizedAccessException) return Forbid();
                return BadRequest(ex.Message);
            }
        }

        // DELETE
        [Authorize(Roles = RoleConstants.Admin + " , " + RoleConstants.Tutor)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> ArchiveCourse(string id)
        {
            var currentUserId = User.IsInRole(RoleConstants.Tutor) ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;

            try
            { 
                var course = await _courseService.Archive(id, User.IsInRole(RoleConstants.Tutor) ? currentUserId : null);
                return Ok(course);
            }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException) return NotFound(ex.Message);
                //if (ex is UnauthorizedAccessException) return Forbid();
                return BadRequest(ex.Message);
            }
        }

        
        
    }
}
