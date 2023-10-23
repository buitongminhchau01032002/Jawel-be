using FluentValidation;
using Jawel_be.Dtos.Category;

namespace Jawel_be.Validators.Category
{
    public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryDtoValidator()
        {
            RuleFor(c => c.Name).NotNull().NotEmpty().MaximumLength(100);
        }
    }
}
