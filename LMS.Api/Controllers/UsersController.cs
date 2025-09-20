global using static LMS.Common.Helpers.ValidationHelpers;
using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Application.Services;
using LMS.Application.Validators;
using LMS.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LMS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;


        public UsersController(IConfiguration config,
                              IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersQueryDto query)
        {
            // data validation
            var errors = await ValidateAsync<GetUsersQueryDto>(query,
                typeof(GetUsersQueryDtoValidator));

            if (errors.Any())
                return BadRequest(new { Errors = errors });

            // if the current user is not an admin => they can only see tutors
            if (!User.IsInRole(RoleConstants.Admin) && query.Role != null && query.Role.ToLower() != RoleConstants.Tutor)
            {
                return Forbid();
            }

            // get users
            var users = await _userService.GetAllAsync(query, withProfile: true);

            return Ok(users);
        }


        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var targetUser = await _userService.GetByIdAsync(id, withProfile: true);

            if (targetUser == null) return NotFound("User not found");

            // tutors cannot see admins
            if (User.IsInRole(RoleConstants.Tutor) && targetUser.RoleName == RoleConstants.Admin)
            {
                return NotFound("User not found"); // generic not found to avoid leaking info
            }

            // students can only see tutors
            if (User.IsInRole(RoleConstants.Student) && targetUser.RoleName != RoleConstants.Tutor)
            { 
                if (currentUserId != targetUser.Id) return NotFound("User not found");
            }

            return Ok(targetUser);
        }


        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPatch("{id}")]
        public async Task<IActionResult> SuspendUser(string id, SuspendUserDto dto)
        {
            // data validation
            var errors = await ValidateAsync<SuspendUserDto>(dto, typeof(SuspendUserDtoValidator));

            if (errors.Any())
                return BadRequest(new { Errors = errors });


            // suspend user
            try
            {
                await _userService.ToggleSuspendedAsync(id, dto.IsSuspended);
                return Ok(dto.IsSuspended ? "User suspended" : "User Unsuspended");
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException) return Forbid();
                else if (ex is KeyNotFoundException) return NotFound("User not found");
                return BadRequest(ex.Message);
            }
        }   



        [Authorize]
        [HttpPatch("me")]
        public async Task<IActionResult> UpdateMyProfile(UserUpdateDto dto)
        {
            // data validation
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return BadRequest();

            var errors = await ValidateAsync<UserUpdateDto>(dto, typeof(UserUpdateDtoValidator));

            if (errors.Any())
                return BadRequest(new { Errors = errors });

            // update
            await _userService.UpdateAsync(userId, dto);
            return Ok();
        }

        

    }

}