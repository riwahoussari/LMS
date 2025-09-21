using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Strategies.Sorting.Concrete
{


    [SortKey("Title")]
    public class TitleSortStrategy : ISortStrategy<Course>
    {
        public string Key => "Title";

        public IQueryable<Course> Apply(IQueryable<Course> query, bool ascending)
            => ascending ? query.OrderBy(c => c.Title)
                            : query.OrderByDescending(c => c.Title);
    }

    [SortKey("Enrollments")]
    public class EnrollmentsSortStrategy : ISortStrategy<Course>
    {
        public string Key => "Enrollments";

        public IQueryable<Course> Apply(IQueryable<Course> query, bool ascending)
            => ascending ? query.OrderBy(c => c.Enrollments.Count())
                            : query.OrderByDescending(c => c.Enrollments.Count());
    }

    [SortKey("SpotsLeft")]
    public class SpotsLeftSortStrategy : ISortStrategy<Course>
    {
        public string Key => "SpotsLeft";

        public IQueryable<Course> Apply(IQueryable<Course> query, bool ascending)
            => ascending ? query.OrderBy(c => c.MaxCapacity - c.Enrollments.Count())
                            : query.OrderByDescending(c => c.MaxCapacity - c.Enrollments.Count());
    }
}
