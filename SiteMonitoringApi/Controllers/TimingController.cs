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
            PhantomJS phantomJs = null;
            try
            {
                var messages = new List<string>();
                phantomJs = new PhantomJS();
                phantomJs.OutputReceived += (sender, e) => { messages.Add(e.Data); };
                phantomJs.ExecutionTimeout = TimeSpan.FromMilliseconds(timeout);
                phantomJs.RunScript(JsModules.NetSniff, new[] { url });
                var value = messages.Aggregate("", (current, message) => current + message);
                try
                {
                    JsonSerializer serializer = new JsonSerializer();
                    var output = serializer.Deserialize(new JsonTextReader(new StringReader(value)));
                    return new HttpResponseMessage
                    {
                        Content = new ObjectContent(output.GetType(), output, new JsonMediaTypeFormatter()),
                        StatusCode = HttpStatusCode.OK
                    };
                }
                catch (Exception)
                {
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(value),
                        StatusCode = HttpStatusCode.OK
                    };
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent(ex.Message + ex.StackTrace + (ex.InnerException ?? new Exception())),
                    StatusCode = HttpStatusCode.OK
                };
            }
            finally
            {
                phantomJs.Abort();
            }
        }
    }
}