using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using SiteMonitoring.Contract.V1.Data;

namespace SiteMonitoringApi.Controllers
{
    public class TimingController : ApiController
    {
        private HttpMessageFactory _messageFactory;
        private PhantomJsModules _phantomJsModules;

        public TimingController()
        {
            _messageFactory = new HttpMessageFactory();
            _phantomJsModules = new PhantomJsModules();
        }

        [HttpGet()]
        public HttpResponseMessage Index(string url, int timeout)
        {
            try
            {
                var timingResults = _phantomJsModules.NetSniff(url, timeout);
                return _messageFactory.GenerateMessageForJson(timingResults, 200);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent(ex.Message + ex.StackTrace + (ex.InnerException ?? new Exception())),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
        
        [HttpGet()]
        public HttpResponseMessage Waterfall(string url, int timeout)
        {
            try
            {
                var timingResults = _phantomJsModules.NetSniff(url, timeout);
                var filteredResults = new TimingWaterfall(timingResults);
                return _messageFactory.GenerateMessageForJson(filteredResults, 200);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage
                {
                    Content = new StringContent(ex.Message + ex.StackTrace + (ex.InnerException ?? new Exception())),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
    }

    public class HttpMessageFactory
    {
        public HttpResponseMessage GenerateMessageForJson(string json, int statusCode)
        {
            return new HttpResponseMessage
            {
                Content = new StringContent(json, new UTF8Encoding(), "application/json"),
                StatusCode = (HttpStatusCode)statusCode
            };
        }

        public HttpResponseMessage GenerateMessageForJson<TObject>(TObject instance, int statusCode)
        {
            return new HttpResponseMessage
            {
                Content = new ObjectContent(instance.GetType(), instance, new JsonMediaTypeFormatter()),
                StatusCode = (HttpStatusCode)statusCode
            };
        }
    }
}