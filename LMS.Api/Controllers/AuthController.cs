global using static LMS.Common.Helpers.ValidationHelpers;
using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Application.Validators;
using LMS.Infrastructure;
using LMS.Infrastructure.Constants;
using LMS.Infrastructure.Seeding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;


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
        /// <summary>
        /// Register a new admin (Admin only).
        /// </summary>
        /// <remarks>
        /// Requires authentication and Admin role.  
        /// Returns a JWT access token and refresh token for the new admin.  
        /// </remarks>
        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPost("register-admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request or validation errors")]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
        [SwaggerResponse(statusCode: 403, description: "User authenticated but not an admin")]
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

        /// <summary>
        /// Register a new student.
        /// </summary>
        /// <remarks>
        /// Public endpoint.  
        /// Returns a JWT access token and refresh token for the new student.  
        /// </remarks>
        [HttpPost("register-student")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request or validation errors")]
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

        /// <summary>
        /// Register a new tutor.
        /// </summary>
        /// <remarks>
        /// Public endpoint.  
        /// Returns a JWT access token and refresh token for the new tutor.  
        /// </remarks>
        [HttpPost("register-tutor")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request or validation errors")]
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
        /// <summary>
        /// Authenticate a user and issue tokens.
        /// </summary>
        /// <remarks>
        /// Public endpoint.  
        /// Requires valid email and password.  
        /// Returns a JWT access token and refresh token on success.  
        /// </remarks>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request or validation errors")]
        [SwaggerResponse(statusCode: 401, description: "Invalid email or password")]
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
            catch (Exception ex)
            {
                if (ex is AuthenticationException) return Unauthorized(ex.Message);
                return BadRequest(ex.Message);
            }
            
        }


        // ------------------- REFRESH -------------------
        /// <summary>
        /// Refresh the JWT access token.
        /// </summary>
        /// <remarks>
        /// Public endpoint.  
        /// Requires a valid refresh token.  
        /// Issues a new JWT access token and refresh token, invalidating the old one.  
        /// </remarks>
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request or validation errors")]
        [SwaggerResponse(statusCode: 401, description: "Invalid or inactive refresh token")]
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

            if (storedToken.User?.Suspended == true) return BadRequest("Your account is suspended");

            var tokenResponse = await CreateTokenResponse(storedToken.User!);
            return Ok(tokenResponse);
        }


        // ------------------- REVOKE -------------------
        /// <summary>
        /// Revoke a refresh token (Authenticated users only).
        /// </summary>
        /// <remarks>
        /// Requires authentication.  
        /// Revokes the specified refresh token, preventing further use.  
        /// </remarks>
        [Authorize]
        [HttpPost("revoke")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request or validation errors")]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
        [SwaggerResponse(statusCode: 404, description: "Token not found or already inactive")]
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
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName),
                new Claim("role", user.RoleName)
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
