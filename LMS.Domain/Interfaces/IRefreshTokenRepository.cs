using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Interfaces
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        void RevokeToken(RefreshToken? token);
        Task<RefreshToken?> FindFirstWithUserAsync(string token);
    }
}
