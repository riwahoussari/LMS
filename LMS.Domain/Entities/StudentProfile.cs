using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Entities
{
    public class StudentProfile
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } // FK (AppUser)
        public AppUser User { get; set; }

        public string? Major { get; set; }

        // one-to-many relashionship with enrollments
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
