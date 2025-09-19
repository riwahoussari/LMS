using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Strategies.Sorting.Concrete
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SortKeyAttribute : Attribute
    {
        public string Key { get; }
        public SortKeyAttribute(string key) => Key = key;
    }


    [SortKey("Age")]
    public class AgeSortStrategy : ISortStrategy<AppUser>
    {
        public string Key => "Age";

        public IQueryable<AppUser> Apply(IQueryable<AppUser> query, bool ascending)
            => ascending ? query.OrderBy(u => u.BirthDate)
                            : query.OrderByDescending(u => u.BirthDate);
    }

    [SortKey("FirstName")]
    public class FirstNameSortStrategy : ISortStrategy<AppUser>
    {
        public string Key => "FirstName";

        public IQueryable<AppUser> Apply(IQueryable<AppUser> query, bool ascending)
            => ascending ? query.OrderBy(u => u.FirstName)
                            : query.OrderByDescending(u => u.FirstName);
    }

    [SortKey("LastName")]
    public class LastNameSortStrategy : ISortStrategy<AppUser>
    {
        public string Key => "LastName";

        public IQueryable<AppUser> Apply(IQueryable<AppUser> query, bool ascending)
            => ascending ? query.OrderBy(u => u.LastName)
                            : query.OrderByDescending(u => u.LastName);
    }
    
}
