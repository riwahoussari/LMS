using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Interfaces
{
    public interface IEnrollmentRepository : IRepository<Enrollment>
    {
        Task<IEnumerable<Enrollment>> GetByStatusAsync(string status);
        Task<IEnumerable<Enrollment>> GetByCourseAsync(Guid courseId);
        Task<IEnumerable<Enrollment>> GetByStudentAsync(Guid studentId);
    }
}
