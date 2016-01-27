using System;
using System.IO;
using Newtonsoft.Json;
using SiteMonitoring.Contract.V1.Data;
using SiteMonitoringApi.Controllers;

namespace SiteMonitoringApi
{
    public class PhantomJsModules
    {
        public TimingResponse NetSniff(string url, int timeout)
        {
            var behaviour = new PhantomJsBehaviour();
            var message = behaviour.Execute("netsniff.js", TimeSpan.FromSeconds(timeout), url);

            using (var streamReader = new StringReader(message))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<TimingResponse>(jsonReader);
            }
        }
    }
}