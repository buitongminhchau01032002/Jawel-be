using FluentValidation;
using Jawel_be.Dtos.Product;

namespace Jawel_be.Validators.Product
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(p => p.Name).NotNull().NotEmpty();
            RuleFor(p => p.Description).NotEmpty();
            RuleFor(p => p.Price).NotNull().GreaterThan(0);
            RuleFor(p => p.Cost).NotNull().GreaterThan(0);
            RuleFor(p => p.Quantity).NotNull().GreaterThan(0);
            RuleFor(p => p.CategoryId).NotNull();
        }
    }
}
