using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SiteMonitoringApi.Controllers
{
    public class TestController : ApiController
    {
        public HttpResponseMessage Get()
        {
            return new HttpResponseMessage
            {
                Content = new StringContent("Online!"),
                StatusCode = HttpStatusCode.OK 
            };
        } 

    }
}