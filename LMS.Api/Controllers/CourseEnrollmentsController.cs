using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Enums;
using LMS.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        [Authorize(Roles = RoleConstants.Admin + " , " + RoleConstants.Tutor)]
        [HttpGet("{id}/enrollments")]
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
        [Authorize(Roles = RoleConstants.Student)]
        [HttpPost("{id}/enrollments")]
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
        [Authorize]
        [HttpGet("{courseId}/enrollements/{studentProfileId}")]
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
        [Authorize(Roles = RoleConstants.Student + " , " + RoleConstants.Tutor)]
        [HttpPatch("{courseId}/enrollements/{studentProfileId}")]
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
