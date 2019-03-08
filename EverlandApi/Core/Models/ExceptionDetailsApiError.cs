using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EverlandApi.Core.Models
{
    public class ExceptionDetailsApiError : ApiError
    {
        public class ExceptionDetails
        {
            public string Message { get; private set; }
            public IDictionary Data { get; private set; }
            public IEnumerable<string> StackTrace { get; private set; }
            public ExceptionDetails InnerDetails { get; private set; }

            public ExceptionDetails(Exception exception)
            {
                Message = exception.Message;
                Data = exception.Data;
                StackTrace = exception.StackTrace.Split(Environment.NewLine)
                    .Select(s => s.Trim());
                InnerDetails = exception.InnerException != null
                    ? new ExceptionDetails(exception.InnerException)
                    : null;
            }
        }

        public ExceptionDetails Details { get; private set; }

        public ExceptionDetailsApiError(Exception exception)
            : base("An unhandled exception occurred.", ApiErrorCode.UnhandledException)
        {
            Details = new ExceptionDetails(exception);
        }
    }
}
