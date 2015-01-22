using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;

namespace Yahoo_Finance_Crawler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void exit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        /* Yahoo! Finance */
        private void button1_Click(object sender, EventArgs e)
        {
            var db = new FinanceCrawlerEntities();
            int duplicates = 0;
            int newData = 0;

            // Retreive source code from a webpage
            string url = textBox1.Text;         // e.g. https://nz.finance.yahoo.com/q?p=finance.yahoo.com&s=ANZ.AX&ql=0
            if (url != null && url.Trim() != "")
            {
                try
                {
                    string sourceCode = WorkerClasses.getSourceCode(url);
                    if (sourceCode == "invalid") throw new UriFormatException();
                    listbox.Items.Add("Process Starts. Please wait for a few minutes.");

                    /* Group */
                    string groupWord = textBox2.Text;
                    if (groupWord == "")
                        groupWord = WorkerClasses.getGroupWord(url);

                    #region YAHOO! FINANCE RESULT ONLY ALLOWED --> Market Data, Analyst Opinion, Estimates
                    try
                    {
                        // Title
                        int startIndex = sourceCode.IndexOf("class=\"title\"");
                        sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                        startIndex = sourceCode.IndexOf("<h");
                        sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                        startIndex = sourceCode.IndexOf(">") + 1;
                        sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                        int endIndex = sourceCode.IndexOf("</");
                        string pageTitle = sourceCode.Substring(0, endIndex);
                        pageTitle = pageTitle.Replace("&amp;", "&");

                        // Price
                        startIndex = sourceCode.IndexOf("class=\"time_rtq_ticker\"");
                        sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                        startIndex = sourceCode.IndexOf("</span") - 15;
                        sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                        startIndex = sourceCode.IndexOf(">") + 1;
                        sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                        endIndex = sourceCode.IndexOf("</");
                        Decimal price = Convert.ToDecimal(sourceCode.Substring(0, endIndex));

                        /*************************************** MARKET DATA ****************************************/
                        #region MARKET DATA
                        /* Recorded DateTime */
                        startIndex = sourceCode.IndexOf("class=\"time_rtq\"");
                        sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                        startIndex = sourceCode.IndexOf("</") - 15;
                        sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                        startIndex = sourceCode.IndexOf(">") + 1;
                        sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                        endIndex = sourceCode.IndexOf("</");
                        string marketDateStr = sourceCode.Substring(0, endIndex);
                        if (marketDateStr == "" || marketDateStr == null) marketDateStr = Convert.ToString(DateTime.Now);
                        DateTime marketDate = Convert.ToDateTime(marketDateStr);

                        if (!db.YahooFinance_Market_Data.Any(f => f.Name == pageTitle && f.Recorded_Date == marketDate))
                        {
                            // Beta
                            startIndex = sourceCode.IndexOf("Beta");
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf("</") - 10;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf(">") + 1;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            endIndex = sourceCode.IndexOf("</");
                            string beta = sourceCode.Substring(0, endIndex);

                            // Price Range
                            startIndex = sourceCode.IndexOf("Day's Range");
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf("</") - 10;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf(">") + 1;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            endIndex = sourceCode.IndexOf("</");
                            string lowRangeStr = sourceCode.Substring(0, endIndex);
                            if (lowRangeStr == "NaN" || lowRangeStr == "N/A" || lowRangeStr == "") lowRangeStr = "0";
                            Decimal lowRange = Convert.ToDecimal(lowRangeStr);
                            startIndex = sourceCode.IndexOf("-");
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf("</") - 6;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf(">") + 1;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            endIndex = sourceCode.IndexOf("</");
                            string highRangeStr = sourceCode.Substring(0, endIndex);
                            if (highRangeStr == "NaN" || highRangeStr == "N/A" || highRangeStr == "") highRangeStr = "0";
                            Decimal highRange = Convert.ToDecimal(highRangeStr);

                            // 52wk Range
                            startIndex = sourceCode.IndexOf("52wk Range");
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf("</") - 10;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf(">") + 1;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            endIndex = sourceCode.IndexOf("</");
                            string low52wkRangeStr = sourceCode.Substring(0, endIndex);
                            if (low52wkRangeStr == "NaN" || low52wkRangeStr == "N/A" || low52wkRangeStr == "") low52wkRangeStr = "0";
                            Decimal low52wkRange = Convert.ToDecimal(low52wkRangeStr);
                            startIndex = sourceCode.IndexOf("-");
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf("</") - 6;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf(">") + 1;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            endIndex = sourceCode.IndexOf("</");
                            string high52wkRangeStr = sourceCode.Substring(0, endIndex);
                            if (high52wkRangeStr == "NaN" || high52wkRangeStr == "N/A" || high52wkRangeStr == "") high52wkRangeStr = "0";
                            Decimal high52wkRange = Convert.ToDecimal(high52wkRangeStr);
                            Decimal Avg52wkRange = 0;
                            if (low52wkRange != 0 && high52wkRange != 0) Avg52wkRange = (low52wkRange + high52wkRange) / 2;

                            // Volume 
                            startIndex = sourceCode.IndexOf("Volume");
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf("</") - 15;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf(">") + 1;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            endIndex = sourceCode.IndexOf("</");
                            string volumeStr = sourceCode.Substring(0, endIndex);
                            if (volumeStr == "NaN" || volumeStr == "N/A" || volumeStr == "") volumeStr = "0";
                            volumeStr = volumeStr.Replace(",", "");
                            int volume = Convert.ToInt32(volumeStr);

                            // Avg Vol 
                            startIndex = sourceCode.IndexOf("Avg Vol");
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf("</") - 15;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf(">") + 1;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            endIndex = sourceCode.IndexOf("</");
                            string avgVolStr = sourceCode.Substring(0, endIndex);
                            if (avgVolStr == "NaN" || avgVolStr == "N/A" || avgVolStr == "") avgVolStr = "0";
                            avgVolStr = avgVolStr.Replace(",", "");
                            int avgVol = Convert.ToInt32(avgVolStr);

                            // Market Cap (Billions)
                            startIndex = sourceCode.IndexOf("Market Cap");
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf("</") - 15;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf(">") + 1;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            endIndex = sourceCode.IndexOf("</");
                            string marketCapStr = sourceCode.Substring(0, endIndex);
                            if (marketCapStr == "NaN" || marketCapStr == "N/A" || marketCapStr == "") marketCapStr = "0";
                            else if (marketCapStr.Contains("b") || marketCapStr.Contains("B"))
                            {
                                marketCapStr = marketCapStr.Replace("b", "");
                                marketCapStr = marketCapStr.Replace("B", "");
                            }
                            else if (marketCapStr.Contains("m") || marketCapStr.Contains("M"))
                            {
                                marketCapStr = marketCapStr.Replace("m", "");
                                marketCapStr = marketCapStr.Replace("M", "");
                                Decimal temp = Convert.ToDecimal(marketCapStr);
                                temp /= 1000;
                                marketCapStr = Convert.ToString(temp);
                            }
                            else
                            {
                                Decimal temp = Convert.ToDecimal(marketCapStr);
                                temp /= 1000000;
                                marketCapStr = Convert.ToString(temp);
                            }
                            marketCapStr = marketCapStr.Replace(",", "");
                            Decimal marketCap = Convert.ToDecimal(marketCapStr);

                            // P/E 
                            startIndex = sourceCode.IndexOf("P/E");
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf("</") - 15;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf(">") + 1;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            endIndex = sourceCode.IndexOf("</");
                            string peStr = sourceCode.Substring(0, endIndex);
                            if (peStr == "NaN" || peStr == "N/A" || peStr == "") peStr = "0";
                            peStr = peStr.Replace(",", "");
                            Decimal pe = Convert.ToDecimal(peStr);

                            // EPS
                            startIndex = sourceCode.IndexOf("EPS");
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf("</") - 15;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf(">") + 1;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            endIndex = sourceCode.IndexOf("</");
                            string epsStr = sourceCode.Substring(0, endIndex);
                            if (epsStr == "NaN" || epsStr == "N/A" || epsStr == "") epsStr = "0";
                            epsStr = epsStr.Replace(",", "");
                            Decimal eps = Convert.ToDecimal(epsStr);

                            db.YahooFinance_Market_Data.Add(new YahooFinance_Market_Data
                            {
                                Name = pageTitle,
                                Price = price,
                                Group = groupWord,
                                Recorded_Date = marketDate,
                                Beta = beta,
                                Avg_Vol_3m_ = avgVol,
                                C52wk_Avg = Avg52wkRange,
                                C52wk_High = high52wkRange,
                                C52wk_Low = low52wkRange,
                                EPS_ttm_ = eps,
                                High_Price = highRange,
                                Low_Price = lowRange,
                                Volume = volume,
                                Market_Cap_b_ = marketCap,
                                P_E_ttm_ = pe
                            });
                            db.SaveChanges();
                            newData++;
                        }
                        else
                        {
                            duplicates++;
                        }
                        #endregion

                        // Find links (Analyst Opinion and Estimates)
                        startIndex = sourceCode.IndexOf("Analysts");  
                        sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                        startIndex = sourceCode.IndexOf("/q/ao?s=") - 20;
                        sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                        startIndex = sourceCode.IndexOf("href") + 4;
                        sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                        startIndex = sourceCode.IndexOf("\"") + 1;
                        sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                        endIndex = sourceCode.IndexOf("\"");
                        string opinionsLink = "https://nz.finance.yahoo.com" + sourceCode.Substring(0, endIndex);
                        startIndex = sourceCode.IndexOf("href") + 4;
                        sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                        startIndex = sourceCode.IndexOf("\"") + 1;
                        sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                        endIndex = sourceCode.IndexOf("\"");
                        string estimatesLink = "https://nz.finance.yahoo.com" + sourceCode.Substring(0, endIndex);

                /************************************* ANALYST OPINION *********************************************/
                        #region ANALYST OPINION
                        sourceCode = WorkerClasses.getSourceCode(opinionsLink);

                        if (sourceCode.Contains("There is no Analyst Opinion data available"))
                            listbox.Items.Add(groupWord + " has no Analyst Opinion data available.");
                        else
                        {
                            /* Recorded DateTime */
                            startIndex = sourceCode.IndexOf("class=\"time_rtq\"");
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf("</") - 15;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf(">") + 1;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            endIndex = sourceCode.IndexOf("</");
                            string recordedDateStr = sourceCode.Substring(0, endIndex);
                            if (recordedDateStr == "" || recordedDateStr == null) recordedDateStr = Convert.ToString(DateTime.Now);
                            DateTime recordedDate = Convert.ToDateTime(recordedDateStr);

                            if (!db.YahooFinance_Analyst_Opinion.Any(f => f.Name == pageTitle && f.Date == recordedDate))
                            {

                                /* Recommendation Summary */
                                startIndex = sourceCode.IndexOf("Recommendation Summary");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("Mean Recommendation");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal recommThisWeek = Convert.ToDecimal(sourceCode.Substring(0, endIndex));
                                startIndex = sourceCode.IndexOf("Mean Recommendation");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal recommLastWeek = Convert.ToDecimal(sourceCode.Substring(0, endIndex));

                                /* Price Target Summary */
                                startIndex = sourceCode.IndexOf("Price Target Summary");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("Mean Target");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal meanTarget = Convert.ToDecimal(sourceCode.Substring(0, endIndex));
                                startIndex = sourceCode.IndexOf("Median Target");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal medianTarget = Convert.ToDecimal(sourceCode.Substring(0, endIndex));
                                startIndex = sourceCode.IndexOf("High Target");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal highTarget = Convert.ToDecimal(sourceCode.Substring(0, endIndex));
                                startIndex = sourceCode.IndexOf("Low Target");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal lowTarget = Convert.ToDecimal(sourceCode.Substring(0, endIndex));
                                startIndex = sourceCode.IndexOf("No. of Brokers");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                int noOfBrokers = Convert.ToInt32(sourceCode.Substring(0, endIndex));

                                /* Recommendation Trends */
                                startIndex = sourceCode.IndexOf("Recommendation Trends");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);

                                // Strong Buy
                                startIndex = sourceCode.IndexOf("Strong Buy");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal strongbuy_Current = Convert.ToDecimal(sourceCode.Substring(0, endIndex));
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal strongbuy_Last = Convert.ToDecimal(sourceCode.Substring(0, endIndex));
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal strongbuy_TwoAgo = Convert.ToDecimal(sourceCode.Substring(0, endIndex));
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal strongbuy_ThreeAgo = Convert.ToDecimal(sourceCode.Substring(0, endIndex));

                                // Buy
                                startIndex = sourceCode.IndexOf("Buy");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal buy_Current = Convert.ToDecimal(sourceCode.Substring(0, endIndex));
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal buy_Last = Convert.ToDecimal(sourceCode.Substring(0, endIndex));
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal buy_TwoAgo = Convert.ToDecimal(sourceCode.Substring(0, endIndex));
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal buy_ThreeAgo = Convert.ToDecimal(sourceCode.Substring(0, endIndex));

                                // Hold
                                startIndex = sourceCode.IndexOf("Hold");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal hold_Current = Convert.ToDecimal(sourceCode.Substring(0, endIndex));
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal hold_Last = Convert.ToDecimal(sourceCode.Substring(0, endIndex));
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal hold_TwoAgo = Convert.ToDecimal(sourceCode.Substring(0, endIndex));
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal hold_ThreeAgo = Convert.ToDecimal(sourceCode.Substring(0, endIndex));

                                // Underperform
                                startIndex = sourceCode.IndexOf("Underperform");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal underperform_Current = Convert.ToDecimal(sourceCode.Substring(0, endIndex));
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal underperform_Last = Convert.ToDecimal(sourceCode.Substring(0, endIndex));
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal underperform_TwoAgo = Convert.ToDecimal(sourceCode.Substring(0, endIndex));
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal underperform_ThreeAgo = Convert.ToDecimal(sourceCode.Substring(0, endIndex));

                                // Sell
                                startIndex = sourceCode.IndexOf("Sell");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal sell_Current = Convert.ToDecimal(sourceCode.Substring(0, endIndex));
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal sell_Last = Convert.ToDecimal(sourceCode.Substring(0, endIndex));
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal sell_TwoAgo = Convert.ToDecimal(sourceCode.Substring(0, endIndex));
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                Decimal sell_ThreeAgo = Convert.ToDecimal(sourceCode.Substring(0, endIndex));

                                db.YahooFinance_Analyst_Opinion.Add(new YahooFinance_Analyst_Opinion
                                {
                                    Name = pageTitle,
                                    Price = price,
                                    Group = groupWord,
                                    Date = recordedDate,
                                    Recomm_ThisWeek = recommThisWeek,
                                    Recomm_LastWeek = recommLastWeek,
                                    Mean_Target = meanTarget,
                                    Median_Target = medianTarget,
                                    High_Target = highTarget,
                                    Low_Target = lowTarget,
                                    No_of_Brokers = noOfBrokers,
                                    CurrentMonth_Strong_Buy = strongbuy_Current,
                                    CurrentMonth_Buy = buy_Current,
                                    CurrentMonth_Hold = hold_Current,
                                    CurrentMonth_Underperform = underperform_Current,
                                    CurrentMonth_Sell = sell_Current,
                                    LastMonth_Strong_Buy = strongbuy_Last,
                                    LastMonth_Buy = buy_Last,
                                    LastMonth_Hold = hold_Last,
                                    LastMonth_Underperform = underperform_Last,
                                    LastMonth_Sell = sell_Last,
                                    TwoMonthsAgo_Strong_Buy = strongbuy_TwoAgo,
                                    TwoMonthsAgo_Buy = buy_TwoAgo,
                                    TwoMonthsAgo_Hold = hold_TwoAgo,
                                    TwoMonthsAgo_Underperform = underperform_TwoAgo,
                                    TwoMonthsAgo_Sell = sell_TwoAgo,
                                    ThreeMonthsAgo_Strong_Buy = strongbuy_ThreeAgo,
                                    ThreeMonthsAgo_Buy = buy_ThreeAgo,
                                    ThreeMonthsAgo_Hold = hold_ThreeAgo,
                                    ThreeMonthsAgo_Underperform = underperform_ThreeAgo,
                                    ThreeMonthsAgo_Sell = sell_ThreeAgo
                                });
                                db.SaveChanges();
                                newData++;
                            }
                            else
                            {
                                duplicates++;
                            }
                        }
                        #endregion

                /************************************* ANALYST ESTIMATES *********************************************/
                        #region ANALYST ESIMATES
                        sourceCode = WorkerClasses.getSourceCode(estimatesLink);

                        if (sourceCode.Contains("There is no Analyst Estimates data available"))
                            listbox.Items.Add(groupWord + " has no Analyst Estimates data available.");
                        else
                        {
                            /* Recorded DateTime */
                            startIndex = sourceCode.IndexOf("class=\"time_rtq\"");
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf("</") - 15;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            startIndex = sourceCode.IndexOf(">") + 1;
                            sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                            endIndex = sourceCode.IndexOf("</");
                            string recordedDateStr = sourceCode.Substring(0, endIndex);
                            if (recordedDateStr == "" || recordedDateStr == null) recordedDateStr = Convert.ToString(DateTime.Now);
                            DateTime recordedDate = Convert.ToDateTime(recordedDateStr);


                            if (!db.YahooFinance_Analyst_Estimates.Any(f => f.Name == pageTitle && f.Recorded_Date == recordedDate))
                            {
                                /* Earning Ests */
                                startIndex = sourceCode.IndexOf("Earnings Est");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("Current Qtr");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                string tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("0-0")) tempStr = "Jan 1";
                                DateTime currentQtrDate = Convert.ToDateTime(tempStr);
                                startIndex = sourceCode.IndexOf("Next Qtr");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("0-0")) tempStr = "Jan 1";
                                DateTime nextQtrDate = Convert.ToDateTime(tempStr);
                                startIndex = sourceCode.IndexOf("Current Year");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("0-0")) tempStr = "Jan 1";
                                DateTime currentYearDate = Convert.ToDateTime(tempStr);
                                startIndex = sourceCode.IndexOf("Next Year");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("0-0")) tempStr = "Jan 1";
                                DateTime nextYearDate = Convert.ToDateTime(tempStr);

                                // Avg. Estimate
                                startIndex = sourceCode.IndexOf("Avg. Estimate");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                Decimal earnCurrentAvgEst = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                Decimal earnNextAvgEst = Convert.ToDecimal(tempStr);

                                // Year Ago EPS
                                startIndex = sourceCode.IndexOf("Year Ago EPS");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                Decimal currentYearAgoEps = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                Decimal nextYearAgoEps = Convert.ToDecimal(tempStr);

                                /* Revenue Est (Billions) */
                                startIndex = sourceCode.IndexOf("Revenue Est");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("Avg. Estimate");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                string revStr = sourceCode.Substring(0, endIndex);
                                if (revStr == "NaN" || revStr == "N/A") revStr = "0";
                                else if (revStr.Contains("b") || revStr.Contains("B"))
                                {
                                    revStr = revStr.Replace("b", "");
                                    revStr = revStr.Replace("B", "");
                                }
                                else if (revStr.Contains("m") || revStr.Contains("M"))
                                {
                                    revStr = revStr.Replace("m", "");
                                    revStr = revStr.Replace("M", "");
                                    Decimal tempVal = Convert.ToDecimal(revStr);
                                    tempVal /= 1000;
                                    revStr = Convert.ToString(tempVal);
                                }
                                else
                                {
                                    Decimal tempVal = Convert.ToDecimal(revStr);
                                    tempVal /= 1000000;
                                    revStr = Convert.ToString(tempVal);
                                }
                                Decimal revCurrentAvgEst = Convert.ToDecimal(revStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                revStr = sourceCode.Substring(0, endIndex);
                                if (revStr == "NaN" || revStr == "N/A") revStr = "0";
                                else if (revStr.Contains("b") || revStr.Contains("B"))
                                {
                                    revStr = revStr.Replace("b", "");
                                    revStr = revStr.Replace("B", "");
                                }
                                else if (revStr.Contains("m") || revStr.Contains("M"))
                                {
                                    revStr = revStr.Replace("m", "");
                                    revStr = revStr.Replace("M", "");
                                    Decimal tempVal = Convert.ToDecimal(revStr);
                                    tempVal /= 1000;
                                    revStr = Convert.ToString(tempVal);
                                }
                                else
                                {
                                    Decimal tempVal = Convert.ToDecimal(revStr);
                                    tempVal /= 1000000;
                                    revStr = Convert.ToString(tempVal);
                                }
                                Decimal revNextYearAgoEst = Convert.ToDecimal(revStr);

                                // Year Ago Sales
                                startIndex = sourceCode.IndexOf("Year Ago Sales");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                revStr = sourceCode.Substring(0, endIndex);
                                if (revStr == "NaN" || revStr == "N/A") revStr = "0";
                                else if (revStr.Contains("b") || revStr.Contains("B"))
                                {
                                    revStr = revStr.Replace("b", "");
                                    revStr = revStr.Replace("B", "");
                                }
                                else if (revStr.Contains("m") || revStr.Contains("M"))
                                {
                                    revStr = revStr.Replace("m", "");
                                    revStr = revStr.Replace("M", "");
                                    Decimal tempVal = Convert.ToDecimal(revStr);
                                    tempVal /= 1000;
                                    revStr = Convert.ToString(tempVal);
                                }
                                else
                                {
                                    Decimal tempVal = Convert.ToDecimal(revStr);
                                    tempVal /= 1000000;
                                    revStr = Convert.ToString(tempVal);
                                }
                                Decimal revCurrentYearAgoSale = Convert.ToDecimal(revStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                revStr = sourceCode.Substring(0, endIndex);
                                if (revStr == "NaN" || revStr == "N/A") revStr = "0";
                                else if (revStr.Contains("b") || revStr.Contains("B"))
                                {
                                    revStr = revStr.Replace("b", "");
                                    revStr = revStr.Replace("B", "");
                                }
                                else if (revStr.Contains("m") || revStr.Contains("M"))
                                {
                                    revStr = revStr.Replace("m", "");
                                    revStr = revStr.Replace("M", "");
                                    Decimal tempVal = Convert.ToDecimal(revStr);
                                    tempVal /= 1000;
                                    revStr = Convert.ToString(tempVal);
                                }
                                else
                                {
                                    Decimal tempVal = Convert.ToDecimal(revStr);
                                    tempVal /= 1000000;
                                    revStr = Convert.ToString(tempVal);
                                }
                                Decimal revNextYearAgoSale = Convert.ToDecimal(revStr);

                                /* Earnings History */
                                startIndex = sourceCode.IndexOf("Earnings History");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("EPS Est");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal earnCurrentEpsEst = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal earnNextEpsEst = Convert.ToDecimal(tempStr);

                                // EPS Actual
                                startIndex = sourceCode.IndexOf("EPS Actual");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal earnCurrentEpsActual = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal earnNextEpsActual = Convert.ToDecimal(tempStr);

                                // Difference
                                startIndex = sourceCode.IndexOf("Difference");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal earnCurrentDifference = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal earnNextDifference = Convert.ToDecimal(tempStr);

                                // Surprise (in %)
                                startIndex = sourceCode.IndexOf("Surprise");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal earnCurrentSurprise = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal earnNextSurprise = Convert.ToDecimal(tempStr);

                                /* Growth Estimate */
                                startIndex = sourceCode.IndexOf("Growth Est");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                // 1. Current Qtr.
                                startIndex = sourceCode.IndexOf("Current Qtr");
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal companyCurrentQtr = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal industryCurrentQtr = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal sectorCurrentQtr = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal sp500CurrentQtr = Convert.ToDecimal(tempStr);

                                // 2. Next Qtr.
                                startIndex = sourceCode.IndexOf("Next Qtr");
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                Decimal companyNextQtr = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal industryNextQtr = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal sectorNextQtr = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal sp500NextQtr = Convert.ToDecimal(tempStr);

                                // 3. This Year
                                startIndex = sourceCode.IndexOf("This Year");
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal companyThisYear = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal industryThisYear = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal sectorThisYear = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal sp500ThisYear = Convert.ToDecimal(tempStr);

                                // 4. Next Year
                                startIndex = sourceCode.IndexOf("Next Year");
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal companyNextYear = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal industryNextYear = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal sectorNextYear = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal sp500NextYear = Convert.ToDecimal(tempStr);

                                // 5. Past 5 Years (per annum)
                                startIndex = sourceCode.IndexOf("Past 5 Years");
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal companyPast5Years = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal industryPast5Years = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal sectorPast5Years = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal sp500Past5Years = Convert.ToDecimal(tempStr);

                                // 6. Next 5 Years (per annum)
                                startIndex = sourceCode.IndexOf("Next 5 Years");
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal companyNext5Years = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal industryNext5Years = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal sectorNext5Years = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal sp500Next5Years = Convert.ToDecimal(tempStr);

                                // 7. Price/Earnings (avg. for comparison categories)
                                startIndex = sourceCode.IndexOf("Price/Earnings");
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal companyPriceEarnings = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal industryPriceEarnings = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal sectorPriceEarnings = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal sp500PriceEarnings = Convert.ToDecimal(tempStr);

                                // 8. PEG Ratio (avg. for comparison categories)
                                startIndex = sourceCode.IndexOf("PEG Ratio");
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal companyPEGRatio = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal industryPEGRatio = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal sectorPEGRatio = Convert.ToDecimal(tempStr);
                                startIndex = sourceCode.IndexOf("yfnc_tabledata1");
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                startIndex = sourceCode.IndexOf(">") + 1;
                                sourceCode = sourceCode.Substring(startIndex, sourceCode.Length - startIndex);
                                endIndex = sourceCode.IndexOf("</");
                                tempStr = sourceCode.Substring(0, endIndex);
                                if (tempStr == "NaN" || tempStr == "N/A") tempStr = "0";
                                if (tempStr.Contains("%")) tempStr = tempStr.Replace("%", "");
                                startIndex = tempStr.IndexOf(">") + 1;
                                if (startIndex != 0) tempStr = tempStr.Substring(startIndex, tempStr.Length - startIndex);
                                Decimal sp500PEGRatio = Convert.ToDecimal(tempStr);


                                db.YahooFinance_Analyst_Estimates.Add(new YahooFinance_Analyst_Estimates
                                {
                                    Name = pageTitle,
                                    Price = price,
                                    Group = groupWord,
                                    Recorded_Date = recordedDate,
                                    Current_Qtr = currentQtrDate,
                                    Next_Qtr = nextQtrDate,
                                    Current_Year = currentYearDate,
                                    Next_Year = nextYearDate,
                                    Earning_Est_Current_Year_Avg_Estimate = earnCurrentAvgEst,
                                    Earning_Est_Next_Year_Avg_Estimate = earnNextAvgEst,
                                    Earning_Est_Current_Year_Year_Ago_EPS = currentYearAgoEps,
                                    Earning_Est_Next_Year_Year_Ago_EPS = nextYearAgoEps,
                                    Revenue_Est_Current_Year_Avg_Estimate = revCurrentAvgEst,
                                    Revenue_Est_Next_Year_Avg_Estimate = revNextYearAgoEst,
                                    Revenue_Est_Current_Year_Year_Ago_Sales = revCurrentYearAgoSale,
                                    Revenue_Est_Next_Year_Year_Ago_Sales = revNextYearAgoSale,
                                    Earnings_History_Current_Year_EPS_Est = earnCurrentEpsEst,
                                    Earnings_History_Current_Year_EPS_Actual = earnCurrentEpsActual,
                                    Earnings_History_Current_Year_Difference = earnCurrentDifference,
                                    Earnings_History_Current_Year_Surprise___ = earnCurrentSurprise,
                                    Earnings_History_Next_Year_EPS_Est = earnNextEpsEst,
                                    Earnings_History_Next_Year_EPS_Actual = earnNextEpsActual,
                                    Earnings_History_Next_Year_Difference = earnNextDifference,
                                    Earnings_History_Next_Year_Surprise___ = earnNextSurprise,
                                    Growth_Est_Company_Current_Qtr___ = companyCurrentQtr,
                                    Growth_Est_Company_Next_Qtr___ = companyNextQtr,
                                    Growth_Est_Company_This_Year___ = companyThisYear,
                                    Growth_Est_Company_Next_Year___ = companyNextYear,
                                    Growth_Est_Company_Past5Years___ = companyPast5Years,
                                    Growth_Est_Company_Next5Years___ = companyNext5Years,
                                    Growth_Est_Company_PriceEarnings = companyPriceEarnings,
                                    Growth_Est_Company_PEGRatio = companyPEGRatio,
                                    Growth_Est_Industry_Current_Qtr___ = industryCurrentQtr,
                                    Growth_Est_Industry_Next_Qtr___ = industryNextQtr,
                                    Growth_Est_Industry_This_Year___ = industryThisYear,
                                    Growth_Est_Industry_Next_Year___ = industryNextYear,
                                    Growth_Est_Industry_Past5Years___ = industryPast5Years,
                                    Growth_Est_Industry_Next5Years___ = industryNext5Years,
                                    Growth_Est_Industry_PriceEarnings = industryPriceEarnings,
                                    Growth_Est_Industry_PEGRatio = industryPEGRatio,
                                    Growth_Est_Sector_Current_Qtr___ = sectorCurrentQtr,
                                    Growth_Est_Sector_Next_Qtr___ = sectorNextQtr,
                                    Growth_Est_Sector_This_Year___ = sectorThisYear,
                                    Growth_Est_Sector_Next_Year___ = sectorNextYear,
                                    Growth_Est_Sector_Past5Years___ = sectorPast5Years,
                                    Growth_Est_Sector_Next5Years___ = sectorNext5Years,
                                    Growth_Est_Sector_PriceEarnings = sectorPriceEarnings,
                                    Growth_Est_Sector_PEGRatio = sectorPEGRatio,
                                    Growth_Est_S_P500_Current_Qtr___ = sp500CurrentQtr,
                                    Growth_Est_S_P500_Next_Qtr___ = sp500NextQtr,
                                    Growth_Est_S_P500_This_Year___ = sp500ThisYear,
                                    Growth_Est_S_P500_Next_Year___ = sp500NextYear,
                                    Growth_Est_S_P500_Past5Years___ = sp500Past5Years,
                                    Growth_Est_S_P500_Next5Years___ = sp500Next5Years,
                                    Growth_Est_S_P500_PriceEarnings = sp500PriceEarnings,
                                    Growth_Est_S_P500_PEGRatio = sp500PEGRatio
                                });
                                db.SaveChanges();
                                newData++;
                            }
                            else
                            {
                                duplicates++;
                            }
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        listbox.Items.Add("Failed because of an error: " + ex);
                    }
                    

                    listbox.Items.Add("\n[" + DateTime.Now + "] Task Ended.\n" + newData + " saved and " + duplicates + " duplicates Found.");
                    #endregion
                }
                catch (Exception)
                {
                    listbox.Items.Add("Invalid URL!");
                    MessageBox.Show("Invalid URL!");
                    textBox1.Text = "";
                }
            }
            else
            {
                listbox.Items.Add("Please enter URL.");
                MessageBox.Show("Please enter URL.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listbox.Items.Add("[" + DateTime.Now + "] YAHOO FINANCE by text file begins! Please wait for a few seconds.");
            DialogResult result = openFileDialog1.ShowDialog();
            string links = "";
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                try
                {
                    links = File.ReadAllText(file);
                }
                catch (IOException)
                {
                    listbox.Items.Add("ERROR: Not appropriate file selected!");
                }
            }

            // Retreive source code from a webpage
            StringReader strReader = new StringReader(links);
            while (true)
            {
                string url = strReader.ReadLine();
                if (url != null && url.Trim() != "")
                {
                    textBox1.Text = url;
                    button1_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("\n[" + DateTime.Now + "] Task Ended.");
                    break;
                }
            }
        }
    }
}
