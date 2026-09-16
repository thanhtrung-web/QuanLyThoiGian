using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyThoiGian.Data;
using QuanLyThoiGian.Models.Entities;

namespace QuanLyThoiGian.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllCategoriesAsync(string userId = "guest_user")
        {
            await SeedDefaultCategoriesAsync(userId);
            return await _context.Categories
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.Id)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id, string userId = "guest_user")
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            category.CreatedAt = DateTime.Now;
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            var existing = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == category.Id && c.UserId == category.UserId);
            if (existing == null) return false;

            existing.Name = category.Name;
            existing.ColorHex = category.ColorHex;
            existing.IconName = category.IconName;
            existing.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id, string userId = "guest_user")
        {
            var existing = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (existing == null) return false;

            existing.IsDeleted = true;
            existing.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task SeedDefaultCategoriesAsync(string userId = "guest_user")
        {
            if (!await _context.Categories.AnyAsync(c => c.UserId == userId))
            {
                var defaults = new List<Category>
                {
                    new Category { Name = "Công việc", ColorHex = "#8083ff", IconName = "work", UserId = userId },
                    new Category { Name = "Học tập", ColorHex = "#4edea3", IconName = "school", UserId = userId },
                    new Category { Name = "Sức khỏe", ColorHex = "#00a572", IconName = "fitness_center", UserId = userId },
                    new Category { Name = "Cá nhân", ColorHex = "#ffb2b7", IconName = "person", UserId = userId },
                    new Category { Name = "Tài chính", ColorHex = "#38bdf8", IconName = "payments", UserId = userId }
                };

                _context.Categories.AddRange(defaults);
                await _context.SaveChangesAsync();
            }
        }
    }
}
