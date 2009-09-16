using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;
using System.Collections;

namespace Library.Code.V3
{
    public class AreaOfInterestLayoutInput : ILayoutInput, ILayoutData
    {
        // Fields
        //private string m_stId;
        private static readonly DataCookie s_propData = DataCookie.ReserveSlot();
        private PrivateObjectWeasel _weasel = null;

        // Methods
        public AreaOfInterestLayoutInput()
        {
            //this._weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.AreaOfInterestLayoutInput, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", new Type[0]);
            string stId = "Focus";
            this._weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.AreaOfInterestLayoutInput, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", new object[] { stId });
        }

        public AreaOfInterestLayoutInput(string stId)
        {
            this._weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.AreaOfInterestLayoutInput, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", new object[] { stId });
            //this.m_stId = stId;
        }

        public override string ToString()
        {
            //return InvariantString.Format("{0}({1})", new object[] { base.GetType().Name, this.m_stId });
            return (string)this._weasel.Invoke("ToString", null);
        }

        // Properties
        public static DataCookie Data
        {
            get
            {
                return s_propData;
            }
        }

        public string Id
        {
            get
            {
                //return this.m_stId;
                return (string)this._weasel.GetProperty("Id");
            }
            set
            {
                //this.m_stId = value;
                this._weasel.SetProperty("Id", value);
            }
        }

        DataCookie ILayoutData.Data
        {
            get
            {
                //return Data;
                return (DataCookie)this._weasel.GetProperty("ILayoutData.Data");
            }
        }

        public object underlyingData
        {
            get
            {
                //return Data;
                return this._weasel.GetProperty("Data");
            }
        }

        public object underlyingObject
        {
            get
            {
                //return Data;
                return this._weasel;
            }
        }

        //protected override void Dispose(bool fInDispose)
        //{
        //    this._weasel.Invoke("Dispose");
        //}
    }

    [MarkupVisible]
    public class AreaOfInterestHelper : BaseModelItem
    {
        public void SetLayoutInput(object Panel, object oNewValue)
        {
            return;
            //if (this.weasel != null)
            //{
            //if (this.weasel != null)
            //{
            //    this.weasel.SetProperty("Id", "notFocus");
            //    this.weasel = null;
            //}
            Type myType = Panel.GetType();
            MethodInfo theMethod = myType.GetMethod("SetLayoutInput", new Type[] { oNewValue.GetType() });
            theMethod.Invoke(Panel, new object[] { oNewValue });
            if (this.weasel != null)
                this.weasel = null;
            return;
    //        //    Type zmyType = Panel.GetType();
    //        //    //PrivateObjectWeasel zweasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.AreaOfInterestLayoutInput, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");

    //        //    MethodInfo ztheMethod = zmyType.GetMethod("MarkLayoutInvalid");
    //        //    ztheMethod.Invoke(Panel, null);
    //        //}

    //        //this.weasel.SetProperty("Id", "notFocus");
    //        //}
            
    //        //return;
    ////            Hashtable PropertiesOfMyObject = new Hashtable();
    ////            Type t = weasel.Instance.GetType();
    ////            PropertyInfo[] pis = t.GetProperties(BindingFlags.FlattenHierarchy |
    ////BindingFlags.IgnoreCase |
    ////BindingFlags.Instance |
    ////BindingFlags.NonPublic |
    ////BindingFlags.Public);
    ////            for (int i = 0; i < pis.Length; i++)
    ////            {
    ////                PropertyInfo pi = (PropertyInfo)pis.GetValue(i);
    ////                PropertiesOfMyObject.Add(pi.Name, pi.GetValue(weasel.Instance, new object[] { }));
    ////            }
    ////        }


    ////        return;
    //        ////////////////////
    //        if (this.weasel != null)
    //        {

    //        }
    //        //Type myType = Panel.GetType();
    //        //Type oType = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject.GetType();
    //        //Type oType2 = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingData.GetType();
    //        //MethodInfo theMethod = myType.GetMethod("SetLayoutInput", new Type[] { oType2, oType });
    //        //theMethod.Invoke(Panel, new object[] { ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject });

    //        Type myType = Panel.GetType();
    ////        MethodInfo mi = weasel.Instance.GetType().GetMethod("get_ILayoutData.Data",
    ////BindingFlags.FlattenHierarchy |
    ////BindingFlags.IgnoreCase |
    ////BindingFlags.Instance |
    ////BindingFlags.NonPublic |
    ////BindingFlags.Public);
    ////        mi.Invoke(weasel.Instance, null);
    //        object o = weasel.Instance.GetType().GetProperty("Microsoft.MediaCenter.UI.ILayoutData.Data", BindingFlags.FlattenHierarchy |
    //BindingFlags.IgnoreCase |
    //BindingFlags.Instance |
    //BindingFlags.NonPublic |
    //BindingFlags.Public|BindingFlags.Static);

    //        object o2 = o.GetType().GetProperty("m_value", BindingFlags.FlattenHierarchy |
    //BindingFlags.IgnoreCase |
    //BindingFlags.Instance |
    //BindingFlags.NonPublic |
    //BindingFlags.Public | BindingFlags.Static);
    //        //ValueType v = (ValueType)o;
    //        //System.ComponentModel.PropertyDescriptorCollection pdc = System.ComponentModel.TypeDescriptor.GetProperties(weasel.Instance);
    //        //foreach (System.ComponentModel.PropertyDescriptor pdcI in pdc)
    //        //{

    //        //}
    //        //PrivateObjectWeasel weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.AreaOfInterestLayoutInput, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", new object[] { "Focus" });

    //        //PrivateObjectWeasel weasel2 = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.ViewItem, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
    //        ////Type oType = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject.GetType();
            
    //        ////Type oType2 = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingData.GetType();
            
    //        MethodInfo theMethod = myType.GetMethod("SetLayoutInput", new Type[] { Type.GetType("Microsoft.MediaCenter.UI.Utility.DataCookie, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"), Type.GetType("Microsoft.MediaCenter.UI.ILayoutInput, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35") });
    //        theMethod.Invoke(Panel, new object[] { o, null });


        }

        private PrivateObjectWeasel weasel;

        public void SetLayoutInput(object Panel)
        {
            //Type myType = Panel.GetType();
            //Type oType = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject.GetType();
            //Type oType2 = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingData.GetType();
            //MethodInfo theMethod = myType.GetMethod("SetLayoutInput", new Type[] { oType2, oType });
            //theMethod.Invoke(Panel, new object[] { ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject });

            Type myType = Panel.GetType();
            weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.AreaOfInterestLayoutInput, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",new object[]{"Focus"});

            //Type oType = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject.GetType();
            //Type oType2 = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingData.GetType();
            MethodInfo theMethod = myType.GetMethod("SetLayoutInput", new Type[] { weasel.Instance.GetType() });
            theMethod.Invoke(Panel, new object[] { weasel.Instance });


        }

        public void RequestFocus(object Panel)
        {
            //Type myType = Panel.GetType();
            //Type oType = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject.GetType();
            //Type oType2 = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingData.GetType();
            //MethodInfo theMethod = myType.GetMethod("SetLayoutInput", new Type[] { oType2, oType });
            //theMethod.Invoke(Panel, new object[] { ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject });
            Microsoft.MediaCenter.UI.ModelItem mi = (Microsoft.MediaCenter.UI.ModelItem)Panel;
            Type myType = mi.GetType();
            //weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.AreaOfInterestLayoutInput, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35",new object[]{"Focus"});

            //Type oType = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingObject.GetType();
            //Type oType2 = ((AreaOfInterestLayoutInput)AreaOfInterest).underlyingData.GetType();
            MethodInfo theMethod = myType.GetMethod("AggressivelyRequestFocus",BindingFlags.Instance|BindingFlags.NonPublic);
            theMethod.Invoke(mi, new object[] {});


        }
    }

    [MarkupVisible]
    public interface ILayoutInput : ILayoutData
    {
    }

    public interface ILayoutData
    {
        // Properties
        DataCookie Data { get; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DataCookie
    {
        public static readonly DataCookie NULL;
        private uint m_value;
        private DataCookie(uint value)
        {
            this.m_value = value;
        }

        public static bool operator ==(DataCookie hl, DataCookie hr)
        {
            return (hl.m_value == hr.m_value);
        }

        public static bool operator !=(DataCookie hl, DataCookie hr)
        {
            return (hl.m_value != hr.m_value);
        }

        public override bool Equals(object oCompare)
        {
            uint num;
            if (oCompare is uint)
            {
                num = (uint)oCompare;
            }
            else if (oCompare is DataCookie)
            {
                num = ((DataCookie)oCompare).m_value;
            }
            else
            {
                return false;
            }
            return (this.m_value == num);
        }

        public override int GetHashCode()
        {
            return (int)this.m_value;
        }

        internal static DataCookie FromUInt32(uint value)
        {
            return new DataCookie(value);
        }

        internal static uint ToUInt32(DataCookie handle)
        {
            return handle.m_value;
        }

        public static DataCookie ReserveSlot()
        {
            return FromUInt32(KeyAllocator.ReserveSlot());
        }

        static DataCookie()
        {
            NULL = new DataCookie();
        }
    }

    internal class KeyAllocator
    {
        // Fields
        private static int s_idxKeyGen;

        // Methods
        internal static uint ReserveSlot()
        {
            return (uint)Interlocked.Increment(ref s_idxKeyGen);
        }
    }

    public class AutoScrollHandler : BaseModelItem
    {
        // Fields
        private Microsoft.MediaCenter.UI.ICommand m_cmd;
        private DateTime m_dtActivated;
        private bool m_fCanScroll;
        private bool m_fDelayScroll;
        private bool m_fIsActive;
        private bool m_fPaused;
        private int m_nDelayInterval = 300;
        private int m_nScrollInterval = 100;
        private int m_nTimeoutInterval = 0x7530;
        private Microsoft.MediaCenter.UI.Timer m_timerScroll;

        // Methods
        private void DisableAutoScroll()
        {
            if (this.m_timerScroll != null)
            {
                this.m_timerScroll.Enabled = false;
            }
            this.LockMouseActive(false);
            this.IsActive = false;
        }

        protected override void Dispose(bool fInDispose)
        {
            if (fInDispose)
            {
                if (this.IsActive)
                {
                    this.DisableAutoScroll();
                }
                if (this.m_timerScroll != null)
                {
                    this.m_timerScroll.Tick -= new EventHandler(this.OnAutoScroll);
                    this.m_timerScroll.Dispose();
                    this.m_timerScroll = null;
                }
            }
            base.Dispose(fInDispose);
        }

        internal void InvokeCommand()
        {
            if (this.m_cmd != null)
            {
                this.m_cmd.Invoke();
            }
        }

        private void LockMouseActive(bool fLock)
        {
            //base.Behavior.UiSession.Form.LockMouseActive(fLock);
        }

        private void OnAutoScroll(object sender, EventArgs args)
        {
            //if (!this.CanScroll && !base.Behavior.FullyEnabled)
            if (!this.CanScroll)
            {
                this.DisableAutoScroll();
            }
            //else if ((this.m_timerScroll.Enabled && base.Enabled) && !this.m_fPaused)
            else if (this.m_timerScroll.Enabled && !this.m_fPaused)
            {
                if (this.m_fDelayScroll)
                {
                    this.m_timerScroll.Interval = this.m_nScrollInterval;
                    this.m_fDelayScroll = false;
                    this.m_dtActivated = DateTime.Now;
                    this.OnAutoScroll(this.m_timerScroll, EventArgs.Empty);
                }
                else
                {
                    TimeSpan span = (TimeSpan)(DateTime.Now - this.m_dtActivated);
                    if (span > TimeSpan.FromMilliseconds((double)this.m_nTimeoutInterval))
                    {
                        this.DisableAutoScroll();
                    }
                    else
                    {
                        this.InvokeCommand();
                    }
                }
            }
        }

        public void OnGainMouseFocus(object sender, EventArgs args)
        {
            //base.OnGainMouseFocus(sender, args);
            if (this.CanScroll)
            {
                this.IsActive = true;
                if (this.m_timerScroll == null)
                {
                    this.m_timerScroll = new Microsoft.MediaCenter.UI.Timer();
                    this.m_timerScroll.Tick += new EventHandler(this.OnAutoScroll);
                }
                if (this.m_nDelayInterval > 0)
                {
                    this.m_timerScroll.Interval = this.m_nDelayInterval;
                    this.m_fDelayScroll = true;
                }
                else
                {
                    this.m_timerScroll.Interval = this.m_nScrollInterval;
                    this.m_fDelayScroll = false;
                    this.m_dtActivated = DateTime.Now;
                    this.OnAutoScroll(this.m_timerScroll, EventArgs.Empty);
                }
                this.m_timerScroll.Enabled = true;
                this.LockMouseActive(true);
            }
        }

        public void OnLoseMouseFocus(object sender, EventArgs args)
        {
            //base.OnLoseMouseFocus(sender, args);
            this.DisableAutoScroll();
        }

        public void OnMouseMove(object sender, EventArgs args)
        {
            if ((this.m_timerScroll != null) && this.m_timerScroll.Enabled)
            {
                this.m_dtActivated = DateTime.Now;
            }
            //base.OnMouseMove(sender, args);
        }

        //private void OnSessionInput(Control sender, UiEventArgs untypedArgs)
        //{
        //    SessionInputArgs args = (SessionInputArgs)untypedArgs;
        //    if (((args.OriginalEvent.eventType == UiEventType.InputCommand) || (args.OriginalEvent.eventType == UiEventType.InputKeyCharacter)) || (args.OriginalEvent.eventType == UiEventType.InputKeyState))
        //    {
        //        bool flag = false;
        //        if (args.HandledStage == EventRouteStages.Direct)
        //        {
        //            flag = args.target != base.Behavior;
        //        }
        //        else
        //        {
        //            flag = true;
        //        }
        //        if (flag)
        //        {
        //            this.DisableAutoScroll();
        //        }
        //    }
        //}

        // Properties
        public bool CanScroll
        {
            get
            {
                return this.m_fCanScroll;
            }
            set
            {
                if (this.m_fCanScroll != value)
                {
                    this.m_fCanScroll = value;
                    base.FirePropertyChanged("CanScroll");
                }
            }
        }

        public int DelayInterval
        {
            get
            {
                return this.m_nDelayInterval;
            }
            set
            {
                if (this.m_nDelayInterval != value)
                {
                    this.m_nDelayInterval = value;
                    base.FirePropertyChanged("DelayInterval");
                }
            }
        }

        public bool IsActive
        {
            get
            {
                return this.m_fIsActive;
            }
            private set
            {
                if (this.m_fIsActive != value)
                {
                    this.m_fIsActive = value;
                    //if (value)
                    //{
                    //    base.Behavior.SessionInput += new UiEventHandler(this.OnSessionInput);
                    //}
                    //else
                    //{
                    //    base.Behavior.SessionInput -= new UiEventHandler(this.OnSessionInput);
                    //}
                    base.FirePropertyChanged("IsActive");
                }
            }
        }

        public Microsoft.MediaCenter.UI.ICommand Model
        {
            get
            {
                return this.m_cmd;
            }
            set
            {
                if (this.m_cmd != value)
                {
                    this.m_cmd = value;
                    base.FirePropertyChanged("Model");
                }
            }
        }

        public bool Paused
        {
            get
            {
                return this.m_fPaused;
            }
            set
            {
                if (this.m_fPaused != value)
                {
                    this.m_fPaused = value;
                    base.FirePropertyChanged("Paused");
                }
            }
        }

        public int ScrollInterval
        {
            get
            {
                return this.m_nScrollInterval;
            }
            set
            {
                if (this.m_nScrollInterval != value)
                {
                    this.m_nScrollInterval = value;
                    base.FirePropertyChanged("ScrollInterval");
                }
            }
        }

        public int TimeoutInterval
        {
            get
            {
                return this.m_nTimeoutInterval;
            }
            set
            {
                if (this.m_nTimeoutInterval != value)
                {
                    this.m_nTimeoutInterval = value;
                    base.FirePropertyChanged("TimeoutInterval");
                }
            }
        }
    }


}
