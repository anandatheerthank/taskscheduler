using TaskSchedular.Models;

namespace TaskSchedular.Interface
{
    public interface ITasks
    {
        Task<IEnumerable<TaskInfo>> GetAllTasksAsync();
        Task<TaskInfo> GetTaskByIdAsync(int id);
        Task AddTaskAsync(TaskInfo task);
        Task UpdateTaskAsync(TaskInfo task);
        Task DeleteTaskAsync(int id);
    }
}
