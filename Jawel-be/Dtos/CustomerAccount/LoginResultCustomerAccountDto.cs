namespace Jawel_be.Dtos.CustomerAccount
{
    public class LoginResultCustomerAccountDto
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string? Address { get; set; }
        public byte[]? Avatar { get; set; }
        public string Token { get; set; }
    }
}
