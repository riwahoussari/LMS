using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetByIdAsync(Guid id);
        Task<T?> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> Query();
        Task<T?> FindFirstAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FindSingleAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
    }

}
