using System;
using System.Threading.Tasks;
using EverlandApi.Accounts.Models;
using Microsoft.AspNetCore.Identity;

namespace EverlandApi.Accounts.Services
{
    public interface IAccountService
    {
        Task<Account> CreateAsync(AccountCreationRequest request);
        Task<Account> GetAsync(Guid id);
        Task<Account> GetAsync(string username);
        Task UpdateAsync(Account account, AccountUpdateRequest request);
        Task DeleteAsync(Account account);

        Task<bool> IsUsernameTaken(string username);
        Task<bool> IsEmailInUse(string email);

        PasswordVerificationResult VerifyPassword(Account account, string password);
    }
}
