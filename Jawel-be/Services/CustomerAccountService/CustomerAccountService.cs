using Jawel_be.Contexts;
using Jawel_be.Dtos.CustomerAccount;
using Jawel_be.Exceptions;
using Jawel_be.Models;
using Microsoft.EntityFrameworkCore;

namespace Jawel_be.Services.CustomerAccountService
{
    public class CustomerAccountService : ICustomerAccountService
    {
        private readonly DataContext _dbContext;

        public CustomerAccountService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CustomerAccount>> GetCustomerAccounts()
        {
            return await _dbContext.CustomerAccounts.ToListAsync();
        }

        public async Task<CustomerAccount?> GetCustomerAccountById(int id)
        {
            return await _dbContext.CustomerAccounts.FindAsync(id);
        }

        public async Task<CustomerAccount?> GetCustomerAccountByPhoneAndPassword(string phone, string password)
        {
            return await _dbContext.CustomerAccounts
                .Where(u => u.Phone == phone && u.Password == password)
                .SingleOrDefaultAsync();
        }

        public async Task<CustomerAccount> CreateCustomerAccount(CreateCustomerAccountDto createCustomerAccountDto)
        {
            var existingCustomerAccount = await _dbContext.CustomerAccounts
                .Where(u => u.Phone == createCustomerAccountDto.Phone)
                .SingleOrDefaultAsync();

            if (existingCustomerAccount != null)
            {
                throw new AlreadyExistCustomerAccountException();
            }
            var newCustomerAccount = new CustomerAccount
            {
                Phone = createCustomerAccountDto.Phone,
                Password = createCustomerAccountDto.Password,
                Name = createCustomerAccountDto.Name,
                Gender = createCustomerAccountDto.Gender,
                Avatar = createCustomerAccountDto.Avatar,
                Address = createCustomerAccountDto.Address
            };
            _dbContext.CustomerAccounts.Add(newCustomerAccount);
            await _dbContext.SaveChangesAsync();
            return newCustomerAccount;
        }

        public async Task<CustomerAccount> UpdateCustomerAccount(int id, UpdateCustomerAccountDto updateCustomerAccountDto)
        {
            var existingCustomerAccount = await _dbContext.CustomerAccounts.FindAsync(id);

            if (existingCustomerAccount != null)
            {
                if (updateCustomerAccountDto.Name != null)
                {
                    existingCustomerAccount.Name = updateCustomerAccountDto.Name;
                }
                if (updateCustomerAccountDto.Gender != null)
                {
                    existingCustomerAccount.Gender = updateCustomerAccountDto.Gender;
                }
                if (updateCustomerAccountDto.Address != null)
                {
                    existingCustomerAccount.Address = updateCustomerAccountDto.Address;
                }
                await _dbContext.SaveChangesAsync();
                return existingCustomerAccount;
            }
            else
            {
                throw new EntityNotFoundException();
            }
        }

        public async Task ChangePasswordCustomerAccount(int id, ChangePasswordCustomerAccountDto changePasswordCustomerAccountDto)
        {
            var existingCustomerAccount = await _dbContext.CustomerAccounts.FindAsync(id);

            if (existingCustomerAccount != null)
            {
                if (existingCustomerAccount.Password != changePasswordCustomerAccountDto.CurrentPassword)
                {
                    throw new CurrentPasswordIncorrectException();
                }
                existingCustomerAccount.Password = changePasswordCustomerAccountDto.NewPassword;
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new EntityNotFoundException();
            }
        }

        public async Task DeleteCustomerAccount(int id)
        {
            var customerAccount = await _dbContext.CustomerAccounts.FindAsync(id);
            if (customerAccount != null)
            {
                _dbContext.CustomerAccounts.Remove(customerAccount);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new EntityNotFoundException();
            }
        }
    }

}
