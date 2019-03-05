using System;
using System.Linq;
using EverlandApi.Core.Services;
using EverlandApi.Accounts.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using EverlandApi.Core.Models;
using EverlandApi.Core.Results;
using Microsoft.AspNetCore.Mvc.Controllers;
using EverlandApi.Accounts.Attributes;

namespace EverlandApi.Accounts.Filters
{
    public class RequiresAccount : ActionFilterAttribute
    {
        private IAuthenticationService<Account> _authService;

        public RequiresAccount(
            IAuthenticationService<Account> authService)
        {
            _authService = authService;
        }

        private string FindTargetParameterName(ActionExecutingContext filterContext)
        {
            // Find the parameter we intend to set
            if (filterContext.ActionDescriptor is
                ControllerActionDescriptor controllerActionDescriptor)
            {
                // We can use MethodInfo to find the AccountTarget parameter
                var param = controllerActionDescriptor.MethodInfo
                    .GetParameters()
                    .FirstOrDefault(
                        p => p.GetCustomAttributes(
                                 typeof(AccountTargetAttribute),
                                 false
                             ).Length == 1
                    );
                if (param != null)
                    return param.Name;
            }

            // If we couldn't use the AccountTargetAttribute, fallback
            // to iterating over ActionArguments.
            var accountParams = filterContext.ActionArguments
                .Where(kv => kv.Value is Account).ToList();
            if (accountParams.Count == 0)
                throw new ArgumentException("Action is missing required Account parameter.");
            return accountParams.First().Key;
        }

        private Guid? FindExpectedId(ActionExecutingContext filterContext)
        {
            if (filterContext.ActionDescriptor is
                ControllerActionDescriptor controllerActionDescriptor)
            {
                // We can use MethodInfo to find the ExpectedAccountId parameter
                var param = controllerActionDescriptor.MethodInfo
                    .GetParameters()
                    .FirstOrDefault(
                        p => p.GetCustomAttributes(
                                 typeof(ExpectedAccountIdAttribute),
                                 false
                             ).Length == 1
                    );
                if (param != null)
                    return filterContext.ActionArguments[param.Name] as Guid?;
            }
            return null;
        }

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext filterContext, ActionExecutionDelegate next)
        {
            // Find the parameter we intend to set, and the expected account id (if present)
            string targetParameterName = FindTargetParameterName(filterContext);
            Guid? expectedAccountId = FindExpectedId(filterContext);

            // Clear the value at the target parameter
            // This is so that URI parameters do not influence the Account object
            filterContext.ActionArguments[targetParameterName] = null;

            // Attempt to authenticate
            try
            {
                var account = await _authService.AuthenticateAsync(
                    filterContext.HttpContext
                );
                if (account == null)
                {
                    filterContext.Result = new UnauthorizedObjectResult(new ApiResult(
                        new AuthenticationError(
                            "Invalid username and/or password.",
                            AuthenticationErrorCode.InvalidCredentials
                        )
                    ));
                    return;
                }

                // Is this the expected account?
                if (expectedAccountId.HasValue && account.Id != expectedAccountId.Value)
                {
                    filterContext.Result = new ForbiddenObjectResult(new ApiResult(
                        new AuthenticationError(
                            "You are not authorized to access the requested resource.",
                            AuthenticationErrorCode.Unauthorized
                        )
                    ));
                    return;
                }

                // Provide the action method with the authentication account and
                // execute the action.
                filterContext.ActionArguments[targetParameterName] = account;
                await base.OnActionExecutionAsync(filterContext, next);
            }
            catch (AuthenticationException e)
            {
                filterContext.Result = new UnauthorizedObjectResult(new ApiResult(
                    new AuthenticationError(e.Message, e.ErrorCode)
                ));
            }
        }
    }
}

