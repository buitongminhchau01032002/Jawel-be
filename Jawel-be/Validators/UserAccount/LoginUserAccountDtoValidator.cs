using FluentValidation;
using Jawel_be.Dtos.UserAccount;

namespace Jawel_be.Validators.UserAccount
{
    public class LoginUserAccountDtoValidator : AbstractValidator<LoginUserAccountDto>
    {
        public LoginUserAccountDtoValidator()
        {
            RuleFor(x => x.Username).NotNull().NotEmpty();
            RuleFor(x => x.Password).NotNull().NotEmpty().MinimumLength(6);
        }
    }
}
