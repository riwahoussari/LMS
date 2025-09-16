using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Link { get; set; }

        // one-to-many relashionship with Notification Recipients
        public ICollection<NotificationRecipient> Recipients { get; set; } = new List<NotificationRecipient>();
    }

    public class NotificationRecipient
    {
        // PK (notification_id, recipient_id) configure in AppDbContext
        public Guid NotificationId { get; set; }
        public Notification Notification { get; set; } = null!;

        public string RecipientId { get; set; }     // user id
        public AppUser Recipient { get; set; } = null!;

        public bool Opened { get; set; } = false;
        public DateTime? OpenedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
