CREATE TABLE AudioTracks
(
	AudioTrackNo		numeric,
	Description			nvarchar(50),
	Format				nvarchar(50)
);

CREATE TABLE Categories
(
	CategoryNo			numeric,
	Description			nvarchar(50)
);

CREATE TABLE Discs
(
	DiscNo				numeric,
	DiscName			nvarchar(50),
	Location			nvarchar(MAX),
	VideoFormat			nvarchar(50)
);

CREATE TABLE Genres
(
	GenreNo				numeric,
	Description			nvarchar(50)
);

CREATE TABLE Languages
(
	LanguageNo			numeric,
	Description			nvarchar(50)
);

CREATE TABLE Persons
(
	PersonNo			numeric,
	Fullname			nvarchar(MAX)
);

CREATE TABLE Photos
(
	PhotoNo				numeric,
	Photo				image
);

CREATE TABLE Studios
(
	StudioNo			numeric,
	Description			nvarchar(50)
);

CREATE TABLE Subtitles
(
	SubtitleNo			numeric,
	LanguageNo			numeric,
	Description			nvarchar(50)
);

CREATE TABLE Tags
(
	TagNo				numeric,
	Description			nvarchar(50)
);

CREATE TABLE Titles
(
	TitleNo				numeric,
	PrimaryTitleNo		numeric,
	DateAdded			datetime,
	MetaDataSourceNo	nvarchar(50),
	SourceName			nvarchar(50),
	FrontCoverNo		numeric,
	FrontCoverMenuNo	numeric,
	BackCoverNo			numeric,
	Runtime				int,
	ParentalRating		nchar(10),
	Synopsis			nvarchar(MAX),
	CountryOfOrigin		nvarchar(50),
	OfficialWebsite		nvarchar(MAX),
	ReleaseDate			datetime,
	ImporterSource		nvarchar(50),
	IMDBId				nvarchar(20),
	UserRating			float,
	AspectRatio			nvarchar(20),
	VideoStandard		nvarchar(20),
	UPC					numeric,
	OriginalName		nvarchar(MAX),
	SortName			nvarchar(MAX),
	RatingReason		nvarchar(MAX),
	VideoResolution		nvarchar(20)
);

CREATE TABLE TitleAudioTracks
(
	TitleNo				numeric,
	AudioTrackNo		numeric
);

CREATE TABLE TitleCategories
(
	TitleNo				numeric,
	CategoryNo			numeric,
);

CREATE TABLE TitleDiscs
(
	TitleNo				numeric,
	DiscNo				numeric,
	DiscOrder			int
);

CREATE TABLE TitleGenres
(
	TitleNo				numeric,
	GenreNo				numeric
);

CREATE TABLE TitleJobs
(
	TitleNo				numeric,
	PersonNo			numeric,
	JobTypeNo			numeric
);

CREATE TABLE TitleStudios
(
	TitleNo				numeric,
	StudioNo			numeric
);

CREATE TABLE TitleSubtitles
(
	TitleNo				numeric,
	SubtitleNo			numeric
);

CREATE TABLE TitleTags
(
	TitleNo				numeric,
	TagNo				numeric
);