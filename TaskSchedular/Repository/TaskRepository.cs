using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using TaskSchedular.Data;
using TaskSchedular.Interface;
using TaskSchedular.Models;

namespace TaskSchedular.Repository
{
    public class TaskRepository : ITasks
    {
        private readonly TaskContext _context;

        public TaskRepository(TaskContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskInfo>> GetAllTasksAsync()
        {
            return await _context.TaskInfos.ToListAsync();
        }

        public async Task<TaskInfo> GetTaskByIdAsync(int id)
        {
            return await _context.TaskInfos.FindAsync(id);
        }

        public async Task AddTaskAsync(TaskInfo task)
        {
            _context.TaskInfos.Add(task);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateTaskAsync(TaskInfo task)
        {
            Console.WriteLine("----11-----");

            TaskInfo item = _context.TaskInfos.Where(w => w.TaskID == task.TaskID).FirstOrDefault();
            item.TaskDescription = task.TaskDescription;
            item.StartDate = task.StartDate;
            item.ExpectedClosureDate = task.ExpectedClosureDate;
            item.CompletionStatus = task.CompletionStatus;
            item.AssignedTo = task.AssignedTo;

            //_context.Attach(task);
            _context.Entry(item).State = EntityState.Modified;
            Task.Delay(2000).Wait();
            await _context.SaveChangesAsync();
        }


        public async Task DeleteTaskAsync(int id)
        {
            var task = await _context.TaskInfos.FindAsync(id);
            if (task != null)
            {
                _context.TaskInfos.Remove(task);
                await _context.SaveChangesAsync();
            }
        }
    }
}
