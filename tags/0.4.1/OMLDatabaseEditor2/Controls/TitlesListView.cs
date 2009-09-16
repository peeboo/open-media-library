using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OMLEngine;
namespace OMLDatabaseEditor.Controls
{
    public partial class TitlesListView : UserControl
    {
        public TitlesListView()
        {
            InitializeComponent();
        }

        public void SetFilter(List<TitleFilter> tf)
        {
            try
            {
                List<Title> titles = TitleCollectionManager.GetFilteredTitles(tf).ToList();
                ShowResults(titles);
            }
            catch
            {
                grdTitles.Rows.Clear();
            }
        }

        private void ShowResults(List<Title> titles)
        {
            //_titles = null;
            grdTitles.Rows.Clear();
            //_titles = titles;
            if (titles != null)
            {
                int i = 0;
                foreach (Title t in titles)
                {
                    if (t != null)
                    {
                        Image coverArt = null;

                        if (t.FrontCoverPath != null)
                        {
                            coverArt = Utilities.ReadImageFromFile(t.FrontCoverPath);
                        }
                        
                        string releaseDate = "";
                        if (t.ReleaseDate.Year > 1900)
                            releaseDate = t.ReleaseDate.Year.ToString();

                        string  Name = t.Name;


                        grdTitles.Rows.Add(i.ToString(), coverArt, Name, t.Synopsis, releaseDate, MakeStringFromList(t.Genres), MakeStringFromPersonList(t.Directors), MakeStringFromRoleList(t.ActingRoles));
                        i++;
                    }
                }
            }

        }

        private string MakeStringFromList(IList<string> list)
        {
            string ret = "";
            if (list != null)
            {
                foreach (string s in list)
                {
                    if (ret.Length > 0) ret += ", ";
                    ret += s;
                }
            }
            return ret;
        }

        private string MakeStringFromRoleList(IList<Role> list)
        {
            string ret = "";
            if (list != null)
            {
                foreach (Role p in list)
                {
                    if (ret.Length > 0) ret += ", ";
                    ret += p.PersonName;
                }
            }
            return ret;
        }

        private string MakeStringFromPersonList(IList<Person> list)
        {
            string ret = "";
            if (list != null)
            {
                foreach (Person p in list)
                {
                    if (ret.Length > 0) ret += ", ";
                    ret += p.full_name;
                }
            }
            return ret;
        }
  
    }
}
