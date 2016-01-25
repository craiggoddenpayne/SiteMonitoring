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

            var result = api.TimingAsWaterfall(new TimingRequest
            {
                Timeout = TimeSpan.FromSeconds(10),
                Url = "http://craig.goddenpayne.co.uk"
            }).Result;

            foreach (var entry in result.Results)
                Console.WriteLine(entry.Start.ToLongTimeString() + ","+ entry.TimeTaken.TotalMilliseconds + "," + entry.Url);
            Console.ReadKey();
        }
    }
}
