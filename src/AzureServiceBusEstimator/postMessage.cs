using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace My.AzureServiceBusEstimator
{
    public static class postMessage
    {
        [FunctionName("postMessage")]
        [return: ServiceBus("myqueue", Connection = "ServiceBusConnection")]
        public static string Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "post/{sizeInMb:int?}")] HttpRequest req,
            ILogger log, ExecutionContext ctx, int? sizeInMb)
        {
            var filePath = Path.Combine(ctx.FunctionAppDirectory, "payloads", $"{sizeInMb}mb.txt");
            if (System.IO.File.Exists(filePath))
            {
                log.LogInformation($"Posting message of {sizeInMb} MB, using {filePath}");
                return System.IO.File.ReadAllText(filePath);
            }
            else
            {
                throw new ArgumentException("Invalid sizeInMb provided, 1-20 supported!");
            }
        }
    }
}
