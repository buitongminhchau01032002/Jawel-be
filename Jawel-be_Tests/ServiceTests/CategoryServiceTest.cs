using Jawel_be.Contexts;
using Jawel_be.Models;
using Jawel_be.Services.CategoryService;
using Jawel_be.Dtos.Category;
using Microsoft.EntityFrameworkCore;
using Jawel_be.Exceptions;

namespace Jawal_beTests.ServiceTests
{
    public class CategoryServiceTest
    {
        private List<Category> _categories;
        private CategoryService _categoryService;
        private DataContext _dbContext;
        private DataContext _dbContextTest;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _categories = new List<Category>()
            {
                new Category { Id = 1, Name = "Nhẫn" },
                new Category { Id = 2, Name = "Dây chuyền" },
            };
        }

        [SetUp]
        public void SetUp()
        {
            // Setup test db
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "testDB")
                .Options;
            _dbContext = new DataContext(options);
            _dbContextTest = new DataContext(options);
            _dbContext.Database.EnsureCreated();
            foreach (var category in _categories)
            {
                _dbContext.Categories.Add(category);
            }
            _dbContext.SaveChanges();

            // Setup service
            _categoryService = new CategoryService(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
        }

        [Test]
        public async Task GetCategories__ReturnAllCategories()
        {
            // Arrange

            // Act
            var result = await _categoryService.GetCategories();

            // Assert
            Assert.AreEqual(_categories.Count, result.Count);
            for (int i = 0; i < result.Count; i++)
            {
                AssertCategory(_categories[i], result[i]);
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task GetCategoryById_ExistId_ReturnExistCategory(int id)
        {
            // Arrage

            // Act
            var result = await _categoryService.GetCategoryById(id);

            // Assert
            Assert.NotNull(result);
            var expectedCategory = _categories.Where(c => c.Id == id).SingleOrDefault();
            Assert.AreEqual(expectedCategory, result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(3)]
        public async Task GetCategoryById_NotExistId_ReturnNull(int id)
        {
            // Arrange

            // Act
            var result = await _categoryService.GetCategoryById(id);

            // Assert
            Assert.Null(result);
        }

        [Test]
        public async Task CreateCategory__SuccessAndReturnNewCategory()
        {
            // Arrange
            var newCategory = new CreateCategoryDto() { Name = "New category" };

            // Act
            var result = await _categoryService.CreateCategory(newCategory);

            // Assert
            Assert.NotNull(result);
            AssertCategory(newCategory, result);
            var categoryInDb = await _dbContextTest.Categories.FindAsync(result.Id);
            Assert.NotNull(categoryInDb);
            AssertCategory(result, categoryInDb);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task UpdateCategory_ExistIdAndUpdateName_SuccessAndReturnUpdatedCategory(int id)
        {
            // Arrange
            var updateCategory = new UpdateCategoryDto() { Name = "Updated name" };

            // Act
            var result = await _categoryService.UpdateCategory(id, updateCategory);

            // Assert
            Assert.NotNull(result);
            AssertCategory(updateCategory, result);
            var categoryInDb = await _dbContextTest.Categories.FindAsync(result.Id);
            Assert.NotNull(categoryInDb);
            AssertCategory(result, categoryInDb);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task UpdateCategory_ExistIdAndNoUpdate_SuccessAndReturnCategory(int id)
        {
            // Arrange
            var updateCategory = new UpdateCategoryDto();

            // Act
            var result = await _categoryService.UpdateCategory(id, updateCategory);

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(3)]
        public async Task UpdateCategory_NotExistId_ThrowEntityNotFoundException(int id)
        {
            try
            {
                // Arrange
                var updateCategory = new UpdateCategoryDto() { Name = "Updated name" };

                // Act
                var result = await _categoryService.UpdateCategory(id, updateCategory);

                // Assert
                Assert.Fail();
            }
            catch (EntityNotFoundException ex)
            {
                // Assert
                Assert.Pass();
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task DeleteCategory_ExistId_SuccessAndNotExistInDb(int id)
        {
            // Arrange

            // Act
            await _categoryService.DeleteCategory(id);

            // Assert
            var categoryInDb = await _dbContextTest.Categories.FindAsync(id);
            Assert.Null(categoryInDb);
        }

        [Test]
        [TestCase(0)]
        [TestCase(3)]
        public async Task DeleteCategory_NotExistId_ThrowEntityNotFoundException(int id)
        {
            try
            {
                // Arrange

                // Act
                await _categoryService.DeleteCategory(id);

                // Assert
                Assert.Fail();
            }
            catch (EntityNotFoundException ex)
            {
                // Assert
                Assert.Pass();
            }
        }

        private void AssertCategory(Category expected, Category actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
        }

        private void AssertCategory(CreateCategoryDto expected, Category actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
        }

        private void AssertCategory(UpdateCategoryDto expected, Category actual)
        {
            if (expected.Name != null)
            {
                Assert.AreEqual(expected.Name, actual.Name);
            }
        }
    }
}
