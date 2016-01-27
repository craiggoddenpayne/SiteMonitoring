using System;

namespace SiteMonitoring.Contract.V1.Data
{
    public class SiteMonitoringRequest
    {
        public TimeSpan Timeout { get; set; }
        public string Url { get; set; }
    }
}