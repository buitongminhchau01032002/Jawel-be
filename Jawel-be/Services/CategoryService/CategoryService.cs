using Jawel_be.Contexts;
using Jawel_be.Dtos.Category;
using Jawel_be.Exceptions;
using Jawel_be.Models;
using Microsoft.EntityFrameworkCore;

namespace Jawel_be.Services.CategoryService
{
    public class CategoryService : ICategoryService
    {
        private readonly DataContext _dbContext;

        public CategoryService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _dbContext.Categories
                .Where(c => c.DeletedAt == null)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _dbContext.Categories
                .Where(c => c.Id == id && c.DeletedAt == null)
                .FirstOrDefaultAsync();
        }

        public async Task<Category> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            var newCategory = new Category { Name = createCategoryDto.Name };
            _dbContext.Categories.Add(newCategory);
            await _dbContext.SaveChangesAsync();
            return newCategory;
        }

        public async Task<Category> UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto)
        {
            var existingCategory = await _dbContext.Categories
                .Where(c => c.Id == id && c.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (existingCategory != null)
            {
                if (updateCategoryDto.Name != null)
                {
                    existingCategory.Name = updateCategoryDto.Name;
                }
                await _dbContext.SaveChangesAsync();
                return existingCategory;
            }
            else
            {
                throw new EntityNotFoundException();
            }
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _dbContext.Categories
                .Where(c => c.Id == id && c.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (category != null)
            {
                category.DeletedAt = DateTime.Now;
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new EntityNotFoundException();
            }
        }
    }

}
