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
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(AppDbContext context) : base(context)
        {
        }

        // override default Repository methods
        public override IQueryable<Course> Query()
        {
            return _dbSet
                .Include(c => c.TutorProfiles)
                    .ThenInclude(t => t.User)
                .Include(c => c.Category)
                .Include(c => c.Tags)
                .Include(c => c.Schedule)
                .Include(c => c.Schedule.Sessions)
                .Include(c => c.Prerequisites)
                    .ThenInclude(p => p.PrerequisiteCourse)
                .AsQueryable();
        }

        public override async Task<Course?> FindSingleAsync(Expression<Func<Course, bool>> predicate)
        {
            return await _dbSet
                .Include(c => c.TutorProfiles)
                    .ThenInclude(t => t.User)
                .Include(c => c.Category)
                .Include(c => c.Tags)
                .Include(c => c.Schedule)
                .Include(c => c.Schedule.Sessions)
                .Include(c => c.Prerequisites)
                    .ThenInclude(p => p.PrerequisiteCourse)
                .Where(predicate)
                .SingleOrDefaultAsync();
        }
    }
}
