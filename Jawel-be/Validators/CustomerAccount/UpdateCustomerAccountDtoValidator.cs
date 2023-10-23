using FluentValidation;
using Jawel_be.Dtos.CustomerAccount;
using Jawel_be.Utils;

namespace Jawel_be.Validators.CustomerAccount
{
    public class UpdateCustomerAccountDtoValidator : AbstractValidator<UpdateCustomerAccountDto>
    {
        public UpdateCustomerAccountDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Gender).In("Male", "Female");
            RuleFor(x => x.Address).NotEmpty();
        }
    }
}
