using LMS.Application.Interfaces;
using LMS.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = RoleConstants.Admin)] // admin-only by default
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analytics;
        public AnalyticsController(IAnalyticsService analytics) => _analytics = analytics;

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary() => Ok(await _analytics.GetSummaryAsync());

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers() => Ok(await _analytics.GetUserAnalyticsAsync());

        [HttpGet("courses")]
        public async Task<IActionResult> GetCourses() => Ok(await _analytics.GetCourseAnalyticsAsync());

        [HttpGet("tutors")]
        public async Task<IActionResult> GetTutors() => Ok(await _analytics.GetTutorAnalyticsAsync());

        [HttpGet("students")]
        public async Task<IActionResult> GetStudents() => Ok(await _analytics.GetStudentAnalyticsAsync());

    }
}
