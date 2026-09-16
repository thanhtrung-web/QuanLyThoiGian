using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuanLyThoiGian.Models.Enums;

namespace QuanLyThoiGian.Models.Entities
{
    public class TimeEntry
    {
        [Key]
        public int Id { get; set; }

        public int? TaskItemId { get; set; }
        [ForeignKey(nameof(TaskItemId))]
        public virtual TaskItem? TaskItem { get; set; }

        public PomodoroSessionType SessionType { get; set; } = PomodoroSessionType.Focus;

        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime? EndTime { get; set; }

        public int DurationMinutes { get; set; } = 25;

        public int DistractionCount { get; set; } = 0;

        public bool IsCompleted { get; set; } = false;

        [StringLength(500)]
        public string? Notes { get; set; }

        public string UserId { get; set; } = "guest_user";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}