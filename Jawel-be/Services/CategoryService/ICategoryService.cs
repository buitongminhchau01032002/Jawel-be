using Jawel_be.Dtos.Category;
using Jawel_be.Models;

namespace Jawel_be.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<List<Category>> GetCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<Category> CreateCategoryAsync(CreateCategoryDto createCategoryDto);
        Task<Category> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto);
        Task DeleteCategoryAsync(int id);
    }
}
