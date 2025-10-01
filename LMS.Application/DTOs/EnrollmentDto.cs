using LMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.DTOs
{
    public class GetEnrollmentsQueryDto
    {
        public EnrollmentStatus? EnrollmentStatus { get; set; }
        public string StudentProfileId { get; set; } = null!;
    }

    public class GetMyEnrollmentsQueryDto
    {
        public EnrollmentStatus? EnrollmentStatus { get; set; }
    }

    public class EnrollmentResponseDto
    { 
        public PartialCourseResponseDto Course { get; set; }
        public string UserId { get; set; }
        public EnrollmentStatus Status { get; set; }
    }

    public class ExtendedEnrollmentResponseDto
    {
        public PartialCourseResponseDto Course { get; set; }
        public PartialUserResponseDto User { get; set; }
        public EnrollmentStatus Status { get; set; }
        public StudentProfileResponseDto StudentProfile { get; set; }
    }


    public class UpdateEnrollmentDto
    { 
        public EnrollmentStatus? Status { get; set; }
    }

}
