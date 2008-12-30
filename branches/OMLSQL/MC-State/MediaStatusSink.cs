using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using Microsoft.Win32;

using OMLEngine;
using System.Diagnostics;

namespace OML.MceState
{
    #region -- Com Interfaces --
    [ComImport, Guid("075FC453-F236-41DA-B90D-9FBB8BBDC101"), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMediaStatusSink
    {
        [DispId(1)]
        void Initialize();
        [DispId(2)]
        IMediaStatusSession CreateSession();
    }

    [ComImport, Guid("A70D81F2-C9D2-4053-AF0E-CDEA39BDD1AD"), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMediaStatusSession
    {
        [DispId(1)]
        void MediaStatusChange(MEDIASTATUSPROPERTYTAG[] Tags, object[] Properties);
        [DispId(2)]
        void Close();
    }
    #endregion

    #region -- Enums --
    public enum MEDIASTATUSPROPERTYTAG
    {
        // Fields
        MSPROPTAG_Application = 0xf001,
        MSPROPTAG_ArtistName = 0x2018,
        MSPROPTAG_CallingPartyName = 0x2029,
        MSPROPTAG_CallingPartyNumber = 0x2028,
        MSPROPTAG_CD = 0x2002,
        MSPROPTAG_CurrentPicture = 0x201b,
        MSPROPTAG_DVD = 0x2001,
        MSPROPTAG_Ejecting = 0x1010,
        MSPROPTAG_Error = 0x100f,
        MSPROPTAG_FF1 = 0x100a,
        MSPROPTAG_FF2 = 0x100b,
        MSPROPTAG_FF3 = 0x100c,
        MSPROPTAG_FS_DVD = 0x2010,
        MSPROPTAG_FS_Guide = 0x2011,
        MSPROPTAG_FS_Home = 0x200e,
        MSPROPTAG_FS_Music = 0x2012,
        MSPROPTAG_FS_Photos = 0x2013,
        MSPROPTAG_FS_Radio = 0x2025,
        MSPROPTAG_FS_RecordedShows = 0x2015,
        MSPROPTAG_FS_TV = 0x200f,
        MSPROPTAG_FS_Unknown = 0x2016,
        MSPROPTAG_FS_Videos = 0x2014,
        MSPROPTAG_GuideLoaded = 0x201d,
        MSPROPTAG_MediaName = 0x2017,
        MSPROPTAG_MediaTime = 0x2007,
        MSPROPTAG_MediaTypes = 0x2000,
        MSPROPTAG_MSASPrivateTags = 0xf000,
        MSPROPTAG_Mute = 0x1000,
        MSPROPTAG_Next = 0x100d,
        MSPROPTAG_NextFrame = 0x2021,
        MSPROPTAG_ParentalAdvisoryRating = 0x202a,
        MSPROPTAG_Pause = 0x1002,
        MSPROPTAG_PhoneCall = 0x2027,
        MSPROPTAG_Photos = 0x201a,
        MSPROPTAG_Play = 0x1001,
        MSPROPTAG_Prev = 0x100e,
        MSPROPTAG_PrevFrame = 0x2022,
        MSPROPTAG_PVR = 0x2003,
        MSPROPTAG_Radio = 0x2023,
        MSPROPTAG_RadioFrequency = 0x2024,
        MSPROPTAG_Recording = 0x1006,
        MSPROPTAG_RepeatSet = 0x1005,
        MSPROPTAG_RequestForTuner = 0x202b,
        MSPROPTAG_Rewind1 = 0x1007,
        MSPROPTAG_Rewind2 = 0x1008,
        MSPROPTAG_Rewind3 = 0x1009,
        MSPROPTAG_Shuffle = 0x1004,
        MSPROPTAG_SlowMotion1 = 0x201e,
        MSPROPTAG_SlowMotion2 = 0x201f,
        MSPROPTAG_SlowMotion3 = 0x2020,
        MSPROPTAG_Stop = 0x1003,
        MSPROPTAG_StreamingContentAudio = 0x2004,
        MSPROPTAG_StreamingContentVideo = 0x2005,
        MSPROPTAG_TitleNumber = 0x200c,
        MSPROPTAG_TotalTracks = 0x2009,
        MSPROPTAG_TrackDuration = 0x200a,
        MSPROPTAG_TrackName = 0x2019,
        MSPROPTAG_TrackNumber = 0x2008,
        MSPROPTAG_TrackTime = 0x200b,
        MSPROPTAG_TransitionTime = 0x201c,
        MSPROPTAG_TVTuner = 0x2006,
        MSPROPTAG_Unknown = 0,
        MSPROPTAG_Visualization = 0x2026,
        MSPROPTAG_Volume = 0x200d
    }

    public enum SessionState
    {
        /// <summary>
        /// CD playback has started.
        /// </summary>
        CD,
        /// <summary>
        /// DVD playback has started.
        /// </summary>
        DVD,
        /// <summary>
        /// TV has started.
        /// </summary>
        TVTuner,
        /// <summary>
        /// Starting a recorded show.
        /// </summary>
        PVR,
        /// <summary>
        /// Radio or radio frequency scanning has started.
        /// </summary>
        Radio,
        /// <summary>
        /// Visualization has started or stopped.
        /// </summary>
        Visualization,
        /// <summary>
        /// Picture was selected.
        /// </summary>
        Photos,
        /// <summary>
        /// Video content being played back from the hard disk.
        /// </summary>
        StreamingContentAudio,
        /// <summary>
        /// Audio content being played back from the hard disk.
        /// </summary>
        StreamingContentVideo,
        /// <summary>
        /// Incoming phone call.
        /// </summary>
        PhoneCall,
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown,
        /// <summary>
        /// None.
        /// </summary>
        None,
        Recording
    }

    /// <summary>
    /// The current global session state of Media Center.
    /// </summary>
    public enum GlobalSessionState
    {
        /// <summary>
        /// Navigating to Media Center Start Page.
        /// </summary>
        Home,
        /// <summary>
        /// Navigating to My TV, or the TV inset was selected.
        /// </summary>
        TV,
        /// <summary>
        /// Navigating to My Music, or the music inset was selected.
        /// </summary>
        Music,
        /// <summary>
        /// Navigating to My Videos, or the video inset was selected.
        /// </summary>
        Videos,
        /// <summary>
        /// Navigating to My Pictures.
        /// </summary>
        Photos,
        /// <summary>
        /// Navigating to My Radio.
        /// </summary>
        Radio,
        /// <summary>
        /// Navigating to Recorded Shows or scheduled recording pages, or the recorded TV inset was selected.
        /// </summary>
        RecordedShows,
        /// <summary>
        /// Navigating to Play DVD, or the DVD inset was selected.
        /// </summary>
        DVD,
        /// <summary>
        /// Navigating to Guide.
        /// </summary>
        Guide,
        /// <summary>
        /// Unknown Media Center status.
        /// </summary>
        Unknown,
        Unassigned
    }

    /// <summary>
    /// The current transport function of the <see cref="SessionState"/> state.
    /// </summary>
    public enum Transport
    {
        /// <summary>
        /// Current media was stopped. This status is sent once when stop is selected, and once when the media has stopped.
        /// </summary>
        Stop,
        /// <summary>
        /// Starting to play the current media.
        /// </summary>
        Play,
        /// <summary>
        /// Pausing the media.
        /// </summary>
        Pause,
        /// <summary>
        /// Rewinding at low speed.
        /// </summary>
        Rewind1,
        /// <summary>
        /// Rewinding at medium speed.
        /// </summary>
        Rewind2,
        /// <summary>
        /// Rewinding at high speed.
        /// </summary>
        Rewind3,
        /// <summary>
        /// Fast forwarding at low speed.
        /// </summary>
        FastForward1,
        /// <summary>
        /// Fast forwarding at medium speed.
        /// </summary>
        FastForward2,
        /// <summary>
        /// Fast forwarding at high speed.
        /// </summary>
        FastForward3,
        /// <summary>
        /// Playing at low slow-motion speed.
        /// </summary>
        SlowMotion1,
        /// <summary>
        /// Playing at medium slow-motion speed.
        /// </summary>
        SlowMotion2,
        /// <summary>
        /// Playing at high slow-motion speed.
        /// </summary>
        SlowMotion3,
        /// <summary>
        /// Skipping backward to the previous track or chapter.
        /// </summary>
        Prev,
        /// <summary>
        /// Skipping backward to the previous frame while paused.0
        /// </summary>
        PrevFrame,
        /// <summary>
        /// Skipping forward to next track or chapter.
        /// </summary>
        Next,
        /// <summary>
        /// Skipping forward to next frame while paused.
        /// </summary>
        NextFrame,
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown
    }
    #endregion

    #region -- Com Implementations --
    [Guid("ECCED884-19CF-4179-0002-879756E3BC46"), ComVisible(true)]
    public class MsasSink : IMediaStatusSink, IDisposable
    {
        #region -- Members --
        int _nextID = 0;
        bool _disposed = false;
        static List<MceSession> sMceSessions = new List<MceSession>();
        static Dictionary<long, Session> sSessionsByID = new Dictionary<long, Session>();
        //static MemoryFile mmf;
        //static bool logSessions = false;
        #endregion

        #region -- Com registration stuff --
        [ComRegisterFunction]
        public static void RegistrationMethod(Type type)
        {
            WriteToLog(EventLogEntryType.Information, "[MsasSink] ComRegisterMethod: {0}, {1}, I: {2}", type, type.Assembly.GetName().Version, type == typeof(MsasSink));
            try
            {

                if (type == typeof(MsasSink))
                {
                    using (Registry.ClassesRoot.CreateSubKey(@"CLSID\{ECCED884-19CF-4179-0002-879756E3BC46}\Implemented Categories\{62C8FE65-4EBB-45E7-B440-6E39B2CDBF29}"))
                    { }
                    using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(@"CLSID\{ECCED884-19CF-4179-0002-879756E3BC46}\Implemented Categories\{FCB0C2A3-9747-4c95-9d02-820AFEDEF13F}"))
                        key.SetValue(null, "OML Media Status Sink", RegistryValueKind.String);
                }
            }
            catch (Exception ex)
            {
                WriteToLog(EventLogEntryType.Error, "[MsasSink] ComRegisterMethod: Exception: {0}", ex);
            }
        }

        [ComUnregisterFunction]
        public static void UnRegistrationMethod(Type type)
        {
            try
            {
                WriteToLog(EventLogEntryType.Information, "[MsasSink] UnRegistrationMethod: {0}, I: {1}", type, type == typeof(MsasSink));
                if (type == typeof(MsasSink))
                {
                    Registry.ClassesRoot.DeleteSubKey(@"CLSID\{ECCED884-19CF-4179-0002-879756E3BC46}\Implemented Categories\{62C8FE65-4EBB-45E7-B440-6E39B2CDBF29}");
                    Registry.ClassesRoot.DeleteSubKey(@"CLSID\{ECCED884-19CF-4179-0002-879756E3BC46}\Implemented Categories\{FCB0C2A3-9747-4c95-9d02-820AFEDEF13F}");
                }
            }
            catch (Exception ex)
            {
                WriteToLog(EventLogEntryType.Error, "[MsasSink] UnRegistrationMethod: Exception: {0}", ex);
            }
        }
        #endregion

        internal static void WriteToLog(EventLogEntryType type, string msg, params object[] args)
        {
            const string source = @"OMLMCState";

            if (!EventLog.SourceExists(source))
                EventLog.CreateEventSource(source, string.Empty);

            msg = string.Format(msg, args);
            EventLog evt = new EventLog(string.Empty) { Source = source };
            evt.WriteEntry(msg + ": " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString(), type);
            Utilities.DebugLine(msg);
        }

        public void Initialize()
        {
            WriteToLog(EventLogEntryType.Information, "[MsasSink] Initialize");
            try
            {
                //mmf = new MemoryFile("MceStateFile", 16384);
                //Utilities.WriteLog("MceStateFile Initialized");
                //Durrant.Common.BaseSettings bs = new Durrant.Common.BaseSettings();
                //logSessions = bs.DebugMceState;
            }
            catch // (Exception ex)
            {
                //Utilities.WriteError(ex.ToString());
            }
        }

        public IMediaStatusSession CreateSession()
        {
            WriteToLog(EventLogEntryType.Information, "[MsasSink] CreateSession({0})", _nextID);
            Session newSession = new Session(_nextID);
            lock (sSessionsByID)
            {
                sSessionsByID[_nextID] = newSession;
                sMceSessions.Add(newSession.MediaSession);
            }
            _nextID++;
            return newSession;
        }

        public static void RemoveSession(int id)
        {
            lock (sSessionsByID)
            {
                sMceSessions.Remove(sSessionsByID[id].MediaSession);
                sSessionsByID.Remove(id);
            }
            WriteFile();
        }

        public static void WriteFile()
        {
            //if (mmf != null)
            //{
            //    try
            //    {
            //        byte[] sessBuffer = Serializer.ObjToBytes(sessToSerialize);
            //        if (sessBuffer != null)
            //            mmf.Write(sessBuffer);
            //        if (logSessions)
            //            foreach (MceSession s in sessToSerialize)
            //                FileLogger.Log(s.ToString());
            //    }
            //    catch (Exception ex)
            //    {
            //        Utilities.WriteError(ex.ToString());
            //    }
            //}
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources.

                //if (disposing)
                //    if (mmf != null)
                //        mmf.Dispose();

                // Call the appropriate methods to clean up 
                // unmanaged resources here.
                // If disposing is false, 
                // only the following code is executed.           
            }
            _disposed = true;
        }

        #endregion
    }

    [Guid("ECCED884-19CF-4179-0003-879756E3BC46"), Serializable(), ComVisible(true)]
    public class Session : IMediaStatusSession
    {
        #region -- Members --
        protected int _id;
        MceSession _session = null;
        #endregion

        public MceSession MediaSession { get { return _session; } }

        public Session(int id)
        {
            MsasSink.WriteToLog(EventLogEntryType.Information, "[Session] Session({0})", id);
            _session = new MceSession(id);
            _id = id;
        }

        public void MediaStatusChange(MEDIASTATUSPROPERTYTAG[] tags, object[] properties)
        {
            MsasSink.WriteToLog(EventLogEntryType.Information, "[Session] MediaStatusChange({0})", _id);
            _session.MediaStatusChange(tags, properties);
            MsasSink.WriteFile();
        }

        public void Close()
        {
            MsasSink.WriteToLog(EventLogEntryType.Information, "[Session] Close({0})", _id);
            MsasSink.RemoveSession(_id);
        }

    }
    #endregion

    #region -- MceSession --
    [Serializable()]
    public class MceSession : IComparable
    {
        #region -- Members --
        int _id;
        Transport _currentTransport;
        SessionState _currentSession;
        string _mediaName = "";
        int _mediaTime;
        string _artistName = "";
        int _titleNumber;
        string _trackName = "";
        int _trackTime;
        int _trackDuration;
        int _trackNumber;
        int _totalTracks;
        string _callingPartyName;
        int _callingPartyNumber;
        bool _shuffle;
        bool _repeat;
        bool _visualization;
        int transitionTime;
        string _parentalAdvisoryRating;
        string _radioFrequency;
        string _currentPicture;
        bool _recording;
        string _tvShow;
        string _albumName;
        string _uniqueId;

        //global properties
        GlobalSessionState _currentGlobalSession = GlobalSessionState.Unassigned;
        bool _mute;
        int _volume;
        bool _guideLoaded;
        bool _ejecting;

        long _lastWrite;
        #endregion

        #region -- Public Properties --
        public string UniqueId { get { return _uniqueId; } }
        public long LastWrite { get { return _lastWrite; } }
        public bool IsGlobalSession { get { return _currentGlobalSession != GlobalSessionState.Unassigned; } }

        /// <summary>
        /// Gets or sets the current transport function.
        /// </summary>
        public Transport CurrentTransport
        {
            get { return this._currentTransport; }
            set { this._currentTransport = value; }
        }

        /// <summary>
        /// Gets or sets the current playback state.
        /// </summary>
        public SessionState CurrentSession
        {
            get { return this._currentSession; }
            set { this._currentSession = value; }
        }

        /// <summary>
        /// Gets or sets the name of the current media. This status is sent when the media starts.
        /// </summary>
        public string MediaName
        {
            get { return this._mediaName; }
            set
            {
//				this.uniqueId = Guid.NewGuid().ToString();

                this._mediaName = value;

                if (this.CurrentSession == SessionState.TVTuner ||
                    this.CurrentSession == SessionState.PVR)
                    this._tvShow = value;
                else
                    this._albumName = value;
            }
        }

        /// <summary>
        /// Length of the current media.
        /// </summary>
        public int MediaTime
        {
            get { return this._mediaTime; }
            set { this._mediaTime = value; }
        }

        /// <summary>
        /// Gets or sets the name of the artist associated with the current track. This status is sent when the track starts.
        /// </summary>
        public string ArtistName
        {
            get { return this._artistName; }
            set { this._artistName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the current track.
        /// </summary>
        public int TitleNumber
        {
            get { return this._titleNumber; }
            set { this._titleNumber = value; }
        }

        /// <summary>
        /// Gets or sets the name of the current track.
        /// </summary>
        public string TrackName
        {
            get { return this._trackName; }
            set { this._trackName = value; }
        }

        /// <summary>
        /// Gets or sets the elapsed time, in seconds, within the current track. This status is incremented and sent every second.
        /// </summary>
        public int TrackTime
        {
            get { return this._trackTime; }
            set { this._trackTime = value; }
        }

        /// <summary>
        /// Gets or sets the duration, in seconds, of the current track.
        /// </summary>
        public int TrackDuration
        {
            get { return this._trackDuration; }
            set { this._trackDuration = value; }
        }

        /// <summary>
        /// Gets or sets the CD track number, DVD chapter number, or TV channel number.
        /// </summary>
        public int TrackNumber
        {
            get { return this._trackNumber; }
            set { this._trackNumber = value; }
        }

        /// <summary>
        /// Gets or sets the total number of tracks on the current media.
        /// </summary>
        public int TotalTracks
        {
            get { return this._totalTracks; }
            set { this._totalTracks = value; }
        }

        /// <summary>
        /// Gets or sets the name of the caller making the incoming phone call.
        /// </summary>
        public string CallingPartyName
        {
            get { return this._callingPartyName; }
            set { this._callingPartyName = value; }
        }

        /// <summary>
        /// Gets or sets the phone number of the caller making the incoming phone call. 
        /// </summary>
        public int CallingPartyNumber
        {
            get { return this._callingPartyNumber; }
            set { this._callingPartyNumber = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if shuffle was enable or disabled.
        /// </summary>
        public bool Shuffle
        {
            get { return this._shuffle; }
            set { this._shuffle = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating repeat was enabled or disabled.
        /// </summary>
        public bool Repeat
        {
            get { return this._repeat; }
            set { this._repeat = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if Visualization has started or stopped.
        /// </summary>
        public bool Visualization
        {
            get { return this._visualization; }
            set { this._visualization = value; }
        }

        /// <summary>
        /// Duration, in seconds, of transitions between slides in a slide show.
        /// </summary>
        public int TransitionTime
        {
            get { return this.transitionTime; }
            set { this.transitionTime = value; }
        }

        /// <summary>
        /// Parental advisory rating of current show.
        /// </summary>
        public string ParentalAdvisoryRating
        {
            get { return this._parentalAdvisoryRating; }
            set { this._parentalAdvisoryRating = value; }
        }

        /// <summary>
        /// Gets or sets the current frequency being listened to, is sent when radio starts playing the frequency.
        /// </summary>
        public string RadioFrequency
        {
            get { return this._radioFrequency; }
            set { this._radioFrequency = value; }
        }

        /// <summary>
        /// Gets or sets the name of current picture, is sent when media starts.
        /// </summary>
        public string CurrentPicture
        {
            get { return this._currentPicture; }
            set { this._currentPicture = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if recording has started.
        /// </summary>
        public bool Recording
        {
            get { return this._recording; }
            set { this._recording = value; }
        }

        //global properties
        /// <summary>
        /// Gets or sets the current Media Center selection.
        /// </summary>
        public GlobalSessionState CurrentGlobalSession
        {
            get { return this._currentGlobalSession; }
            set { this._currentGlobalSession = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if muting was turned on or off. 
        /// </summary>
        public bool Mute
        {
            get { return this._mute; }
            set { this._mute = value; }
        }

        /// <summary>
        /// Gets or sets the current volume level. This status is sent when the volume changes.
        /// </summary>
        public int Volume
        {
            get { return this._volume; }
            set { this._volume = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating a new guide was downloaded.
        /// </summary>
        public bool GuideLoaded
        {
            get { return this._guideLoaded; }
            set { this._guideLoaded = value; }
        }

        /// <summary>
        /// Disk was ejected from the CD/DVD drive. This status is sent only if Media Center was playing the disk.
        /// </summary>
        public bool Ejecting
        {
            get { return this._ejecting; }
            set { this._ejecting = value; }
        }
        #endregion

        public MceSession(int id)
        {
            this._id = id;
            this.CurrentSession = SessionState.None;
        }

        void Clear()
        {
            if (CurrentSession == SessionState.StreamingContentAudio)
            {
                MediaName = string.Empty;
                _trackDuration = 0;
                _trackName = string.Empty;
                _artistName = string.Empty;
            }
        }

        #region IMediaStatusSession

        public void MediaStatusChange(MEDIASTATUSPROPERTYTAG[] tags, object[] properties)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                MEDIASTATUSPROPERTYTAG tag = tags[i];
                object property = properties[i];

                switch (tag)
                {
                    //set the current transport control
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_Play:
                        this.CurrentTransport = Transport.Play;
                        this._uniqueId = Guid.NewGuid().ToString();
//						Console.WriteLine(uniqueId);
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_Pause:
                        this.CurrentTransport = Transport.Pause;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_Stop:
                        this.CurrentTransport = Transport.Stop;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_Rewind1:
                        this.CurrentTransport = Transport.Rewind1;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_Rewind2:
                        this.CurrentTransport = Transport.Rewind2;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_Rewind3:
                        this.CurrentTransport = Transport.Rewind3;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_FF1:
                        this.CurrentTransport = Transport.FastForward1;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_FF2:
                        this.CurrentTransport = Transport.FastForward2;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_FF3:
                        this.CurrentTransport = Transport.FastForward3;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_Prev:
                        this.CurrentTransport = Transport.Prev;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_PrevFrame:
                        this.CurrentTransport = Transport.PrevFrame;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_Next:
                        this.CurrentTransport = Transport.Next;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_NextFrame:
                        this.CurrentTransport = Transport.NextFrame;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_SlowMotion1:
                        this.CurrentTransport = Transport.SlowMotion1;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_SlowMotion2:
                        this.CurrentTransport = Transport.SlowMotion2;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_SlowMotion3:
                        this.CurrentTransport = Transport.SlowMotion3;
                        break;
                    // set the global session state
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_FS_DVD:
                        this.CurrentGlobalSession = GlobalSessionState.DVD;
                        this.CurrentSession = SessionState.DVD;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_FS_Guide:
                        this.CurrentGlobalSession = GlobalSessionState.Guide;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_FS_Home:
                        this.CurrentGlobalSession = GlobalSessionState.Home;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_FS_Music:
                        this.CurrentGlobalSession = GlobalSessionState.Music;
                        this.CurrentSession = SessionState.StreamingContentAudio;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_FS_Photos:
                        this.CurrentGlobalSession = GlobalSessionState.Photos;
                        this.CurrentSession = SessionState.Photos;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_FS_Radio:
                        this.CurrentGlobalSession = GlobalSessionState.Radio;
                        this.CurrentSession = SessionState.Radio;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_FS_RecordedShows:
                        this.CurrentGlobalSession = GlobalSessionState.RecordedShows;
                        this.CurrentSession = SessionState.PVR;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_FS_TV:
                        this.CurrentGlobalSession = GlobalSessionState.TV;
                        this.CurrentSession = SessionState.TVTuner;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_FS_Unknown:
                        this.CurrentGlobalSession = GlobalSessionState.Unknown;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_FS_Videos:
                        this.CurrentGlobalSession = GlobalSessionState.Videos;
                        this.CurrentSession = SessionState.StreamingContentVideo;
                        break;

                    // set other stuff
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_Ejecting:
                        this.Ejecting = true;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_Mute:
                        this.Mute = System.Convert.ToBoolean(property);
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_Volume:
                        this.Volume = System.Convert.ToInt32(property);
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_GuideLoaded:
                        this.GuideLoaded = true;
                        break;

                    // set the current session state
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_CD:
                        this.CurrentSession = SessionState.CD;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_DVD:
                        this.CurrentSession = SessionState.DVD;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_PVR:
                        this.CurrentSession = SessionState.PVR;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_TVTuner:
                        this.CurrentSession = SessionState.TVTuner;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_Photos:
                        this.CurrentSession = SessionState.Photos;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_Radio:
                        this.CurrentSession = SessionState.Radio;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_StreamingContentAudio:
                        this.CurrentSession = SessionState.StreamingContentAudio;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_StreamingContentVideo:
                        this.CurrentSession = SessionState.StreamingContentVideo;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_PhoneCall:
                        this.CurrentSession = SessionState.PhoneCall;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_Unknown:
                        this.CurrentSession = SessionState.Unknown;
                        break;

                    // Caller ID
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_CallingPartyName:
                        this.CallingPartyName = System.Convert.ToString(property);
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_CallingPartyNumber:
                        this.CallingPartyNumber = System.Convert.ToInt32(property);
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_Recording:
                        this.CurrentSession = SessionState.Recording;
                        this.Recording = true;
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_ArtistName:
                        this.ArtistName = System.Convert.ToString(property);
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_MediaName:
                        this.MediaName = System.Convert.ToString(property);
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_MediaTime:
                        this.MediaTime = System.Convert.ToInt32(property);
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_TitleNumber:
                        this.TitleNumber = System.Convert.ToInt32(property);
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_TrackName:
                        this.TrackName = System.Convert.ToString(property);
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_TrackTime:
                        this.TrackTime = System.Convert.ToInt32(property);
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_TrackDuration:
                        this.TrackDuration = System.Convert.ToInt32(property);
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_TotalTracks:
                        this.TotalTracks = System.Convert.ToInt32(property);
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_TrackNumber:
                        this.TrackNumber = System.Convert.ToInt32(property);
                        Clear();
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_Visualization:
                        this.Shuffle = System.Convert.ToBoolean(property);
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_RepeatSet:
                        this.Shuffle = System.Convert.ToBoolean(property);
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_Shuffle:
                        this.Shuffle = System.Convert.ToBoolean(property);
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_TransitionTime:
                        this.TransitionTime = System.Convert.ToInt32(property);
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_ParentalAdvisoryRating:
                        this.ParentalAdvisoryRating = System.Convert.ToString(property);
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_RadioFrequency:
                        this.RadioFrequency = System.Convert.ToString(property);
                        break;
                    case MEDIASTATUSPROPERTYTAG.MSPROPTAG_CurrentPicture:
                        this.CurrentPicture = System.Convert.ToString(property);
                        break;
                }
            }
            _lastWrite = DateTime.Now.Ticks;
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is MceSession)
            {
                MceSession temp = ((MceSession)(obj));
                return LastWrite.CompareTo(temp.LastWrite);
            }
            throw new ArgumentException("object is not a MceSession");
        }

        #endregion

        public override string ToString()
        {
            Type typeInfo = this.GetType();
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            PropertyInfo[] props = typeInfo.GetProperties(flags);
            StringBuilder sb = new StringBuilder();

            foreach (PropertyInfo p in props)
                sb.AppendFormat("{0}: {1} | ", p.Name, p.GetValue(this, null));
            return sb.ToString();
        }
    }
    #endregion
}
