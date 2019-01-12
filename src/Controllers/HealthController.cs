using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace MTQueue.Controllers
{
    [Route("v1/[controller]")]
    public class HealthController : ControllerBase
    {


        public HealthController()
        {

        }

        [HttpGet("")]
        public IActionResult GetHealth()
        {
            return Ok(new { data = new {  }, code = 0 });
        }

        [HttpGet("maxThreads")]
        public IActionResult GetThreads()
        {
            int workerThreads;
            int portThreads;

            ThreadPool.GetMaxThreads(out workerThreads, out portThreads);


            return Ok(new { data = new { workerThreads = workerThreads, portThreads =portThreads }, code = 0 });
        }

        [HttpGet("availableThreads")]
        public IActionResult GetAvailableThreads()
        {
            int workerThreads;
            int portThreads;

            ThreadPool.GetAvailableThreads(out workerThreads, 
            out portThreads);

            return Ok(new { data = new { workerThreads = workerThreads, portThreads =portThreads }, code = 0 });
        }

    
    }
}
