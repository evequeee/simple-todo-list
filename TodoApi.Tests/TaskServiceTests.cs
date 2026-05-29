using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Tests;

public class TaskServiceTests : IDisposable
{
    private readonly TodoContext _context;
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        var options = new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new TodoContext(options);
        _service = new TaskService(_context);
    }

    [Fact]
    public async Task GetAllTasksAsync_ShouldReturnTasksSortedByCreatedAtDescending()
    {
        var earlier = new TaskItem { Title = "Earlier", CreatedAt = DateTime.UtcNow.AddHours(-1) };
        var later = new TaskItem { Title = "Later", CreatedAt = DateTime.UtcNow };

        _context.Tasks.AddRange(earlier, later);
        await _context.SaveChangesAsync();

        var tasks = await _service.GetAllTasksAsync();
        var taskList = tasks.ToList();

        Assert.Equal(2, taskList.Count);
        Assert.Equal(later.Id, taskList[0].Id);
        Assert.Equal(earlier.Id, taskList[1].Id);
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldCreateNewTask()
    {
        var task = await _service.CreateTaskAsync("Test Task");

        Assert.NotEqual(0, task.Id);
        Assert.Equal("Test Task", task.Title);
        Assert.False(task.IsCompleted);
        Assert.NotEqual(DateTime.MinValue, task.CreatedAt);

        var dbTask = await _context.Tasks.FindAsync(task.Id);
        Assert.NotNull(dbTask);
        Assert.Equal("Test Task", dbTask.Title);
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldTrimWhitespace()
    {
        var task = await _service.CreateTaskAsync("  Trimmed  ");

        Assert.Equal("Trimmed", task.Title);
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldUpdateIsCompleted()
    {
        var task = new TaskItem { Title = "Test", IsCompleted = false, CreatedAt = DateTime.UtcNow };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var success = await _service.UpdateTaskAsync(task.Id, true, null);

        Assert.True(success);
        var updated = await _context.Tasks.FindAsync(task.Id);
        Assert.True(updated.IsCompleted);
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldUpdateTitle()
    {
        var task = new TaskItem { Title = "Old", IsCompleted = false, CreatedAt = DateTime.UtcNow };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var success = await _service.UpdateTaskAsync(task.Id, null, "New");

        Assert.True(success);
        var updated = await _context.Tasks.FindAsync(task.Id);
        Assert.Equal("New", updated.Title);
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldReturnFalseIfNotFound()
    {
        var success = await _service.UpdateTaskAsync(999, true, null);

        Assert.False(success);
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldRemoveTask()
    {
        var task = new TaskItem { Title = "Delete Me", CreatedAt = DateTime.UtcNow };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var success = await _service.DeleteTaskAsync(task.Id);

        Assert.True(success);
        var deleted = await _context.Tasks.FindAsync(task.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldReturnFalseIfNotFound()
    {
        var success = await _service.DeleteTaskAsync(999);

        Assert.False(success);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
