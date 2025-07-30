using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Domain.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendTaskDueReminderAsync(string email, string taskTitle, DateTime dueDate);
        Task SendTaskCompletedNotificationAsync(string email, string taskTitle);
    }
}

