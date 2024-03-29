USE [OML]


/****** Drop the schema version UDF******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSchemaVersion]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GetSchemaVersion]
GO

/****** Create the new schema version UDF ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSchemaVersion]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GetSchemaVersion]
GO

execute dbo.sp_executesql @statement = N' 
  
CREATE FUNCTION [dbo].[GetSchemaVersion]() 
RETURNS varchar(10) 
AS 
BEGIN 
        RETURN ''1.2'' 
END 
'  
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[ImageMappings]
	ADD OriginalName [nvarchar](80) NULL;
	
	
ALTER TABLE [dbo].[Titles]
	ADD SeasonNumber [smallint] NULL;
	
ALTER TABLE [dbo].[Titles]
	ADD EpisodeNumber [smallint] NULL;
GO
