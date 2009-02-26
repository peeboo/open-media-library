using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.MediaCenter.UI;

namespace Library
{
     public class SettingsUIWrapper:ModelItem
         
    {
        Settings _settings;

        #region Properties
        private Choice _ChoiceGalleryAdvancedSettingsItems;
        public Choice ChoiceGalleryAdvancedSettingsItems
        {
            get { return _ChoiceGalleryAdvancedSettingsItems; }
        }

        private Choice _ChoiceGalleryViewSettingsItems;
        public Choice ChoiceGalleryViewSettingsItems
        {
            get { return _ChoiceGalleryViewSettingsItems; }
        }

        private Choice _ChoiceExtenderSettingsItems;
        public Choice ChoiceExtenderSettingsItems
        {
            get { return _ChoiceExtenderSettingsItems; }
        }

        private Choice _ChoiceFilterOptions;
        public Choice ChoiceFilterOptions
        {
            get { return _ChoiceFilterOptions; }
        }
         
        
        #endregion

        public SettingsUIWrapper ()
        {
            _ChoiceFilterOptions = new Choice();
        }

        public void Init(Settings settings)
        {
            _settings = settings;
           
            _ChoiceGalleryAdvancedSettingsItems = new Choice();
            ArrayListDataSet aryListDS = new ArrayListDataSet();

            aryListDS.Add(new SettingsItem(0, _settings.DimUnselectedCovers, "Dim Unselected Covers",false));
            aryListDS.Add(new SettingsItem(0, _settings.UseOnScreenAlpha, "Show Alpha Jump Bar", false));
            aryListDS.Add(new SettingsItem(0, _settings.ShowMovieDetails, "Show Movie Details", false));
            aryListDS.Add(new SettingsItem(0, _settings.ShowWatchedIcon, "Show Watched Icon", false));
            aryListDS.Add(new SettingsItem(0, _settings.UseOriginalCoverArt, "Use Original Cover Art(may slow browsing)", true));

            aryListDS.Add(new SettingsItem(1, _settings.CoverArtRows, "Cover Art Rows:", false));
            aryListDS.Add(new SettingsItem(1, _settings.CoverArtSpacing, "Cover Art Spacing:", true));
            aryListDS.Add(new SettingsItem(1, _settings.MainPageBackDropInterval, "Random Backdrop Interval:", false));

            aryListDS.Add(new SettingsItem(3, _settings.MainPageBackDropAlpha, "Backdrop Opacity:", true));

            _ChoiceGalleryAdvancedSettingsItems.Options = aryListDS;

            _ChoiceGalleryViewSettingsItems = new Choice();
            ArrayListDataSet aryListDS2 = new ArrayListDataSet();

            aryListDS2.Add(new SettingsItem(2, _settings.MovieView, "Movie Gallery View:", false));
            aryListDS2.Add(new SettingsItem(2, _settings.MovieSort, "Movie Sort:", false));
            aryListDS2.Add(new SettingsItem(2, _settings.StartPage, "Start Page:", false));

            _ChoiceGalleryViewSettingsItems.Options = aryListDS2;

            _ChoiceFilterOptions=new Choice();
            ArrayListDataSet aryListDS3=new ArrayListDataSet();

            aryListDS3.Add(new CheckBox(_settings.ShowFilterActors, "Actors"));
            aryListDS3.Add(new CheckBox(_settings.ShowFilterCountry, "Country"));
            aryListDS3.Add(new CheckBox(_settings.ShowFilterDateAdded, "Date Added"));
            aryListDS3.Add(new CheckBox(_settings.ShowFilterDirectors, "Directors"));
            aryListDS3.Add(new CheckBox(_settings.ShowFilterGenres, "Genres"));
            aryListDS3.Add(new CheckBox(_settings.ShowFilterParentalRating, "Parental Rating"));
            aryListDS3.Add(new CheckBox(_settings.ShowFilterYear, "Release Year"));
            aryListDS3.Add(new CheckBox(_settings.ShowFilterRuntime, "Runtime"));
            aryListDS3.Add(new CheckBox(_settings.ShowFilterTags, "Tags"));
            aryListDS3.Add(new CheckBox(_settings.ShowFilterTrailers, "Trailers"));
            aryListDS3.Add(new CheckBox(_settings.ShowFilterUserRating, "User Rating"));
            aryListDS3.Add(new CheckBox(_settings.ShowFilterUnwatched, "Unwatched"));
            aryListDS3.Add(new CheckBox(_settings.ShowFilterFormat, "Video Format"));

            _ChoiceFilterOptions.Options = aryListDS3;

            _ChoiceExtenderSettingsItems = new Choice();
            ArrayListDataSet aryListDS4 = new ArrayListDataSet();

            aryListDS4.Add(new SettingsItem(4, _settings.ImpersonationUsername, "Impersonation Username:", false));
            aryListDS4.Add(new SettingsItem(4, _settings.ImpersonationPassword, "Impersonation Password:", false));
            aryListDS4.Add(new SettingsItem(4, _settings.TranscodingBufferDelay, "Number Of Seconds To Delay:", true));
            aryListDS4.Add(new SettingsItem(0, _settings.TranscodeAVIFiles, "Transcode AVI Files", false));
            aryListDS4.Add(new SettingsItem(0, _settings.TranscodeMKVFiles, "Transcode MKV Files", false));
            aryListDS4.Add(new SettingsItem(0, _settings.TranscodeOGMFiles, "Transcode OGM Files", false));
            aryListDS4.Add(new SettingsItem(0, _settings.PreserveAudioOnTranscode, "Preserve audio format on transcode of non DVD files", false));
            aryListDS4.Add(new SettingsItem(0, _settings.DebugTranscoding, "Transcode ALL DVDs (DEBUG Only)", false));

            _ChoiceExtenderSettingsItems.Options = aryListDS4;
        }

        public void LoadGallery(OMLApplication app)
        {
           // app.Startup(null);
            app.GoToBackPage();
        }
    }

    public class SettingsItem
    {
        #region Properties
        private int _Type;
        public int Type
        {
            get { return _Type; }
        }

        private object _ObjData;
        public object ObjData
        {
            get { return _ObjData; }
        }

        private string _Text;
        public string Text
        {
            get { return _Text; }
        }

        private bool _ShowDividerLine;
        public bool ShowDividerLine
        {
            get { return _ShowDividerLine; }
        }


     
        #endregion

        public SettingsItem(int settingType, Object data, string textValue, bool dividerLine)
        {
            _Type = settingType;
            _ObjData = data;
            _Text = textValue;
            _ShowDividerLine = dividerLine;
        }
    }

    public class CheckBox
    {
        private BooleanChoice _Checked;
        public BooleanChoice Checked
        {
            get { return _Checked; }
            set { _Checked = value; }
        }

        private string _Text;
        public string Text
        {
            get { return _Text; }
        }
        
 
        public CheckBox(BooleanChoice val, string str)
        {
            _Checked = val;
            _Text = str;
        }
    }
}


