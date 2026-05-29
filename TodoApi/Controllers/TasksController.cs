using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly TodoContext _context;

    public TasksController(TodoContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
    {
        var tasks = await _context.Tasks
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<ActionResult<TaskItem>> CreateTask([FromBody] CreateTaskDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            return BadRequest("Title is required");
        }

        var task = new TaskItem
        {
            Title = dto.Title.Trim(),
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTasks), new { id = task.Id }, task);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto dto)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(dto.Title))
        {
            task.Title = dto.Title.Trim();
        }

        if (dto.IsCompleted.HasValue)
        {
            task.IsCompleted = dto.IsCompleted.Value;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;
}

public class UpdateTaskDto
{
    public string? Title { get; set; }
    public bool? IsCompleted { get; set; }
}
