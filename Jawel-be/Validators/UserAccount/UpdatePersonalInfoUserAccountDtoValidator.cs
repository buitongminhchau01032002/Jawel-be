using FluentValidation;
using Jawel_be.Dtos.UserAccount;
using Jawel_be.Utils;

namespace Jawel_be.Validators.UserAccount
{
    public class UpdatePersonalInfoUserAccountDtoValidator : AbstractValidator<UpdatePersonalInfoUserAccountDto>
    {
        public UpdatePersonalInfoUserAccountDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Gender).In("Male", "Female");
        }
    }
}
