using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Domain.Enums;

namespace LMS.Domain.Entities
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? MaxCapacity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public CourseStatus Status { get; set; } = CourseStatus.Draft;


        public Guid CategoryId { get; set; } // FK (Category)
        public Category Category { get; set; }


        public Guid ScheduleId { get; set; } // FK (Schedule)
        public Schedule Schedule { get; set; }


        // many-to-many relashionship with Tags
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();


        // many-to-many relashionship with Tutors
        public ICollection<TutorProfile> TutorProfiles { get; set; } = new List<TutorProfile>();


        // prerequisites (many-to-two)
        public ICollection<Prerequisite> Prerequisites { get; set; } = new List<Prerequisite>(); // this course is the target course
        public ICollection<Prerequisite> PrerequisiteFor { get; set; } = new List<Prerequisite>(); // this course is the prerequisite course


        // one-to-many relashionship with enrollments
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    }


    public class Prerequisite
    {
        // join table prerequisites 
        // PK (target_course_id, prerequisite_course_id) Configured in AppDbContext
        public Guid TargetCourseId { get; set; }
        public Course TargetCourse { get; set; } = null!;

        public Guid PrerequisiteCourseId { get; set; }
        public Course PrerequisiteCourse { get; set; } = null!;
    }

}
