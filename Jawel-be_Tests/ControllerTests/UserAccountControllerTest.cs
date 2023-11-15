using Jawel_be.Controllers;
using Jawel_be.Dtos.UserAccount;
using Jawel_be.Exceptions;
using Jawel_be.Models;
using Jawel_be.Services.UserAccountService;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Data;
using System.Reflection;

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

            _mockService = new Mock<IUserAccountService>();
            _controller = new UserAccountController(_mockService.Object);
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
        [TestCase("username", "password", "Chau", "Male", "Admin", "Active")]
        [TestCase("username", "password", "Chau", "Female", "Employee", "Active")]
        [TestCase("username", "password", "Chau", "Female", "Employee", "Inactive")]
        public async Task CreateUserAccount_Valid_ReturnOkAndValueIsNewUserAccount(string username, string password, string name, string gender, string role, string status)
        {
            // Arrange
            CreateUserAccountDto createUserAccount = new CreateUserAccountDto
            {
                Username = username,
                Password = password,
                Name = name,
                Gender = gender,
                Role = role,
                Status = status
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
        // invalid username
        [TestCase("", "password", "Chau", "Male", "Admin", "Active")]
        // invalid password
        [TestCase("username", "pass", "Chau", "Male", "Admin", "Active")]
        // invalid name
        [TestCase("username", "password", "" , "Male", "Admin", "Active")]
        // invalid gender
        [TestCase("username", "password", "Chau", "dsfas", "Admin", "Active")]
        // invalide role
        [TestCase("username", "password", "Chau", "Female", "dfsd", "Active")]
        // invalide status
        [TestCase("username", "password", "Chau", "Female", "Employee", "aaa")]
        public async Task CreateUserAccount_Invalid_ReturnBadRequest(string username, string password, string name, string gender, string role, string status)
        {
            // Arrange
            CreateUserAccountDto createUserAccount = new CreateUserAccountDto
            {
                Username = username,
                Password = password,
                Name = name,
                Gender = gender,
                Role = role,
                Status = status
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

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task UpdateUserAccount_ExistIdAndValidName_ReturnOkAndValueIsUpdatedUserAccount(int id)
        {
            // Arrange
            UpdateUserAccountDto updateUserAccountDto = new UpdateUserAccountDto()
            {
                Name = "Minh Chau 2",
                Gender = "Female",
                Role = "Employee",
                Status = "Active"
            };
            var matchedUserAccount = _userAccounts.Find(c => c.Id == id);
            UserAccount updatedUserAccount = new UserAccount();
            if (matchedUserAccount != null)
            {
                updatedUserAccount = new UserAccount { Id = matchedUserAccount.Id, Name = updateUserAccountDto.Name };
                _mockService.Setup(m => m.UpdateUserAccount(id, updateUserAccountDto)).ReturnsAsync(updatedUserAccount);
            }

            // Act
            var actionResult = await _controller.UpdateUserAccount(id, updateUserAccountDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(actionResult);
            var result = ((OkObjectResult)actionResult).Value as UserAccountDto;
            Assert.NotNull(result);
            AssertUserAccount(updatedUserAccount, result);
        }

        // Name, Gender, Role, Status
        [Test]
        // invalid name
        [TestCase(1, "", "Male", "Admin", "Active")]
        // invalid gender
        [TestCase(1, "Chau", "", "Admin", "Active")]
        [TestCase(1, "Chau", "dsfas", "Admin", "Active")]
        // invalide role
        [TestCase(1, "Chau", "Male", "", "Active")]
        [TestCase(1, "Chau", "Female", "dfsd", "Active")]
        // invalide status
        [TestCase(1, "Chau", "Male", "Admin", "")]
        [TestCase(1, "Chau", "Female", "Employee", "aaa")]
        public async Task UpdateUserAccount_ExistIdAndInvalid_ReturnBadRequest(int id, string name, string gender, string role, string status)
        {
            // Arrange
            UpdateUserAccountDto updateUserAccountDto = new UpdateUserAccountDto()
            {
                Name = name,
                Gender = gender,
                Role = role,
                Status = status
            };
            var matchedUserAccount = _userAccounts.Find(c => c.Id == id);
            UserAccount updatedUserAccount = new UserAccount();
            if (matchedUserAccount != null)
            {
                updatedUserAccount = new UserAccount { Id = matchedUserAccount.Id, Name = updateUserAccountDto.Name };
                _mockService.Setup(m => m.UpdateUserAccount(id, updateUserAccountDto)).ReturnsAsync(updatedUserAccount);
            }

            // Act
            var actionResult = await _controller.UpdateUserAccount(id, updateUserAccountDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(actionResult);
        }

        [Test]
        [TestCase(0)]
        [TestCase(3)]
        public async Task UpdateUserAccount_NotExistId_ReturnNotFound(int id)
        {
            // Arrange
            UpdateUserAccountDto updateUserAccountDto = new UpdateUserAccountDto()
            {
                Name = "Minh Chau 2",
                Gender = "Female",
                Role = "Employee",
                Status = "Active"
            };
            _mockService.Setup(m => m.UpdateUserAccount(id, updateUserAccountDto)).ThrowsAsync(new EntityNotFoundException());

            // Act
            var actionResult = await _controller.UpdateUserAccount(id, updateUserAccountDto);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task DeleteUserAccount_ExistId_ReturnOk(int id)
        {
            // Arrange
            _mockService.Setup(m => m.DeleteUserAccount(id));

            // Act
            var actionResult = await _controller.DeleteUserAccount(id);

            // Assert
            Assert.IsInstanceOf<OkResult>(actionResult);
        }

        [Test]
        [TestCase(0)]
        [TestCase(3)]
        public async Task DeleteUserAccount_NotExistId_ReturnNotFound(int id)
        {
            // Arrange
            _mockService.Setup(m => m.DeleteUserAccount(id)).Throws(new EntityNotFoundException());

            // Act
            var actionResult = await _controller.DeleteUserAccount(id);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
        }

        [Test]
        [TestCase(1, "testtttt", "test00000")]
        public async Task ChangePasswordUserAccount_ExistIdValidAndCorrectPassword_ReturnOk(int id, string currentPassword, string password)
        {
            // Arrange
            ChangePasswordUserAccountDto changePasswordUserAccountDto = new ChangePasswordUserAccountDto()
            {
                CurrentPassword = currentPassword,
                NewPassword = password
            };
            _mockService.Setup(m => m.ChangePasswordUserAccount(id, changePasswordUserAccountDto));

            // Act
            var actionResult = await _controller.ChangePasswordUserAccount(id, changePasswordUserAccountDto);

            // Assert
            Assert.IsInstanceOf<OkResult>(actionResult);
        }


        [Test]
        [TestCase(1, "testtttt", "test00000")]
        public async Task ChangePasswordUserAccount_ExistIdValidAndIncorrectCurrentPassword_ReturnOk(int id, string currentPassword, string password)
        {
            // Arrange
            ChangePasswordUserAccountDto changePasswordUserAccountDto = new ChangePasswordUserAccountDto()
            {
                CurrentPassword = currentPassword,
                NewPassword = password
            };
            _mockService.Setup(m => m.ChangePasswordUserAccount(id, changePasswordUserAccountDto)).ThrowsAsync(new CurrentPasswordIncorrectException());

            // Act
            var actionResult = await _controller.ChangePasswordUserAccount(id, changePasswordUserAccountDto);

            // Assert
            Assert.IsInstanceOf<BadRequestResult>(actionResult);
        }


        [Test]
        [TestCase(1, "test", "test0")]
        [TestCase(1, "test00000", "test0")]
        [TestCase(1, "test", "test000000")]
        public async Task ChangePasswordUserAccount_ExistIdInvalid_ReturnBadRequest(int id, string currentPassword, string password)
        {
            // Arrange
            ChangePasswordUserAccountDto changePasswordUserAccountDto = new ChangePasswordUserAccountDto()
            {
                CurrentPassword = currentPassword,
                NewPassword = password
            };
            _mockService.Setup(m => m.ChangePasswordUserAccount(id, changePasswordUserAccountDto));

            // Act
            var actionResult = await _controller.ChangePasswordUserAccount(id, changePasswordUserAccountDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(actionResult);
        }

        [Test]
        [TestCase(4, "testfffff", "test0ffff")]
        public async Task ChangePasswordUserAccount_NotExistId_ReturnNotFound(int id, string currentPassword, string password)
        {
            // Arrange
            ChangePasswordUserAccountDto changePasswordUserAccountDto = new ChangePasswordUserAccountDto()
            {
                CurrentPassword = currentPassword,
                NewPassword = password
            };
            _mockService.Setup(m => m.ChangePasswordUserAccount(id, changePasswordUserAccountDto)).ThrowsAsync(new EntityNotFoundException());

            // Act
            var actionResult = await _controller.ChangePasswordUserAccount(id, changePasswordUserAccountDto);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(actionResult);
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
            if (expected.Gender != null)
            {
                Assert.AreEqual(expected.Gender, actual.Gender);
            }
            if (expected.Role != null)
            {
                Assert.AreEqual(expected.Role, actual.Role);
            }
        }
    }
}
