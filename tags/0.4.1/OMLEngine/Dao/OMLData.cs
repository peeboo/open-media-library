namespace OMLEngine.Dao
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    partial class OMLDataDataContext
    {
    }

    partial class Title
    {
        private decimal percentComplete = -1;

        public decimal PercentComplete
        {
            get
            {
                if (percentComplete == -1)
                {
                    decimal score = 0;
                    decimal possible = 12;

                    if (!String.IsNullOrEmpty(Studio)) score++;
                    if (Runtime > 0) score++;
                    if (ReleaseDate != DateTime.MinValue) score++;
                    if (!String.IsNullOrEmpty(Synopsis)) score++;
                    if (Genres.Count > 0) score++;
                    if (Images.FirstOrDefault(t => t.ImageType == (byte)ImageType.FrontCoverImage) != null) score++;
                    if (!String.IsNullOrEmpty(AspectRatio)) score++;
                    if (!String.IsNullOrEmpty(CountryOfOrigin)) score++;
                    if (!String.IsNullOrEmpty(VideoResolution)) score++;
                    if (!String.IsNullOrEmpty(VideoStandard)) score++;
                    if (People.Where(p => p.Role == (byte)PeopleRole.Actor).Count() > 0) score++;
                    if (People.Where(p => p.Role == (byte)PeopleRole.Director).Count() > 0) score++;

                    percentComplete = score / possible;
                }

                return percentComplete;
            }
        }

        internal void ResetPercentComplete()
        {
            percentComplete = -1;
        }

        public string UpdatedFrontCoverPath { get; set; }
        public string UpdatedBackCoverPath { get; set; }
        public List<string> UpdatedGenres { get; set; }
        public List<string> UpdatedTags { get; set; }
        
        public List<Role> UpdatedActors { get; set; }
        public List<Role> UpdatedNonActingRoles { get; set; }

        public List<OMLEngine.Person> UpdatedDirectors { get; set; }
        public List<OMLEngine.Person> UpdatedWriters { get; set; }
        public List<OMLEngine.Person> UpdatedProducers { get; set; }

        public List<string> UpdatedFanArtPaths { get; set; }

        public override string ToString()
        {
            if (_Name != null)
                return _Name;
            else
                return base.ToString();
        }
    }

    partial class BioData
    {
        public string UpdatedImagePath { get; set; }
    }

    partial class GenreMetaData
    {
        public string UpdatedImagePath { get; set; }
    }
}
