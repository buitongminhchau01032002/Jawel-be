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

        public async Task<List<Category>> GetCategories()
        {
            return await _dbContext.Categories.ToListAsync();
        }

        public async Task<Category?> GetCategoryById(int id)
        {
            return await _dbContext.Categories.FindAsync(id);
        }

        public async Task<Category> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            var newCategory = new Category { Name = createCategoryDto.Name };
            _dbContext.Categories.Add(newCategory);
            await _dbContext.SaveChangesAsync();
            return newCategory;
        }

        public async Task<Category> UpdateCategory(int id, UpdateCategoryDto updateCategoryDto)
        {
            var existingCategory = await _dbContext.Categories.FindAsync(id);

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

        public async Task DeleteCategory(int id)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category != null)
            {
                _dbContext.Categories.Remove(category);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new EntityNotFoundException();
            }
        }
    }

}
