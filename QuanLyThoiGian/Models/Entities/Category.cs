using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThoiGian.Models.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(20)]
        public string ColorHex { get; set; } = "#6366F1"; // Mã màu hiển thị (VD: #6366F1)

        [StringLength(50)]
        public string IconName { get; set; } = "bi-tag"; // Icon bootstrap

        public string UserId { get; set; } = "guest_user"; // Dành cho đồng bộ & auth sau này

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}