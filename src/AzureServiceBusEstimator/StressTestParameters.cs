using System;

namespace My.AzureServiceBusEstimator
{
    public class StressTestParameters
    {
        public int NumberOfMessages { get; set; }
        public int MinMessageSize { get; set; }
        public int MaxMessageSize { get; set; }
    }
}