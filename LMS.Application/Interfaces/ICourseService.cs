using LMS.Application.DTOs;
using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Interfaces
{
    public interface ICourseService
    {
        Task<CourseResponseDto> CreateCourseAsync(string tutorId, CreateCourseDto dto);
        //Task<Course?> GetCourseByIdAsync(Guid id);
        //Task<IEnumerable<Course>> GetCoursesAsync();
        //Task UpdateCourseAsync(UpdateCourseDto dto);
        //Task DeleteCourseAsync(Guid id);
    }

}
