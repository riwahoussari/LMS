using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Application.Services;
using LMS.Application.Validators;
using LMS.Domain.Enums;
using LMS.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IUserService _userService;

        public CoursesController(ICourseService courseService, IEnrollmentService enrollmentService, IUserService userService)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
            _userService = userService;
        }

        // CREATE
        /// <summary>
        /// Create a new course (Tutor only).
        /// </summary>
        /// <remarks>
        /// - Requires **Tutor** role.  
        /// - The authenticated tutor is automatically assigned to the course.  
        /// - Returns the created course details.  
        /// </remarks>
        [Authorize(Roles = RoleConstants.Tutor)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request or validation errors")]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
        [SwaggerResponse(statusCode: 403, description: "User authenticated but not a Tutor")]
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
        /// <summary>
        /// Get a list of courses.
        /// </summary>
        /// <remarks>
        /// - Requires authentication.  
        /// - **Admins** can query all courses with any status.  
        /// - **Non-admins** can only view courses with status `Published`.  
        /// - Use query parameters to filter, sort and paginate results.  
        /// </remarks>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<CourseResponseDto>))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request or validation errors")]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
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
            var (courses, total) = await _courseService.GetAllAsync(query);

            // mark user enrollment
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var enrollments = await _enrollmentService.GetMyEnrollmentsAsync(currentUserId, null);
                if (enrollments == null || enrollments.Count() == 0)
                {
                    foreach (CourseResponseDto course in courses) course.isUserEnrolled = false;
                }
                else
                {
                    var enrollmentsList = enrollments.ToList();
                    foreach (CourseResponseDto course in courses)
                    {
                        course.isUserEnrolled = false;

                        var enrollment = enrollmentsList.FirstOrDefault(e => e.Course.Id == course.Id);
                        if (enrollment != null)
                        {
                            enrollmentsList.Remove(enrollment);
                            course.isUserEnrolled = true;
                        }
                    }
                }
            }
            catch { foreach (CourseResponseDto course in courses) course.isUserEnrolled = false; }


            var result = new PagedResult<CourseResponseDto>
            {
                Items = courses,
                Total = total,
                Limit = query.Limit,
                Offset = query.Offset
            };

            return Ok(result);
        }


        /// <summary>
        /// Get a course by ID.
        /// </summary>
        /// <remarks>
        /// - Requires authentication.  
        /// - **Published** courses are visible to all authenticated users.  
        /// - **Non-published** courses are visible only to **Admins** and **Assigned Tutors**.  
        /// - Returns course details if accessible.  
        /// </remarks>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request or validation errors")]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
        [SwaggerResponse(statusCode: 404, description: "Course not found (can also mean that a user is unauthorized to access the specified course)")]
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
        /// <summary>
        /// Publish a course (Admin only).
        /// </summary>
        /// <remarks>
        /// - Requires **Admin** role.  
        /// - Sets the course status to `Published`.  
        /// - Returns the updated course.  
        /// </remarks>
        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPatch("{id}/publish")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request or validation errors")]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
        [SwaggerResponse(statusCode: 403, description: "User authenticated but not an Admin")]
        [SwaggerResponse(statusCode: 404, description: "Course not found")]
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

        /// <summary>
        /// Update a course (assigned Tutor only).
        /// </summary>
        /// <remarks>
        /// - Requires **Tutor** role.  
        /// - Tutors can only update courses they are assigned to.  
        /// - Returns the updated course details.  
        /// </remarks>
        [Authorize(Roles = RoleConstants.Tutor)]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request or validation errors")]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
        [SwaggerResponse(statusCode: 403, description: "User authenticated but not an assigned Tutor to this course")]
        [SwaggerResponse(statusCode: 404, description: "Course not found")]
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
        /// <summary>
        /// Archive (soft-delete) a course (Admin/assigned Tutor).
        /// </summary>
        /// <remarks>
        /// - Requires **Admin** or **Tutor** role.  
        /// - **Admins** can archive any course.  
        /// - **Tutors** can only archive courses they are assigned to.  
        /// - Returns the archived course details.  
        /// </remarks>
        [Authorize(Roles = RoleConstants.Admin + " , " + RoleConstants.Tutor)]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request or validation errors")]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
        [SwaggerResponse(statusCode: 404, description: "Course not found")]
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
                return BadRequest(ex.Message);
            }
        }

        
        
    }
}
