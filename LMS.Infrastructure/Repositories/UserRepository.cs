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

namespace LMS.Infrastructure.Repositories
{
    public class UserRepository : Repository<AppUser>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }


        public async Task<AppUser?> GetByIdWithProfileAsync(string id)
        {
            return await _context.Users
                .Include(u => u.TutorProfile)
                .Include(u => u.StudentProfile)
                .FirstOrDefaultAsync( u => u.Id == id);
        }

        public async Task<IEnumerable<AppUser>> GetAllWithProfilesAsync()
        {
            return await _context.Users
                .Include(u => u.TutorProfile)
                .Include(u => u.StudentProfile)
                .ToListAsync();
        }

        public async Task<IEnumerable<AppUser>> FindWithProfilesAsync(Expression<Func<AppUser, bool>> predicate) 
        {
            return await _context.Users
                .Include(u => u.TutorProfile)
                .Include(u => u.StudentProfile)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<AppUser?> FindFirstWithProfileAsync(Expression<Func<AppUser, bool>> predicate) 
        {
            return await _context.Users
                .Include(u => u.TutorProfile)
                .Include(u => u.StudentProfile)
                .Where(predicate)
                .FirstOrDefaultAsync();
        }

        public async Task<AppUser?> FindSingleWithProfileAsync(Expression<Func<AppUser, bool>> predicate)
        {
            return await _context.Users
                .Include(u => u.TutorProfile)
                .Include(u => u.StudentProfile)
                .Where(predicate)
                .SingleOrDefaultAsync();
        }

    }
}
