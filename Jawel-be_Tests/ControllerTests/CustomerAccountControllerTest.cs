using Jawel_be.Controllers;
using Jawel_be.Dtos.CustomerAccount;
using Jawel_be.Exceptions;
using Jawel_be.Models;
using Jawel_be.Services.CustomerAccountService;
using Microsoft.AspNetCore.Mvc;
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

            _mockService = new Mock<ICustomerAccountService>();
            _controller = new CustomerAccountController(_mockService.Object);
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
            CreateCustomerAccountDto createCustomerAccount = new CreateCustomerAccountDto() { Phone = "3", Name = "Tester3", Password = "123456", Gender = "Male" };

            _mockService.Setup(m => m.CreateCustomerAccount(createCustomerAccount)).ReturnsAsync(new CustomerAccount() { Phone = createCustomerAccount.Phone, Name = createCustomerAccount.Name, Password = createCustomerAccount.Password, Gender = createCustomerAccount.Gender });

            // Act
            var actionResult = await _controller.CreateCustomerAccount(createCustomerAccount);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(actionResult);
            var result = ((CreatedAtActionResult)actionResult).Value as CustomerAccountDto;
            Assert.NotNull(result);
            AssertCustomerAccount(createCustomerAccount, result);

        }

        [Test]
        public async Task CreateCustomerAccount_AlreadyExistCustomerAccount_ReturnBadRequest()
        {
            // Arrange
            CreateCustomerAccountDto createCustomerAccount = new CreateCustomerAccountDto
            {
                Phone = "013456789",
                Password = "testtest",
                Name = "Minh Chau",
                Gender = "Female",
            };

            _mockService.Setup(m => m.CreateCustomerAccount(createCustomerAccount)).ThrowsAsync(new AlreadyExistCustomerAccountException());

            // Act
            var actionResult = await _controller.CreateCustomerAccount(createCustomerAccount);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }

        [Test]
        //Case all Empty
        [TestCase("", "", "", "")]
        //case Password.length() <6
        [TestCase("3", "Tester3", "123", "Male")]
        //Case gender is not in "Male" or "Female"
        [TestCase("3", "Tester3", "123456", "Nam")]
        //case name empty
        [TestCase("3", "", "password", "Male")]
        //case phone empty
        [TestCase("", "Tester3", "password", "Male")]
        public async Task CreateCustomerAccount_InvalidName_ReturnBadRequest(string phone, string name, string password, string gender)
        {
            // Arrange
            CreateCustomerAccountDto createCustomerAccount = new CreateCustomerAccountDto() { Phone = phone, Name = name, Password = password, Gender = gender };

            _mockService.Setup(m => m.CreateCustomerAccount(createCustomerAccount)).ReturnsAsync(new CustomerAccount() { Phone = createCustomerAccount.Phone, Name = createCustomerAccount.Name, Password = createCustomerAccount.Password, Gender = createCustomerAccount.Gender });


            // Act
            var actionResult = await _controller.CreateCustomerAccount(createCustomerAccount);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(actionResult);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task UpdateCustomerAccount_ExistIdAndValidInformation_ReturnOkAndValueIsUpdatedCustomerAccount(int id)
        {
            // Arrange
            UpdateCustomerAccountDto updateCustomerAccountDto = new UpdateCustomerAccountDto() { Name = "Updated name", Gender = "Female", Address = "Updated Adress" };
            var matchedCustomerAccount = _customers.Find(x => x.Id == id);
            CustomerAccount updatedCustomerAccount = new CustomerAccount();
            if (matchedCustomerAccount != null)
            {
                updatedCustomerAccount = new CustomerAccount { Phone = matchedCustomerAccount.Phone, Name = matchedCustomerAccount.Name, Password = matchedCustomerAccount.Password, Gender = matchedCustomerAccount.Password };
                _mockService.Setup(m => m.UpdateCustomerAccount(id, updateCustomerAccountDto)).ReturnsAsync(updatedCustomerAccount);


            }


            // Act
            var actionResult = await _controller.UpdateCustomerAccount(id, updateCustomerAccountDto);


            // Assert
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            var result = ((OkObjectResult)actionResult).Value as CustomerAccountDto;
            Assert.NotNull(result);
            AssertCustomerAccount(updatedCustomerAccount, result);

        }

        [Test]
        //Case all Empty
        [TestCase(1, "", "", "")]
        //Case gender is not in "Male" or "Female"
        [TestCase(1, "Tester3", "male", "UpdateAddress")]
        [TestCase(1, "Tester3", "", "UpdateAddress")]
        //case name empty
        [TestCase(1, "", "Male", "UpdateAddress")]
        public async Task UpdateCustomerAccount_ExistIdAndInvalidInformation_ReturnBadRequest(int id, string name, string gender, string address)
        {

            // Arrange
            UpdateCustomerAccountDto updateCustomerAccountDto = new UpdateCustomerAccountDto() { Name = name, Gender = gender, Address = address };
            var matchedCustomerAccount = _customers.Find(x => x.Id == id);
            CustomerAccount updatedCustomerAccount = new CustomerAccount();
            if (matchedCustomerAccount != null)
            {
                updatedCustomerAccount = new CustomerAccount { Phone = matchedCustomerAccount.Phone, Name = matchedCustomerAccount.Name, Password = matchedCustomerAccount.Password, Gender = matchedCustomerAccount.Password };
                _mockService.Setup(m => m.UpdateCustomerAccount(id, updateCustomerAccountDto)).ReturnsAsync(updatedCustomerAccount);


            }


            // Act
            var actionResult = await _controller.UpdateCustomerAccount(id, updateCustomerAccountDto);


            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(actionResult);


        }

        [Test]
        [TestCase(0)]
        [TestCase(3)]
        public async Task UpdateCustomerAccount_NotExistId_ReturnNotFound(int id)
        {
            // Arrange
            UpdateCustomerAccountDto updateCustomerAccountDto = new UpdateCustomerAccountDto() { Name = "Updated name", Gender = "Female", Address = "Updated Adress" };
            _mockService.Setup(m => m.UpdateCustomerAccount(id, updateCustomerAccountDto)).Throws(new EntityNotFoundException());


            // Act
            var actionResult = await _controller.UpdateCustomerAccount(id, updateCustomerAccountDto);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task DeleteCustomerAccount_ExistId_ReturnOk(int id)
        {
            // Arrange
            _mockService.Setup(m => m.DeleteCustomerAccount(id));

            // Act
            var actionResult = await _controller.DeleteCustomerAccount(id);

            // Assert
            Assert.IsInstanceOf<OkResult>(actionResult);
        }

        [Test]
        [TestCase(0)]
        [TestCase(3)]
        public async Task DeleteCustomerAccount_NotExistId_ReturnNotFound(int id)
        {
            // Arrange
            _mockService.Setup(m => m.DeleteCustomerAccount(id)).Throws(new EntityNotFoundException());


            // Act
            var actionResult = await _controller.DeleteCustomerAccount(id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        [TestCase(1)]

        public async Task ChangePasswordCustomerAccount_ExistIdAndValidCurrentPasswordAndValidNewPassword_ReturnOK(int id)
        {


            // Arrange
            ChangePasswordCustomerAccountDto changePasswordCustomerAccountDto = new ChangePasswordCustomerAccountDto() { CurrentPassword = "1", NewPassword = "123456" };


            // Act
            var actionResult = await _controller.ChangePasswordCustomerAccount(id, changePasswordCustomerAccountDto);


            // Assert
            Assert.IsInstanceOf<OkResult>(actionResult);



        }




        [Test]
        [TestCase(0)]
        [TestCase(3)]
        public async Task ChangePasswordCustomerAccount_NotExistId_ThrowEntityNotFoundException(int id)
        {

            // Arrange
            ChangePasswordCustomerAccountDto changePasswordCustomerAccountDto = new ChangePasswordCustomerAccountDto() { CurrentPassword = "111111", NewPassword = "123456" };
            _mockService.Setup(m => m.ChangePasswordCustomerAccount(id, changePasswordCustomerAccountDto)).Throws(new EntityNotFoundException());
            // Act
            var actionResult = await _controller.ChangePasswordCustomerAccount(id, changePasswordCustomerAccountDto);


            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        [TestCase(1)]
        public async Task ChangePasswordCustomerAccount_ExistIdAndInvalidNCurentPassword_CurrentPasswordIncorrectException(int id)
        {

            // Arrange
            ChangePasswordCustomerAccountDto changePasswordCustomerAccountDto = new ChangePasswordCustomerAccountDto() { CurrentPassword = "123456", NewPassword = "123456" };
            _mockService.Setup(m => m.ChangePasswordCustomerAccount(id, changePasswordCustomerAccountDto)).Throws(new CurrentPasswordIncorrectException());

            // Act
            var actionResult = await _controller.ChangePasswordCustomerAccount(id, changePasswordCustomerAccountDto);

            // Assert

            Assert.IsInstanceOf<BadRequestResult>(actionResult);

        }
        [Test]
        [TestCase(1)]
        public async Task ChangePasswordCustomerAccount_ExistIdAndInvalidNNewPassword_ReturnBadRequest(int id)
        {

            // Arrange
            ChangePasswordCustomerAccountDto changePasswordCustomerAccountDto = new ChangePasswordCustomerAccountDto() { CurrentPassword = "1", NewPassword = "3" };

            // Act
            var actionResult = await _controller.ChangePasswordCustomerAccount(id, changePasswordCustomerAccountDto);

            // Assert

            Assert.IsInstanceOf<BadRequestObjectResult>(actionResult);

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
