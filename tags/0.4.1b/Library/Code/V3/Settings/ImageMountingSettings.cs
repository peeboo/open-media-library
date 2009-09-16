using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MediaCenter.UI;
using Microsoft.MediaCenter;
using OMLEngine.Settings;
using System.Collections;
using OMLEngine.FileSystem;
using System.Management;

namespace Library.Code.V3
{
    public class ImageMountingSettings : ModelItem
    {
        private bool useAdvancedSettings;
        private bool enableSave=true;
        private string descriptiveText;
        public string DescriptiveText
        {
            get { return this.descriptiveText; }
            set
            {
                if (this.descriptiveText != value)
                {
                    this.descriptiveText = value;
                    FirePropertyChanged("DescriptiveText");
                }
            }
        }

        private BooleanChoice mountingToolChoiceEnabled;
        public BooleanChoice MountingToolChoiceEnabled
        {
            get { return this.mountingToolChoiceEnabled; }
            set {
                if (this.mountingToolChoice != value)
                {
                    this.mountingToolChoiceEnabled = value;
                    FirePropertyChanged("MountingToolChoiceEnabled");
                }
            }
        }
        private Boolean enableDaemonTools;
        public Boolean EnableDaemonTools
        {
            get { return this.enableDaemonTools; }
            set { this.enableDaemonTools = value; }
        }
        private Boolean enableVirtualCloneDrive;
        public Boolean EnableVirtualCloneDrive
        {
            get { return this.enableVirtualCloneDrive; }
            set { this.enableVirtualCloneDrive = value; }
        }

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
            this.descriptiveText = "If your library includes DVD images OML can automatically mount them with either DAEMON Tools Lite or SlySoft Virtual CloneDrive.";
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

            //advanced command
            Command advancedCmd = new Command();
            advancedCmd.Description = "Advanced";
            advancedCmd.Invoked += new EventHandler(advancedCmd_Invoked);
            this.commands.Add(advancedCmd);


            this.mountingToolChoiceEnabled = new BooleanChoice(this);
            this.useAdvancedSettings = OMLSettings.MountingToolUseAdvanced;
            

                this.mountingToolChoice = new Choice(this);
                List<string> toolList = new List<string>();

                if (!String.IsNullOrEmpty(MountingTool.GetMountingToolPath(MountingTool.Tool.DaemonTools)))
                {
                    toolList.Add("DAEMON Tools Lite");
                    this.enableDaemonTools = false;
                }
                else
                {
                    //toolList.Add("DAEMON Tools Lite (Not Installed)");
                    this.enableDaemonTools = true;
                }

                if (!String.IsNullOrEmpty(MountingTool.GetMountingToolPath(MountingTool.Tool.VirtualCloneDrive)))
                {
                    toolList.Add("Virtual CloneDrive");
                    this.enableVirtualCloneDrive = false;
                }
                else
                {
                    //toolList.Add("Virtual CloneDrive (Not Installed)");
                    this.enableVirtualCloneDrive = true;
                }

                if (toolList.Count == 0)
                    toolList.Add("None");
                this.mountingToolChoice.Options = toolList;

                this.virtualDriveOptions = new Choice(this);
                this.virtualDriveOptions.Options = MountingTool.GetAvailableDriveLetters();

                string chosenDrive = string.Format(@"{0}:\", OMLSettings.VirtualDiscDrive);
                if (this.virtualDriveOptions.Options.IndexOf(chosenDrive) > -1)
                    this.virtualDriveOptions.Chosen = chosenDrive;
                else
                    this.virtualDriveOptions.ChosenIndex = this.virtualDriveOptions.Options.IndexOf(MountingTool.GetFirstFreeDriveLetter());

                MountingTool.Tool selectedTool = (MountingTool.Tool)OMLSettings.MountingToolSelection;


                this.enableImageMounting = new BooleanChoice(this);
                this.enableImageMounting.Description = "Enable Image Mounting";
                if (selectedTool != MountingTool.Tool.None)
                    this.enableImageMounting.Value = true;
                else
                    this.enableImageMounting.Value = false;

                if (this.mountingToolChoice.Options[0].ToString() != "None")
                {
                    this.mountingToolChoiceEnabled.Value = true;
                    switch (selectedTool)
                    {
                        case MountingTool.Tool.DaemonTools:
                            this.mountingToolChoice.ChosenIndex = 0;
                            break;
                        case MountingTool.Tool.VirtualCloneDrive:
                            if (this.mountingToolChoice.Options.Count > 1)
                                this.mountingToolChoice.ChosenIndex = 1;
                            else
                                this.mountingToolChoice.ChosenIndex = 0;
                            break;
                    }
                }
                else
                {
                    this.mountingToolChoiceEnabled.Value = false;//nothing detected
                    this.enableSave = false;
                    if (this.useAdvancedSettings == true)
                        this.descriptiveText = "OML is currently configured to use advanced options for image mounting. Use \"Advanced\" to modify these settings.";
                    else
                        this.descriptiveText = "OML was unable to automatically detect DAEMON Tools Lite or SlySoft Virtual CloneDrive. One of these tools is required for OML to automatically mount DVD images. \n\nPlease refer to the manual for installation instructions or choose \"Advanced\" if you would like to configure image mounting manually.";
                }
        }

        void advancedCmd_Invoked(object sender, EventArgs e)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();

            Library.Code.V3.ImageMountingSettingsAdvanced page = new Library.Code.V3.ImageMountingSettingsAdvanced();
            properties["Page"] = page;
            properties["Application"] = OMLApplication.Current;

            OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_ImageMountingSettingsAdvanced", properties);
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
            if (this.enableSave == true)
            {
                if (OMLSettings.VirtualDiscDrive != virtualDriveOptions.Chosen.ToString().Replace(@":\", ""))
                    return true;
                MountingTool.Tool tool = MountingTool.Tool.None;

                switch (this.mountingToolChoice.Chosen.ToString())
                {
                    case "DAEMON Tools Lite":
                        tool = MountingTool.Tool.DaemonTools;
                        break;
                    case "Virtual CloneDrive":
                        tool = MountingTool.Tool.VirtualCloneDrive;
                        break;
                    default:
                        tool = MountingTool.Tool.None;
                        break;
                }
                if (OMLSettings.MountingToolSelection != tool)
                    return true;

                if (OMLSettings.MountingToolPath != MountingTool.GetMountingToolPath(tool))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// saving settings
        /// </summary>
        public void Save()
        {
            if (this.enableSave == true)
            {
                MountingTool.Tool tool = MountingTool.Tool.None;

                switch (this.mountingToolChoice.Chosen.ToString())
                {
                    case "DAEMON Tools Lite":
                        tool = MountingTool.Tool.DaemonTools;
                        break;
                    case "Virtual CloneDrive":
                        tool = MountingTool.Tool.VirtualCloneDrive;
                        break;
                    default:
                        tool = MountingTool.Tool.None;
                        break;
                }
                if (this.EnableImageMounting.Value == false)
                    tool = MountingTool.Tool.None;//set it to none if we have disabled it

                OMLSettings.MountingToolPath = MountingTool.GetMountingToolPath(tool);
                OMLSettings.MountingToolSelection = tool;

                OMLSettings.VirtualDiscDrive = virtualDriveOptions.Chosen.ToString().Replace(@":\", "");
            }
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

    public class ImageMountingSettingsAdvanced : ModelItem
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

        public ImageMountingSettingsAdvanced()
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

            this.mountingToolPath = new EditableText(this);
            this.mountingToolPath.Value = OMLSettings.MountingToolPath;
            this.mountingToolChoice = new Choice(this);
            List<string> toolList = new List<string>();

                toolList.Add("DAEMON Tools Lite");
                toolList.Add("Virtual CloneDrive");

            this.mountingToolChoice.Options = toolList;

            this.virtualDriveOptions = new Choice(this);
            this.virtualDriveOptions.Options = MountingTool.GetAvailableDriveLetters();

            string chosenDrive = string.Format(@"{0}:\", OMLSettings.VirtualDiscDrive);
            if (this.virtualDriveOptions.Options.IndexOf(chosenDrive) > -1)
                this.virtualDriveOptions.Chosen = chosenDrive;
            else
                this.virtualDriveOptions.ChosenIndex = this.virtualDriveOptions.Options.IndexOf(MountingTool.GetFirstFreeDriveLetter());

            MountingTool.Tool selectedTool = (MountingTool.Tool)OMLSettings.MountingToolSelection;


            this.enableImageMounting = new BooleanChoice(this);
            this.enableImageMounting.Description = "Manually configure image mounting";
            if (selectedTool != MountingTool.Tool.None)
                this.enableImageMounting.Value = true;
            else
                this.enableImageMounting.Value = false;

            switch (selectedTool)
            {
                case MountingTool.Tool.DaemonTools:
                    this.mountingToolChoice.ChosenIndex = 0;
                    break;
                case MountingTool.Tool.VirtualCloneDrive:
                    this.mountingToolChoice.ChosenIndex = 1;
                    break;
            }
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
            if (this.EnableImageMounting.Value !=OMLSettings.MountingToolUseAdvanced)

            if (OMLSettings.VirtualDiscDrive != virtualDriveOptions.Chosen.ToString().Replace(@":\", ""))
                return true;
            MountingTool.Tool tool = MountingTool.Tool.None;

            switch (this.mountingToolChoice.ChosenIndex)
            {
                case 0:
                    tool = MountingTool.Tool.DaemonTools;
                    break;
                case 1:
                    tool = MountingTool.Tool.VirtualCloneDrive;
                    break;
                default:
                    tool = MountingTool.Tool.None;
                    break;
            }
            
            if (OMLSettings.MountingToolSelection != tool)
                return true;
            if (OMLSettings.MountingToolPath != this.mountingToolPath.Value)
                return true;

            return false;
        }

        /// <summary>
        /// saving settings
        /// </summary>
        public void Save()
        {
            OMLSettings.MountingToolUseAdvanced = this.EnableImageMounting.Value;
            if (this.EnableImageMounting.Value)
            {
                MountingTool.Tool tool = MountingTool.Tool.None;

                switch (this.mountingToolChoice.ChosenIndex)
                {
                    case 0:
                        tool = MountingTool.Tool.DaemonTools;
                        break;
                    case 1:
                        tool = MountingTool.Tool.VirtualCloneDrive;
                        break;
                    default:
                        tool = MountingTool.Tool.None;
                        break;
                }

                OMLSettings.MountingToolPath = MountingTool.GetMountingToolPath(tool);
                OMLSettings.MountingToolSelection = tool;

                OMLSettings.VirtualDiscDrive = virtualDriveOptions.Chosen.ToString().Replace(@":\", "");
            }
            else
            {
                OMLSettings.MountingToolSelection = MountingTool.Tool.None;
                OMLSettings.MountingToolPath = string.Empty;
                OMLSettings.VirtualDiscDrive = string.Empty;
            }
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
