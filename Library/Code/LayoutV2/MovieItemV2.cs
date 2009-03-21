using System;
using System.IO;
using Microsoft.MediaCenter.UI;
using OMLEngine;

namespace Library
{
    public class MovieItemV2 : BaseModelItem
    {
        Title title;
        Image frontCoverImage;

        public String Name
        {
            get { return title.Name; }
        }

        public Image FrontCoverImage
        {
            get { return frontCoverImage; }
            set
            {
                frontCoverImage = value;
                FirePropertyChanged("FrontCoverImage");
            }
        }

        public Title TitleObject
        {
            get { return title; }
        }

        public MovieItemV2(Title t)
        {
            title = t;
            if (!string.IsNullOrEmpty(t.FrontCoverPath) && File.Exists(t.FrontCoverPath))
            {
                FrontCoverImage = new Image("file://" + t.FrontCoverPath);
            }
        }
    }
}
