using LMS.Application.DTOs;
using LMS.Application.Interfaces;
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
    public class CourseTutorsController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseTutorsController(ICourseService courseService, IEnrollmentService enrollmentService)
        {
            _courseService = courseService;
        }

        // ASSIGN 
        /// <summary>
        /// Assign a tutor to a course. (Assigned Tutor only)
        /// </summary>
        /// <remarks>
        /// - Requires **Tutor** role.  
        /// - Tutors can only update courses they are assigned to.    
        /// - Adds the specified tutor to the course.  
        /// - Returns the updated course details.  
        /// </remarks>
        [Authorize(Roles = RoleConstants.Tutor)]
        [HttpPost("{id}/tutors")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request or validation errors")]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
        [SwaggerResponse(statusCode: 403, description: "User authenticated but not an assigned tutor")]
        [SwaggerResponse(statusCode: 404, description: "Course or tutor not found")]
        public async Task<IActionResult> AssignTutor(string id, [FromForm] string tutorId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var course = await _courseService.AssignTutor(id: id, tutorId: tutorId, requesterId: currentUserId);
                return Ok(course);
            }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException) return NotFound(ex.Message);
                if (ex is UnauthorizedAccessException) return Forbid();
                return BadRequest(ex.Message);
            }
        }

        // UNASSIGN
        /// <summary>
        /// Unassign a tutor from a course.(Assigned Tutor only)
        /// </summary>
        /// <remarks>
        /// - Requires **Tutor** role.  
        /// - Tutors can only update courses they are assigned to.  
        /// - Removes the specified tutor from the course.  
        /// - Returns the updated course details.  
        /// </remarks>
        [HttpDelete("{id}/tutors")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request or validation errors")]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
        [SwaggerResponse(statusCode: 403, description: "User authenticated but not an assigned tutor")]
        [SwaggerResponse(statusCode: 404, description: "Course or tutor not found")]
        public async Task<IActionResult> UnassignTutor(string id, [FromForm] string tutorId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var course = await _courseService.UnassignTutor(id: id, tutorId: tutorId, requesterId: currentUserId);
                return Ok(course);
            }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException) return NotFound(ex.Message);
                if (ex is UnauthorizedAccessException) return Forbid();
                return BadRequest(ex.Message);
            }
        }

    }
}
