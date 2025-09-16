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
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Single role per user FK
        public string RoleId { get; set; } = string.Empty;        
        public IdentityRole Role { get; set; } = null!;

        // Navigation

        // Profiles
        public TutorProfile? TutorProfile { get; set; }
        public StudentProfile? StudentProfile { get; set; }

        // Notifications
        public ICollection<NotificationRecipient>? NotificationRecipients { get; set; }
    }

}
