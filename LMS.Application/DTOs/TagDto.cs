using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.DTOs
{
    public class CreateTagDto
    {
        public string Name { get; set; } = null!;
    }

    public class UpdateTagDto
    {
        public string Name { get; set; } = null!;
    }


    public class TagResponseDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
    }

    public class TagStatsResponseDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int TotalCourses { get; set; }
        public int TotalEnrollments { get; set; }
    }
}
