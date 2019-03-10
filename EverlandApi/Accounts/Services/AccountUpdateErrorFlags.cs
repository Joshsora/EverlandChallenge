using System;

namespace EverlandApi.Accounts.Services
{
    [Flags]
    public enum AccountUpdateErrorFlags
    {
        None = 0,
        EmailInUse = 1
    }
}
