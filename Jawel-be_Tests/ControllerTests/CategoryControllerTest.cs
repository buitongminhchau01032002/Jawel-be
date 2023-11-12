using Jawel_be.Controllers;
using Jawel_be.Dtos.Category;
using Jawel_be.Exceptions;
using Jawel_be.Models;
using Jawel_be.Services.CategoryService;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Jawal_beTests.ControllerTests
{
    public class CategoryControllerTest
    {
        private List<Category> _categories;
        private Mock<ICategoryService> _mockService;
        private CategoryController _controller;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _categories = new List<Category>()
            {
                new Category { Id = 1, Name = "Nhẫn" },
                new Category { Id = 2, Name = "Dây chuyền" },
            };

            _mockService = new Mock<ICategoryService>();
            _controller = new CategoryController(_mockService.Object);
        }


        [Test]
        public async Task GetCategories__ReturnOkAndValueIsAllCategories()
        {
            // Arrange
            _mockService.Setup(m => m.GetCategories()).ReturnsAsync(_categories);

            // Act
            var actionResult = await _controller.GetCategories();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            var result = ((OkObjectResult)actionResult).Value as List<CategoryDto>;
            Assert.NotNull(result);
            for (int i = 0; i < result.Count; i++)
            {
                AssertCategory(_categories[i], result[i]);
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task GetCategory_ExistId_ReturnOkAndValueIsCategory(int id)
        {
            // Arrange
            _mockService.Setup(m => m.GetCategoryById(id)).ReturnsAsync(_categories.Find(c => c.Id == id));

            // Act
            var actionResult = await _controller.GetCategory(id);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            var result = ((OkObjectResult)actionResult).Value as CategoryDto;
            Assert.NotNull(result);
            var expectedCategory = _categories.Find(c => c.Id == id);
            AssertCategory(expectedCategory, result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(3)]
        public async Task GetCategory_NotExistId_ReturnNotFound(int id)
        {
            // Arrange
            _mockService.Setup(m => m.GetCategoryById(id)).ReturnsAsync(_categories.Find(c => c.Id == id));

            // Act
            var actionResult = await _controller.GetCategory(id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public async Task CreateCategory_Valid_ReturnOkAndValueIsNewCategory()
        {
            // Arrange
            CreateCategoryDto createCategory = new CreateCategoryDto() { Name = "New category" };
            _mockService.Setup(m => m.CreateCategory(createCategory)).ReturnsAsync(new Category() { Name = createCategory.Name });

            // Act
            var actionResult = await _controller.CreateCategory(createCategory);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(actionResult);
            var result = ((CreatedAtActionResult)actionResult).Value as CategoryDto;
            Assert.NotNull(result);
            AssertCategory(createCategory, result);
        }

        [Test]
        [TestCase("")]
        [TestCase("This length of string longer than 100 character. This length of string longer than 100 character. This length of string longer than 100 character.")]
        public async Task CreateCategory_InvalidName_ReturnBadRequest(string name)
        {
            // Arrange
            CreateCategoryDto createCategory = new CreateCategoryDto() { Name = name };
            _mockService.Setup(m => m.CreateCategory(createCategory)).ReturnsAsync(new Category() { Name = createCategory.Name });

            // Act
            var actionResult = await _controller.CreateCategory(createCategory);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(actionResult);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task UpdateCategory_ExistIdAndValidName_ReturnOkAndValueIsUpdatedCategory(int id)
        {
            // Arrange
            UpdateCategoryDto updateCategoryDto = new UpdateCategoryDto() { Name = "Updated name" };
            var matchedCategory = _categories.Find(c => c.Id == id);
            Category updatedCategory = new Category();
            if (matchedCategory != null)
            {
                updatedCategory = new Category { Id = matchedCategory.Id, Name = updateCategoryDto.Name };
                _mockService.Setup(m => m.UpdateCategory(id, updateCategoryDto)).ReturnsAsync(updatedCategory);
            }

            // Act
            var actionResult = await _controller.UpdateCategory(id, updateCategoryDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            var result = ((OkObjectResult)actionResult).Value as CategoryDto;
            Assert.NotNull(result);
            AssertCategory(updatedCategory, result);
        }

        [Test]
        [TestCase(1, "")]
        [TestCase(2, "This length of string longer than 100 character. This length of string longer than 100 character. This length of string longer than 100 character.")]
        public async Task UpdateCategory_ExistIdAndInvalidName_ReturnBadRequest(int id, string name)
        {
            // Arrange
            UpdateCategoryDto updateCategoryDto = new UpdateCategoryDto() { Name = name };
            var matchedCategory = _categories.Find(c => c.Id == id);
            Category updatedCategory = new Category();
            if (matchedCategory != null)
            {
                updatedCategory = new Category { Id = matchedCategory.Id, Name = updateCategoryDto.Name };
                _mockService.Setup(m => m.UpdateCategory(id, updateCategoryDto)).ReturnsAsync(updatedCategory);
            }

            // Act
            var actionResult = await _controller.UpdateCategory(id, updateCategoryDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(actionResult);
        }

        [Test]
        [TestCase(0)]
        [TestCase(3)]
        public async Task UpdateCategory_NotExistId_ReturnNotFound(int id)
        {
            // Arrange
            UpdateCategoryDto updateCategoryDto = new UpdateCategoryDto() { Name = "Updated name" };
            _mockService.Setup(m => m.UpdateCategory(id, updateCategoryDto)).Throws(new EntityNotFoundException());

            // Act
            var actionResult = await _controller.UpdateCategory(id, updateCategoryDto);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task DeleteCategory_ExistId_ReturnOk(int id)
        {
            // Arrange
            _mockService.Setup(m => m.DeleteCategory(id));

            // Act
            var actionResult = await _controller.DeleteCategory(id);

            // Assert
            Assert.IsInstanceOf<OkResult>(actionResult);
        }

        [Test]
        [TestCase(0)]
        [TestCase(3)]
        public async Task DeleteCategory_NotExistId_ReturnNotFound(int id)
        {
            // Arrange
            _mockService.Setup(m => m.DeleteCategory(id)).Throws(new EntityNotFoundException());

            // Act
            var actionResult = await _controller.DeleteCategory(id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        private void AssertCategory(Category expected, Category actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
        }

        private void AssertCategory(Category expected, CategoryDto actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
        }

        private void AssertCategory(CreateCategoryDto expected, CategoryDto actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
        }
    }
}
