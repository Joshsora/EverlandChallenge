using Microsoft.EntityFrameworkCore;

namespace EverlandApi.Accounts.Models
{
    public class AccountContext : DbContext, IAccountContext
    {
        public DbSet<Account> Accounts { get; set; }

        public AccountContext(DbContextOptions<AccountContext> options)
                : base(options) {}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var entityBuilder = builder.Entity<Account>();

            // Add a unique constraint on account usernames
            entityBuilder.HasIndex(a => a.Username).IsUnique();

            // Add a unique constraint on account email addresses
            entityBuilder.HasIndex(a => a.Email).IsUnique();
        }
    }
}
