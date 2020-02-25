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
using System.ComponentModel.DataAnnotations;

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

            // Validate data from HttpRequest.
            var valid = validateRequest(req);
            if (!valid.valid)
            {
                return (ActionResult)new BadRequestObjectResult(valid.errorMessage);
            }

            // Handle multiple flattery values from req.
            string flattery = req.Query["flattery"];
            string[] flatteyArray = flattery.Split(",");
            List<string> flatteryList = new List<string>(flatteyArray);

            // Handle a request that doesn't include an explicit HowSoon date.
            DateTime? howsoon = null;
            if ((string)req.Query["howsoon"] != null && (string)req.Query["howsoon"] != "")
            {
                howsoon = DateTime.Parse(req.Query["howsoon"]);
            }

            var message = new MessageToMom
            {
                Flattery = flatteryList,
                Greeting = req.Query["greeting"],
                HowMuch = decimal.Parse(req.Query["howmuch"]),
                HowSoon = howsoon,
                From = req.Query["from"]
            };

            await letterCollector.AddAsync(message);

            return (ActionResult)new OkObjectResult($"Success");
        }

        // Validate fields from HttpRequest.
        // Required fields: string flattery (can have multiple), string greeting, decimal howmuch, string from.
        // Optional fields: decimal howmuch.
        private (bool valid, string errorMessage) validateRequest(HttpRequest req)
        {
            bool valid = true;
            string errorMessage = "";

            string flattery = req.Query["flattery"];
            if (flattery == null)
            {
                valid = false;
                errorMessage += "Value 'flattery' is required. Multiple flattery values are permitted but at least one is required.\n";
            }
            string greeting = req.Query["greeting"];
            if (greeting == null)
            {
                valid = false;
                errorMessage += "Value 'greeting' is required.\n";
            }
            string howmuch = req.Query["howmuch"];
            try
            {
                decimal.Parse(howmuch);
            }
            catch
            {
                valid = false;
                errorMessage += "Value 'howmuch' is required and must be a decimal value.\n";
            }
            string howsoon = req.Query["howsoon"];
            if (howsoon != null && howsoon != "")
            {
                try
                {
                    DateTime.Parse(howsoon);
                }
                catch
                {
                    valid = false;
                    errorMessage += "Value 'howsoon' is optional but if included it must be a date in the format MM/DD/YYYY.\n";
                }
            }
            string from = req.Query["from"];
            if (!validateEmailFormat(from))
            {
                valid = false;
                errorMessage += "Value 'from' is required and must be a properly formatted email address (e.g. email@email.com).\n";
            }

            return (valid, errorMessage);
        }

        // Validate the format of an email.
        public bool validateEmailFormat(string email)
        {
            if (email == null)
            {
                return false;
            }
            var emailValidator = new EmailAddressAttribute();
            return emailValidator.IsValid(email);
        }
    }


}
