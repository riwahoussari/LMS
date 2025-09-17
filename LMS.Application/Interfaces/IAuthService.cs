using LMS.Application.DTOs;
using LMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Interfaces
{
    public interface IAuthService
    {

        // Register
        Task<UserResponseDto> RegisterAdminAsync(RegisterUserBaseDto dto);
        Task<UserResponseDto> RegisterTutorAsync(RegisterTutorDto dto);
        Task<UserResponseDto> RegisterStudentAsync(RegisterStudentDto dto);

        // Login
        Task<UserResponseDto> LoginAsync(string email, string password);

        // Refresh Tokens
        Task<RefreshTokenDto> CreateRefreshTokenAsync(string userId);
        Task<RefreshTokenDto?> GetRefreshTokenAsync(string token, bool includeUser = false);
        Task RevokeRefreshTokenAsync(string token);
    }
}
