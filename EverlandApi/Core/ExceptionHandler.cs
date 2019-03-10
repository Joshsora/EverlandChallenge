using System;
using System.Threading.Tasks;
using EverlandApi.Core.Models;
using EverlandApi.Core.Results;
using EverlandApi.Core.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace EverlandApi.Core
{
    public class ExceptionHandler
    {
        private RequestDelegate _next;
        private IHostingEnvironment _env;
        private IActionResultExecutor<ObjectResult> _resultExecutor;

        public ExceptionHandler(
            RequestDelegate next,
            IHostingEnvironment env,
            IActionResultExecutor<ObjectResult> resultExecutor)
        {
            _next = next;
            _env = env;
            _resultExecutor = resultExecutor;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
            }
            catch (Exception e) when (
                (e is DbUpdateException) ||
                (e is DbUpdateConcurrencyException) ||
                (e is MySqlException))
            {
                await WriteResult(httpContext, new ApiResult(
                    StatusCodes.Status500InternalServerError,
                    new ApiResponse(new ApiError(
                        "An error occurred while trying to access the database.",
                        ApiErrorCode.DatabaseError
                    ))
                ));
            }
            catch (Exception e) when (e is AuthenticationException ae)
            {
                await WriteResult(httpContext, new ApiResult(
                    StatusCodes.Status401Unauthorized,
                    new ApiResponse(new AuthenticationApiError(
                        ae.Message, ae.ErrorCode
                    ))
                ));
            }
            catch (Exception e)
            {
                // If we're in development, it's useful to see the exception that
                // occurred in the JSON output
                if (_env.IsDevelopment())
                {
                    await WriteResult(httpContext, new ApiResult(
                        StatusCodes.Status500InternalServerError,
                        new ApiResponse(new ExceptionDetailsApiError(e))
                    ));
                }
                else
                {
                    // Preferably, this should never happen
                    await WriteResult(httpContext, new ApiResult(
                        StatusCodes.Status500InternalServerError,
                        new ApiResponse(new ApiError(
                            "An unhandled exception was thrown.",
                            ApiErrorCode.UnhandledException
                        ))
                    ));
                }
            }
        }

        private async Task WriteResult(
            HttpContext context, ApiResult result)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            await _resultExecutor.ExecuteAsync(
                new ActionContext(
                    context,
                    context.GetRouteData() ?? new RouteData(),
                    new ActionDescriptor()
                ),
                result
            );
        }
    }
}
