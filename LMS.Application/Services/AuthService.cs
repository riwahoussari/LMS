using AutoMapper;
using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;
using System.Security.Cryptography;

namespace LMS.Application.Services
{
    

    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _uow;
        
        public AuthService(UserManager<AppUser> userManager, 
                        IMapper mapper,
                        RoleManager<IdentityRole> roleManager,
                        IUnitOfWork uow)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _uow = uow;
            _mapper = mapper;
        }

        // Register
        public async Task<UserResponseDto> RegisterStudentAsync(RegisterStudentDto dto)
        {
            // Create AppUser
            var role = await _roleManager.FindByNameAsync("student");
            var user = new AppUser
            {
                UserName = dto.Email.ToLower(),
                Email = dto.Email.ToLower(),
                FirstName = dto.FirstName.ToLower(),
                LastName = dto.LastName.ToLower(),
                BirthDate = dto.BirthDate,
                RoleId = role.Id
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            // Assign role
            await _userManager.AddToRoleAsync(user, "student");

            // Create Student Profile
            var studentProfile = new StudentProfile
            {
                UserId = user.Id,
                Major = dto.Major
            };

            await _uow.StudentProfiles.AddAsync(studentProfile);
            await _uow.CompleteAsync();

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> RegisterTutorAsync(RegisterTutorDto dto)
        {
            // Create AppUser
            var role = await _roleManager.FindByNameAsync("tutor");
            var user = new AppUser
            {
                UserName = dto.Email.ToLower(),
                Email = dto.Email.ToLower(),
                FirstName = dto.FirstName.ToLower(),
                LastName = dto.LastName.ToLower(),
                BirthDate = dto.BirthDate,
                RoleId = role.Id
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            // Assign role
            await _userManager.AddToRoleAsync(user, "tutor");

            // Create Tutor Profile
            var tutorProfile = new TutorProfile
            {
                UserId = user.Id,
                Bio = dto.Bio,
                Expertise = dto.Expertise
            };

            await _uow.TutorProfiles.AddAsync(tutorProfile);
            await _uow.CompleteAsync();

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> RegisterAdminAsync(RegisterUserBaseDto dto)
        {
            var role = await _roleManager.FindByNameAsync("admin");
            var user = new AppUser
            {
                UserName = dto.Email.ToLower(),
                Email = dto.Email.ToLower(),
                FirstName = dto.FirstName.ToLower(),
                LastName = dto.LastName.ToLower(),
                BirthDate = dto.BirthDate,
                RoleId = role.Id
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            // Assign role
            await _userManager.AddToRoleAsync(user, "admin");

            return _mapper.Map<UserResponseDto>(user);

        }


        // Login
        public async Task<UserResponseDto> LoginAsync(string email, string password)
        {
            var user = await _uow.Users.FindSingleAsync(u => u.Email == email.ToLower());
     
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                throw new AuthenticationException("Invalid Credentials");

            return _mapper.Map<UserResponseDto>(user);
        }


        // Refresh Tokens
        public async Task<RefreshTokenDto> CreateRefreshTokenAsync(string userId)
        {
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(7) // configurable
            };
            
            await _uow.RefreshTokens.AddAsync(refreshToken);
            await _uow.CompleteAsync();

            return _mapper.Map<RefreshTokenDto>(refreshToken);
        }

        public async Task<RefreshTokenDto?> GetRefreshTokenAsync(string token, bool includeUser = false)
        {
            var refreshToken = includeUser ? 
                await _uow.RefreshTokens.FindFirstWithUserAsync(token) :
                await _uow.RefreshTokens.FindFirstAsync(r => r.Token == token);

            return refreshToken == null ? null : _mapper.Map<RefreshTokenDto>(refreshToken);
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            
            var storedToken = await _uow.RefreshTokens.FindFirstAsync(r => r.Token == token);
            _uow.RefreshTokens.RevokeToken(storedToken);
            await _uow.CompleteAsync();
        }
    }
}
