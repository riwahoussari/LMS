using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.DTOs
{
    // Register
    public class RegisterUserBaseDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string BirthDate { get; set; } = null!;
    }

    public class RegisterStudentDto : RegisterUserBaseDto
    {
        public string Major { get; set; } = null!;
    }

    public class RegisterTutorDto : RegisterUserBaseDto
    {
        public string Bio { get; set; } = null!;
        public string Expertise { get; set; } = null!;
    }


    // Login
    public class LoginDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }


    // Tokens
    public class JwtResponseDto
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public string Username { get; set; } = null!;
        public string RoleName { get; set; } = null!;
    }

    public class RefreshTokenDto
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserResponseDto? User { get; set; }
    }

    public class RefreshRequestDto
    {
        public string RefreshToken { get; set; } = null!;
    }


    // Response
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public double ExpiresIn { get; set; }
        public UserResponseDto User { get; set; } = null!;
    }
}
