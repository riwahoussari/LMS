using LMS.Application.Services;
using LMS.Application.Strategies.Sorting.Concrete;
using LMS.Domain.Entities;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Strategies.Sorting
{
    public class SortStrategyFactory<T> where T : class
    {
        private readonly Dictionary<string, ISortStrategy<T>> _strategies;

        public SortStrategyFactory(IEnumerable<ISortStrategy<T>> strategies)
        {
            _strategies = strategies.ToDictionary(s => s.Key.ToLower(), StringComparer.OrdinalIgnoreCase);
        }

        public IQueryable<T> Apply(IQueryable<T> query, string? sortBy, bool ascending = true)
        {
            if (string.IsNullOrEmpty(sortBy) || !_strategies.ContainsKey(sortBy.ToLower()))
                return query; // fallback: no sort

            return _strategies[sortBy].Apply(query, ascending);
        }

        public IEnumerable<string> AvailableKeys() => _strategies.Keys; // for documentation
    }

}
