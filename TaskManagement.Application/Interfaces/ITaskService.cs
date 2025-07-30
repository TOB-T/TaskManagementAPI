using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Application.Common;
using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Interfaces
{
    public interface ITaskService
    {
        Task<PaginatedResult<TaskDto>> GetAllTasksAsync(
            Guid? categoryId, Priority? priority, bool? isCompleted,
            string? search, int page, int pageSize);

        Task<ServiceResult<TaskDto>> GetTaskByIdAsync(Guid id);
        Task<ServiceResult<TaskDto>> CreateTaskAsync(CreateTaskDto createTaskDto);
        Task<ServiceResult<TaskDto>> UpdateTaskAsync(Guid id, UpdateTaskDto updateTaskDto);
        Task<ServiceResult<bool>> DeleteTaskAsync(Guid id);
        Task<ServiceResult<TaskDto>> MarkAsCompletedAsync(Guid id);
    }
}
