using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace My.AzureServiceBusEstimator
{
    public class startStressTests
    {
        private readonly HttpClient _client;
        private readonly Random random;

        public startStressTests(IHttpClientFactory httpClientFactory)
        {
            this._client = httpClientFactory.CreateClient();
            this.random = new Random();
        }

        [FunctionName("startStressTests")]
        public async Task<long> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var stressTestParameters = context.GetInput<StressTestParameters>();

            int[] payloads = await context.CallActivityAsync<int[]>(
                "PreparePayloads",
                stressTestParameters);

            var tasks = new Task<int>[payloads.Length];
            for (int i = 0; i < payloads.Length; i++)
            {
                tasks[i] = context.CallActivityAsync<int>(
                    "ExecuteTest",
                    payloads[i]);
            }

            await Task.WhenAll(tasks);

            long totalMessageSizes = tasks.Sum(t => t.Result);
            return totalMessageSizes;
        }

        [FunctionName("PreparePayloads")]
        public int[] PreparePayloads([ActivityTrigger] StressTestParameters stressTestParameters, ILogger log)
        {
            List<int> payloads = new List<int>();

            log.LogInformation($"Preparing {stressTestParameters.NumberOfTests} payloads!");

            for (int i = 0; i < stressTestParameters.NumberOfTests; i++)
            {
                payloads.Add(random.Next(stressTestParameters.MinMessageSize, stressTestParameters.MaxMessageSize));
            }

            return payloads.ToArray();
        }

        [FunctionName("ExecuteTest")]
        public async Task<int> ExecuteTest([ActivityTrigger] int payload, ILogger log)
        {
            var baseUrl = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");
            var scheme = baseUrl.StartsWith("localhost") ? "http" : "https";
            var url = $"{scheme}://{baseUrl}/api/post/{payload}";
            var sleep = random.Next(1, payload) * 1000;

            log.LogInformation($"Executing payload {payload} after {sleep}ms, with {url}");

            // Adding a random sleep time to avoid flooding the Service Bus
            System.Threading.Thread.Sleep(sleep);

            var response = await this._client.GetAsync(url);

            return payload;
        }
    }
}