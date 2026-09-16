using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuanLyThoiGian.Models.Enums;

namespace QuanLyThoiGian.Models.Entities
{
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tiêu đề công việc không được để trống")]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime? Deadline { get; set; }

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public TaskItemStatus Status { get; set; } = TaskItemStatus.ToDo;

        // Ước lượng Pomodoro
        public int EstimatedPomodoros { get; set; } = 1;
        public int CompletedPomodoros { get; set; } = 0;

        // Liên kết Category
        public int? CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public virtual Category? Category { get; set; }

        public string UserId { get; set; } = "guest_user";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
        public virtual ICollection<TaskSubtask> Subtasks { get; set; } = new List<TaskSubtask>();
        public virtual ICollection<DistractionItem> Distractions { get; set; } = new List<DistractionItem>();
    }
}