using System;

namespace SiteMonitoringApi
{
    public static class AppData 
    {
        public static string GetPathTo(string filename)
        {
            return AppDomain.CurrentDomain.GetData("DataDirectory") + "\\" + filename;
        }
    }
}