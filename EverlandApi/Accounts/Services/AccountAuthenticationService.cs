using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using EverlandApi.Accounts.Models;
using EverlandApi.Core.Services;
using EverlandApi.Core.Models;

namespace EverlandApi.Accounts.Services
{
    public class AccountAuthenticationService : IAuthenticationService<Account>
    {
        private class AuthorizationHeader
        {
            public AuthenticationMethod Method { get; private set; }
            public string Credentials { get; private set; }

            public AuthorizationHeader(string headerValue)
            {
                string[] parts = headerValue.Split(' ');
                if (parts.Length < 2)
                    throw new AuthenticationException(
                        "Invalid Authorization header.",
                        AuthenticationErrorCode.InvalidHeader
                    );

                string authMethodStr = parts[0];
                if (!Enum.TryParse<AuthenticationMethod>(authMethodStr, out var authMethod))
                    throw new AuthenticationException(
                        $"Unknown authentication method: {authMethodStr}",
                        AuthenticationErrorCode.UnknownAuthenticationMethod
                    );
                Method = authMethod;
                Credentials = parts[1];
            }
        }

        private IAccountService _accountService;

        public AccountAuthenticationService(IAccountService accountService)
        {
            _accountService = accountService;
        }

        private AuthorizationHeader PickBestAuthenticationMethod(
            IEnumerable<AuthorizationHeader> options)
        {
            // TODO: Prefer token-based authentication
            return options.FirstOrDefault();
        }

        public async Task<Account> AuthenticateAsync(HttpContext httpContext)
        {
            // httpContext is a required parameter
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            // If there is not a Authorization header in the request, then we
            // cannot authenticate.
            if (!httpContext.Request.Headers
                .TryGetValue("Authorization", out var authorizationValues))
            {
                throw new AuthenticationException(
                    "The HTTP request did not contain a 'Authorization' header.",
                    AuthenticationErrorCode.MissingHeader
                );
            }

            // Pick the most secure/fastest way to authenticate if there are multiple
            // options.
            AuthorizationHeader auth = PickBestAuthenticationMethod(
                authorizationValues.Select(s => new AuthorizationHeader(s))
            );
            if (auth == null)
                throw new AuthenticationException(
                    "An 'Authorization' header was supplied but no usable authentication method was found.",
                    AuthenticationErrorCode.NoUsableMethodFound
                );
            switch (auth.Method)
            {
                case AuthenticationMethod.Basic:
                    return await HandleBasicAuthentication(httpContext, auth.Credentials);

                default:
                    throw new AuthenticationException(
                        "Unhandled authentication method.",
                        AuthenticationErrorCode.UnhandledMethod
                    );
            }
        }

        private async Task<Account> HandleBasicAuthentication(
            HttpContext httpContext, string credentials)
        {
            // TODO: Do not allow Basic authentication through HTTP

            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));
            if (credentials == null)
                throw new ArgumentNullException(nameof(credentials));

            // Get the username and password from the credentials string
            byte[] data = Convert.FromBase64String(credentials);
            string[] parts = Encoding.UTF8.GetString(data).Split(':');
            if (parts.Length < 2)
                throw new AuthenticationException(
                    "Invalid Basic authentication credentials.",
                    AuthenticationErrorCode.MalformedCredentials
                );
            string username = parts[0], password = parts[1];

            // Does a user with this username exist?
            Account account = await _accountService.GetAsync(username);
            if (account == null)
                throw new AuthenticationException(
                    "Invalid username and/or password.",
                    AuthenticationErrorCode.InvalidCredentials
                );

            // Did they provide the correct password?
            var authResult = _accountService.VerifyPassword(account, password);
            switch (authResult)
            {
                case PasswordVerificationResult.Success:
                    return account;

                case PasswordVerificationResult.SuccessRehashNeeded:
                    await _accountService.UpdatePasswordAsync(account, password);
                    return account;

                default:
                    throw new AuthenticationException(
                        "Invalid username and/or password.",
                        AuthenticationErrorCode.InvalidCredentials
                    );
            }
        }
    }
}
