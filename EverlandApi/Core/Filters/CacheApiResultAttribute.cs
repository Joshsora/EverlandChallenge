using System.Collections;
using System.IO;
using EverlandApi.Core.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using EverlandApi.Extensions;
using System.Threading.Tasks;
using EverlandApi.Core.Results;
using System;

namespace EverlandApi.Core.Filters
{
    public class CacheApiResultAttribute : ActionFilterAttribute
    {
        public IRedisService RedisService { get; private set; }
        public string CacheKey { get; private set; }
        public int Expire { get; private set; }

        public CacheApiResultAttribute(string cacheKey, int expire = 0)
        {
            CacheKey = cacheKey;
            Expire = expire;
        }

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Do we have a IRedisService implementation?
            try
            {
                RedisService = context.HttpContext
                    .RequestServices.GetService<IRedisService>();
            }
            catch (InvalidOperationException)
            {
                // Caching has not been configured
                await next();
                return;
            }

            // Attempt to get the response data from the Redis cache
            CacheKey = CacheKey.Format((IDictionary)context.ActionArguments);
            var result = await RedisService.GetCachedResult(CacheKey);
            if (result != null)
            {
                context.Result = result;
                return;
            }

            // Execute the action normally, and then cache the result
            ActionExecutedContext executedContext = await next();
            await RedisService.CacheResult(
                CacheKey, executedContext.Result as ApiResult,
                expire: Expire
            );
        }
    }
}
