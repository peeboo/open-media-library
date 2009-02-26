using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.MediaCenter.UI;
using System.Collections;
using System.Threading;

namespace Library.Code.V3
{
    [MarkupVisible]
    public class ThumbnailCommand : Command, IThumbnailCommand, ICommand, IModelItem, IPropertyObject, IModelItemOwner
    {
        // Fields
        //private static readonly DataCookie s_propImage = DataCookie.ReserveSlot();

        // Methods
        public ThumbnailCommand()
            : this(null)
        {
        }

        public ThumbnailCommand(IModelItemOwner owner)
            : this(owner, null, null)
        {
        }

        public ThumbnailCommand(IModelItemOwner owner, EventHandler ehInvoked)
            : this(owner, null, ehInvoked)
        {
        }

        public ThumbnailCommand(IModelItemOwner owner, string stDescription, EventHandler ehInvoked)
            : base(owner, stDescription, ehInvoked)
        {
        }

        public ThumbnailCommand(IModelItemOwner owner, string stDescription, EventHandler ehInvoked, Image defaultImage, Image focusImage, Image dormantImage)
            : base(owner, stDescription, ehInvoked)
        {
            this.DefaultImage = defaultImage;
            this.DormantImage = dormantImage;
            this.FocusImage = focusImage;
        }

        //public ThumbnailCommand(IModelItemOwner owner, string stDescription, EventHandler ehInvoked, ImageSet imgset)
        //    : base(owner, stDescription, ehInvoked)
        //{
        //    this.Image = imgset;
        //}

        //public ThumbnailCommand(IModelItemOwner owner, string stDescription, EventHandler ehInvoked, Image img)
        //    : this(owner, stDescription, ehInvoked, ImageSet.FromImage(img))
        //{
        //}

        // Properties
        private Image defaultImage = null;
        public Image DefaultImage
        {
            get
            {
                return this.defaultImage;
            }
            set
            {
                if (this.defaultImage != value)
                {
                    this.defaultImage = value;
                    base.FirePropertyChanged("DefaultImage");
                }
            }
        }

        private Image fanArt = null;
        public Image FanArt
        {
            get
            {
                return this.fanArt;
            }
            set
            {
                if (this.fanArt != value)
                {
                    this.fanArt = value;
                    base.FirePropertyChanged("FanArt");
                }
            }
        }

        //for slowload
        private string imagePath;

        public void SlowLoadImage(string path)
        {
            this.imagePath = path;
            Microsoft.MediaCenter.UI.Application.DeferredInvokeOnWorkerThread
                                (
                // Delegate to be invoked on background thread
                                 startLoadSlowImage,

                                 // Delegate to be invoked on app thread
                                 endLoadSlowImage,

                                 // Parameter to be passed to both delegates, we don't need it
                                 null
                                 );
        }

        private void startLoadSlowImage(object obj)
        {
            //slow this thread down
            ThreadPriority priority = Thread.CurrentThread.Priority;
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            try
            {
                //TODO: implement in OML
                //Image slowImage = ImageHelper.LoadImage(imagePath);
                Image slowImage = null;
                if (slowImage != null)
                    Microsoft.MediaCenter.UI.Application.DeferredInvoke(notifySlowImage, slowImage);
            }
            finally
            {
                //
                // Reset our thread's priority back to its previous value
                //
                Thread.CurrentThread.Priority = priority;
            }
        }

        private void notifySlowImage(object obj)
        {
            if (!IsDisposed)
            {
                this.DefaultImage = (Image)obj;
            }
        }


        private void endLoadSlowImage(object obj)
        {
            //nothing yet
        }

        private Image dormantImage = null;
        public Image DormantImage
        {
            get
            {
                return this.dormantImage;
            }
            set
            {
                if (this.dormantImage != value)
                {
                    this.dormantImage = value;
                    base.FirePropertyChanged("DormantImage");
                }
            }
        }

        private Image focusImage = null;
        public Image FocusImage
        {
            get
            {
                return this.focusImage;
            }
            set
            {
                if (this.focusImage != value)
                {
                    this.focusImage = value;
                    base.FirePropertyChanged("FocusImage");
                }
            }
        }
    }
}
