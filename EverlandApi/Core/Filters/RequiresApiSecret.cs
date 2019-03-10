using System.Linq;
using EverlandApi.Core.Models;
using EverlandApi.Core.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EverlandApi.Core.Filters
{
    public class RequiresApiKey : ActionFilterAttribute
    {
        private SecurityOptions _securityOptions;

        public RequiresApiKey(IOptionsMonitor<SecurityOptions> securityOptions)
        {
            _securityOptions = securityOptions.CurrentValue;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var secrets = context.HttpContext.Request.Headers["X-Api-Key"];
            if (secrets.Count == 0)
            {
                context.Result = new ApiResult(
                    StatusCodes.Status401Unauthorized,
                    new ApiResponse(new AuthenticationApiError(
                        "This action requires the X-Api-Key header to be set.",
                        AuthenticationErrorCode.MissingHeader
                    ))
                );
                return;
            }

            if (secrets.First() != _securityOptions.SecretKey)
                context.Result = new ApiResult(
                    StatusCodes.Status401Unauthorized,
                    new ApiResponse(new AuthenticationApiError(
                        "Incorrect X-Api-Key value.",
                        AuthenticationErrorCode.InvalidCredentials
                    ))
                );
        }
    }
}
