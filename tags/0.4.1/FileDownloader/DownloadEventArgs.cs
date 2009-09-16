using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileDownloader {
    public class DownloadEventArgs : EventArgs {
        public event DownloadEventsHandler DownloadEvent;
        protected string status;

        private DownloadEventArgs() { }

        public DownloadEventArgs(string status) {
            this.status = status;
        }

        protected virtual void OnDownloadEvent(DownloadEventArgs e) {
            if (e != null)
                DownloadEvent(this, e);
        }
    }

    public delegate void DownloadEventsHandler(object sender, DownloadEventArgs e);
}
