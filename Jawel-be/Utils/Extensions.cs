using Jawel_be.Dtos.Category;
using Jawel_be.Models;

namespace Jawel_be.Utils
{
    public static class Extensions
    {
        public static CategoryDto AsDto(this Category category)
        {
            return new CategoryDto()
            {
                Id = category.Id,
                Name = category.Name,
            };
        }
    }
}
