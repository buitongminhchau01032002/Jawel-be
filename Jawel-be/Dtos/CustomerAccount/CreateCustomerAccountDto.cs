namespace Jawel_be.Dtos.CustomerAccount
{
    public class CreateCustomerAccountDto
    {
        public string Phone { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string? Address { get; set; }
        public byte[]? Avatar { get; set; }
    }
}
