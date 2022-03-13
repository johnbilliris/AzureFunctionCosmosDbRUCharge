using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public static class SimpleAPI
    {

        [FunctionName("HelloWorld")]
        public static IActionResult HelloWorld(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string resultMessage = $"Hello World from the HTTP trigger function.";
            return new OkObjectResult(resultMessage);
        }

    }
}
