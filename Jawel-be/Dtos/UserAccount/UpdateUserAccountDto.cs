namespace Jawel_be.Dtos.UserAccount
{
    public class UpdateUserAccountDto
    {
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public string? Role { get; set; } // Admin, Employee
        public string? Status { get; set; } // Active, Inactive
    }
}
