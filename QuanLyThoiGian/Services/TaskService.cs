using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyThoiGian.Data;
using QuanLyThoiGian.Models.Entities;
using QuanLyThoiGian.Models.Enums;

namespace QuanLyThoiGian.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;

        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaskItem>> GetTasksAsync(string userId = "guest_user", string? filter = null, int? categoryId = null, string? search = null)
        {
            var query = _context.Tasks
                .Include(t => t.Category)
                .Where(t => t.UserId == userId);

            // Tìm kiếm theo từ khóa
            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(t => t.Title.ToLower().Contains(s) || (t.Description != null && t.Description.ToLower().Contains(s)));
            }

            // Lọc theo Category
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
            }

            // Lọc theo Chế độ xem nhanh (Today / Upcoming / Completed)
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            if (filter == "today")
            {
                query = query.Where(t => t.Deadline.HasValue && t.Deadline.Value.Date == today);
            }
            else if (filter == "upcoming")
            {
                query = query.Where(t => t.Deadline.HasValue && t.Deadline.Value.Date >= tomorrow);
            }
            else if (filter == "completed")
            {
                query = query.Where(t => t.Status == TaskItemStatus.Done);
            }

            // Sắp xếp mặc định: Việc chưa hoàn thành lên trước, theo Priority cao -> thấp, theo Deadline gần nhất
            return await query
                .OrderBy(t => t.Status == TaskItemStatus.Done ? 1 : 0)
                .ThenByDescending(t => t.Priority)
                .ThenBy(t => t.Deadline)
                .ToListAsync();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int id, string userId = "guest_user")
        {
            return await _context.Tasks
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }

        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            task.CreatedAt = DateTime.Now;
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> UpdateTaskAsync(TaskItem task)
        {
            var existing = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == task.Id && t.UserId == task.UserId);
            if (existing == null) return false;

            existing.Title = task.Title;
            existing.Description = task.Description;
            existing.Deadline = task.Deadline;
            existing.Priority = task.Priority;
            existing.Status = task.Status;
            existing.EstimatedPomodoros = task.EstimatedPomodoros;
            existing.CategoryId = task.CategoryId;
            existing.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTaskAsync(int id, string userId = "guest_user")
        {
            var existing = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (existing == null) return false;

            existing.IsDeleted = true;
            existing.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleTaskStatusAsync(int id, string userId = "guest_user")
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (task == null) return false;

            task.Status = task.Status == TaskItemStatus.Done ? TaskItemStatus.ToDo : TaskItemStatus.Done;
            task.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IncrementPomodoroAsync(int id, string userId = "guest_user")
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (task == null) return false;

            task.CompletedPomodoros++;
            // Nếu đã hoàn thành đủ số phiên ước lượng thì tự động đánh dấu Done
            if (task.CompletedPomodoros >= task.EstimatedPomodoros)
            {
                task.Status = TaskItemStatus.Done;
            }
            task.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TaskItem?> GetTaskDetailAsync(int id, string userId = "guest_user")
        {
            return await _context.Tasks
                .Include(t => t.Category)
                .Include(t => t.Subtasks.OrderBy(s => s.Id))
                .Include(t => t.Distractions.OrderByDescending(d => d.CreatedAt))
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }

        public async Task<TaskSubtask> AddSubtaskAsync(int taskId, string title)
        {
            var subtask = new TaskSubtask
            {
                TaskItemId = taskId,
                Title = title,
                IsDone = false,
                CreatedAt = DateTime.Now
            };
            _context.Subtasks.Add(subtask);
            await _context.SaveChangesAsync();
            return subtask;
        }

        public async Task<bool> ToggleSubtaskAsync(int subtaskId)
        {
            var subtask = await _context.Subtasks.FindAsync(subtaskId);
            if (subtask == null) return false;

            subtask.IsDone = !subtask.IsDone;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSubtaskAsync(int subtaskId)
        {
            var subtask = await _context.Subtasks.FindAsync(subtaskId);
            if (subtask == null) return false;

            _context.Subtasks.Remove(subtask);
            await _context.SaveChangesAsync();
            return true;
        }

        // Logic Mở lại / Lặp lại công việc khi đã Done:
        public async Task<bool> RepeatTaskAsync(int id, int additionalPomodoros, string userId = "guest_user")
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (task == null) return false;

            // Cộng dồn phiên ước lượng và mở lại task
            task.EstimatedPomodoros += Math.Max(1, additionalPomodoros);
            task.Status = TaskItemStatus.InProgress;
            task.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<DistractionItem> LogDistractionAsync(int? taskId, string content, string tag, string userId = "guest_user")
        {
            var item = new DistractionItem
            {
                TaskItemId = taskId,
                Content = content,
                Tag = string.IsNullOrWhiteSpace(tag) ? "#Ý_tưởng" : tag,
                UserId = userId,
                CreatedAt = DateTime.Now
            };
            _context.Distractions.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<List<DistractionItem>> GetDistractionsAsync(int? taskId = null, string userId = "guest_user")
        {
            var query = _context.Distractions.Where(d => d.UserId == userId);
            if (taskId.HasValue)
            {
                query = query.Where(d => d.TaskItemId == taskId.Value);
            }
            return await query.OrderByDescending(d => d.CreatedAt).ToListAsync();
        }
    }
}
