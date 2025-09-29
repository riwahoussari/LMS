using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static FluentValidation.Validators.PredicateValidator;

namespace LMS.Infrastructure.Repositories
{
    public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
    {
        public EnrollmentRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task<int> CountAsync(Expression<Func<Enrollment, bool>> predicate) => await _dbSet.Include(e => e.Course).CountAsync(predicate);
        public override async Task<IEnumerable<Enrollment>> GetAllAsync()
        {
            return await _dbSet
                .Include(e => e.Course)
                .Include(e => e.Student)
                    .ThenInclude(sp => sp.User)
                .ToListAsync();
        }

        public override async Task<Enrollment?> FindFirstAsync(Expression<Func<Enrollment, bool>> predicate)
        {
            return await _dbSet
                .Include(e => e.Course)
                .Include(e => e.Student)
                    .ThenInclude(sp => sp.User)
                .Where(predicate).FirstOrDefaultAsync();
        }

        public override async Task<IEnumerable<Enrollment>> FindAsync(Expression<Func<Enrollment, bool>> predicate)
        {
            return await _dbSet
                .Include(e => e.Course)
                .Include(e => e.Student)
                    .ThenInclude(sp => sp.User)
                .Where(predicate)
                .ToListAsync();
        }

        public override async Task<Enrollment?> FindSingleAsync(Expression<Func<Enrollment, bool>> predicate)
        {
            return await _dbSet
                .Include(e => e.Course)
                .Include(e => e.Student)
                    .ThenInclude(sp => sp.User)
                .Where(predicate).SingleOrDefaultAsync();
        }

    }
}
