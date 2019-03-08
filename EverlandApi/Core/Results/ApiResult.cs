using EverlandApi.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace EverlandApi.Core.Results
{
    public class ApiResult : ObjectResult
    {
        public ApiResult(int statusCode, ApiResponse response)
            : base(response)
        {
            StatusCode = statusCode;
        }
    }
}
