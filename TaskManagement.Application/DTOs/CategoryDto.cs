using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Application.DTOs
{
    public class CategoryDto
    {
        public Guid id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int TaskCount { get; set; }
        public int ActiveTasksCount { get; set; }
        public int OverdueTasksCount { get; set; }
    }
}
