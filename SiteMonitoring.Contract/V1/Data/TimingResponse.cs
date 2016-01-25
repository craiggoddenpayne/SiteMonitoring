using System;

namespace SiteMonitoring.Contract.V1.Data
{
    public class TimingResponse
    {
        public Log log { get; set; }

        public class Log
        {
            public string version { get; set; }
            public Creator creator { get; set; }
            public Page[] pages { get; set; }
            public Entry[] entries { get; set; }

            public class Creator
            {
                public string name { get; set; }
                public string version { get; set; }
            }

            public class Page
            {
                public DateTime startedDateTime { get; set; }
                public string id { get; set; }
                public string title { get; set; }
                public PageTimings pageTimings { get; set; }

                public class PageTimings
                {
                    public int onLoad { get; set; }
                }
            }

            public class Entry
            {
                public DateTime startedDateTime { get; set; }
                public int time { get; set; }
                public Request request { get; set; }
                public Response response { get; set; }
                public string redirectURL { get; set; }
                public int headersSize { get; set; }
                public int bodySize { get; set; }
                public Content content { get; set; }
            }

            public class Content
            {
                public int size { get; set; }
                public string mimeType { get; set; }
            }


            public class Response
            {
                public int status { get; set; }
                public string statusText { get; set; }
                public string httpVersion { get; set; }
                public Cookie[] cookies { get; set; }
            }


            public class Cookie
            {

            }

            public class Header
            {
                public string name { get; set; }
                public string value { get; set; }
            }

            public class QueryString
            {

            }


            public class Request
            {
                public string method { get; set; }
                public string url { get; set; }
                public string httpVersion { get; set; }
                public Cookie[] cookies { get; set; }
                public Header[] headers { get; set; }
                public int headersSize { get; set; }
                public int bodySize { get; set; }
            }
        }
    }
}
