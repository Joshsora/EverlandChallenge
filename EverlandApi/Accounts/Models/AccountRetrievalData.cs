using System;

namespace EverlandApi.Accounts.Models
{
    public class AccountRetrievalData
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public static AccountRetrievalData FromAccount(Account account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            return new AccountRetrievalData
            {
                Id = account.Id,
                Username = account.Username,
                Email = account.Email
            };
        }
    }
}
