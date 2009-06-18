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
    public class StartMenuItemSettings : ModelItem
    {
        private StartMenuHelper helper;
        private StartMenuItem selectedItem;

        public StartMenuItemSettings(StartMenuItem SelectedItem, StartMenuHelper Helper)
        {
            this.helper = Helper;
            this.selectedItem = SelectedItem;

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

            
            this.SetupStartMenu();
        }

        private Context GetFreeContext()
        {
            List<Context> ctxArray = new List<Context>();
            ctxArray.Add(Context.Custom1);
            ctxArray.Add(Context.Custom2);
            ctxArray.Add(Context.Custom3);
            ctxArray.Add(Context.Custom4);
            ctxArray.Add(Context.Custom5);
            foreach (StartMenuItem item in helper.StartMenuItems)
            {
                if (item != this.selectedItem && ctxArray.IndexOf(item.Context) > -1)
                {
                    ctxArray.Remove(item.Context);
                }
            }
            if (ctxArray.Count > 0)
                return ctxArray[0];
            else
                return Context.Custom5;
        }

        private void SetupStartMenu()
        {
            //this.entryPoints = new Choice(this);
            //List<string> entryPointItems = new List<string>();
            //entryPointItems.Add("OML Home");
            //entryPointItems.Add("Movies");
            //entryPointItems.Add("TV");
            //entryPointItems.Add("Trailers");
            //entryPointItems.Add("Favorites");
            //entryPoints.Options = entryPointItems;
            //entryPoints.Chosen = DisplayFromTrailerFormat(OMLSettings.TrailersDefinition);
            //this.selectedTrailerResolution = DisplayFromTrailerFormat(OMLSettings.TrailersDefinition);

            this.name = new EditableText(this);
            this.name.Value = this.selectedItem.Title;

            this.Favorites = new Choice(this);
            List<StartMenuSelection> favoritesItems = new List<StartMenuSelection>();
            favoritesItems.Add(new StartMenuSelection("OML Home", Context.Home,string.Empty));
            favoritesItems.Add(new StartMenuSelection("Trailers", Context.Trailers, string.Empty));
            favoritesItems.Add(new StartMenuSelection("Search",Context.Search,string.Empty));
            favoritesItems.Add(new StartMenuSelection("Settings",Context.Settings, string.Empty));
            Context availableContext = this.GetFreeContext();
            int selectedItemIndex=0;
            foreach (UserFilter filt in OMLSettings.UserFilters)
            {
                StartMenuSelection filtSelection = new StartMenuSelection(string.Format("Favorites: {0}", filt.Name), availableContext, filt.ToString());
                favoritesItems.Add(filtSelection);
                if (this.selectedItem.ExtendedContext == filtSelection.ExtendedContext)
                    selectedItemIndex = favoritesItems.IndexOf(filtSelection);
            }
            Favorites.Options = favoritesItems;
            Favorites.ChosenIndex = selectedItemIndex;
            switch (this.selectedItem.Context)
            {
                case Context.Home:
                    Favorites.ChosenIndex = 0;
                    break;

                case Context.Trailers:
                    Favorites.ChosenIndex = 1;
                    break;

                case Context.Search:
                    Favorites.ChosenIndex = 2;
                    break;
                case Context.Settings:
                    Favorites.ChosenIndex = 3;
                    break;
            }
        }

        private EditableText name;
        public EditableText Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        private string selectedEntryPoint;
        private Choice entryPoints;
        public Choice EntryPoints
        {
            get { return entryPoints; }
            set { entryPoints = value; }
        }

        private Choice favorites;
        public Choice Favorites
        {
            get { return favorites; }
            set { favorites = value; }
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
            if (this.selectedItem.Title != this.name.Value)
                return true;
            return false;
        }

        /// <summary>
        /// saving settings
        /// </summary>
        public void Save()
        {
            this.selectedItem.Title = this.name.Value;
            this.selectedItem.Context = ((StartMenuSelection)this.Favorites.Chosen).Context;
            this.selectedItem.ExtendedContext = ((StartMenuSelection)this.Favorites.Chosen).ExtendedContext;
            if (string.IsNullOrEmpty(this.selectedItem.ItemId))
            {
                this.selectedItem.Description = "some desc";
                this.selectedItem.ImageUrl = @"C:\Program Files\Open Media Library\Application.png";
                this.helper.AddStartMenuItem(this.selectedItem);
            }
            else
                this.helper.SaveStartMenu();
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

        public class StartMenuSelection
        {
            public string Name { get; set; }
            public Context Context { get; set; }
            public string ExtendedContext { get; set; }
            public StartMenuSelection(string name, Context ctx, string extCtx)
            {
                this.Name = name;
                this.Context = ctx;
                this.ExtendedContext = extCtx;
            }

            public override string ToString()
            {
                return this.Name;
            }
        }
    }
}
