using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Domain.Enums;

namespace LMS.Domain.Entities
{
    public class Enrollment
    {
        // composite PK: (student_profile_id, course_id) — configured in DbContext
        public Guid CourseId { get; set; }
        public Course Course { get; set; }

        public Guid StudentId { get; set; }
        public StudentProfile Student { get; set; }

        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
    }
}
