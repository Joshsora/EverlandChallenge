using System;
using System.ComponentModel.DataAnnotations;

namespace EverlandApi.Accounts.Models
{
    public class AccountCreationResponse
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public bool VerificationEmailSent { get; set; }

        [Required]
        public bool EmailVerificationRequired { get; set; }
    }
}
