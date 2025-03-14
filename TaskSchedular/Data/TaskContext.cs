using Microsoft.EntityFrameworkCore;
using TaskSchedular.Models;

namespace TaskSchedular.Data
{
    public class TaskContext : DbContext
    {
        public TaskContext(DbContextOptions<TaskContext> options) : base(options) { }

        //Implement the Code here
        public DbSet<TaskInfo> TaskInfos { get; set; }
    }
}
