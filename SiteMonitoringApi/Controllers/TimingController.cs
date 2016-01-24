using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json;
using NReco.PhantomJS;

namespace SiteMonitoringApi.Controllers
{
    public class TimingController : ApiController
    {
        public HttpResponseMessage Get(string url, int timeout)
        {
            var messages = new List<string>();
            var phantomJs = new PhantomJS();
            try
            {
                phantomJs.OutputReceived += (sender, e) => { messages.Add(e.Data); };
                phantomJs.RunScript(JsModules.NetSniff, new[] {url});
                phantomJs.ExecutionTimeout = TimeSpan.FromMilliseconds(timeout);
                var value = messages.Aggregate("", (current, message) => current + message);
                JsonSerializer serializer = new JsonSerializer();
                var output = serializer.Deserialize(new JsonTextReader(new StringReader(value)));

                return new HttpResponseMessage
                {
                    Content = new ObjectContent(output.GetType(), output, new JsonMediaTypeFormatter()),
                    StatusCode = HttpStatusCode.OK
                };

            }
            catch (Exception ex)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent(ex.ToString()),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
            finally
            {
                phantomJs.Abort();
            }
        }
    }
}