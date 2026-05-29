using TodoApi.Models;

namespace TodoApi.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskItem>> GetAllTasksAsync();
    Task<TaskItem> CreateTaskAsync(string title);
    Task<bool> UpdateTaskAsync(int id, bool? isCompleted, string? title);
    Task<bool> DeleteTaskAsync(int id);
}
