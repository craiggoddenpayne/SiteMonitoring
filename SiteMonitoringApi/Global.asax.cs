using System;
using System.Web;
using System.Web.Http;

namespace SiteMonitoringApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Response.Clear();
            Response.AddHeader("Exception", exception.ToString());

        }
    }
}
