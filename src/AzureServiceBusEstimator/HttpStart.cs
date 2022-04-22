using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace My.AzureServiceBusEstimator
{
    public static class HttpStart
    {
        [FunctionName("HttpStart")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, methods: "post", Route = "orchestrators/{functionName}")] HttpRequest req,
            [DurableClient] IDurableClient starter,
            string functionName,
            ILogger log)
        {
            // Function input comes from the request content.
            var eventData = JsonConvert.DeserializeObject<StressTestParameters>(await req.ReadAsStringAsync());

            if (eventData.NumberOfMessages >= 1 && eventData.MinMessageSize >= 1 && eventData.MaxMessageSize <= 20)
            {
                //req.Host
                string instanceId = await starter.StartNewAsync(functionName, eventData);

                log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

                return starter.CreateCheckStatusResponse(req, instanceId);
            }
            else
            {
                return new BadRequestObjectResult("Invalid Stress Test Parameters");
            }
        }
    }
}
