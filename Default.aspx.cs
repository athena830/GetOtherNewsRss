using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Linq;
using System.Data;
using System.Timers;

namespace GetOtherNewsRss
{
    public partial class _Default : System.Web.UI.Page
    {
        //System.Timers.Timer myTimer;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RssService.onStartRSS();

                //try
                //{
                //    int Interval = 30;
                //    myTimer = new System.Timers.Timer(Interval * 1000);
                //    myTimer.Elapsed += new ElapsedEventHandler(Button1_Click);
                //    myTimer.Enabled = true;
                //    myTimer.Start();


                //    //RunRSS();
                //}
                //catch { }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            lbrss.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            //Button1.Enabled = false;
            updpanel.DataBind();
            updpanel.Update();
            //List<NewsFiles> newsfiles = new List<NewsFiles>();
            //XDocument xDoc = new XDocument();
            //xDoc = XDocument.Load("D:\\workbench\\NLSC\\ConnectToFB\\ConnectToFB\\GetOtherNewsRss\\RssFile.xml");
            //var items = (from x in xDoc.Descendants("item")
            //             select new
            //             {
            //                 title = x.Element("title").Value,
            //                 type = x.Element("type").Value,
            //                 url = x.Element("url").Value
            //             });
            //
            //if (items != null)
            //{
            //    foreach (var i in items)
            //    {
            //        //NewsFiles news = new NewsFiles
            //        //{
            //        //    Title = i.title,
            //        //    Type = i.type,
            //        //    URL = i.url
            //        //};
            //        //newsfiles.Add(news);
            //        feed = PopulateRssFeed(i.url);
            //        feeds.AddRange(feed);
            //        Thread.Sleep(2000); //Delay 1秒=1000
            //    }
            //}
            //gvRss.DataSource = feeds;
            //gvRss.DataBind();
        }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            //List<RSSItem> feeds = showNews();
            //gvRss.DataSource = feeds;
            //gvRss.DataBind();
            //updpanel.Update();
        }

        /// <summary>
        /// 顯示今天的新聞
        /// </summary>
        /// <returns></returns>
        public List<RSSItem> showNews()
        {
            List<RSSItem> feeds = new List<RSSItem>();
            DataTable dt = RssService.getTodayNews();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                RSSItem rssitem = new RSSItem
                {
                    Title = dt.Rows[i]["RM_Title"].ToString(),
                    Link = dt.Rows[i]["RM_Link"].ToString(),
                    PubDate = dt.Rows[i]["RM_PubDate"].ToString(),
                    Description = dt.Rows[i]["RM_Description"].ToString()
                };
                feeds.Add(rssitem);
            }

            return feeds;
        }

        //private List<RSSItem> PopulateRssFeed(string RssFeedUrl)
        //{
        //    //string RssFeedUrl = "https://tw.news.yahoo.com/rss/";
        //    if (!RssFeedUrl.EndsWith("/")) RssFeedUrl += "/";
        //    List<RSSItem> feeds = new List<RSSItem>();
        //    try
        //    {
        //        XDocument xDoc = new XDocument();
        //        xDoc = XDocument.Load(RssFeedUrl);
        //        var items = (from x in xDoc.Descendants("item")
        //                     select new
        //                    {
        //                        title = x.Element("title").Value,
        //                        link = x.Element("link").Value,
        //                        pubDate = x.Element("pubDate").Value,
        //                        description = x.Element("description").Value
        //                    });
        //        if (items != null)
        //        {
        //            foreach (var i in items)
        //            {
        //                RSSItem rssitem = new RSSItem
        //                {
        //                    Title = i.title,
        //                    Link = i.link,
        //                    PubDate = i.pubDate,
        //                    Description = i.description
        //                };
        //                feeds.Add(rssitem);
        //            }
        //        }
        //        return feeds;
        //        //gvRss.DataSource = feeds;
        //        //gvRss.DataBind();
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}
    }
}