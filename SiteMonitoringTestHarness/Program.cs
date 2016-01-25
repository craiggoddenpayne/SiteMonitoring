using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SiteMonitoring.Contract.V1.Adapters;
using SiteMonitoring.Contract.V1.Data;

namespace SiteMonitoringTestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            //var api = new ApiAdapter("http://server1.goddenpayne.co.uk/SiteMonitoringApi");
            var api = new ApiAdapter("http://localhost:62323");

            var result = api.Timing(new TimingRequest
            {
                Timeout = TimeSpan.FromSeconds(10),
                Url = "http://craig.goddenpayne.co.uk"
            }).Result;

            Console.WriteLine("Checking: " + result.log.pages[0].id + " which in total took " + result.log.pages[0].pageTimings.onLoad + "ms");
            foreach (var entry in result.log.entries)
                Console.WriteLine("Dependency:" + (entry.request.url.Length > 60 ? entry.request.url.Substring(0, 50) + "..." : entry.request.url) + " took :" + entry.time + "ms");
            Console.ReadKey();
        }
    }
}
