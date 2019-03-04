using System;
using System.ComponentModel.DataAnnotations;

namespace EverlandApi.Accounts.Models
{
    public class AccountCreationResponse
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public bool ValidationEmailSent { get; set; }

        [Required]
        public bool EmailValidationRequired { get; set; }
    }
}
