using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{
    public abstract class BaseRepository<T> where T : class
    {
        protected readonly TaskContext Context;
        protected readonly DbSet<T> DbSet;

        protected BaseRepository(TaskContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

       
    }
}
