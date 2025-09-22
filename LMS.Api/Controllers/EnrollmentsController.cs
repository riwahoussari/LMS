using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Application.Validators;
using LMS.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpGet]
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

        [Authorize(Roles = RoleConstants.Student)]
        [HttpGet("my-enrollments")]
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
