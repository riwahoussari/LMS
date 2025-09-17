using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Interfaces
{
    public interface IUserRepository : IRepository<AppUser>
    {
        Task<AppUser?> GetByIdWithProfileAsync(string id);
        Task<IEnumerable<AppUser>> GetAllWithProfilesAsync();
        Task<IEnumerable<AppUser>> FindWithProfilesAsync(Expression<Func<AppUser, bool>> predicate);
        Task<AppUser?> FindFirstWithProfileAsync(Expression<Func<AppUser, bool>> predicate);
        Task<AppUser?> FindSingleWithProfileAsync(Expression<Func<AppUser, bool>> predicate);
    }
}
