using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Interfaces.Repositories;
using Xunit;

namespace TaskManagement.Tests.Application
{
    public class TaskServiceTests
    {

            private static readonly Guid _taskId1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
            private static readonly Guid _taskId2 = Guid.Parse("22222222-2222-2222-2222-222222222222");
            private static readonly Guid _categoryId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

            private readonly Mock<ITaskRepository> _mockTaskRepository;
            private readonly Mock<ICategoryRepository> _mockCategoryRepository;
            private readonly Mock<ILogger<TaskService>> _mockLogger;
            private readonly TaskService _taskService;

            public TaskServiceTests()
            {
                _mockTaskRepository = new Mock<ITaskRepository>();
                _mockCategoryRepository = new Mock<ICategoryRepository>();
                _mockLogger = new Mock<ILogger<TaskService>>();

                _taskService = new TaskService(
                    _mockTaskRepository.Object,
                    _mockCategoryRepository.Object,
                    _mockLogger.Object);
            }

        [Fact]
        public async System.Threading.Tasks.Task GetAllTasksAsync_ReturnsTasksAndCount()
        {
            // Arrange
            var tasks = new List<TaskManagement.Domain.Entities.Task>
    {
        new() { Id = _taskId1, Title = "Test Task 1", Priority = Priority.High, CategoryId = _categoryId, Category = new Category { Id = _categoryId, Name = "Work", Color = "#FF5722" } },
        new() { Id = _taskId2, Title = "Test Task 2", Priority = Priority.Medium, CategoryId = _categoryId, Category = new Category { Id = _categoryId, Name = "Work", Color = "#FF5722" } }
    };

            _mockTaskRepository
                .Setup(r => r.GetAllAsync(null, null, null, null, 1, 10))
                .ReturnsAsync(tasks);
            _mockTaskRepository
                .Setup(r => r.GetTotalCountAsync(null, null, null, null))
                .ReturnsAsync(tasks.Count);

            // Act
            var result = await _taskService.GetAllTasksAsync(null, null, null, null, 1, 10);

            // Assert
            Assert.Equal(2, result.Data.Count());           // ← result.Data (not result.Data.Data)
            Assert.Equal(2, result.Pagination.TotalCount);  // ← result.Pagination (not result.Data.Pagination)
        }

        [Fact]
            public async System.Threading.Tasks.Task CreateTaskAsync_WithValidData_ReturnsCreatedTask()
            {
                // Arrange
                var createTaskDto = new CreateTaskDto
                {
                    Title = "New Task",
                    Description = "Task description",
                    Priority = Priority.High,
                    CategoryId = _categoryId
                };

                var category = new Category { Id = _categoryId, Name = "Work", Color = "#FF5722" };
                var createdTask = new TaskManagement.Domain.Entities.Task
                {
                    Id = _taskId1,
                    Title = createTaskDto.Title,
                    Description = createTaskDto.Description,
                    Priority = createTaskDto.Priority,
                    CategoryId = _categoryId,
                    Category = category,
                    CreatedDate = DateTime.UtcNow
                };

                _mockCategoryRepository
                    .Setup(r => r.ExistsAsync(_categoryId))
                    .ReturnsAsync(true);
                _mockTaskRepository
                    .Setup(r => r.CreateAsync(It.IsAny<TaskManagement.Domain.Entities.Task>()))
                    .ReturnsAsync(createdTask);

                // Act
                var result = await _taskService.CreateTaskAsync(createTaskDto);

                // Assert
                Assert.True(result.IsSuccess);
                Assert.Equal(_taskId1, result.Data.Id);
                Assert.Equal(createTaskDto.Title, result.Data.Title);
                Assert.Equal(createTaskDto.Description, result.Data.Description);
                Assert.Equal(createTaskDto.Priority, result.Data.Priority);
                Assert.Equal(createTaskDto.CategoryId, result.Data.CategoryId);
            }

            [Fact]
            public async System.Threading.Tasks.Task CreateTaskAsync_WithInvalidCategory_ReturnsFailure()
            {
                // Arrange
                var invalidCatId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
                var createTaskDto = new CreateTaskDto
                {
                    Title = "New Task",
                    Priority = Priority.High,
                    CategoryId = invalidCatId
                };

                _mockCategoryRepository
                    .Setup(r => r.ExistsAsync(invalidCatId))
                    .ReturnsAsync(false);

                // Act
                var result = await _taskService.CreateTaskAsync(createTaskDto);

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains($"Category with ID {invalidCatId} does not exist", result.ErrorMessage);
            }

            [Fact]
            public async System.Threading.Tasks.Task GetTaskByIdAsync_WithValidId_ReturnsTask()
            {
                // Arrange
                var task = new TaskManagement.Domain.Entities.Task
                {
                    Id = _taskId1,
                    Title = "Test Task",
                    Priority = Priority.High,
                    CategoryId = _categoryId,
                    Category = new Category { Id = _categoryId, Name = "Work", Color = "#FF5722" }
                };

                _mockTaskRepository
                    .Setup(r => r.GetByIdAsync(_taskId1))
                    .ReturnsAsync(task);

                // Act
                var result = await _taskService.GetTaskByIdAsync(_taskId1);

                // Assert
                Assert.True(result.IsSuccess);
                Assert.Equal(_taskId1, result.Data.Id);
                Assert.Equal("Test Task", result.Data.Title);
            }

            [Fact]
            public async System.Threading.Tasks.Task GetTaskByIdAsync_WithInvalidId_ReturnsFailure()
            {
                // Arrange
                var invalidId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
                _mockTaskRepository
                    .Setup(r => r.GetByIdAsync(invalidId))
                    .ReturnsAsync((TaskManagement.Domain.Entities.Task?)null);

                // Act
                var result = await _taskService.GetTaskByIdAsync(invalidId);

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains($"Task with ID {invalidId} not found", result.ErrorMessage);
            }

            [Fact]
            public async System.Threading.Tasks.Task MarkAsCompletedAsync_WithValidId_ReturnsCompletedTask()
            {
                // Arrange
                var original = new TaskManagement.Domain.Entities.Task
                {
                    Id = _taskId1,
                    Title = "Test Task",
                    Priority = Priority.High,
                    CategoryId = _categoryId,
                    Category = new Category { Id = _categoryId, Name = "Work", Color = "#FF5722" },
                    IsCompleted = false
                };
                var completed = new TaskManagement.Domain.Entities.Task
                {
                    Id = _taskId1,
                    Title = "Test Task",
                    Priority = Priority.High,
                    CategoryId = _categoryId,
                    Category = new Category { Id = _categoryId, Name = "Work", Color = "#FF5722" },
                    IsCompleted = true
                };

                _mockTaskRepository
                    .Setup(r => r.GetByIdAsync(_taskId1))
                    .ReturnsAsync(original);
                _mockTaskRepository
                    .Setup(r => r.UpdateAsync(It.IsAny<TaskManagement.Domain.Entities.Task>()))
                    .ReturnsAsync(completed);

                // Act
                var result = await _taskService.MarkAsCompletedAsync(_taskId1);

                // Assert
                Assert.True(result.IsSuccess);
                Assert.True(result.Data.IsCompleted);
            }

            [Fact]
            public async System.Threading.Tasks.Task DeleteTaskAsync_WithValidId_ReturnsSuccess()
            {
                // Arrange
                _mockTaskRepository
                    .Setup(r => r.DeleteAsync(_taskId1))
                    .ReturnsAsync(true);

                // Act
                var result = await _taskService.DeleteTaskAsync(_taskId1);

                // Assert
                Assert.True(result.IsSuccess);
                Assert.True(result.Data);
            }

            [Fact]
            public async System.Threading.Tasks.Task DeleteTaskAsync_WithInvalidId_ReturnsFailure()
            {
                // Arrange
                var invalidId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
                _mockTaskRepository
                    .Setup(r => r.DeleteAsync(invalidId))
                    .ReturnsAsync(false);

                // Act
                var result = await _taskService.DeleteTaskAsync(invalidId);

                // Assert
                Assert.False(result.IsSuccess);
                Assert.Contains($"Task with ID {invalidId} not found", result.ErrorMessage);
            }
        }
}

