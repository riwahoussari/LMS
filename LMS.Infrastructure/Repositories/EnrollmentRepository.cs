using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Repositories
{
    public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
    {
        public EnrollmentRepository(AppDbContext context) : base(context)
        {
        }

        public Task<IEnumerable<Enrollment>> GetByCourseAsync(Guid courseId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Enrollment>> GetByStatusAsync(string status)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Enrollment>> GetByStudentAsync(Guid studentId)
        {
            throw new NotImplementedException();
        }
    }
}
