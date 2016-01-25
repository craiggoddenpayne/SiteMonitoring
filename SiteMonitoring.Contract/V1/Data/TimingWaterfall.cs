using System;
using System.Collections.Generic;
using System.Linq;

namespace SiteMonitoring.Contract.V1.Data
{
    public class TimingWaterfall
    {
        private TimingResponse _timingResponse;
        public DateTime Start { get; set; }
        public IEnumerable<TimingResult> Results { get; set; }

        public class TimingResult
        {
            public string Url { get; set; }
            public DateTime Start { get; set; }
            public TimeSpan TimeTaken { get; set; }
        }

        public TimingWaterfall(TimingResponse response)
        {
            _timingResponse = response;
            Start = response.log.pages[0].startedDateTime;
            Results = from entry in response.log.entries
                      orderby entry.startedDateTime
                      select new TimingWaterfall.TimingResult
                      {
                          Url = entry.request.url,
                          Start = entry.startedDateTime,
                          TimeTaken = TimeSpan.FromMilliseconds(entry.time)
                      };
        }
    }
}