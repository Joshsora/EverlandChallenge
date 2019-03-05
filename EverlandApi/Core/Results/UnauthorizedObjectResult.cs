using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EverlandApi.Core.Results
{
    public class UnauthorizedObjectResult : ObjectResult
    {
        public UnauthorizedObjectResult(object value)
            : base(value)
        {
            StatusCode = StatusCodes.Status401Unauthorized;
        }

        public UnauthorizedObjectResult()
            : this(null)
        { }
    }
}
