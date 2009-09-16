using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MediaCenter.UI;
using Microsoft.MediaCenter;
using OMLEngine.Settings;
using System.Collections;
using System.Threading;

namespace Library.Code.V3
{
    public class TrailersSettings : ModelItem
    {
        public TrailersSettings()
        {
            this.commands = new ArrayListDataSet(this);

            //save command
            Command saveCmd = new Command();
            saveCmd.Description = "Save";
            saveCmd.Invoked += new EventHandler(saveCmd_Invoked);
            this.commands.Add(saveCmd);

            //cancel command
            Command cancelCmd = new Command();
            cancelCmd.Description = "Cancel";
            cancelCmd.Invoked += new EventHandler(cancelCmd_Invoked);
            this.commands.Add(cancelCmd);

            this.trailerResolutions = new Choice(this);
            this.SetupTrailers();
        }

        private void SetupTrailers()
        {
            List<string> trailerFormats = new List<string>();
            trailerFormats.Add("HD");
            trailerFormats.Add("Standard");
            trailerResolutions.Options = trailerFormats;
            trailerResolutions.Chosen = DisplayFromTrailerFormat(OMLSettings.TrailersDefinition);
            this.selectedTrailerResolution = DisplayFromTrailerFormat(OMLSettings.TrailersDefinition);
        }

        private string DisplayFromTrailerFormat(string format)
        {
            if (format == "Hi")
                return "HD";
            else
                return "Standard";
        }
        private string selectedTrailerResolution;
        private Choice trailerResolutions;
        public Choice TrailerResolutions
        {
            get { return trailerResolutions; }
            set { trailerResolutions = value; }
        }

        private Boolean isBusy = false;
        public Boolean IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                FirePropertyChanged("IsBusy");
            }
        }

        /// <summary>
        /// detecting changes
        /// </summary>
        /// <returns></returns>
        public bool IsDirty()
        {
            if ((string)this.trailerResolutions.Chosen != this.selectedTrailerResolution)
                return true;
            return false;
        }

        /// <summary>
        /// saving settings
        /// </summary>
        public void Save()
        {
            if ((string)this.trailerResolutions.Chosen == "Hi")
                OMLSettings.TrailersDefinition = "Hi";
            else
                OMLSettings.TrailersDefinition = "Std";
        }

        /// <summary>
        /// Detects if the settings are dirty 
        /// and prompts the user to save if they have not already
        /// </summary>
        public void ConfirmSave()
        {
            if (this.IsDirty())
            {
                DialogResult res = OMLApplication.Current.MediaCenterEnvironment.Dialog("Do you want to save the changes that you have made to these settings?", "SAVE CHANGES", DialogButtons.Yes | DialogButtons.No, -1, true);
                if (res == DialogResult.Yes)
                {
                    //save!
                    this.Save();
                }
            }
            OMLApplication.Current.Session.BackPage();

        }

        /// <summary>
        /// A list of actions that can be performed on this object.
        /// This list should only contain objects of type Command.
        /// </summary>
        private IList commands;
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

        void cancelCmd_Invoked(object sender, EventArgs e)
        {
            OMLApplication.Current.Session.BackPage();
        }

        void saveCmd_Invoked(object sender, EventArgs e)
        {
            this.Save();
            OMLApplication.Current.Session.BackPage();
        }
    }
}
