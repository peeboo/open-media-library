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
        ParentalControls controls;
        ParentalControlSetting parentalSettings;
        Timer _timer;
        DateTime unlockedTime { get; set; }
        int unlockPeriod { get; set; }

        public bool isLocked
        {
            get
            {
                //if (parentalSettings != null)
                //    return parentalSettings.Enabled;

                return false;
            }
        }

        public bool BlockUnrated
        {
            get
            {
                if (parentalSettings != null)
                    return parentalSettings.BlockUnrated;

                return false;
            }
        }

        public ParentalControlManager()
        {
            controls =
                OMLApplication.Current.MediaCenterEnvironment.ParentalControls;

            foreach (string ratingSystem in controls.GetAvailableRatingSystems())
            {
                if (ratingSystem.ToUpperInvariant().Contains("movie".ToUpperInvariant()))
                    parentalSettings = controls[ratingSystem];
            }

            _timer = new Timer();
            _timer.Enabled = false;
            _timer.Interval = 60000;
            _timer.Tick += new EventHandler(_timer_Tick);
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if (DateTime.UtcNow >= this.unlockedTime.AddHours(this.unlockPeriod)) this.ReLock();
        }

        public void PlayMovie(Library.Code.V3.MovieItem item)
        {
            if (this.isLocked)
            {
                if (!string.IsNullOrEmpty(item.TitleObject.ParentalRating))
                {
                    controls.PromptForPin(delegate(bool goodPin)
                    {
                        if (goodPin)
                            item.PlayMovie();
                    });
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
            if (this.isLocked)
            {
                if (!string.IsNullOrEmpty(item.TitleObject.ParentalRating))
                {
                    controls.PromptForPin(delegate(bool goodPin)
                    {
                        if (goodPin)
                            item.PlayAllDisks();
                    });
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
            } else
                item.PlayAllDisks();
        }

        public void UnLock()
        {
            MediaCenterEnvironment env = Microsoft.MediaCenter.Hosting.AddInHost.Current.MediaCenterEnvironment;
            controls = env.ParentalControls;
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
