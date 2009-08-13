using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OMLEngine.DatabaseManagement
{
    public class DatabaseIntegrityChecks
    {


        public bool CheckOrphanedTitles(out List<Title> OrphanedTitles)
        {
            Dictionary<int, Title> _movieList;
            OrphanedTitles = new List<Title>();

            _movieList = (TitleCollectionManager.GetAllTitles(TitleTypes.Everything)).ToDictionary(k => k.Id);

            foreach (KeyValuePair<int, Title> t in _movieList)
            {
                if (((t.Value.TitleType & TitleTypes.Root) == 0) && (t.Value.ParentTitleId != null))
                {
                    if (!_movieList.ContainsKey((int)t.Value.ParentTitleId))
                    {
                        // Cannot find parent
                        OrphanedTitles.Add(t.Value);
                    }
                }
            }

            if (OrphanedTitles.Count() != 0)
                return true;
            else
                return false;
        }
    }
}
