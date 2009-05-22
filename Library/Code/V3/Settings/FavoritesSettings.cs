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
    public class FavoritesSettings : ModelItem
    {
        private string emptyContentMessage;
        public string EmptyContentMessage
        {
            get { return emptyContentMessage; }
            set
            {
                emptyContentMessage = value;
                FirePropertyChanged("EmptyContentMessage");
            }
        }

        private Boolean showEmptyContentMessage = false;
        public Boolean ShowEmptyContentMessage
        {
            get { return showEmptyContentMessage; }
            set
            {
                showEmptyContentMessage = value;
                FirePropertyChanged("ShowEmptyContentMessage");
            }
        }

        public FavoritesSettings()
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

            this.SetupFavorites();
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

        private ArrayListDataSet favoritesArray = new ArrayListDataSet();
        public ArrayListDataSet FavoritesArray
        {
            get { return favoritesArray; }
            set
            {
                if (favoritesArray != value)
                {
                    favoritesArray = value;
                    FirePropertyChanged("FavoritesArray");
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

        private void SetupFavorites()
        {
            foreach (UserFilter filt in OMLSettings.UserFilters)
            {
                FavoritesCommand cmd = new FavoritesCommand(this, filt);
                cmd.Description = filt.Name;
                cmd.Invoked += delegate(object favoritesSender, EventArgs favoritesArgs)
                {
                    if (favoritesSender is FavoritesCommand)
                    {
                        //do a delete!
                        //this.IsBusy = true;
                        //Microsoft.MediaCenter.UI.Application.DeferredInvokeOnWorkerThread(beginEject, endEject, (object)discSender);
                    }
                };
                this.favoritesArray.Add(cmd);
            }
        }
    }

    public class FavoritesCommand : Command
    {
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
        public FavoritesCommand(IModelItem Owner, UserFilter Filter)
            : base(Owner)
        {
            this.filter = Filter;
            this.edit = new Command(this);
            this.edit.Invoked += new EventHandler(edit_Invoked);
        }

        void edit_Invoked(object sender, EventArgs e)
        {
            //navigate to edit page
        }

        private UserFilter filter;
        public UserFilter Filter
        {
            get { return this.filter; }
            set
            {
                if (this.filter != value)
                {
                    this.filter = value;
                    FirePropertyChanged("Filter");
                }
            }
        }

        public string Name
        {
            get
            {
                return this.filter.Name;
            }
        }
    }
}
