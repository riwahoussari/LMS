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
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context)
        {
        }

        // custom
        public void RevokeToken(RefreshToken? token)
        {
            if (token != null && token.IsActive)
            {
                token.RevokedAt = DateTime.UtcNow;
            }
        }

        public async Task<RefreshToken?> FindFirstWithUserAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(r => r.User)
                .Include(r => r.User.Role)
                .FirstOrDefaultAsync(r => r.Token == token);
        }
    }
}
