using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThoiGian.Models.Entities
{
    public class DistractionItem
    {
        [Key]
        public int Id { get; set; }

        public int? TaskItemId { get; set; }
        [ForeignKey(nameof(TaskItemId))]
        public virtual TaskItem? TaskItem { get; set; }

        [Required]
        [StringLength(500)]
        public string Content { get; set; } = string.Empty;

        [StringLength(50)]
        public string Tag { get; set; } = "#Ý_tưởng";

        public string UserId { get; set; } = "guest_user";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}