using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Reflection;

namespace FileDownloader {
    public class DownloadEngine {
        public delegate void DownloadEventsHandler(string status);
        public delegate void CurrentBytesEventHandler(int bytes);

        public event DownloadEventsHandler Log;
        public event CurrentBytesEventHandler Bytes;

        protected void FireEvent(int bytes) {
            if (Bytes != null)
                Bytes(bytes);
        }

        protected void FireEvent(string msg) {
            if (Log != null)
                Log(msg);
        }

        public string UserAgent {
            get;
            set;
        }

        public string Url {
            get;
            set;
        }

        public string DownloadedFile {
            get;
            private set;
        }

        public DownloadEngine(string url) {
            Url = url;
        }

        public long TotalBytes {
            get;
            private set;
        }

        public bool Download() {
            int bytesDone = 0;

            Stream readStream = null;
            Stream writeStream = null;

            WebResponse res = null;

            try {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url);

                if (req != null) {
                    if (!string.IsNullOrEmpty(UserAgent))
                        req.UserAgent = UserAgent;

                    res = req.GetResponse();
                    if (res != null) {
                        TotalBytes = res.ContentLength;
                        readStream = res.GetResponseStream();
                        DownloadedFile = _localfile();
                        writeStream = File.Create(DownloadedFile);
                        byte[] buffer = new byte[1024];

                        int bytesRead = 0;

                        do {
                            bytesRead = readStream.Read(buffer, 0, buffer.Length);
                            writeStream.Write(buffer, 0, bytesRead);
                            bytesDone += bytesRead;
                            FireEvent(bytesDone);
                        } while (bytesRead > 0);
                    }
                }
            } catch (Exception ex) {
                bytesDone = 0;
                throw ex;
            } finally {
                if (res != null)
                    res = null;
                if (readStream != null)
                    readStream.Close();
                if (writeStream != null)
                    writeStream.Close();
            }

            if (bytesDone > 0)
                return true;

            return false;
        }

        public bool Download(bool AllowUnsafeHeaders) {
            if (SetAllowUnsafeHeaderParsing20())
                return Download();

            return false;
        }

        private string _localfile() {
            return Path.GetTempFileName();
        }

        private static bool SetAllowUnsafeHeaderParsing20() {
            //Get the assembly that contains the internal class
            Assembly aNetAssembly = Assembly.GetAssembly(typeof(System.Net.Configuration.SettingsSection));
            if (aNetAssembly != null) {
                //Use the assembly in order to get the internal type for the internal class
                Type aSettingsType = aNetAssembly.GetType("System.Net.Configuration.SettingsSectionInternal");
                if (aSettingsType != null) {
                    //Use the internal static property to get an instance of the internal settings class.
                    //If the static instance isn't created allready the property will create it for us.
                    object anInstance = aSettingsType.InvokeMember("Section",
                      BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.NonPublic, null, null, new object[] { });

                    if (anInstance != null) {
                        //Locate the private bool field that tells the framework is unsafe header parsing should be allowed or not
                        FieldInfo aUseUnsafeHeaderParsing = aSettingsType.GetField("useUnsafeHeaderParsing", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (aUseUnsafeHeaderParsing != null) {
                            aUseUnsafeHeaderParsing.SetValue(anInstance, true);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
