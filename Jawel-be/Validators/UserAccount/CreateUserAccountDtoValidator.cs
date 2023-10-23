using FluentValidation;
using Jawel_be.Dtos.UserAccount;
using Jawel_be.Utils;

namespace Jawel_be.Validators.UserAccount
{
    public class CreateUserAccountDtoValidator : AbstractValidator<CreateUserAccountDto>
    {
        public CreateUserAccountDtoValidator()
        {
            RuleFor(x => x.Username).NotNull().NotEmpty();
            RuleFor(x => x.Password).NotNull().MinimumLength(6);
            RuleFor(x => x.Name).NotNull().NotEmpty();
            RuleFor(x => x.Gender).NotNull().In("Male", "Female");
            RuleFor(x => x.Role).NotNull().In("Admin", "Employee");
            RuleFor(x => x.Status).NotNull().In("Active", "Inactive");
        }
    }
}
