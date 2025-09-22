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
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate) => await _dbSet.CountAsync(predicate);
        
        public virtual async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);
        public virtual async Task<T?> GetByIdAsync(string id) => await _dbSet.FindAsync(id);

        public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) => await _dbSet.Where(predicate).ToListAsync();

        public virtual IQueryable<T> Query() => _dbSet.AsQueryable();

        public virtual async Task<T?> FindFirstAsync(Expression<Func<T, bool>> predicate) =>
            await _dbSet.Where(predicate).FirstOrDefaultAsync();

        public virtual async Task<T?> FindSingleAsync(Expression<Func<T, bool>> predicate) =>
            await _dbSet.Where(predicate).SingleOrDefaultAsync();

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        public void Update(T entity) => _dbSet.Update(entity);

        public void Remove(T entity) => _dbSet.Remove(entity);

        
    }

}
