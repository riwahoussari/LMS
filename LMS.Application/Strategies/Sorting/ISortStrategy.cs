using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Strategies.Sorting
{
    public interface ISortStrategy
    {
        string Key { get; }
    }

    public interface ISortStrategy<T> : ISortStrategy
    {
        IQueryable<T> Apply(IQueryable<T> query, bool ascending);
    }
}
