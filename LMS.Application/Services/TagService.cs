using AutoMapper;
using LMS.Application.DTOs;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Services
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public TagService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        // CREATE
        public async Task<TagResponseDto> CreateTag(CreateTagDto dto)
        {
            if (await _uow.Tag.FindFirstAsync(tag => tag.Name.ToLower() == dto.Name.ToLower().Trim()) != null)
                throw new Exception("A tag with that name already exists.");

            var tag = new Tag { Name = dto.Name.Trim() };

            await _uow.Tag.AddAsync(tag);
            await _uow.CompleteAsync();

            return _mapper.Map<TagResponseDto>(tag);
        }

        // READ
        public async Task<IEnumerable<TagResponseDto>> GetTags()
        {
            var tags = await _uow.Tag.GetAllAsync();
            return _mapper.Map<IEnumerable<TagResponseDto>>(tags);
        }

        public async Task<IEnumerable<TagStatsResponseDto>> GetTagsWithStats()
        {
            var tags = await _uow.Tag.GetAllAsync();
            var result = new List<TagStatsResponseDto>();

            foreach (var tag in tags)
            {
                result.Add(new TagStatsResponseDto
                {
                    Id = tag.Id.ToString(),
                    Name = tag.Name,
                    TotalCourses = await _uow.Courses.CountAsync(c => c.Tags.Contains(tag)),
                    TotalEnrollments = await _uow.Enrollments.CountAsync(e => e.Course.Tags.Contains(tag))
                });
            }

            return result;
        }

        public async Task<TagResponseDto?> GetTag(string id)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("Invalid id");
            }

            var tag = await _uow.Tag.FindSingleAsync(t => t.Id == guid);

            return tag == null ? null : _mapper.Map<TagResponseDto>(tag);
        }

        // UPDATE
        public async Task<TagResponseDto> UpdateTag(string id, UpdateTagDto dto)
        {
            // Find category
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("Invalid id");
            }

            var tag = await _uow.Tag.FindSingleAsync(t => t.Id == guid);

            if (tag == null)
            {
                throw new Exception("Tag not found");
            }

            if (await _uow.Tag.FindFirstAsync(tag => tag.Name.ToLower() == dto.Name.ToLower().Trim()) != null)
                throw new Exception("A tag with that name already exists.");

            // update category
            tag.Name = string.IsNullOrEmpty(dto.Name) ? tag.Name : dto.Name.Trim();

            await _uow.CompleteAsync();
            return _mapper.Map<TagResponseDto>(tag);
        }

        // DELETE
        public async Task<bool> DeleteTag(string id)
        {
            // Find category
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("Invalid id");
            }

            var tag = await _uow.Tag.FindSingleAsync(t => t.Id == guid);

            if (tag == null)
            {
                throw new Exception("Tag not found");
            }

            // Delete category
            _uow.Tag.Remove(tag);
            await _uow.CompleteAsync();
            return true;
            
        }  
    }
}
