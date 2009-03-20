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
    public class AreaOfInterestHelper : Microsoft.MediaCenter.UI.ModelItem
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

}
