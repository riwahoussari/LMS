using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        [HttpPost("{id}/tutors")]
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
        [HttpDelete("{id}/tutors")]
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
