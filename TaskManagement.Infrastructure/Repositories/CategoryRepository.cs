using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Interfaces.Repositories;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{
    public class CategoryRepository :  BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(TaskContext context) : base(context) { }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await Context.Categories
                .Include(c => c.Tasks)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await Context.Categories
                .Include(c => c.Tasks)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await Context.Categories
                .Include(c => c.Tasks)
                .FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await Context.Categories.AnyAsync(c => c.Id == id);
        }
    }
}

