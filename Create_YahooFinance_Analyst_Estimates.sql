USE [FinanceCrawler]
GO

/****** Object:  Table [dbo].[YahooFinance_Analyst_Estimates]    Script Date: 15/12/2014 2:36:07 p.m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[YahooFinance_Analyst_Estimates](
	[Identity] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Price] [numeric](18, 2) NOT NULL,
	[Recorded_Date] [datetime] NOT NULL,
	[Current_Qtr] [datetime] NULL,
	[Next_Qtr] [datetime] NULL,
	[Current_Year] [datetime] NULL,
	[Next_Year] [datetime] NULL,
	[Earning_Est_Current_Year_Avg.Estimate] [numeric](18, 2) NULL,
	[Earning_Est_Next_Year_Avg.Estimate] [numeric](18, 2) NULL,
	[Earning_Est_Current_Year_Year_Ago_EPS] [numeric](18, 2) NULL,
	[Earning_Est_Next_Year_Year_Ago_EPS] [numeric](18, 2) NULL,
	[Revenue_Est_Current_Year_Avg.Estimate] [numeric](18, 2) NULL,
	[Revenue_Est_Next_Year_Avg.Estimate] [numeric](18, 2) NULL,
	[Revenue_Est_Current_Year_Year_Ago_Sales] [numeric](18, 2) NULL,
	[Revenue_Est_Next_Year_Year_Ago_Sales] [numeric](18, 2) NULL,
	[Earnings_History_Current_Year_EPS_Est] [numeric](18, 2) NULL,
	[Earnings_History_Next_Year_EPS_Est] [numeric](18, 2) NULL,
	[Earnings_History_Current_Year_EPS_Actual] [numeric](18, 2) NULL,
	[Earnings_History_Next_Year_EPS_Actual] [numeric](18, 2) NULL,
	[Earnings_History_Current_Year_Difference] [numeric](18, 2) NULL,
	[Earnings_History_Next_Year_Difference] [numeric](18, 2) NULL,
	[Earnings_History_Current_Year_Surprise(%)] [numeric](18, 2) NULL,
	[Earnings_History_Next_Year_Surprise(%)] [numeric](18, 2) NULL,
	[Growth_Est_Company_Current_Qtr(%)] [numeric](18, 2) NULL,
	[Growth_Est_Industry_Current_Qtr(%)] [numeric](18, 2) NULL,
	[Growth_Est_Sector_Current_Qtr(%)] [numeric](18, 2) NULL,
	[Growth_Est_S&P500_Current_Qtr(%)] [numeric](18, 2) NULL,
	[Growth_Est_Company_Next_Qtr(%)] [numeric](18, 2) NULL,
	[Growth_Est_Industry_Next_Qtr(%)] [numeric](18, 2) NULL,
	[Growth_Est_Sector_Next_Qtr(%)] [numeric](18, 2) NULL,
	[Growth_Est_S&P500_Next_Qtr(%)] [numeric](18, 2) NULL,
	[Growth_Est_Company_This_Year(%)] [numeric](18, 2) NULL,
	[Growth_Est_Industry_This_Year(%)] [numeric](18, 2) NULL,
	[Growth_Est_Sector_This_Year(%)] [numeric](18, 2) NULL,
	[Growth_Est_S&P500_This_Year(%)] [numeric](18, 2) NULL,
	[Growth_Est_Company_Next_Year(%)] [numeric](18, 2) NULL,
	[Growth_Est_Industry_Next_Year(%)] [numeric](18, 2) NULL,
	[Growth_Est_Sector_Next_Year(%)] [numeric](18, 2) NULL,
	[Growth_Est_S&P500_Next_Year(%)] [numeric](18, 2) NULL,
	[Growth_Est_Company_Past5Years(%)] [numeric](18, 2) NULL,
	[Growth_Est_Industry_Past5Years(%)] [numeric](18, 2) NULL,
	[Growth_Est_Sector_Past5Years(%)] [numeric](18, 2) NULL,
	[Growth_Est_S&P500_Past5Years(%)] [numeric](18, 2) NULL,
	[Growth_Est_Company_Next5Years(%)] [numeric](18, 2) NULL,
	[Growth_Est_Industry_Next5Years(%)] [numeric](18, 2) NULL,
	[Growth_Est_Sector_Next5Years(%)] [numeric](18, 2) NULL,
	[Growth_Est_S&P500_Next5Years(%)] [numeric](18, 2) NULL,
	[Growth_Est_Company_PriceEarnings] [numeric](18, 2) NULL,
	[Growth_Est_Industry_PriceEarnings] [numeric](18, 2) NULL,
	[Growth_Est_Sector_PriceEarnings] [numeric](18, 2) NULL,
	[Growth_Est_S&P500_PriceEarnings] [numeric](18, 2) NULL,
	[Growth_Est_Company_PEGRatio] [numeric](18, 2) NULL,
	[Growth_Est_Industry_PEGRatio] [numeric](18, 2) NULL,
	[Growth_Est_Sector_PEGRatio] [numeric](18, 2) NULL,
	[Growth_Est_S&P500_PEGRatio] [numeric](18, 2) NULL,
	[Group] [nvarchar](max) NOT NULL
PRIMARY KEY CLUSTERED 
(
	[Identity] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


