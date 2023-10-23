namespace Jawel_be.Dtos.CustomerAccount
{
    public class UpdateCustomerAccountDto
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public string? Address { get; set; }
        public byte[]? Avatar { get; set; }
    }
}
