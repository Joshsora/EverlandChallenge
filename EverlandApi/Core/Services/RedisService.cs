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
            IOptions<RedisOptions> redisOptions,
            JsonOutputFormatter outputFormatter)
        {
            try
            {
                _connection = ConnectionMultiplexer.Connect(
                    redisOptions.Value.ConnectionString
                );
            }
            catch (RedisConnectionException)
            {
                _connection = null;
            }
            _outputFormatter = outputFormatter;
        }

        public async Task CacheResult(string cacheKey, ApiResult result,
            int expire = 0)
        {
            if (cacheKey == null)
                throw new ArgumentNullException(nameof(cacheKey));
            if (_connection == null)
                return;

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
            if (cacheKey == null)
                throw new ArgumentNullException(nameof(cacheKey));
            if (_connection == null)
                return null;

            RedisValue storedValue = await _connection.GetDatabase().StringGetAsync(cacheKey);
            if (storedValue == RedisValue.Null)
                return null;
            return JsonConvert.DeserializeObject<ApiResult>(storedValue);
        }
    }
}
