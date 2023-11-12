using Jawel_be.Controllers;
using Jawel_be.Dtos.Category;
using Jawel_be.Dtos.CustomerAccount;
using Jawel_be.Exceptions;
using Jawel_be.Models;
using Jawel_be.Services.CategoryService;
using Jawel_be.Services.CustomerAccountService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;


namespace Jawal_beTests.ControllerTests
{
    public class CustomerAccountControllerTest
    {

       
            private List<CustomerAccount> _customers;
            private Mock<ICustomerAccountService> _mockService;
            private CustomerAccountController _controller;

            [OneTimeSetUp]
            public void OneTimeSetUp()
            {
            _customers = new List<CustomerAccount>()
            {
                new CustomerAccount {Id=1, Phone = "1", Name = "Tester1", Password="1", Gender="Male" },
                 new CustomerAccount {Id=2, Phone = "2", Name = "Tester2", Password="2", Gender="Male" }
            };
            IConfiguration config = new ConfigurationBuilder().Build();

            _mockService = new Mock<ICustomerAccountService>();
                _controller = new CustomerAccountController(_mockService.Object, config);
            }


            [Test]
            public async Task GetCustomerAccounts__ReturnOkAndValueIsAllCustomerAccounts()
            {
                // Arrange
                _mockService.Setup(m => m.GetCustomerAccounts()).ReturnsAsync(_customers);

                // Act
                var actionResult = await _controller.GetCustomerAccounts();

                // Assert
                Assert.IsInstanceOf<OkObjectResult>(actionResult);
                var result = ((OkObjectResult)actionResult).Value as List<CustomerAccountDto>;
                Assert.NotNull(result);
                for (int i = 0; i < result.Count; i++)
                {
                    AssertCustomerAccount(_customers[i], result[i]);
                }
            }

            [Test]
            [TestCase(1)]
            [TestCase(2)]
            public async Task GetCustomerAccount_ExistId_ReturnOkAndValueIsCustomerAccount(int id)
            {
                // Arrange
                _mockService.Setup(m => m.GetCustomerAccountById(id)).ReturnsAsync(_customers.Find(c => c.Id == id));

                // Act
                var actionResult = await _controller.GetCustomerAccount(id);

                // Assert
                Assert.IsInstanceOf<OkObjectResult>(actionResult);
                var result = ((OkObjectResult)actionResult).Value as CustomerAccountDto;
                Assert.NotNull(result);
                var expectedCustomerAccount = _customers.Find(c => c.Id == id);
                AssertCustomerAccount(expectedCustomerAccount, result);
            }

            [Test]
            [TestCase(0)]
            [TestCase(3)]
            public async Task GetCustomerAccount_NotExistId_ReturnNotFound(int id)
            {
                // Arrange
                _mockService.Setup(m => m.GetCustomerAccountById(id)).ReturnsAsync(_customers.Find(c => c.Id == id));

                // Act
                var actionResult = await _controller.GetCustomerAccount(id);

                // Assert
                Assert.IsInstanceOf<NotFoundResult>(actionResult);
            }

            [Test]
            public async Task CreateCustomerAccount_Valid_ReturnOkAndValueIsNewCustomerAccount()
            {
                // Arrange
                CreateCustomerAccountDto createCustomerAccount = new CreateCustomerAccountDto { Phone = "3", Name = "Tester3", Password = "123456", Gender = "Male" };
          
            _mockService.Setup(m => m.CreateCustomerAccount(createCustomerAccount)).ReturnsAsync(new CustomerAccount() { Phone = createCustomerAccount.Phone, Name = createCustomerAccount.Name, Password = createCustomerAccount.Password, Gender = createCustomerAccount.Password });

            // Act
            var actionResult = await _controller.CreateCustomerAccount(createCustomerAccount);

                // Assert
                Assert.IsInstanceOf<CreatedAtActionResult>(actionResult);
                var result = ((CreatedAtActionResult)actionResult).Value as CustomerAccountDto;
                Assert.NotNull(result);
            AssertCustomerAccount(createCustomerAccount, result);
               
            }

            [Test]
            //Case all Empty
            [TestCase("", "","","" )]
            //case Password.length() <6
            [TestCase("3", "Tester3", "123","Male")]
        //Case gender is not in "Male" or "Female"
             [TestCase("3", "Tester3", "123456", "Nam")]
        public async Task CreateCategory_InvalidName_ReturnBadRequest(string phone, string name, string password, string gender)
            {
            // Arrange
            CreateCustomerAccountDto createCustomerAccount = new CreateCustomerAccountDto() { Phone = phone, Name = name, Password = password, Gender = gender };
               
                _mockService.Setup(m => m.CreateCustomerAccount(createCustomerAccount)).ReturnsAsync( new CustomerAccount() { Phone = createCustomerAccount.Phone, Name = createCustomerAccount.Name, Password = createCustomerAccount.Password, Gender = createCustomerAccount.Password });


            // Act
            var actionResult = await _controller.CreateCustomerAccount(createCustomerAccount);

                // Assert
                Assert.IsInstanceOf<BadRequestObjectResult>(actionResult);
            }

         /*   [Test]
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
            }*/

        private void AssertCustomerAccount(CustomerAccount expected, CustomerAccount actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Password, actual.Password);
            Assert.AreEqual(expected.Gender, actual.Gender);
            Assert.AreEqual(expected.Address, actual.Address);
            Assert.AreEqual(expected.Avatar, actual.Avatar);
        }

        private void AssertCustomerAccount(CustomerAccount expected, CustomerAccountDto actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
           
            Assert.AreEqual(expected.Gender, actual.Gender);
            Assert.AreEqual(expected.Address, actual.Address);
            Assert.AreEqual(expected.Avatar, actual.Avatar);
        }

        private void AssertCustomerAccount(CreateCustomerAccountDto expected, CustomerAccountDto actual)
        {
            if (expected.Name != null)
            {
                Assert.AreEqual(expected.Name, actual.Name);
                Assert.AreEqual(expected.Gender, actual.Gender);
                Assert.AreEqual(expected.Address, actual.Address);
                Assert.AreEqual(expected.Avatar, actual.Avatar);
            }
        }
    }
    }
