using Microsoft.EntityFrameworkCore;
using TodoApi.Controllers;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Tests;

public class TasksControllerTests : IDisposable
{
    private readonly TodoContext _context;
    private readonly TasksController _controller;

    public TasksControllerTests()
    {
        var options = new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new TodoContext(options);
        _controller = new TasksController(_context);
    }

    [Fact]
    public async Task GetTasks_ShouldReturnEmptyList_WhenNoneExist()
    {
        var result = await _controller.GetTasks();

        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        var returnedTasks = Assert.IsAssignableFrom<IEnumerable<TaskItem>>(okResult.Value);
        Assert.Empty(returnedTasks);
    }

    [Fact]
    public async Task GetTasks_ShouldReturnAllTasks_WhenExist()
    {
        var task1 = new TaskItem { Title = "Task 1", IsCompleted = false, CreatedAt = DateTime.UtcNow };
        var task2 = new TaskItem { Title = "Task 2", IsCompleted = false, CreatedAt = DateTime.UtcNow.AddSeconds(1) };

        _context.Tasks.AddRange(task1, task2);
        await _context.SaveChangesAsync();

        var result = await _controller.GetTasks();

        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        var returnedTasks = Assert.IsAssignableFrom<IEnumerable<TaskItem>>(okResult.Value);
        Assert.Equal(2, returnedTasks.Count());
    }

    [Fact]
    public async Task GetTasks_ShouldReturnTasksSortedByCreatedAtDesc()
    {
        var earlier = DateTime.UtcNow.AddHours(-1);
        var later = DateTime.UtcNow;

        var task1 = new TaskItem { Title = "Task 1", IsCompleted = false, CreatedAt = earlier };
        var task2 = new TaskItem { Title = "Task 2", IsCompleted = false, CreatedAt = later };

        _context.Tasks.AddRange(task1, task2);
        await _context.SaveChangesAsync();

        var result = await _controller.GetTasks();

        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        var returnedTasks = Assert.IsAssignableFrom<IEnumerable<TaskItem>>(okResult.Value).ToList();

        Assert.Equal(task2.Id, returnedTasks[0].Id);
        Assert.Equal(task1.Id, returnedTasks[1].Id);
    }

    [Fact]
    public async Task CreateTask_ShouldAddTask_WithValidTitle()
    {
        var dto = new CreateTaskDto { Title = "New Task" };

        var result = await _controller.CreateTask(dto);

        var actionResult = Assert.IsType<Microsoft.AspNetCore.Mvc.ActionResult<TaskItem>>(result);
        var createdResult = Assert.IsType<Microsoft.AspNetCore.Mvc.CreatedAtActionResult>(actionResult.Result);
        var returnedTask = Assert.IsType<TaskItem>(createdResult.Value);

        Assert.NotEqual(0, returnedTask.Id);
        Assert.Equal("New Task", returnedTask.Title);
        Assert.False(returnedTask.IsCompleted);

        var taskInDb = await _context.Tasks.FindAsync(returnedTask.Id);
        Assert.NotNull(taskInDb);
        Assert.Equal("New Task", taskInDb.Title);
    }

    [Fact]
    public async Task CreateTask_ShouldReturnBadRequest_WhenTitleIsEmpty()
    {
        var dto = new CreateTaskDto { Title = "" };

        var result = await _controller.CreateTask(dto);

        var actionResult = Assert.IsType<Microsoft.AspNetCore.Mvc.ActionResult<TaskItem>>(result);
        var badResult = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(actionResult.Result);
        Assert.Equal("Title is required", badResult.Value);
    }

    [Fact]
    public async Task CreateTask_ShouldReturnBadRequest_WhenTitleIsNull()
    {
        var dto = new CreateTaskDto { Title = null };

        var result = await _controller.CreateTask(dto);

        var actionResult = Assert.IsType<Microsoft.AspNetCore.Mvc.ActionResult<TaskItem>>(result);
        var badResult = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(actionResult.Result);
        Assert.Equal("Title is required", badResult.Value);
    }

    [Fact]
    public async Task CreateTask_ShouldTrimTitle()
    {
        var dto = new CreateTaskDto { Title = "  Spaced Task  " };

        var result = await _controller.CreateTask(dto);

        var actionResult = Assert.IsType<Microsoft.AspNetCore.Mvc.ActionResult<TaskItem>>(result);
        var createdResult = Assert.IsType<Microsoft.AspNetCore.Mvc.CreatedAtActionResult>(actionResult.Result);
        var returnedTask = Assert.IsType<TaskItem>(createdResult.Value);

        Assert.Equal("Spaced Task", returnedTask.Title);
    }

    [Fact]
    public async Task DeleteTask_ShouldRemoveTask_WhenExists()
    {
        var task = new TaskItem { Title = "Task to Delete", IsCompleted = false, CreatedAt = DateTime.UtcNow };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var result = await _controller.DeleteTask(task.Id);

        Assert.IsType<Microsoft.AspNetCore.Mvc.NoContentResult>(result);

        var deletedTask = await _context.Tasks.FindAsync(task.Id);
        Assert.Null(deletedTask);
    }

    [Fact]
    public async Task DeleteTask_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        var result = await _controller.DeleteTask(999);

        Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateTask_ShouldToggleIsCompleted_WhenExists()
    {
        var task = new TaskItem { Title = "Task to Update", IsCompleted = false, CreatedAt = DateTime.UtcNow };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var dto = new UpdateTaskDto { IsCompleted = true };
        var result = await _controller.UpdateTask(task.Id, dto);

        Assert.IsType<Microsoft.AspNetCore.Mvc.NoContentResult>(result);

        var updatedTask = await _context.Tasks.FindAsync(task.Id);
        Assert.NotNull(updatedTask);
        Assert.True(updatedTask.IsCompleted);
    }

    [Fact]
    public async Task UpdateTask_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        var dto = new UpdateTaskDto { IsCompleted = true };
        var result = await _controller.UpdateTask(999, dto);

        Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
