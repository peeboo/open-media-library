using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.UI;
using System.Collections;
using System.Runtime.InteropServices;
using System.Globalization;

namespace Library.Code.V3
{
    public enum DiscManagerSortType
    {
        Name,
        Type
    }

    public class MediaChangerManagerPage : ModelItem
    {
        private void DetermineShowErrorMessage()
        {
            if (this.discArray.Count == 0)
            {
                //no changer detected
                if (!OMLApplication.Current.MediaChangers.HasMediaChangers)
                    this.ErrorMessage = "OML was unable to detect a media changer. If you have a media changer installed please verify that you do not have any other Media Center sessions open.";
                //changer is empty
                else
                    this.ErrorMessage = "There are no discs in the changer";
                    this.ShowErrorMessage = true;
            }
        }

        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                FirePropertyChanged("ErrorMessage");
            }
        }

        private Boolean showErrorMessage = false;
        public Boolean ShowErrorMessage
        {
            get { return showErrorMessage; }
            set
            {
                showErrorMessage = value;
                FirePropertyChanged("ShowErrorMessage");
            }
        }

        private ICommand selectedDisc;
        public ICommand SelectedDisc
        {
            get
            {
                return selectedDisc;
            }
            set
            {
                if (selectedDisc != value)
                {
                    selectedDisc = value;
                    FirePropertyChanged("SelectedDisc");
                }
            }
        }

        private IList managerCommands;
        public IList ManagerCommands
        {
            get { return managerCommands; }
            set
            {
                if (managerCommands != value)
                {
                    managerCommands = value;
                    FirePropertyChanged("ManagerCommands");
                }
            }
        }

        private Choice sortCommands;
        public Choice SortCommands
        {
            get
            {
                return this.sortCommands;
            }
            set 
            {
                this.sortCommands = value;
            }
        }

        public DiscManagerSortType DiscManagerSort;
 
        private void SortList()
        {
            if (this.discArray != null)
            {
                switch (this.DiscManagerSort)
                {
                    case DiscManagerSortType.Name:
                        //Array.Sort(this.discArray, new CompareMediaChangerManagerDiscName());
                        this.discArray.Sort(new CompareMediaChangerManagerDiscName());
                        break;

                    case DiscManagerSortType.Type:
                        //Array.Sort(this.discArray, new CompareMediaChangerManagerDiscType());
                        this.discArray.Sort(new CompareMediaChangerManagerDiscType());
                        break;
                }
                this.RefreshList();
            }
        }

        private void RefreshList()
        {
            FirePropertyChanged("DiscArray");
        }

        /// <summary>
        /// detecting changes
        /// </summary>
        /// <returns></returns>
        public bool IsDirty()
        {
            if (this.enableMediaChangers.Value != Properties.Settings.Default.MediaChangersEnabled)
                return true;
            if (this.detectMediaChangers.Value != Properties.Settings.Default.MediaChangersDetect)
                return true;
            if (this.retrieveMetaData.Value != Properties.Settings.Default.MediaChangersRetrieveMetaData)
                return true;
            return false;
        }

        /// <summary>
        /// saving settings
        /// </summary>
        public void Save()
        {
            Properties.Settings.Default.MediaChangersEnabled = this.enableMediaChangers.Value;
            Properties.Settings.Default.MediaChangersDetect = this.detectMediaChangers.Value;
            Properties.Settings.Default.MediaChangersRetrieveMetaData = this.retrieveMetaData.Value;
            Properties.Settings.Default.Save();
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
        private BooleanChoice enableMediaChangers;
        public BooleanChoice EnableMediaChangers
        {
            get
            {
                return this.enableMediaChangers;
            }
            set
            {
                this.enableMediaChangers = value;
                FirePropertyChanged("EnableMediaChangers");
            }
        }
        private BooleanChoice detectMediaChangers;
        public BooleanChoice DetectMediaChangers
        {
            get
            {
                return this.detectMediaChangers;
            }
            set
            {
                this.detectMediaChangers = value;
                FirePropertyChanged("DetectMediaChangers");
            }
        }
        private BooleanChoice retrieveMetaData;
        public BooleanChoice RetrieveMetaData
        {
            get
            {
                return this.retrieveMetaData;
            }
            set
            {
                this.retrieveMetaData = value;
                FirePropertyChanged("RetrieveMetaData");
            }
        }
        private Command manageChangers;
        public Command ManageChangers
        {
            get { return manageChangers; }
            set
            {
                if (manageChangers != value)
                {
                    manageChangers = value;
                    FirePropertyChanged("ManageChangers");
                }
            }
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

        private ArrayListDataSet discArray = new ArrayListDataSet();
        public ArrayListDataSet DiscArray
        {
            get { return discArray; }
            set
            {
                if (discArray != value)
                {
                    discArray = value;
                    FirePropertyChanged("DiscArray");
                }
            }
        }

        public MediaChangerManagerPage()
        {

            this.managerCommands = new ArrayListDataSet(this);

            //save command
            Command rescanCmd = new Command();
            rescanCmd.Description = "Rescan Discs";
            rescanCmd.Invoked += new EventHandler(rescanCmd_Invoked);
            this.managerCommands.Add(rescanCmd);

            this.sortCommands = new Choice(this);
            ArrayListDataSet sortSet = new ArrayListDataSet();

            Command sortByName=new Command(this);
            sortByName.Description="Sort by Name";
            sortSet.Add(sortByName);

            Command sortByType = new Command(this);
            sortByType.Description = "Sort by Type";
            sortSet.Add(sortByType);

            this.sortCommands.Options = sortSet;
            this.SortCommands.ChosenChanged += new EventHandler(SortCommands_ChosenChanged);

            
            this.enableMediaChangers = new BooleanChoice(this, "Enable support for media changers");
            this.enableMediaChangers.Value = Properties.Settings.Default.MediaChangersEnabled;
            this.detectMediaChangers = new BooleanChoice(this, "Detect new discs added to your changer");
            this.detectMediaChangers.Value = Properties.Settings.Default.MediaChangersDetect;
            this.retrieveMetaData = new BooleanChoice(this, "Retrieve MetaData from Microsoft");
            this.retrieveMetaData.Value = Properties.Settings.Default.MediaChangersRetrieveMetaData;
            this.manageChangers = new Command(this);
            this.manageChangers.Description = "Manage Discs";
            this.manageChangers.Invoked += delegate(object changerSender, EventArgs changerArgs)
            {
                Dictionary<string, object> manageProperties = new Dictionary<string, object>();
                manageProperties["Page"] = this;
                manageProperties["Application"] = Library.OMLApplication.Current;
                Library.OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_MediaChangerManagerPage", manageProperties);
            };

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

            this.discArray = new ArrayListDataSet();
            if (OMLApplication.Current.MediaChangers.HasMediaChangers)
            {
                foreach (DiscDataEx disc in OMLApplication.Current.MediaChangers.KnownDiscs)
                {
                    DiscCommand discItem = new DiscCommand(this);
                    discItem.Description = disc.VolumeLabel;
                    discItem.Disc = disc;
                    if (!string.IsNullOrEmpty(disc.Title))
                        discItem.Description = disc.Title;

                    discItem.Invoked += delegate(object discSender, EventArgs discArgs)
                    {
                        if(discSender is DiscCommand)
                        {
                            this.IsBusy = true;
                            Microsoft.MediaCenter.UI.Application.DeferredInvokeOnWorkerThread(beginEject, endEject, (object)discSender);
                            //((DiscCommand)discSender).Disc.Eject();
                            //this.updateDiscArray();
                        }
                    };
                    this.discArray.Add(discItem);
                }
            }
            
            this.DiscManagerSort = DiscManagerSortType.Name;
            this.SortList();
            //for (int i = 0; i < 20; ++i)
            //{
            //    Command c = new Command(this);
            //    c.Description = string.Format("This is a very long description of a Test Disc {0}", i.ToString());
            //    this.discArray.Add(c);
            //}
            this.DetermineShowErrorMessage();
        }

        private void beginEject(object discCommand)
        {
            ((DiscCommand)discCommand).Disc.Eject();
        }

        private void endEject(object discCommand)
        {
            if (!IsDisposed)
            {
                this.updateDiscArray();
                this.IsBusy = false;
            }
        }

        void rescanCmd_Invoked(object sender, EventArgs e)
        {
            DialogResult res = OMLApplication.Current.MediaCenterEnvironment.Dialog("Are you sure you want to update your disc library now? It may take several minutes to load and scan each new disc.", "UPDATE DISC LIBRARY", DialogButtons.Yes | DialogButtons.No, -1, true);
            if (res == DialogResult.Yes)
            {
                //save!
                //MediaChangeManagerHelper.RescanAllDiscs();
                this.IsBusy = true;
                Microsoft.MediaCenter.UI.Application.DeferredInvokeOnWorkerThread
                                (
                                 // Delegate to be invoked on background thread
                                 beginRescan,

                                 // Delegate to be invoked on app thread
                                 endRescan,

                                 // Parameter to be passed to both delegates, we don't need it
                                 null
                                 );
            }
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

        private void beginRescan(object obj)
        {
            //slow this thread down
            //ThreadPriority priority = Thread.CurrentThread.Priority;
            //Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            try
            {
                MediaChangeManagerHelper.RescanAllDiscs();
                //Microsoft.MediaCenter.UI.Application.DeferredInvoke(notifyRescanComplete, null);
            }
            finally
            {
                //
                // Reset our thread's priority back to its previous value
                //
                //Thread.CurrentThread.Priority = priority;
            }
        }

        private void notifyRescanComplete(object obj)
        {
            if (!IsDisposed)
            {
                this.updateDiscArray();
                this.IsBusy = false;
            }
        }


        private void endRescan(object obj)
        {
            if (!IsDisposed)
            {
                this.updateDiscArray();
                this.IsBusy = false;
            }
            this.DetermineShowErrorMessage();
        }

        void SortCommands_ChosenChanged(object sender, EventArgs e)
        {
            if(this.SortCommands.ChosenIndex==1)
                this.DiscManagerSort = DiscManagerSortType.Type;
            else
                this.DiscManagerSort = DiscManagerSortType.Name;
            this.SortList();
        }

        private void updateDiscArray()
        {
            ArrayListDataSet discArray = new ArrayListDataSet();
            if (OMLApplication.Current.MediaChangers.HasMediaChangers)
            {
                foreach (DiscDataEx disc in OMLApplication.Current.MediaChangers.KnownDiscs)
                {
                    DiscCommand discItem = new DiscCommand(this);
                    discItem.Description = disc.VolumeLabel;
                    discItem.Disc = disc;
                    if (!string.IsNullOrEmpty(disc.Title))
                        discItem.Description = disc.Title;

                    discItem.Invoked += delegate(object discSender, EventArgs discArgs)
                    {
                        if (discSender is DiscCommand)
                        {
                            ((DiscCommand)discSender).Disc.Eject();
                        }
                    };
                    discArray.Add(discItem);
                }
            }
            this.DiscArray = discArray;
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

        //private MediaChangeManager mediaChangers;
        public VirtualList Content
        {
            get { return content; }
            set
            {
                if (content != value)
                {
                    content = value;
                    FirePropertyChanged("Content");
                    if (content != null && content.Count > 0 && content[0] is ThumbnailCommand)
                    {
                        this.SelectedItem = (ThumbnailCommand)content[0];
                    }
                }
            }
        }


        private VirtualList content;

        public ThumbnailCommand SelectedItem
        {
            get
            {
                if (selectedItem != null)
                {
                    return selectedItem;
                }
                else
                {
                    return new ThumbnailCommand();
                }
            }
            set
            {
                if (selectedItem != value && value != null)
                {
                    selectedItem = value;
                    FirePropertyChanged("SelectedItem");
                }
            }
        }

        private ThumbnailCommand selectedItem = null;

        public ICommand SelectedItemCommand
        {
            get
            {
                if (selectedItemCommand != null)
                {
                    return selectedItemCommand;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (selectedItemCommand != value && value != null)
                {
                    selectedItemCommand = value;
                    FirePropertyChanged("SelectedItemCommand");
                }
            }
        }

        private ICommand selectedItemCommand = null;
    }

    public class MediaChangeManager : BaseModelItem
    {
        public bool NeedsFullScan
        {
            get
            {
                if (this.unknownDiscs.Count > 0)
                    return true;
                else
                    return false;
            }
        }
        public bool HasMediaChangers
        {
            get
            {
                if (this.changers.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public void ReScan()
        {
            //clear everything
            this.changers = new Collection<MediaChangerEx>();
            this.knownDiscs = new Collection<DiscDataEx>();
            this.unknownDiscs = new Collection<DiscDataEx>();
            this.Scan();
        }

        private void Scan()
        {
            this.changers = MediaChangeManagerHelper.GetChangers();
            //pop known/unknown discs
            foreach (MediaChangerEx changer in this.changers)
            {
                foreach (DiscDataEx disc in changer.Discs)
                {
                    if (disc.DiscType == DiscType.Unknown)
                        this.unknownDiscs.Add(disc);
                    else
                        this.knownDiscs.Add(disc);
                }
            }

            //let the gallery load first-this is somewhat hackish
            if (this.NeedsFullScan)
            {
                //Timer scanTimer = new Timer();
                //scanTimer.AutoRepeat = false;
                //scanTimer.Tick += new EventHandler(scanTimer_Tick);

                //scanTimer.Interval = 9000;
                //scanTimer.Enabled = true;
                //scanTimer.Start();
            }
        }

        void scanTimer_Tick(object sender, EventArgs e)
        {
            this.PromptForScan();
        }

        public void PromptForScan()
        {
            if (this.NeedsFullScan)
            {
                DialogResult res = OMLApplication.Current.MediaCenterEnvironment.Dialog("New discs have been detected in the disc changer. Do you want to update your disc library now? It may take several minutes to load and scan each new disc.", "NEW DISCS FOUND", DialogButtons.Yes | DialogButtons.No, -1, true);
                if (res == DialogResult.Yes)
                {
                    MediaChangeManagerHelper.RescanAllDiscs();
                }
            }
        }
        private void IdentificationScan()
        {

        }

        public MediaChangeManager()
        {
            this.changers = new Collection<MediaChangerEx>();
            this.knownDiscs = new Collection<DiscDataEx>();
            this.unknownDiscs = new Collection<DiscDataEx>();
            Microsoft.MediaCenter.UI.Application.Idle += new EventHandler(Application_Idle);
            this.Scan();
        }

        void Application_Idle(object sender, EventArgs e)
        {
            Microsoft.MediaCenter.UI.Application.Idle -= new EventHandler(Application_Idle);
            if (this.NeedsFullScan && Properties.Settings.Default.MediaChangersEnabled && Properties.Settings.Default.MediaChangersDetect)
                PromptForScan();
        }

        private Collection<MediaChangerEx> changers;
        public Collection<MediaChangerEx> Changers
        {
            get { return changers; }
        }

        //known discs are discs that have a dicsid
        private Collection<DiscDataEx> knownDiscs;
        public Collection<DiscDataEx> KnownDiscs
        {
            get { return knownDiscs; }
        }

        //never loaded by OML or MS-just shoved in a slot
        private Collection<DiscDataEx> unknownDiscs;
        public Collection<DiscDataEx> UnnownDiscs
        {
            get { return unknownDiscs; }
        }
    }

    /// <summary>
    /// Remoting results in buggy ms objects, using internal cache
    /// </summary>
    public class DiscDataEx
    {
        public DiscDataEx(DiscData disc, int slotId, int changerId)
        {
            this.DiscId = disc.DiscId;
            this.DiscType = disc.DiscType;
            this.DrivePath = disc.DrivePath;
            this.VolumeLabel = disc.VolumeLabel;
            this.SlotId = slotId;
            this.ChangerId = changerId;

            if (disc.DiscType == DiscType.MovieDvd)
            {
                //load additional movie crud
                this.Director = this.GetMetaString(disc.MediaMetadata, "Director");
                this.Title = this.GetMetaString(disc.MediaMetadata, "Title");
                this.ReleaseDate = this.GetMetaString(disc.MediaMetadata, "ReleaseDate");
                this.MPAARating = this.GetMetaString(disc.MediaMetadata, "MPAARating");
                this.Duration = this.GetMetaString(disc.MediaMetadata, "Duration");
            }
        }

        private string GetMetaString(MediaMetadata MediaMetadata, string Key)
        {
            string retStr = "";
            object objStr = "";
            MediaMetadata.TryGetValue(Key, out objStr);
            if (!string.IsNullOrEmpty((string)objStr))
                retStr = (string)objStr;
            return retStr;
        }
        //public int Address { get; }//address is useless
        public int ChangerId;
        public int SlotId;
        public string DiscId;
        public DiscType DiscType;
        public string DrivePath;
        public string VolumeLabel;

        //from metadata-we don't care about other disc types...
        public string Director;
        public string Title;
        public string ReleaseDate;
        public string MPAARating;
        public string Duration;

        public void Eject()
        {
            MediaChangeManagerHelper.EjectDisc(this);
        }

        public void Load()
        {
            this.DrivePath=MediaChangeManagerHelper.LoadDisc(this);
        }
    }

    public class MediaChangerEx
    {
        public MediaChangerEx(MediaChanger changer, int ChangerId)
        {
            this.Discs = new Collection<DiscDataEx>();
            Collection<DiscData> slotDiscs = changer.GetSlotDiscData();
            Collection<DiscData> driveDiscs = changer.GetDriveDiscData();
            int slotId = -1;//is -1 because it is in the drive
            foreach (DiscData disc in driveDiscs)
            {
                if (disc.DiscType == DiscType.Empty)
                {
                    //nothing to add here
                }
                else
                    this.Discs.Add(new DiscDataEx(disc, slotId, ChangerId ));
            }

            slotId = 0;
            foreach (DiscData disc in slotDiscs)
            {
                if (disc.DiscType == DiscType.Empty)
                {
                    //nothing to add here
                }
                else
                    this.Discs.Add(new DiscDataEx(disc, slotId, ChangerId));
                slotId++;
            }

        }
        public Collection<DiscDataEx> Discs;
    }

    internal static class MediaChangeManagerHelper
    {
        public static Collection<MediaChangerEx> GetChangers()
        {
            Collection<MediaChangerEx> retChangers = new Collection<MediaChangerEx>();

            Collection<MediaChanger> changers = OMLApplication.Current.MediaCenterEnvironment.MediaChangers;
            if (changers != null)
            {
                int changerID = 0;
                foreach (MediaChanger changer in changers)
                {
                    MediaChangerEx changerEx = new MediaChangerEx(changer, changerID);
                    retChangers.Add(changerEx);
                    changerID++;
                }
            }

            return retChangers;
        }

        //eject the disc
        public static void EjectDisc(DiscDataEx Disc)
        {
            Collection<MediaChanger> changers = OMLApplication.Current.MediaCenterEnvironment.MediaChangers;

            if (changers != null && changers.Count > Disc.ChangerId)
            {
                //our changer is valid
                MediaChanger changer = changers[Disc.ChangerId];
                Collection<DiscData> discs = changer.GetSlotDiscData();

                //check the slot...
                if (Disc.SlotId != -1)
                {
                    //the slot is not valid anymore
                    if (Disc.SlotId >= discs.Count || Disc.DiscId != discs[Disc.SlotId].DiscId)
                        Disc.SlotId = -1;
                }
                //disc was in a drive when we scanned
                if (Disc.SlotId == -1)
                {
                    //see if it is still there
                    int driveIdx = 0;
                    foreach (DiscData disc in changer.GetDriveDiscData())
                    {
                        if (disc.DiscId == Disc.DiscId)
                        {
                            //get it out of the drive
                            Disc.SlotId = changer.UnloadDisc(driveIdx);
                            //force a rescan
                            OMLApplication.Current.MediaChangers.ReScan();
                            //GetChangers the discs AGAIN
                            discs = changer.GetSlotDiscData();
                        }
                    }
                }

                //our id still valid
                if (discs.Count > Disc.SlotId && Disc.SlotId != -1 && discs[Disc.SlotId].DiscId == Disc.DiscId)
                {
                    changer.EjectDisc(Disc.SlotId);
                    //force a rescan
                    OMLApplication.Current.MediaChangers.ReScan();
                }
                else
                    throw new Exception("Disc Not Found.");
            }
            else
                throw new Exception("Disc Not Found.");
        }

        /// <summary>
        /// This is somewhat awkward
        /// </summary>
        /// <param name="Disc"></param>
        /// <returns></returns>
        public static string LoadDisc(DiscDataEx Disc)
        {
            //the drive letter of our disc once we load it
            string discDrive = "";
            Collection<MediaChanger> changers = OMLApplication.Current.MediaCenterEnvironment.MediaChangers;

            if (changers != null && changers.Count > Disc.ChangerId)
            {
                //our changer is valid
                MediaChanger changer = changers[Disc.ChangerId];
                Collection<DiscData> discs = changer.GetSlotDiscData();

                //disc was in a drive when we scanned
                if (Disc.SlotId == -1)
                {
                    //see if it is still there
                    foreach (DiscData disc in changer.GetDriveDiscData())
                    {
                        if (disc.DiscId == Disc.DiscId)
                        {
                            //found it-lets get out of here
                            discDrive = disc.DrivePath;
                            return discDrive;
                        }
                    }
                    //if we are here the disc is not in the drive
                    int intSlotId = 0;
                    foreach(DiscData disc in changer.GetSlotDiscData())
                    {
                        if (Disc.DiscId == disc.DiscId)
                            Disc.SlotId = intSlotId;
                        intSlotId++;
                    }
                }
                
                //load from slot
                if (Disc.SlotId != -1 && Disc.SlotId < discs.Count && Disc.DiscId == discs[Disc.SlotId].DiscId)
                {
                    try { changer.UnloadDisc(0); }
                    finally
                    {
                        changer.LoadDisc(Disc.SlotId, 0);//we always load to the first drive
                    }
                    //force a rescan
                    OMLApplication.Current.MediaChangers.ReScan();
                    foreach (DiscData disc in changer.GetDriveDiscData())
                    {
                        if (disc.DiscId == Disc.DiscId)
                        {
                            //found it-lets get out of here
                            discDrive = disc.DrivePath;
                            return discDrive;
                        }
                    }
                }
                //if we are here lets throw
                throw new Exception("Disc Not Found.");

            }
            else
                throw new Exception("Disc Not Found.");
            //return discDrive;
        }

        /// <summary>
        /// if we ned to check for unknowns
        /// There are known knowns and there are unknown knowns...
        /// </summary>
        public static void RescanAllDiscs()
        {
            Collection<MediaChanger> changers = OMLApplication.Current.MediaCenterEnvironment.MediaChangers;
            if (changers != null)
            {
                foreach (MediaChanger changer in changers)
                {
                    int intSlotAddress = 0;
                    foreach (DiscData disc in changer.GetSlotDiscData())
                    {
                        if (disc.DiscType == DiscType.Unknown)
                        {
                            try { changer.UnloadDisc(0); }
                            finally
                            {
                                changer.LoadDisc(intSlotAddress, 0);//we are assuming that all changers have a 0 drive...
                                changer.RescanDisc(0);
                                //changer.UnloadDisc(0);
                            }
                        }
                        intSlotAddress++;
                    }
                }
            }
            OMLApplication.Current.MediaChangers.ReScan();  
        }

        public static string GetStringWithoutPrefix(string str)
        {
            string[] prefixIgnoreStrings = new string[]{"a","the","and"};//should pull from settings
            return RemovePrefixIgnoreStrings(str, prefixIgnoreStrings);
        }

        internal static string RemovePrefixIgnoreStrings(string strOldString, string[] rgPrefixIgnoreStrings)
        {
            if ((strOldString != null) && (strOldString.Length != 0))
            {
                if ((rgPrefixIgnoreStrings == null) || (rgPrefixIgnoreStrings.Length == 0))
                {
                    return strOldString;
                }
                int length = rgPrefixIgnoreStrings.Length;
                int index = 0;
                while (index < length)
                {
                    string strA = rgPrefixIgnoreStrings[index];
                    if (((strA != null) && (strA.Length > 0)) && ((strOldString.Length > strA.Length) && (string.Compare(strA, 0, strOldString, 0, strA.Length, true, CultureInfo.CurrentUICulture) == 0)))
                    {
                        strOldString = strOldString.Substring(strA.Length);
                        index = 0;
                    }
                    else
                    {
                        index++;
                    }
                }
            }
            return strOldString;
        }

        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int StrCmpLogicalW(string st1, string st2);

    }

    internal sealed class CompareMediaChangerManagerDiscName : IComparer
    {
        // Methods
        public int Compare(object x, object y)
        {
            if ((x == null) && (y == null))
            {
                return 0;
            }
            if (x != null)
            {
                if (y == null)
                {
                    return 1;
                }
                DiscCommand disc = (DiscCommand)x;
                DiscCommand disc2 = (DiscCommand)y;
                if (disc.IsDrive == disc2.IsDrive)
                {
                    return MediaChangeManagerHelper.StrCmpLogicalW(disc.SortingName, disc2.SortingName);
                }
                if (!disc.IsDrive)
                {
                    return 1;
                }
            }
            return -1;
        }
    }

    internal sealed class CompareMediaChangerManagerDiscType : IComparer
    {
        // Methods
        public int Compare(object x, object y)
        {
            if ((x == null) && (y == null))
            {
                return 0;
            }
            if (x == null)
            {
                return -1;
            }
            if (y == null)
            {
                return 1;
            }
            DiscCommand disc = (DiscCommand)x;
            DiscCommand disc2 = (DiscCommand)y;
            if (disc.IsDrive != disc2.IsDrive)
            {
                if (!disc.IsDrive)
                {
                    return 1;
                }
                return -1;
            }
            int num = string.Compare(disc.Type, disc2.Type, true, CultureInfo.CurrentUICulture);
            if (num == 0)
            {
                return MediaChangeManagerHelper.StrCmpLogicalW(disc.SortingName, disc2.SortingName);
            }
            return num;
        }
    }
    
    public class DiscCommand : Command
    {
        private Command load;
        public Command Load
        {
            get
            {
                return this.load;
            }
            set
            {
                this.load = value;
            }
        }
        public DiscCommand(IModelItem Owner)
            : base(Owner)
        {
            this.load = new Command(this);
            this.load.Invoked += new EventHandler(load_Invoked);
        }

        void load_Invoked(object sender, EventArgs e)
        {
            //load the disc
            this.Disc.Load();
        }
        public DiscDataEx Disc;

        public string Name
        {
            get
            {
                if (this.Disc.Title == null)
                {
                    return this.Disc.VolumeLabel;
                }
                return this.Disc.Title;
            }
        }

        public string SortingName
        {
            get
            {
                return MediaChangeManagerHelper.GetStringWithoutPrefix(this.Name);
            }
        }

        public bool IsDrive
        {
            get
            {
                if (this.Disc.DrivePath == null)
                {
                    return false;
                }
                return true;
            }
        }

        public string DisplayType
        {
            get
            {
                switch (this.Disc.DiscType)
                {
                    case DiscType.MovieDvd:
                        return "DVD";
                    case DiscType.BlankDvd:
                        return "Blank DVD";
                    case DiscType.AudioCD:
                        return "CD";
                    default:
                        return this.Type;
                }
            }
        }
        public string Type
        {
            get
            {
                return this.Disc.DiscType.ToString();
            }
        }
    }
}
