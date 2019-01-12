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

namespace MTQueue.Service
{
    public class ZhihuClient
    {

        private readonly ILogger<ZhihuClient> _logger;

        public ZhihuClient(ILogger<ZhihuClient> logger, IOptions<AppSettings> options)
        {
            _logger = logger;


        }

        public JToken GetZhuanlan(JToken data)
        {

            var client = new RestClient("https://zhuanlan.zhihu.com/");
            var request = new RestRequest(Method.GET);
            request.AddHeader("accept-language", "zh-CN,zh;q=0.9,en;q=0.8,da;q=0.7,zh-TW;q=0.6");
            request.AddHeader("accept-encoding", "gzip, deflate, br");
            request.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            request.AddHeader("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36");
            request.AddHeader("upgrade-insecure-requests", "1");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("pragma", "no-cache");
            request.AddHeader("connection", "keep-alive");
            IRestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                return JToken.Parse("{'code':0,'success':'true'");
            }
            return default(JToken);
        }

    }

}