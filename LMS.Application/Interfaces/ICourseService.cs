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
        // CREATE
        Task<CourseResponseDto> CreateAsync(string tutorId, CreateCourseDto dto);

        // READ
        Task<IEnumerable<CourseResponseDto>> GetAllAsync(GetCoursesQueryDto dto);
        Task<CourseResponseDto?> GetByIdAsync(string id);

        // UPDATE
        Task<CourseResponseDto> Publish(string id);
        Task<CourseResponseDto> Update(string id, string tutorId, UpdateCourseDto dto);


        // DELETE
        Task<CourseResponseDto> Archive(string id, string? tutorId);

        // TUTORS
        Task<CourseResponseDto> AssignTutor(string id, string tutorId, string requesterId);
        Task<CourseResponseDto> UnassignTutor(string id, string tutorId, string requesterId);
    }

}
