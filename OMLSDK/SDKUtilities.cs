using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using OMLEngine;
using OMLEngine.FileSystem;


namespace OMLSDK
{
    public static class SDKUtilities
    {
        public static void DebugLine(string log)
        {
            Utilities.DebugLine("[DVDLibraryImporter] created");
        } 
        
        public static void DebugLine(string log, params object[] paramArray)
        {
            Utilities.DebugLine("[DVDLibraryImporter] created", paramArray);
        }

        public static string PluginsDirectory 
        {
            get
            {
                return FileSystemWalker.PluginsDirectory;
            }
        }


        public static bool IsDVD(string fullPath)
        {
            return FileScanner.IsDVD(fullPath);
        }
        
        public static bool IsBluRay(string fullPath)
        {
            return FileScanner.IsBluRay(fullPath);
        }
        
        public static bool IsHDDVD(string fullPath)
        {
            return FileScanner.IsHDDVD(fullPath);
        }


        public static string GetFolderNameString(string name)
        {
            return FileHelper.GetFolderNameString(name);
        }  

        public static string GetNameFromPath(string path)
        {
            return FileHelper.GetFolderNameString(path);
        }

        public static IEnumerable<string> GetUniqueMetaIds(IEnumerable<string> metaIds, string metaDataSource)
        {
            return TitleCollectionManager.GetUniqueMetaIds(metaIds, metaDataSource);
        }


        private static string CopyString(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";
            else
                return s;
        }

        public static Title ConvertOMLSDKTitleToTitle(OMLSDKTitle omlsdktitle)
        {
            Title _title = new Title();

            if (omlsdktitle != null)
            {
                _title.Name = CopyString(omlsdktitle.Name);
                _title.OriginalName = CopyString(omlsdktitle.OriginalName);
                _title.SortName = CopyString(omlsdktitle.SortName);
                _title.Synopsis = CopyString(omlsdktitle.Synopsis);
                _title.ProductionYear = omlsdktitle.ProductionYear;
                _title.ReleaseDate = omlsdktitle.ReleaseDate;
                _title.DateAdded = omlsdktitle.DateAdded;
                _title.Runtime = omlsdktitle.Runtime;
                _title.Studio = CopyString(omlsdktitle.Studio);
                _title.UPC = CopyString(omlsdktitle.UPC);
                _title.WatchedCount = omlsdktitle.WatchedCount;
                _title.UserStarRating = omlsdktitle.UserStarRating;

                _title.EpisodeNumber = omlsdktitle.EpisodeNumber;
                _title.SeasonNumber = omlsdktitle.SeasonNumber;

                _title.AspectRatio = CopyString(omlsdktitle.AspectRatio);
                _title.VideoDetails = CopyString(omlsdktitle.VideoDetails);
                _title.VideoResolution = CopyString(omlsdktitle.VideoResolution);
                _title.VideoStandard = CopyString(omlsdktitle.VideoStandard);
                //_title.VideoFormat = (VideoFormat)Enum.Parse(typeof(VideoFormat), omlsdktitle.VideoFormat.ToString());

                _title.FrontCoverPath = CopyString(omlsdktitle.FrontCoverPath);
                _title.BackCoverPath = CopyString(omlsdktitle.BackCoverPath);

                _title.CountryOfOrigin = CopyString(omlsdktitle.CountryOfOrigin);

                _title.MetadataSourceID = CopyString(omlsdktitle.MetadataSourceID);
                _title.MetadataSourceName = CopyString(omlsdktitle.MetadataSourceName);
                _title.ImporterSource = CopyString(omlsdktitle.ImporterSource);

                _title.OfficialWebsiteURL = CopyString(omlsdktitle.OfficialWebsiteURL);
                _title.ParentalRating = CopyString(omlsdktitle.ParentalRating);
                _title.ParentalRatingReason = CopyString(omlsdktitle.ParentalRatingReason);


                #region Disks
                foreach (OMLSDKDisk omlsdkdisk in omlsdktitle.Disks)
                {
                    Disk disk = new Disk();
                    disk.Name = omlsdkdisk.Name;
                    disk.Path = omlsdkdisk.Path;
                    disk.Format = (VideoFormat)Enum.Parse(typeof(VideoFormat), omlsdkdisk.Format.ToString());
                    disk.ExtraOptions = omlsdkdisk.ExtraOptions;
                    _title.AddDisk(disk);
                }
                #endregion


                #region Extra Features
                _title.ExtraFeatures = omlsdktitle.ExtraFeatures;
                #endregion
                
                
                #region Trailers
                foreach (string Trailer in omlsdktitle.Trailers)
                {
                    _title.AddTrailer(Trailer);
                }
                #endregion


                #region Genres
                if (omlsdktitle.Genres != null)
                {
                    foreach (string genre in omlsdktitle.Genres)
                    {
                        _title.AddGenre(genre);
                    }
                }
                #endregion


                #region Actring Roles
                if (omlsdktitle.ActingRoles != null)
                {
                    foreach (OMLSDKRole role in omlsdktitle.ActingRoles)
                    {
                        _title.AddActingRole(role.PersonName, role.RoleName);
                    }
                }
                #endregion


                #region Non Acting Roles
                if (omlsdktitle.NonActingRoles != null)
                {
                    foreach (OMLSDKRole role in omlsdktitle.NonActingRoles)
                    {
                        _title.AddNonActingRole(role.PersonName, role.RoleName);
                    }
                }
                #endregion


                #region Directors
                if (omlsdktitle.Directors != null)
                {
                    foreach (OMLSDKPerson person in omlsdktitle.Directors)
                    {
                        _title.AddDirector(ConvertOMLSDKPersonToPerson(person));
                    }
                }
                #endregion


                #region Writers
                if (omlsdktitle.Writers != null)
                {
                    foreach (OMLSDKPerson person in omlsdktitle.Writers)
                    {
                        _title.AddWriter(ConvertOMLSDKPersonToPerson(person));
                    }
                }
                #endregion


                #region Producers
                if (omlsdktitle.Producers != null)
                {
                    foreach (OMLSDKPerson person in omlsdktitle.Producers)
                    {
                        _title.AddProducer(ConvertOMLSDKPersonToPerson(person));
                    }
                }
                #endregion


                #region Tags
                if (omlsdktitle.Tags != null)
                {
                    foreach (string tag in omlsdktitle.Tags)
                    {
                        _title.AddTag(tag);
                    }
                }
                #endregion


                #region Audio Tracks
                if (omlsdktitle.AudioTracks != null)
                {
                    foreach (string track in omlsdktitle.AudioTracks)
                    {
                        _title.AddAudioTrack(track);
                    }
                }
                #endregion


                #region Subtitles
                if (omlsdktitle.Subtitles != null)
                {
                    foreach (string subtitles in omlsdktitle.Subtitles)
                    {
                        _title.AddSubtitle(subtitles);
                    }
                }
                #endregion
               

                #region Fanart Paths
                if (omlsdktitle.FanArtPaths != null)
                {
                    foreach (string path in omlsdktitle.FanArtPaths)
                    {
                        _title.AddFanArtImage(path);
                    }
                }
                #endregion
            }

            return _title;
        }


        public static Title[] ConvertOMLSDKTitlesToTitles(OMLSDKTitle[] omlsdktitles)
        {
            List<Title> titles = new List<Title>();

            foreach (OMLSDKTitle omlsdktitle in omlsdktitles)
            {
                if (omlsdktitle != null)
                {
                    titles.Add(ConvertOMLSDKTitleToTitle(omlsdktitle));
                }
            }
            return titles.ToArray();
        }

        public static IList<Title> ConvertOMLSDKTitlesToTitles(IList<OMLSDKTitle> omlsdktitles)
        {
            IList<Title> titles = new List<Title>();

            foreach (OMLSDKTitle omlsdktitle in omlsdktitles)
            {
                if (omlsdktitle != null)
                {
                    titles.Add(ConvertOMLSDKTitleToTitle(omlsdktitle));
                }
            }
            return titles.ToArray();
        }

        private static Person ConvertOMLSDKPersonToPerson(OMLSDKPerson person)
        {
            Person newperson = new Person();
            newperson.BirthDate = person.BirthDate;
            newperson.full_name = person.full_name;
            newperson.PhotoPath = person.PhotoPath;
            newperson.sex = (Sex)person.sex;
            return newperson;
        }

    }
}
