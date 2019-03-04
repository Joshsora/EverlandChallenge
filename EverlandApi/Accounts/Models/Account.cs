﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EverlandApi.Accounts.Models
{
    public class Account
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MinLength(6)]
        public string Username { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 60)]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public static bool TryCreateFromRequest(
            AccountCreationRequest request,
            IPasswordHasher<Account> hasher,
            out Account account)
        {
            account = new Account
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email
            };
            account.Password = hasher.HashPassword(account, request.Password);

            ICollection<ValidationResult> results = new Collection<ValidationResult>();
            return Validator.TryValidateObject(account, new ValidationContext(account), results);
        }
    }
}
