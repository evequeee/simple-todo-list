using Moq;
using TodoApi.Controllers;
using TodoApi.Models;
using TodoApi.Services;

namespace TodoApi.Tests;

public class TasksControllerTests
{
    private readonly Mock<ITaskService> _mockTaskService;
    private readonly TasksController _controller;

    public TasksControllerTests()
    {
        _mockTaskService = new Mock<ITaskService>();
        _controller = new TasksController(_mockTaskService.Object);
    }

    [Fact]
    public async Task GetTasks_ShouldReturnEmptyList_WhenNoneExist()
    {
        _mockTaskService.Setup(s => s.GetAllTasksAsync())
            .ReturnsAsync(new List<TaskItem>());

        var result = await _controller.GetTasks();

        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        var returnedTasks = Assert.IsAssignableFrom<IEnumerable<TaskItem>>(okResult.Value);
        Assert.Empty(returnedTasks);
        _mockTaskService.Verify(s => s.GetAllTasksAsync(), Times.Once);
    }

    [Fact]
    public async Task GetTasks_ShouldReturnAllTasks_WhenExist()
    {
        var tasks = new List<TaskItem>
        {
            new TaskItem { Id = 1, Title = "Task 1", IsCompleted = false, CreatedAt = DateTime.UtcNow },
            new TaskItem { Id = 2, Title = "Task 2", IsCompleted = false, CreatedAt = DateTime.UtcNow.AddSeconds(1) }
        };

        _mockTaskService.Setup(s => s.GetAllTasksAsync())
            .ReturnsAsync(tasks);

        var result = await _controller.GetTasks();

        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result.Result);
        var returnedTasks = Assert.IsAssignableFrom<IEnumerable<TaskItem>>(okResult.Value);
        Assert.Equal(2, returnedTasks.Count());
    }

    [Fact]
    public async Task CreateTask_ShouldAddTask_WithValidTitle()
    {
        var task = new TaskItem { Id = 1, Title = "New Task", IsCompleted = false, CreatedAt = DateTime.UtcNow };
        var dto = new CreateTaskDto { Title = "New Task" };

        _mockTaskService.Setup(s => s.CreateTaskAsync("New Task"))
            .ReturnsAsync(task);

        var result = await _controller.CreateTask(dto);

        var actionResult = Assert.IsType<Microsoft.AspNetCore.Mvc.ActionResult<TaskItem>>(result);
        var createdResult = Assert.IsType<Microsoft.AspNetCore.Mvc.CreatedAtActionResult>(actionResult.Result);
        var returnedTask = Assert.IsType<TaskItem>(createdResult.Value);

        Assert.NotEqual(0, returnedTask.Id);
        Assert.Equal("New Task", returnedTask.Title);
        Assert.False(returnedTask.IsCompleted);
        _mockTaskService.Verify(s => s.CreateTaskAsync("New Task"), Times.Once);
    }

    [Fact]
    public async Task CreateTask_ShouldReturnBadRequest_WhenTitleIsEmpty()
    {
        var dto = new CreateTaskDto { Title = "" };

        var result = await _controller.CreateTask(dto);

        var actionResult = Assert.IsType<Microsoft.AspNetCore.Mvc.ActionResult<TaskItem>>(result);
        var badResult = Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestObjectResult>(actionResult.Result);
        Assert.Equal("Title is required", badResult.Value);
        _mockTaskService.Verify(s => s.CreateTaskAsync(It.IsAny<string>()), Times.Never);
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
    public async Task DeleteTask_ShouldRemoveTask_WhenExists()
    {
        _mockTaskService.Setup(s => s.DeleteTaskAsync(1))
            .ReturnsAsync(true);

        var result = await _controller.DeleteTask(1);

        Assert.IsType<Microsoft.AspNetCore.Mvc.NoContentResult>(result);
        _mockTaskService.Verify(s => s.DeleteTaskAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteTask_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        _mockTaskService.Setup(s => s.DeleteTaskAsync(999))
            .ReturnsAsync(false);

        var result = await _controller.DeleteTask(999);

        Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateTask_ShouldToggleIsCompleted_WhenExists()
    {
        var dto = new UpdateTaskDto { IsCompleted = true };
        _mockTaskService.Setup(s => s.UpdateTaskAsync(1, true, null))
            .ReturnsAsync(true);

        var result = await _controller.UpdateTask(1, dto);

        Assert.IsType<Microsoft.AspNetCore.Mvc.NoContentResult>(result);
        _mockTaskService.Verify(s => s.UpdateTaskAsync(1, true, null), Times.Once);
    }

    [Fact]
    public async Task UpdateTask_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        var dto = new UpdateTaskDto { IsCompleted = true };
        _mockTaskService.Setup(s => s.UpdateTaskAsync(999, true, null))
            .ReturnsAsync(false);

        var result = await _controller.UpdateTask(999, dto);

        Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result);
    }
}
