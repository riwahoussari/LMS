using LMS.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Interfaces
{
    public interface IAnalyticsService
    {
        Task<UserAnalyticsDto> GetUserAnalyticsAsync();
        Task<CourseAnalyticsDto> GetCourseAnalyticsAsync();
        Task<TutorAnalyticsDto> GetTutorAnalyticsAsync();
        Task<StudentAnalyticsDto> GetStudentAnalyticsAsync();

        // whole summary
        Task<AnalyticsSummaryDto> GetSummaryAsync();
    }

}
