using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EverlandApi.Accounts.Binders;
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
        private const int AccountGetCacheExpire = 60;

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
        [ValidateModel]
        [CacheApiResult("Get:Account:Id:{account.Id}", expire: AccountGetCacheExpire)]
        public ActionResult Get(
            [BindRequired, ModelBinder(
                BinderType = typeof(AuthorizedAccountBinder)
            )] Account account)
        {
            // Authentication succeeded, send their account information
            return Success(
                AccountRetrievalData.FromAccount(account)
            );
        }

        [HttpGet("{id}")]
        [ServiceFilter(typeof(RequiresApiKey))]
        [CacheApiResult("Get:Account:Id:{id}", expire: AccountGetCacheExpire)]
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
                AccountRetrievalData.FromAccount(account)
            );
        }

        [HttpGet("username/{username}")]
        [ServiceFilter(typeof(RequiresApiKey))]
        [CacheApiResult("Get:Account:Username:{username}", expire: AccountGetCacheExpire)]
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
                AccountRetrievalData.FromAccount(account)
            );
        }

        private ActionResult HandleAccountUpdateException(
            AccountUpdateException e)
        {
            var errors = new List<ApiError>();

            if ((e.ErrorFlags & AccountUpdateErrorFlags.EmailInUse) != 0)
                errors.Add(new ApiError(
                    "An account already exists under that email address.",
                    ApiErrorCode.AccountEmailInUse
                ));

            return Error(
                StatusCodes.Status422UnprocessableEntity,
                errors
            );
        }

        [HttpPut]
        [ValidateModel]
        public async Task<ActionResult> Update(
            [ModelBinder(
                 BinderType = typeof(AuthorizedAccountBinder)
             ), BindRequired] Account account,
            [FromBody, BindRequired] AccountUpdateRequest request)
        {
            try
            {
                await _accountService.UpdateAsync(account, request);
                return Success();
            }
            catch (AccountUpdateException e)
            {
                return HandleAccountUpdateException(e);
            }
        }

        [HttpPut("{id}")]
        [ValidateModel]
        [ServiceFilter(typeof(RequiresApiKey))]
        public async Task<ActionResult> Update(
            Guid id,
            [FromBody, BindRequired] AccountUpdateRequest request)
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

            try
            {
                await _accountService.UpdateAsync(account, request);
                return Success();
            }
            catch (AccountUpdateException e)
            {
                return HandleAccountUpdateException(e);
            }
        }

        [HttpPut("username/{username}")]
        [ValidateModel]
        [ServiceFilter(typeof(RequiresApiKey))]
        public async Task<ActionResult> Update(
            string username,
            [FromBody, BindRequired] AccountUpdateRequest request)
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

            try
            {
                await _accountService.UpdateAsync(account, request);
                return Success();
            }
            catch (AccountUpdateException e)
            {
                return HandleAccountUpdateException(e);
            }
        }

        [HttpDelete]
        [ValidateModel]
        public async Task<ActionResult> Delete(
            [BindRequired, ModelBinder(
                 BinderType = typeof(AuthorizedAccountBinder)
             )] Account account)
        {
            // Authentication succeeded, delete their account
            await _accountService.DeleteAsync(account);
            return Success();
        }

        [HttpDelete("{id}")]
        [ServiceFilter(typeof(RequiresApiKey))]
        public async Task<ActionResult> Delete(Guid id)
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
            await _accountService.DeleteAsync(account);
            return Success();
        }

        [HttpDelete("username/{username}")]
        [ServiceFilter(typeof(RequiresApiKey))]
        public async Task<ActionResult> Delete(string username)
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
            await _accountService.DeleteAsync(account);
            return Success();
        }
    }
}
