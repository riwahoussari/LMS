using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.DTOs
{
    public class ScheduleDto
    {
        public string StartDate { get; set; } = null!;
        public string EndDate { get; set; } = null!;
        public ScheduleSessionDto[] Sessions { get; set; } = null!;
    }

    public class ScheduleSessionDto
    {
        public int DayOfWeek { get; set; }
        public string StartTime { get; set; } = null!;
        public string EndTime { get; set; } = null!;
        public string Location { get; set; } = null!;
    }

    // RESPONSE
    public class ScheduleResponseDto
    {
        public string StartDate { get; set; } = null!;
        public string EndDate { get; set; } = null!;
        public ScheduleSessionResponseDto[] Sessions { get; set; } = null!;
    }

    public class ScheduleSessionResponseDto
    {
        public string DayOfWeek { get; set; } = null!;
        public string StartTime { get; set; } = null!;
        public string EndTime { get; set; } = null!;
        public string Location { get; set; } = null!;
    }
}
