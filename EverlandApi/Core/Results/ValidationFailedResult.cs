using EverlandApi.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EverlandApi.Core.Results
{
    public class ValidationFailedResult : ObjectResult
    {
        public ValidationFailedResult(ModelStateDictionary modelState)
            : base(new ApiResult(new ApiValidationError(modelState)))
        {
            StatusCode = StatusCodes.Status400BadRequest;
        }
    }
}
