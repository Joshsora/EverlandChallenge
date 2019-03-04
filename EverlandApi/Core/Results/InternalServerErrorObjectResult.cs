using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EverlandApi.Core.Results
{
    public class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object value)
            : base(value)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }

        public InternalServerErrorObjectResult()
            : this(null)
        {}
    }
}
