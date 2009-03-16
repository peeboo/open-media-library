using System;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;

namespace Library
{
    public class BasePage : ModelItem
    {
        #region private variables
        bool _ShowWaitCursor;
        bool _StartingTranscodeJob;
        string _PageName;
        bool _ShowAnimations;
        bool _IsParentalControlActive = false;
        OMLSettings _OMLSettings;
        I18n _I18n;
        #endregion

        #region properties
        public OMLApplication App
        {
            get { return OMLApplication.Current; }
        }

        public I18n I18n
        {
            get { return _I18n; }
        }

        public OMLSettings UISettings
        {
            get { return _OMLSettings; }
        }

        public bool IsParentalControlActive
        {
            get { return _IsParentalControlActive; }
            set
            {
                _IsParentalControlActive = value;
                FirePropertyChanged("IsParentalControlActive");
            }
        }

        public bool ShowAnimations
        {
            get { return _ShowAnimations; }
            set
            {
                _ShowAnimations = value;
                FirePropertyChanged("ShowAnimations");
            }
        }

        public string PageName
        {
            get { return _PageName; }
            set
            {
                _PageName = value;
                FirePropertyChanged("PageName");
            }
        }

        public bool ShowWaitCursor
        {
            get { return _ShowWaitCursor; }
            set
            {
                _ShowWaitCursor = value;
                FirePropertyChanged("ShowWaitCursor");
            }
        }

        public bool StartingTranscodeJob
        {
            get { return _StartingTranscodeJob; }
            set
            {
                _StartingTranscodeJob = value;
                FirePropertyChanged("StartingTranscodeJob");
            }
        }

        public AddInHost Host
        {
            get { return AddInHost.Current; }
        }

        public HistoryOrientedPageSession Session
        {
            get { return OMLApplication.Current.Session; }
        }

        #endregion

        public BasePage()
        {
            _OMLSettings = new OMLSettings();
            _I18n = I18n.Instance;
        }
    }
}
