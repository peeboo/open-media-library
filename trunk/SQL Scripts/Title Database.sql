USE [OML]
GO
/****** Object:  ForeignKey [FK_Disks_Disks]    Script Date: 04/16/2009 13:20:44 ******/
ALTER TABLE [dbo].[Disks] DROP CONSTRAINT [FK_Disks_Disks]
GO
/****** Object:  ForeignKey [FK_Genres_GenreMetaData]    Script Date: 04/16/2009 13:20:44 ******/
ALTER TABLE [dbo].[Genres] DROP CONSTRAINT [FK_Genres_GenreMetaData]
GO
/****** Object:  ForeignKey [FK_Genres_Titles]    Script Date: 04/16/2009 13:20:44 ******/
ALTER TABLE [dbo].[Genres] DROP CONSTRAINT [FK_Genres_Titles]
GO
/****** Object:  ForeignKey [FK_ImageMappings_DBImages]    Script Date: 04/16/2009 13:20:44 ******/
ALTER TABLE [dbo].[ImageMappings] DROP CONSTRAINT [FK_ImageMappings_DBImages]
GO
/****** Object:  ForeignKey [FK_ImageMappings_Titles]    Script Date: 04/16/2009 13:20:44 ******/
ALTER TABLE [dbo].[ImageMappings] DROP CONSTRAINT [FK_ImageMappings_Titles]
GO
/****** Object:  ForeignKey [FK_People_Bio]    Script Date: 04/16/2009 13:20:44 ******/
ALTER TABLE [dbo].[People] DROP CONSTRAINT [FK_People_Bio]
GO
/****** Object:  ForeignKey [FK_People_Titles]    Script Date: 04/16/2009 13:20:44 ******/
ALTER TABLE [dbo].[People] DROP CONSTRAINT [FK_People_Titles]
GO
/****** Object:  ForeignKey [FK_Tags_Titles]    Script Date: 04/16/2009 13:20:44 ******/
ALTER TABLE [dbo].[Tags] DROP CONSTRAINT [FK_Tags_Titles]
GO
/****** Object:  Table [dbo].[Disks]    Script Date: 04/16/2009 13:20:44 ******/
ALTER TABLE [dbo].[Disks] DROP CONSTRAINT [FK_Disks_Disks]
GO
DROP TABLE [dbo].[Disks]
GO
/****** Object:  Table [dbo].[Tags]    Script Date: 04/16/2009 13:20:44 ******/
ALTER TABLE [dbo].[Tags] DROP CONSTRAINT [FK_Tags_Titles]
GO
DROP TABLE [dbo].[Tags]
GO
/****** Object:  Table [dbo].[Genres]    Script Date: 04/16/2009 13:20:44 ******/
ALTER TABLE [dbo].[Genres] DROP CONSTRAINT [FK_Genres_GenreMetaData]
GO
ALTER TABLE [dbo].[Genres] DROP CONSTRAINT [FK_Genres_Titles]
GO
DROP TABLE [dbo].[Genres]
GO
/****** Object:  Table [dbo].[ImageMappings]    Script Date: 04/16/2009 13:20:44 ******/
ALTER TABLE [dbo].[ImageMappings] DROP CONSTRAINT [FK_ImageMappings_DBImages]
GO
ALTER TABLE [dbo].[ImageMappings] DROP CONSTRAINT [FK_ImageMappings_Titles]
GO
DROP TABLE [dbo].[ImageMappings]
GO
/****** Object:  Table [dbo].[People]    Script Date: 04/16/2009 13:20:44 ******/
ALTER TABLE [dbo].[People] DROP CONSTRAINT [FK_People_Bio]
GO
ALTER TABLE [dbo].[People] DROP CONSTRAINT [FK_People_Titles]
GO
DROP TABLE [dbo].[People]
GO
/****** Object:  Table [dbo].[ScannerSettings]    Script Date: 04/16/2009 13:20:44 ******/
DROP TABLE [dbo].[ScannerSettings]
GO
/****** Object:  Table [dbo].[Settings]    Script Date: 04/16/2009 13:20:44 ******/
DROP TABLE [dbo].[Settings]
GO
/****** Object:  Table [dbo].[MataDataMappings]    Script Date: 04/16/2009 13:20:44 ******/
DROP TABLE [dbo].[MataDataMappings]
GO
/****** Object:  Table [dbo].[Titles]    Script Date: 04/16/2009 13:20:44 ******/
DROP TABLE [dbo].[Titles]
GO
/****** Object:  Table [dbo].[WatchedFolders]    Script Date: 04/16/2009 13:20:44 ******/
DROP TABLE [dbo].[WatchedFolders]
GO
/****** Object:  Table [dbo].[GenreMappings]    Script Date: 04/16/2009 13:20:44 ******/
DROP TABLE [dbo].[GenreMappings]
GO
/****** Object:  Table [dbo].[GenreMetaData]    Script Date: 04/16/2009 13:20:44 ******/
DROP TABLE [dbo].[GenreMetaData]
GO
/****** Object:  Table [dbo].[BioData]    Script Date: 04/16/2009 13:20:44 ******/
DROP TABLE [dbo].[BioData]
GO
/****** Object:  Table [dbo].[DBImages]    Script Date: 04/16/2009 13:20:44 ******/
DROP TABLE [dbo].[DBImages]
GO
/****** Object:  Table [dbo].[DBImages]    Script Date: 04/16/2009 13:20:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DBImages](
	[Id] [int] IDENTITY(-2147483647,1) NOT NULL,
	[Image] [image] NOT NULL,
 CONSTRAINT [PK_Images] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BioData]    Script Date: 04/16/2009 13:20:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BioData](
	[FullName] [nvarchar](255) NOT NULL,
	[Photo] [image] NULL,
	[Id] [bigint] IDENTITY(-9223372036854775808,1) NOT NULL,
	[DateOfBirth] [smalldatetime] NULL,
 CONSTRAINT [PK_BioData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ_BioData_FullName] UNIQUE NONCLUSTERED 
(
	[FullName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_BioData] ON [dbo].[BioData] 
(
	[FullName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GenreMetaData]    Script Date: 04/16/2009 13:20:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GenreMetaData](
	[Name] [nvarchar](255) NOT NULL,
	[Photo] [image] NULL,
	[Id] [bigint] IDENTITY(-9223372036854775808,1) NOT NULL,
 CONSTRAINT [PK_GenreMetaData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UQ_GenreMetaData_Name] UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GenreMappings]    Script Date: 04/16/2009 13:20:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GenreMappings](
	[RowID] [int] IDENTITY(1,1) NOT NULL,
	[GenreName] [nvarchar](50) NOT NULL,
	[GenreMapTo] [nvarchar](50) NULL,
 CONSTRAINT [PK_GenreMappings] PRIMARY KEY CLUSTERED 
(
	[GenreName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WatchedFolders]    Script Date: 04/16/2009 13:20:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WatchedFolders](
	[Folder] [nvarchar](255) NOT NULL,
	[IncludeSubFolders] [bit] NULL,
 CONSTRAINT [PK_WatchedFolders] PRIMARY KEY CLUSTERED 
(
	[Folder] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Titles]    Script Date: 04/16/2009 13:20:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Titles](
	[Id] [int] IDENTITY(-2147483647,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[SortName] [nvarchar](255) NULL,
	[WatchedCount] [int] NULL,
	[MetaDataSource] [nvarchar](200) NULL,
	[Runtime] [smallint] NULL,
	[ParentalRating] [nvarchar](20) NULL,
	[Synopsis] [nvarchar](max) NULL,
	[Studio] [nvarchar](255) NULL,
	[CountryOfOrigin] [nvarchar](255) NULL,
	[WebsiteUrl] [nvarchar](255) NULL,
	[ReleaseDate] [smalldatetime] NULL,
	[DateAdded] [smalldatetime] NULL,
	[AudioTracks] [nvarchar](255) NULL,
	[UserRating] [tinyint] NULL,
	[AspectRatio] [nvarchar](10) NULL,
	[VideoStandard] [nvarchar](10) NULL,
	[UPC] [nvarchar](100) NULL,
	[Trailers] [nvarchar](255) NULL,
	[ParentalRatingReason] [nvarchar](255) NULL,
	[VideoDetails] [nvarchar](max) NULL,
	[Subtitles] [nvarchar](255) NULL,
	[VideoResolution] [nvarchar](20) NULL,
	[OriginalName] [nvarchar](255) NULL,
	[ImporterSource] [nvarchar](255) NULL,
	[GroupId] [int] NULL,
	[PercentComplete] [int] NULL,
	[MetaDataSourceItemId] [nvarchar](255) NULL,
	[ProductionYear] [int] NULL,
 CONSTRAINT [PK_Titles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MataDataMappings]    Script Date: 04/16/2009 13:20:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MataDataMappings](
	[RowID] [int] IDENTITY(1,1) NOT NULL,
	[MatadataProperty] [nvarchar](50) NOT NULL,
	[MetadataProvider] [nvarchar](50) NULL,
 CONSTRAINT [PK_MataDataMappings] PRIMARY KEY CLUSTERED 
(
	[MatadataProperty] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Settings]    Script Date: 04/16/2009 13:20:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Settings](
	[SettingName] [nvarchar](50) NOT NULL,
	[SettingValue] [nvarchar](max) NULL,
	[InstanceName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Settings] PRIMARY KEY CLUSTERED 
(
	[SettingName] ASC,
	[InstanceName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScannerSettings]    Script Date: 04/16/2009 13:20:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScannerSettings](
	[Enabled] [bit] NULL,
	[MetaData] [nvarchar](500) NULL,
	[LastModified] [datetime] NOT NULL,
 CONSTRAINT [PK_ScannerSettings] PRIMARY KEY CLUSTERED 
(
	[LastModified] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[People]    Script Date: 04/16/2009 13:20:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[People](
	[CharacterName] [nvarchar](255) NULL,
	[TitleId] [int] NOT NULL,
	[Sort] [smallint] NOT NULL,
	[Role] [tinyint] NOT NULL,
	[BioId] [bigint] NOT NULL,
	[Id] [bigint] IDENTITY(-9223372036854775808,1) NOT NULL,
 CONSTRAINT [PK_People] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [CIX_People_Title] ON [dbo].[People] 
(
	[TitleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_People] ON [dbo].[People] 
(
	[BioId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ImageMappings]    Script Date: 04/16/2009 13:20:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImageMappings](
	[TitleId] [int] NOT NULL,
	[ImageId] [int] NOT NULL,
	[ImageType] [tinyint] NOT NULL,
 CONSTRAINT [PK_ImageMappings] PRIMARY KEY CLUSTERED 
(
	[TitleId] ASC,
	[ImageId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ImageMappings] ON [dbo].[ImageMappings] 
(
	[TitleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Genres]    Script Date: 04/16/2009 13:20:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Genres](
	[TitleId] [int] NOT NULL,
	[GenreMetaDataId] [bigint] NOT NULL,
	[Id] [bigint] IDENTITY(-9223372036854775808,1) NOT NULL,
 CONSTRAINT [PK_Genres] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [CIX_Genres_Title] ON [dbo].[Genres] 
(
	[TitleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tags]    Script Date: 04/16/2009 13:20:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tags](
	[TitleId] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Id] [bigint] IDENTITY(-9223372036854775808,1) NOT NULL,
 CONSTRAINT [PK_Tags] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [CIX_Tags_Title] ON [dbo].[Tags] 
(
	[TitleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Disks]    Script Date: 04/16/2009 13:20:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Disks](
	[Name] [nvarchar](255) NOT NULL,
	[Path] [nvarchar](255) NOT NULL,
	[VideoFormat] [tinyint] NOT NULL,
	[Id] [bigint] IDENTITY(-9223372036854775808,1) NOT NULL,
	[TitleId] [int] NOT NULL,
	[ExtraOptions] [nvarchar](255) NULL,
	[MainFeatureXRes] [int] NULL,
	[MainFeatureYRes] [int] NULL,
	[MainFeatureAspectRatio] [nvarchar](10) NULL,
	[MainFeatureFPS] [float] NULL,
	[MainFeatureLength] [int] NULL,
 CONSTRAINT [PK_Disks] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE CLUSTERED INDEX [CIX_Disks_Title] ON [dbo].[Disks] 
(
	[TitleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  ForeignKey [FK_Disks_Disks]    Script Date: 04/16/2009 13:20:44 ******/
ALTER TABLE [dbo].[Disks]  WITH CHECK ADD  CONSTRAINT [FK_Disks_Disks] FOREIGN KEY([TitleId])
REFERENCES [dbo].[Titles] ([Id])
GO
ALTER TABLE [dbo].[Disks] CHECK CONSTRAINT [FK_Disks_Disks]
GO
/****** Object:  ForeignKey [FK_Genres_GenreMetaData]    Script Date: 04/16/2009 13:20:44 ******/
ALTER TABLE [dbo].[Genres]  WITH CHECK ADD  CONSTRAINT [FK_Genres_GenreMetaData] FOREIGN KEY([GenreMetaDataId])
REFERENCES [dbo].[GenreMetaData] ([Id])
GO
ALTER TABLE [dbo].[Genres] CHECK CONSTRAINT [FK_Genres_GenreMetaData]
GO
/****** Object:  ForeignKey [FK_Genres_Titles]    Script Date: 04/16/2009 13:20:44 ******/
ALTER TABLE [dbo].[Genres]  WITH CHECK ADD  CONSTRAINT [FK_Genres_Titles] FOREIGN KEY([TitleId])
REFERENCES [dbo].[Titles] ([Id])
GO
ALTER TABLE [dbo].[Genres] CHECK CONSTRAINT [FK_Genres_Titles]
GO
/****** Object:  ForeignKey [FK_ImageMappings_DBImages]    Script Date: 04/16/2009 13:20:44 ******/
ALTER TABLE [dbo].[ImageMappings]  WITH CHECK ADD  CONSTRAINT [FK_ImageMappings_DBImages] FOREIGN KEY([ImageId])
REFERENCES [dbo].[DBImages] ([Id])
GO
ALTER TABLE [dbo].[ImageMappings] CHECK CONSTRAINT [FK_ImageMappings_DBImages]
GO
/****** Object:  ForeignKey [FK_ImageMappings_Titles]    Script Date: 04/16/2009 13:20:44 ******/
ALTER TABLE [dbo].[ImageMappings]  WITH CHECK ADD  CONSTRAINT [FK_ImageMappings_Titles] FOREIGN KEY([TitleId])
REFERENCES [dbo].[Titles] ([Id])
GO
ALTER TABLE [dbo].[ImageMappings] CHECK CONSTRAINT [FK_ImageMappings_Titles]
GO
/****** Object:  ForeignKey [FK_People_Bio]    Script Date: 04/16/2009 13:20:44 ******/
ALTER TABLE [dbo].[People]  WITH CHECK ADD  CONSTRAINT [FK_People_Bio] FOREIGN KEY([BioId])
REFERENCES [dbo].[BioData] ([Id])
GO
ALTER TABLE [dbo].[People] CHECK CONSTRAINT [FK_People_Bio]
GO
/****** Object:  ForeignKey [FK_People_Titles]    Script Date: 04/16/2009 13:20:44 ******/
ALTER TABLE [dbo].[People]  WITH CHECK ADD  CONSTRAINT [FK_People_Titles] FOREIGN KEY([TitleId])
REFERENCES [dbo].[Titles] ([Id])
GO
ALTER TABLE [dbo].[People] CHECK CONSTRAINT [FK_People_Titles]
GO
/****** Object:  ForeignKey [FK_Tags_Titles]    Script Date: 04/16/2009 13:20:44 ******/
ALTER TABLE [dbo].[Tags]  WITH CHECK ADD  CONSTRAINT [FK_Tags_Titles] FOREIGN KEY([TitleId])
REFERENCES [dbo].[Titles] ([Id])
GO
ALTER TABLE [dbo].[Tags] CHECK CONSTRAINT [FK_Tags_Titles]
GO
