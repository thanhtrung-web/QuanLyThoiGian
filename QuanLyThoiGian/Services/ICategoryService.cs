using System.Collections.Generic;
using System.Threading.Tasks;
using QuanLyThoiGian.Models.Entities;

namespace QuanLyThoiGian.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategoriesAsync(string userId = "guest_user");
        Task<Category?> GetCategoryByIdAsync(int id, string userId = "guest_user");
        Task<Category> CreateCategoryAsync(Category category);
        Task<bool> UpdateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(int id, string userId = "guest_user");
        Task SeedDefaultCategoriesAsync(string userId = "guest_user");
    }
}
