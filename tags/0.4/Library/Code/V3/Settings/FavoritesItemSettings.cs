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
    public class FavoritesItemSettings : ModelItem
    {
        private UserFilter filter;
        private bool create;

        public FavoritesItemSettings(UserFilter Filter, bool Create)
        {
            this.create = Create;
            this.filter = Filter;

            if (create == true)
                this.Description = "NEW FAVORITE";
            else
                this.Description = "EDIT FAVORITE";
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


            this.SetupFavoriteItem();
        }

        private void SetupFavoriteItem()
        {
            this.name = new EditableText(this);
            this.name.Value = this.filter.Name;
        }

        private EditableText name;
        public EditableText Name
        {
            get { return this.name; }
            set { this.name = value; }
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
            if (this.create)
            {
                IList<UserFilter> oldFilters = OMLSettings.UserFilters;
                UserFilter[] newFilters = new UserFilter[oldFilters.Count + 1];
                //deal with the existing userfilters
                for (int i = 0; i < oldFilters.Count; i++)
                {
                    newFilters[i] = oldFilters[i];
                }

                //OMLEngine.TitleFilter[] newFilter = new OMLEngine.TitleFilter[this.filter.Filters.Count];
                //for (int i = 0; i < this.filter.Filters.Count; i++)
                //{
                //    newFilter[i] = this.filter.Filters[i];
                //}

                UserFilter filter = new UserFilter(this.name.Value, this.filter.Filters);

                newFilters[oldFilters.Count] = filter;
                OMLSettings.UserFilters = newFilters;
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
