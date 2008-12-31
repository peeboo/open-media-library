using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMLEngine.Dao
{
    internal static class TitleCollectionDao
    {
        public static GenreMetaData GetGenreMetaDataByName(string name)
        {
            return DBContext.Instance.GenreMetaDatas.SingleOrDefault(t => t.Name.ToLower() == name.ToLower());
        }

        public static BioData GetPersonBioDataByName(string name)
        {
            return DBContext.Instance.BioDatas.SingleOrDefault(t => t.FullName.ToLower() == name.ToLower());
        }

        public static void AddTitle(Title title)
        {
            DBContext.Instance.Titles.InsertOnSubmit(title);
            DBContext.Instance.SubmitChanges();
        }
    }
}
