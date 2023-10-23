using Jawel_be.Dtos.UserAccount;
using Jawel_be.Models;

namespace Jawel_be.Services.UserAccountService
{
    public interface IUserAccountService
    {
        Task<List<UserAccount>> GetUserAccounts();
        Task<UserAccount?> GetUserAccountById(int id);
        Task<UserAccount?> GetUserAccountByUsernameAndPassword(string username, string password);
        Task<UserAccount> CreateUserAccount(CreateUserAccountDto createUserAccountDto);
        Task<UserAccount> UpdateUserAccount(int id, UpdateUserAccountDto updateUserAccountDto);
        Task ChangePasswordUserAccount(int id, ChangePasswordUserAccountDto changePasswordUserAccountDto);
        Task DeleteUserAccount(int id);
    }
}
