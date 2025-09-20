using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // one-to-many relashionship with Courses
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
