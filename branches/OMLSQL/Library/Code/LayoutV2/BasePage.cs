using System;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;

namespace Library
{
    public class BasePage : ModelItem
    {
        #region private variables
        private bool showWaitCursor;
        private bool startingTranscodeJob;
        private string pageName;
        private Image customBackgroundImage;
        private bool showAnimations;
        #endregion

        #region properties
        public bool ShowAnimations
        {
            get { return showAnimations; }
            set
            {
                showAnimations = value;
                FirePropertyChanged("ShowAnimations");
            }
        }

        public string PageName
        {
            get { return "TemplatePage"; }
            set
            {
                pageName = value;
                FirePropertyChanged("PageName");
            }
        }

        public bool ShowWaitCursor
        {
            get { return showWaitCursor; }
            set
            {
                showWaitCursor = value;
                FirePropertyChanged("ShowWaitCursor");
            }
        }

        public bool StartingTranscodeJob
        {
            get { return startingTranscodeJob; }
            set
            {
                startingTranscodeJob = value;
                FirePropertyChanged("StartingTranscodeJob");
            }
        }

        public Image CustomBackgroundImage
        {
            get { return customBackgroundImage; }
            set
            {
                customBackgroundImage = value;
                FirePropertyChanged("CustomBackgroundImage");
            }
        }

        public OMLApplication App
        {
            get { return OMLApplication.Current; }
        }

        public AddInHost Host
        {
            get { return AddInHost.Current; }
        }

        public HistoryOrientedPageSession Session
        {
            get { return App.Session; }
        }


        #endregion

        public BasePage()
        {
        }
    }
}
