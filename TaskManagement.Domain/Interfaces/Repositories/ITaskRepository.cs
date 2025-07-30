using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Interfaces.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<Entities.Task>> GetAllAsync(
           Guid? categoryId = null,
           Priority? priority = null,
           bool? isCompleted = null,
           string? search = null,
           int page = 1,
           int pageSize = 10);

        Task<Entities.Task?> GetByIdAsync(Guid id);
        Task<Entities.Task> CreateAsync(Entities.Task task);
        Task<Entities.Task?> UpdateAsync(Entities.Task task);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> GetTotalCountAsync(
            Guid? categoryId = null,
            Priority? priority = null,
            bool? isCompleted = null,
            string? search = null);
        Task<IEnumerable<Entities.Task>> GetOverdueTasksAsync();
        Task<IEnumerable<Entities.Task>> GetTasksDueSoonAsync(int days = 3);
    }
}
