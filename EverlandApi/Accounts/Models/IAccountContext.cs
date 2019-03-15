using EverlandApi.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace EverlandApi.Accounts.Models
{
    public interface IAccountContext : IDbContext
    {
        DbSet<Account> Accounts { get; }
    }
}
