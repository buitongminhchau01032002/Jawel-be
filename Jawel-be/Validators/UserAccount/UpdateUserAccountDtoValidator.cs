using FluentValidation;
using Jawel_be.Dtos.UserAccount;
using Jawel_be.Utils;

namespace Jawel_be.Validators.UserAccount
{
    public class UpdateUserAccountDtoValidator : AbstractValidator<UpdateUserAccountDto>
    {
        public UpdateUserAccountDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Gender).In("Male", "Female");
            RuleFor(x => x.Role).In("Admin", "Employee");
            RuleFor(x => x.Status).In("Active", "Inactive");
        }
    }
}
