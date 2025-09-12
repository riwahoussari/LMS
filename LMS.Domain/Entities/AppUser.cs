using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Entities
{
    // Single-role enforcement: user has only one Role
    public class AppUser : IdentityUser
    {
        public string RoleId { get; set; } = string.Empty;         // FK to Role
        public IdentityRole Role { get; set; } = null!;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // Add profile navigation if needed
        //public TutorProfile? TutorProfile { get; set; }
        //public StudentProfile? StudentProfile { get; set; }
    }

}
