using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EverlandApi.Core.Results
{
    public class ForbiddenObjectResult : ObjectResult
    {
        public ForbiddenObjectResult(object value)
            : base(value)
        {
            StatusCode = StatusCodes.Status403Forbidden;
        }

        public ForbiddenObjectResult()
            : this(null)
        { }
    }
}
