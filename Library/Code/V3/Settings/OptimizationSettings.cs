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
    public class OptimizationSettings : ModelItem
    {
        private EditableText optimizationHour;
        public EditableText OptimizationHour
        {
            get { return this.optimizationHour; }
            set { this.optimizationHour = value; }
        }

        private EditableText optimizationMinute;
        public EditableText OptimizationMinute
        {
            get { return this.optimizationMinute; }
            set { this.optimizationMinute = value; }
        }

        private BooleanChoice enableOptimization;
        public BooleanChoice EnableOptimization
        {
            get
            {
                return this.enableOptimization;
            }
            set
            {
                this.enableOptimization = value;
                FirePropertyChanged("EnableOptimization");
            }
        }

        private Command optimizeNow;
        public Command OptimizeNow
        {
            get { return this.optimizeNow; }
            set { this.optimizeNow = value; }
        }

        public OptimizationSettings()
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

            this.aMPM = new Choice(this);
            List<string> ampmlist = new List<string>();
            ampmlist.Add("AM");
            ampmlist.Add("PM");
            this.aMPM.Options = ampmlist;


            this.enableOptimization = new BooleanChoice(this);
            this.enableOptimization.Description = "Perform Optimization";
            this.enableOptimization.Value = true;
            this.optimizeNow = new Command();
            this.optimizeNow.Description = "Optimize Now";

            this.optimizationHour = new EditableText(this);
            this.optimizationHour.Value = "4";
            this.optimizationMinute = new EditableText(this);
            this.optimizationMinute.Value = "00";
        }

        private Choice aMPM;
        public Choice AMPM
        {
            get { return aMPM; }
            set { aMPM = value; }
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
