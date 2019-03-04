using System;
using EverlandApi.Accounts.Models;
using EverlandApi.Core.Filters;
using EverlandApi.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EverlandApi.Accounts
{
    [Route("api/v1/account")]
    public class AccountController : ControllerBase
    {
        private AccountContext _accountContext;

        public AccountController(AccountContext accountContext)
        {
            _accountContext = accountContext;
        }

        [ValidateModel]
        [HttpPost("create")]
        public ActionResult Create(
            [FromBody, BindRequired] AccountCreationRequest request)
        {
            return Ok(new ApiResult<AccountCreationResponse>(
                new AccountCreationResponse
                {
                    Id = Guid.NewGuid(),
                    EmailValidationRequired = false,
                    ValidationEmailSent = false
                }
            ));
        }
    }
}
