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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }


        // CREATE
        public async Task<CategoryResponseDto> CreateCategory(CreateCategoryDto dto)
        {
            if (await _uow.Category.FindFirstAsync(cat => cat.Name.ToLower() == dto.Name.ToLower().Trim()) != null)
                throw new Exception("A category with that name already exists.");

            var category = new Category { Name = dto.Name.Trim()};

            await _uow.Category.AddAsync(category);
            await _uow.CompleteAsync();

            return _mapper.Map<CategoryResponseDto>(category);
        }

        // READ
        public async Task<IEnumerable<CategoryResponseDto>> GetCategories()
        {
            var categories = await _uow.Category.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
        }

        public async Task<IEnumerable<CategoryStatsResponseDto>> GetCategoriesWithStats()
        {
            var categories = await _uow.Category.GetAllAsync();
            var result = new List<CategoryStatsResponseDto>();

            foreach (var category in categories)
            {
                result.Add(new CategoryStatsResponseDto
                {
                    Id = category.Id.ToString(),
                    Name = category.Name,
                    TotalCourses = await _uow.Courses.CountAsync(c => c.CategoryId == category.Id),
                    TotalEnrollments = await _uow.Enrollments.CountAsync(e => e.Course.CategoryId == category.Id)
                });
            }

            return result;
        }


        public async Task<CategoryResponseDto?> GetCategory(string id)
        {
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("Invalid id");
            }

            var category = await _uow.Category.FindSingleAsync(c => c.Id == guid);

            return category == null ? null : _mapper.Map<CategoryResponseDto>(category);
        }

        // UPDATE
        public async Task<CategoryResponseDto> UpdateCategory(string id, UpdateCategoryDto dto)
        {
            // Find category
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("Invalid id");
            }

            var category = await _uow.Category.FindSingleAsync(c => c.Id == guid);

            if (category == null)
            {
                throw new Exception("Category not found");
            }

            // update category
            if (await _uow.Category.FindFirstAsync(cat => cat.Name.ToLower() == dto.Name.ToLower().Trim()) != null)
                throw new Exception("A category with that name already exists.");

            category.Name = string.IsNullOrEmpty(dto.Name) ? category.Name : dto.Name.Trim();
              
            await _uow.CompleteAsync();
            return _mapper.Map<CategoryResponseDto>(category);
        }

        // DELETE
        public async Task<bool> DeleteCategory(string id)
        {
            // Find category
            if (!Guid.TryParse(id, out Guid guid))
            {
                throw new Exception("Invalid id");
            }

            var category = await _uow.Category.FindSingleAsync(c => c.Id == guid);

            if (category == null)
            {
                throw new Exception("Category not found");
            }

            // Delete category
            try
            {
                _uow.Category.Remove(category);
                await _uow.CompleteAsync();
                return true;
            }
            catch(DbUpdateException ex)
            {
                // Check if it's a foreign key constraint violation
                if (ex.InnerException != null)
                {
                    if (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23503")
                    {
                        // Handle foreign key constraint violation
                        throw new InvalidOperationException("Cannot delete category because it has associated courses.");
                    }
                }
                // Re-throw if it's not a constraint violation
                throw;
            }
        }

    }
}
