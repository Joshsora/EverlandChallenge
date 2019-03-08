using System;

namespace EverlandApi.Accounts.Services
{
    public class AccountCreationException : Exception
    {
        public AccountCreationErrorFlags ErrorFlags { get; private set; }

        public AccountCreationException(string message,
            AccountCreationErrorFlags errorFlags) : base(message)
        {
            ErrorFlags = errorFlags;
        }
    }
}
