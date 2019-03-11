using System.Threading.Tasks;
using EverlandApi.Core.Results;

namespace EverlandApi.Core.Services
{
    public interface IRedisService
    {
        Task CacheResult(string cacheKey, ApiResult result, int expire = 0);
        Task<ApiResult> GetCachedResult(string cacheKey);
    }
}
