using Jawel_be.Controllers;
using Jawel_be.Dtos.UserAccount;
using Jawel_be.Dtos.UserAccount;
using Jawel_be.Dtos.UserAccount;
using Jawel_be.Exceptions;
using Jawel_be.Models;
using Jawel_be.Services.UserAccountService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jawal_beTests.ControllerTests
{
    public class UserAccountControllerTest
    {
        private List<UserAccount> _userAccounts;
        private Mock<IUserAccountService> _mockService;
        private UserAccountController _controller;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _userAccounts = new List<UserAccount>()
            {
                new UserAccount
                {
                    Id = 1,
                    Username = "test",
                    Password = "test",
                    Name = "Chau",
                    Gender = "Male",
                    Role = "Admin",
                    Status = "Active"
                },
                new UserAccount
                {
                    Id = 2,
                    Username = "test2",
                    Password = "test2",
                    Name = "Minh Chau",
                    Gender = "Male",
                    Role = "Admin",
                    Status = "Active"
                },
            };

            IConfiguration config = new ConfigurationBuilder().Build();

            _mockService = new Mock<IUserAccountService>();
            _controller = new UserAccountController(_mockService.Object, config);
        }

        [Test]
        public async Task GetUserAccounts__ReturnOkAndValueIsAllUserAccounts()
        {
            // Arrange
            _mockService.Setup(m => m.GetUserAccounts()).ReturnsAsync(_userAccounts);

            // Act
            var actionResult = await _controller.GetUserAccounts();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            var result = ((OkObjectResult)actionResult).Value as List<UserAccountDto>;
            Assert.NotNull(result);
            for (int i = 0; i < result.Count; i++)
            {
                AssertUserAccount(_userAccounts[i], result[i]);
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task GetUserAccount_ExistId_ReturnOkAndValueIsUserAccount(int id)
        {
            // Arrange
            _mockService.Setup(m => m.GetUserAccountById(id)).ReturnsAsync(_userAccounts.Find(c => c.Id == id));

            // Act
            var actionResult = await _controller.GetUserAccount(id);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            var result = ((OkObjectResult)actionResult).Value as UserAccountDto;
            Assert.NotNull(result);
            var expectedUserAccount = _userAccounts.Find(c => c.Id == id);
            AssertUserAccount(expectedUserAccount, result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(3)]
        public async Task GetUserAccount_NotExistId_ReturnNotFound(int id)
        {
            // Arrange
            _mockService.Setup(m => m.GetUserAccountById(id)).ReturnsAsync(_userAccounts.Find(c => c.Id == id));

            // Act
            var actionResult = await _controller.GetUserAccount(id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        public async Task CreateUserAccount_Valid_ReturnOkAndValueIsNewUserAccount()
        {
            // Arrange
            CreateUserAccountDto createUserAccount = new CreateUserAccountDto
            {
                Username = "test3",
                Password = "testtest",
                Name = "Minh Chau",
                Gender = "Male",
                Role = "Admin",
                Status = "Active"
            };

            _mockService.Setup(m => m.CreateUserAccount(createUserAccount)).ReturnsAsync(new UserAccount
            {
                Username = createUserAccount.Username,
                Password = createUserAccount.Password,
                Name = createUserAccount.Name,
                Gender = createUserAccount.Gender,
                Role = createUserAccount.Role,
                Status = createUserAccount.Status
            });

            // Act
            var actionResult = await _controller.CreateUserAccount(createUserAccount);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(actionResult);
            var result = ((CreatedAtActionResult)actionResult).Value as UserAccountDto;
            Assert.NotNull(result);
            AssertUserAccount(createUserAccount, result);
        }

        [Test]
        public async Task CreateUserAccount_Invalid_ReturnBadRequest()
        {
            // Arrange
            CreateUserAccountDto createUserAccount = new CreateUserAccountDto
            {
                Username = "",
                Password = "testtest",
                Name = "Minh Chau",
                Gender = "Male",
                Role = "Admin",
                Status = "Active"
            };

            _mockService.Setup(m => m.CreateUserAccount(createUserAccount)).ReturnsAsync(new UserAccount
            {
                Username = createUserAccount.Username,
                Password = createUserAccount.Password,
                Name = createUserAccount.Name,
                Gender = createUserAccount.Gender,
                Role = createUserAccount.Role,
                Status = createUserAccount.Status
            });

            // Act
            var actionResult = await _controller.CreateUserAccount(createUserAccount);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(actionResult);
        }

        [Test]
        public async Task CreateUserAccount_AlreadyExistUserAccount_ReturnBadRequest()
        {
            // Arrange
            CreateUserAccountDto createUserAccount = new CreateUserAccountDto
            {
                Username = "test",
                Password = "testtest",
                Name = "Minh Chau",
                Gender = "Male",
                Role = "Admin",
                Status = "Active"
            };

            _mockService.Setup(m => m.CreateUserAccount(createUserAccount)).ThrowsAsync(new AlreadyExistUserAccountException());

            // Act
            var actionResult = await _controller.CreateUserAccount(createUserAccount);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }

        private void AssertUserAccount(UserAccount expected, UserAccount actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Username, actual.Username);
            Assert.AreEqual(expected.Password, actual.Password);
            Assert.AreEqual(expected.Gender, actual.Gender);
            Assert.AreEqual(expected.Status, actual.Status);
            Assert.AreEqual(expected.Role, actual.Role);
        }

        private void AssertUserAccount(UserAccount expected, UserAccountDto actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Username, actual.Username);
            Assert.AreEqual(expected.Gender, actual.Gender);
            Assert.AreEqual(expected.Status, actual.Status);
            Assert.AreEqual(expected.Role, actual.Role);
        }

        private void AssertUserAccount(CreateUserAccountDto expected, UserAccountDto actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Username, actual.Username);
            Assert.AreEqual(expected.Gender, actual.Gender);
            Assert.AreEqual(expected.Status, actual.Status);
            Assert.AreEqual(expected.Role, actual.Role);
        }

        private void AssertUserAccount(UpdateUserAccountDto expected, UserAccount actual)
        {
            if (expected.Name != null)
            {
                Assert.AreEqual(expected.Name, actual.Name);
            }
            if (expected.Status != null)
            {
                Assert.AreEqual(expected.Status, actual.Status);
            }
            if (expected.Role != null)
            {
                Assert.AreEqual(expected.Role, actual.Role);
            }
        }
    }
}
