using FluentValidation;
using Jawel_be.Dtos.Category;

namespace Jawel_be.Validators.Category
{
    public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
    {
        public UpdateCategoryDtoValidator()
        {
            RuleFor(p => p.Name).NotEmpty().MaximumLength(100);
        }
    }
}
