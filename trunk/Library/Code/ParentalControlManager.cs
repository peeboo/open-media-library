using System;
using System.Collections.Generic;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;


// NOTE: I borrowed much of the logic and some of the code
// from MediaBrowser on 2009/08/15
// mediabrowser.googlecode.com

namespace Library
{
    public class ParentalControlManager : ModelItem
    {
        Dictionary<string, int> MCMovieRatings = new Dictionary<string, int>();
        Dictionary<int, string> MCMovieRatingStrings = new Dictionary<int, string>();
        Timer _timer;
        DateTime unlockedTime { get; set; }
        int unlockPeriod { get; set; }

        #region properties
        ParentalControls controls
        {
            get
            {
                return Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment.ParentalControls;
            }
        }
        ParentalControlSetting parentalSettings
        {
            get
            {
                foreach (string ratingSystem in controls.GetAvailableRatingSystems())
                {
                    if (ratingSystem.ToUpperInvariant().Contains("movie".ToUpperInvariant()))
                        return controls[ratingSystem];
                }
                return null;
            }
        }
        public bool Enabled
        {
            get
            {
                return parentalSettings.Enabled;
            }
        }
        public bool BlockUnrated
        {
            get
            {
                return parentalSettings.BlockUnrated;
            }
        }
        public int MaxAllowed
        {
            get
            {
                return parentalSettings.MaxAllowed;
            }
        }
        #endregion properties

        public ParentalControlManager()
        {
            MCMovieRatings.Add("G", 1);
            MCMovieRatings.Add("PG", 2);
            MCMovieRatings.Add("PG-13", 3);
            MCMovieRatings.Add("R", 4);
            MCMovieRatings.Add("NC-17", 5);
            MCMovieRatings.Add("Unrated", 10);
            MCMovieRatings.Add("X", 15);
            MCMovieRatings.Add("XXX", 20);

            foreach (string key in MCMovieRatings.Keys)
                MCMovieRatingStrings.Add(MCMovieRatings[key], key);

            _timer = new Timer();
            _timer.Enabled = false;
            _timer.Interval = 60000;
            _timer.Tick += new EventHandler(_timer_Tick);
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if (DateTime.UtcNow >= this.unlockedTime.AddHours(this.unlockPeriod)) this.ReLock();
        }

        public bool ItemIsAllowed(Library.MovieItem item)
        {
            if (this.Enabled)
            {
                string itemRating = item.TitleObject.ParentalRating.ToUpperInvariant();
                if (string.IsNullOrEmpty(itemRating))
                {
                    if (MCMovieRatings[itemRating] != null)
                        if (MCMovieRatings[itemRating] <= this.MaxAllowed)
                            return true;
                }
                else
                {
                    if (this.BlockUnrated)
                        return false;
                }
            }
            return true;
        }

        public bool ItemIsAllowed(Library.Code.V3.MovieItem item)
        {
            if (this.Enabled)
            {
                string itemRating = item.TitleObject.ParentalRating.ToUpperInvariant();
                if (string.IsNullOrEmpty(itemRating))
                {
                    if (MCMovieRatings[itemRating] != null)
                        if (MCMovieRatings[itemRating] <= this.MaxAllowed)
                            return true;
                }
                else
                {
                    if (this.BlockUnrated)
                        return false;
                }
            }
            return true;
        }

        public void PlayMovie(Library.Code.V3.MovieItem item)
        {
            if (this.Enabled)
            {
                string itemRating = item.TitleObject.ParentalRating.ToUpperInvariant();
                if (!string.IsNullOrEmpty(itemRating))
                {
                    if (MCMovieRatings[itemRating] != null)
                    {
                        if (MCMovieRatings[itemRating] > this.MaxAllowed)
                        {
                            controls.PromptForPin(delegate(bool goodPin)
                            {
                                if (goodPin)
                                    item.PlayMovie();
                            });
                        }
                    }
                }
                else
                {
                    if (this.BlockUnrated)
                    {
                        controls.PromptForPin(delegate(bool goodPin)
                        {
                            if (goodPin)
                                item.PlayMovie();
                        });
                    }
                }
            } else
                item.PlayMovie();
        }

        public void PlayAllDisks(Library.Code.V3.MovieItem item)
        {
            if (this.Enabled)
            {
                string itemRating = item.TitleObject.ParentalRating.ToUpperInvariant();
                if (!string.IsNullOrEmpty(itemRating))
                {
                    if (MCMovieRatings[itemRating] != null)
                    {
                        if (MCMovieRatings[itemRating] > this.MaxAllowed)
                        {
                            controls.PromptForPin(delegate(bool goodPin)
                            {
                                if (goodPin)
                                    item.PlayAllDisks();
                            });
                        }
                    }
                }
                else
                {
                    if (this.BlockUnrated)
                    {
                        controls.PromptForPin(delegate(bool goodPin)
                        {
                            if (goodPin)
                                item.PlayAllDisks();
                        });
                    }
                }
            }
            else
                item.PlayAllDisks();
        }

        public void UnLock()
        {
            controls.PromptForPin(delegate(bool goodPin)
            {
                if (goodPin)
                {
                    this.unlockedTime = DateTime.UtcNow;
                    this.unlockPeriod = 60000; // currently set to 1 hour
                    _timer.Start();
                }
            });
        }

        public void ReLock()
        {
            _timer.Stop();
        }
    }
}

//Notes about Parental Ratings research:

//MediaCenterEnvironment.ParentalControls.GetAvailableRatingSystems() // which returns 'EN-US-TV', and 'EN-US-MOVIE'


//MediaCenterEnvironment.ParentalControls["EN-US-TV"]
//MediaCenterEnvironment.ParentalControls["EN-US-MOVIE"]

//.Enabled
//.BlockUnrated
//.MaxAllowed
//.RatingSystem

//TV:

//block all rated content: 0
//TV-Y: 1
//TV-Y7: 2
//TV-G: 3
//TV-PG: 4
//TV-14: 5
//TV-MA: 6

//MOVIE:
//block all rated movies: 0
//G: 1
//PG: 2
//PG-13: 3
//R: 4
//NC-17: 5

//we'll have to come up with our own for adult content
