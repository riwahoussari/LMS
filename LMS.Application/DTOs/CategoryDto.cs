using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.DTOs
{
    public class CreateCategoryDto
    {
        public string Name { get; set; } = null!;
    }

    public class UpdateCategoryDto
    {
        public string Name { get; set; } = null!;
    }


    public class CategoryResponseDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
    }

    public class CategoryStatsResponseDto
    { 
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int TotalCourses { get; set; }    
        public int TotalEnrollments { get; set; }    
    }

}
