using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces.Repositories;

namespace TaskManagement.Application.Services
{
    public class CategoryService : ICategoryService
    {

        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<CategoryDto>>> GetAllCategoriesAsync()
        {
            _logger.LogInformation("Retrieving all categories");

            var categories = await _categoryRepository.GetAllAsync();
            var categoryDtos = categories.Select(c => new CategoryDto
            {
                id = c.Id,
                Name = c.Name,
                Color = c.Color,
                TaskCount = c.Tasks.Count,
                ActiveTasksCount = GetActiveTasksCount(c),
                OverdueTasksCount = GetOverdueTasksCount(c)
            });

            return ServiceResult<IEnumerable<CategoryDto>>.Success(categoryDtos);
        }

        public async Task<ServiceResult<CategoryDto>> GetCategoryByIdAsync(Guid id)
        {
            _logger.LogInformation("Retrieving category with ID: {CategoryId}", id);

            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return ServiceResult<CategoryDto>.Failure($"Category with ID {id} not found");
            }

            var categoryDto = new CategoryDto
            {
                id = category.Id,
                Name = category.Name,
                Color = category.Color,
                TaskCount = category.Tasks.Count,
                ActiveTasksCount = GetActiveTasksCount(category),
                OverdueTasksCount = GetOverdueTasksCount(category)
            };

            return ServiceResult<CategoryDto>.Success(categoryDto);
        }

        // Business logic methods - All business rules here in service layer
        private static int GetActiveTasksCount(Category category)
        {
            return category.Tasks.Count(t => !t.IsCompleted);
        }

        private static int GetOverdueTasksCount(Category category)
        {
            return category.Tasks.Count(t =>
                t.DueDate.HasValue &&
                t.DueDate < DateTime.UtcNow &&
                !t.IsCompleted);
        }
    }
}
