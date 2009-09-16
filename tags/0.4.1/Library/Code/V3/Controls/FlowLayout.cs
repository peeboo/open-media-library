using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.MediaCenter.UI;
using System.Globalization;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Net;
using System.Threading;
using System.Security;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Permissions;
using System.Runtime.Serialization;
using System.Web;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.Xml;
using System.Text.RegularExpressions;


namespace Library.Code.V3
{
    [MarkupVisible]
    public class FlowLayout : ILayout
    {
        private object _flowLayout = null;
        private Hashtable PropertiesOfMyObject;

        [MarkupVisible]
        public FlowLayout()
        {
            //// obtaining a reference to System.Web assembly
            //Assembly systemWeb = typeof(Microsoft.MediaCenter.UI).Assembly;
            //if (systemWeb == null)
            //{
            //    throw new InvalidOperationException(
            //        "Unable to load System.Web.");
            //}
            //// obtaining a reference to the internal class CookieProtectionHelper
            //Type cookieProtectionHelper = systemWeb.GetType(
            //        "Microsoft.MediaCenter.UI.FlowLayout");
            //if (cookieProtectionHelper == null)
            //{
            //    throw new InvalidOperationException(
            //        "Unable to get the internal class CookieProtectionHelper.");
            //}


            //            Type type1 = typeof(
            //typeof("Microsoft.MediaCenter.UI", "FlowLayout");

            //Create an instance of the type
            this._flowLayout = Activator.CreateInstance("Microsoft.MediaCenter.UI", "FlowLayout");

            PropertiesOfMyObject = new Hashtable();
            Type t = this._flowLayout.GetType();
            PropertyInfo[] pis = t.GetProperties();
            for (int i = 0; i < pis.Length; i++)
            {
                PropertyInfo pi = (PropertyInfo)pis.GetValue(i);
                PropertiesOfMyObject.Add(pi.Name, pi);
            }

            //object[] mParam = new object[] { 5, 10 };

            ////invoke AddMethod, passing in two parameters

            //int res = (int)type1.InvokeMember("AddNumb", BindingFlags.InvokeMethod, null, obj, mParam);
        }



        void ILayout.Layout(ILayoutParentAgent laSubject, Constraint constraint, out Size sizeUsed)
        {
            sizeUsed = new Size();
            //object[] mParam = new object[] { laSubject, constraint, sizeUsed };
            //this._flowLayout.GetType().InvokeMember("Layout", BindingFlags.InvokeMethod, null, this._flowLayout, mParam);

        }

        // Properties
        [DefaultValue(false), MarkupVisible]
        public bool AllowWrap
        {
            get
            {
                return (bool)((PropertyInfo)this.PropertiesOfMyObject["AllowWrap"]).GetValue(this._flowLayout, new object[] { });
            }
            set
            {
                ((PropertyInfo)this.PropertiesOfMyObject["AllowWrap"]).SetValue(this._flowLayout, value, null);
            }
        }

        [MarkupVisible, DefaultValue(false)]
        public bool FillStrip
        {
            get
            {
                return (bool)((PropertyInfo)this.PropertiesOfMyObject["FillStrip"]).GetValue(this._flowLayout, new object[] { });
            }
            set
            {
                ((PropertyInfo)this.PropertiesOfMyObject["FillStrip"]).SetValue(this._flowLayout, value, null);
            }
        }

        [MarkupVisible, DefaultValue(true)]
        public bool HideOffscreenItems
        {
            get
            {
                return (bool)((PropertyInfo)this.PropertiesOfMyObject["HideOffscreenItems"]).GetValue(this._flowLayout, new object[] { });
            }
            set
            {
                ((PropertyInfo)this.PropertiesOfMyObject["HideOffscreenItems"]).SetValue(this._flowLayout, value, null);
            }
        }

        [MarkupVisible, DefaultValue(0)]
        public ItemAlignment ItemAlignment
        {
            get
            {
                return (ItemAlignment)((PropertyInfo)this.PropertiesOfMyObject["ItemAlignment"]).GetValue(this._flowLayout, new object[] { });
            }
            set
            {
                ((PropertyInfo)this.PropertiesOfMyObject["ItemAlignment"]).SetValue(this._flowLayout, value, null);
            }
        }

        [MarkupVisible, DefaultValue(3)]
        public int MinimumSampleSize
        {
            get
            {
                return (int)((PropertyInfo)this.PropertiesOfMyObject["MinimumSampleSize"]).GetValue(this._flowLayout, new object[] { });
            }
            set
            {
                ((PropertyInfo)this.PropertiesOfMyObject["MinimumSampleSize"]).SetValue(this._flowLayout, value, null);
            }
        }

        [MarkupVisible, DefaultValue(0)]
        public MissingItemPolicy MissingItemPolicy
        {
            get
            {
                return (MissingItemPolicy)((PropertyInfo)this.PropertiesOfMyObject["MissingItemPolicy"]).GetValue(this._flowLayout, new object[] { });
            }
            set
            {
                ((PropertyInfo)this.PropertiesOfMyObject["MissingItemPolicy"]).SetValue(this._flowLayout, value, null);
            }
        }

        [DefaultValue(0), MarkupVisible]
        public Orientation Orientation
        {
            get
            {
                return (Orientation)((PropertyInfo)this.PropertiesOfMyObject["Orientation"]).GetValue(this._flowLayout, new object[] { });
            }
            set
            {
                ((PropertyInfo)this.PropertiesOfMyObject["Orientation"]).SetValue(this._flowLayout, value, null);
            }
        }

        [MarkupVisible, DefaultValue(0)]
        public RepeatPolicy Repeat
        {
            get
            {
                return (RepeatPolicy)((PropertyInfo)this.PropertiesOfMyObject["Repeat"]).GetValue(this._flowLayout, new object[] { });
            }
            set
            {
                ((PropertyInfo)this.PropertiesOfMyObject["Repeat"]).SetValue(this._flowLayout, value, null);
            }
        }

        [MarkupVisible]
        public MajorMinor RepeatGap
        {
            get
            {
                return (MajorMinor)((PropertyInfo)this.PropertiesOfMyObject["RepeatGap"]).GetValue(this._flowLayout, new object[] { });
            }
            set
            {
                ((PropertyInfo)this.PropertiesOfMyObject["RepeatGap"]).SetValue(this._flowLayout, value, null);
            }
        }

        [MarkupVisible]
        public MajorMinor Spacing
        {
            get
            {
                return (MajorMinor)((PropertyInfo)this.PropertiesOfMyObject["Spacing"]).GetValue(this._flowLayout, new object[] { });
            }
            set
            {
                ((PropertyInfo)this.PropertiesOfMyObject["Spacing"]).SetValue(this._flowLayout, value, null);
            }
        }

        [DefaultValue(false)]
        public bool StopOnEmptyItem
        {
            get
            {
                return (bool)((PropertyInfo)this.PropertiesOfMyObject["StopOnEmptyItem"]).GetValue(this._flowLayout, new object[] { });
            }
            set
            {
                ((PropertyInfo)this.PropertiesOfMyObject["StopOnEmptyItem"]).SetValue(this._flowLayout, value, null);
            }
        }

        [DefaultValue(0), MarkupVisible]
        public StripAlignment StripAlignment
        {
            get
            {
                return (StripAlignment)((PropertyInfo)this.PropertiesOfMyObject["StripAlignment"]).GetValue(this._flowLayout, new object[] { });
            }
            set
            {
                ((PropertyInfo)this.PropertiesOfMyObject["StripAlignment"]).SetValue(this._flowLayout, value, null);
            }
        }
    }

    internal class LayoutTypeConverter : TypeConverter
    {
        // Methods
        static LayoutTypeConverter()
        {

        }

        public override bool CanConvertFrom(ITypeDescriptorContext tdc, Type typeSource)
        {
            return true;
        }

        public override object ConvertFrom(ITypeDescriptorContext tdc, CultureInfo ci, object value)
        {
            return string.Empty;
        }
    }

    [TypeConverter(typeof(LayoutTypeConverter)), MarkupVisible]
    public interface ILayout
    {
        // Methods
        void Layout(ILayoutParentAgent laSubject, Constraint constraint, out Size sizeUsed);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Constraint
    {
        public const int Undefined = 0;
        public const int Infinite = 0xffffff;
        public static readonly Size UndefinedSize;
        //private Size m_sizeMax;
        //private Size m_sizeMin;
        //private Rectangle m_rcView;
        //private Rectangle m_rcPeripheralView;
        //private float m_flPageStep;
        //private bool m_fView;
        public Constraint(Size sizeMax)
        {
        }

        public Constraint(Size sizeMax, Size sizeMin)
        {
            //this.m_sizeMax = sizeMax;
            //this.m_sizeMin = sizeMin;
            //this.m_fView = false;
            //this.m_rcView = this.m_rcPeripheralView = Rectangle.Empty;
            //this.m_flPageStep = 0f;
        }

        public Constraint(Size sizeMax, Size sizeMin, Rectangle rcView)
        {
        }

        public Constraint(Size sizeMax, Size sizeMin, Rectangle rcView, Rectangle rcViewPeripheral, float flPageStep)
        {
            //this.m_sizeMax = sizeMax;
            //this.m_sizeMin = sizeMin;
            //this.m_rcView = rcView;
            //this.m_rcPeripheralView = rcViewPeripheral;
            //this.m_flPageStep = flPageStep;
            //this.m_fView = true;
        }

        public Size Max
        {
            get
            {
                return new Size();
            }
            //set
            //{
            //    this.m_sizeMin = value;
            //}
        }
        public Size Min
        {
            get
            {
                return new Size();
            }
            //set
            //{
            //    this.m_sizeMin = value;
            //}
        }
        public Rectangle View
        {
            get
            {
                return new Rectangle();

            }
            //set
            //{
            //    this.m_rcView = value;
            //    this.m_fView = true;
            //}
        }
        public Rectangle PeripheralView
        {
            get
            {
                return new Rectangle();

            }
            //set
            //{
            //    this.m_rcPeripheralView = value;
            //    this.m_fView = true;
            //}
        }
        public float PageStep
        {
            get
            {
                return new float();
            }
            set
            {

            }
        }
        public bool IsInvalid
        {
            get
            {

                return true;
            }
        }
        public void Deflate(Inset inset)
        {

        }

        public Size Clamp(Size size)
        {
            return new Size();
        }

        public bool IsViewBoundsSpecified
        {
            get
            {
                return true;
            }
        }
        public static Difference Compare(Constraint left, Constraint right)
        {
            Difference identical = Difference.Identical;
            return identical;
        }

        public static bool operator ==(Constraint left, Constraint right)
        {

            return false;
        }

        public static bool operator !=(Constraint left, Constraint right)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return true;
        }

        public override int GetHashCode()
        {
            //return (((this.m_sizeMax.GetHashCode() ^ this.m_sizeMin.GetHashCode()) ^ this.m_rcView.GetHashCode()) ^ this.m_rcPeripheralView.GetHashCode());
            return 1;
        }

        public override string ToString()
        {
            //StringBuilder builder = new StringBuilder();
            //builder.Append(base.GetType().Name);
            //builder.Append("(");
            //builder.Append("Max=");
            //builder.Append(this.m_sizeMax);
            //if (!this.m_sizeMin.IsZero)
            //{
            //    builder.Append(", Min=");
            //    builder.Append(this.m_sizeMin);
            //}
            //if (this.m_fView)
            //{
            //    builder.Append(", View=");
            //    builder.Append(this.m_rcView);
            //    if (this.m_rcPeripheralView != this.m_rcView)
            //    {
            //        builder.Append(", Peripheral=");
            //        builder.Append(this.m_rcPeripheralView);
            //    }
            //}
            //builder.Append(")");
            //return builder.ToString();
            return string.Empty;
        }

        static Constraint()
        {
            UndefinedSize = new Size(0, 0);
        }
        // Nested Types
        [Flags]
        public enum Difference
        {
            Identical,
            MinMax,
            View
        }
    }

    public interface ILayoutParentAgent : ILayoutBaseParentAgent, IBaseLayoutAgent
    {
        // Methods
        void RequestMoreChildren(int cChildren);
        void RequestSpecificChildren(int[] arIndicies);

        // Properties
        LayoutReason ReasonForLayout { get; }
    }

    public interface ILayoutBaseParentAgent
    {
        // Properties
        IList<ILayoutChildAgent> Descendents { get; }
    }

    public interface IBaseLayoutAgent
    {
        // Methods
        void AddAreaOfInterest(AreaOfInterest interest);
        ExtendedLayoutOutput GetExtendedLayoutOutput(DataCookie idOutput);
        object GetLayoutInput(DataCookie idInput);
        void SetExtendedLayoutOutput(ExtendedLayoutOutput eloNewData);
        void Trace(object objSource, string stMessage);
        void Trace(object objSource, string stMessage, params object[] arobjMessageParams);
        void TraceCloseBrace(object objSource);
        void TraceIndent(int nDelta);
        void TraceOpenBrace(object objSource);
        void TraceOpenBrace(object objSource, string stMessage, params object[] arobjMessageParams);

        // Properties
        bool IsTracingNodeLayout { get; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AreaOfInterest
    {
        //private Rectangle m_rectangle;
        //private string m_stId;
        public AreaOfInterest(Size size, string id)
        {
        }

        public AreaOfInterest(Rectangle rectangle, string id)
        {
            //this.m_rectangle = rectangle;
            //this.m_stId = id;
        }

        public AreaOfInterest Transform(Point offset)
        {
            return new AreaOfInterest();
        }

        public static void AddAreaOfInterest(AreaOfInterest interest, ref List<AreaOfInterest> listAreasOfInterest)
        {
            //if (listAreasOfInterest == null)
            //{
            //    listAreasOfInterest = new List<AreaOfInterest>();
            //}
            //for (int i = 0; i < listAreasOfInterest.Count; i++)
            //{
            //    AreaOfInterest interest2 = listAreasOfInterest[i];
            //    if (interest2.Id == interest.Id)
            //    {
            //        listAreasOfInterest[i] = interest;
            //        return;
            //    }
            //}
            //listAreasOfInterest.Add(interest);
        }

        public static string ToString(List<AreaOfInterest> listAreasOfInterest)
        {
            return "<unknown>";
        }

        public override string ToString()
        {
            return string.Empty;
        }

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle();
            }
        }
        public string Id
        {
            get
            {
                return string.Empty;
            }
        }
    }

    public abstract class ExtendedLayoutOutput : ILayoutData
    {
        // Fields
        internal ExtendedLayoutOutput eloNext;

        // Methods
        protected ExtendedLayoutOutput()
        {
        }

        // Properties
        DataCookie ILayoutData.Data
        {
            get
            {
                return this.OutputID;
            }
        }

        public abstract DataCookie OutputID { get; }
    }

    [Flags]
    public enum LayoutReason
    {
        MinMaxConstraintChanged = 1,
        Other = 4,
        ViewingRectangleChanged = 2
    }

    public interface ILayoutChildAgent : IBaseLayoutAgent
    {
        // Methods
        void Layout(Constraint constraint);
        void MarkHidden();
        void Store(Point ptOffsetParent);
        void Store(StoreArgs args);
        void Store(Point ptOffsetParent, Size sizeExtent);
        void Store(Point ptOffsetParent, bool fVisible);
        void Store(Point ptOffsetParent, Size sizeExtent, bool fVisible);
        void Store(Point ptOffsetParent, Size sizeExtent, Vector3 scale, bool fVisible);

        // Properties
        List<AreaOfInterest> AreasOfInterest { get; }
        Size IdealSize { get; }
        Size UsedSize { get; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct StoreArgs
    {
        //private bool m_initiailized;
        //private bool m_visible;
        //private Point m_offset;
        //private Vector3 m_scale;
        //private Rotation m_rotation;
        //private bool m_offscreen;
        public StoreArgs(Point offset)
            : this(offset, true)
        {
        }

        public StoreArgs(Point offset, bool visible)
        {
            //this.m_visible = visible;
            //this.m_offset = offset;
            //this.m_scale = Vector3.UnitVector;
            //this.m_rotation = Rotation.Default;
            //this.m_offscreen = false;
            //this.m_initiailized = true;
        }

        public bool Visible
        {
            get
            {
                return true;
            }
            //set
            //{
            //    this.EnsureInitialized();
            //    this.m_visible = value;
            //}
        }
        public Point Offset
        {
            get
            {
                return new Point();
            }
            //set
            //{
            //    this.EnsureInitialized();
            //    this.m_offset = value;
            //}
        }
        public Vector3 Scale
        {
            get
            {
                return new Vector3();
            }
            //set
            //{
            //    this.EnsureInitialized();
            //    this.m_scale = value;
            //}
        }
        public Rotation Rotation
        {
            get
            {
                return Rotation.Default;
            }
            //set
            //{
            //    this.EnsureInitialized();
            //    this.m_rotation = value;
            //}
        }
        public bool Offscreen
        {
            get
            {
                return true;
            }
            //set
            //{
            //    this.EnsureInitialized();
            //    this.m_offscreen = value;
            //}
        }
        //private void EnsureInitialized()
        //{
        //    if (!this.m_initiailized)
        //    {
        //        this.m_visible = true;
        //        this.m_offset = Point.Zero;
        //        this.m_scale = Vector3.UnitVector;
        //        this.m_rotation = Rotation.Default;
        //        this.m_offscreen = false;
        //        this.m_initiailized = true;
        //    }
        //}
    }

    [MarkupVisible]
    public enum StripAlignment
    {
        Near,
        Center,
        Far
    }

    [MarkupVisible]
    public enum RepeatPolicy
    {
        Never,
        WhenTooBig,
        WhenTooSmall,
        Always
    }

    [MarkupVisible]
    public enum MissingItemPolicy
    {
        SizeToAverage,
        SizeToSmallest,
        SizeToLargest,
        Wait
    }

    [MarkupVisible]
    public enum ItemAlignment
    {
        Near,
        Center,
        Far,
        Fill
    }

    public class FlowSizeMemoryLayoutInput : ILayoutInput, ILayoutData
    {
        // Fields
        private Dictionary<int, Size> m_cache = new Dictionary<int, Size>();
        private static readonly DataCookie s_propData = DataCookie.ReserveSlot();

        // Methods
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            bool flag = true;
            foreach (KeyValuePair<int, Size> pair in this.m_cache)
            {
                if (!flag)
                {
                    builder.Append(", ");
                }
                flag = false;
                builder.Append("[");
                builder.Append(pair.Key);
                builder.Append("]=");
                builder.Append(pair.Value);
            }
            return InvariantString.Format("{0}({1})", new object[] { base.GetType().Name, builder });
        }

        // Properties
        public static DataCookie Data
        {
            get
            {
                return s_propData;
            }
        }

        public Dictionary<int, Size> KnownSizes
        {
            get
            {
                return this.m_cache;
            }
            set
            {
                this.m_cache = value;
            }
        }

        DataCookie ILayoutData.Data
        {
            get
            {
                return Data;
            }
        }
    }

}
