using System.ComponentModel.DataAnnotations;

namespace EverlandApi.Accounts.Models
{
    public class AccountUpdateRequest
    {
        [StringLength(72, MinimumLength = 8, ErrorMessage = "Your password must be between {2}-{1} characters.")]
        public string Password { get; set; }

        [EmailAddress]
        public string Email { get; set; }
    }
}
