using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.DTOs
{
    public class CreateCourseDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int MaxCapacity { get; set; }
        public string CategoryId { get; set; } = null!;
        public ScheduleDto Schedule { get; set; } = null!;
        public string[] TagIds { get; set; } = null!;
        public string[] PrerequisiteIds { get; set; } = null!;
    }

    public class ScheduleDto
    {
        public string StartDate { get; set; } = null!;
        public string EndDate { get; set; } = null!;
        public ScheduleSessionDto[] Sessions { get; set; } = null!;
    }

    public class ScheduleSessionDto
    {
        public int DayOfWeek { get; set; }
        public string StartTime { get; set; } = null!;
        public string EndTime { get; set; } = null!;
        public string Location { get; set; } = null!;
    }

    // Responses
    public class CourseResponseDto
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int MaxCapacity { get; set; }
        public string Status { get; set; } = null!;
        public TutorProfileResponseDto[] TutorProfiles { get; set; } = null!;
        public CategoryResponseDto Category { get; set; } = null!;
        public ScheduleResponseDto Schedule { get; set; } = null!;
        public TagResponseDto[] Tags { get; set; } = null!;
        public PrerequisiteCourseResponseDto[] Prerequisites { get; set; } = null!;
    }

    public class PrerequisiteCourseResponseDto
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
    }

    public class ScheduleResponseDto
    {
        public string StartDate { get; set; } = null!;
        public string EndDate { get; set; } = null!;
        public ScheduleSessionResponseDto[] Sessions { get; set; } = null!;
    }

    public class ScheduleSessionResponseDto
    {
        public string DayOfWeek { get; set; } = null!;
        public string StartTime { get; set; } = null!;
        public string EndTime { get; set; } = null!;
        public string Location { get; set; } = null!;
    }


}
