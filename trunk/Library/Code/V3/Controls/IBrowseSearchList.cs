using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Microsoft.MediaCenter.UI;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.Text.RegularExpressions;
using System.Drawing;

namespace Library.Code.V3
{
    public interface IBrowseSearchList
    {
        // Methods
        int FindContentItem(string strSearch);

        // Properties
        bool SupportsJIL { get; }
    }

    public interface IBrowseGroup
    {
        // Properties
        IList Content { get; }
        string ContentLabelTemplate { get; }
        string Description { get; }
    }

    //public class BrowseGroup : ModelItem, IBrowseGroup
    //{
    //    // Fields
    //    private IList m_list;
    //    private string m_stContentLabelTemplate;

    //    // Methods
    //    public BrowseGroup(IModelItemOwner owner, string stDescription, IList list)
    //        : base(owner, stDescription)
    //    {
    //        this.m_list = list;
    //        this.m_stContentLabelTemplate = StandardContentLabelTemplate;
    //    }

    //    // Methods
    //    public BrowseGroup()
    //        : base()
    //    {
    //        //this.m_list = list;
    //        this.m_stContentLabelTemplate = StandardContentLabelTemplate;
    //    }

    //    // Properties
    //    public IList Content
    //    {
    //        get
    //        {
    //            return this.m_list;
    //        }
    //        set
    //        {
    //            this.m_list = value;
    //        }
    //    }

    //    public string ContentLabelTemplate
    //    {
    //        get
    //        {
    //            return this.m_stContentLabelTemplate;
    //        }
    //        set
    //        {
    //            if (this.m_stContentLabelTemplate != value)
    //            {
    //                this.m_stContentLabelTemplate = value;
    //                base.FirePropertyChanged("ContentLabelTemplate");
    //            }
    //        }
    //    }

    //    public static string StandardContentLabelTemplate
    //    {
    //        get
    //        {
    //            return "resx://Library/Library.Resources/V3_Controls_BrowseGroupedGallery#TextContentLabel";
    //        }
    //    }
    //}


    [MarkupVisible]
    public class NowPlaying : ViewItem, INowPlayingInsetPlaceholder, ITrackableUIElement, ICustomNavigationJunction
    {
        // Fields
        private PrivateObjectWeasel _weasel = null;

        // Events
        ////event EventHandler INowPlayingInsetPlaceholder.OptionsChange;
        #region INowPlayingInsetPlaceholder Members

        public event EventHandler OptionsChange;

        #endregion

        // Methods
        [MarkupVisible]
        public NowPlaying(object viewOwner)
        {
            this._weasel = new PrivateObjectWeasel("Microsoft.MediaCenter.UI.NowPlaying, Microsoft.MediaCenter.UI, Version=6.0.6000.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", new object[] { viewOwner });
        }

        protected override void Dispose(bool fInDispose)
        {
            this._weasel.Invoke("Dispose");
        }

        bool ICustomNavigationJunction.NavigateIn(Direction dirSearch, RectangleF rcfStart, bool fIsDefault)
        {
            return (bool)this._weasel.Invoke("NavigateIn", new object[] { dirSearch, rcfStart, fIsDefault });
        }

        bool ICustomNavigationJunction.NavigateOut(Direction dirSearch, RectangleF rcfStart)
        {
            return (bool)this._weasel.Invoke("NavigateOut", new object[] { dirSearch, rcfStart });
        }

        void INowPlayingInsetPlaceholder.OnInsetActive(bool fInsetActive)
        {
            this._weasel.Invoke("OnInsetActive", new object[] { fInsetActive });
        }

        void INowPlayingInsetPlaceholder.OnInsetFocused(bool fInsetFocused)
        {
            this._weasel.Invoke("OnInsetFocused", new object[] { fInsetFocused });
        }

        //private void NowPlayingOptionsChanged() { }
        //private void OnInputPropertyChange(object sender, string propName) { }

        // Properties
        public Microsoft.MediaCenter.UI.Color BorderColor
        {
            get
            {
                return (Microsoft.MediaCenter.UI.Color)this._weasel.GetProperty("BorderColor");
            }
            set
            {
                this._weasel.SetProperty("BorderColor", value);
            }
        }

        public Microsoft.MediaCenter.UI.Color FocusBorderColor
        {
            get
            {
                return (Microsoft.MediaCenter.UI.Color)this._weasel.GetProperty("FocusBorderColor");
            }
            set
            {
                this._weasel.SetProperty("FocusBorderColor", value);
            }
        }

        [MarkupVisible]
        public bool IsActive
        {
            get
            {
                return (bool)this._weasel.GetProperty("IsActive");
            }
        }

        public bool IsInsetFocused
        {
            get
            {
                return (bool)this._weasel.GetProperty("IsInsetFocused");
            }
        }

        NowPlayingInsetOptions INowPlayingInsetPlaceholder.DesiredOptions
        {
            get
            {
                return (NowPlayingInsetOptions)this._weasel.GetProperty("DesiredOptions");
            }
        }

        IUiZoneChild INowPlayingInsetPlaceholder.InputChildLink
        {
            get
            {
                return (IUiZoneChild)this._weasel.GetProperty("InputChildLink");
            }
        }

        [MarkupVisible]
        public MetadataVisibility ShowFullMetadata
        {
            get
            {
                return (MetadataVisibility)this._weasel.GetProperty("ShowFullMetadata");
            }
            set
            {
                this._weasel.SetProperty("ShowFullMetadata", value);
            }
        }

        [MarkupVisible]
        public bool SnapToDefaultPosition
        {
            get
            {
                return (bool)this._weasel.GetProperty("SnapToDefaultPosition");
            }
            set
            {
                this._weasel.SetProperty("SnapToDefaultPosition", value);
            }
        }

        public PipVisualState VisualState
        {
            get
            {
                return (PipVisualState)this._weasel.GetProperty("VisualState");
            }
            set
            {
                this._weasel.SetProperty("VisualState", value);
            }
        }

    }

    public interface INowPlayingInsetPlaceholder : ITrackableUIElement
    {
        // Events
        event EventHandler OptionsChange;

        // Methods
        void OnInsetActive(bool fInsetActive);
        void OnInsetFocused(bool fInsetFocused);

        // Properties
        NowPlayingInsetOptions DesiredOptions { get; }
        IUiZoneChild InputChildLink { get; }
    }

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct NowPlayingInsetOptions
    {
        public Microsoft.MediaCenter.UI.Color clrBorder;
        public Microsoft.MediaCenter.UI.Color clrFocusBorder;
        public MetadataVisibility visMetadata;
        public bool fCustomPosition;
        public bool fInputDisabled;
        public PipVisualState visualState;
        public override int GetHashCode()
        {
            return (((((this.clrBorder.GetHashCode() ^ this.clrFocusBorder.GetHashCode()) ^ this.visMetadata.GetHashCode()) ^ this.fCustomPosition.GetHashCode()) ^ this.fInputDisabled.GetHashCode()) ^ this.visualState.GetHashCode());
        }

        public override bool Equals(object oCompare)
        {
            if ((oCompare != null) && (oCompare is NowPlayingInsetOptions))
            {
                return this.EqualsImpl((NowPlayingInsetOptions)oCompare);
            }
            return false;
        }

        public static bool operator ==(NowPlayingInsetOptions o1, NowPlayingInsetOptions o2)
        {
            return o1.EqualsImpl(o2);
        }

        public static bool operator !=(NowPlayingInsetOptions o1, NowPlayingInsetOptions o2)
        {
            return !o1.EqualsImpl(o2);
        }

        private bool EqualsImpl(NowPlayingInsetOptions other)
        {
            if ((((this.clrBorder == other.clrBorder) && (this.clrFocusBorder == other.clrFocusBorder)) && ((this.visMetadata == other.visMetadata) && (this.fCustomPosition == other.fCustomPosition))) && (this.fInputDisabled == other.fInputDisabled))
            {
                return (this.visualState == other.visualState);
            }
            return false;
        }
    }

    [MarkupVisible]
    public enum MetadataVisibility
    {
        Always = 1,
        Default = 2,
        Never = 0,
        OnFocus = 2
    }

    public enum PipVisualState
    {
        Active,
        Inactive
    }


    public interface ICustomNavigationJunction
    {
        // Methods
        bool NavigateIn(Direction dirSearch, RectangleF rcfStart, bool fIsDefault);
        bool NavigateOut(Direction dirSearch, RectangleF rcfStart);
    }

    public enum Direction
    {
        North,
        South,
        East,
        West,
        Previous,
        Next
    }
}
