using Jawel_be.Dtos.CustomerAccount;
using Jawel_be.Models;

namespace Jawel_be.Services.CustomerAccountService
{
    public interface ICustomerAccountService
    {
        Task<List<CustomerAccount>> GetCustomerAccounts();
        Task<CustomerAccount?> GetCustomerAccountById(int id);
        Task<CustomerAccount?> GetCustomerAccountByPhoneAndPassword(string phone, string password);
        Task<CustomerAccount> CreateCustomerAccount(CreateCustomerAccountDto createCustomerAccountDto);
        Task<CustomerAccount> UpdateCustomerAccount(int id, UpdateCustomerAccountDto updateCustomerAccountDto);
        Task ChangePasswordCustomerAccount(int id, ChangePasswordCustomerAccountDto changePasswordCustomerAccountDto);
        Task DeleteCustomerAccount(int id);
    }
}
