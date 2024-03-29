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
        RETURN ''1.1'' 
END 
'  
GO






/****** Add new field to the Bio table ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER TABLE [dbo].[Titles]
	ALTER COLUMN [ParentalRating] [nvarchar](80) NULL;
GO
