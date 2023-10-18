using Jawel_be.Dtos.Category;
using Jawel_be.Models;

namespace Jawel_be.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<List<Category>> GetCategories();
        Task<Category?> GetCategoryById(int id);
        Task<Category> CreateCategory(CreateCategoryDto createCategoryDto);
        Task<Category> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto);
        Task DeleteCategory(int id);
    }
}
