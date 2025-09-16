using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Domain.Enums;

namespace LMS.Domain.Entities
{
    public class ScheduleSession
    {
        public Guid Id { get; set; }
        public Guid ScheduleId { get; set; } // FK (Schedule)
        public Schedule Schedule { get; set; }

        public DayOfWeekEnum DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public string? Location { get; set; }
    }
}
