using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Application.Validators;
using LMS.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace LMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentsService;


        public EnrollmentsController(IConfiguration config,
                              IEnrollmentService enrollmentService)
        {
            _enrollmentsService = enrollmentService;
        }

        /// <summary>
        /// Get all enrollments or filter by student profile (Admin only).
        /// </summary>
        /// <remarks>
        /// - Requires **Admin** role.  
        /// - If no `StudentProfileId` is provided, returns all enrollments, optionally filtered by status.   
        /// - If `StudentProfileId` is provided, returns enrollments for that student, optionally filtered by status.  
        /// - Returns 404 if student not found.  
        /// </remarks>
        [Authorize(Roles = RoleConstants.Admin)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EnrollmentResponseDto>))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request or validation errors")]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
        [SwaggerResponse(statusCode: 403, description: "User authenticated but not an Admin")]
        [SwaggerResponse(statusCode: 404, description: "Student or enrollments not found")]
        public async Task<IActionResult> GetEnrollments([FromQuery] GetEnrollmentsQueryDto query)
        {
            // data validation
            var errors = await ValidateAsync<GetEnrollmentsQueryDto>(query,
                typeof(GetEnrollmentsQueryDtoValidator));

            if (errors.Any())
                return BadRequest(new { Errors = errors });


            // get enrollments
            try
            {
                IEnumerable<EnrollmentResponseDto> enrollments;
                if (string.IsNullOrEmpty(query.StudentProfileId))
                {
                    enrollments = await _enrollmentsService.GetAllAsync();
                }
                else
                {
                    enrollments = await _enrollmentsService.GetByStudentAsync(query.StudentProfileId, query.EnrollmentStatus);
                }
                return Ok(enrollments);
            }
            catch (Exception ex)
            {
                if (ex is KeyNotFoundException) return NotFound(ex.Message);
                if (ex is UnauthorizedAccessException) return Forbid();
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Get the current student's enrollments. (Student Only)
        /// </summary>
        /// <remarks>
        /// - Requires **Student** role.  
        /// - Returns all enrollments for the authenticated student, optionally filtered by status.  
        /// - Returns 401 if user ID cannot be resolved from claims.  
        /// </remarks>
        [Authorize(Roles = RoleConstants.Student)]
        [HttpGet("my-enrollments")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EnrollmentResponseDto>))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request or validation errors")]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated or user ID not found")]
        [SwaggerResponse(statusCode: 403, description: "User authenticated but not a Student")]
        public async Task<IActionResult> GetEnrollments([FromQuery] GetMyEnrollmentsQueryDto query)
        {

            // get enrollments
            try
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (currentUserId == null) return Unauthorized("Counld't find user id");

                var enrollments = await _enrollmentsService.GetMyEnrollmentsAsync(currentUserId, query.EnrollmentStatus);
                return Ok(enrollments);
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
