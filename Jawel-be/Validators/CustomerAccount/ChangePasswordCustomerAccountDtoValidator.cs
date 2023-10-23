using FluentValidation;
using Jawel_be.Dtos.CustomerAccount;

namespace Jawel_be.Validators.CustomerAccount
{
    public class ChangePasswordCustomerAccountDtoValidator : AbstractValidator<ChangePasswordCustomerAccountDto>
    {
        public ChangePasswordCustomerAccountDtoValidator()
        {
            RuleFor(x => x.CurrentPassword).NotNull().NotEmpty().MinimumLength(6);
            RuleFor(x => x.NewPassword).NotNull().NotEmpty().MinimumLength(6);
        }
    }
}
