using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Timers;
using System.Web;

namespace GetOtherNewsRss
{
    public class RssService
    {
        public static Dictionary<string, int> FilterKeys;
        static Timer myTimer;

        public static void onStartRSS()
        {
            try
            {
                int Interval = 60;
                myTimer = new System.Timers.Timer(Interval * 1000);
                myTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                myTimer.Enabled = true;
                myTimer.Start();


                //RunRSS();
            }
            catch { }
        }

        /// <summary>
        /// 執行RSS處理程序
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            RunRSS();
        }

        /// <summary>
        /// 讀取過濾關鍵字
        /// </summary>
        private static void getFilterKey()
        {
            var cmd = new SqlCommand();
            cmd.CommandText = @"
                select * from Keyword
            ";
            DataTable rssFilter = Persister.Execute(cmd);

            FilterKeys = new Dictionary<string, int>();

            foreach (DataRow keyRow in rssFilter.Rows)
            {
                FilterKeys.Add(keyRow["KW_Keyword"].ToString(), Int32.Parse(keyRow["KW_ID"].ToString()));
            }
        }

        /// <summary>
        /// 檢查新聞標題是否在過濾清單中
        /// </summary>
        /// <returns></returns>
        private static int matchTitle(string Title)
        {
            foreach (string aStr in FilterKeys.Keys)
            {

                if (Title.IndexOf(aStr) >= 0)
                    return FilterKeys[aStr];
            }
            return -1;
        }

        /// <summary>
        /// 檢查該筆新聞是否已經接收過
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="vdb"></param>
        /// <returns></returns>
        private static bool isExist(string Title)
        {
            var cmd = new SqlCommand();
            cmd.CommandText = @"
                select top 1 RM_ID from RSS_message where RM_Title=@Title and RM_Time>getDate()-350
            ";
            cmd.Parameters.Add("@Title", SqlDbType.NVarChar).Value = Title;
            DataTable theTable = Persister.Execute(cmd);

            if (theTable.Rows.Count > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 取得今天的新聞
        /// </summary>
        /// <returns></returns>
        public static DataTable getTodayNews()
        {
            var cmd = new SqlCommand();
            cmd.CommandText = @"
                select * from RSS_message where RM_Time<=getDate() order by RM_ID desc
            ";
            DataTable theTable = Persister.Execute(cmd);

            if (theTable.Rows.Count > 0)
                return theTable;
            else
                return null;
        }

        /// <summary>
        /// 記錄 RSS 新聞
        /// </summary>
        private static void insertRSS(string SourceID, int KeywordID, RSSItem feeds)
        {
            var cmd = new SqlCommand();
            cmd.CommandText = @"Insert into RSS_message (
                RM_SourceID,RM_Title,RM_Link,RM_Description,
                RM_PubDate,RM_KWID,RM_Time
                ) values (
                @SourceID, @Title, @Link, @Description,
                @PubDate, @KW_ID, getDate())";
            cmd.Parameters.Add("@SourceID", SqlDbType.NVarChar).Value = SourceID;
            cmd.Parameters.Add("@Title", SqlDbType.NVarChar).Value = feeds.Title;
            cmd.Parameters.Add("@Link", SqlDbType.NVarChar).Value = feeds.Link;
            cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = feeds.Description;
            cmd.Parameters.Add("@PubDate", SqlDbType.NVarChar).Value = feeds.PubDate;
            cmd.Parameters.Add("@KW_ID", SqlDbType.Int).Value = KeywordID;
            Persister.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// 讀取rss新聞媒體
        /// </summary>
        private static DataTable getRSSNewsMedia()
        {
            var cmd = new SqlCommand();
            cmd.CommandText = @"
                select * from RSS_Reg
            ";
            DataTable dt = Persister.Execute(cmd);
            return dt;
        }

        /// <summary>
        /// 接收 RSS
        /// </summary>
        private static void RunRSS()
        {
            getFilterKey();

            DataTable rssTable = getRSSNewsMedia();

            string SourceID = ""; //RSS頻道ID
            int KeywordID = -1; //關鍵字ID
            foreach (DataRow rssRow in rssTable.Rows)
            {
                SourceID = rssRow["RS_ID"].ToString();

                string rsurl = rssRow["RS_URL"].ToString();
                if (!rsurl.EndsWith("/")) rsurl += "/";
                //RSS rss = new RSS(rsurl);

                //************************************************

                List<RSSItem> pLists = RSS.getRssItem(rsurl);
                foreach (RSSItem pItem in pLists)
                {
                    KeywordID = matchTitle(pItem.Title);
                    //標題在過濾清單內
                    //if (KeywordID > 0)
                    //{
                    //假如沒接收過
                    if (!isExist(pItem.Title))
                    {
                        DateTime dt = DateTime.Now;
                        DateTime dtnow = DateTime.Now;

                        //2012.09.06
                        //加入時間的處理
                        //只加入二十天前的資料
                        bool isDate = true;
                        try
                        {
                            dt = DateTime.Parse(pItem.PubDate);
                        }
                        catch
                        {
                            isDate = false;
                        }

                        if (isDate)
                        {
                            TimeSpan span = dtnow.Subtract(dt);
                            if (span.Days <= 20)//小於二十天才寫入到資料庫
                            {
                                insertRSS(SourceID, KeywordID, pItem);
                            }

                        }
                    }
                    //}

                }
                //rss = null;
                System.Threading.Thread.Sleep(2000);

            }

        }

    }
}