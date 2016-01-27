using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace SiteMonitoringApi.Controllers
{
    public class PluginController : ApiController
    {
        public HttpResponseMessage Get(string plugin, int timeout, string arg1 = null, string arg2 = null, string arg3 = null)
        {
            try
            {
                var behaviour = new PhantomJsBehaviour();

                string result;
                if (arg1 == null) result = behaviour.Execute(plugin, TimeSpan.FromSeconds(timeout));
                else if (arg2 == null) result = behaviour.Execute(plugin, TimeSpan.FromSeconds(timeout), arg1);
                else if (arg3 == null) result = behaviour.Execute(plugin, TimeSpan.FromSeconds(timeout), arg1, arg2);
                else result = behaviour.Execute(plugin, TimeSpan.FromSeconds(timeout), arg1, arg2, arg3);

                return new HttpResponseMessage
                {
                    Content = new StringContent(result, new UTF8Encoding(), "application/json"),
                    StatusCode = HttpStatusCode.OK
                };
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
}