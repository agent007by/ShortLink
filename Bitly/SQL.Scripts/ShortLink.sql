USE [Education.CRM]
GO

/****** Object:  Table [dbo].[Banners_ShowsDaily]    Script Date: 10.04.2017 18:58:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ShortLinks](
	[ShortLinkId] [int] IDENTITY(1,1) NOT NULL,
	[NativeUrl] [nvarchar](2048) NOT NULL,
	[ShortUrl] [nvarchar](8) NOT NULL,	
	[IsActive] [bit] NULL,	
	[CreateDate] [smalldatetime] NOT NULL,	
 CONSTRAINT [PK_ShortLinkId] PRIMARY KEY Clustered
(
	[ShortLinkId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE NONCLUSTERED INDEX [IX1_ShortLinks_ShortUrl] ON [dbo].[ShortLinks]
(
	[ShortUrl] ASC
)

GO

CREATE TABLE [dbo].[ShortLinks_RedirectDaily](
	[ShortLinkId] [int] NOT NULL,	
	[CreateDate] [date] NOT NULL,
	[RedirectCount] [int] NULL,
	[IP] [int] NULL,	
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[ShortLinks_RedirectArchive](
	[ShortLinkId] [int] NOT NULL,	
	[CreateDate] [date] NOT NULL,
	[RedirectCount] [int] NULL	
) ON [PRIMARY]

GO

--Утилизация:
--DROP TABLE [dbo].[ShortLinks]
--DROP TABLE [dbo].[ShortLinks_RedirectDaily]
--DROP TABLE [dbo].[ShortLinks_RedirectArchive]
