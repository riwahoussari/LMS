using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Entities
{
    public class Schedule
    {
        public Guid Id { get; set; }

        public required string StartDate { get; set; }
        public required string EndDate { get; set; }

        // one-to-many relashionship to Courses
        public ICollection<Course> Courses { get; set; } = new List<Course>();

        // one-to-many relashionship to Schedule Sessions
        public ICollection<ScheduleSession> Sessions { get; set; } = new List<ScheduleSession>();
    }
}
