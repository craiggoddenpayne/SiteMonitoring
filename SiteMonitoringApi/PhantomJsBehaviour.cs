using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;

namespace SiteMonitoringApi.Controllers
{
    public class PhantomJsBehaviour
    {
        public string Execute(string phantomPlugin, TimeSpan timeout, params string[] arguments)
        {
            using (var inputStream = new MemoryStream())
            using (var outputStream = new MemoryStream())
            using (var browser = new PhantomJsBrowser(inputStream, outputStream))
            {
                browser.Run(AppData.GetPathTo(phantomPlugin), timeout, arguments);
                outputStream.Position = 0;
                using (StreamReader sr = new StreamReader(outputStream))
                {
                    return sr.ReadToEnd();
                }
            }
        }


    }
    
}