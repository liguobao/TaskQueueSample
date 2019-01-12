using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorrelationId;
using Microsoft.AspNetCore.Mvc;
using MTQueue.Service;
using Newtonsoft.Json.Linq;

namespace MTQueue.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {

        private readonly ICorrelationContextAccessor _correlationContext;

        private readonly RequestService _requestService;

        private readonly ZhihuClient _mtAgentClient;

        public RequestsController(ICorrelationContextAccessor correlationContext,
         RequestService requestService, ZhihuClient mtAgentClient)
        {
            _correlationContext = correlationContext;
            _requestService = requestService;
            _mtAgentClient = mtAgentClient;
        }



        [HttpGet("{requestId}")]
        public IActionResult Get(string requestId)
        {
            var result = _requestService.GetRequest(requestId);
            var resource = $"/v1/requests/{requestId}";
            if (result.Item1 == default(JToken))
            {
                return NotFound(new { rel = "self", href = resource, method = "GET", index = result.Item2 });
            }
            return Ok(result.Item1);
        }

        [HttpPost]
        public IActionResult Post([FromBody] JToken data, [FromHeader(Name = "Prefer")]string prefer)
        {
            if (!string.IsNullOrEmpty(prefer) && prefer == "respond-async")
            {
                var index = _requestService.AddRequest(data);
                var requestId = _correlationContext.CorrelationContext.CorrelationId;
                var resource = $"/v1/requests/{requestId}";
                return Accepted(resource, new { rel = "self", href = resource, method = "GET", index = index });
            }
            return Ok(_mtAgentClient.GetZhuanlan(data));
        }
    }
}
