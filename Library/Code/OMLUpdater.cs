/* This code originally taken from the Media Browser project *
 * at http://code.google.com/p/videobrowser/                 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Diagnostics;
using Microsoft.MediaCenter;
using Microsoft.MediaCenter.Hosting;
using Microsoft.MediaCenter.UI;
using System.Reflection;

namespace Library
{
    public class OMLUpdater
    {
        public OMLUpdater()
        {
        }

        public static System.Version CurrentVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version;
            }
        }

        public void checkUpdate(object stateInfo)
        {
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(new XmlTextReader(infoURL));

                XmlNode node;
                node = xDoc.SelectSingleNode("/Config/Release");

                newVersion = new System.Version(node.Attributes["version"].Value);
                remoteFile = node.Attributes["url"].Value;

                if (CurrentVersion < newVersion)
                {
                    if (OMLApplication.Current.MediaCenterEnvironment.Capabilities.ContainsKey("Console"))
                    {
                        DialogResult reply = OMLApplication.Current.MediaCenterEnvironment.Dialog("", "", DialogButtons.Yes | DialogButtons.No, 10, true);

                        if (reply == DialogResult.Yes)
                            DownloadUpdate();
                    }
                    else
                    {
                        // at the extender
                        OMLApplication.Current.MediaCenterEnvironment.Dialog("An update for OML is available, please update from your Media Center PC.", "Update Available", DialogButtons.Ok, 10, true);
                    }
                }
            }
            catch (Exception)
            {
                OMLApplication.DebugLine("Update failed");
            }
        }

        void DownloadUpdate()
        {
            int bytesDone = 0;

            localFile = Path.Combine(Path.GetTempFileName(), ".msi");

            Stream readStream = null;
            Stream writeStream = null;

            WebResponse res = null;

            try
            {
                WebRequest req = WebRequest.Create(remoteFile);
                if (req != null)
                {
                    res = req.GetResponse();
                    if (res != null)
                    {
                        readStream = res.GetResponseStream();
                        writeStream = File.Create(localFile);
                        byte[] buffer = new byte[1024];

                        int bytesRead = 0;

                        do
                        {
                            bytesRead = readStream.Read(buffer, 0, buffer.Length);
                            writeStream.Write(buffer, 0, bytesRead);
                            bytesDone += bytesRead;
                        } while (bytesRead > 0);
                    }
                }
            }
            catch (Exception)
            {
                bytesDone = 0;
            }
            finally
            {
                if (res != null)
                    res.Close();
                if (readStream != null)
                    readStream.Close();
                if (writeStream != null)
                    writeStream.Close();
            }

            if (bytesDone > 0)
                DownloadComplete();
            else
                OMLApplication.Current.MediaCenterEnvironment.Dialog(
                    "OML will continue with its current version.",
                    "Update download failed",
                    DialogButtons.Ok, 5, true);
                                                            
        }

        public void DownloadComplete()
        {
            OMLApplication.Current.MediaCenterEnvironment.Dialog(
                "OML will now exit to install the update, and will be automatically restated once complete.",
                "Ready to Update",
                DialogButtons.Ok, 5, false);

            string updateBatch = "msiexec.exe /qb /i \"" + localFile + "\"n";
            string ehshellPath = Path.Combine(Environment.ExpandEnvironmentVariables("%SystemRoot%"), @"ehome\eshell");
            updateBatch += ehshellPath + " /entrypoint:{ad208fce-2431-47d6-abed-1974a2a0555f}\\{7533724D-C7CB-4ac2-8AEE-1B0B91ADD393}";
            string filename = Path.Combine(Path.GetTempFileName(), ".bat");
            File.WriteAllText(filename, updateBatch);

            Process update = new Process();
            update.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            update.StartInfo.FileName = filename;
            update.Start();

            AddInHost context = AddInHost.Current;
            context.ApplicationContext.CloseApplication();
        }


        string remoteFile;
        string localFile;
        System.Version newVersion;

        const string infoURL = "http://open-media-library.googlecode.com/svn/trunk/Library/OMLInfo.xml";
    }
}
