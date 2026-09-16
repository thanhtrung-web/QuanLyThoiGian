using System.Collections.Generic;
using System.Threading.Tasks;
using QuanLyThoiGian.Models.Entities;
using QuanLyThoiGian.Models.Enums;

namespace QuanLyThoiGian.Services
{
    public interface ITaskService
    {
        Task<List<TaskItem>> GetTasksAsync(string userId = "guest_user", string? filter = null, int? categoryId = null, string? search = null);
        Task<TaskItem?> GetTaskByIdAsync(int id, string userId = "guest_user");
        Task<TaskItem> CreateTaskAsync(TaskItem task);
        Task<bool> UpdateTaskAsync(TaskItem task);
        Task<bool> DeleteTaskAsync(int id, string userId = "guest_user");
        Task<bool> ToggleTaskStatusAsync(int id, string userId = "guest_user");
        Task<TaskItem?> GetTaskDetailAsync(int id, string userId = "guest_user");
        Task<TaskSubtask> AddSubtaskAsync(int taskId, string title);
        Task<bool> ToggleSubtaskAsync(int subtaskId);
        Task<bool> DeleteSubtaskAsync(int subtaskId);
        Task<bool> RepeatTaskAsync(int id, int additionalPomodoros, string userId = "guest_user");
        Task<DistractionItem> LogDistractionAsync(int? taskId, string content, string tag, string userId = "guest_user");
        Task<List<DistractionItem>> GetDistractionsAsync(int? taskId = null, string userId = "guest_user");
    }
}
