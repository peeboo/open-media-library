using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.MediaCenter.UI;
using System.Collections;
using System.Globalization;

namespace Library.Code.V3
{
    public class BrowseModel : ModelItem
    {
        // Fields
        private Choice m_choicePivots;
        private IList m_listCommands;

        // Methods
        public BrowseModel()
        {
        }

        public BrowseModel(IModelItemOwner owner)
            : base(owner)
        {
        }

        // Properties
        public IList Commands
        {
            get
            {
                return this.m_listCommands;
            }
            set
            {
                if (this.m_listCommands != value)
                {
                    this.m_listCommands = value;
                    base.FirePropertyChanged("Commands");
                }
            }
        }

        public Choice Pivots
        {
            get
            {
                return this.m_choicePivots;
            }
            set
            {
                if (this.m_choicePivots != value)
                {
                    this.m_choicePivots = value;
                    base.FirePropertyChanged("Pivots");
                }
            }
        }
    }

    public class DescriptionTransformer : FormatTransformer
    {
        // Methods
        protected override object Transform(object value)
        {
            ModelItem item = value as ModelItem;
            if (item != null)
            {
                return base.Transform(item.Description);
            }
            return base.Transform(value);
        }
    }

    [MarkupVisible]
    public class FormatTransformer : ITransformer
    {
        // Fields
        private bool m_fToLower;
        private bool m_fToUpper;
        private string m_stExtendedFormat;
        private string m_stFormat = s_stDefaultFormat;
        private static string s_stDefaultFormat = "{0}";

        // Methods
        object ITransformer.Transform(object value)
        {
            return this.Transform(value);
        }

        protected virtual object Transform(object value)
        {
            if (this.m_stExtendedFormat != null)
            {
                IFormattable formattable = value as IFormattable;
                if (formattable != null)
                {
                    value = formattable.ToString(this.m_stExtendedFormat, null);
                }
            }
            string text = InvariantString.Format(this.m_stFormat, new object[] { value });
            //if (MarkupSystem.EnableStringCaseTransforms)
            //{
            //    if (this.m_fToUpper)
            //    {
            //        return text.ToUpper(CultureInfo.CurrentUICulture);
            //    }
            //    if (this.m_fToLower)
            //    {
            //        text = text.ToLower(CultureInfo.CurrentUICulture);
            //    }
            //}
            return text;
        }

        // Properties
        [MarkupVisible]
        public string ExtendedFormat
        {
            get
            {
                return this.m_stExtendedFormat;
            }
            set
            {
                this.m_stExtendedFormat = value;
            }
        }

        [MarkupVisible]
        public string Format
        {
            get
            {
                return this.m_stFormat;
            }
            set
            {
                this.m_stFormat = value;
            }
        }

        [MarkupVisible]
        public bool ToLower
        {
            get
            {
                return this.m_fToLower;
            }
            set
            {
                this.m_fToLower = value;
            }
        }

        [MarkupVisible]
        public bool ToUpper
        {
            get
            {
                return this.m_fToUpper;
            }
            set
            {
                this.m_fToUpper = value;
            }
        }
    }

    public class PageState : ModelItem
    {
        // Fields
        private bool m_isCurrentPage;
        private PageTransitionState m_transitionState;

        private EditableText eTPageState;
        public EditableText ETPageState
        {
            get
            {
                return this.eTPageState;
            }
            set
            {
                if (value.Value != this.eTPageState.Value)
                {
                    this.eTPageState.Value = value.Value;
                    switch (value.Value)
                    {
                        case "Idle":
                            this.TransitionState = PageTransitionState.Idle;
                            break;
                        case "NavigatingAwayBackward":
                            this.TransitionState = PageTransitionState.NavigatingAwayBackward;
                            break;
                        case "NavigatingAwayForward":
                            this.TransitionState = PageTransitionState.NavigatingAwayForward;
                            break;
                        case "NavigatingToBackward":
                            this.TransitionState = PageTransitionState.NavigatingToBackward;
                            break;
                        case "NavigatingToForward":
                            this.TransitionState = PageTransitionState.NavigatingToForward;
                            break;
                    }
                    base.FirePropertyChanged("ETPageState");
                }
            }
        }

        // Methods
        public PageState()
            : base()
        {
        }

        public PageState(IModelItemOwner owner)
            : base(owner)
        {
            this.m_transitionState = PageTransitionState.NavigatingToForward;
            this.eTPageState = new EditableText(this);
            this.eTPageState.Value = "NavigatingToForward";
        }

        private static PageTransitionState CalculateTransitionState(bool isCurrentPage, bool wasCurrentPage, bool isNavigating, NavigationDirection navigationDirection)
        {
            if (isNavigating && (isCurrentPage || wasCurrentPage))
            {
                if (navigationDirection == NavigationDirection.Forward)
                {
                    if (isCurrentPage)
                    {
                        return PageTransitionState.NavigatingToForward;
                    }
                    return PageTransitionState.NavigatingAwayForward;
                }
                if (isCurrentPage)
                {
                    return PageTransitionState.NavigatingToBackward;
                }
                return PageTransitionState.NavigatingAwayBackward;
            }
            if (isCurrentPage)
            {
                return PageTransitionState.IdleActive;
            }
            return PageTransitionState.IdleInactive;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    UnregisterPageState(this);
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        private void OnEnvironmentPropertyChanged(IPropertyObject sender, string property)
        {
            if ((property == "IsNavigating") || (property == "NavigationDirection"))
            {
                this.UpdateTransitionState();
            }
        }

        private static void RegisterPageState(PageState state)
        {
        }

        private static void ScheduleStateDump()
        {
        }

        private static void UnregisterPageState(PageState state)
        {
        }

        private void UpdateTransitionState()
        {
            this.UpdateTransitionState(this.IsCurrentPage);
        }

        private void UpdateTransitionState(bool wasCurrentPage)
        {
            //Environment environment = Environment.Get(UiSession.Default);
            //this.TransitionState = CalculateTransitionState(this.IsCurrentPage, wasCurrentPage, environment.IsNavigating, environment.NavigationDirection);
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new System.Timers.ElapsedEventHandler(aTimer_Elapsed);
            // Set the Interval to 50 seconds.
            //aTimer.Interval = 1180;
            aTimer.AutoReset = false;
            aTimer.Interval = 15000;
            aTimer.Enabled = true;
        }

        void aTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //this.TransitionState = PageTransitionState.Idle;
        }

        // Properties
        public bool IsCurrentPage
        {
            get
            {
                return this.m_isCurrentPage;
            }
            internal set
            {
                if (this.m_isCurrentPage != value)
                {
                    this.m_isCurrentPage = value;
                    base.FirePropertyChanged("IsCurrentPage");
                    //hack
                    this.TransitionState = PageTransitionState.Idle;
                    //this.TransitionState = PageTransitionState.NavigatingToForward;
                    //this.TransitionState = PageTransitionState.IdleActive;
                    this.UpdateTransitionState(!value);
                }
            }
        }

        public PageTransitionState TransitionState
        {
            get
            {
                return this.m_transitionState;
            }
            //private set
            //fix
            set
            {
                if (this.m_transitionState != value)
                {
                    this.ETPageState.Value = value.ToString();
                    this.m_transitionState = value;
                    base.FirePropertyChanged("TransitionState");
                    if (value == PageTransitionState.Idle)
                    {
                        //
                    }
                    //ScheduleStateDump();
                }
            }
        }
    }

    public enum PageTransitionState
    {
        IdleInactive,
        IdleActive,
        NavigatingToForward,
        NavigatingToBackward,
        NavigatingAwayForward,
        NavigatingAwayBackward,
        Idle,
    }

    public enum NavigationDirection
    {
        Backward,
        Forward
    }
}
