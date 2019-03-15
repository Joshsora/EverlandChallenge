using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EverlandApi.Core.Models
{
    public interface IDbContext : IDisposable,
        IInfrastructure<IServiceProvider>, IDbContextDependencies,
        IDbSetCache, IDbQueryCache, IDbContextPoolable
    {
        EntityEntry<TEntity> Add<TEntity>(TEntity account) where TEntity : class;
        EntityEntry<TEntity> Remove<TEntity>(TEntity account) where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken token = default(CancellationToken));
    }
}
