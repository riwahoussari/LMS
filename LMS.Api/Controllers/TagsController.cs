using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Application.Services;
using LMS.Application.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;

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
        [HttpPost]
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
        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            return Ok(await _tagService.GetTags());
        }

        [HttpGet("{id}")]
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
        [HttpPatch("{id}")]
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
        [HttpDelete("{id}")]
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
