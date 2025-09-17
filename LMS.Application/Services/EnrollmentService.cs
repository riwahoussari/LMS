using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        public Task<Enrollment> EnrollAsync(Guid studentProfileId, Guid courseId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentAsync(Guid studentProfileId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateEnrollmentStatusAsync(Guid studentProfileId, Guid courseId, EnrollmentStatus status)
        {
            throw new NotImplementedException();
        }
    }

}
