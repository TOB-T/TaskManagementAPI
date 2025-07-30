using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Enums;

namespace TaskManagement.APIx.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        /// <summary>
        /// Get all tasks with optional filtering and pagination
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetTasks(
            [FromQuery] Guid? categoryId = null,
            [FromQuery] Priority? priority = null,
            [FromQuery] bool? isCompleted = null,
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var result = await _taskService.GetAllTasksAsync(
                    categoryId, priority, isCompleted, search, page, pageSize);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks");
                return StatusCode(500, "An error occurred while retrieving tasks");
            }
        }

        /// <summary>
        /// Get a specific task by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTask(Guid id)
        {
            var result = await _taskService.GetTaskByIdAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Create a new task
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _taskService.CreateTaskAsync(createTaskDto);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return CreatedAtAction(nameof(GetTask), new { id = result.Data!.Id }, result.Data);
        }

        /// <summary>
        /// Update an existing task
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TaskDto>> UpdateTask(Guid id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _taskService.UpdateTaskAsync(id, updateTaskDto);

            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask(Guid id)
        {
            var result = await _taskService.DeleteTaskAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }

            return NoContent();
        }

        /// <summary>
        /// Mark a task as completed
        /// </summary>
        [HttpPatch("{id}/complete")]
        public async Task<ActionResult<TaskDto>> MarkAsCompleted(Guid id)
        {
            var result = await _taskService.MarkAsCompletedAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Get overdue tasks
        /// </summary>
        //[HttpGet("overdue")]
        //public async Task<ActionResult> GetOverdueTasks()
        //{
        //    var result = await _taskService.GetOverdueTasksAsync();

        //    if (!result.IsSuccess)
        //    {
        //        return StatusCode(500, result.ErrorMessage);
        //    }

        //    return Ok(result.Data);
        //}



        /// <summary>
        /// Get tasks due soon
        /// </summary>
        //[HttpGet("due-soon")]
        //public async Task<ActionResult> GetTasksDueSoon([FromQuery] int days = 3)
        //{
        //    var result = await _taskService.GetTasksDueSoonAsync(days);

        //    if (!result.IsSuccess)
        //    {
        //        return StatusCode(500, result.ErrorMessage);
        //    }

        //    return Ok(result.Data);
        //}
    }
}
