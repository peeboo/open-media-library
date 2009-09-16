using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using OMLEngine;

namespace OMLDatabaseEditor
{
    public partial class DownloadingBackDropsForm : DevExpress.XtraEditors.XtraForm
    {
        List<string> urls;
        Title title;

        int processed;
        int totalurls;

        public DownloadingBackDropsForm(Title _title, List<string> _urls)
        {
            urls = _urls;
            title = _title;

            InitializeComponent();

            processed = 0;
            totalurls = urls.Count;
            SetProgressMax(totalurls);
            SetProgress(0);

            // Trigger downloading images
            for (int i = 0; i < 5; i++)
            {
                if (urls.Count > 0)
                {
                    LoadImage();
                }
            }
        }

        private void SetProgressMax(int p)
        {
            pbProgress.Properties.Minimum = 0;
            pbProgress.Properties.Maximum = p;
            pbProgress.Position = 0;
        }

        private void SetProgress(int p)
        {
            pbProgress.Position = p;
            pbProgress.Refresh();
            lcStatus.Text = processed.ToString() + " of " + totalurls.ToString();
        }

        private void LoadImage()
        {
            if (urls.Count > 0)
            {
                Uri src = new Uri(urls[0]);

                urls.RemoveAt(0);

                string filename = Path.Combine(FileSystemWalker.ImageDownloadDirectory, Path.GetFileName(src.LocalPath));

                WebClient web = new WebClient();
                web.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadingBackDropsFormFileDownloadedEvent);
                web.DownloadFileAsync(src, filename, filename);
            }
            else
            {
                if (processed >= totalurls) this.Close();
            }
        }

        private void DownloadingBackDropsFormFileDownloadedEvent(object sender, AsyncCompletedEventArgs c)
        {
            string img = (string)c.UserState;
            
            if (File.Exists(img))
            {
                title.AddFanArtImage(img);

                processed++;

                SetProgress(processed);
            }
            LoadImage();
        }
    }
}
