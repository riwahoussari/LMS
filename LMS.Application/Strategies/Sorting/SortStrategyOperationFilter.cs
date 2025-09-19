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
    public class SortStrategyOperationFilter<T> : IOperationFilter
        where T : class
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null || !operation.Parameters.Any()) return;

            // match query parameter name (case-insensitive)
            var param = operation.Parameters
                .FirstOrDefault(p => string.Equals(p.Name, "sortBy", StringComparison.OrdinalIgnoreCase));

            if (param == null) return;

            // discover strategy types (search all loaded assemblies)
            var strategyTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return Array.Empty<Type>(); }
                })
                .Where(t => typeof(ISortStrategy<T>).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToList();

            var values = new List<string>();

            foreach (var t in strategyTypes)
            {

                // try to instantiate and get Key property
                try
                {
                    if (Activator.CreateInstance(t) is ISortStrategy<T> s)
                    {
                        if (!string.IsNullOrWhiteSpace(s.Key))
                            values.Add(s.Key);
                        continue;
                    }
                }
                catch
                {
                    // ignore
                }

                // fallback: strip "SortStrategy" from type name
                var name = t.Name.Replace("SortStrategy", "", StringComparison.OrdinalIgnoreCase);
                if (!string.IsNullOrWhiteSpace(name))
                    values.Add(name);
            }

            values = values
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(v => v, StringComparer.OrdinalIgnoreCase)
                .ToList();

            // ensure parameter schema exists and is string typed
            param.Schema ??= new OpenApiSchema { Type = "string" };
            param.Schema.Type = "string";

            // set the enum values shown in Swagger
            param.Schema.Enum = values.Select(v => new OpenApiString(v) as IOpenApiAny).ToList();

        }
    }
}
