using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThoiGian.Models.Entities
{
    public class TaskSubtask
    {
        [Key]
        public int Id { get; set; }

        public int TaskItemId { get; set; }
        [ForeignKey(nameof(TaskItemId))]
        public virtual TaskItem? TaskItem { get; set; }

        [Required]
        [StringLength(300)]
        public string Title { get; set; } = string.Empty;

        public bool IsDone { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}