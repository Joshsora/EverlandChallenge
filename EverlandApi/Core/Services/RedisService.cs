using System;
using System.IO;
using System.Threading.Tasks;
using EverlandApi.Core.Results;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace EverlandApi.Core.Services
{
    public class RedisService : IRedisService
    {
        private ConnectionMultiplexer _connection;
        private JsonOutputFormatter _outputFormatter;

        public RedisService(
            IOptionsMonitor<RedisOptions> redisOptions,
            JsonOutputFormatter outputFormatter)
        {
            _connection = ConnectionMultiplexer.Connect(
                redisOptions.CurrentValue.ConnectionString
            );
            _outputFormatter = outputFormatter;
        }

        public async Task CacheResult(string cacheKey, ApiResult result,
            int expire = 0)
        {
            // Store the IActionResult object as a JSON string
            using (var writer = new StringWriter())
            {
                _outputFormatter.WriteObject(writer, result);
                string storedValue = writer.ToString();
                await _connection.GetDatabase().StringSetAsync(
                    cacheKey, storedValue, expiry: TimeSpan.FromSeconds(expire)
                );
            }
        }

        public async Task<ApiResult> GetCachedResult(string cacheKey)
        {
            RedisValue storedValue = await _connection.GetDatabase().StringGetAsync(cacheKey);
            if (storedValue == RedisValue.Null)
                return null;
            return JsonConvert.DeserializeObject<ApiResult>(storedValue);
        }
    }
}
