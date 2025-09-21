using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Application.Validators;
using LMS.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
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
                var course = await _courseService.CreateCourseAsync(tutorId, dto);
                return Ok(course);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        // READ
        [HttpGet]
        public async Task<IActionResult> GetCourses()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(string id)
        {
            return Ok();
        }

        // UPDATE
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateCourse(string id)
        {
            return Ok();
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> ArchiveCourse(string id)
        {
            return Ok();
        }
    }
}
