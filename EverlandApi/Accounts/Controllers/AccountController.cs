using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EverlandApi.Accounts.Attributes;
using EverlandApi.Accounts.Filters;
using EverlandApi.Accounts.Models;
using EverlandApi.Accounts.Services;
using EverlandApi.Core;
using EverlandApi.Core.Filters;
using EverlandApi.Core.Models;
using EverlandApi.Core.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EverlandApi.Accounts.Controllers
{
    [Route("api/v1/accounts")]
    public class AccountController : ApiController
    {
        private IAccountService _accountService;

        public AccountController(
            IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        [ValidateModel]
        public async Task<ApiResult> Create(
            [FromBody, BindRequired] AccountCreationRequest request)
        {
            try
            {
                // Attempt to create the account through the AccountService
                // implementation we were provided
                Account account = await _accountService.CreateAsync(request);
                return Success(AccountCreationData.FromAccount(
                    account,

                    // TODO: Email verification
                    emailVerificationRequired: false,
                    verificationEmailSent: false
                ));
            }
            catch (AccountCreationException e)
            {
                var errors = new List<ApiError>();

                if ((e.ErrorFlags & AccountCreationErrorFlags.InvalidAccount) != 0)
                    errors.Add(new ApiError(
                        "Unable to create valid Account with supplied data.",
                        ApiErrorCode.AccountCreationFailed
                    ));
                if ((e.ErrorFlags & AccountCreationErrorFlags.UsernameTaken) != 0)
                    errors.Add(new ApiError(
                        "An account has already been created under that username.",
                        ApiErrorCode.AccountUsernameInUse
                    ));
                if ((e.ErrorFlags & AccountCreationErrorFlags.EmailInUse) != 0)
                    errors.Add(new ApiError(
                        "An account has already been created under that email address.",
                        ApiErrorCode.AccountEmailInUse
                    ));

                return Error(
                    StatusCodes.Status422UnprocessableEntity,
                    errors
                );
            }
        }

        [HttpGet]
        [ServiceFilter(typeof(RequiresAccount))]
        public ActionResult Get(
            [AccountTarget] Account account)
        {
            // Authentication succeeded, send their account information
            return Success(
                AccountRetrievalResponse.FromAccount(account)
            );
        }

        [HttpGet("{id}")]
        [ServiceFilter(typeof(RequiresApiKey))]
        public async Task<ActionResult> Get(Guid id)
        {
            Account account = await _accountService.GetAsync(id);
            if (account == null)
                return Error(
                    StatusCodes.Status404NotFound,
                    new ApiError(
                        "An account does not exist with the specified id.",
                        ApiErrorCode.NotFound
                    )
                );
            return Success(
                AccountRetrievalResponse.FromAccount(account)
            );
        }

        [HttpGet("username/{username}")]
        [ServiceFilter(typeof(RequiresApiKey))]
        public async Task<ActionResult> Get(string username)
        {
            Account account = await _accountService.GetAsync(username);
            if (account == null)
                return Error(
                    StatusCodes.Status404NotFound,
                    new ApiError(
                        "An account does not exist with the specified username.",
                        ApiErrorCode.NotFound
                    )
                );
            return Success(
                AccountRetrievalResponse.FromAccount(account)
            );
        }
    }
}
