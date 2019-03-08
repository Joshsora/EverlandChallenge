using System.Collections.Generic;
using EverlandApi.Core.Models;
using EverlandApi.Core.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EverlandApi.Core
{
    public class ApiController : ControllerBase
    {
        /// <summary>
        /// Produces an error ApiResult with the specified HTTP status code.
        /// </summary>
        public ApiResult Error(int statusCode, IEnumerable<ApiError> errors)
            =>  new ApiResult(statusCode, new ApiResponse(errors));

        /// <summary>
        /// Produces an error ApiResult with the specified HTTP status code.
        /// </summary>
        public ApiResult Error(int statusCode, params ApiError[] errors)
            => Error(statusCode, errors);

        /// <summary>
        /// Produces an empty ApiResult with HTTP status 200OK.
        /// </summary>
        public ApiResult Success()
            => new ApiResult(
                StatusCodes.Status200OK,
                ApiResponse.Empty
            );

        /// <summary>
        /// Produces an ApiResult with HTTP status 200OK containing the
        /// specified response data.
        /// </summary>
        public ApiResult Success<TData>(TData data)
            => new ApiResult(
                StatusCodes.Status200OK,
                new ApiResponse<TData>(data)
            );
    }
}
