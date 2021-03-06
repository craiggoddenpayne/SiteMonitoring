﻿using System.Net.Http;
using System.Threading.Tasks;
using SiteMonitoring.Contract.V1.Data;

namespace SiteMonitoring.Contract.V1.Adapters
{
    public class ApiAdapter
    {
        private readonly string _apiUrl;

        public ApiAdapter(string apiUrl)
        {
            if (!apiUrl.EndsWith("/"))apiUrl = apiUrl + "/";
            _apiUrl = apiUrl;
        }

        public async Task<TimingResponse> Timing(SiteMonitoringRequest request)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(_apiUrl + "Timing?url=" + request.Url + "&timeout=" + request.Timeout.TotalMilliseconds);
                return await response.Content.ReadAsAsync<TimingResponse>();
            }
        }

        public async Task<TimingWaterfall> TimingAsWaterfall(SiteMonitoringRequest request)
        {
            var timingResponse = (await Timing(request));
            return new TimingWaterfall(timingResponse);
        }

        public async Task<string> SpeedAnalysis(SiteMonitoringRequest request)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(_apiUrl + "SpeedAnalysis?url=" + request.Url + "&timeout=" + request.Timeout.TotalMilliseconds);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
