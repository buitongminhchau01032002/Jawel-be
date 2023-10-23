using FluentValidation;
using Jawel_be.Dtos.UserAccount;

namespace Jawel_be.Validators.UserAccount
{
    public class ChangePasswordUserAccountDtoValidator : AbstractValidator<ChangePasswordUserAccountDto>
    {
        public ChangePasswordUserAccountDtoValidator()
        {
            RuleFor(x => x.CurrentPassword).NotNull().NotEmpty().MinimumLength(6);
            RuleFor(x => x.NewPassword).NotNull().NotEmpty().MinimumLength(6);
        }
    }
}
