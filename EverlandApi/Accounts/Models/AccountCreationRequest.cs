using System.ComponentModel.DataAnnotations;

namespace EverlandApi.Accounts.Models
{
    public class AccountCreationRequest
    {
        [Required]
        [MinLength(6, ErrorMessage = "Your username must be at least {1} characters in length.")]
        public string Username { get; set; }

        [Required]
        [StringLength(72, MinimumLength = 8, ErrorMessage = "Your password must be between {2}-{1} characters.")]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
