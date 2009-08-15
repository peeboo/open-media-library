using System;
using System.Collections.Generic;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;

namespace Library
{
    public class ParentalControlManager : ModelItem
    {
        ParentalControls controls;
        IList<string> ratingsSystems;

        public ParentalControlManager()
        {
            controls =
                OMLApplication.Current.MediaCenterEnvironment.ParentalControls;
            ratingsSystems = controls.GetAvailableRatingSystems();
        }

        public void AskForPin()
        {
            ParentalPromptCompletedCallback parentalCallback = UnlockParentalLimits;
            controls.PromptForPin(parentalCallback);
        }

        public void UnlockParentalLimits(bool correctPinEntered)
        {
        }
    }
}
