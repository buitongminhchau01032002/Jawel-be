using FluentValidation;
using Jawel_be.Dtos.CustomerAccount;

namespace Jawel_be.Validators.CustomerAccount
{
    public class LoginCustomerAccountDtoValidator : AbstractValidator<LoginCustomerAccountDto>
    {
        public LoginCustomerAccountDtoValidator()
        {
            RuleFor(x => x.Phone).NotNull().NotEmpty();
            RuleFor(x => x.Password).NotNull().NotEmpty().MinimumLength(6);
        }
    }
}
