using System.ComponentModel.DataAnnotations;

namespace EverlandApi.Accounts.Models
{
    public class AccountCreationRequest
    {
        [Required]
        [MinLength(6, ErrorMessage = "Your username must be at least {0} characters in length.")]
        public string Username { get; set; }

        [Required]
        [StringLength(72, MinimumLength = 3, ErrorMessage = "Your password must be between {0}-{1} characters.")]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
