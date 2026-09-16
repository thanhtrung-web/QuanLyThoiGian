using Microsoft.EntityFrameworkCore;
using QuanLyThoiGian.Models.Entities;

namespace QuanLyThoiGian.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<TimeEntry> TimeEntries { get; set; }
        public DbSet<TaskSubtask> Subtasks { get; set; }
        public DbSet<DistractionItem> Distractions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình Soft Delete Filter
            modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<TaskItem>().HasQueryFilter(t => !t.IsDeleted);

            // Quan hệ Category - Tasks
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tasks)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Quan hệ Task - TimeEntries
            modelBuilder.Entity<TimeEntry>()
                .HasOne(te => te.TaskItem)
                .WithMany(t => t.TimeEntries)
                .HasForeignKey(te => te.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ Task - Subtasks
            modelBuilder.Entity<TaskSubtask>()
                .HasOne(s => s.TaskItem)
                .WithMany(t => t.Subtasks)
                .HasForeignKey(s => s.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ Task - Distractions
            modelBuilder.Entity<DistractionItem>()
                .HasOne(d => d.TaskItem)
                .WithMany(t => t.Distractions)
                .HasForeignKey(d => d.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
