using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Application.Validators;
using LMS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        [HttpPost]
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
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            return Ok(await _categoryService.GetCategories());
        }

        [HttpGet("{id}")]
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
        [HttpPatch("{id}")]
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
        [HttpDelete("{id}")]
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
