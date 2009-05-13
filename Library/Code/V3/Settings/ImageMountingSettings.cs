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
    public class ImageMountingSettings : ModelItem
    {
        private EditableText mountingToolPath;
        public EditableText MountingToolPath
        {
            get { return this.mountingToolPath; }
            set { this.mountingToolPath = value; }
        }

        private EditableText optimizationMinute;
        public EditableText OptimizationMinute
        {
            get { return this.optimizationMinute; }
            set { this.optimizationMinute = value; }
        }

        private BooleanChoice enableImageMounting;
        public BooleanChoice EnableImageMounting
        {
            get
            {
                return this.enableImageMounting;
            }
            set
            {
                this.enableImageMounting = value;
                FirePropertyChanged("EnableImageMounting");
            }
        }

        private Command scanNow;
        public Command ScanNow
        {
            get { return this.scanNow; }
            set { this.scanNow = value; }
        }

        public ImageMountingSettings()
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

            this.mountingToolChoice = new Choice(this);
            List<string> toolList = new List<string>();
            toolList.Add("DAEMON Tools");
            toolList.Add("Virtual CloneDrive");
            this.mountingToolChoice.Options = toolList;

            this.driveOptions = new Choice(this);
            List<string> driveList = new List<string>();
            //tmp
            driveList.Add(@"C:\");
            driveList.Add(@"H:\");
            driveList.Add(@"I:\");
            driveList.Add(@"M:\");
            this.driveOptions.Options = driveList;

            this.virtualDriveOptions = new Choice(this);
            List<string> virtualDriveList = new List<string>();
            //tmp
            virtualDriveList.Add(@"N:\");
            virtualDriveList.Add(@"O:\");
            virtualDriveList.Add(@"P:\");
            virtualDriveList.Add(@"Q:\");
            this.virtualDriveOptions.Options = virtualDriveList;

            this.enableImageMounting = new BooleanChoice(this);
            this.enableImageMounting.Description = "Enable Image Mounting";
            this.enableImageMounting.Value = true;
            this.scanNow = new Command();
            this.scanNow.Description = "Begin Scan";

            this.mountingToolPath = new EditableText(this);
            //this.mountingToolPath.Value = "4";
        }

        private Choice driveOptions;
        public Choice DriveOptions
        {
            get { return driveOptions; }
            set { driveOptions = value; }
        }

        private Choice virtualDriveOptions;
        public Choice VirtualDriveOptions
        {
            get { return virtualDriveOptions; }
            set { virtualDriveOptions = value; }
        }

        private Choice mountingToolChoice;
        public Choice MountingToolChoice
        {
            get { return mountingToolChoice; }
            set { mountingToolChoice = value; }
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
