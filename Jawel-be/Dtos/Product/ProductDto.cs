using Jawel_be.Dtos.Category;

namespace Jawel_be.Dtos.Product
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public byte[]? Image { get; set; }
        public int Price { get; set; }
        public int Cost { get; set; }
        public int Quantity { get; set; }
        public CategoryDto? Category { get; set; }
    }
}
