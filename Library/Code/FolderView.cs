using System;
using Microsoft.MediaCenter.UI;
using OMLEngine;

namespace Library
{
    public class FolderView : BaseModelItem
    {
        #region variables
        public static Image DefaultFolderImage =
            new Image("resx://Library/Library.Resources/Folder_Closed");

        VirtualDirectory vdir;
        Title title;
        Image folderImage;
        #endregion

        #region Properties
        public bool ContainsTitle
        {
            get { return (title == null); }
        }

        public Title Title
        {
            get { return title; }
            set
            {
                title = value;
                FirePropertyChanged("Title");
            }
        }

        public Image FolderImage
        {
            get { return folderImage; }
            set
            {
                folderImage = value;
                FirePropertyChanged("FolderImage");
            }
        }
        #endregion

        public FolderView()
        {
            vdir = new VirtualDirectory();
            FolderImage = FolderView.DefaultFolderImage;

            foreach (string baseFolder in Properties.Settings.Default.BaseFolders)
            {
                vdir.AddChildDirectory(baseFolder);
            }
        }
    }
}
