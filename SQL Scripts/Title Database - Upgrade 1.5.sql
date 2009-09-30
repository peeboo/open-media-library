USE [OML]
GO

--enable filestreams
EXEC sp_configure filestream_access_level, 2 -- 0 : Disable , 1 : Transact Sql Access , 2 : Win IO Access   
GO   
RECONFIGURE   
GO

--create the file group
ALTER DATABASE OML
ADD FILEGROUP OMLFILEGROUP
  CONTAINS FILESTREAM
GO

--create the file for the filegroup
DECLARE @TABLENAME VARCHAR(MAX)
--select @TABLENAME= REPLACE([filename], 'OML_Data.mdf','OML_FSData') from oml.dbo.sysfiles where [fileid]=1--this probably needs to change to be more dynamic
SELECT @TABLENAME= SUBSTRING([filename], 1, len([filename])-charindex(reverse('\'),reverse([filename])))+'\OML_FSData' from oml.dbo.sysfiles where [fileid]=1

--ALTER DATABASE OML
--ADD FILE
--  (NAME = 'OMLFS_Group'
--   , FILENAME = 'c:\programdata\openmedialibrary\FSData'
--   )
--TO FILEGROUP OMLFILEGROUP
EXEC('ALTER DATABASE OML ADD FILE (NAME = ''OMLFS_Group'', FILENAME = '''+@TABLENAME+''')TO FILEGROUP OMLFILEGROUP')
GO


--IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DBImagesTemp]') AND type in (N'U'))
--DROP TABLE [dbo].[DBImagesTemp]
--GO

--create a temp table for our filestream images
CREATE TABLE [dbo].[DBImagesTemp](
	[Id] [int] IDENTITY(-2147483647,1) NOT NULL,
	--[Id] [int] NOT NULL,
	--[Image] [image] NOT NULL,
	[ItemGUID] UNIQUEIDENTIFIER ROWGUIDCOL NOT NULL UNIQUE,
	[ItemImage] VARBINARY(MAX) FILESTREAM NULL)

--copy the records to the new table
SET IDENTITY_INSERT [dbo].[DBImagesTemp] ON
INSERT INTO [dbo].[DBImagesTemp]
           ([Id]
           --,[Image]
           ,[ItemGUID]
           ,[ItemImage])
SELECT [Id]
     -- ,[Image]
      ,NEWID()
      ,[Image]
  FROM [OML].[dbo].[DBImages]
GO

SET IDENTITY_INSERT [dbo].[DBImagesTemp] OFF
GO

--drop and add keys
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ImageMappings_DBImages]') AND parent_object_id = OBJECT_ID(N'[dbo].[ImageMappings]'))
ALTER TABLE [dbo].[ImageMappings] DROP CONSTRAINT [FK_ImageMappings_DBImages]
GO

/****** Object:  Index [PK_Images]    Script Date: 09/22/2009 01:24:16 ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DBImages]') AND name = N'PK_Images')
ALTER TABLE [dbo].[DBImages] DROP CONSTRAINT [PK_Images]
GO

/****** Object:  Index [PK_Images]    Script Date: 09/22/2009 01:24:16 ******/
ALTER TABLE [dbo].[DBImagesTemp] ADD  CONSTRAINT [PK_Images] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 70) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ImageMappings]  WITH CHECK ADD  CONSTRAINT [FK_ImageMappings_DBImages] FOREIGN KEY([ImageId])
REFERENCES [dbo].[DBImagesTemp] ([Id])
GO

ALTER TABLE [dbo].[ImageMappings] CHECK CONSTRAINT [FK_ImageMappings_DBImages]
GO

--say goodbye to dbimages
DROP TABLE [dbo].[DBImages]
GO

--change dbimagestemp to dbimages
sp_RENAME 'DBImagesTemp', 'DBImages'
GO

--generate the guid on insert
ALTER TABLE [dbo].[DBImages] ADD  CONSTRAINT [DF_DBImages_ItemGUID]  DEFAULT (newid()) FOR [ItemGUID]
GO

--update our schema
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSchemaVersion]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
execute dbo.sp_executesql @statement = N'

CREATE FUNCTION [dbo].[GetSchemaVersion]()
RETURNS varchar(10)
AS
BEGIN
	RETURN ''1.5''
END
' 
END
GO