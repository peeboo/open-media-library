using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MediaCenter.UI;
using Microsoft.MediaCenter;
using OMLEngine.Settings;
using System.Collections;

namespace Library.Code.V3
{
    public class BackgroundSettings : ModelItem
    {
        private BooleanChoice enableCustomBackground;
        public BooleanChoice EnableCustomBackground
        {
            get
            {
                return this.enableCustomBackground;
            }
            set
            {
                this.enableCustomBackground = value;
                FirePropertyChanged("EnableCustomBackground");
            }
        }

        public BackgroundSettings()
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

            this.transparancyOptions = new Choice(this);
            this.SetupTransparancyOptions();
            this.transparancyOptions.ChosenChanged += new EventHandler(transparancyOptions_ChosenChanged);

            this.rotationOptions = new Choice(this);
            this.SetupRotationOptions();
            

            this.enableCustomBackground = new BooleanChoice(this);
            this.enableCustomBackground.Description = "Enable Custom Background";
            this.enableCustomBackground.Value = Properties.Settings.Default.EnableMainPageBackDrop;
        }

        private float selectedTransparancy = 1;
        public float SelectedTransparancy
        {
            get { return selectedTransparancy; }
            set
            {
                if (this.selectedTransparancy != value)
                {
                    this.selectedTransparancy = value;
                    FirePropertyChanged("SelectedTransparancy");
                }
            }
        }

        void transparancyOptions_ChosenChanged(object sender, EventArgs e)
        {
            //decimal d = Convert.ToDecimal(((string)this.transparancyOptions.Chosen).Replace("%", "")) / 100;
            //this.SelectedTransparancy = Decimal.ToSingle(d);
            //this.SelectedTransparancy = .5;
            this.SetTransparancy();
        }

        private void SetTransparancy()
        {
            decimal d = Convert.ToDecimal(((string)this.transparancyOptions.Chosen).Replace("%", "")) / 100;
            this.SelectedTransparancy = Decimal.ToSingle(d);
        }

        private void SetupTransparancyOptions()
        {
            List<string> delays = new List<string>();

            for (int i = 1; i < 101; i++)
            {
                delays.Add(string.Format("{0}%", i.ToString()));
            }

            this.transparancyOptions.Options = delays;
            this.transparancyOptions.Chosen = string.Format("{0}%", (Properties.Settings.Default.MainPageBackDropAlpha * 100).ToString());
            this.SetTransparancy();
        }

        private void SetupRotationOptions()
        {
            List<string> delays = new List<string>();

            int iterationcount = 5;
            for (int i = 0; i < 10; i++)
            {
                delays.Add(string.Format("{0} seconds", iterationcount.ToString()));
                iterationcount = iterationcount + 5;//delay by 5 sec
            }

            rotationOptions.Options = delays;
            rotationOptions.Chosen = string.Format("{0} seconds", Properties.Settings.Default.MainPageBackDropRotationInSeconds.ToString());
            //this.selectedTranscodingDelay = string.Format("{0} seconds", OMLSettings.TranscodeBufferDelay.ToString());
        }

        private Choice transparancyOptions;
        public Choice TransparancyOptions
        {
            get { return transparancyOptions; }
            set { transparancyOptions = value; }
        }

        private Choice rotationOptions;
        public Choice RotationOptions
        {
            get { return rotationOptions; }
            set { rotationOptions = value; }
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
            if (this.SelectedTransparancy != (Properties.Settings.Default.MainPageBackDropAlpha))
                return true;
            if (this.enableCustomBackground.Value != Properties.Settings.Default.EnableMainPageBackDrop)
                return true;
            if (Convert.ToInt32(rotationOptions.Chosen.ToString().Replace(" seconds", "")) != Properties.Settings.Default.MainPageBackDropRotationInSeconds)
                return true;

            return false;
        }

        /// <summary>
        /// saving settings
        /// </summary>
        public void Save()
        {
            Properties.Settings.Default.EnableMainPageBackDrop = this.EnableCustomBackground.Value;
            Properties.Settings.Default.MainPageBackDropAlpha = this.SelectedTransparancy;
            Properties.Settings.Default.MainPageBackDropRotationInSeconds = Convert.ToInt32(rotationOptions.Chosen.ToString().Replace(" seconds", ""));
            Properties.Settings.Default.Save();
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
