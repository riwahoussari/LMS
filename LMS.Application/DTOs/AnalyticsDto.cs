using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.DTOs
{
    // User Analytics
    public class UserGrowthPoint { 
        public DateTime Date { get; set; } 
        public int Count { get; set; } 
    }
    public class UserAnalyticsDto
    {
        public int TotalUsers { get; set; }
        public int TotalAdmins { get; set; }
        public int TotalTutors { get; set; }
        public int TotalStudents { get; set; }
        public IEnumerable<UserGrowthPoint> GrowthLast30Days { get; set; } = new List<UserGrowthPoint>();
    }

    // Course Analytics
    public class CourseEnrollmentPoint { 
        public Guid CourseId { get; set; } 
        public string Title { get; set; } = string.Empty; 
        public int Enrollments { get; set; } 
    }
    public class CourseAnalyticsDto
    {
        public int TotalCourses { get; set; }
        public int Published { get; set; }
        public int Draft { get; set; }
        public int Archived { get; set; }
        public double AvgCapacityUtilizationPct { get; set; } // 0..100
        public double GlobalCompletionRatePct { get; set; } // 0..100
        public IEnumerable<CourseEnrollmentPoint> TopCoursesByEnrollment { get; set; } = new List<CourseEnrollmentPoint>();
    }

    // TutorAnalytics
    public class TutorWorkloadItem { 
        public string TutorId { get; set; } = string.Empty; 
        public string TutorName { get; set; } = string.Empty; 
        public int CoursesCount { get; set; } 
        public int TotalEnrollments { get; set; } 
    }
    public class TutorAnalyticsDto
    {
        public int TotalTutors { get; set; }
        public IEnumerable<TutorWorkloadItem> TopTutorsByEnrollments { get; set; } = new List<TutorWorkloadItem>();
    }

    // Student Analytics
    public class StudentAnalyticsDto
    {
        public int TotalStudents { get; set; }
        public double AvgActiveEnrollmentsPerStudent { get; set; }
        public double DropoutRatePct { get; set; }
        public double PassRatePct { get; set; }
    }

    // All in one
    public class AnalyticsSummaryDto
    {
        public UserAnalyticsDto Users { get; set; } = new();
        public CourseAnalyticsDto Courses { get; set; } = new();
        public TutorAnalyticsDto Tutors { get; set; } = new();
        public StudentAnalyticsDto Students { get; set; } = new();
    }
}
