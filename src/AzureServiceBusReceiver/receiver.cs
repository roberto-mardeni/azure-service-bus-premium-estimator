using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace My.AzureServiceBusReceiver
{
    public static class receive
    {
        [FunctionName("receive")]
        public static void Run([ServiceBusTrigger("myqueue", Connection = "ServiceBusConnection")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"ServiceBus message with length: {myQueueItem.Length}");
        }
    }
}
