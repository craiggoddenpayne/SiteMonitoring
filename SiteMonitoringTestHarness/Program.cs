using System;
using SiteMonitoring.Contract.V1.Adapters;
using SiteMonitoring.Contract.V1.Data;

namespace SiteMonitoringTestHarness
{
    class Program
    {
        private static ApiAdapter _api;

        static void Main(string[] args)
        {
            //var api = new ApiAdapter("http://server1.goddenpayne.co.uk/SiteMonitoringApi");
            _api = new ApiAdapter("http://localhost:62323");

            Timing();
            //SpeedAnalysis();
            Console.ReadKey();
        }

        public static void Timing()
        {
            var result = _api.TimingAsWaterfall(new SiteMonitoringRequest
            {
                Timeout = TimeSpan.FromSeconds(10),
                Url = "http://craig.goddenpayne.co.uk"
            }).Result;

            foreach (var entry in result.Results)
                Console.WriteLine(entry.Start.ToLongTimeString() + "," + entry.TimeTaken.TotalMilliseconds + "," + entry.Url);
            
        }

        public static void SpeedAnalysis()
        {
            var result = _api.SpeedAnalysis(new SiteMonitoringRequest
            {
                Timeout = TimeSpan.FromSeconds(60),
                Url = "http://craig.goddenpayne.co.uk"
            }).Result;

            Console.WriteLine(result);
            
        }

    }
}
