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
    public class StudentProfileRepository : Repository<StudentProfile>, IStudentProfileRepository
    {
        public StudentProfileRepository(AppDbContext context) : base(context)
        {
        }
    }
}
