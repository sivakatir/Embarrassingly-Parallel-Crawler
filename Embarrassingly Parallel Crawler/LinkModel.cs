using System;

namespace Embarrassingly_Parallel_Crawler
{
    public class LinkModel
    {
        public Uri SiteUrl { get; set; }
        public int StatusCode { get; set; }
        public string PageTitle { get; set; }
    }
}
