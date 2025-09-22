using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Application.Services;
using LMS.Application.Validators;
using LMS.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        // CREATE
        /// <summary>
        /// Create a new tag. (Admin only)
        /// </summary>
        /// <remarks>
        /// - Required **Admin** role.
        /// - Requires a unique tag name.  
        /// </remarks>
        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TagResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request or validation errors")]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
        [SwaggerResponse(statusCode: 403, description: "User authenticated but not an Admin")]
        public async Task<IActionResult> CreateTag(CreateTagDto dto)
        {
            // data validation
            var errors = await ValidateAsync<CreateTagDto>(dto, typeof(CreateTagDtoValidator));

            if (errors.Any())
                return BadRequest(new { Errors = errors });

            // create tag
            try
            {
                return Ok(await _tagService.CreateTag(dto));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // READ
        /// <summary>
        /// Get all tags.
        /// </summary>
        /// <remarks>
        /// - Requires authentication 
        /// - Returns a list of all available tags.  
        /// </remarks>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TagResponseDto>))]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
        public async Task<IActionResult> GetTags()
        {
            return Ok(await _tagService.GetTags());
        }


        /// <summary>
        /// Get a tag by ID.
        /// </summary>
        /// <remarks>
        /// - Requires authentication.  
        /// - Returns 404 if the tag is not found.  
        /// </remarks>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TagResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request")]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
        [SwaggerResponse(statusCode: 404, description: "Tag not found")]
        public async Task<IActionResult> GetTag(string id)
        {
            try
            {
                var tag = await _tagService.GetTag(id);

                if (tag == null) return NotFound("Tag not found");
                return Ok(tag);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // UPDATE 
        /// <summary>
        /// Update a tag. (Admin only)
        /// </summary>
        /// <remarks>
        /// - Required **Admin** role.
        /// - Requires a valid tag ID.  
        /// - Requires a unique tag name
        /// </remarks>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TagResponseDto))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request or duplicate tag name.")]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
        [SwaggerResponse(statusCode: 403, description: "User authenticated but not an Admin")]
        [SwaggerResponse(statusCode: 404, description: "Tag not found")]
        public async Task<IActionResult> UpdateTag(string id, UpdateTagDto dto)
        {
            try
            {
                return Ok(await _tagService.UpdateTag(id, dto));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        // DELETE
        /// <summary>
        /// Delete a tag. (Admin only)
        /// </summary>
        /// <remarks>
        /// - Required **Admin** Role.
        /// - Requires a valid tag ID.  
        /// - Returns 404 if the tag does not exist.  
        /// </remarks>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [SwaggerResponse(statusCode: 400, description: "Invalid request")]
        [SwaggerResponse(statusCode: 401, description: "User not authenticated")]
        [SwaggerResponse(statusCode: 403, description: "User authenticated but not an Admin")]
        [SwaggerResponse(statusCode: 404, description: "Tag not found")]
        public async Task<IActionResult> DeleteTag(string id)
        {
            try
            {
                await _tagService.DeleteTag(id);
                return Ok("Tag deleted.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
