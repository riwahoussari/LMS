using LMS.Application.DTOs;
using LMS.Domain.Entities;
using LMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Interfaces
{
    public interface IEnrollmentService
    {
        // CREATE
        Task<EnrollmentResponseDto> EnrollAsync(string studentUserId, string courseId);

        // READ
        Task<IEnumerable<EnrollmentResponseDto>> GetAllAsync();
        Task<EnrollmentResponseDto> GetOneAsync(string courseId, string studentProfileId, string requesterId);
        Task<IEnumerable<EnrollmentResponseDto>> GetByStudentAsync(string studentProfileId, EnrollmentStatus? statusFilter);
        Task<IEnumerable<EnrollmentResponseDto>> GetMyEnrollmentsAsync(string studentId, EnrollmentStatus? statusFilter);
        Task<IEnumerable<ExtendedEnrollmentResponseDto>> GetByCourseAsync(string courseId);

        // Update
        Task<EnrollmentResponseDto> UpdateAsync(string courseId, string studentProfileId, string requesterId, EnrollmentStatus status);
        //Task UpdateEnrollmentStatusAsync(Guid studentProfileId, Guid courseId, EnrollmentStatus status);
    }
}
