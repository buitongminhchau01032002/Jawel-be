using FluentValidation;
using Jawel_be.Dtos.CustomerAccount;
using Jawel_be.Utils;

namespace Jawel_be.Validators.CustomerAccount
{
    public class CreateCustomerAccountDtoValidator : AbstractValidator<CreateCustomerAccountDto>
    {
        public CreateCustomerAccountDtoValidator()
        {
            //RuleFor(x => x.Phone).NotNull().NotEmpty();
            //RuleFor(x => x.Password).NotNull().MinimumLength(6);
            //RuleFor(x => x.Name).NotNull().NotEmpty();
            //RuleFor(x => x.Gender).NotNull().In("Male", "Female");
            //RuleFor(x => x.Address).NotEmpty();
        }
    }
}
