using EverlandApi.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EverlandApi.Core.Results
{
    public class ValidationFailedApiResult : ApiResult
    {
        public ValidationFailedApiResult(ModelStateDictionary modelState)
            : base(
                StatusCodes.Status400BadRequest,
                new ApiResponse(new ValidationApiError(modelState))
            )
        {}
    }
}
