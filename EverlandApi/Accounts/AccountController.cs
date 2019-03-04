using System.Collections.Generic;
using System.Threading.Tasks;
using EverlandApi.Accounts.Models;
using EverlandApi.Core;
using EverlandApi.Core.Filters;
using EverlandApi.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace EverlandApi.Accounts
{
    [Route("api/v1/account")]
    public class AccountController : ApiController
    {
        private AccountContext _accountContext;
        private IPasswordHasher<Account> _passwordHasher;

        public AccountController(
            AccountContext accountContext,
            IPasswordHasher<Account> passwordHasher)
        {
            _accountContext = accountContext;
            _passwordHasher = passwordHasher;
        }

        [ValidateModel]
        [HttpPost("create")]
        public async Task<ActionResult> Create(
            [FromBody, BindRequired] AccountCreationRequest request)
        {
            // Attempt to create an Account instance from the request data
            if (!Account.TryCreateFromRequest(request, _passwordHasher, out var account))
            {
                // The account is in some way invalid, even though the request is well-formed
                return UnprocessableEntity(new ApiResult(
                    new ApiError(
                        "Unable to create valid Account with supplied data.",
                        ApiErrorCode.AccountCreationFailed
                    )
                ));
            }

            try
            {
                // Make sure that an account does not exist with the same username
                ICollection<ApiError> errors = new List<ApiError>();
                if ((await _accountContext.Accounts
                        .CountAsync(a => a.Username == account.Username)) >= 1)
                {
                    errors.Add(new ApiError(
                        "An account has already been created under that username.",
                        ApiErrorCode.AccountUsernameInUse
                    ));
                }

                // Make sure that an account does not exist with the same email
                if ((await _accountContext.Accounts
                        .CountAsync(a => a.Email == account.Email)) >= 1)
                {
                    errors.Add(new ApiError(
                        "An account has already been created under that email address.",
                        ApiErrorCode.AccountEmailInUse
                    ));
                }

                // If there was a confliction, do not attempt to insert into the database
                if (errors.Count > 0)
                    return Conflict(new ApiResult(errors));

                // Attempt to add the new account to the database
                _accountContext.Add(account);
                await _accountContext.SaveChangesAsync();
            }
            catch (DbUpdateException e) when
                (e.InnerException is MySqlException mySqlException)
            {
                return HandleMySqlException(mySqlException);
            }
            catch (MySqlException e)
            {
                return HandleMySqlException(e);
            }

            // Everything went okay!
            return Ok(new ApiResult<AccountCreationResponse>(
                new AccountCreationResponse
                {
                    Id = account.Id,

                    // TODO: Email verification
                    EmailVerificationRequired = false,
                    VerificationEmailSent = false
                }
            ));
        }
    }
}
