using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuanLyThoiGian.Models.Entities;
using QuanLyThoiGian.Models.Enums;
using QuanLyThoiGian.Services;

namespace QuanLyThoiGian.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly ICategoryService _categoryService;

        public TaskController(ITaskService taskService, ICategoryService categoryService)
        {
            _taskService = taskService;
            _categoryService = categoryService;
        }

        // GET: /Task hoặc GET: /
        public async Task<IActionResult> Index(string? filter = "today", int? categoryId = null, string? search = null)
        {
            ViewBag.ActiveFilter = filter ?? "today";
            ViewBag.ActiveCategoryId = categoryId;
            ViewBag.SearchQuery = search;
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();

            var tasks = await _taskService.GetTasksAsync("guest_user", filter, categoryId, search);
            return View(tasks);
        }

        // POST: /Task/Create
        [HttpPost]
        public async Task<IActionResult> Create(TaskItem task)
        {
            if (ModelState.IsValid)
            {
                task.UserId = "guest_user";
                await _taskService.CreateTaskAsync(task);
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: /Task/ToggleStatus
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = await _taskService.ToggleTaskStatusAsync(id, "guest_user");
            return Json(new { success = result });
        }

        // GET: /Task/GetDetail?id=1
        [HttpGet]
        public async Task<IActionResult> GetDetail(int id)
        {
            var task = await _taskService.GetTaskDetailAsync(id, "guest_user");
            if (task == null) return NotFound();

            return Json(new
            {
                id = task.Id,
                title = task.Title,
                description = task.Description,
                deadline = task.Deadline?.ToString("yyyy-MM-ddTHH:mm"),
                priority = (int)task.Priority,
                status = (int)task.Status,
                estimatedPomodoros = task.EstimatedPomodoros,
                completedPomodoros = task.CompletedPomodoros,
                categoryId = task.CategoryId,
                categoryName = task.Category?.Name ?? "Chưa phân loại",
                subtasks = task.Subtasks.Select(s => new
                {
                    id = s.Id,
                    title = s.Title,
                    isDone = s.IsDone
                }),
                distractions = task.Distractions.Select(d => new
                {
                    id = d.Id,
                    content = d.Content,
                    tag = d.Tag,
                    time = d.CreatedAt.ToString("HH:mm")
                })
            });
        }

        // POST: /Task/Update
        [HttpPost]
        public async Task<IActionResult> Update(TaskItem task)
        {
            task.UserId = "guest_user";
            var result = await _taskService.UpdateTaskAsync(task);
            return Json(new { success = result });
        }

        // POST: /Task/AddSubtask
        [HttpPost]
        public async Task<IActionResult> AddSubtask(int taskId, string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return BadRequest();
            var subtask = await _taskService.AddSubtaskAsync(taskId, title.Trim());
            return Json(new { success = true, subtask = new { id = subtask.Id, title = subtask.Title, isDone = subtask.IsDone } });
        }

        // POST: /Task/ToggleSubtask
        [HttpPost]
        public async Task<IActionResult> ToggleSubtask(int subtaskId)
        {
            var result = await _taskService.ToggleSubtaskAsync(subtaskId);
            return Json(new { success = result });
        }

        // POST: /Task/DeleteSubtask
        [HttpPost]
        public async Task<IActionResult> DeleteSubtask(int subtaskId)
        {
            var result = await _taskService.DeleteSubtaskAsync(subtaskId);
            return Json(new { success = result });
        }

        // POST: /Task/Repeat
        [HttpPost]
        public async Task<IActionResult> Repeat(int taskId, int additionalPomodoros = 1)
        {
            var result = await _taskService.RepeatTaskAsync(taskId, additionalPomodoros, "guest_user");
            return Json(new { success = result });
        }

        // POST: /Task/Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _taskService.DeleteTaskAsync(id, "guest_user");
            return Json(new { success = result });
        }
    }
}
