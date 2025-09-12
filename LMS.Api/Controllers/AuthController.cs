using FluentValidation;
using LMS.Application.DTOs;
using LMS.Application.Services;
using LMS.Application.Validators;
using LMS.Domain.Entities;
using LMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly IRefreshTokenService _refreshTokenService;


        public AuthController(UserManager<AppUser> userManager,
                              RoleManager<IdentityRole> roleManager,
                              IConfiguration config,
                              IRefreshTokenService refreshTokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _refreshTokenService = refreshTokenService;
        }

        // ------------------- REGISTER -------------------
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            // data validation
            var validator = new RegisterDtoValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new {
                    Property = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            // creating user
            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                RoleId = (await _roleManager.FindByNameAsync(dto.RoleName.ToLower()))!.Id
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);


            // Tokens
            var jwtToken = await GenerateJwtToken(user);
            RefreshToken refreshToken = await _refreshTokenService.CreateAsync(user);

            return Ok(new
            {
                AccessToken = jwtToken,
                RefreshToken = refreshToken.Token, // Only return the token string
                ExpiresIn = Convert.ToDouble(_config.GetSection("Jwt")["ExpireMinutes"]) * 60, // access token expiry in seconds
                User = new
                {
                    id = user.Id,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    email = user.Email,
                    role = dto.RoleName
                }
            });
        }

        // ------------------- LOGIN -------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // data validation
            var validator = new LoginDtoValidator();
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new {
                    Property = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            // sign in user
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized("Invalid credentials");

            // Tokens
            var role = await _roleManager.FindByIdAsync(user.RoleId!);
            var jwtToken = await GenerateJwtToken(user);
            RefreshToken refreshToken = await _refreshTokenService.CreateAsync(user);

            return Ok(new
            {
                AccessToken = jwtToken,
                RefreshToken = refreshToken.Token, // Only return the token string
                ExpiresIn = Convert.ToDouble(_config.GetSection("Jwt")["ExpireMinutes"]) * 60, // access token expiry in seconds
                User = new
                {
                    id = user.Id,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    email = user.Email,
                    role = role.Name
                }
            });
        }

        // ------------------- REFRESH -------------------
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest dto)
        {
            var storedToken = await _refreshTokenService.GetAsync(dto.RefreshToken, true);

            if (storedToken == null || !storedToken.IsActive)
                return Unauthorized("Invalid refresh token");

            var newJwt = await GenerateJwtToken(storedToken.User);

            // rotate token (invalidate old, issue new)
            await _refreshTokenService.RevokeAsync(storedToken.Token);
            RefreshToken newRefresh = await _refreshTokenService.CreateAsync(storedToken.User);


            return Ok(new
            {
                AccessToken = newJwt,
                RefreshToken = newRefresh.Token, // Only return the token string
                ExpiresIn = Convert.ToDouble(_config.GetSection("Jwt")["ExpireMinutes"]) * 60, // access token expiry in seconds
                User = new
                {
                    id = storedToken.User.Id,
                    firstName = storedToken.User.FirstName,
                    lastName = storedToken.User.LastName,
                    email = storedToken.User.Email,
                    role = storedToken.User.Role.Name,
                }
            });
        }

        // ------------------- REVOKE -------------------
        [Authorize]
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RefreshRequest dto)
        {
            var storedToken = await _refreshTokenService.GetAsync(dto.RefreshToken, false);

            if (storedToken == null || !storedToken.IsActive)
                return NotFound("Token not found");

            await _refreshTokenService.RevokeAsync(storedToken.Token);

            return Ok("Refresh token revoked");
        }

        // ------------------- HELPERS -------------------
        private async Task<string> GenerateJwtToken(AppUser user)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var role = await _roleManager.FindByIdAsync(user.RoleId);
            

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("Role", role.Name)
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

    }
}
