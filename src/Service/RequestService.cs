using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorrelationId;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using static StackExchange.Redis.RedisChannel;

namespace MTQueue.Service
{
    public class RequestService
    {


        private readonly ICorrelationContextAccessor _correlationContext;

        private readonly ConnectionMultiplexer _redisMultiplexer;

        private readonly IServiceProvider _services;

        private readonly ILogger<RequestService> _logger;

        public RequestService(ICorrelationContextAccessor correlationContext,
        ConnectionMultiplexer redisMultiplexer, IServiceProvider services,
        ILogger<RequestService> logger)
        {
            _correlationContext = correlationContext;
            _redisMultiplexer = redisMultiplexer;
            _services = services;
            _logger = logger;
        }

        public long? AddRequest(JToken data)
        {
            var requestId = _correlationContext.CorrelationContext.CorrelationId;
            var redisDB = _redisMultiplexer.GetDatabase(CommonConst.DEFAULT_DB);
            var index = redisDB.SortedSetRank(CommonConst.REQUESTS_SORT_SETKEY, requestId);
            if (index == null)
            {
                data["requestId"] = requestId;
                redisDB.SortedSetAdd(CommonConst.REQUESTS_SORT_SETKEY, requestId, GetTotalSeconds());
                PushRedisMessage(data.ToString());
            }
            return redisDB.SortedSetRank(CommonConst.REQUESTS_SORT_SETKEY, requestId);
        }

        public static long GetTotalSeconds()
        {
            return (long)(DateTime.Now.ToLocalTime() - new DateTime(1970, 1, 1).ToLocalTime()).TotalSeconds;
        }

        private void PushRedisMessage(string message)
        {
            Task.Run(() =>
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var multiplexer = scope.ServiceProvider.GetRequiredService<ConnectionMultiplexer>();
                        multiplexer.GetSubscriber().PublishAsync(CommonConst.REQUEST_CHANNEL, message);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(-1, ex, message);
                }
            });
        }

        public Tuple<JToken, long?> GetRequest(string requestId)
        {
            var redisDB = _redisMultiplexer.GetDatabase(CommonConst.DEFAULT_DB);
            var keyIndex = redisDB.SortedSetRank(CommonConst.REQUESTS_SORT_SETKEY, requestId);
            var response = redisDB.StringGet(requestId);
            if (response.IsNull)
            {
                return Tuple.Create<JToken, long?>(default(JToken), keyIndex);
            }
            return Tuple.Create<JToken, long?>(JToken.Parse(response), keyIndex);
        }

    }
}