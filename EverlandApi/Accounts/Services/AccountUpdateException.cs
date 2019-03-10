using System;

namespace EverlandApi.Accounts.Services
{
    public class AccountUpdateException : Exception
    {
        public AccountUpdateErrorFlags ErrorFlags { get; private set; }

        public AccountUpdateException(string message,
            AccountUpdateErrorFlags errorFlags) : base(message)
        {
            ErrorFlags = errorFlags;
        }
    }
}