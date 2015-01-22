using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace Yahoo_Finance_Crawler
{
    class WorkerClasses
    {
        public static string getSourceCode(string url)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                StreamReader sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);
                string sourceCode = sr.ReadToEnd();
                sr.Close();
                resp.Close();
                return sourceCode;
            }
            catch
            {
                return "invalid";
            }
        }

        public static string getGroupWord(string url)
        {
            int startIndex = url.IndexOf("s=") + 2;
            url = url.Substring(startIndex, url.Length - startIndex);
            int endIndex = url.IndexOf("&");
            string groupWord = url.Substring(0, endIndex);
            groupWord = groupWord.Replace("&nbsp;", " ");
            groupWord = groupWord.Replace("&#39;", "'");
            groupWord = groupWord.Replace("&quot;", "\"");
            groupWord = groupWord.Replace("&amp;", "&");
            groupWord = groupWord.Replace("%3A", ":");
            return groupWord;
        }
    }
}
