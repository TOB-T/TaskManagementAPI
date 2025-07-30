using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Interfaces.Repositories;

namespace TaskManagement.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<TaskService> _logger;

        public TaskService(
            ITaskRepository taskRepository,
            ICategoryRepository categoryRepository,
            ILogger<TaskService> logger)
        {
            _taskRepository = taskRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<PaginatedResult<TaskDto>> GetAllTasksAsync(
     Guid? categoryId,
     Priority? priority,
     bool? isCompleted,
     string? search,
     int page,
     int pageSize)
        {
            _logger.LogInformation("Retrieving tasks with filters - Category: {CategoryId}, Priority: {Priority}, Completed: {IsCompleted}, Search: {Search}",
                categoryId, priority, isCompleted, search);

            var tasks = await _taskRepository.GetAllAsync(categoryId, priority, isCompleted, search, page, pageSize);
            var totalCount = await _taskRepository.GetTotalCountAsync(categoryId, priority, isCompleted, search);

            var taskDtos = tasks.Select(MapToDto).ToList();

            // ← This MUST return PaginatedResult<TaskDto>, not ServiceResult!
            return new PaginatedResult<TaskDto>(taskDtos, page, pageSize, totalCount);
        }

        public async Task<ServiceResult<TaskDto>> GetTaskByIdAsync(Guid id)
        {
            try
            {
                var task = await _taskRepository.GetByIdAsync(id);
                if (task == null)
                {
                    return ServiceResult<TaskDto>.Failure("Task not found");
                }

                var taskDto = MapToDto(task);
                return ServiceResult<TaskDto>.Success(taskDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting task with ID {TaskId}", id);
                return ServiceResult<TaskDto>.Failure("An error occurred while retrieving the task");
            }
        }

        public async Task<ServiceResult<TaskDto>> CreateTaskAsync(CreateTaskDto createTaskDto)
        {
            try
            {
                _logger.LogInformation("Creating new task: {Title}", createTaskDto.Title);

                // Validate category exists
                if (!await _categoryRepository.ExistsAsync(createTaskDto.CategoryId))
                {
                    return ServiceResult<TaskDto>.Failure($"Category with ID {createTaskDto.CategoryId} does not exist");
                }

                var task = new Domain.Entities.Task
                {
                    Id = Guid.NewGuid(),
                    Title = createTaskDto.Title,
                    Description = createTaskDto.Description,
                    Priority = createTaskDto.Priority,
                    CategoryId = createTaskDto.CategoryId,
                    DueDate = createTaskDto.DueDate,
                    IsCompleted = false,
                    CreatedDate = DateTime.UtcNow,
                   
                };

                var createdTask = await _taskRepository.CreateAsync(task);
                var taskDto = MapToDto(createdTask);

                return ServiceResult<TaskDto>.Success(taskDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating task");
                return ServiceResult<TaskDto>.Failure("An error occurred while creating the task");
            }
        }

        public async Task<ServiceResult<TaskDto>> UpdateTaskAsync(Guid id, UpdateTaskDto updateTaskDto)
        {
            try
            {
                var existingTask = await _taskRepository.GetByIdAsync(id);
                if (existingTask == null)
                {
                    return ServiceResult<TaskDto>.Failure("Task not found");
                }

                // Validate category exists if it's being changed
                if (updateTaskDto.CategoryId.HasValue &&
                    !await _categoryRepository.ExistsAsync(updateTaskDto.CategoryId.Value))
                {
                    return ServiceResult<TaskDto>.Failure($"Category with ID {updateTaskDto.CategoryId} does not exist");
                }

                // Update properties (only update if values are provided)
                if (!string.IsNullOrEmpty(updateTaskDto.Title))
                    existingTask.Title = updateTaskDto.Title;

                if (updateTaskDto.Description != null)
                    existingTask.Description = updateTaskDto.Description;

                if (updateTaskDto.Priority.HasValue)
                    existingTask.Priority = updateTaskDto.Priority.Value;

                if (updateTaskDto.CategoryId.HasValue)
                    existingTask.CategoryId = updateTaskDto.CategoryId.Value;

                if (updateTaskDto.DueDate.HasValue)
                    existingTask.DueDate = updateTaskDto.DueDate.Value;

                if (updateTaskDto.IsCompleted.HasValue)
                    existingTask.IsCompleted = updateTaskDto.IsCompleted.Value;

                var updatedTask = await _taskRepository.UpdateAsync(existingTask);
                if (updatedTask == null)
                {
                    return ServiceResult<TaskDto>.Failure("Failed to update task");
                }

                var taskDto = MapToDto(updatedTask);
                return ServiceResult<TaskDto>.Success(taskDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating task with ID {TaskId}", id);
                return ServiceResult<TaskDto>.Failure("An error occurred while updating the task");
            }
        }
        public async Task<ServiceResult<bool>> DeleteTaskAsync(Guid id)
        {
            try
            {
                var exists = await _taskRepository.ExistsAsync(id);
                if (!exists)
                {
                    return ServiceResult<bool>.Failure("Task not found");
                }

                var deleted = await _taskRepository.DeleteAsync(id);
                return ServiceResult<bool>.Success(deleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting task with ID {TaskId}", id);
                return ServiceResult<bool>.Failure("An error occurred while deleting the task");
            }
        }

        public async Task<ServiceResult<TaskDto>> MarkAsCompletedAsync(Guid id)
        {
            try
            {
                var existingTask = await _taskRepository.GetByIdAsync(id);
                if (existingTask == null)
                {
                    return ServiceResult<TaskDto>.Failure("Task not found");
                }

                existingTask.IsCompleted = true;
               

                var updatedTask = await _taskRepository.UpdateAsync(existingTask);
                if (updatedTask == null)
                {
                    return ServiceResult<TaskDto>.Failure("Failed to mark task as completed");
                }

                var taskDto = MapToDto(updatedTask);
                return ServiceResult<TaskDto>.Success(taskDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while marking task as completed with ID {TaskId}", id);
                return ServiceResult<TaskDto>.Failure("An error occurred while marking the task as completed");
            }
        }

        private static TaskDto MapToDto(Domain.Entities.Task task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Priority = task.Priority,
                CategoryId = task.CategoryId,
                DueDate = task.DueDate,
                IsCompleted = task.IsCompleted,
                CreatedDate = task.CreatedDate,
                
            };
        }
    }
}
