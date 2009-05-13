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
    public class GallerySettings : ModelItem
    {
        private BooleanChoice enableAlphaJump;
        public BooleanChoice EnableAlphaJump
        {
            get
            {
                return this.enableAlphaJump;
            }
            set
            {
                this.enableAlphaJump = value;
                FirePropertyChanged("EnableAlphaJump");
            }
        }

        private BooleanChoice enableTwoRow;
        public BooleanChoice EnableTwoRow
        {
            get
            {
                return this.enableTwoRow;
            }
            set
            {
                this.enableTwoRow = value;
                FirePropertyChanged("EnableTwoRow");
            }
        }

        private BooleanChoice enableThreeRow;
        public BooleanChoice EnableThreeRow
        {
            get
            {
                return this.enableThreeRow;
            }
            set
            {
                this.enableThreeRow = value;
                FirePropertyChanged("EnableThreeRow");
            }
        }

        public GallerySettings()
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

            this.threeRowOptions = new Choice(this);
            this.SetupThreeRowOptions();

            this.twoRowOptions = new Choice(this);
            this.SetupTwoRowOptions();

            this.EnableAlphaJump = new BooleanChoice(this);
            this.EnableAlphaJump.Description = "Use Alpha Jump";
            this.EnableAlphaJump.Value = Properties.Settings.Default.GalleryEnableAlphaJump;

            this.EnableThreeRow = new BooleanChoice(this);
            this.EnableThreeRow.Description = "Show three rows";
            this.EnableThreeRow.Value = Properties.Settings.Default.GalleryEnableThreeRow;

            this.EnableTwoRow = new BooleanChoice(this);
            this.EnableTwoRow.Description = "Show two rows";
            this.EnableTwoRow.Value = Properties.Settings.Default.GalleryEnableTwoRow;

            this.selectedViewOptions = new Choice(this);
            List<string> selectedViewList = new List<string>();
            //tmp
            selectedViewList.Add(@"List view");
            selectedViewList.Add(@"Poster view");
            //selectedViewList.Add(@"Show titles in a list");
            //selectedViewList.Add(@"Show titles as posters");
            this.selectedViewOptions.Options = selectedViewList;
            if(Properties.Settings.Default.GallerySelectedView=="list")
                this.selectedViewOptions.ChosenIndex = 0;
            else
                this.selectedViewOptions.ChosenIndex = 1;
            this.enablePosterOptions = new BooleanChoice(this);
            this.enablePosterOptions.Value = false;
            if (this.selectedViewOptions.ChosenIndex == 1)
                this.enablePosterOptions.Value = true;
            this.selectedViewOptions.ChosenChanged += new EventHandler(selectedViewOptions_ChosenChanged);
        }

        private BooleanChoice enablePosterOptions;
        public BooleanChoice EnablePosterOptions
        {
            get { return this.enablePosterOptions; }
            set
            {
                if (this.enablePosterOptions != value)
                {
                    this.enablePosterOptions = value;
                    FirePropertyChanged("EnablePosterOptions");
                }
            }
        }

        void selectedViewOptions_ChosenChanged(object sender, EventArgs e)
        {
            if(this.selectedViewOptions.ChosenIndex==1)
                this.EnablePosterOptions.Value = true;
            else
                this.EnablePosterOptions.Value = false;
        }

        private Choice selectedViewOptions;
        public Choice SelectedViewOptions
        {
            get { return selectedViewOptions; }
            set { selectedViewOptions = value; }
        }
        
        private void SetupTwoRowOptions()
        {
            List<string> options = new List<string>();

            int iterationcount = 5;
            for (int i = 1; i < 101; i++)
            {
                options.Add(string.Format("{0} items", iterationcount.ToString()));
                iterationcount = iterationcount + 5;//delay by 5 sec
            }

            this.twoRowOptions.Options = options;
            this.twoRowOptions.Chosen = string.Format("{0} items", Properties.Settings.Default.GalleryTwoRowMin.ToString());
        }

        private void SetupThreeRowOptions()
        {
            List<string> options = new List<string>();

            int iterationcount = 5;
            for (int i = 1; i < 101; i++)
            {
                options.Add(string.Format("{0} items", iterationcount.ToString()));
                iterationcount = iterationcount + 5;//delay by 5 sec
            }

            this.threeRowOptions.Options = options;
            this.threeRowOptions.Chosen = string.Format("{0} items", Properties.Settings.Default.GalleryThreeRowMin.ToString());
        }

        private Choice twoRowOptions;
        public Choice TwoRowOptions
        {
            get { return twoRowOptions; }
            set { twoRowOptions = value; }
        }

        private Choice threeRowOptions;
        public Choice ThreeRowOptions
        {
            get { return threeRowOptions; }
            set { threeRowOptions = value; }
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
            return false;
        }

        /// <summary>
        /// saving settings
        /// </summary>
        public void Save()
        {
            //Properties.Settings.Default.Save();
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
