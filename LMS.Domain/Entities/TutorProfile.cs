using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Entities
{
    public class TutorProfile
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } // FK (AppUser)
        public AppUser User { get; set; }

        public string? Bio { get; set; }
        public string? Expertise { get; set; }

        // many-to-many relashionship with Courses
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }

}
