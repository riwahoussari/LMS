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

        // Custom for Users
        public async Task<AppUser?> GetByIdWithProfileAsync(string id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.TutorProfile)
                .Include(u => u.StudentProfile)
                .FirstOrDefaultAsync( u => u.Id == id);
        }

        public async Task<IEnumerable<AppUser>> GetAllWithProfilesAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.TutorProfile)
                .Include(u => u.StudentProfile)
                .ToListAsync();
        }

        public async Task<IEnumerable<AppUser>> FindWithProfilesAsync(Expression<Func<AppUser, bool>> predicate) 
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.TutorProfile)
                .Include(u => u.StudentProfile)
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<AppUser?> FindFirstWithProfileAsync(Expression<Func<AppUser, bool>> predicate) 
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.TutorProfile)
                .Include(u => u.StudentProfile)
                .Where(predicate)
                .FirstOrDefaultAsync();
        }

        public async Task<AppUser?> FindSingleWithProfileAsync(Expression<Func<AppUser, bool>> predicate)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.TutorProfile)
                .Include(u => u.StudentProfile)
                .Where(predicate)
                .SingleOrDefaultAsync();
        }


        // Override default Repository methods to include Role for all Users
        public override async Task<AppUser?> GetByIdAsync(string id) => await _context.Users.Include(u => u.Role).SingleOrDefaultAsync(u => u.Id == id);

        public override async Task<IEnumerable<AppUser>> GetAllAsync() => await _context.Users.Include(u => u.Role).ToListAsync();

        public override async Task<IEnumerable<AppUser>> FindAsync(Expression<Func<AppUser, bool>> predicate) =>
            await _context.Users.Include(u => u.Role).Where(predicate).ToListAsync();

        public override async Task<AppUser?> FindFirstAsync(Expression<Func<AppUser, bool>> predicate) =>
            await _context.Users.Include(u => u.Role).Where(predicate).FirstOrDefaultAsync();

        public override async Task<AppUser?> FindSingleAsync(Expression<Func<AppUser, bool>> predicate) =>
            await _context.Users.Include(u => u.Role).Where(predicate).SingleOrDefaultAsync();
    }
}
