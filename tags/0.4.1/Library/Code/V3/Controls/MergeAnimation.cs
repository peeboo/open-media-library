using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.MediaCenter.UI;
using System.ComponentModel;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Library.Code.V3
{
    [MarkupVisible]
    public class MergeAnimation : IAnimationProvider
    {
        private PrivateObjectWeasel _weasel = null;
        public MergeAnimation()
        {
            this._weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.MergeAnimation, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
        }

        // Methods
        public object Build(object args)
        {
            return this._weasel.Invoke("Build", new object[] { args });
        }

        protected void ClearCache()
        {
            this._weasel.Invoke("ClearCache");
        }

        // Properties
        [MarkupVisible]
        public IList Sources
        {
            set
            {
                this._weasel.SetProperty("Sources", value);
            }
        }

        [DefaultValue(11), MarkupVisible]
        public AnimationEventType Type
        {
            get
            {
                return (AnimationEventType)this._weasel.GetProperty("Type");
            }
            set
            {
                this._weasel.SetProperty("Type", value);
            }
        }
    }

    [MarkupVisible]
    public interface IAnimationProvider
    {
        // Methods
        object Build(object args);

        // Properties
        [MarkupVisible]
        AnimationEventType Type { get; }
    }

    [MarkupVisible]
    public enum AnimationEventType
    {
        Show,
        Hide,
        Move,
        Size,
        Scale,
        Rotate,
        Alpha,
        GainFocus,
        LoseFocus,
        ContentChangeShow,
        ContentChangeHide,
        Idle
    }

    public class GradientOffsetKeyframe : BaseFloatKeyframe, IGradientKeyframe
    {
        private PrivateObjectWeasel _weasel = null;

        public GradientOffsetKeyframe()
        {
            this._weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.GradientOffsetKeyframe, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
        }

        // Methods
        public override void Apply(object visualTarget, float flValue)
        {
            this._weasel.Invoke("Apply", new object[] { visualTarget, flValue });
        }

        protected override object CreateProxy(object anim, object aseq)
        {
            return this._weasel.Invoke("CreateProxy", new object[] { anim, aseq });
        }

        public override float GetEffectiveValue(object objTarget, float flBaseValue, object args)
        {
            return (float)this._weasel.Invoke("GetEffectiveValue", new object[] { objTarget, flBaseValue, args });
        }

        // Properties
        public object Target
        {
            get
            {
                return this._weasel.GetProperty("Target");
            }
            set
            {
                this._weasel.SetProperty("Target", value);
            }
        }

        public override AnimationType Type
        {
            get
            {
                return (AnimationType)this._weasel.GetProperty("Type");
            }
        }
    }

    public enum AnimationType
    {
        Alpha = 2,
        Color = 3,
        First = 0,
        GradientColorMask = 7,
        GradientOffset = 6,
        Last = 7,
        NumTypes = 8,
        Position = 0,
        Rotate = 5,
        Scale = 4,
        Size = 1
    }

    public interface IGradientKeyframe
    {
        // Properties
        object Target { get; set; }
    }

    public abstract class BaseFloatKeyframe : BaseKeyframe
    {

        private PrivateObjectWeasel _weasel = null;

        // Methods
        public BaseFloatKeyframe()
        {
            this._weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.BaseFloatKeyframe, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
        }

        public override void Apply(object visualTarget, object args)
        {
            this._weasel.Invoke("Apply", new object[] { visualTarget, args });
        }

        public abstract void Apply(object visualTarget, float flValue);
        //{
        //    return this._weasel.Invoke("Apply", new object[] { visualTarget, flValue });
        //}

        public virtual float GetEffectiveValue(object objTarget, float flBaseValue, object args)
        {
            return (float)this._weasel.Invoke("GetEffectiveValue", new object[] { objTarget, flBaseValue, args });
        }

        public override void MagnifyValue(float flMagnify)
        {
            this._weasel.Invoke("MagnifyValue", new object[] { flMagnify });
        }
        protected override void PopulateAnimationWorker(object objTarget, object objAnimation, object interpolation, object args)
        {
            this._weasel.Invoke("PopulateAnimationWorker", new object[] { objTarget, objAnimation, interpolation, args });
        }

        // Properties
        protected virtual bool Multiply
        {
            get
            {
                return (bool)this._weasel.GetProperty("Multiply");
            }
        }
        public override object ObjectValue
        {
            get
            {
                return this._weasel.GetProperty("ObjectValue");
            }
        }

        [MarkupVisible]
        public float Value
        {
            get
            {
                return (float)this._weasel.GetProperty("Value");
            }
            set
            {
                this._weasel.SetProperty("Value", value);
            }
        }
    }

    public abstract class BaseKeyframe : ITypedKeyframe, IKeyframe, ICloneable
    {
        private PrivateObjectWeasel _weasel = null;
        // Methods
        public BaseKeyframe()
            : this(0f)
        {
        }
        public BaseKeyframe(float flTime)
        {
            this._weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.BaseKeyframe, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
        }

        public abstract void Apply(object visualTarget, object args);

        protected virtual object CreateProxy(object anim, object aseq)
        {
            return this._weasel.Invoke("CreateProxy", new object[] { anim, aseq });
        }

        public abstract void MagnifyValue(float flMagnify);

        void ITypedKeyframe.AddtoAnimation(object anim, object aseq, object args, ref object animation)
        {
            this._weasel.Invoke("AddtoAnimation", new object[] { anim, aseq, args, animation });
        }

        protected abstract void PopulateAnimationWorker(object objTarget, object animation, object interpolation, object args);

        object ICloneable.Clone()
        {
            return this._weasel.Invoke("Clone");
        }

        public override string ToString()
        {
            return (string)this._weasel.Invoke("ToString");
        }

        // Properties
        [MarkupVisible]
        public object Interpolation
        {
            get
            {
                return this._weasel.GetProperty("Interpolation");
            }
            set
            {
                this._weasel.SetProperty("Interpolation", value);
            }
        }
        AnimationType ITypedKeyframe.Type
        {
            get
            {
                return (AnimationType)this._weasel.GetProperty("Type");
            }
        }
        public abstract object ObjectValue
        {
            get;
            //get
            //{
            //    return this._weasel.GetProperty("ObjectValue");
            //}
        }
        [MarkupVisible]
        public KeyframeValueReference RelativeTo
        {
            get
            {
                return (KeyframeValueReference)this._weasel.GetProperty("RelativeTo");
            }
            set
            {
                this._weasel.SetProperty("RelativeTo", value);
            }
        }
        [MarkupVisible]
        public float Time
        {
            get
            {
                return (float)this._weasel.GetProperty("Time");
            }
            set
            {
                this._weasel.SetProperty("Time", value);
            }
        }
        public abstract AnimationType Type
        {
            get;
            //get
            //{
            //    return (AnimationType)this._weasel.GetProperty("Type");
            //}
        }
    }

    public interface ITypedKeyframe : IKeyframe, ICloneable
    {
        // Methods
        void AddtoAnimation(object anim, object aseq, object args, ref object animation);

        // Properties
        AnimationType Type { get; }
    }


    public interface IKeyframe : ICloneable
    {
        // Properties
        float Time { get; }
    }

    [MarkupVisible]
    public enum KeyframeValueReference
    {
        Absolute,
        Current,
        Final
    }

    public class GradientColorMaskKeyframe : BaseARGBColorKeyframe, IGradientKeyframe
    {
        private PrivateObjectWeasel _weasel = null;
        // Methods
        public GradientColorMaskKeyframe()
        {
            this._weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.GradientColorMaskKeyframe, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
        }
        public override void Apply(object visualTarget, Color clrValue)
        {
            this._weasel.Invoke("Apply", new object[] { visualTarget, clrValue });
        }

        protected override object CreateProxy(object anim, object aseq)
        {
            return this._weasel.Invoke("CreateProxy", new object[] { anim, aseq });
        }

        public override Color GetEffectiveValue(object objTarget, Color clrBaseValue, object args)
        {
            return (Color)this._weasel.Invoke("GetEffectiveValue", new object[] { objTarget, clrBaseValue, args });
        }

        private bool ValidateAttached()
        {
            return (bool)this._weasel.Invoke("ValidateAttached");
        }

        // Properties
        public object Target
        {
            get
            {
                return this._weasel.GetProperty("Target");
            }
            set
            {
                this._weasel.SetProperty("Target", value);
            }
        }
        public override AnimationType Type
        {
            get
            {
                return (AnimationType)this._weasel.GetProperty("Type");
            }
        }
    }

    public abstract class BaseARGBColorKeyframe : BaseColorKeyframe
    {
        private PrivateObjectWeasel _weasel = null;
        // Methods
        protected BaseARGBColorKeyframe()
        {
            this._weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.BaseARGBColorKeyframe, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
        }
        protected override void PopulateAnimationWorker(object objTarget, object objAnimation, object interpolation, object args)
        {
            this._weasel.Invoke("PopulateAnimationWorker", new object[] { objTarget, objAnimation, interpolation, args });
        }
    }

    public abstract class BaseColorKeyframe : BaseKeyframe
    {
        private PrivateObjectWeasel _weasel = null;
        // Methods
        public BaseColorKeyframe()
        {
            this._weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.BaseColorKeyframe, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
        }
        public override void Apply(object visualTarget, object args)
        {
            this._weasel.Invoke("Apply", new object[] { visualTarget, args });
        }

        public abstract void Apply(object visualTarget, Color clrValue);

        public virtual Color GetEffectiveValue(object objTarget, Color clrBaseValue, object args)
        {
            return (Color)this._weasel.Invoke("GetEffectiveValue", new object[] { objTarget, clrBaseValue, args });
        }

        protected int MagnifyChannel(byte bChannel, float flMagnify)
        {
            return (int)this._weasel.Invoke("MagnifyChannel", new object[] { bChannel, flMagnify });
        }

        public override void MagnifyValue(float flMagnify)
        {
            this._weasel.Invoke("MagnifyValue", new object[] { flMagnify });
        }

        protected override void PopulateAnimationWorker(object objTarget, object objAnimation, object interpolation, object args)
        {
            this._weasel.Invoke("PopulateAnimationWorker", new object[] { objTarget, objAnimation, interpolation, args });
        }

        // Properties
        public override object ObjectValue
        {
            get
            {
                return this._weasel.GetProperty("ObjectValue");
            }
        }

        [MarkupVisible]
        public Color Value
        {
            get
            {
                return (Color)this._weasel.GetProperty("Value");
            }
            set
            {
                this._weasel.SetProperty("Value", value);
            }
        }
    }

    public class Gradient : ViewItem
    {
        private PrivateObjectWeasel _weasel = null;

        // Methods
        static Gradient()
        {
            //this._weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.Gradient, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
        }

        //public Gradient(object zone)
        //{
        //    this._weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.Gradient, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", new object[] { zone });
        //}


        public Gradient(object zone)
        {
            this._weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.ViewItem, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", new object[] { zone });

        }

        public void AddRampValue(RampValue rv)
        {
            this._weasel.Invoke("AddRampValue", new object[] { rv });
        }

        public void AddRampValue(float value, float position)
        {
            this._weasel.Invoke("AddRampValue", new object[] { value, position });
        }

        public void AddRampValue(float value, float position, RelativeSpace relative)
        {
            this._weasel.Invoke("AddRampValue", new object[] { value, position, relative });
        }

        protected override void BuildAnimationWorker(ref object animation)
        {
            this._weasel.Invoke("BuildAnimationWorker", new object[] { animation });
        }
        public void ClearRamp()
        {
            this._weasel.Invoke("ClearRamp");
        }

        protected override void Dispose(bool fInDispose)
        {
            this._weasel.Invoke("Dispose", new object[] { fInDispose });
        }

        protected override void OnLayoutComplete(ViewItem sender, EventArgs args)
        {
            this._weasel.Invoke("OnLayoutComplete", new object[] { sender, args });
        }

        protected override void OnPaint(object dc)
        {
            this._weasel.Invoke("OnPaint", new object[] { dc });
        }

        // Properties
        [StudioProperty("Appearance")]
        public Color ColorMask
        {
            get
            {
                return (Color)this._weasel.GetProperty("ColorMask");
            }
            set
            {
                this._weasel.SetProperty("ColorMask", value);
            }
        }

        public IAnimationProvider ColorMaskAnimation
        {
            get
            {
                return (IAnimationProvider)this._weasel.GetProperty("ColorMaskAnimation");
            }
            set
            {
                this._weasel.SetProperty("ColorMaskAnimation", value);
            }
        }

        [StudioProperty("Appearance"), DefaultValue(0)]
        public object DefaultCoordinateSpace
        {
            get
            {
                return this._weasel.GetProperty("DefaultCoordinateSpace");
            }
            set
            {
                this._weasel.SetProperty("DefaultCoordinateSpace", value);
            }
        }

        [DefaultValue((float)0f)]
        public float Offset
        {
            get
            {
                return (float)this._weasel.GetProperty("Offset");
            }
            set
            {
                this._weasel.SetProperty("Offset", value);
            }
        }

        public IAnimationProvider OffsetAnimation
        {
            get
            {
                return (IAnimationProvider)this._weasel.GetProperty("OffsetAnimation");
            }
            set
            {
                this._weasel.SetProperty("OffsetAnimation", value);
            }
        }

        public string OffsetToAreaOfInterest
        {
            get
            {
                return (string)this._weasel.GetProperty("OffsetToAreaOfInterest");
            }
            set
            {
                this._weasel.SetProperty("OffsetToAreaOfInterest", value);
            }
        }

        [StudioProperty("Appearance"), DefaultValue(0)]
        public Orientation Orientation
        {
            get
            {
                return (Orientation)this._weasel.GetProperty("Orientation");
            }
            set
            {
                this._weasel.SetProperty("Orientation", value);
            }
        }

        public IList Ramp
        {
            set
            {
                this._weasel.SetProperty("Ramp", value);
            }
        }
    }

    [MarkupVisible]
    public class ViewItem : TreeNode, IPropertyObject, INavigationSite, IUiZoneDisplayChild, ITrackableUIElement, ITrackableUIElementEvents
    {
        private PrivateObjectWeasel _weasel = null;

        // Fields
        protected const char k_chIndexPrefix = '#';
        protected const char k_chPathDelimiter = ' ';
        protected const string k_strIndexPrefix = "#";

        // Events
        public event EventHandler DeepLayoutChange;
        public event EventHandler InitializationComplete;
        public event EventHandler LayoutComplete;

        event EventHandler UIChange;
        public event PaintHandler Paint;
        public event EventHandler PaintInvalid;
        public event EventHandler PreInitializationComplete;

        // Methods
        //static ViewItem()
        //{
        //    //this._weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.ViewItem, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
        //}
        //public ViewItem(object zone)
        //{
        //    this._weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.ViewItem, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", new object[] { zone });
        //}


        //public ViewItem(View viewOwner)
        //{
        //    this.CommonCreate(viewOwner);
        //}

        //public ViewItem(object zone)
        //{
        //    this._weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.ViewItem, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", new object[] { zone });

        //}
        //public ViewItem(View viewOwner);
        public bool ApplyAnimatableValue(AnimationEventType type, object args, object objOldValue, object objNewValue, bool fOnlyApplyIfAnimating)
        {
            return (bool)this._weasel.Invoke("ApplyAnimatableValue", new object[] { type, args, objOldValue, objNewValue, fOnlyApplyIfAnimating });
        }

        public void ApplyFinalAnimationState(object anim, object args)
        {
            this._weasel.Invoke("ApplyAnimatableValue", new object[] { anim, args });
        }

        public void ApplyLayoutOutputs(bool fVisibilityChanging)
        {
            this._weasel.Invoke("ApplyAnimatableValue", new object[] { fVisibilityChanging });
        }

        [MarkupVisible]
        public void AttachAnimation(IAnimationProvider animation)
        {
            this._weasel.Invoke("AttachAnimation", new object[] { animation });
        }

        protected virtual void BuildAnimationWorker(ref object animation)
        {
            this._weasel.Invoke("BuildAnimationWorker", new object[] { animation });
        }

        protected virtual object ChildForId(string strPath, out ViewItem viResult)
        {
            viResult = new ViewItem();
            return this._weasel.Invoke("ChildForId", new object[] { strPath, viResult });
        }

        public void ClearCachedMeasureInfo(bool fRecursive)
        {
            this._weasel.Invoke("ClearCachedMeasureInfo", new object[] { fRecursive });
        }

        internal void ClearStickyFocus()
        {
            this._weasel.Invoke("ClearStickyFocus");
        }

        public Vector3 ComputeEffectiveScale()
        {
            return (Vector3)this._weasel.Invoke("ClearStickyFocus");
        }

        [Conditional("DEBUG")]
        public void DEBUG_ValidatePostLayoutState()
        {
            this._weasel.Invoke("DEBUG_ValidatePostLayoutState");
        }

        internal void DeliverEffectiveScaleChange(bool fParentChanged)
        {
            this._weasel.Invoke("DeliverEffectiveScaleChange", new object[] { fParentChanged });
        }

        [MarkupVisible]
        public void DetachAnimation(AnimationEventType type)
        {
            this._weasel.Invoke("DetachAnimation", new object[] { type });
        }

        public override void DisconnectEventHandlers()
        {
            this._weasel.Invoke("DisconnectEventHandlers");
        }

        protected override void Dispose(bool fInDispose)
        {
            this._weasel.Invoke("Dispose", new object[] { fInDispose });
        }

        internal virtual void FaultInChild(string stChildId, object handler)
        {
            this._weasel.Invoke("FaultInChild", new object[] { stChildId, handler });
        }

        internal object FindChildFromPath(string stPath, out ViewItem viResult, out string failedComponent)
        {
            viResult = new ViewItem();
            failedComponent = string.Empty;

            return this._weasel.Invoke("FindChildFromPath", new object[] { stPath, viResult, failedComponent });
        }
        public IServiceProvider FindServiceProvider()
        {
            return (IServiceProvider)this._weasel.Invoke("FindServiceProvider");
        }

        protected void FirePropertyChanged(string stPropertyName)
        {
            this._weasel.Invoke("FirePropertyChanged", new object[] { stPropertyName });
        }

        [MarkupVisible]
        public void ForceContentChange()
        {
            this._weasel.Invoke("ForceContentChange");
        }

        public static void GetAccumulatedOffsetAndScale(object childStart, object childStop, out Vector3 vecParentOffsetPxl, out Vector3 vecScale)
        {
            vecParentOffsetPxl = new Vector3();
            vecScale = new Vector3();
            //this._weasel.Invoke("GetAccumulatedOffsetAndScale", new object[] { childStart, childStop, vecParentOffsetPxl, vecScale });
        }

        public IAnimationProvider GetAnimation(AnimationEventType type)
        {
            return (IAnimationProvider)this._weasel.Invoke("GetAnimation", new object[] { type });
        }

        public object GetCachedMeasureInfo()
        {
            return this._weasel.Invoke("GetCachedMeasureInfo");
        }

        internal object GetDescendantFocusRect()
        {
            return this._weasel.Invoke("GetCachedMeasureInfo");
        }

        public ExtendedLayoutOutput GetExtendedLayoutOutput(DataCookie idOutput)
        {
            return (ExtendedLayoutOutput)this._weasel.Invoke("GetAnimation", new object[] { idOutput });
        }

        internal AreaOfInterest GetLastKnownAreaOfInterest(string stId)
        {
            return (AreaOfInterest)this._weasel.Invoke("GetLastKnownAreaOfInterest", new object[] { stId });
        }

        protected List<AreaOfInterest> GetLastKnownAreasOfInterest()
        {
            return (List<AreaOfInterest>)this._weasel.Invoke("GetLastKnownAreasOfInterest");
        }

        public ILayoutInput GetLayoutInput(DataCookie idInput)
        {
            return (ILayoutInput)this._weasel.Invoke("GetLayoutInput", new object[] { idInput });
        }

        public object GetLayoutOutput()
        {
            return this._weasel.Invoke("GetLayoutOutput");
        }

        protected virtual string IdForChild(ViewItem viChild)
        {
            return (string)this._weasel.Invoke("IdForChild", new object[] { viChild });
        }

        public void InitializeTree(object pass)
        {
            this._weasel.Invoke("InitializeTree", new object[] { pass });
        }

        public bool IsOffscreen()
        {
            return (bool)this._weasel.Invoke("IsOffscreen");
        }

        public void MarkLayoutInvalid()
        {
            this._weasel.Invoke("MarkLayoutInvalid");
        }

        public void MarkLayoutOutputDirty(bool fForce)
        {
            this._weasel.Invoke("MarkLayoutOutputDirty", new object[] { fForce });
        }

        public void MarkPaintInvalid()
        {
            this._weasel.Invoke("MarkLayoutInvalid");
        }

        bool INavigationSite.ComputeBounds(out Vector3 vecPositionPxl, out Vector3 vecSizePxl)
        {
            vecPositionPxl = new Vector3();
            vecSizePxl = new Vector3();

            return (bool)this._weasel.Invoke("ComputeBounds", new object[] { vecPositionPxl, vecSizePxl });
        }

        bool INavigationSite.DoExternalNavigate(object dirSearch, object rcfStart)
        {
            return (bool)this._weasel.Invoke("DoExternalNavigate", new object[] { dirSearch, rcfStart });
        }
        INavigationSite INavigationSite.LookupChildById(object objUniqueId)
        {
            return (INavigationSite)this._weasel.Invoke("LookupChildById", new object[] { objUniqueId });
        }

        Rectangle ITrackableUIElement.EstimatePosition(IUiZoneDisplayChild ancestor)
        {
            return (Rectangle)this._weasel.Invoke("EstimatePosition", new object[] { ancestor });
        }

        [MarkupVisible]
        public void NavigateInto()
        {
            this._weasel.Invoke("NavigateInto");
        }

        [MarkupVisible]
        public void NavigateInto(bool isDefault)
        {
            this._weasel.Invoke("NavigateInto", new object[] { isDefault });
        }

        internal void NotifyEffectiveScaleChange(bool fForce)
        {
            this._weasel.Invoke("NotifyEffectiveScaleChange", new object[] { fForce });
        }

        protected virtual void OnEffectiveScaleChange()
        {
            this._weasel.Invoke("OnEffectiveScaleChange");
        }

        protected virtual void OnInitializationComplete(ViewItem sender, EventArgs args)
        {
            this._weasel.Invoke("OnInitializationComplete", new object[] { sender, args });
        }

        protected virtual void OnInitialize()
        {
            this._weasel.Invoke("OnInitialize");
        }

        protected virtual void OnLayoutComplete(ViewItem sender, EventArgs args)
        {
            this._weasel.Invoke("OnLayoutComplete", new object[] { sender, args });
        }

        protected virtual void OnLayoutVisibilityChanged()
        {
            this._weasel.Invoke("OnLayoutVisibilityChanged");
        }

        protected virtual void OnPaint(object dc)
        {
            this._weasel.Invoke("OnPaint", new object[] { dc });
        }

        //protected override void OnParentChanged(object nodeOldParent, object nodeNewParent)
        //{
        //    this._weasel.Invoke("OnParentChanged", new object[] { nodeOldParent, nodeNewParent });
        //}

        protected virtual void OnPreInitializationComplete(ViewItem sender, EventArgs args)
        {
            this._weasel.Invoke("OnPreInitializationComplete", new object[] { sender, args });
        }

        protected virtual void OnPreInitialize()
        {
            this._weasel.Invoke("OnPreInitialize");
        }

        protected virtual void OnPropertyChanged(string property)
        {
            this._weasel.Invoke("OnPropertyChanged", new object[] { property });
        }

        public void OnScaleChange(Vector3 vecOldScale, Vector3 vecNewScale)
        {
            this._weasel.Invoke("OnScaleChange", new object[] { vecOldScale, vecNewScale });
        }

        protected virtual void OnVisualChanged()
        {
            this._weasel.Invoke("OnVisualChanged");
        }

        public void PaintTree(object packet)
        {
            this._weasel.Invoke("PaintTree", new object[] { packet });
        }

        public bool PlayAnimation(AnimationEventType type)
        {
            return (bool)this._weasel.Invoke("PlayAnimation", new object[] { type });
        }

        public bool PlayAnimation(IAnimationProvider ab)
        {
            return (bool)this._weasel.Invoke("PlayAnimation", new object[] { ab });
        }


        public void PlayHideAnimation(object visual, object orphans)
        {
            this._weasel.Invoke("PlayHideAnimation", new object[] { visual, orphans });
        }

        public void PlayShowAnimation()
        {
            this._weasel.Invoke("PlayShowAnimation");
        }

        public void ResendExistingContentTree()
        {
            this._weasel.Invoke("ResendExistingContentTree");
        }
        public void ResetLayoutInvalid()
        {
            this._weasel.Invoke("ResetLayoutInvalid");
        }

        public void ResumeLayout(bool fResetLayoutInvalid)
        {
            this._weasel.Invoke("ResumeLayout", new object[] { fResetLayoutInvalid });
        }


        internal void SetBlueprintId(int id)
        {
            this._weasel.Invoke("SetBlueprintId", new object[] { id });
        }

        public void SetLayoutInput(ILayoutInput oNewValue)
        {
            this._weasel.Invoke("SetLayoutInput", new object[] { oNewValue });
        }

        public void SetLayoutInput(DataCookie idInput, ILayoutInput oNewValue)
        {
            this._weasel.Invoke("SetLayoutInput", new object[] { idInput, oNewValue });
        }

        public void StoreCachedMeasureInfo(object results)
        {
            this._weasel.Invoke("StoreCachedMeasureInfo", new object[] { results });
        }

        public virtual void StoreLayoutOutput(object loNewOutput)
        {
            this._weasel.Invoke("StoreLayoutOutput", new object[] { loNewOutput });
        }

        public void SuspendLayout()
        {
            this._weasel.Invoke("SuspendLayout");
        }

        public override string ToString()
        {
            return (string)this._weasel.Invoke("ToString");
        }

        public void Uninitialize()
        {
            this._weasel.Invoke("Uninitialize");
        }

        public void Update(bool fRecursive)
        {
            this._weasel.Invoke("Update", new object[] { fRecursive });
        }


        // Properties
        [StudioProperty("Visual Layout"), MarkupVisible]
        public float Alpha
        {
            get
            {
                return (float)this._weasel.GetProperty("Alpha");
            }
            set
            {
                this._weasel.SetProperty("Alpha", value);
            }
        }

        public object Animation
        {
            set
            {
                this._weasel.SetProperty("Animation", value);
            }
        }

        [MarkupVisible]
        public IList Animations
        {
            set
            {
                this._weasel.SetProperty("Animations", value);
            }
        }

        [StudioProperty(PropertyFlags.DebugOnly, "Animations"), DefaultValue((string)null)]
        public object AnimationSet
        {
            get
            {
                return this._weasel.GetProperty("AnimationSet");
            }
        }

        [MarkupVisible, StudioProperty("Visual Layout")]
        public Vector3 CenterPointOffset
        {
            get
            {
                return (Vector3)this._weasel.GetProperty("CenterPointOffset");
            }
            set
            {
                this._weasel.SetProperty("CenterPointOffset", value);
            }
        }

        [MarkupVisible, StudioProperty("Visual Layout")]
        public Vector3 CenterPointPercent
        {
            get
            {
                return (Vector3)this._weasel.GetProperty("CenterPointPercent");
            }
            set
            {
                this._weasel.SetProperty("CenterPointPercent", value);
            }
        }

        [MarkupVisible]
        public IList Children
        {
            get
            {
                return (IList)this._weasel.GetProperty("Children");
            }
        }

        [StudioProperty("Visual Layout"), MarkupVisible]
        public Color ColorFilter
        {
            get
            {
                return (Color)this._weasel.GetProperty("ColorFilter");
            }
            set
            {
                this._weasel.SetProperty("ColorFilter", value);
            }
        }

        [MarkupVisible]
        public Color DebugOutline
        {
            get
            {
                return (Color)this._weasel.GetProperty("DebugOutline");
            }
            set
            {
                this._weasel.SetProperty("DebugOutline", value);
            }
        }

        [StudioProperty("Input"), DefaultValue(false)]
        public bool EffectiveMouseInteractive
        {
            get
            {
                return (bool)this._weasel.GetProperty("EffectiveMouseInteractive");
            }
        }

        [StudioProperty("Input"), MarkupVisible, DefaultValue(0x7fffffff)]
        public int FocusOrder
        {
            get
            {
                return (int)this._weasel.GetProperty("FocusOrder");
            }
            set
            {
                this._weasel.SetProperty("FocusOrder", value);
            }
        }
        public string IdPath
        {
            get
            {
                return (string)this._weasel.GetProperty("IdPath");
            }
        }

        [DefaultValue(false)]
        public bool IsDisposed
        {
            get
            {
                return (bool)this._weasel.GetProperty("IsDisposed");
            }
        }

        [StudioProperty(PropertyFlags.NonBrowsable), DefaultValue(false)]
        public bool IsInitialized
        {
            get
            {
                return (bool)this._weasel.GetProperty("IsInitialized");
            }
        }

        [StudioProperty("Visual Layout")]
        public uint Layer
        {
            get
            {
                return (uint)this._weasel.GetProperty("Layer");
            }
            set
            {
                this._weasel.SetProperty("Layer", value);
            }
        }

        [MarkupVisible, DefaultValue((string)null), StudioProperty("Visual Layout")]
        public ILayout Layout
        {
            get
            {
                return (ILayout)this._weasel.GetProperty("Layout");
            }
            set
            {
                this._weasel.SetProperty("Layout", value);
            }
        }

        [MarkupVisible]
        public ILayoutInput LayoutInput
        {
            set
            {
                this._weasel.SetProperty("LayoutInput", value);
            }
        }

        [DefaultValue(false), StudioProperty(PropertyFlags.DebugOnly, "Visual Layout")]
        public bool LayoutInvalid
        {
            get
            {
                return (bool)this._weasel.GetProperty("LayoutInvalid");
            }
        }

        [StudioProperty(PropertyFlags.DebugOnly, "Visual Layout"), DefaultValue(false)]
        public bool LayoutSuspended
        {
            get
            {
                return (bool)this._weasel.GetProperty("LayoutSuspended");
            }
        }

        [MarkupVisible, StudioProperty("Visual Layout")]
        public Inset Margins
        {
            get
            {
                return (Inset)this._weasel.GetProperty("Margins");
            }
            set
            {
                this._weasel.SetProperty("Margins", value);
            }
        }

        [MarkupVisible, StudioProperty("Visual Layout")]
        public Size MaximumSize
        {
            get
            {
                return (Size)this._weasel.GetProperty("MaximumSize");
            }
            set
            {
                this._weasel.SetProperty("MaximumSize", value);
            }
        }

        ICollection INavigationSite.Children
        {
            get
            {
                return (ICollection)this._weasel.GetProperty("Children");
            }
        }

        string INavigationSite.Description
        {
            get
            {
                return (string)this._weasel.GetProperty("Description");
            }
        }

        int INavigationSite.FocusOrder
        {
            get
            {
                return (int)this._weasel.GetProperty("FocusOrder");
            }
        }
        bool INavigationSite.IsLogicalJunction
        {
            get
            {
                return (bool)this._weasel.GetProperty("IsLogicalJunction");
            }
        }

        object INavigationSite.Mode
        {
            get
            {
                return this._weasel.GetProperty("Mode");
            }
        }

        object INavigationSite.Navigability
        {
            get
            {
                return this._weasel.GetProperty("Navigability");
            }
        }

        INavigationSite INavigationSite.Parent
        {
            get
            {
                return (INavigationSite)this._weasel.GetProperty("Parent");
            }
        }

        object INavigationSite.StateCache
        {
            get
            {
                return this._weasel.GetProperty("StateCache");
            }
            set
            {
                this._weasel.SetProperty("StateCache", value);
            }
        }

        object INavigationSite.UniqueId
        {
            get
            {
                return this._weasel.GetProperty("UniqueId");
            }
        }

        bool INavigationSite.Visible
        {
            get
            {
                return (bool)this._weasel.GetProperty("Visible");
            }
        }

        bool ITrackableUIElement.IsUIVisible
        {
            get
            {
                return (bool)this._weasel.GetProperty("IsUIVisible");
            }
        }

        object IUiZoneDisplayChild.Transforms
        {
            get
            {
                return this._weasel.GetProperty("Transforms");
            }
        }

        [StudioProperty("Visual Layout"), MarkupVisible]
        public Size MinimumSize
        {
            get
            {
                return (Size)this._weasel.GetProperty("MinimumSize");
            }
            set
            {
                this._weasel.SetProperty("MinimumSize", value);
            }
        }

        [MarkupVisible, StudioProperty("Input"), DefaultValue(false)]
        public bool MouseInteractive
        {
            get
            {
                return (bool)this._weasel.GetProperty("MouseInteractive");
            }
            set
            {
                this._weasel.SetProperty("MouseInteractive", value);
            }
        }

        [StudioProperty("State"), MarkupVisible, DefaultValue((string)null)]
        public string Name
        {
            get
            {
                return (string)this._weasel.GetProperty("Name");
            }
            set
            {
                this._weasel.SetProperty("Name", value);
            }
        }

        [StudioProperty("Input"), DefaultValue(0), MarkupVisible]
        public object Navigation
        {
            get
            {
                return this._weasel.GetProperty("Navigation");
            }
            set
            {
                this._weasel.SetProperty("Navigation", value);
            }
        }

        [StudioProperty("Visual Layout"), MarkupVisible]
        public Inset Padding
        {
            get
            {
                return (Inset)this._weasel.GetProperty("Padding");
            }
            set
            {
                this._weasel.SetProperty("Padding", value);
            }
        }

        [StudioProperty(PropertyFlags.NonBrowsable)]
        public ViewItem Parent
        {
            get
            {
                return (ViewItem)this._weasel.GetProperty("Parent");
            }
        }

        [StudioProperty("Visual Layout"), MarkupVisible]
        public Rotation Rotation
        {
            get
            {
                return (Rotation)this._weasel.GetProperty("Rotation");
            }
            set
            {
                this._weasel.SetProperty("Rotation", value);
            }
        }
        [StudioProperty("Visual Layout"), MarkupVisible]
        public Vector3 Scale
        {
            get
            {
                return (Vector3)this._weasel.GetProperty("Scale");
            }
            set
            {
                this._weasel.SetProperty("Scale", value);
            }
        }
        [StudioProperty(PropertyFlags.DebugOnly, "Appearance"), DefaultValue(false)]
        public bool SkipCutOut
        {
            get
            {
                return (bool)this._weasel.GetProperty("SkipCutOut");
            }
            set
            {
                this._weasel.SetProperty("SkipCutOut", value);
            }
        }
        public bool TraceLayout
        {
            get
            {
                return (bool)this._weasel.GetProperty("TraceLayout");
            }
            set
            {
                this._weasel.SetProperty("TraceLayout", value);
            }
        }
        [StudioProperty(PropertyFlags.DebugOnly, "Behavior")]
        public object View
        {
            get
            {
                return this._weasel.GetProperty("View");
            }
        }

        [DefaultValue(true), StudioProperty("Visual Layout"), MarkupVisible]
        public bool Visible
        {
            get
            {
                return (bool)this._weasel.GetProperty("Visible");
            }
            set
            {
                this._weasel.SetProperty("Visible", value);
            }
        }
        [StudioProperty(PropertyFlags.DebugOnly, "Appearance"), DefaultValue((string)null)]
        public object Visual
        {
            get
            {
                return this._weasel.GetProperty("Visual");
            }
            set
            {
                this._weasel.SetProperty("Visual", value);
            }
        }
        [StudioProperty(PropertyFlags.NonBrowsable)]
        public object Zone
        {
            get
            {
                return (this._weasel.GetProperty("Zone"));
            }
        }

        // Nested Types
        //private enum Bits : uint
        //{
        //    ActiveAnimations = 0x20000,
        //    AnimationBuilders = 0x40000,
        //    DeepLayoutNotifySelf = 0x1000000,
        //    DeepLayoutNotifyTree = 0x2000000,
        //    HasCachedMeasureInfo = 0x400000,
        //    HasFocusOrder = 0x200000,
        //    HasNavMode = 0x100000,
        //    HasScale = 0x40,
        //    IdleAnimation = 0x80000,
        //    InsideContentChange = 0x4000000,
        //    IsDisposed = 0x80000000,
        //    LayoutInputMargins = 0x400,
        //    LayoutInputMaxSize = 0x100,
        //    LayoutInputMinSize = 0x200,
        //    LayoutInputPadding = 0x800,
        //    LayoutInputVisible = 0x1000,
        //    LayoutInvalid = 0x8000,
        //    LayoutSuspended = 0x10000,
        //    MouseInteractive = 0x10,
        //    OutputSelfDirty = 0x2000,
        //    OutputTreeDirty = 0x4000,
        //    PaintInvalid = 0x20,
        //    ScaleChanged = 0x80,
        //    SelfInitialized = 4,
        //    SelfPreInitialized = 1,
        //    SkipCutOut = 0x800000,
        //    TreeInitialized = 8,
        //    TreePreInitialized = 2
        //}

        //[StructLayout(LayoutKind.Sequential)]
        //private struct LayoutApplyParams
        //{
        //    public bool fFullyVisible;
        //    public bool fVisibilityChanging;
        //    public bool fAllowAnimations;
        //    public bool fDeepLayoutChanged;
        //    public bool fAnyDeepChangesDelivered;
        //}

        //public class PaintArgs : EventArgs
        //{
        //    // Fields
        //    private DrawingContext m_dc;

        //    // Methods
        //    public PaintArgs(DrawingContext dc);

        //    // Properties
        //    public DrawingContext DrawingContext { get; }
        //}

        public delegate void PaintHandler(ViewItem viSender, object args);

        #region IPropertyObject Members

        event Microsoft.MediaCenter.UI.PropertyChangedEventHandler IPropertyObject.PropertyChanged
        {
            add { throw new Exception("The method or operation is not implemented."); }
            remove { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class StudioPropertyAttribute : Attribute
    {
        // Fields
        private static StudioPropertyAttribute Default = new StudioPropertyAttribute(PropertyFlags.Browsable);
        private string m_Category;
        private PropertyFlags m_fPropertyFlags;


        // Methods
        public StudioPropertyAttribute(PropertyFlags flags)
        {
            this.m_Category = null;
            this.m_fPropertyFlags = flags;
        }

        public StudioPropertyAttribute(string category)
        {
            this.m_fPropertyFlags = PropertyFlags.Browsable;
            this.m_Category = category;
        }

        public StudioPropertyAttribute(PropertyFlags flags, string category)
        {
            this.m_fPropertyFlags = flags;
            this.m_Category = category;
        }

        public static StudioPropertyAttribute FromDescriptor(PropertyDescriptor property)
        {
            if (property != null)
            {
                foreach (Attribute attribute in property.Attributes)
                {
                    StudioPropertyAttribute attribute2 = attribute as StudioPropertyAttribute;
                    if (attribute2 != null)
                    {
                        return attribute2;
                    }
                }
            }
            return Default;
        }

        public bool ShowProperty(bool fStudioDebugMode)
        {
            return ((this.m_fPropertyFlags == PropertyFlags.Browsable) || ((this.m_fPropertyFlags == PropertyFlags.DebugOnly) && fStudioDebugMode));
        }

        // Properties
        public string Category
        {
            get
            {
                return this.m_Category;
            }
        }

        public PropertyFlags PropertyFlags
        {
            get
            {
                return this.m_fPropertyFlags;
            }
        }
    }

    public enum PropertyFlags
    {
        NonBrowsable,
        DebugOnly,
        Browsable
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RampValue
    {
        private float m_flValue;
        private float m_flPosition;
        private RelativeSpace m_relative;
        public RampValue(float flValue, float flPosition)
            : this(flValue, flPosition, RelativeSpace.Unspecified)
        {
        }

        public RampValue(float flValue, float flPosition, RelativeSpace relative)
        {
            this.m_flValue = flValue;
            this.m_flPosition = flPosition;
            this.m_relative = relative;
        }

        public float Value
        {
            get
            {
                return this.m_flValue;
            }
            set
            {
                this.m_flValue = value;
            }
        }
        public float Position
        {
            get
            {
                return this.m_flPosition;
            }
            set
            {
                this.m_flPosition = value;
            }
        }
        public RelativeSpace Relative
        {
            get
            {
                return this.m_relative;
            }
            set
            {
                this.m_relative = value;
            }
        }
    }

    public enum RelativeSpace
    {
        GadgetMax = 1,
        GadgetMin = 0,
        Global = 4,
        MeshMax = 3,
        MeshMin = 2,
        Unspecified = -1
    }

    public interface ITrackableUIElementEvents
    {
        // Events
        //event EventHandler UIChange;
    }


    public interface ITrackableUIElement
    {
        // Methods
        Rectangle EstimatePosition(IUiZoneDisplayChild ancestor);

        // Properties
        bool IsUIVisible { get; }
    }

    public interface IUiZoneDisplayChild
    {
        // Properties
        object Transforms { get; }
    }

    public interface INavigationSite
    {
        // Methods
        bool ComputeBounds(out Vector3 vecPositionPxl, out Vector3 vecSizePxl);
        bool DoExternalNavigate(object dirSearch, object rcfStart);
        INavigationSite LookupChildById(object objUniqueId);

        // Properties
        ICollection Children { get; }
        string Description { get; }
        int FocusOrder { get; }
        bool IsLogicalJunction { get; }
        object Mode { get; }
        object Navigability { get; }
        INavigationSite Parent { get; }
        object StateCache { get; set; }
        object UniqueId { get; }
        bool Visible { get; }
    }

    public abstract class TreeNode : ITreeNode, IUiZoneChild
    {
        private PrivateObjectWeasel _weasel = null;

        // Events
        public event EventHandler DeepParentChange;

        // Methods
        public TreeNode()
        {
            //this._weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.TreeNode, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
        }
        public TreeNode(object tree)
        {
            this._weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.TreeNode, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", new object[] { tree });
        }
        protected bool AddEventHandler(object cookie, Delegate handlerToAdd)
        {
            return (bool)this._weasel.Invoke("AddEventHandler", new object[] { cookie, handlerToAdd });
        }

        public static object AllocExtenderId()
        {
            return null;
        }

        public void ChangeParent(TreeNode nodeNewParent)
        {
            this._weasel.Invoke("ChangeParent", new object[] { nodeNewParent });
        }

        public void ChangeParent(TreeNode nodeNewParent, TreeNode nodeSibling, object lt)
        {
            this._weasel.Invoke("ChangeParent", new object[] { nodeNewParent, nodeSibling, lt });
        }

        [Conditional("DEBUG")]
        public void DEBUG_CheckGeneration(int idxIntendedGeneration)
        {
            this._weasel.Invoke("DEBUG_CheckGeneration", new object[] { idxIntendedGeneration });
        }

        [Conditional("DEBUG")]
        public virtual void DEBUG_UpdateSpy()
        {
            this._weasel.Invoke("DEBUG_UpdateSpy");
        }

        public virtual void DisconnectEventHandlers()
        {
            this._weasel.Invoke("DisconnectEventHandlers");
        }

        public void Dispose()
        {
            this._weasel.Invoke("Dispose");
        }

        protected virtual void Dispose(bool fInDispose)
        {
            this._weasel.Invoke("Dispose", new object[] { fInDispose });
        }



        public override bool Equals(object oRHS)
        {
            return (bool)this._weasel.Invoke("Equals", new object[] { oRHS });
        }
        //protected override void Finalize()
        //{
        //    this._weasel.Invoke("Finalize");
        //}

        protected object GetData(DataCookie cookie)
        {
            return this._weasel.Invoke("GetData", new object[] { cookie });
        }

        protected Delegate GetEventHandler(object cookie)
        {
            return (Delegate)this._weasel.Invoke("GetEventHandler", new object[] { cookie });
        }
        public object GetExtender(object id)
        {
            return this._weasel.Invoke("GetExtender", new object[] { id });
        }

        public override int GetHashCode()
        {
            return (int)this._weasel.Invoke("GetHashCode");
        }


        public bool HasDescendant(TreeNode nodeOther)
        {
            return (bool)this._weasel.Invoke("HasDescendant", new object[] { nodeOther });
        }

        public void InvokeCallback(object infoInvoke)
        {
            this._weasel.Invoke("InvokeCallback", new object[] { infoInvoke });
        }

        public void MoveNode(TreeNode nodeSibling, object lt)
        {
            this._weasel.Invoke("MoveNode", new object[] { nodeSibling, lt });
        }

        protected virtual void OnParentChanged(TreeNode nodeOldParent, TreeNode nodeNewParent)
        {
            this._weasel.Invoke("OnParentChanged", new object[] { nodeOldParent, nodeNewParent });
        }

        public void RegisterCallbackHandler(object handlerNew)
        {
            this._weasel.Invoke("RegisterCallbackHandler", new object[] { handlerNew });
        }

        public void RemoveAllChildren(bool fDisposeChildren)
        {
            this._weasel.Invoke("RemoveAllChildren", new object[] { fDisposeChildren });
        }

        protected bool RemoveEventHandler(object cookie, Delegate handlerToRemove)
        {
            return (bool)this._weasel.Invoke("RemoveEventHandler", new object[] { cookie, handlerToRemove });
        }

        protected void RemoveEventHandlers(object cookie)
        {
            this._weasel.Invoke("RemoveEventHandlers", new object[] { cookie });
        }

        protected void SetData(DataCookie cookie, object value)
        {
            this._weasel.Invoke("SetData", new object[] { cookie, value });
        }

        public void SetExtender(object id, object extender)
        {
            this._weasel.Invoke("SetExtender", new object[] { id, extender });
        }

        public bool TestNodes(NodeTest test, object nScope, bool fParentChainsMustGoToRoot)
        {
            return (bool)this._weasel.Invoke("TestNodes", new object[] { test, nScope, fParentChainsMustGoToRoot });
        }

        // Properties
        [StudioProperty(PropertyFlags.NonBrowsable)]
        public object Ancestors
        {
            get
            {
                return this._weasel.GetProperty("Ancestors");
            }
        }
        [StudioProperty(PropertyFlags.DebugOnly, "State")]
        public int ChildCount
        {
            get
            {
                return (int)this._weasel.GetProperty("ChildCount");
            }
        }
        public int DEBUG_CurrentGeneration
        {
            get
            {
                return (int)this._weasel.GetProperty("DEBUG_CurrentGeneration");
            }
        }
        [StudioProperty(PropertyFlags.NonBrowsable)]
        public object FastChildren
        {
            get
            {
                return this._weasel.GetProperty("FastChildren");
            }
        }
        [StudioProperty(PropertyFlags.NonBrowsable)]
        public TreeNode FirstChild
        {
            get
            {
                return (TreeNode)this._weasel.GetProperty("FirstChild");
            }
        }
        [StudioProperty(PropertyFlags.NonBrowsable)]
        public TreeNode FirstSibling
        {
            get
            {
                return (TreeNode)this._weasel.GetProperty("FirstSibling");
            }
        }
        [StudioProperty(PropertyFlags.NonBrowsable)]
        public bool HasChildren
        {
            get
            {
                return (bool)this._weasel.GetProperty("HasChildren");
            }
        }
        [StudioProperty(PropertyFlags.DebugOnly, "State")]
        public int InstanceID
        {
            get
            {
                return (int)this._weasel.GetProperty("InstanceID");
            }
        }

        private static EventHandler s_handlerTreeChange;
        [StudioProperty(PropertyFlags.NonBrowsable)]
        public static EventHandler InternalTreeWatcher
        {
            get
            {
                return s_handlerTreeChange;
                //return (EventHandler)this._weasel.GetProperty("InternalTreeWatcher");
            }
            set
            {
                //this._weasel.SetProperty("InternalTreeWatcher", value);
            }
        }
        [StudioProperty(PropertyFlags.NonBrowsable)]
        public bool IsRoot
        {
            get
            {
                return (bool)this._weasel.GetProperty("IsRoot");
            }
        }
        [StudioProperty(PropertyFlags.NonBrowsable)]
        public TreeNode LastChild
        {
            get
            {
                return (TreeNode)this._weasel.GetProperty("LastChild");
            }
        }
        [StudioProperty(PropertyFlags.NonBrowsable)]
        public TreeNode LastSibling
        {
            get
            {
                return (TreeNode)this._weasel.GetProperty("LastSibling");
            }
        }
        ICollection ITreeNode.Children
        {
            get
            {
                return (ICollection)this._weasel.GetProperty("Children");
            }
        }
        bool ITreeNode.HasChildren
        {
            get
            {
                return (bool)this._weasel.GetProperty("HasChildren");
            }
        }
        ITreeNode ITreeNode.Parent
        {
            get
            {
                return (ITreeNode)this._weasel.GetProperty("Parent");
            }
        }
        object IUiZoneChild.Zone
        {
            get
            {
                return this._weasel.GetProperty("Zone");
            }
        }
        [StudioProperty(PropertyFlags.NonBrowsable)]
        public TreeNode NextSibling
        {
            get
            {
                return (TreeNode)this._weasel.GetProperty("NextSibling");
            }
        }
        [StudioProperty(PropertyFlags.NonBrowsable)]
        public TreeNode Parent
        {
            get
            {
                return (TreeNode)this._weasel.GetProperty("Parent");
            }
        }
        [StudioProperty(PropertyFlags.NonBrowsable)]
        public TreeNode PreviousSibling
        {
            get
            {
                return (TreeNode)this._weasel.GetProperty("PreviousSibling");
            }
        }
        [StudioProperty(PropertyFlags.NonBrowsable)]
        public object Tree
        {
            get
            {
                return this._weasel.GetProperty("Tree");
            }
        }
        [StudioProperty(PropertyFlags.NonBrowsable)]
        public object UiSession
        {
            get
            {
                return this._weasel.GetProperty("UiSession");
            }
        }
        [StudioProperty(PropertyFlags.NonBrowsable)]
        public object Zone
        {
            get
            {
                return this._weasel.GetProperty("Zone");
            }
        }

        // Nested Types
        //[StructLayout(LayoutKind.Sequential)]
        //public struct ExtenderId
        //{
        //    public DataCookie _cookie;
        //    public ExtenderId(DataCookie cookie);
        //}

        //public enum LinkType
        //{
        //    Before = 1,
        //    Behind = 2,
        //    First = 3,
        //    Last = 4
        //}

        //[Flags]
        //public enum NodeRelation
        //{
        //    None,
        //    Self,
        //    Parents,
        //    FullChain
        //}

        public delegate bool NodeTest(TreeNode node);
    }

    public interface IUiZoneChild
    {
        // Properties
        object Zone { get; }
    }


    public interface ITreeNode
    {
        // Properties
        [StudioProperty(PropertyFlags.NonBrowsable)]
        ICollection Children { get; }
        [StudioProperty(PropertyFlags.NonBrowsable)]
        bool HasChildren { get; }
        [StudioProperty(PropertyFlags.NonBrowsable)]
        ITreeNode Parent { get; }
    }

}
