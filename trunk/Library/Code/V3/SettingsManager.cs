using System;
using System.Collections;
using Microsoft.MediaCenter.UI;
using System.Text;
using System.Threading;
using System.Data;
using System.Collections.Generic;


namespace Library.Code.V3
{
    /// <summary>
    /// This object contains the standard set of information displayed in the 
    /// details page UI.
    /// </summary>
    public class SettingsManager : BaseModelItem
    {
        public SettingsManager()
            : base()
        {
            this.title = "settings";
            this.Commands = new ArrayListDataSet();

            //filters
            Command generalCmd = new Command();
            generalCmd.Description = "General Settings";
            generalCmd.Invoked += new EventHandler(generalCmd_Invoked);
            this.Commands.Add(generalCmd);

            //favorites
            Command favoritesCmd = new Command();
            favoritesCmd.Description = "Favorites";
            favoritesCmd.Invoked += new EventHandler(favoritesCmd_Invoked);
            this.Commands.Add(favoritesCmd);

            //filters
            Command filtersCmd = new Command();
            filtersCmd.Description = "Filters";
            filtersCmd.Invoked += new EventHandler(filtersCmd_Invoked);
            this.Commands.Add(filtersCmd);

            //changers
            Command mediaChangersCmd = new Command();
            mediaChangersCmd.Description = "Media Changers";
            mediaChangersCmd.Invoked += new EventHandler(mediaChangersCmd_Invoked);
            this.Commands.Add(mediaChangersCmd);

            //optimization
            Command optimizationCmd = new Command();
            optimizationCmd.Description = "Optimization";
            optimizationCmd.Invoked += new EventHandler(optimizationCmd_Invoked);
            this.Commands.Add(optimizationCmd);

            //image mounting
            Command imageMountingCmd = new Command();
            imageMountingCmd.Description = "Image Mounting";
            imageMountingCmd.Invoked += new EventHandler(imageMountingCmd_Invoked);
            this.Commands.Add(imageMountingCmd);

            //lang
            Command languageCmd = new Command();
            languageCmd.Description = "Language";
            languageCmd.Invoked += new EventHandler(languageCmd_Invoked);
            this.Commands.Add(languageCmd);

            //trailers
            Command trailersCmd = new Command();
            trailersCmd.Description = "Trailers";
            trailersCmd.Invoked += new EventHandler(trailersCmd_Invoked);
            this.Commands.Add(trailersCmd);

            //extender
            Command extenderCmd = new Command();
            extenderCmd.Description = "Extender";
            extenderCmd.Invoked += new EventHandler(extenderCmd_Invoked);
            this.Commands.Add(extenderCmd);
        }

        void extenderCmd_Invoked(object sender, EventArgs e)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();

            Library.Code.V3.ExtenderSettings page = new Library.Code.V3.ExtenderSettings();
            properties["Page"] = page;
            properties["Application"] = OMLApplication.Current;

            OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_ExtenderSettings", properties);
        }

        void trailersCmd_Invoked(object sender, EventArgs e)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();

            Library.Code.V3.TrailersSettings page = new Library.Code.V3.TrailersSettings();
            properties["Page"] = page;
            properties["Application"] = OMLApplication.Current;

            OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_TrailersSettings", properties);
        }

        void languageCmd_Invoked(object sender, EventArgs e)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();

            Library.Code.V3.LanguageSettings page = new Library.Code.V3.LanguageSettings();
            properties["Page"] = page;
            properties["Application"] = OMLApplication.Current;

            OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_LanguageSettings", properties);
        }

        void imageMountingCmd_Invoked(object sender, EventArgs e)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();

            Library.Code.V3.ImageMountingSettings page = new Library.Code.V3.ImageMountingSettings();
            properties["Page"] = page;
            properties["Application"] = OMLApplication.Current;

            OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_ImageMountingSettings", properties);

        }

        void optimizationCmd_Invoked(object sender, EventArgs e)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();

            Library.Code.V3.OptimizationSettings page = new Library.Code.V3.OptimizationSettings();
            properties["Page"] = page;
            properties["Application"] = OMLApplication.Current;

            OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_OptimizationSettings", properties);

        }

        void mediaChangersCmd_Invoked(object sender, EventArgs e)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();

            Library.Code.V3.MediaChangerManagerPage page = new Library.Code.V3.MediaChangerManagerPage();
            properties["Page"] = page;
            properties["Application"] = OMLApplication.Current;

            OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_MediaChangerManagerSettings", properties);

        }

        void generalCmd_Invoked(object sender, EventArgs e)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();

            Library.Code.V3.GeneralSettings page = new Library.Code.V3.GeneralSettings();
            properties["Page"] = page;
            properties["Application"] = OMLApplication.Current;

            OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_GeneralSettings", properties);
        }


        void favoritesCmd_Invoked(object sender, EventArgs e)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();

            Library.Code.V3.FavoritesSettings page = new Library.Code.V3.FavoritesSettings();
            properties["Page"] = page;
            properties["Application"] = OMLApplication.Current;

            OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_FavoritesSettings", properties);
        }

        void filtersCmd_Invoked(object sender, EventArgs e)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();

            Library.Code.V3.FilterSettings page = new Library.Code.V3.FilterSettings();
            properties["Page"] = page;
            properties["Application"] = OMLApplication.Current;

            OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_Settings_FilterSettings", properties);
        }
        /// <summary>
        /// The primary title of the object.
        /// </summary>
        public string Title
        {
            get { return title; }
            set
            {
                if (title != value)
                {
                    title = value;
                    FirePropertyChanged("Title");
                }
            }
        }

        /// <summary>
        /// A list of actions that can be performed on this object.
        /// This list should only contain objects of type Command.
        /// </summary>
        public IList Commands
        {
            get { return commands; }
            set
            {
                if (commands != value)
                {
                    commands = value;
                    FirePropertyChanged("Commands");
                }
            }
        }

        private string title;
        private IList commands;

    }
}
