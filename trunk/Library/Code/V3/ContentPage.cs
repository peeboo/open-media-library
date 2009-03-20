using System;
using System.Collections;
using Microsoft.MediaCenter.UI;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;
using System.IO;
using Library.Code.V3;

namespace Library.Code.V3
{
    /// <summary>
    /// This object contains the standard set of information displayed in a
    /// content page UI (e.g. Gallery).
    /// </summary>
    public class ContentPage : ModelItem
    {
        public ContentPage()
        {
        }

        /// <summary>
        /// The list of items in the page.
        /// This list should only contain objects of type ThumbnailCommand.
        /// </summary>
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


    /// <summary>
    /// This object contains the standard set of information displayed in a 
    /// filtered content page UI (e.g. Gallery).
    /// </summary>
    public class FilteredContentPage : ContentPage
    {
        /// <summary>
        /// A choice of available filters on the content.
        /// </summary>
        public virtual Choice Filters
        {
            get { return filters; }
            set
            {
                if (filters != value)
                {
                    // Unhook events on the old value
                    if (filters != null)
                        filters.ChosenChanged -= new EventHandler(OnActiveFilterChanged);

                    filters = value;

                    // Hook up events to the new value
                    if (filters != null)
                        filters.ChosenChanged += new EventHandler(OnActiveFilterChanged);

                    // Fire the "chagned" event
                    OnActiveFilterChanged(filters, EventArgs.Empty);
                    FirePropertyChanged("Filters");
                }
            }
        }

        /// <summary>
        /// Fired when the Chosen value within Filters has been modified
        /// or if the instance of Filters has been changed.
        /// </summary>
        public event EventHandler ActiveFilterChanged;

        /// <summary>
        /// Fire the event for the active filter changing.
        /// </summary>
        protected virtual void OnActiveFilterChanged(object sender, EventArgs args)
        {
            if (ActiveFilterChanged != null)
                ActiveFilterChanged(this, EventArgs.Empty);
        }


        private Choice filters;
    }


    public class EnormoList : VirtualList
    {
        //
        // Enormous list construction.
        //

        public EnormoList()
        {
            VisualReleaseBehavior = ReleaseBehavior.Dispose;
            EnableSlowDataRequests = true;
        }

        protected override void OnRequestItem(int index, ItemRequestCallback callback)
        {
            ThumbnailData t = new ThumbnailData(this, index.ToString(CultureInfo.CurrentUICulture));
            callback(this, index, t);
        }

        protected override void OnRequestSlowData(int index)
        {
            if (_pendingPictureAcquires.ContainsKey(index))
            {
                return;
            }

            SlowDataResult result = new SlowDataResult();
            result.Index = index;
            _pendingPictureAcquires[index] = result;
            Microsoft.MediaCenter.UI.Application.DeferredInvokeOnWorkerThread(AcquireSlowData, StoreSlowData, result);
        }

        private static void AcquireSlowData(object args)
        {
            ThreadPriority priority = Thread.CurrentThread.Priority;
            try
            {
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;

                SlowDataResult result = (SlowDataResult)args;

                result.PicturePath = @"\ProgramData\My Movies\Covers\tmb5cb25864-26c1-4b24-86b4-93d724e815fa.jpg";

            }
            finally
            {
                Thread.CurrentThread.Priority = priority;
            }
        }

        private void StoreSlowData(object args)
        {
            SlowDataResult result = (SlowDataResult)args;
            _pendingPictureAcquires.Remove(result.Index);

            if (IsDisposed || !IsItemAvailable(result.Index))
            {
                if (result.PicturePath != null)
                {
                    ////File.Delete(result.PicturePath);
                }

                return;
            }

            ThumbnailData t = (ThumbnailData)this[result.Index];
            t.SetPicture(result.PicturePath);
        }

        private class SlowDataResult
        {
            public int Index;
            public string PicturePath;
        }

        private Dictionary<int, object> _pendingPictureAcquires = new Dictionary<int, object>();
        private const string TempPictureFileExtension = "acdv";
    }
    public class ThumbnailData : GalleryItem
    {
        public ThumbnailData(IModelItemOwner owner, string caption)
            :
            base(owner)
        {
            _caption = caption;
        }

        public string Caption
        {
            get
            {
                return _caption;
            }
        }

        public Image Picture
        {
            get
            {
                return _picture;
            }
        }

        public void SetPicture(string picturePath)
        {
            _picturePath = picturePath;

            _picture = new Image("file://" + picturePath);

            FirePropertyChanged("Picture");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (_picture != null)
                {
                    _picture.Dispose();
                    _picture = null;
                }

                if (_picturePath != null)
                {
                    ////File.Delete(_picturePath);
                    _picturePath = null;
                }
            }
        }


        private string _caption;

        private string _picturePath;
        private Image _picture;
    }
}
