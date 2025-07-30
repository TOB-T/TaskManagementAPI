using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Infrastructure.Data
{
    public class DataSeeder
    {
        public static void SeedData(TaskContext context)
        {
            // Seed Categories
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new() { Name = "Work", Color = "#FF5722" },
                    new() { Name = "Personal", Color = "#2196F3" },
                    new() { Name = "Shopping", Color = "#4CAF50" },
                    new() { Name = "Health", Color = "#9C27B0" },
                    new() { Name = "Finance", Color = "#FF9800" }
                };

                context.Categories.AddRange(categories);
                context.SaveChanges();
            }

            // Seed Tasks
            if (!context.Tasks.Any())
            {
                // Retrieve seeded categories
                var workCategory = context.Categories.First(c => c.Name == "Work");
                var personalCategory = context.Categories.First(c => c.Name == "Personal");
                var shoppingCategory = context.Categories.First(c => c.Name == "Shopping");
                var healthCategory = context.Categories.First(c => c.Name == "Health");
                var financeCategory = context.Categories.First(c => c.Name == "Finance");

                var tasks = new List<Domain.Entities.Task>
                {
                    new()
                    {
                        Title = "Complete project documentation",
                        Description = "Write comprehensive documentation for the new API",
                        Priority = Priority.High,
                        CategoryId = workCategory.Id,
                        DueDate = DateTime.UtcNow.AddDays(3)
                    },
                    new()
                    {
                        Title = "Review code changes",
                        Description = "Review pending pull requests",
                        Priority = Priority.Medium,
                        CategoryId = workCategory.Id,
                        DueDate = DateTime.UtcNow.AddDays(1)
                    },
                    new()
                    {
                        Title = "Book dentist appointment",
                        Description = "Schedule routine dental checkup",
                        Priority = Priority.Low,
                        CategoryId = healthCategory.Id,
                        IsCompleted = true
                    },
                    new()
                    {
                        Title = "Plan weekend trip",
                        Description = "Research destinations and book accommodations",
                        Priority = Priority.Medium,
                        CategoryId = personalCategory.Id,
                        DueDate = DateTime.UtcNow.AddDays(7)
                    },
                    new()
                    {
                        Title = "Grocery shopping",
                        Description = "Buy ingredients for weekly meals",
                        Priority = Priority.Low,
                        CategoryId = shoppingCategory.Id
                    },
                    new()
                    {
                        Title = "Pay credit card bill",
                        Description = "Ensure payment is made before due date",
                        Priority = Priority.High,
                        CategoryId = financeCategory.Id,
                        DueDate = DateTime.UtcNow.AddDays(2)
                    }
                };

                context.Tasks.AddRange(tasks);
                context.SaveChanges();
            }
        }
    }
}
