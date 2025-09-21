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

    public static class TypeExtensions
    {
        public static bool ImplementsGenericInterface(this Type type, Type genericInterface)
        {
            return type.GetInterfaces()
                .Any(i => i.IsGenericType &&
                          i.GetGenericTypeDefinition() == genericInterface);
        }
    }


    public class SortStrategyOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null || !operation.Parameters.Any()) return;

            var param = operation.Parameters
                .FirstOrDefault(p => string.Equals(p.Name, "sortBy", StringComparison.OrdinalIgnoreCase));
            if (param == null) return;

            // Detect entity type from controller name (could also use custom attribute)
            var controllerName = context.ApiDescription.ActionDescriptor.RouteValues["controller"];
            Type? targetEntity = controllerName switch
            {
                "Users" => typeof(AppUser),
                "Courses" => typeof(Course),
                _ => null
            };
            if (targetEntity == null) return;

            // Find strategies for this entity
            var strategyTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return Array.Empty<Type>(); }
                })
                .Where(t => !t.IsInterface && !t.IsAbstract)
                .Where(t => t.ImplementsGenericInterface(typeof(ISortStrategy<>)))
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(ISortStrategy<>) &&
                    i.GetGenericArguments()[0] == targetEntity))
                .ToList();

            var values = new List<string>();
            foreach (var t in strategyTypes)
            {
                if (Activator.CreateInstance(t) is ISortStrategy s &&
                    !string.IsNullOrWhiteSpace(s.Key))
                {
                    values.Add(s.Key);
                }
            }

            if (!values.Any()) return;

            values = values
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(v => v, StringComparer.OrdinalIgnoreCase)
                .ToList();

            param.Schema ??= new OpenApiSchema { Type = "string" };
            param.Schema.Enum = values.Select(v => (IOpenApiAny)new OpenApiString(v)).ToList();
        }


        private Type? DetectEntityType(OperationFilterContext context)
        {
            // Example: check the controller or return type
            var controllerName = context.ApiDescription.ActionDescriptor.RouteValues["controller"];

            return controllerName switch
            {
                "Users" => typeof(ISortStrategy<AppUser>),
                "Courses" => typeof(ISortStrategy<Course>),
                _ => null
            };
        }

        

    }
}
