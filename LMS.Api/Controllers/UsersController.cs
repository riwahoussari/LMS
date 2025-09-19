global using static LMS.Common.Helpers.ValidationHelpers;
using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Application.Services;
using LMS.Application.Validators;
using LMS.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

        //[Authorize(Roles = RoleConstants.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersQueryDto query)
        {
            // data validation
            var errors = await ValidateAsync<GetUsersQueryDto>(query,
                typeof(GetUsersQueryDtoValidator));

            if (errors.Any())
                return BadRequest(new { Errors = errors });

            // filter by role
            var users = await _userService.GetAllAsync(query, withProfile: true);

            return Ok(users);
        }
    }

}