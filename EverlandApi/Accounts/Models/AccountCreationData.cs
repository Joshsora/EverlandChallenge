using System;
using System.ComponentModel.DataAnnotations;

namespace EverlandApi.Accounts.Models
{
    public class AccountCreationData
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public bool VerificationEmailSent { get; set; }

        [Required]
        public bool EmailVerificationRequired { get; set; }

        public static AccountCreationData FromAccount(Account account,
            bool verificationEmailSent, bool emailVerificationRequired)
        {
            return new AccountCreationData
            {
                Id = account.Id,
                VerificationEmailSent = verificationEmailSent,
                EmailVerificationRequired = emailVerificationRequired
            };
        }
    }
}
