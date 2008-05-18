using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MediaCenter.UI;
using System.Diagnostics;
using OMLEngine;

namespace Library
{
    public class CategoryFilter
    {
        public const string CategoryGenres = "Genre";
        public const string CategoryDirector = "Director";
        public const string CategoryActor = "Actor";
        public const string CategoryRuntime = "Runtime";
        public const string CategoryCountry = "Country";
        public const string CategoryParentRating = "Parent Rating";
        public const string CategoryUserRating = "User Rating";
        public const string CategoryYear = "Year";

        public CategoryFilter(Choice choice, EventHandler e)
        {
            _availableChoices = choice;
            _filterChangedEvent = e;
        }

        public Choice AvailableChoices
        {
            get { return _availableChoices; }
            set { _availableChoices = value; }
        }

        public EventHandler FilterChangedEvent
        {
            get { return _filterChangedEvent; }
            set { _filterChangedEvent = value; }
        }

        private EventHandler _filterChangedEvent;
        private Choice _availableChoices;

    }

    /// <summary>
    /// A list of movies (MovieItems)
    /// </summary>
    public class Gallery : ModelItem
    {
        private string _currentCategory;

        public string CurrentCategory
        {
            get { return _currentCategory; }
            set { _currentCategory = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gallery"/> class.
        /// </summary>
        public Gallery()
        {
        }

        /// <summary>
        /// A list of MovieItems used by the UI
        /// </summary>
        /// <value>The movies.</value>
        virtual public VirtualList Items
        {
            get { return null; }
            set { ; }
        }

        /// <summary>
        /// Sets the filters.
        /// </summary>
        /// <param name="category">The category (Genre, Actors, etc).</param>
        /// <param name="filters">The filters (possible genres, possible actors, etc).</param>
        public void SetFilters(string category, CategoryFilter filters)
        {
            if (_filters.ContainsKey(category))
            {
                if (filters != _filters[category])
                {
                    // update the filters
                    _filters[category] = filters;
                    //filters.AvailableChoices.ChosenChanged -= filters.FilterChangedEvent;
                }
            }
            else
            {
                _filters.Add(category, filters);
            }

            filters.AvailableChoices.ChosenChanged += filters.FilterChangedEvent;
            //filters.FilterChangedEvent(this, EventArgs.Empty);
            //FirePropertyChanged("Filters");

        }

        /// <summary>
        /// Gets the filter for a specified category ( a list of possible filters such as genres, etc).
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        public CategoryFilter GetFilter(string category)
        {
            if (_filters.ContainsKey(category))
            {
                // update the filters
                return (CategoryFilter)_filters[category];
            }
            else
            {
                return null;
            }
        }

        public CategoryFilter CurrentFilter
        {
            get { return GetFilter(CurrentCategory); }
        }

        private Hashtable _filters = new Hashtable();
    }

    public class Filter : ModelItem
    {
        public const string AllFilter = "All";

        public Filter(Gallery page, string description, string category)
            : base(page, description)
        {
            _category = category;
        }

        private string _category;

        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }
    }
}
