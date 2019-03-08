using System;

namespace EverlandApi.Accounts.Services
{
    [Flags]
    public enum AccountCreationErrorFlags
    {
        None = 0,
        InvalidAccount = 1,
        UsernameTaken = 2,
        EmailInUse = 4
    }
}
