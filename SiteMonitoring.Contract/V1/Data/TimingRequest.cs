using System;

namespace SiteMonitoring.Contract.V1.Data
{
    public class TimingRequest
    {
        public TimeSpan Timeout { get; set; }
        public string Url { get; set; }
    }
}