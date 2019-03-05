using System;
using EverlandApi.Core.Models;

namespace EverlandApi.Core.Services
{
    public class AuthenticationException : Exception
    {
        public AuthenticationErrorCode ErrorCode { get; private set; }

        public AuthenticationException(string message, AuthenticationErrorCode errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
