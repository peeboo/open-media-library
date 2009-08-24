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
    public class AboutSettings : ModelItem
    {
        public AboutSettings()
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

            this.languageOptions = new Choice(this);
            this.SetupUILanguage();
        }

        private void SetupUILanguage()
        {
            string selected = null;
            List<string> list = new List<string>();
            string configuredLangId = OMLSettings.UILanguage;

            foreach (var availableCulture in I18n.AvailableCultures)
            {
                string name = availableCulture.TextInfo.ToTitleCase(availableCulture.NativeName);
                if (string.CompareOrdinal(availableCulture.Name, configuredLangId) == 0)
                {
                    selected = name;
                }
                list.Add(name);
            }

            list.Sort((a, b) => string.Compare(a, b, true, Thread.CurrentThread.CurrentCulture));

            list.Insert(0, "Use system language");
            if (string.IsNullOrEmpty(selected)) selected = list[0];

            languageOptions.Options = list;
            languageOptions.Chosen = selected;
            this.selectedLanguage = selected;
        }

        private string selectedLanguage;
        private Choice languageOptions;
        public Choice LanguageOptions
        {
            get { return languageOptions; }
            set { languageOptions = value; }
        }

        private static String CultureIdFromDisplayName(string displayName)
        {
            foreach (var availableCulture in I18n.AvailableCultures)
            {
                if (string.CompareOrdinal(availableCulture.TextInfo.ToTitleCase(availableCulture.NativeName), displayName) == 0)
                {
                    return availableCulture.Name;
                }
            }
            return null;
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
            if ((string)this.languageOptions.Chosen != this.selectedLanguage)
                return true;
            return false;
        }

        /// <summary>
        /// saving settings
        /// </summary>
        public void Save()
        {
            OMLSettings.UILanguage = CultureIdFromDisplayName(this.languageOptions.Chosen as string);
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

        private Choice radioCommands;
        public Choice RadioCommands
        {
            get
            {
                return this.radioCommands;
            }
            set
            {
                this.radioCommands = value;
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
