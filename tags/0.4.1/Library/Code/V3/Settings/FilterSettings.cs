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
    public class FilterSettings : ModelItem
    {
        public FilterSettings()
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

            this.SetupFilters();
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
            if (this._showFilterUnwatched.Value != OMLSettings.ShowFilterUnwatched)
                return true;
            if (this._showFilterActors.Value != OMLSettings.ShowFilterActors)
                return true;
            if (this._showFilterCountry.Value != OMLSettings.ShowFilterCountry)
                return true;
            if (this._showFilterDateAdded.Value != OMLSettings.ShowFilterDateAdded)
                return true;
            if (this._showFilterDirectors.Value != OMLSettings.ShowFilterDirectors)
                return true;
            if (this._showFilterFormat.Value != OMLSettings.ShowFilterFormat)
                return true;
            if (this._showFilterGenres.Value != OMLSettings.ShowFilterGenres)
                return true;
            if (this._showFilterParentalRating.Value != OMLSettings.ShowFilterParentalRating)
                return true;
            if (this._showFilterRuntime.Value != OMLSettings.ShowFilterRuntime)
                return true;
            if (this._showFilterTags.Value != OMLSettings.ShowFilterTags)
                return true;
            if (this._showFilterUserRating.Value != OMLSettings.ShowFilterUserRating)
                return true;
            if (this._showFilterYear.Value != OMLSettings.ShowFilterYear)
                return true;
            return false;
        }

        /// <summary>
        /// saving settings
        /// </summary>
        public void Save()
        {
            OMLSettings.ShowFilterUnwatched = (bool)_showFilterUnwatched.Chosen;
            OMLSettings.ShowFilterActors = (bool)_showFilterActors.Chosen;
            OMLSettings.ShowFilterCountry = (bool)_showFilterCountry.Chosen;
            OMLSettings.ShowFilterDateAdded = (bool)_showFilterDateAdded.Chosen;
            OMLSettings.ShowFilterDirectors = (bool)_showFilterDirectors.Chosen;
            OMLSettings.ShowFilterFormat = (bool)_showFilterFormat.Chosen;
            OMLSettings.ShowFilterGenres = (bool)_showFilterGenres.Chosen;
            OMLSettings.ShowFilterParentalRating = (bool)_showFilterParentalRating.Chosen;
            OMLSettings.ShowFilterRuntime = (bool)_showFilterRuntime.Chosen;
            OMLSettings.ShowFilterTags = (bool)_showFilterTags.Chosen;
            OMLSettings.ShowFilterUserRating = (bool)_showFilterUserRating.Chosen;
            OMLSettings.ShowFilterYear = (bool)_showFilterYear.Chosen;
            //OMLSettings.ShowFilterTrailers = (bool)_showFilterTrailers.Chosen; 
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

        private ArrayListDataSet filtersArray = new ArrayListDataSet();
        public ArrayListDataSet FiltersArray
        {
            get { return filtersArray; }
            set
            {
                if (filtersArray != value)
                {
                    filtersArray = value;
                    FirePropertyChanged("FiltersArray");
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

        #region Filters
        BooleanChoice _showFilterDirectors = new BooleanChoice();
        BooleanChoice _showFilterActors = new BooleanChoice();
        BooleanChoice _showFilterFormat = new BooleanChoice();
        //BooleanChoice _showFilterTrailers = new BooleanChoice();
        BooleanChoice _showFilterDateAdded = new BooleanChoice();
        BooleanChoice _showFilterYear = new BooleanChoice();
        BooleanChoice _showFilterUserRating = new BooleanChoice();
        BooleanChoice _showFilterTags = new BooleanChoice();
        BooleanChoice _showFilterParentalRating = new BooleanChoice();
        BooleanChoice _showFilterCountry = new BooleanChoice();
        BooleanChoice _showFilterRuntime = new BooleanChoice();
        BooleanChoice _showFilterUnwatched = new BooleanChoice();
        BooleanChoice _showFilterGenres = new BooleanChoice();
        #endregion Filters

        private void SetupFilters()
        {
            //"Genres",
            //"Directors",
            //"Actors",
            //"Runtime",
            //"Country",
            //"Parental Rating",
            //"Tags",
            //"User Rating",
            //"Year",
            //"Date Added",
            //"Format",
            //"Trailers"
            this.filtersArray = new ArrayListDataSet();

            this._showFilterUnwatched.Chosen = OMLSettings.ShowFilterUnwatched;
            this._showFilterUnwatched.Description = "Unwatched";
            this.filtersArray.Add(this._showFilterUnwatched);

            this._showFilterActors.Chosen = OMLSettings.ShowFilterActors;
            this._showFilterActors.Description = "Actors";
            this.filtersArray.Add(this._showFilterActors);

            this._showFilterCountry.Chosen = OMLSettings.ShowFilterCountry;
            this._showFilterCountry.Description = "Country";
            this.filtersArray.Add(this._showFilterCountry);

            this._showFilterDateAdded.Chosen = OMLSettings.ShowFilterDateAdded;
            this._showFilterDateAdded.Description = "Date Added";
            this.filtersArray.Add(this._showFilterDateAdded);

            this._showFilterDirectors.Chosen = OMLSettings.ShowFilterDirectors;
            this._showFilterDirectors.Description = "Directors";
            this.filtersArray.Add(this._showFilterDirectors);

            this._showFilterFormat.Chosen = OMLSettings.ShowFilterFormat;
            this._showFilterFormat.Description = "Format";
            this.filtersArray.Add(this._showFilterFormat);

            this._showFilterGenres.Chosen = OMLSettings.ShowFilterGenres;
            this._showFilterGenres.Description = "Genres";
            this.filtersArray.Add(this._showFilterGenres);

            this._showFilterParentalRating.Chosen = OMLSettings.ShowFilterParentalRating;
            this._showFilterParentalRating.Description = "Parental Rating";
            this.filtersArray.Add(this._showFilterParentalRating);

            this._showFilterRuntime.Chosen = OMLSettings.ShowFilterRuntime;
            this._showFilterRuntime.Description = "Runtime";
            this.filtersArray.Add(this._showFilterRuntime);

            this._showFilterTags.Chosen = OMLSettings.ShowFilterTags;
            this._showFilterTags.Description = "Tags";
            this.filtersArray.Add(this._showFilterTags);

            this._showFilterUserRating.Chosen = OMLSettings.ShowFilterUserRating;
            this._showFilterUserRating.Description = "User Rating";
            this.filtersArray.Add(this._showFilterUserRating);

            this._showFilterYear.Chosen = OMLSettings.ShowFilterYear;
            this._showFilterYear.Description = "Year";
            this.filtersArray.Add(this._showFilterYear);

            //this._showFilterTrailers.Chosen = OMLSettings.ShowFilterTrailers;
            //this._showFilterTrailers.Description = "Trailers";
            //this.filtersArray.Add(this._showFilterTrailers);
        }
    }
}
