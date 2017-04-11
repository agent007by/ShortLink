USE [Education.CRM]
--USE ShortLinks
GO

/****** Object:  Table [dbo].[Banners_ShowsDaily]    Script Date: 10.04.2017 18:58:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
--- Таблицы и индексы ---
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
	[RedirectCount] [int] NULL	
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[ShortLinks_RedirectArchive](
	[ShortLinkId] [int] NOT NULL,	
	[CreateDate] [date] NOT NULL,
	[RedirectCount] [int] NULL	
) ON [PRIMARY]

GO
--- Хранимые процедуры: ---

CREATE PROCEDURE [dbo].[ShortLinks_AddRedirectDailyToArchive]
AS
BEGIN
	SET NOCOUNT ON
	BEGIN TRANSACTION
	BEGIN TRY 
	
	DECLARE @TS date = CAST(CAST(DATEADD(DAY,1,GETDATE()) as DATE) as smalldatetime)

	INSERT INTO [ShortLinks_RedirectArchive](
											 [ShortLinkId]
											,[CreateDate]											
											,[RedirectCount]
											)
	SELECT 
		[ShortLinkId]			
		,CAST(MAX(CreateDate) as DATE)
		,SUM([RedirectCount]) as RedirectCount	
	FROM 
		[dbo].[ShortLinks_RedirectDaily]
	GROUP BY 
		[ShortLinkId]		

	DELETE FROM
		[dbo].[ShortLinks_RedirectDaily] 
	

	COMMIT
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION
		DECLARE @E_MESSAGE nvarchar(4000), @E_SEVERITY int, @E_STATE int, @E_NUMBER int
		SELECT @E_MESSAGE = ERROR_MESSAGE(), @E_SEVERITY = ERROR_SEVERITY(), @E_STATE = ERROR_STATE(), @E_NUMBER = ERROR_NUMBER()
		IF @E_NUMBER > 50000
			RAISERROR (@E_NUMBER, @E_SEVERITY, @E_STATE)
		ELSE
			RAISERROR (@E_MESSAGE, @E_SEVERITY, @E_STATE)
	END CATCH
END
GO



CREATE PROCEDURE [dbo].[ShortLinks_GetFullStat] (@DateStart date, @DateEnd date)
AS
BEGIN
	CREATE TABLE #tmp  (ShortLinkId int,  RedirectCount int NULL, CreateDate date NULL)
		
	INSERT INTO #tmp
	SELECT shows.ShortLinkId, shows.RedirectCount,shows.TS
	FROM (SELECT 
				ShortLinkId, SUM(RedirectCount) AS RedirectCount, CreateDate as TS
		  FROM
				(SELECT 
					rd.ShortLinkId					
					,SUM(rd.RedirectCount) AS RedirectCount
					,CAST(rd.CreateDate AS DATE) AS CreateDate 		 
				FROM 				
					[dbo].[Banners_ShowsDaily]  AS rd			
				GROUP BY 
					rd.ShortLinkId,CAST(CreateDate AS DATE) 	
				UNION ALL
				SELECT 
					ra.ShortLinkId, 					
					ra.RedirectCount, 
					ra.CreateDate 
				FROM 				
					ShortLinks_RedirectArchive AS ra
				WHERE 
					ra.CreateDate BETWEEN @DateStart	AND @DateEnd
				) n
				GROUP BY 
					ra.ShortLinkId,ra.CreateDate
			) shows 	

	SELECT
		ShortLinkId,CreateDate,ISNULL(RedirectCount,0) as RedirectCount
	FROM 
		#tmp 
	ORDER BY
		CreateDate,ShortLinkId	
	
	drop table #tmp
END
GO



CREATE PROCEDURE [dbo].[ShortLinks_Save] (
	@NativeLink nvarchar(2048),
	@ShortLink nvarchar(8)
)
AS
BEGIN 
SET NOCOUNT ON;
		INSERT INTO [dbo].[ShortLinks] (		
		NativeUrl,
		ShortUrl,
		IsActive,
		CreateDate		
		)
		VALUES (
			@NativeLink,			
			@ShortLink,
			0,		
			GETDATE()
	    )
END
GO



CREATE PROCEDURE [dbo].[ShortLinks_Get](
	@ShortLink nvarchar(8) NULL	
	)
AS
BEGIN
	SELECT  
	    ShortLinkId,
		NativeUrl,
		ShortUrl,
		IsActive,
		CreateDate	
	FROM 
		[dbo].[ShortLinks]
	WHERE  
	(@ShortLink IS NULL OR ShortUrl = @ShortLink)
	 
END
GO



--табличный тип для сохранения переходов по ссылкам
CREATE TYPE [dbo].[ShortLinksRedirectsCountData] AS TABLE(
	[ShortLinkId] int NOT NULL,	
	[RedirectCount] [int] NOT NULL
) 
GO


CREATE PROCEDURE [dbo].[ShortLinks_RedirectsAdd] @Redirects ShortLinksRedirectsCountData readonly 
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE 
		@dateCurrent DATETIME
	
	SELECT	@dateCurrent = GETDATE()	
	      
	INSERT INTO [dbo].[ShortLinks_RedirectDaily]
	SELECT [ShortLinkId]
		,@dateCurrent
		,[RedirectCount]		  
	  FROM 
		@Redirects
END



--- Утилизация: ---
--DROP TABLE [dbo].[ShortLinks]
--DROP TABLE [dbo].[ShortLinks_RedirectDaily]
--DROP TABLE [dbo].[ShortLinks_RedirectArchive]

--DROP PROCEDURE [dbo].[ShortLinks_AddRedirectDailyToArchive]
--DROP PROCEDURE [dbo].[ShortLinks_GetFullStat]
--DROP PROCEDURE [dbo].[ShortLinks_Save]
--DROP PROCEDURE [dbo].[ShortLinks_Get]
--DROP PROCEDURE [dbo].[ShortLinks_RedirectsAdd]
--DROP TYPE [dbo].[ShortLinksRedirectsCountData]

