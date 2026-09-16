using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuanLyThoiGian.Services;

namespace QuanLyThoiGian.Controllers
{
    public class FocusController : Controller
    {
        private readonly ITaskService _taskService;

        public FocusController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // GET: /Focus?taskId=1
        public async Task<IActionResult> Index(int? taskId = null)
        {
            var allTasks = await _taskService.GetTasksAsync("guest_user");
            ViewBag.AllTasks = allTasks;

            if (taskId.HasValue)
            {
                var task = await _taskService.GetTaskByIdAsync(taskId.Value, "guest_user");
                ViewBag.ActiveTask = task;
            }
            else if (allTasks.Count > 0)
            {
                ViewBag.ActiveTask = allTasks[0];
            }

            return View();
        }

        // POST: /Focus/CompletePomodoro
        [HttpPost]
        public async Task<IActionResult> CompletePomodoro(int? taskId)
        {
            if (taskId.HasValue)
            {
                await _taskService.IncrementPomodoroAsync(taskId.Value, "guest_user");
            }
            return Json(new { success = true });
        }
    }
}
