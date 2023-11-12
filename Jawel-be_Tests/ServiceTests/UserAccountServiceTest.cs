using Jawel_be.Contexts;
using Jawel_be.Dtos.UserAccount;
using Jawel_be.Exceptions;
using Jawel_be.Models;
using Jawel_be.Services.UserAccountService;
using Microsoft.EntityFrameworkCore;

namespace Jawal_beTests.ServiceTests
{
    public class UserAccountServiceTest
    {
        private List<UserAccount> _userAccounts;
        private UserAccountService _userAccountService;
        private DataContext _dbContext;
        private DataContext _dbContextTest;

        [SetUp]
        public void SetUp()
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

            // Setup test db
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "testDB")
                .Options;
            _dbContext = new DataContext(options);
            _dbContextTest = new DataContext(options);
            _dbContext.Database.EnsureCreated();
            foreach (var userAccount in _userAccounts)
            {
                _dbContext.UserAccounts.Add(userAccount);
            }
            _dbContext.SaveChanges();

            // Setup service
            _userAccountService = new UserAccountService(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
        }

        [Test]
        public async Task GetUserAccounts__ReturnAllUserAccount()
        {
            // Arrange

            // Act
            var result = await _userAccountService.GetUserAccounts();

            // Assert
            Assert.AreEqual(_userAccounts.Count, result.Count);
            for (int i = 0; i < result.Count; i++)
            {
                AssertUserAccount(_userAccounts[i], result[i]);
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task GetUserAccountById_ExistId_ReturnExistUserAccount(int id)
        {
            // Arrage

            // Act
            var result = await _userAccountService.GetUserAccountById(id);

            // Assert
            Assert.NotNull(result);
            var expectedUserAccount = _userAccounts.Find(c => c.Id == id);
            AssertUserAccount(expectedUserAccount, result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(3)]
        public async Task GetUserAccountById_NotExistId_ReturnNull(int id)
        {
            // Arrange

            // Act
            var result = await _userAccountService.GetUserAccountById(id);

            // Assert
            Assert.Null(result);
        }

        [Test]
        public async Task CreateUserAccount__SuccessAndReturnNewUserAccount()
        {
            // Arrange
            var newUserAccount = new CreateUserAccountDto()
            {
                Username = "test3",
                Password = "test",
                Name = "Chau",
                Gender = "Male",
                Role = "Admin",
                Status = "Active"
            };

            // Act
            var result = await _userAccountService.CreateUserAccount(newUserAccount);

            // Assert
            Assert.NotNull(result);
            AssertUserAccount(newUserAccount, result);
            var userAccountInDb = await _dbContextTest.UserAccounts.FindAsync(result.Id);
            Assert.NotNull(userAccountInDb);
            AssertUserAccount(result, userAccountInDb);
        }

        [Test]
        public async Task CreateUserAccount_ExistUsername_ThrowAlreadyExistUserAccountException()
        {
            try
            {
                // Arrange
                var newUserAccount = new CreateUserAccountDto()
                {
                    Username = "test",
                    Password = "test",
                    Name = "Chau",
                    Gender = "Male",
                    Role = "Admin",
                    Status = "Active"
                };

                // Act
                var result = await _userAccountService.CreateUserAccount(newUserAccount);

                // Assert
                Assert.Fail();
            }
            catch (AlreadyExistUserAccountException ex)
            {
                // Assert
                Assert.Pass();
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task UpdateUserAccount_ExistId_SuccessAndReturnUpdatedUserAccount(int id)
        {
            // Arrange
            var updateUserAccount = new UpdateUserAccountDto()
            {
                Name = "Chau",
                Gender = "Male",
                Role = "Admin",
                Status = "Active"
            };

            // Act
            var result = await _userAccountService.UpdateUserAccount(id, updateUserAccount);

            // Assert
            Assert.NotNull(result);
            AssertUserAccount(updateUserAccount, result);
            var userAccountInDb = await _dbContextTest.UserAccounts.FindAsync(result.Id);
            Assert.NotNull(userAccountInDb);
            AssertUserAccount(result, userAccountInDb);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task UpdateUserAccount_ExistIdAndNoUpdate_SuccessAndReturnUserAccount(int id)
        {
            // Arrange
            var updateUserAccount = new UpdateUserAccountDto();

            // Act
            var result = await _userAccountService.UpdateUserAccount(id, updateUserAccount);

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(3)]
        public async Task UpdateUserAccount_NotExistId_ThrowEntityNotFoundException(int id)
        {
            try
            {
                // Arrange
                var updateUserAccount = new UpdateUserAccountDto() { Name = "Updated name" };

                // Act
                var result = await _userAccountService.UpdateUserAccount(id, updateUserAccount);

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
        public async Task DeleteUserAccount_ExistId_SuccessAndNotExistInDb(int id)
        {
            // Arrange

            // Act
            await _userAccountService.DeleteUserAccount(id);

            // Assert
            var userAccountInDb = await _dbContextTest.Categories.FindAsync(id);
            Assert.Null(userAccountInDb);
        }

        [Test]
        [TestCase(0)]
        [TestCase(3)]
        public async Task DeleteUserAccount_NotExistId_ThrowEntityNotFoundException(int id)
        {
            try
            {
                // Arrange

                // Act
                await _userAccountService.DeleteUserAccount(id);

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
        [TestCase("test", "test")]
        public async Task GetUserAccountByUsernameAndPassword_ExistUserAccount_ReturnUserAccount(string username, string password)
        {
            // Arrange

            // Act
            var result = await _userAccountService.GetUserAccountByUsernameAndPassword(username, password);

            // Assert
            Assert.NotNull(result);
            var expectedUserAccount = _userAccounts.Find(c => c.Username == username && c.Password == password);
            AssertUserAccount(expectedUserAccount, result);
        }

        [Test]
        public async Task ChangePasswordUserAccount_ExistUserAccountAndCorrectCurrentPassword()
        {
            // Arrange
            var changePasswordDto = new ChangePasswordUserAccountDto()
            {
                CurrentPassword = "test",
                NewPassword = "testttt"
            };

            // Act
            await _userAccountService.ChangePasswordUserAccount(1, changePasswordDto);

            // Assert
            Assert.Pass();
        }

        [Test]
        public async Task ChangePasswordUserAccount_ExistUserAccountAndIncorrectCurrentPassword_ThrowCurrentPasswordIncorrectException()
        {
            try
            {
                // Arrange
                var changePasswordDto = new ChangePasswordUserAccountDto()
                {
                    CurrentPassword = "testt",
                    NewPassword = "testttt"
                };

                // Act
                await _userAccountService.ChangePasswordUserAccount(1, changePasswordDto);

                // Assert
                Assert.Fail();
            }
            catch (CurrentPasswordIncorrectException ex)
            {
                // Assert
                Assert.Pass();
            }
        }

        [Test]
        public async Task ChangePasswordUserAccount_NotExistUserAccount_Throw()
        {
            try
            {
                // Arrange
                var changePasswordDto = new ChangePasswordUserAccountDto()
                {
                    CurrentPassword = "testt",
                    NewPassword = "testttt"
                };

                // Act
                await _userAccountService.ChangePasswordUserAccount(0, changePasswordDto);

                // Assert
                Assert.Fail();
            }
            catch (EntityNotFoundException ex)
            {
                // Assert
                Assert.Pass();
            }
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

        private void AssertUserAccount(CreateUserAccountDto expected, UserAccount actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Username, actual.Username);
            Assert.AreEqual(expected.Password, actual.Password);
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
