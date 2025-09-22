using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Enums;
using LMS.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace LMS.Api.Controllers
{
    [Route("api/courses")]
    [ApiController]
    public class CourseEnrollmentsController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;

        public CourseEnrollmentsController(ICourseService courseService, IEnrollmentService enrollmentService)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
        }

        // GET ALL
        /// <summary>
        /// Get all enrollments for a course (Admin/Assigned Tutor only).
        /// </summary>
        /// <remarks>
        /// - Requires **Admin** or **Tutor** role.  
        /// - Tutors can only access enrollments for courses they are assigned to.  
        /// - Returns a list of enrollments to the specified course.  
        /// </remarks>
        [Authorize(Roles = RoleConstants.Admin + " , " + RoleConstants.Tutor)]
        [HttpGet("{id}/enrollments")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EnrollmentResponseDto>))]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
        [SwaggerResponse(statusCode: 403, description: "User authenticated but not an Admin nor Assigned Tutor")]
        [SwaggerResponse(statusCode: 404, description: "Course not found")]
        public async Task<IActionResult> GetCourseEnrollments(string id)
        {
            // Find Course
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

            // Ensure requester is a admin or an assigned tutor
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (User.IsInRole(RoleConstants.Tutor) && !course.TutorProfiles.Any(tp => tp.UserId == currentUserId))
                return Forbid();

            // Get enrollments
            var enrollments = await _enrollmentService.GetByCourseAsync(course.Id);
            return Ok(enrollments);

        }

        // POST (enroll)
        /// <summary>
        /// Enroll the current student into a course.
        /// </summary>
        /// <remarks>
        /// - Requires **Student** role.  
        /// - Enrolls the authenticated student into the specified course.  
        /// - Returns the created enrollment record.  
        /// </remarks>
        [Authorize(Roles = RoleConstants.Student)]
        [HttpPost("{id}/enrollments")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EnrollmentResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request or validation errors")]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
        [SwaggerResponse(statusCode: 403, description: "User authenticated but not a Student")]
        [SwaggerResponse(statusCode: 404, description: "Course not found")]
        public async Task<IActionResult> EnrollIntoCourse(string id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null) return Unauthorized("Unable to find your user id");

            try
            {
                var enrollment = await _enrollmentService.EnrollAsync(currentUserId, id);
                return Ok(enrollment);
            }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException) return NotFound(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        // GET ONE
        /// <summary>
        /// Get a single enrollment record for a course and student.
        /// </summary>
        /// <remarks>
        /// - Requires authentication.  
        /// - Students can only access their own enrollments.  
        /// - Tutors may access enrollments for courses they manage.  
        /// - Admins may access all enrollments
        /// - Returns the enrollment details for the given course and student.  
        /// </remarks>
        [Authorize]
        [HttpGet("{courseId}/enrollements/{studentProfileId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EnrollmentResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request or validation errors")]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
        [SwaggerResponse(statusCode: 403, description: "User authenticated but not authorized (view rules above)")]
        [SwaggerResponse(statusCode: 404, description: "Enrollment not found")]
        public async Task<IActionResult> GetEnrollement(string courseId, string studentProfileId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null) return Unauthorized("Unable to find your user id");

            try
            {
                var enrollment = await _enrollmentService.GetOneAsync(courseId, studentProfileId, requesterId: currentUserId);
                return Ok(enrollment);
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException) return Forbid();
                if (ex is KeyNotFoundException) return NotFound(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        // UPDATE
        /// <summary>
        /// Update an enrollment record (Enrolled Student/Assigned Tutor only).
        /// </summary>
        /// <remarks>
        /// - Requires **Student** or **Tutor** role.  
        /// - Students can update their own enrollment (e.g. Drop).  
        /// - Tutors can update enrollment status for students in their courses. (e.g. Pass - Fail - Suspend) 
        /// </remarks>
        [Authorize(Roles = RoleConstants.Student + " , " + RoleConstants.Tutor)]
        [HttpPatch("{courseId}/enrollements/{studentProfileId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EnrollmentResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request or validation errors")]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
        [SwaggerResponse(statusCode: 403, description: "User authenticated but not an assigned tutor nor the enrolled student")]
        [SwaggerResponse(statusCode: 404, description: "Enrollment not found")]
        public async Task<IActionResult> UpdateEnrollment(string courseId, string studentProfileId, UpdateEnrollmentDto dto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null) return Unauthorized("Unable to find your user id");

            try
            {
                if (dto.Status == null)
                    return Ok(await _enrollmentService.GetOneAsync(courseId, studentProfileId, requesterId: currentUserId));

                EnrollmentStatus status = dto.Status.Value;

                var enrollment = await _enrollmentService.UpdateAsync(courseId, studentProfileId, requesterId: currentUserId, status: status);
                return Ok(enrollment);
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException) return Forbid();
                if (ex is KeyNotFoundException) return NotFound(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
