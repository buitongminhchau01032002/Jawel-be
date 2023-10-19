using Jawel_be.Contexts;
using Jawel_be.Dtos.Product;
using Jawel_be.Exceptions;
using Jawel_be.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Jawel_be.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly DataContext _dbContext;

        public ProductService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Product>> GetProducts()
        {
            return await _dbContext.Products.Include(p => p.Category).ToListAsync();
        }

        public async Task<Product?> GetProductById(int id)
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .Where(p => p.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<Product> CreateProduct(CreateProductDto createProductDto)
        {
            var category = await _dbContext.Categories.FindAsync(createProductDto.CategoryId);
            if (category == null)
            {
                throw new EntityNotFoundException();
            }
            var newProduct = new Product 
            { 
                Name = createProductDto.Name,
                Description= createProductDto.Description,
                Image= createProductDto.Image,
                Cost= createProductDto.Cost,
                Price = createProductDto.Price,
                Quantity= createProductDto.Quantity,
                CategoryId = createProductDto.CategoryId,
                Category = category
            };
            _dbContext.Products.Add(newProduct);
            await _dbContext.SaveChangesAsync();
            return newProduct;
        }

        public async Task<Product> UpdateProduct(int id, UpdateProductDto updateProductDto)
        {
            var existingProduct = await _dbContext.Products
                .Include(p => p.Category)
                .Where(p => p.Id == id)
                .SingleOrDefaultAsync();

            if (existingProduct != null)
            {
                if (updateProductDto.Name != null)
                {
                    existingProduct.Name = updateProductDto.Name;
                }
                if (updateProductDto.Description != null)
                {
                    existingProduct.Description = updateProductDto.Description;
                }
                if (updateProductDto.Image != null)
                {
                    existingProduct.Image = updateProductDto.Image;
                }
                if (updateProductDto.Cost != null)
                {
                    existingProduct.Cost = (int)updateProductDto.Cost;
                }
                if (updateProductDto.Price != null)
                {
                    existingProduct.Price = (int)updateProductDto.Price;
                }
                if (updateProductDto.Quantity != null)
                {
                    existingProduct.Quantity = (int)updateProductDto.Quantity;
                }
                if (updateProductDto.CategoryId != null)
                {
                    var category = await _dbContext.Categories.FindAsync(updateProductDto.CategoryId);
                    if (category == null)
                    {
                        throw new EntityNotFoundException();
                    }
                    existingProduct.CategoryId = (int)updateProductDto.CategoryId;
                }
                await _dbContext.SaveChangesAsync();
                return existingProduct;
            }
            else
            {
                throw new EntityNotFoundException();
            }
        }

        public async Task DeleteProduct(int id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product != null)
            {
                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new EntityNotFoundException();
            }
        }
    }
}
