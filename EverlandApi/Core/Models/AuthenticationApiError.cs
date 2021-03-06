﻿namespace EverlandApi.Core.Models
{
    public class AuthenticationApiError : ApiError
    {
        public class ErrorDetails
        {
            public string Reason { get; private set; }
            public AuthenticationErrorCode ErrorCode { get; private set; }

            public ErrorDetails(string reason, AuthenticationErrorCode errorCode)
            {
                Reason = reason;
                ErrorCode = errorCode;
            }
        }

        public ErrorDetails Details { get; private set; }

        public AuthenticationApiError(string reason, AuthenticationErrorCode errorCode)
            : base(
                "An error occurred while authenticating the request.",
                ApiErrorCode.AuthenticationFailed
            )
        {
            Details = new ErrorDetails(reason, errorCode);
        }
    }
}
