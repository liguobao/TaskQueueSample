using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MTQueue.Model;
using MTQueue.Service;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using static StackExchange.Redis.RedisChannel;

namespace MTQueue.Listener
{
    public class RedisMQListener : IHostedService
    {
        private readonly ConnectionMultiplexer _redisMultiplexer;

        private readonly IServiceProvider _services;

        private readonly ILogger<RedisMQListener> _logger;

        public RedisMQListener(IServiceProvider services, ConnectionMultiplexer redisMultiplexer,
        ILogger<RedisMQListener> logger)
        {
            _services = services;
            _redisMultiplexer = redisMultiplexer;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Register();
            return Task.CompletedTask;
        }


        public virtual bool Process(RedisChannel ch, RedisValue message)
        {
            _logger.LogInformation("Process start,message: " + message);
            var redisDB = _services.GetRequiredService<ConnectionMultiplexer>()
            .GetDatabase(CommonConst.DEFAULT_DB);
            var messageJson = JToken.Parse(message);
            var requestId = messageJson["requestId"]?.ToString();
            if (string.IsNullOrEmpty(requestId))
            {
                _logger.LogWarning("requestId not in message.");
                return false;
            }
            var mtAgent = _services.GetRequiredService<ZhihuClient>();
            var text = mtAgent.GetZhuanlan(messageJson);
            redisDB.StringSet(requestId, text.ToString(), CommonConst.RESPONSE_TS);
            _logger.LogInformation("Process finish,requestId:" + requestId);
            redisDB.SortedSetRemove(CommonConst.REQUESTS_SORT_SETKEY, requestId);
            return true;
        }


        public void Register()
        {
            var sub = _redisMultiplexer.GetSubscriber();
            var channel = CommonConst.REQUEST_CHANNEL;
            sub.SubscribeAsync(channel, (ch, value) =>
            {
                Process(ch, value);
            });
        }

        public void DeRegister()
        {
            // this.connection.Close();
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            // this.connection.Close();
            return Task.CompletedTask;
        }
    }

}
