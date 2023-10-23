using FluentValidation;
using Jawel_be.Dtos.Product;

namespace Jawel_be.Validators.Product
{
    public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductDtoValidator()
        {
            RuleFor(p => p.Name).NotEmpty();
            RuleFor(p => p.Description).NotEmpty();
            RuleFor(p => p.Price).GreaterThan(0);
            RuleFor(p => p.Cost).GreaterThan(0);
            RuleFor(p => p.Quantity).GreaterThan(0);
        }
    }
}
