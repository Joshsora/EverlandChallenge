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
        PasswordVerificationResult VerifyPassword(Account account, string password);
        Task UpdatePasswordAsync(Account account, string password);
    }
}
