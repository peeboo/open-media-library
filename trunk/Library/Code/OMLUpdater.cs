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
using OMLEngine.Settings;
using FileDownloader;
using System.Threading;

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
                if (OMLSettings.EnableAutomaticUpdatesDailyBuilds)
                    node = xDoc.SelectSingleNode("/Config/Beta");
                else
                    node = xDoc.SelectSingleNode("/Config/Release");

                newVersion = new System.Version(node.Attributes["version"].Value);
                remoteFile = node.Attributes["url"].Value;

                if (CurrentVersion < newVersion) {
                    if (OMLApplication.Current.MediaCenterEnvironment.Capabilities.ContainsKey("Console")) {
                        DialogResult reply = OMLApplication.Current.MediaCenterEnvironment.Dialog("A new version of OML is available, would you like to upgrade now?", "Update Available", DialogButtons.Yes | DialogButtons.No, 10, true);

                        if (reply == DialogResult.Yes)
                            DownloadUpdate();
                    } else {
                        // at the extender
                        OMLApplication.Current.MediaCenterEnvironment.Dialog("An update for OML is available, please update from your Media Center PC.", "Update Available", DialogButtons.Ok, 10, true);
                    }
                } else {
                    // ok the master version is the same, lets check some of the plugins and extra files
                    XmlNode downloaderNode = xDoc.SelectSingleNode("/Config/FileDownloader");
                    ProcessSingleFileUpdate(
                        new System.Version(downloaderNode.Attributes["version"].Value),
                        Convert.ToBoolean(downloaderNode.Attributes["GAC"].Value),
                        downloaderNode.Attributes["type"].Value,
                        downloaderNode.Attributes["url"].Value,
                        @"C:\Program Files\OpenMediaLibrary\FileDownloader.exe");

                    XmlNode maximizerNode = xDoc.SelectSingleNode("/Config/Maximizer");
                    ProcessSingleFileUpdate(
                        new System.Version(maximizerNode.Attributes["version"].Value),
                        Convert.ToBoolean(maximizerNode.Attributes["GAC"].Value),
                        maximizerNode.Attributes["type"].Value,
                        maximizerNode.Attributes["url"].Value,
                        @"C:\Program Files\OpenMediaLibrary\Maximizer.exe");

                    XmlNode mencoderNode = xDoc.SelectSingleNode("/Config/MEncoder");
                    ProcessSingleFileUpdate(
                        new System.Version(mencoderNode.Attributes["version"].Value),
                        Convert.ToBoolean(mencoderNode.Attributes["GAC"].Value),
                        mencoderNode.Attributes["type"].Value,
                        mencoderNode.Attributes["url"].Value,
                        @"C:\Program Files\OpenMediaLibrary\Mencoder.exe");
                }
            }
            catch (Exception)
            {
                OMLApplication.DebugLine("Update failed");
            }
        }

        private void ProcessSingleFileUpdate(System.Version newVersion, bool updateGAC, string type, string url, string assemblyFile) {
            System.Version currentVersion = VersionOfAssembly(assemblyFile);
            if (currentVersion != null && (currentVersion < newVersion)) {
                DownloadEngine downloader = new DownloadEngine(url);
                if (downloader.Download()) {
                    string newfile = downloader.DownloadedFile;
                    if (updateGAC) {
                        RemoveOldGACEntry(assemblyFile);
                        downloader = null;
                    }

                    switch (type) {
                        case "Full":
                            break;
                        case "Replace":
                            File.Move(assemblyFile, assemblyFile + currentVersion);
                            File.Move(downloader.DownloadedFile, assemblyFile);
                            break;
                        case "Restart":
                            File.Move(assemblyFile, assemblyFile + currentVersion);
                            File.Move(downloader.DownloadedFile, assemblyFile);
                            break;
                        default:
                            break;
                    }
                    if (updateGAC)
                        AddNewGACEntry(assemblyFile);
                }
            }
        }

        private void RemoveOldGACEntry(string assemblyFile) {
            Process removeGAC = new Process();
            removeGAC.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            removeGAC.StartInfo.FileName = @"gacutil.exe";
            removeGAC.StartInfo.Arguments = string.Format("/u {0}", assemblyFile);
            removeGAC.Start();
            Thread.Sleep(new TimeSpan(0, 0, 2)); // sleep for 2 seconds
        }

        private void AddNewGACEntry(string assemblyFile) {
            Process startGAC = new Process();
            startGAC.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startGAC.StartInfo.FileName = @"gacutil.exe";
            startGAC.StartInfo.Arguments = string.Format("/u {0}", assemblyFile);
            startGAC.Start();
            Thread.Sleep(new TimeSpan(0, 0, 2)); // sleep for 2 seconds
        }

        private System.Version VersionOfAssembly(string path) {
            if (File.Exists(path)) {
                FileVersionInfo info = FileVersionInfo.GetVersionInfo(path);
                return new System.Version(info.ProductVersion);
            }
            return null;
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
