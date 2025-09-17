using LMS.Domain.Entities;
using LMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Interfaces
{
    public interface IEnrollmentService
    {
        Task<Enrollment> EnrollAsync(Guid studentProfileId, Guid courseId);
        Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentAsync(Guid studentProfileId);
        Task UpdateEnrollmentStatusAsync(Guid studentProfileId, Guid courseId, EnrollmentStatus status);
    }
}
