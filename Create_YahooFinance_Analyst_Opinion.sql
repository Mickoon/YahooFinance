USE [FinanceCrawler]
GO

/****** Object:  Table [dbo].[YahooFinance_Analyst_Opinion]    Script Date: 14/12/2014 9:00:37 p.m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[YahooFinance_Analyst_Opinion](
	[Identity] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Price] [numeric](18, 2) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Recomm_ThisWeek] [numeric](18, 1) NULL,
	[Recomm_LastWeek] [numeric](18, 1) NULL,
	[Mean_Target] [numeric](18, 2) NULL,
	[Median_Target] [numeric](18, 2) NULL,
	[High_Target] [numeric](18, 2) NULL,
	[Low_Target] [numeric](18, 2) NULL,
	[No_of_Brokers] [bigint] NULL,
	[CurrentMonth_Strong_Buy] [numeric](18, 1) NULL,
	[LastMonth_Strong_Buy] [numeric](18, 1) NULL,
	[TwoMonthsAgo_Strong_Buy] [numeric](18, 1) NULL,
	[ThreeMonthsAgo_Strong_Buy] [numeric](18, 1) NULL,
	[CurrentMonth_Buy] [numeric](18, 1) NULL,
	[LastMonth_Buy] [numeric](18, 1) NULL,
	[TwoMonthsAgo_Buy] [numeric](18, 1) NULL,
	[ThreeMonthsAgo_Buy] [numeric](18, 1) NULL,
	[CurrentMonth_Hold] [numeric](18, 1) NULL,
	[LastMonth_Hold] [numeric](18, 1) NULL,
	[TwoMonthsAgo_Hold] [numeric](18, 1) NULL,
	[ThreeMonthsAgo_Hold] [numeric](18, 1) NULL,
	[CurrentMonth_Underperform] [numeric](18, 1) NULL,
	[LastMonth_Underperform] [numeric](18, 1) NULL,
	[TwoMonthsAgo_Underperform] [numeric](18, 1) NULL,
	[ThreeMonthsAgo_Underperform] [numeric](18, 1) NULL,
	[CurrentMonth_Sell] [numeric](18, 1) NULL,
	[LastMonth_Sell] [numeric](18, 1) NULL,
	[TwoMonthsAgo_Sell] [numeric](18, 1) NULL,
	[ThreeMonthsAgo_Sell] [numeric](18, 1) NULL,
	[Group] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Identity] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


