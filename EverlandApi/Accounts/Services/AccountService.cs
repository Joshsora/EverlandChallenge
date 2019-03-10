using System;
using System.Threading.Tasks;
using EverlandApi.Accounts.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EverlandApi.Accounts.Services
{
    public class AccountService : IAccountService
    {
        private AccountContext _accountContext;
        private IPasswordHasher<Account> _passwordHasher;

        public AccountService(
            AccountContext accountContext,
            IPasswordHasher<Account> passwordHasher)
        {
            _accountContext = accountContext;
            _passwordHasher = passwordHasher;
        }

        public async Task<Account> CreateAsync(
            AccountCreationRequest request)
        {
            var errorFlags = AccountCreationErrorFlags.None;

            // Attempt to create an Account instance from the request data
            if (!Account.TryCreateFromRequest(request, _passwordHasher, out var account))
                errorFlags |= AccountCreationErrorFlags.InvalidAccount;

            // Make sure that an account does not exist with the same username
            if (await IsUsernameTaken(account.Username))
                errorFlags |= AccountCreationErrorFlags.UsernameTaken;

            // Make sure that an account does not exist with the same email
            if (await IsEmailInUse(account.Email))
                errorFlags |= AccountCreationErrorFlags.EmailInUse;

            // If there was a confliction, do not attempt to insert into the database
            if (errorFlags != AccountCreationErrorFlags.None)
                throw new AccountCreationException(
                    "An error occurred while creating the account.",
                    errorFlags
                );

            // Attempt to add the new account to the database
            _accountContext.Add(account);
            await _accountContext.SaveChangesAsync();
            return account;
        }

        public async Task<Account> GetAsync(Guid id)
        {
            try
            {
                return await _accountContext.Accounts
                    .FirstAsync(a => a.Id == id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public async Task<Account> GetAsync(string username)
        {
            try
            {
                return await _accountContext.Accounts
                    .FirstAsync(a => a.Username == username);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public async Task UpdatePasswordAsync(Account account, string password)
        {
            account.Password = _passwordHasher.HashPassword(account, password);
            await _accountContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Account account, AccountUpdateRequest request)
        {
            AccountUpdateErrorFlags errorFlags = AccountUpdateErrorFlags.None;

            if (request.Password != null)
                account.Password = _passwordHasher.HashPassword(account, request.Password);
            if (request.Email != null && account.Email != request.Email)
            {
                if (await IsEmailInUse(request.Email))
                    errorFlags |= AccountUpdateErrorFlags.EmailInUse;
                else
                    account.Email = request.Email;
            }

            if (errorFlags != AccountUpdateErrorFlags.None)
                throw new AccountUpdateException(
                    "An error occurred while updating the account.",
                    errorFlags
                );

            return await _accountContext.SaveChangesAsync() >= 1;
        }

        public async Task DeleteAsync(Account account)
        {
            _accountContext.Remove(account);
            await _accountContext.SaveChangesAsync();
        }

        public async Task<bool> IsUsernameTaken(string username)
        {
            return (await _accountContext.Accounts
                .CountAsync(a => a.Username == username)) >= 1;
        }

        public async Task<bool> IsEmailInUse(string email)
        {
            return (await _accountContext.Accounts
                       .CountAsync(a => a.Email == email)) >= 1;
        }

        public PasswordVerificationResult VerifyPassword(Account account, string password)
            => _passwordHasher.VerifyHashedPassword(account, account.Password, password);
    }
}
