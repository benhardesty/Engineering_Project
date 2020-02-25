using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FunctionApp1.Messages;
using System.Collections.Generic;

namespace FunctionApp1
{
    public class ApiFunction
    {
        [FunctionName("ApiFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Queue("messagetomom")] IAsyncCollector<MessageToMom> letterCollector,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //TODO model HttpRequest from fields of MessageToMom
            //Map new model values (from HttpRequest) to MessageToMom below

            var message = new MessageToMom {
                Flattery = new List<string> { "amazing", "fabulous", "profitable" },
                Greeting = "So Good To Hear From You",
                HowMuch = 1222.22M,
                HowSoon = DateTime.UtcNow.AddDays(1),
                From = "yourbelovedson@gmail.com"
            
            };

            await letterCollector.AddAsync(message);

            return (ActionResult)new OkObjectResult($"Hello, Johnny");
        }
    }
}
