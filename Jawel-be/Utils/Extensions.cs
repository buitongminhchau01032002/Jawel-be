using Jawel_be.Dtos.Category;
using Jawel_be.Dtos.Product;
using Jawel_be.Models;

namespace Jawel_be.Utils
{
    public static class Extensions
    {
        public static CategoryDto AsDto(this Category category)
        {
            return new CategoryDto()
            {
                Id = category.Id,
                Name = category.Name,
            };
        }

        public static ProductDto AsDto(this Product product)
        {
            return new ProductDto()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Image = product.Image,
                Cost = product.Cost,
                Price = product.Price,
                Quantity = product.Quantity,
                Category = product.Category?.AsDto(),
            };
        }
    }
}
