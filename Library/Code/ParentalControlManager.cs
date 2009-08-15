using System;
using System.Collections.Generic;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;

namespace Library
{
    public class ParentalControlManager : ModelItem
    {
        Library.Code.V3.MovieItem currentItem;
        ParentalControls controls;
        IList<string> ratingsSystems;

        public bool isLocked
        {
            get
            {
                return false;
            }
        }

        public ParentalControlManager()
        {
            controls =
                OMLApplication.Current.MediaCenterEnvironment.ParentalControls;
            ratingsSystems = controls.GetAvailableRatingSystems();
        }

        public void PlayMovie(Library.Code.V3.MovieItem item)
        {
            this.currentItem = item;
            if (this.isLocked)
            {
                ParentalPromptCompletedCallback cb = cbPlayMovie;
                controls.PromptForPin(cb);
            } else
                item.PlayMovie();
        }

        public void cbPlayMovie(bool correctPinEntered)
        {
            if (correctPinEntered)
                this.currentItem.PlayMovie();
        }

        public void PlayAllDisks(Library.Code.V3.MovieItem item)
        {
            this.currentItem = item;
            if (this.isLocked)
            {
                ParentalPromptCompletedCallback cb = cbPlayAllDisks;
                controls.PromptForPin(cb);
            } else
                item.PlayAllDisks();
        }

        public void cbPlayAllDisks(bool correctPinEntered)
        {
            if (correctPinEntered)
                this.currentItem.PlayAllDisks();
        }
    }
}
