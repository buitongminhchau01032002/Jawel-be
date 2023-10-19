using Jawel_be.Dtos.Product;
using Jawel_be.Models;

namespace Jawel_be.Services.ProductService
{
    public interface IProductService
    {
        Task<List<Product>> GetProducts();
        Task<Product?> GetProductById(int id);
        Task<Product> CreateProduct(CreateProductDto createProductDto);
        Task<Product> UpdateProduct(int id, UpdateProductDto updateProductDto);
        Task DeleteProduct(int id);
    }
}
