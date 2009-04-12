namespace OMLEngine.Dao
{
    using System;
    using System.Linq;

    partial class OMLDataDataContext
    {
    }

    partial class Title
    {
        public decimal PercentComplete
        {
            get
            {
                decimal score = 0;
                decimal possible = 12;

                if (!String.IsNullOrEmpty(Studio.Trim())) score++;
                if (Runtime > 0) score++;
                if (ReleaseDate != DateTime.MinValue) score++;
                if (!String.IsNullOrEmpty(Synopsis.Trim())) score++;
                if (Genres.Count > 0) score++;
                if (!String.IsNullOrEmpty(FrontCoverPath.Trim())) score++;
                if (!String.IsNullOrEmpty(AspectRatio.Trim())) score++;
                if (!String.IsNullOrEmpty(CountryOfOrigin.Trim())) score++;
                if (!String.IsNullOrEmpty(VideoResolution.Trim())) score++;
                if (!String.IsNullOrEmpty(VideoStandard.Trim())) score++;
                if (People.Where(p => p.Role == (byte)PeopleRole.Actor).Count() > 0) score++;
                if (People.Where(p => p.Role == (byte)PeopleRole.Director).Count() > 0) score++;

                return score / possible;
            }
        }
    }
}
