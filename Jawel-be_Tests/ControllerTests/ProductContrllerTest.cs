using Jawel_be.Controllers;
using Jawel_be.Dtos.Category;
using Jawel_be.Dtos.Product;
using Jawel_be.Models;
using Jawel_be.Services.ProductService;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jawal_beTests.ControllerTests
{
    class ProductControllerTest
    {
        private List<Product> _products;
        private List<Category> _categories;
        private Mock<IProductService> _mockService;
        private ProductController _controller;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _categories = new List<Category>()
            {
                new Category { Id = 1, Name = "Nhẫn" },
                new Category { Id = 2, Name = "Dây chuyền" },
            };

            _products = new List<Product>()
            {
                new Product
                {
                    Id = 1,
                    Name = "Nhẫn gì đó",
                    Description = "Mô tả",
                    Cost = 200000,
                    Price = 300000,
                    Quantity = 1,
                    CategoryId = 1,
                    Category = _categories[0],
                },
                new Product
                {
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

            _mockService = new Mock<IProductService>();
            _controller = new ProductController(_mockService.Object);
        }

        //[Test]
        //public async Task GetProducts_ReturnsOkResult()
        //{
        //    // Arrange
        //    var products = new List<Product> { /* mock list of products */ };
        //    _productServiceMock.Setup(s => s.GetProducts()).ReturnsAsync(products);

        //    // Act
        //    var result = await _productController.GetProducts();

        //    // Assert
        //    result.Should().BeOfType<OkObjectResult>();
        //    var okResult = result as OkObjectResult;
        //    okResult.Value.Should().BeEquivalentTo(products);
        //}

        [Test]
        public async Task GetProducts_ReturnsOkAndValueIsAllProducts()
        {
            // Arrange
            _mockService.Setup(m => m.GetProducts()).ReturnsAsync(_products);

            // Act
            var actionResult = await _controller.GetProducts();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(actionResult);

            var okResult = ((OkObjectResult)actionResult).Value as List<ProductDto>;            
            Assert.NotNull(okResult);
       
            for (int i = 0; i < _products.Count; i++)
            {
                AssertProduct(_products[i], okResult[i]);
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task GetProduct_ExistId_ReturnOkAndValueIsProduct(int id)
        {
            // Arrange
            _mockService.Setup(m => m.GetProductById(id)).ReturnsAsync(_products.Find(c => c.Id == id));

            // Act
            var actionResult = await _controller.GetProduct(id);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            var result = ((OkObjectResult)actionResult).Value as ProductDto;
            Assert.NotNull(result);
            var expectedProduct = _products.Find(c => c.Id == id);
            AssertProduct(expectedProduct, result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(3)]
        public async Task GetProduct_NotExistId_ReturnNotFound(int id)
        {
            // Arrange
            _mockService.Setup(m => m.GetProductById(id)).ReturnsAsync(_products.Find(c => c.Id == id));

            // Act
            var actionResult = await _controller.GetProduct(id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public async Task CreateProduct_Valid_ReturnOkAndValueIsNewCategory()
        {
            // Arrange
            CreateProductDto createProduct = new CreateProductDto() { 
                Name = "New product",
                Description = "Mô tả",
                Cost = 200000,
                Price = 300000,
                Quantity = 1,
                CategoryId = 1,
            };
            _mockService.Setup(m => m.CreateProduct(createProduct)).ReturnsAsync(new Product() { 
                Name = createProduct.Name ,
                Description = createProduct.Description ,
                Cost = createProduct.Cost ,
                Price = createProduct.Price ,
                Quantity = createProduct.Quantity ,
                CategoryId = createProduct.CategoryId ,
            });

            // Act
            var actionResult = await _controller.CreateProduct(createProduct);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(actionResult);
            var result = ((CreatedAtActionResult)actionResult).Value as ProductDto;
            Assert.NotNull(result);
            AssertProduct(createProduct, result);
        }

        private void AssertProduct(Product? expected, ProductDto? result)
        {
            Assert.AreEqual(expected.Id, result.Id);
            Assert.AreEqual(expected.Name, result.Name);
            Assert.AreEqual(expected.Price, result.Price);
            Assert.AreEqual(expected.Cost, result.Cost);
            Assert.AreEqual(expected.Description, result.Description);
            Assert.AreEqual(expected.CategoryId, result.Category.Id);
            Assert.AreEqual(expected.Quantity, result.Quantity);
        }
        private void AssertProduct(CreateProductDto expected, ProductDto result)
        {
            Assert.AreEqual(expected.Name, result.Name);
            Assert.AreEqual(expected.Price, result.Price);
            Assert.AreEqual(expected.Cost, result.Cost);
            Assert.AreEqual(expected.Description, result.Description);
            Assert.AreEqual(expected.CategoryId, result.Category.Id);
            Assert.AreEqual(expected.Quantity, result.Quantity);
        }

        //[Test]
        //public async Task GetProduct_WithValidId_ReturnsOkResult()
        //{
        //    // Arrange
        //    int productId = 1;
        //    var product = new Product { /* mock product */ };
        //    _mockService.Setup(s => s.GetProductById(productId)).ReturnsAsync(product);

        //    // Act
        //    var result = await _controller.GetProduct(productId);

        //    // Assert
        //    result.Should().BeOfType<OkObjectResult>();
        //    var okResult = result as OkObjectResult;
        //    okResult.Value.Should().BeEquivalentTo(product);
        //}

        //[Test]
        //public async Task GetProduct_WithInvalidId_ReturnsNotFoundResult()
        //{
        //    // Arrange
        //    int productId = 1;
        //    _productServiceMock.Setup(s => s.GetProductById(productId)).ReturnsAsync(null);

        //    // Act
        //    var result = await _productController.GetProduct(productId);

        //    // Assert
        //    result.Should().BeOfType<NotFoundResult>();
        //}

        // Write similar tests for CreateProduct, UpdateProduct, and DeleteProduct methods
        // ...

    }
}
