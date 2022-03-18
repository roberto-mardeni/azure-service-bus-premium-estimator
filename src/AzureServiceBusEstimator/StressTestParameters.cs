using System;

namespace My.AzureServiceBusEstimator
{
    public class StressTestParameters
    {
        public int NumberOfTests { get; set; }
        public int MinMessageSize { get; set; }
        public int MaxMessageSize { get; set; }
    }
}