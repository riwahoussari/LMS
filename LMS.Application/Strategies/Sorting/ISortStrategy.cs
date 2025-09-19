using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Strategies.Sorting
{
    public interface ISortStrategy<T> where T : class
    {
        string Key { get; }  // e.g., "Age", "FirstName", etc.
        IQueryable<T> Apply(IQueryable<T> query, bool ascending);
    }
}
