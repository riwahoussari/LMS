using LMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.DTOs
{
    // Create
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

    // Read
    public class GetCoursesQueryDto
    {
        public string? Title { get; set; }
        public CourseStatus? Status { get; set; }
        public string? CategoryId { get; set; }
        public string? TutorProfileId { get; set; }
        public string[]? TagIds { get; set; }
        public string? SortBy { get; set; }
        public bool? SortAsc { get; set; }
        public int? Limit { get; set; }
        public int? Offset { get; set; }
    }

    // Update
    public class UpdateCourseDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? MaxCapacity { get; set; }
        public string? CategoryId { get; set; }
        public ScheduleDto? Schedule { get; set; }
        public string[]? TagIds { get; set; } 
        public string[]? PrerequisiteIds { get; set; }
    }


    // Responses
    public class CourseResponseDto
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int MaxCapacity { get; set; }
        public int SpotsLeft { get; set; }
        public bool isUserEnrolled { get; set; }
        public CourseStatus Status { get; set; }
        public TutorProfileExtendedResponseDto[] TutorProfiles { get; set; } = null!;
        public CategoryResponseDto Category { get; set; } = null!;
        public ScheduleResponseDto Schedule { get; set; } = null!;
        public TagResponseDto[] Tags { get; set; } = null!;
        public PartialCourseResponseDto[] Prerequisites { get; set; } = null!;
        public string CreatedAt { get; set; } = null!;
    }

    public class PartialCourseResponseDto
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
    }

    


}
