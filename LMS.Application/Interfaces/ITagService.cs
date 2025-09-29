using LMS.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Interfaces
{
    public interface ITagService
    {
        // CREATE
        Task<TagResponseDto> CreateTag(CreateTagDto dto);

        // READ
        Task<IEnumerable<TagResponseDto>> GetTags();
        Task<IEnumerable<TagStatsResponseDto>> GetTagsWithStats();
        Task<TagResponseDto?> GetTag(string id);

        // UPDATE
        Task<TagResponseDto> UpdateTag(string id, UpdateTagDto dto);

        // DELETE
        Task<bool> DeleteTag(string id);
    }
}
