using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SiteMonitoringApi.Controllers
{
    public class TestController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Online()
        {
            return new HttpResponseMessage(HttpStatusCode.Continue)
            {
                Content = new StringContent("Service is online")
            };
        }
    }
}