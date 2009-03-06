using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections;

namespace Library.Code.V3
{
    public class OverlaySlideDeck : SlideDeck
    {
        protected override string BuildUIContext()
        {
            throw new NotImplementedException();
        }

        public override void Show()
        {
            //throw new NotImplementedException();
        }

        public string GetString(string id)
        {
            return id;
        }

        private IList m_listCommands;

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

        private IList m_additionalCommands;

        public IList AdditionalCommands
        {
            get
            {
                return this.m_additionalCommands;
            }
            set
            {
                if (this.m_additionalCommands != value)
                {
                    this.m_additionalCommands = value;
                    base.FirePropertyChanged("AdditionalCommands");
                }
            }
        }
    }

    public class DetailsSlideDeck : OverlaySlideDeck
    {
        
    }

    public class MovieDetailsSlideDeck : DetailsSlideDeck
    {
        private IList m_Actors;

        public IList Actors
        {
            get
            {
                return this.m_Actors;
            }
            set
            {
                if (this.m_Actors != value)
                {
                    this.m_Actors = value;
                    base.FirePropertyChanged("Actors");
                }
            }
        }

        private Microsoft.MediaCenter.UI.Choice m_SimilarMovies;

        public Microsoft.MediaCenter.UI.Choice SimilarMovies
        {
            get
            {
                return this.m_SimilarMovies;
            }
            set
            {
                if (this.m_SimilarMovies != value)
                {
                    this.m_SimilarMovies = value;
                    base.FirePropertyChanged("SimilarMovies");
                }
            }
        }

        private Microsoft.MediaCenter.UI.Choice m_OnDemandOffers;

        public Microsoft.MediaCenter.UI.Choice OnDemandOffers
        {
            get
            {
                return this.m_OnDemandOffers;
            }
            set
            {
                if (this.m_OnDemandOffers != value)
                {
                    this.m_OnDemandOffers = value;
                    base.FirePropertyChanged("OnDemandOffers");
                }
            }
        }

        [MarkupVisible]
        public bool HasSimilarPrograms
        {
            get
            {
                return true;
            }
            set
            {
                //this.m_stExtendedFormat = value;
            }
        }

        [MarkupVisible]
        public bool HasOnDemand
        {
            get
            {
                return false;
            }
            set
            {
                //this.m_stExtendedFormat = value;
            }
        }

        

        [MarkupVisible]
        public bool CanHaveOtherShowings
        {
            get
            {
                return true;
            }
            set
            {
                //this.m_stExtendedFormat = value;
            }
        }

        [MarkupVisible]
        public bool IsHD
        {
            get
            {
                return true;
            }
            set
            {
                //this.m_stExtendedFormat = value;
            }
        }

        [MarkupVisible]
        public bool IsDolbyDigital
        {
            get
            {
                return false;
            }
            set
            {
                //this.m_stExtendedFormat = value;
            }
        }

        public Microsoft.MediaCenter.UI.Image RecordingStatusImage
        {
            get { return null; }
        }

        public Microsoft.MediaCenter.UI.Image StarRatingImage
        {
            get { return null; }
        }

        public string RecordingStatusText
        {
            get { return ""; }
        }

        public string CopyStatusText
        {
            get { return ""; }
        }

        public string MovieType
        {
            get { return "RecordedTV"; }
        }

        public string ReleaseYear
        {
            get
            {
                return DateTime.Now.Year.ToString();
            }
        }

        public string Duration
        {
            get
            {
                return "100";
            }
        }

        public DateTime StartTime
        {
            get
            {
                return DateTime.Now;
            }
        }

        public DateTime EndTime
        {
            get
            {
                return DateTime.Now;
            }
        }

        public string Title
        {
            get { return "Movie Title"; }
        }

        public string SubTitle
        {
            get { return "Movie Title"; }
        }

        public string ShowingType
        {
            get { return ""; }
        }

        public bool IsMovie
        {
            get { return true; }
        }

        [MarkupVisible]
        public bool IsInteractive
        {
            get
            {
                return false;
            }
            set
            {
                //this.m_stExtendedFormat = value;
            }
        }

        [MarkupVisible]
        public bool HasReview
        {
            get
            {
                return true;
            }
            set
            {
                //this.m_stExtendedFormat = value;
            }
        }

        [MarkupVisible]
        public string Review
        {
            get
            {
                return "this is a test";
            }
            set
            {
                //this.m_stExtendedFormat = value;
            }
        }

        [MarkupVisible]
        public bool ScheduleEntryPresent
        {
            get
            {
                return true;
            }
            set
            {
                //this.m_stExtendedFormat = value;
            }
        }

        private string m_Synopsis;
        [MarkupVisible]
        public string Synopsis
        {
            get
            {
                return m_Synopsis;
            }
            set
            {
                this.m_Synopsis = value;
            }
        }

        [MarkupVisible]
        public string Plot
        {
            get
            {
                return "this is a test";
            }
            set
            {
                //this.m_stExtendedFormat = value;
            }
        }
    }

    public abstract class SlideDeck : Microsoft.MediaCenter.UI.Choice, INavigationEventSink, INavigationEventSource
    {
        // Fields
        private bool _autoHideEnabled;
        private Microsoft.MediaCenter.UI.Timer _autoHideTimer;
        private SlideDeckBlueprint _blueprint;
        private Microsoft.MediaCenter.UI.ICommand _commandClearOverlays;
        private Microsoft.MediaCenter.UI.ICommand _commandNextSlide;
        private Microsoft.MediaCenter.UI.ICommand _commandPopOverlay;
        private Microsoft.MediaCenter.UI.ICommand _commandPrevSlide;
        private Microsoft.MediaCenter.UI.ICommand _commandPushOverlay;
        private string _context;
        private SlideBlueprint _defaultSlide;
        private bool _enabled;
        private SlideMarkupFinder _finder;
        private Guid _id;
        private bool _initialized;
        private Microsoft.MediaCenter.UI.IModelItem _model;
        private NavigationDirection _navigateAwayDirection;
        private NavigationDirection _navigateToDirection;
        protected bool _visible;

        // Events
        public event EventHandler<EventArgs<NavigationDirection>> NavigatedAway;

        public event EventHandler<EventArgs<NavigationDirection>> NavigatedTo;

        // Methods
        public SlideDeck()
            : this(0)
        {
        }

        public SlideDeck(int autoHideInterval)
        {
            this._enabled = true;
            this.Finder = new SlideMarkupFinder();
            this._autoHideTimer = new Microsoft.MediaCenter.UI.Timer();
            this._autoHideTimer.Interval = autoHideInterval;
            this._autoHideTimer.Tick += new EventHandler(this.OnAutoHideTimerTick);
            if (autoHideInterval != 0)
            {
                this._autoHideEnabled = true;
            }
            this._id = Guid.NewGuid();
        }

        protected abstract string BuildUIContext();
        public void DisableSlide(string name)
        {
            this.EnableOrDisableSlide(name, SlideInsertion.Remove);
        }

        protected override void Dispose(bool disposing)
        {
            if (this._autoHideTimer != null)
            {
                this._autoHideTimer.Tick -= new EventHandler(this.OnAutoHideTimerTick);
                this._autoHideTimer.Dispose();
                this._autoHideTimer = null;
            }
            if (this._commandNextSlide != null)
            {
                this._commandNextSlide.Invoked -= new EventHandler(this.OnNavigatedNextSlide);
                this._commandNextSlide = null;
            }
            if (this._commandPrevSlide != null)
            {
                this._commandPrevSlide.Invoked -= new EventHandler(this.OnNavigatedPreviousSlide);
                this._commandPrevSlide = null;
            }
            IDisposable model = this.Model as IDisposable;
            if (model != null)
            {
                model.Dispose();
                model = null;
            }
            base.Dispose(disposing);
        }

        private void EnableOrDisableSlide(string title, SlideInsertion insertion)
        {
            int num = -1;
            int num2 = 0;
            int num3 = -1;
            SlideBlueprint chosen = base.Chosen as SlideBlueprint;
            if ((this._blueprint != null) && (this._blueprint.Slides != null))
            {
                for (int i = 0; i < this._blueprint.Slides.Count; i++)
                {
                    SlideBlueprint other = this._blueprint.Slides[i];
                    if (other != null)
                    {
                        if (string.Compare(other.Title, title, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            if (other.Insertion == insertion)
                            {
                                return;
                            }
                            num = i;
                        }
                        if (other.Insertion == SlideInsertion.Add)
                        {
                            if (-1 == num3)
                            {
                                num3 = i;
                            }
                            if ((chosen == null) && other.Equals(this._blueprint.DefaultSlide))
                            {
                                chosen = other;
                            }
                            num2++;
                        }
                        else if ((chosen != null) && chosen.Equals(other))
                        {
                            chosen = null;
                        }
                    }
                }
                if ((-1 != num) && ((insertion != SlideInsertion.Remove) || (1 != num2)))
                {
                    this._blueprint.Slides[num].Insertion = insertion;
                    if (chosen == null)
                    {
                        chosen = this._blueprint.Slides[num3];
                    }
                    this.SafeUpdateOptions(chosen);
                }
            }
        }

        public void EnableSlide(string name)
        {
            this.EnableOrDisableSlide(name, SlideInsertion.Add);
        }

        ~SlideDeck()
        {
            this.Dispose(false);
        }

        public virtual void Hide()
        {
            this.AutoHideTimer.Stop();
        }

        public virtual void Initialize()
        {
            if (!this._initialized)
            {
                this._initialized = true;
                this.Context = this.BuildUIContext();
            }
        }

        public string JoinStrings(string separator, IList source)
        {
            List<string> list = new List<string>();
            foreach (object obj2 in source)
            {
                if (obj2 != null)
                {
                    string item = obj2.ToString().Trim();
                    if (0 < item.Length)
                    {
                        list.Add(item);
                    }
                }
            }
            return string.Join(separator, list.ToArray());
        }

        protected virtual void OnAutoHideTimerTick(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void OnCommandPopOverlay(object sender, EventArgs args)
        {
            this.Hide();
        }

        private void OnCommandPushOverlay(object sender, EventArgs args)
        {
            this.Show();
        }

        private void OnFinderStateChanged(object sender, ContextObjectStateEventArgs e)
        {
            switch (e.State)
            {
                case ContextObjectState.NotInitialized:
                case ContextObjectState.Loading:
                    break;

                case ContextObjectState.Ready:
                    lock (this._finder)
                    {
                        SlideDeckBlueprint blueprint = this.Finder.SlideDeckBlueprint.Clone();
                        if (blueprint != this._blueprint)
                        {
                            this._blueprint = blueprint;
                            if (this.DefaultSlide != null)
                            {
                                this._blueprint.DefaultSlide = this.DefaultSlide;
                            }
                            this.SafeUpdateOptions(this._blueprint.DefaultSlide);
                        }
                    }
                    break;

                default:
                    return;
            }
        }

        public virtual void OnNavigatedAway(object sender, EventArgs<NavigationDirection> e)
        {
            this.NavigateAwayDirection = e.Value;
            this._visible = false;
            if (this.NavigatedAway != null)
            {
                this.NavigatedAway(sender, e);
            }
            this.AutoHideTimer.Stop();
            base.FirePropertyChanged("OnNavigatedAway");
            base.FirePropertyChanged("Visible");
        }

        protected virtual void OnNavigatedNextSlide(object sender, EventArgs e)
        {
        }

        protected virtual void OnNavigatedPreviousSlide(object sender, EventArgs e)
        {
        }

        public virtual void OnNavigatedTo(object sender, EventArgs<NavigationDirection> e)
        {
            this.Initialize();
            this.NavigateToDirection = e.Value;
            this._visible = true;
            if (this.NavigatedTo != null)
            {
                this.NavigatedTo(sender, e);
            }
            if (this.AutoHideEnabled)
            {
                this.AutoHideTimer.Start();
            }
            base.FirePropertyChanged("OnNavigatedTo");
            base.FirePropertyChanged("Visible");
        }

        private void SafeUpdateOptions(object oSelectedBlueprint)
        {
            if (!Microsoft.MediaCenter.UI.Application.IsApplicationThread)
            {
                Microsoft.MediaCenter.UI.Application.DeferredInvoke(new Microsoft.MediaCenter.UI.DeferredHandler(this.SafeUpdateOptions), oSelectedBlueprint);
            }
            else
            {
                SlideBlueprint other = (SlideBlueprint)oSelectedBlueprint;
                List<SlideBlueprint> list = new List<SlideBlueprint>();
                for (int i = 0; i < this._blueprint.Slides.Count; i++)
                {
                    SlideBlueprint item = this._blueprint.Slides[i];
                    if (item.Insertion == SlideInsertion.Add)
                    {
                        list.Add(item);
                    }
                }
                if (other.Insertion == SlideInsertion.Remove)
                {
                    if (this._blueprint.DefaultSlide.Equals(other))
                    {
                        other = list[0];
                    }
                    else
                    {
                        other = this._blueprint.DefaultSlide;
                    }
                }
                base.Options = list;
                base.Chosen = other;
            }
        }

        public abstract void Show();

        // Properties
        public virtual bool AnimateContentArea
        {
            get
            {
                return true;
            }
        }

        public bool AutoHideEnabled
        {
            get
            {
                return this._autoHideEnabled;
            }
            set
            {
                if (this._autoHideEnabled != value)
                {
                    this._autoHideEnabled = value;
                    if (value)
                    {
                        if (this.AutoHideTimer.Interval == 0)
                        {
                            this.Hide();
                        }
                        else
                        {
                            this.AutoHideTimer.Start();
                        }
                    }
                    base.FirePropertyChanged("AutoHideEnabled");
                }
            }
        }

        public Microsoft.MediaCenter.UI.Timer AutoHideTimer
        {
            get
            {
                return this._autoHideTimer;
            }
        }

        public Microsoft.MediaCenter.UI.ICommand CommandClearOverlays
        {
            get
            {
                return this._commandClearOverlays;
            }
            set
            {
                this._commandClearOverlays = value;
                base.FirePropertyChanged("CommandClearOverlays");
            }
        }

        public virtual Microsoft.MediaCenter.UI.ICommand CommandNextSlide
        {
            get
            {
                if (this._commandNextSlide == null)
                {
                    this._commandNextSlide = new Microsoft.MediaCenter.UI.Command();
                    this._commandNextSlide.Invoked += new EventHandler(this.OnNavigatedNextSlide);
                }
                return this._commandNextSlide;
            }
        }

        public Microsoft.MediaCenter.UI.ICommand CommandPopOverlay
        {
            get
            {
                return this._commandPopOverlay;
            }
            set
            {
                if (this._commandPopOverlay != null)
                {
                    this._commandPopOverlay.Invoked -= new EventHandler(this.OnCommandPopOverlay);
                }
                this._commandPopOverlay = value;
                if (this._commandPopOverlay != null)
                {
                    this._commandPopOverlay.Invoked += new EventHandler(this.OnCommandPopOverlay);
                }
                base.FirePropertyChanged("CommandPopOverlay");
            }
        }

        public virtual Microsoft.MediaCenter.UI.ICommand CommandPrevSlide
        {
            get
            {
                if (this._commandPrevSlide == null)
                {
                    this._commandPrevSlide = new Microsoft.MediaCenter.UI.Command();
                    this._commandPrevSlide.Invoked += new EventHandler(this.OnNavigatedPreviousSlide);
                }
                return this._commandPrevSlide;
            }
        }

        public Microsoft.MediaCenter.UI.ICommand CommandPushOverlay
        {
            get
            {
                return this._commandPushOverlay;
            }
            set
            {
                if (this._commandPushOverlay != null)
                {
                    this._commandPushOverlay.Invoked -= new EventHandler(this.OnCommandPushOverlay);
                }
                this._commandPushOverlay = value;
                if (this._commandPushOverlay != null)
                {
                    this._commandPushOverlay.Invoked += new EventHandler(this.OnCommandPushOverlay);
                }
                base.FirePropertyChanged("CommandPushOverlay");
            }
        }

        public string Context
        {
            get
            {
                return this._context;
            }
            set
            {
                if (value != this._context)
                {
                    this._context = value;
                    if ((this._context != null) && (this.Finder != null))
                    {
                        this._blueprint = this.Finder.Lookup(this._context);
                    }
                    base.FirePropertyChanged("Context");
                }
            }
        }

        public SlideBlueprint DefaultSlide
        {
            get
            {
                return this._defaultSlide;
            }
            set
            {
                this._defaultSlide = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return this._enabled;
            }
            set
            {
                if (this._enabled != value)
                {
                    this._enabled = value;
                    if (!this._enabled)
                    {
                        this.Hide();
                    }
                    base.FirePropertyChanged("Enabled");
                }
            }
        }

        public SlideMarkupFinder Finder
        {
            get
            {
                return this._finder;
            }
            set
            {
                if (this._finder != null)
                {
                    this._finder.StateChanged -= new SlideStateEventHandler(this.OnFinderStateChanged);
                    this._finder = null;
                }
                this._finder = value;
                if (this._finder != null)
                {
                    this._finder.StateChanged += new SlideStateEventHandler(this.OnFinderStateChanged);
                }
                base.FirePropertyChanged("Finder");
            }
        }

        public virtual Microsoft.MediaCenter.UI.IModelItem Model
        {
            get
            {
                return this._model;
            }
            set
            {
                this._model = value;
                this.Context = this.BuildUIContext();
                base.FirePropertyChanged("Model");
            }
        }

        public NavigationDirection NavigateAwayDirection
        {
            get
            {
                return this._navigateAwayDirection;
            }
            private set
            {
                this._navigateAwayDirection = value;
            }
        }

        public NavigationDirection NavigateToDirection
        {
            get
            {
                return this._navigateToDirection;
            }
            private set
            {
                this._navigateToDirection = value;
            }
        }

        public Guid OverlayId
        {
            get
            {
                return this._id;
            }
        }

        public object StateObject
        {
            get
            {
                return this.Model;
            }
            set
            {
                this.Model = value as Microsoft.MediaCenter.UI.IModelItem;
            }
        }

        public string UIName
        {
            get
            {
                this.Initialize();
                SlideDeckBlueprint blueprint = this.Finder.Lookup(this.Context);
                if (blueprint == null)
                {
                    throw new InvalidOperationException("SlideDeck context lookup failed!");
                }
                return blueprint.UIName;
            }
        }

        public bool Visible
        {
            get
            {
                return this._visible;
            }
        }
    }

    public interface INavigationEventSink
    {
        // Methods
        void OnNavigatedAway(object sender, EventArgs<NavigationDirection> e);
        void OnNavigatedTo(object sender, EventArgs<NavigationDirection> e);

        // Properties
        NavigationDirection NavigateAwayDirection { get; }
        NavigationDirection NavigateToDirection { get; }
    }

    public interface INavigationEventSource
    {
        // Events
        event EventHandler<EventArgs<NavigationDirection>> NavigatedAway;
        event EventHandler<EventArgs<NavigationDirection>> NavigatedTo;
    }

    public class EventArgs<A> : EventArgs
    {
        // Fields
        private A _value;

        // Methods
        public EventArgs(A value)
        {
            this._value = value;
        }

        // Properties
        public A Value
        {
            get
            {
                return this._value;
            }
        }
    }


    public class ContextObjectStateEventArgs : EventArgs
    {
        // Fields
        private ContextObjectState _state;

        // Methods
        public ContextObjectStateEventArgs(ContextObjectState state)
        {
            this._state = state;
        }

        // Properties
        public ContextObjectState State
        {
            get
            {
                return this._state;
            }
        }
    }

    public enum ContextObjectState
    {
        NotInitialized,
        Ready,
        Loading,
        ProgressUpdated,
        Interactive,
        Denial,
        DenialWithdrawn
    }
}
