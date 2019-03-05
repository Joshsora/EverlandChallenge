using System;

namespace EverlandApi.Accounts.Models
{
    public class AccountRetrievalResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public static AccountRetrievalResponse FromAccount(Account account)
        {
            return new AccountRetrievalResponse
            {
                Id = account.Id,
                Username = account.Username,
                Email = account.Email
            };
        }
    }
}
