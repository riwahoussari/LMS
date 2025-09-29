using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = RoleConstants.Admin)] // admin-only by default
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analytics;
        public AnalyticsController(IAnalyticsService analytics) => _analytics = analytics;

        /// <summary>
        /// Get overall platform summary.
        /// </summary>
        /// <remarks>
        /// Returns a summary of users, courses, tutors, and students counts.
        /// </remarks>
        [HttpGet("summary")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnalyticsSummaryDto))]
        [SwaggerOperation(Summary = "Get analytics summary", Description = "Returns overall platform statistics.")]
        [SwaggerResponse(200, "Success")]
        public async Task<IActionResult> GetSummary() => Ok(await _analytics.GetSummaryAsync());

        /// <summary>
        /// Get user analytics.
        /// </summary>
        /// <remarks>
        /// Provides analytics about the users (e.g., total users, active users, role distribution).
        /// </remarks>
        [HttpGet("users")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserAnalyticsDto))]
        [SwaggerOperation(Summary = "Get user analytics", Description = "Returns analytics about users.")]
        [SwaggerResponse(200, "Success")]
        public async Task<IActionResult> GetUsers() => Ok(await _analytics.GetUserAnalyticsAsync());

        /// <summary>
        /// Get course analytics.
        /// </summary>
        /// <remarks>
        /// Provides analytics about courses (e.g., published vs draft, enrollments per course).
        /// </remarks>
        [HttpGet("courses")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CourseAnalyticsDto))]
        [SwaggerOperation(Summary = "Get course analytics", Description = "Returns analytics about courses.")]
        [SwaggerResponse(200, "Success")]
        public async Task<IActionResult> GetCourses() => Ok(await _analytics.GetCourseAnalyticsAsync());

        /// <summary>
        /// Get tutor analytics.
        /// </summary>
        /// <remarks>
        /// Provides analytics about tutors (e.g., total tutors, average number of courses taught).
        /// </remarks>
        [HttpGet("tutors")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TutorAnalyticsDto))]
        [SwaggerOperation(Summary = "Get tutor analytics", Description = "Returns analytics about tutors.")]
        [SwaggerResponse(200, "Success")]
        public async Task<IActionResult> GetTutors() => Ok(await _analytics.GetTutorAnalyticsAsync());

        /// <summary>
        /// Get student analytics.
        /// </summary>
        /// <remarks>
        /// Provides analytics about students (e.g., active students, average enrollments per student).
        /// </remarks>
        [HttpGet("students")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentAnalyticsDto))]
        [SwaggerOperation(Summary = "Get student analytics", Description = "Returns analytics about students.")]
        [SwaggerResponse(200, "Success")]
        public async Task<IActionResult> GetStudents() => Ok(await _analytics.GetStudentAnalyticsAsync());

    }
}
