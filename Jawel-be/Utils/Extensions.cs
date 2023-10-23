using FluentValidation;
using Jawel_be.Dtos.Category;
using Jawel_be.Dtos.Product;
using Jawel_be.Dtos.UserAccount;
using Jawel_be.Models;

namespace Jawel_be.Utils
{
    public static class Extensions
    {
        public static IRuleBuilderOptions<T, TProperty> In<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, params TProperty[] validOptions)
        {
            string formatted;
            if (validOptions == null || validOptions.Length == 0)
            {
                throw new ArgumentException("At least one valid option is expected", nameof(validOptions));
            }
            else if (validOptions.Length == 1)
            {
                formatted = validOptions[0].ToString();
            }
            else
            {
                // format like: option1, option2 or option3
                formatted = $"{string.Join(", ", validOptions.Select(vo => vo.ToString()).ToArray(), 0, validOptions.Length - 1)} or {validOptions.Last()}";
            }

            return ruleBuilder
                .Must(validOptions.Contains)
                .WithMessage($"{{PropertyName}} must be one of these values: {formatted}");
        }

        public static CategoryDto AsDto(this Category category)
        {
            return new CategoryDto()
            {
                Id = category.Id,
                Name = category.Name,
            };
        }

        public static ProductDto AsDto(this Product product)
        {
            return new ProductDto()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Image = product.Image,
                Cost = product.Cost,
                Price = product.Price,
                Quantity = product.Quantity,
                Category = product.Category?.AsDto(),
            };
        }
        public static UserAccountDto AsDto(this UserAccount userAccount)
        {
            return new UserAccountDto()
            {
                Id = userAccount.Id,
                Username = userAccount.Username,
                Name = userAccount.Name,
                Gender = userAccount.Gender,
                Avatar = userAccount.Avatar,
                Role = userAccount.Role,
                Status = userAccount.Status,
            };
        }

    }
}
