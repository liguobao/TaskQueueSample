using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MTQueue.Model;
using Newtonsoft.Json.Linq;
using RestSharp;
using StackExchange.Redis;
using static StackExchange.Redis.RedisChannel;

namespace MTQueue.Service
{
    public class CommonConst
    {

        public static readonly int DEFAULT_DB = 7;

         public static readonly TimeSpan RESPONSE_TS = new TimeSpan(TimeSpan.TicksPerHour * 1);


         public static readonly string REQUESTS_SORT_SETKEY = "request_sort_set";

        public static readonly RedisChannel REQUEST_CHANNEL = new RedisChannel("requests", PatternMode.Auto);

    }
}