using Jawel_be.Contexts;
using Jawel_be.Dtos.UserAccount;
using Jawel_be.Exceptions;
using Jawel_be.Models;
using Microsoft.EntityFrameworkCore;

namespace Jawel_be.Services.UserAccountService
{
    public class UserAccountService : IUserAccountService
    {
        private readonly DataContext _dbContext;

        public UserAccountService(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<UserAccount>> GetUserAccounts()
        {
            return await _dbContext.UserAccounts.ToListAsync();
        }

        public async Task<UserAccount?> GetUserAccountById(int id)
        {
            return await _dbContext.UserAccounts.FindAsync(id);
        }

        public async Task<UserAccount?> GetUserAccountByUsernameAndPassword(string username, string password)
        {
            return await _dbContext.UserAccounts
                .Where(u => u.Username == username && u.Password == password)
                .SingleOrDefaultAsync();
        }

        public async Task<UserAccount> CreateUserAccount(CreateUserAccountDto createUserAccountDto)
        {
            var existingUserAccount = await _dbContext.UserAccounts
                .Where(u => u.Username == createUserAccountDto.Username)
                .SingleOrDefaultAsync();

            if (existingUserAccount != null)
            {
                throw new AlreadyExistUserAccountException();
            }
            var newUserAccount = new UserAccount 
            { 
                Username = createUserAccountDto.Username,
                Password = createUserAccountDto.Password,
                Name = createUserAccountDto.Name,
                Gender = createUserAccountDto.Gender,
                Avatar = createUserAccountDto.Avatar,
                Role = createUserAccountDto.Role,
                Status = createUserAccountDto.Status
            };
            _dbContext.UserAccounts.Add(newUserAccount);
            await _dbContext.SaveChangesAsync();
            return newUserAccount;
        }

        public async Task<UserAccount> UpdateUserAccount(int id, UpdateUserAccountDto updateUserAccountDto)
        {
            var existingUserAccount = await _dbContext.UserAccounts.FindAsync(id);

            if (existingUserAccount != null)
            {
                if (updateUserAccountDto.Name != null)
                {
                    existingUserAccount.Name = updateUserAccountDto.Name;
                }
                if (updateUserAccountDto.Gender != null)
                {
                    existingUserAccount.Gender = updateUserAccountDto.Gender;
                }
                if (updateUserAccountDto.Avatar != null)
                {
                    existingUserAccount.Avatar = updateUserAccountDto.Avatar;
                }
                if (updateUserAccountDto.Status != null)
                {
                    existingUserAccount.Status = updateUserAccountDto.Status;
                }
                await _dbContext.SaveChangesAsync();
                return existingUserAccount;
            }
            else
            {
                throw new EntityNotFoundException();
            }
        }

        public async Task ChangePasswordUserAccount(int id, ChangePasswordUserAccountDto changePasswordUserAccountDto)
        {
            var existingUserAccount = await _dbContext.UserAccounts.FindAsync(id);

            if (existingUserAccount != null)
            {
                if (existingUserAccount.Password != changePasswordUserAccountDto.CurrentPassword)
                {
                    throw new CurrentPasswordIncorrectException();
                }
                existingUserAccount.Password = changePasswordUserAccountDto.NewPassword;
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new EntityNotFoundException();
            }
        }

        public async Task DeleteUserAccount(int id)
        {
            var userAccount = await _dbContext.UserAccounts.FindAsync(id);
            if (userAccount != null)
            {
                _dbContext.UserAccounts.Remove(userAccount);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new EntityNotFoundException();
            }
        }
    }

}
