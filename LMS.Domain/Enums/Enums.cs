using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Enums
{
    public enum EnrollmentStatus
    {
        Pending,
        Active,
        Passed,
        Failed,
        Suspended,
        Dropped
    }

    public enum CourseStatus
    {
        Draft,
        Published,
        Archived
    }

    public enum DayOfWeekEnum
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }
}
