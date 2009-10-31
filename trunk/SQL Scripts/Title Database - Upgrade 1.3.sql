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
        RETURN ''1.3'' 
END 
'  
GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


/****** Object:  Table [dbo].[WatchedFolders]    Script Date: 09/21/2009 14:39:49 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WatchedFolders]') AND type in (N'U'))
DROP TABLE [dbo].[WatchedFolders]
GO



/****** Object:  Table [dbo].[WatchedFolders]    Script Date: 09/21/2009 14:40:12 ******/
CREATE TABLE [dbo].[WatchedFolders](
	[InstanceName] [nvarchar](50) NOT NULL,
	[Folder] [nchar](255) NOT NULL,
	[ParentTitle] [int] NULL,
 CONSTRAINT [PK_WatchedFolders] PRIMARY KEY CLUSTERED 
(
	[InstanceName] ASC,
	[Folder] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Disks]
	ADD MainFeatureVideoBitrate [int] NULL;
GO

ALTER TABLE [dbo].[Disks]
	ADD MainFeatureVideoEndoding [nvarchar](50) NULL;
GO
	
ALTER TABLE [dbo].[Disks]
	ADD MainFeatureAudioBitrate [int] NULL;
GO	
	
ALTER TABLE [dbo].[Disks]
	ADD MainFeatureAudioEncoding [nvarchar](50) NULL;
GO
