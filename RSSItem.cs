using System;
using System.Collections.Generic;
using System.Web;

namespace GetOtherNewsRss
{
    public class RSSItem
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string PubDate { get; set; }
        public string Description { get; set; }
    }

    //public class NewsFiles 
    //{
    //    public string Title { get; set; }
    //    public string Type { get; set; }
    //    public string URL { get; set; }

    //}
}