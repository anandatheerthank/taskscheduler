using Microsoft.AspNetCore.Mvc;
using TaskSchedular.Interface;
using TaskSchedular.Models;
using TaskSchedular.Repository;

namespace TaskSchedular.Controllers
{
    //[Route("Tasks")]
    public class TaskController : Controller
    {
        //hhh
        private readonly ITasks _taskRepository;

        public TaskController(ITasks taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<IActionResult> Index()
        {
            var tasks = await _taskRepository.GetAllTasksAsync();
            return View(tasks);
        }

        public IActionResult CreateTask()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskInfo task)
        {
            if (ModelState.IsValid)
            {
                await _taskRepository.AddTaskAsync(task);
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        public async Task<IActionResult> EditTask(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        // POST: Tasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTask(int id, TaskInfo task)
        {
            if (id != task.TaskID)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _taskRepository.UpdateTaskAsync(task);
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        [HttpPost, ActionName("DeleteTask")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _taskRepository.DeleteTaskAsync(id);
            return RedirectToAction(nameof(Index));
            //   return View();

        }
    }

}
