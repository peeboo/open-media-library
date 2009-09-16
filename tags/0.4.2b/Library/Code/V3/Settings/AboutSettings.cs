using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MediaCenter.UI;
using Microsoft.MediaCenter;
using OMLEngine.Settings;
using System.Collections;
using System.Threading;
using System.Xml;
using System.Reflection;
using System.IO;
using System.Xml.XPath;

namespace Library.Code.V3
{
    public class AboutSettings : ModelItem
    {
        public Boolean ShowAbout
        {
            get
            {
                if (this.radioCommands.ChosenIndex == 0)
                    return true;
                else
                    return false;
            }
        }

        public Boolean ShowCredits
        {
            get
            {
                if (this.radioCommands.ChosenIndex == 0)
                    return false;
                else
                    return true;
            }
        }

        public string AboutText { get; set; }       
        private string aboutTitle="SOFTWARE VERSION";

        public string CreditsText { get; set; }
        private string creditsTitle="CREDITS";

        public string ActiveTitle
        {
            get
            {
                if (this.radioCommands.ChosenIndex == 0)
                    return aboutTitle;
                else
                    return creditsTitle;
            }
        }

        public AboutSettings()
        {
            

            System.Resources.ResourceManager RM = new System.Resources.ResourceManager("Library.Resources",System.Reflection.Assembly.GetExecutingAssembly());
            string creditsString = (string)RM.GetObject("Credits");
            byte[] byteArray = Encoding.ASCII.GetBytes(creditsString);
            MemoryStream stream = new MemoryStream(byteArray);
            //XmlTextReader reader = new XmlTextReader(stream);
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(stream);
            XPathNavigator nav = xDoc.CreateNavigator();
            XPathNodeIterator it = nav.Select("Credits/Developers/Person");

            this.CreditsText = "DEVELOPMENT TEAM:";
            while (it.MoveNext())
            {
                this.CreditsText += "\n" + it.Current.Value;
            }
            this.CreditsText += "\n\n";
            it = nav.Select("Credits/Contributors/Person");

            this.CreditsText += "COMPANIES AND INDIVIDUALS:";
            while (it.MoveNext())
            {
                this.CreditsText += "\n" + it.Current.Value;
            }
            this.CreditsText += "\n\n";
            it = nav.Select("Credits/Special/Person");

            this.CreditsText += "SPECIAL THANKS:";
            while (it.MoveNext())
            {
                this.CreditsText += "\n" + it.Current.Value;
            }

            string version=Assembly.GetExecutingAssembly().GetName().Version.ToString();

            this.AboutText = "Open Media Library (replace with image)\n\nVersion: " + version + " (Revision: " + OMLApplication.Current.RevisionNumber+")";
            this.AboutText += "\nCopyright © 2008, GNU General Public License v3";

            this.radioCommands = new Choice(this);
            ArrayListDataSet radioSet = new ArrayListDataSet();

            Command aboutCmd=new Command(this);
            aboutCmd.Description="About";
            radioSet.Add(aboutCmd);

            Command creditsCmd = new Command(this);
            creditsCmd.Description = "Credits";
            radioSet.Add(creditsCmd);

            //this.CreditsText = "this is a credit";

            this.radioCommands.Options = radioSet;
            this.radioCommands.ChosenChanged += new EventHandler(radioCommands_ChosenChanged);
           
        }

        void radioCommands_ChosenChanged(object sender, EventArgs e)
        {
            FirePropertyChanged("ActiveTitle");
            FirePropertyChanged("ShowAbout");
            FirePropertyChanged("ShowCredits");
            
            //FirePropertyChanged("ActiveContent");
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
