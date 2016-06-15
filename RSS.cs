using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace GetOtherNewsRss
{
    public class RSS
    {

        private HttpWebRequest m_HttpWebRequest;
        private string m_URL;
        private byte[] m_byte;
        private string m_RSSXML;
        public string m_EncodingName;
        public int m_CodePage;

        public string readNews(string Link)
        {
            HttpWebRequest pHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(Link);
            Stream pStream = pHttpWebRequest.GetResponse().GetResponseStream();

            byte[] pBytes = ReadFully(pStream);
            pStream.Close();


            Encoding enc = Encoding.Default;

            m_RSSXML = enc.GetString(pBytes);
            m_RSSXML.ToLower();

            if (m_RSSXML.IndexOf("charset=utf-8") > 0 || m_RSSXML.IndexOf("charset=UTF-8") > 0)
            {
                m_CodePage = 65001;
            }
            else
            {
                m_CodePage = 950;
            }


            switch (m_CodePage)
            {
                case 950:
                    enc = Encoding.Default;
                    break;
                case 65001:
                    enc = Encoding.UTF8;
                    break;
                default:
                    enc = Encoding.Default;
                    break;
            }


            string HTML = enc.GetString(pBytes);


            return HTML;
        }

        public RSS()
        {
        }

        public RSS(string URL)
        {

            /*
            XmlTextReader xmlTextReader = new XmlTextReader(URL);
            xmlTextReader.Read();      
            m_EncodingName = xmlTextReader.Encoding.EncodingName;
            m_CodePage = xmlTextReader.Encoding.CodePage;
            xmlTextReader = null;
            */


            Encoding enc = Encoding.Default;

            m_URL = URL;
            m_HttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(URL);
            Stream pStream = m_HttpWebRequest.GetResponse().GetResponseStream();
            m_byte = ReadFully(pStream);
            pStream.Close();

            m_RSSXML = enc.GetString(m_byte);

            if (m_RSSXML.IndexOf("encoding=\"UTF-8\"") > 0 || m_RSSXML.IndexOf("encoding=\"utf-8\"") > 0)
            {
                m_CodePage = 65001;
            }
            else
            {
                m_CodePage = 950;
            }


            switch (m_CodePage)
            {
                case 950:
                    enc = Encoding.Default;
                    break;
                case 65001:
                    enc = Encoding.UTF8;
                    break;
                default:
                    enc = Encoding.Default;
                    break;
            }

            m_RSSXML = enc.GetString(m_byte);
            if (m_CodePage == 65001)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(m_RSSXML);
                if (bytes[0] == 0xef && bytes[1] == 0xbb && bytes[2] == 0xbf)
                {
                    string byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
                    m_RSSXML = m_RSSXML.Remove(0, byteOrderMarkUtf8.Length);
                }
            }
        }

        public string getRSSXML()
        {
            return m_RSSXML;
        }


        /// <summary>
        /// 取得element中的資訊
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="TagName"></param>
        /// <returns></returns>
        //public string readElementContent(XmlDocument xmlDoc, string TagName)
        //{
        //    string Content = "";
        //    try
        //    {
        //        Content = xmlDoc.GetElementsByTagName(TagName)[0].InnerText;
        //    }
        //    catch { }

        //    return Content;
        //}

        //public List<RSSItem> getRssItem()
        //{

        //    XmlDocument xml = new XmlDocument();
        //    xml.LoadXml(m_RSSXML);


        //    XmlNodeList xmlNodeList = xml.GetElementsByTagName("item");

        //    List<RSSItem> pLists = new List<RSSItem>();
        //    foreach (XmlNode aXmlNode in xmlNodeList)
        //    {
        //        XmlDocument xmlItem = new XmlDocument();
        //        xmlItem.LoadXml("<XML>" + aXmlNode.InnerXml + "</XML>");

        //        RSSItem pRss = new RSSItem();

        //        pRss.Title = readElementContent(xmlItem, "title");
        //        pRss.Link = readElementContent(xmlItem, "link");
        //        pRss.PubDate = readElementContent(xmlItem, "pubDate");




        //        if (pRss.PubDate.Length > 0)
        //        {
        //            DateTime MyDateTime;
        //            MyDateTime = new DateTime();
        //            MyDateTime = DateTime.Parse(pRss.PubDate);

        //            pRss.PubDate = MyDateTime.ToString("yyyy/MM/dd hh:mm:ss");
        //        }


        //        pRss.Description = readElementContent(xmlItem, "description");
        //        //將<![CDATA[取代掉
        //        if (pRss.Description.IndexOf("<![CDATA[") > 0)
        //        {
        //            pRss.Description = pRss.Description.Replace("<![CDATA[", "");
        //            pRss.Description = pRss.Description.Replace("]]", "");
        //        }

        //        /*
        //        pRss.Title = xmlItem.GetElementsByTagName("title")[0].InnerText;
        //        pRss.Link = xmlItem.GetElementsByTagName("link")[0].InnerText;
        //        pRss.PubDate = xmlItem.GetElementsByTagName("pubdate")[0].InnerText;
        //        pRss.Description = xmlItem.GetElementsByTagName("description")[0].InnerText;
        //        */
        //        pLists.Add(pRss);
        //    }

        //    return pLists;

        //}

        public static List<RSSItem> getRssItem(string URL)
        {
            List<RSSItem> feeds = new List<RSSItem>();
            try
            {
                XDocument xDoc = new XDocument();
                xDoc = XDocument.Load(URL);
                var items = (from x in xDoc.Descendants("item")
                             select new
                             {
                                 title = x.Element("title").Value,
                                 link = x.Element("link").Value,
                                 pubDate = x.Element("pubDate").Value,
                                 description = x.Element("description").Value
                             });
                if (items != null)
                {
                    foreach (var i in items)
                    {
                        string description = i.description;
                        //將<![CDATA[取代掉
                        if (description.IndexOf("<![CDATA[") > 0)
                        {
                            description = description.Replace("<![CDATA[", "");
                            description = description.Replace("]]", "");
                        }
                        RSSItem rssitem = new RSSItem
                        {
                            Title = i.title,
                            Link = i.link,
                            PubDate = i.pubDate,
                            Description = description
                        };
                        feeds.Add(rssitem);
                    }
                }
                return feeds;
                //gvRss.DataSource = feeds;
                //gvRss.DataBind();
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        /// <summary>
        /// Reads data from a stream until the end is reached. The
        /// data is returned as a byte array. An IOException is
        /// thrown if any of the underlying IO calls fail.
        /// </summary>
        /// <param name="stream">The stream to read data from</param>
        private byte[] ReadFully(Stream stream)
        {
            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }
    }
}