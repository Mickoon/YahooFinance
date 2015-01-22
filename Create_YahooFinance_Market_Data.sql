USE [FinanceCrawler]
GO

/****** Object:  Table [dbo].[YahooFinance_Market_Data]    Script Date: 15/12/2014 3:45:57 p.m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[YahooFinance_Market_Data](
	[Identity] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Recorded_Date] [datetime] NOT NULL,
	[Price] [numeric](18, 2) NULL,
	[Low_Price] [numeric](18, 2) NULL,
	[High_Price] [numeric](18, 2) NULL,
	[52wk_Low] [numeric](18, 2) NULL,
	[52wk_High] [numeric](18, 2) NULL,
	[52wk_Avg] [numeric](18, 2) NULL,
	[Volume] [bigint] NULL,
	[Avg_Vol(3m)] [bigint] NULL,
	[Beta] [nvarchar](50) NULL,
	[Market_Cap(b)] [numeric](18, 2) NULL,
	[P/E(ttm)] [numeric](18, 2) NULL,
	[EPS(ttm)] [numeric](18, 2) NULL,
	[Group] [nvarchar](max) NOT NULL
PRIMARY KEY CLUSTERED 
(
	[Identity] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


