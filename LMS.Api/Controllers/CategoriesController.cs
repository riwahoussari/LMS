using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Application.Validators;
using LMS.Domain.Entities;
using LMS.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // CREATE

        /// <summary>
        /// Create a new category (Admin only).
        /// </summary>
        /// <remarks>
        /// Requires authentication and <b>Admin</b> role. 
        /// Category name should be unique
        /// </remarks>
        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Category name already exists")]
        [SwaggerResponse(statusCode: 401, description: "User not authenticate")]
        [SwaggerResponse(statusCode: 403, description: "User authenticated but not an admin")]
        public async Task<IActionResult> CreateCategory(CreateCategoryDto dto)
        {
            // data validation
            var errors = await ValidateAsync<CreateCategoryDto>(dto, typeof(CreateCategoryDtoValidator));

            if (errors.Any())
                return BadRequest(new { Errors = errors });

            // create category
            try
            {
                return Ok(await _categoryService.CreateCategory(dto));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // READ

        /// <summary>
        /// Get all categories (Authenticated users only).
        /// </summary>
        /// <remarks>
        /// Requires authentication.
        /// </remarks>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CategoryResponseDto>))]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
        public async Task<IActionResult> GetCategories()
        {
            return Ok(await _categoryService.GetCategories());
        }

        /// <summary>
        /// Get a single category by ID (Authenticated users only).
        /// </summary>
        /// <remarks>
        /// Requires authentication.
        /// </remarks>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryResponseDto))]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
        [SwaggerResponse(statusCode: 404, description: "Category not found")]
        public async Task<IActionResult> GetCategory(string id)
        {
            try
            {
                var category = await _categoryService.GetCategory(id);

                if (category == null) return NotFound("Category not found");
                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // UPDATE

        /// <summary>
        /// Update a category (Admin only).
        /// </summary>
        /// <remarks>
        /// Requires authentication and Admin role.
        /// Category name must be unique.
        /// </remarks>
        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request or update failed (a category with that name already exists)")]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
        [SwaggerResponse(statusCode: 403, description: "User authenticated but not an admin")]
        public async Task<IActionResult> UpdateCategory(string id, UpdateCategoryDto dto)
        {
            try
            {
                return Ok(await _categoryService.UpdateCategory(id, dto));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // DELETE

        /// <summary>
        /// Delete a category (Admin only).
        /// </summary>
        /// <remarks>
        /// Requires authentication and Admin role.
        /// </remarks>
        [Authorize(Roles = RoleConstants.Admin)]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request or deletion failed")]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
        [SwaggerResponse(statusCode: 403, description: "User authenticated but not an admin")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            try
            {
                await _categoryService.DeleteCategory(id);
                return Ok("Category deleted.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
