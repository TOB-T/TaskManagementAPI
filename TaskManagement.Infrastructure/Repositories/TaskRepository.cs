using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.Interfaces.Repositories;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{
    public class TaskRepository : BaseRepository<Domain.Entities.Task>, ITaskRepository
    {
        public TaskRepository(TaskContext context) : base(context) { }

        public async Task<IEnumerable<Domain.Entities.Task>> GetAllAsync(
            Guid? categoryId = null,
            Priority? priority = null,
            bool? isCompleted = null,
            string? search = null,
            int page = 1,
            int pageSize = 10)
        {
            var query = Context.Tasks.Include(t => t.Category).AsQueryable();

            // Apply filters
            if (categoryId.HasValue)
                query = query.Where(t => t.CategoryId == categoryId.Value);

            if (priority.HasValue)
                query = query.Where(t => t.Priority == priority.Value);

            if (isCompleted.HasValue)
                query = query.Where(t => t.IsCompleted == isCompleted.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(t => t.Title.Contains(search) ||
                                   (t.Description != null && t.Description.Contains(search)));

            return await query
                .OrderByDescending(t => t.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Domain.Entities.Task?> GetByIdAsync(Guid id)
        {
            return await Context.Tasks
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Domain.Entities.Task> CreateAsync(Domain.Entities.Task task)
        {
            Context.Tasks.Add(task);
            await Context.SaveChangesAsync();

            // Return with category loaded
            return await GetByIdAsync(task.Id) ?? task;
        }

        public async Task<Domain.Entities.Task?> UpdateAsync(Domain.Entities.Task task)
        {
            Context.Entry(task).State = EntityState.Modified;
            await Context.SaveChangesAsync();
            return await GetByIdAsync(task.Id);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var task = await GetByIdAsync(id);
            if (task == null) return false;

            Context.Tasks.Remove(task);
            await Context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await Context.Tasks.AnyAsync(t => t.Id == id);
        }

        public async Task<int> GetTotalCountAsync(
            Guid? categoryId = null,
            Priority? priority = null,
            bool? isCompleted = null,
            string? search = null)
        {
            var query = Context.Tasks.AsQueryable();

            // Apply same filters as GetAllAsync
            if (categoryId.HasValue)
                query = query.Where(t => t.CategoryId == categoryId.Value);

            if (priority.HasValue)
                query = query.Where(t => t.Priority == priority.Value);

            if (isCompleted.HasValue)
                query = query.Where(t => t.IsCompleted == isCompleted.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(t => t.Title.Contains(search) ||
                                   (t.Description != null && t.Description.Contains(search)));

            return await query.CountAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Task>> GetOverdueTasksAsync()
        {
            return await Context.Tasks
                .Include(t => t.Category)
                .Where(t => t.DueDate.HasValue &&
                           t.DueDate < DateTime.UtcNow &&
                           !t.IsCompleted)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Domain.Entities.Task>> GetTasksDueSoonAsync(int days = 3)
        {
            var dueDate = DateTime.UtcNow.AddDays(days);

            return await Context.Tasks
                .Include(t => t.Category)
                .Where(t => t.DueDate.HasValue &&
                           t.DueDate <= dueDate &&
                           t.DueDate > DateTime.UtcNow &&
                           !t.IsCompleted)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }
    }
}
