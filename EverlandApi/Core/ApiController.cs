using EverlandApi.Core.Models;
using EverlandApi.Core.Results;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace EverlandApi.Core
{
    public class ApiController : ControllerBase
    {
        protected ActionResult HandleMySqlException(MySqlException e)
        {
            // By default, return a generic database error
            return InternalServerError(new ApiResult(
                new ApiError(
                    "A database error occurred.",
                    ApiErrorCode.DatabaseError
                )
            ));
        }

        public ActionResult Unauthorized(object value)
        {
            return new UnauthorizedObjectResult(value);
        }

        public ActionResult Forbidden()
        {
            return new ForbiddenObjectResult();
        }

        public ActionResult Forbidden(object value)
        {
            return new ForbiddenObjectResult(value);
        }

        public ActionResult InternalServerError()
        {
            return new InternalServerErrorObjectResult();
        }

        public ActionResult InternalServerError(object value)
        {
            return new InternalServerErrorObjectResult(value);
        }
    }
}
