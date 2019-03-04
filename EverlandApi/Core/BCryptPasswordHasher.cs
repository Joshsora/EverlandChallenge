using BCrypt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace EverlandApi.Core
{
    public class BCryptPasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
    {
        private BCryptOptions _options;

        public BCryptPasswordHasher(
            IOptionsMonitor<BCryptOptions> configuration)
        {
            _options = configuration.CurrentValue;
        }

        public string HashPassword(TUser user, string password)
        {
            string salt = BCryptHelper.GenerateSalt(
                _options.SaltWorkFactor,
                _options.SaltRevision
            );
            return BCryptHelper.HashPassword(password, salt);
        }

        public PasswordVerificationResult VerifyHashedPassword(
            TUser user,
            string hashedPassword,
            string providedPassword
        ) => BCryptHelper.CheckPassword(providedPassword, hashedPassword) ?
            PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
    }
}
