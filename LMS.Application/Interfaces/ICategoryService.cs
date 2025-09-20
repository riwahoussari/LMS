using LMS.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Interfaces
{
    public interface ICategoryService
    {
        // CREATE
        Task<CategoryResponseDto> CreateCategory(CreateCategoryDto dto);

        // READ
        Task<IEnumerable<CategoryResponseDto>> GetCategories();
        Task<CategoryResponseDto?> GetCategory(string id);

        // UPDATE
        Task<CategoryResponseDto> UpdateCategory(string id, UpdateCategoryDto dto);

        // DELETE
        Task<bool> DeleteCategory(string id);

    }
}
