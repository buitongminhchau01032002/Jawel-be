using Jawel_be.Contexts;
using Jawel_be.Dtos.CustomerAccount;
using Jawel_be.Exceptions;
using Jawel_be.Models;
using Jawel_be.Services.CustomerAccountService;
using Microsoft.EntityFrameworkCore;

namespace Jawal_beTests.ServiceTests
{
    public class CustomerAccountServiceTest
    {
        private List<CustomerAccount> _customers;
        private CustomerAccountService _customerAccountService;
        private DataContext _dbContext;
        private DataContext _dbContextTest;

        

        [SetUp]
        public void SetUp()
        {
            _customers = new List<CustomerAccount>()
            {
                new CustomerAccount {Id=1, Phone = "1", Name = "Tester1", Password="1", Gender="Nam" },
                 new CustomerAccount {Id=2, Phone = "2", Name = "Tester2", Password="2", Gender="Nam" }
            };
            // Setup test db
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "testDB")
                .Options;
            _dbContext = new DataContext(options);
            _dbContextTest = new DataContext(options);
            _dbContext.Database.EnsureCreated();
            foreach (var customer in _customers)
            {
                _dbContext.CustomerAccounts.Add(customer);
            }
            _dbContext.SaveChanges();

            // Setup service
            _customerAccountService = new CustomerAccountService(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
        }

        [Test]
        public async Task GetCustomerAccounts__ReturnAllCustomers()
        {
            // Arrange

            // Act
            var result = await _customerAccountService.GetCustomerAccounts();

            // Assert
            Assert.AreEqual(_customers.Count, result.Count);
            for (int i = 0; i < result.Count; i++)
            {
                AssertCustomerAccount(_customers[i], result[i]);
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task GetCustomerAccountById_ExistId_ReturnExistCustomerAccount(int id)
        {
            // Arrage

            // Act
            var result = await _customerAccountService.GetCustomerAccountById(id);

            // Assert
            Assert.NotNull(result);
            var expectedCustomerAccount = _customers.Find(c => c.Id == id);
            AssertCustomerAccount(expectedCustomerAccount, result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(3)]
        public async Task GetCustomerAccountById_NotExistId_ReturnNull(int id)
        {
            // Arrange

            // Act
            var result = await _customerAccountService.GetCustomerAccountById(id);

            // Assert
            Assert.Null(result);
        }

        [Test]
        public async Task CreateCustomerAccount__SuccessAndReturnNewCustomerAccount()
        {
            // Arrange
            var newCustomerAccount = new CreateCustomerAccountDto() { Phone = "3", Name = "Tester3", Password = "3", Gender = "Nam" };

            // Act
            var result = await _customerAccountService.CreateCustomerAccount(newCustomerAccount);

            // Assert
            Assert.NotNull(result);

            AssertCustomerAccount(newCustomerAccount, result);
            var customerAccountInDb = await _dbContextTest.CustomerAccounts.FindAsync(result.Id);
            Assert.NotNull(customerAccountInDb);
            AssertCustomerAccount(result, customerAccountInDb);
        }
        [Test]

        public async Task CreateCustomerAccount_AlreadyExistCustomerAccount_ThrowAlreadyExistCustomerAccountException()
        {
            try {
            //Arange
            var newCustomerAccount = new CreateCustomerAccountDto() { Phone = "1", Name = "Tester3", Password = "3", Gender = "Nam" };
                //Act
                await _customerAccountService.CreateCustomerAccount(newCustomerAccount);
                //Assert
                Assert.Fail();
            }
            catch(AlreadyExistCustomerAccountException ax)
            {
                Assert.Pass();
            }
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task UpdateCustomerAccount_ExistIdAndUpdateName_SuccessAndReturnUpdatedCustomerAccount(int id)
        {
            // Arrange
            var updateCustomerAccount = new UpdateCustomerAccountDto() { Name = "Updated name", Gender = "Updated Gender", Address="Updated Adress" };

            // Act
            var result = await _customerAccountService.UpdateCustomerAccount(id, updateCustomerAccount);

            // Assert
            Assert.NotNull(result);
            AssertCustomerAccount(updateCustomerAccount, result);
            var customerAccountInDb = await _dbContextTest.CustomerAccounts.FindAsync(result.Id);
            Assert.NotNull(customerAccountInDb);
            AssertCustomerAccount(result, customerAccountInDb);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public async Task UpdateCustomerAccount_ExistIdAndNoUpdate_SuccessAndReturnCustomerAccount(int id)
        {
            // Arrange
            var updateCustomerAccount = new UpdateCustomerAccountDto();

            // Act
            var result = await _customerAccountService.UpdateCustomerAccount(id, updateCustomerAccount);

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        [TestCase(0)]
        [TestCase(3)]
        public async Task UpdateCustomerAccount_NotExistId_ThrowEntityNotFoundException(int id)
        {
            try
            {
                // Arrange
                var updateCustomerAccount = new UpdateCustomerAccountDto() { Name = "Updated name", Gender = "Updated Gender", Address="Updated Address" };

                // Act
                var result = await _customerAccountService.UpdateCustomerAccount(id, updateCustomerAccount);

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
        public async Task DeleteCustomerAccount_ExistId_SuccessAndNotExistInDb(int id)
        {
            // Arrange

            // Act
            await _customerAccountService.DeleteCustomerAccount(id);


            // Assert
            var customerAccountInDb = await _dbContextTest.CustomerAccounts.FindAsync(id);

            Assert.Null(customerAccountInDb);
        }

        [Test]
        [TestCase(0)]
        [TestCase(3)]
        public async Task DeleteCustomerAccount_NotExistId_ThrowEntityNotFoundException(int id)
        {
            try
            {
                // Arrange

                // Act
                await _customerAccountService.DeleteCustomerAccount(id);


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
        [TestCase("1", "1")]
        [TestCase("2", "2")]
        public async Task GetCustomerAccountByPhoneAndPassword_ExistPhoneAndPassword_ReturnExistCustomerAccount(string phone, string password)
        {
            // Arrage

            // Act
            var result = await _customerAccountService.GetCustomerAccountByPhoneAndPassword(phone, password);

            // Assert
            Assert.NotNull(result);
            var expectedCustomerAccount = _customers.Find(c => c.Phone == phone && c.Password == password);
            AssertCustomerAccount(expectedCustomerAccount, result);

        }

        [Test]
        [TestCase("0", "0")]
        [TestCase("3", "3")]
        public async Task GetCustomerAccountByPhoneAndPassword_NotExistPhoneAndPassword_ReturnNull(string phone, string password)
        {
            var result = await _customerAccountService.GetCustomerAccountByPhoneAndPassword(phone, password);

            // Assert
            Assert.Null(result);

        }


        [Test]
        [TestCase(1)]

        public async Task ChangePasswordCustomerAccount_ExistIdAndValidCurrentPasswordAndValidNewPassword_SuccessAndChangePassword(int id)
        {
            // Arrange
            var changePasswordCustomerAccount = new ChangePasswordCustomerAccountDto() { CurrentPassword = "1", NewPassword = "3" };


            // Act
            await _customerAccountService.ChangePasswordCustomerAccount(id, changePasswordCustomerAccount);



            // Assert
            Assert.Pass();
        }
        
       
        [Test]
        [TestCase(0)]
        [TestCase(3)]
        public async Task ChangePasswordCustomerAccount_NotExistId_ThrowEntityNotFoundException(int id)
        {
            try
            {
                // Arrange
                var changePasswordCustomerAccount = new ChangePasswordCustomerAccountDto() { CurrentPassword = "1", NewPassword = "3" };

                // Act
                 await _customerAccountService.ChangePasswordCustomerAccount(id, changePasswordCustomerAccount);

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
        public async Task ChangePasswordCustomerAccount_ExistIdAndInvalidCurrentPassword_CurrentPasswordIncorrectException(int id)
        {
            try
            {
                // Arrange
                var changePasswordCustomerAccount = new ChangePasswordCustomerAccountDto() { CurrentPassword = "2", NewPassword = "3" };

                // Act
                await _customerAccountService.ChangePasswordCustomerAccount(id, changePasswordCustomerAccount);

                // Assert
                Assert.Fail();
            }
            catch (CurrentPasswordIncorrectException cx)
            {
                // Assert
                Assert.Pass();
            }
        }



        private void AssertCustomerAccount(CustomerAccount expected, CustomerAccount actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Password, actual.Password);
            Assert.AreEqual(expected.Gender, actual.Gender);
            Assert.AreEqual(expected.Address, actual.Address);
            Assert.AreEqual(expected.Avatar, actual.Avatar);
        }

        private void AssertCustomerAccount(CreateCustomerAccountDto expected, CustomerAccount actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Password, actual.Password);
            Assert.AreEqual(expected.Gender, actual.Gender);
            Assert.AreEqual(expected.Address, actual.Address);
            Assert.AreEqual(expected.Avatar, actual.Avatar);
        }

        private void AssertCustomerAccount(UpdateCustomerAccountDto expected, CustomerAccount actual)
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
