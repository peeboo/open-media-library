using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MediaCenter.UI;
using Microsoft.MediaCenter;
using OMLEngine.Settings;
using System.Collections;
using Microsoft.MediaCenter.Hosting;

namespace Library.Code.V3
{
    public class StartMenuSettings : ModelItem
    {
        private StartMenuHelper helper;
        public StartMenuSettings()
        {
            this.commands = new ArrayListDataSet(this);

            //save command
            Command saveCmd = new Command();
            saveCmd.Description = "New Shortcut";
            saveCmd.Invoked += new EventHandler(newCmd_Invoked);
            this.commands.Add(saveCmd);

            //cancel command
            Command cancelCmd = new Command();
            cancelCmd.Description = "Done";
            cancelCmd.Invoked += new EventHandler(cancelCmd_Invoked);
            this.commands.Add(cancelCmd);

            //this.SetupStartMenu();
            this.helper = new StartMenuHelper();
            this.helper.Changed += new ChangedEventHandler(helper_Changed);
            this.LoadArray();
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

        private ArrayListDataSet startMenuArray;
        public ArrayListDataSet StartMenuArray
        {
            get { return startMenuArray; }
            set
            {
                if (startMenuArray != value)
                {
                    startMenuArray = value;
                    FirePropertyChanged("StartMenuArray");
                }
            }
        }

        void cancelCmd_Invoked(object sender, EventArgs e)
        {
            OMLApplication.Current.Session.BackPage();
        }

        void newCmd_Invoked(object sender, EventArgs e)
        {
            if (this.helper.StartMenuItems.Count < 5)
            {
                StartMenuItem newEntryPoint = new StartMenuItem();
                newEntryPoint.Context = Context.Home;

                //navigate to edit page
                Dictionary<string, object> properties = new Dictionary<string, object>();

                Library.Code.V3.StartMenuItemSettings page = new Library.Code.V3.StartMenuItemSettings(newEntryPoint, helper);
                properties["Page"] = page;
                properties["Application"] = OMLApplication.Current;

                OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_StartMenuItemSettings", properties);
            }
            else
            {
                DialogResult res = OMLApplication.Current.MediaCenterEnvironment.Dialog("You can have only five entries on the start menu?", "START MENU FULL", DialogButtons.Ok, -1, true);
            }
        }

        //private void SetupStartMenu()
        //{
        //    this.helper = new StartMenuHelper();
        //    this.helper.Changed += new ChangedEventHandler(helper_Changed);
        //    this.LoadArray();
        //}

        private void LoadArray()
        {
             this.startMenuArray = new ArrayListDataSet();
            foreach (StartMenuItem entryPoint in helper.StartMenuItems)
            {
                StartMenuCommand cmd = new StartMenuCommand(this, this.helper, entryPoint);
                cmd.Description = entryPoint.Title;
                cmd.Invoked += delegate(object Sender, EventArgs Args)
                {
                    if (Sender is StartMenuCommand)
                    {
                        string strDelete = string.Format("Are you sure you want to delete {0}?", ((StartMenuCommand)Sender).Name);
                        DialogResult res = OMLApplication.Current.MediaCenterEnvironment.Dialog(strDelete, "DELETE ITEM", DialogButtons.Yes | DialogButtons.No, -1, true);
                        if (res == DialogResult.Yes)
                        {
                            this.helper.DeleteStartMenuItem(((StartMenuCommand)Sender).EntryPoint);
                        }
                    }
                };
                this.startMenuArray.Add(cmd);
                Guid entryPointGuid = new Guid(entryPoint.ItemId);
                bool isregistered = AddInHost.Current.ApplicationContext.IsEntryPointRegistered(entryPointGuid);
            }
            FirePropertyChanged("StartMenuArray");
        }

        void helper_Changed(object sender, EventArgs e)
        {
            this.LoadArray();
        }

        public static bool DeleteFavorite()
        {
            return true;
        }
    }

    public enum EntryPointType
    {
        AllTitles,
        Movies,
        TV,
        Trailers,
        Favorite
    }
    public class StartMenuCommand : Command
    {
        private Command moveUp;
        public Command MoveUp
        {
            get
            {
                return this.moveUp;
            }
            set
            {
                this.moveUp = value;
            }
        }
        private Command moveDown;
        public Command MoveDown
        {
            get
            {
                return this.moveDown;
            }
            set
            {
                this.moveDown = value;
            }
        }
        private Command edit;
        public Command Edit
        {
            get
            {
                return this.edit;
            }
            set
            {
                this.edit = value;
            }
        }
        public StartMenuCommand(IModelItem Owner, StartMenuHelper helper, StartMenuItem Item)
            : base(Owner)
        {
            this.entryPoint = Item;
            this.edit = new Command(this);

            //this.edit.Invoked += new EventHandler(edit_Invoked);
            this.edit.Invoked += delegate(object Sender, EventArgs Args)
                {
                    //navigate to edit page
                    Dictionary<string, object> properties = new Dictionary<string, object>();

                    Library.Code.V3.StartMenuItemSettings page = new Library.Code.V3.StartMenuItemSettings(entryPoint, helper);
                    properties["Page"] = page;
                    properties["Application"] = OMLApplication.Current;

                    OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_StartMenuItemSettings", properties);
                };

            this.moveUp = new Command(this);
            this.moveUp.Invoked += delegate(object Sender, EventArgs Args)
            {
                helper.MoveItemUp(this.entryPoint);
            };
            this.moveDown = new Command(this);
            this.moveDown.Invoked += delegate(object Sender, EventArgs Args)
            {
                helper.MoveItemDown(this.entryPoint);
            };
        }

        private StartMenuItem entryPoint;
        public StartMenuItem EntryPoint
        {
            get { return this.entryPoint; }
            set
            {
                if (this.entryPoint != value)
                {
                    this.entryPoint = value;
                    FirePropertyChanged("EntryPoint");
                }
            }
        }

        public string Name
        {
            get
            {
                return this.EntryPoint.Title;
            }
        }

        private Image defaultImage = null;
        public Image DefaultImage
        {
            get
            {
                return this.defaultImage;
            }
            set
            {
                if (this.defaultImage != value)
                {
                    this.defaultImage = value;
                    base.FirePropertyChanged("DefaultImage");
                }
            }
        }
    }
}
