namespace Jawel_be.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public byte[]? Image { get; set; }
        public int Price { get; set; }
        public int Cost { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
