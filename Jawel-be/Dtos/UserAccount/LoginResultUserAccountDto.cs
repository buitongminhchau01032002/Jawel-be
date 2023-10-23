namespace Jawel_be.Dtos.UserAccount
{
    public class LoginResultUserAccountDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public byte[]? Avatar { get; set; }
        public string Role { get; set; } // Admin, Employee
        public string Token { get; set; }
    }
}
