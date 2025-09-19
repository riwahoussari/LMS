global using static LMS.Common.Helpers.ValidationHelpers;
using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Application.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using LMS.Infrastructure;
using LMS.Infrastructure.Seeding;
using LMS.Infrastructure.Constants;


namespace LMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthService _authService;
        

        public AuthController(IConfiguration config,
                              IAuthService authService)
        {
            _config = config;
            _authService = authService;
        }

        // ------------------- REGISTER -------------------
        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin(RegisterUserBaseDto dto)
        {
            // data validation
            var errors = await ValidateAsync<RegisterUserBaseDto>(dto,
                typeof(RegisterUserBaseDtoValidator));

            if (errors.Any())
                return BadRequest(new { Errors = errors });

            // Registration
            try
            {
                var user = await _authService.RegisterAdminAsync(dto);
                var tokenResponse = await CreateTokenResponse(user);
                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("register-student")]
        public async Task<IActionResult> RegisterStudent(RegisterStudentDto dto)
        {
            // data validation
            var errors = await ValidateAsync<RegisterStudentDto>(dto,
                typeof(RegisterUserBaseDtoValidator),
                typeof(RegisterStudentDtoValidator));

            if (errors.Any())
                return BadRequest(new { Errors = errors });

            // Registration
            try
            {
                var user = await _authService.RegisterStudentAsync(dto);
                var tokenResponse = await CreateTokenResponse(user);
                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("register-tutor")]
        public async Task<IActionResult> RegisterTutor(RegisterTutorDto dto)
        {
            // data validation
            var errors = await ValidateAsync<RegisterTutorDto>(dto,
                typeof(RegisterUserBaseDtoValidator),
                typeof(RegisterTutorDtoValidator));

            if (errors.Any())
                return BadRequest(new { Errors = errors });

            // Registration
            try
            {
                var user = await _authService.RegisterTutorAsync(dto);
                var tokenResponse = await CreateTokenResponse(user);
                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        // ------------------- LOGIN -------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // data validation
            var errors = await ValidateAsync<LoginDto>(dto,
                typeof(LoginDtoValidator));

            if (errors.Any())
                return BadRequest(new { Errors = errors });

            // sign in user
            try
            {
                var user = await _authService.LoginAsync(dto.Email, dto.Password);
                var tokenResponse = await CreateTokenResponse(user);
                return Ok(tokenResponse);
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(ex.Message);
            }
            
        }


        // ------------------- REFRESH -------------------
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
        {
            // data validation
            var errors = await ValidateAsync<RefreshRequestDto>(dto,
                typeof(RefreshRequestDtoValidator));

            if (errors.Any())
                return BadRequest(new { Errors = errors });

            // validate token
            var storedToken = await _authService.GetRefreshTokenAsync(dto.RefreshToken, true);

            if (storedToken == null || !storedToken.IsActive)
                return Unauthorized("Invalid refresh token");

            // rotate token (invalidate old, issue new)
            await _authService.RevokeRefreshTokenAsync(storedToken.Token);

            var tokenResponse = await CreateTokenResponse(storedToken.User!);
            return Ok(tokenResponse);
        }


        // ------------------- REVOKE -------------------
        [Authorize]
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RefreshRequestDto dto)
        {
            // data validation
            var errors = await ValidateAsync<RefreshRequestDto>(dto,
                typeof(RefreshRequestDtoValidator));

            if (errors.Any())
                return BadRequest(new { Errors = errors });

            // validate token
            var storedToken = await _authService.GetRefreshTokenAsync(dto.RefreshToken, false);

            if (storedToken == null || !storedToken.IsActive)
                return NotFound("Token not found");

            // revoke
            await _authService.RevokeRefreshTokenAsync(storedToken.Token);

            return Ok("Refresh token revoked");
        }


        // ------------------- TOKEN RESPONSE HELPERS -------------------
        private string GenerateJwtToken(UserResponseDto user)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
          
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim(ClaimTypes.Role, user.RoleName)
            };


            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        private async Task<AuthResponseDto> CreateTokenResponse(UserResponseDto user)
        {
            var jwtToken = GenerateJwtToken(user);
            var refreshToken = await _authService.CreateRefreshTokenAsync(user.Id);

            return new AuthResponseDto
            {
                AccessToken = jwtToken,
                RefreshToken = refreshToken.Token,
                ExpiresIn = Convert.ToDouble(_config.GetSection("Jwt")["ExpiryMinutes"]) * 60,
                User = user
            };
        }

    }
}
