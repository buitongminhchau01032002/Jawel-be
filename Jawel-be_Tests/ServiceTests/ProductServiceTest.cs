using Jawel_be.Contexts;
using Jawel_be.Models;
using Jawel_be.Services.ProductService;
using Jawel_be.Dtos.Product;
using Microsoft.EntityFrameworkCore;
using Jawel_be.Exceptions;

namespace Jawal_beTests.ServiceTests
{
    public class ProductServiceTest
    {
        private List<Product> _products;
        private List<Category> _categories;
        private ProductService _productService;
        private DataContext _dbContext;
        private DataContext _dbContextTest;


        [SetUp]
        public void SetUp()
        {
            _categories = new List<Category>()
            {
                new Category { Id = 1, Name = "Nhẫn" },
                new Category { Id = 2, Name = "Dây chuyền" },
            };
            _products = new List<Product>()
            {
                new Product {
                    Id = 1,
                    Name = "Nhẫn gì đó",
                    Description = "Mô tả",
                    Cost = 200000,
                    Price = 300000,
                    Quantity = 1,
                    CategoryId = 1,
                    Category = _categories[0],
                },
                new Product {
                    Id = 2,
                    Name = "Day chuyen gì đó",
                    Description = "Mô tả",
                    Cost = 200000,
                    Price = 300000,
                    Quantity = 1,
                    CategoryId = 2,
                    Category = _categories[1],
                },
            };

            // Setup test db
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "testDB")
                .Options;
            _dbContext = new DataContext(options);
            _dbContextTest = new DataContext(options);
            _dbContext.Database.EnsureCreated();
            foreach (var product in _products)
            {
                _dbContext.Products.Add(product);
                
            }
            _dbContext.SaveChanges();

            // Setup service
            _productService = new ProductService(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
        }

        [Test]
        public async Task GetProducts__ReturnAllProducts()
        {
            // Arrange

            // Act
            var result = await _productService.GetProducts();

            // Assert
            Assert.AreEqual(_products.Count, result.Count);
            for (int i = 0; i < result.Count; i++)
            {
                AssertProduct(_products[i], result[i]);
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task GetProductById_ExistId_ReturnExistProduct(int id)
        {
            // Arrage

            // Act
            var result = await _productService.GetProductById(id);

            // Assert
            Assert.NotNull(result);
            var expectedProduct = _products.Find(c => c.Id == id);
            AssertProduct(expectedProduct, result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(3)]
        public async Task GetProductById_NotExistId_ReturnNull(int id)
        {
            // Arrange

            // Act
            var result = await _productService.GetProductById(id);

            // Assert
            Assert.Null(result);
        }

        [Test]
        public async Task CreateProduct__SuccessAndReturnNewProduct()
        {
            // Arrange
            var newProduct = new CreateProductDto() {
                Name = "Nhẫn gì đó",
                Description = "Mô tả",
                Cost = 200000,
                Price = 300000,
                Quantity = 1,
                CategoryId = 1,
            };

            // Act
            var result = await _productService.CreateProduct(newProduct);

            // Assert
            Assert.NotNull(result);
            AssertProduct(newProduct, result);
            var productInDb = await _dbContextTest.Products.FindAsync(result.Id);
            Assert.NotNull(productInDb);
            AssertProduct(result, productInDb);
        }

        [Test]
        public async Task CreateProduct_EmtyCategory_ThrowEntityNotFoundException()
        {
            try
            {
                // Arrange
                var newProduct = new CreateProductDto()
                {
                    Name = "Nhẫn gì đó",
                    Description = "Mô tả",
                    Cost = 200000,
                    Price = 300000,
                    Quantity = 1,

                };

                // Act
                var result = await _productService.CreateProduct(newProduct);

                Assert.Fail();
            } 
            
            catch (EntityNotFoundException e)
            {
                Assert.Pass();
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task UpdateProduct_ExistId_SuccessAndReturnUpdatedProduct(int id)
        {
            // Arrange
            var updateProduct = new UpdateProductDto() {
                Name = "Ten tgi do",
                Description = "Mô tả cua ten gi do",
                Cost = 200,
                Price = 800000,
                Quantity = 5,
                CategoryId = 2,
            };

            // Act
            var result = await _productService.UpdateProduct(id, updateProduct);

            // Assert
            Assert.NotNull(result);
            AssertProduct(updateProduct, result);
            var productInDb = await _dbContextTest.Products.FindAsync(result.Id);
            Assert.NotNull(productInDb);
            AssertProduct(result, productInDb);
        }

        [Test]

        public async Task UpdateProduct_ExistIdNoAvailableCategoryId_ThrowEntityNotFoundException()
        {
            try
            {
                // Arrange
                var updateProduct = new UpdateProductDto()
                {
                    Name = "Ten tgi do",
                    Description = "Mô tả cua ten gi do",
                    Cost = 200,
                    Price = 800000,
                    Quantity = 5,
                    CategoryId = 4,
                };

                // Act
                var result = await _productService.UpdateProduct(1, updateProduct);

                Assert.Fail();
            } 
            catch(EntityNotFoundException e)
            {
                Assert.Pass();
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task UpdateProduct_ExistIdAndNoUpdate_SuccessAndReturnProduct(int id)
        {
            // Arrange
            var updateProduct = new UpdateProductDto();

            // Act
            var result = await _productService.UpdateProduct(id, updateProduct);

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(3)]
        public async Task UpdateProduct_NotExistId_ThrowEntityNotFoundException(int id)
        {
            try
            {
                // Arrange
                var updateProduct = new UpdateProductDto() { Name = "Updated name" };

                // Act
                var result = await _productService.UpdateProduct(id, updateProduct);

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
        public async Task DeleteProduct_ExistId_SuccessAndNotExistInDb(int id)
        {
            // Arrange

            // Act
            await _productService.DeleteProduct(id);

            // Assert
            var productInDb = await _dbContextTest.Products.FindAsync(id);
            Assert.Null(productInDb);
        }

        [Test]
        [TestCase(0)]
        [TestCase(3)]
        public async Task DeleteProduct_NotExistId_ThrowEntityNotFoundException(int id)
        {
            try
            {
                // Arrange

                // Act
                await _productService.DeleteProduct(id);

                // Assert
                Assert.Fail();
            }
            catch (EntityNotFoundException ex)
            {
                // Assert
                Assert.Pass();
            }
        }

        private void AssertProduct(Product expected, Product actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.Quantity, actual.Quantity);
            Assert.AreEqual(expected.Price, actual.Price);
            Assert.AreEqual(expected.Cost, actual.Cost);
            Assert.AreEqual(expected.CategoryId, actual.CategoryId);
        }

        private void AssertProduct(CreateProductDto expected, Product actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Description, actual.Description);
            Assert.AreEqual(expected.Quantity, actual.Quantity);
            Assert.AreEqual(expected.Price, actual.Price);
            Assert.AreEqual(expected.Cost, actual.Cost);
            Assert.AreEqual(expected.CategoryId, actual.CategoryId);
        }

        private void AssertProduct(UpdateProductDto expected, Product actual)
        {
            if (expected.Name != null)
            {
                Assert.AreEqual(expected.Name, actual.Name);
            }
            if (expected.Description != null)
            {
                Assert.AreEqual(expected.Description, actual.Description);
            }
            if (expected.Quantity != null)
            {
                Assert.AreEqual(expected.Quantity, actual.Quantity);
            }
            if (expected.Price != null)
            {
                Assert.AreEqual(expected.Price, actual.Price);
            }
            if (expected.Cost != null)
            {
                Assert.AreEqual(expected.Cost, actual.Cost);
            }
            if (expected.CategoryId != null)
            {
                Assert.AreEqual(expected.CategoryId, actual.CategoryId);
            }
        }
    }
}
