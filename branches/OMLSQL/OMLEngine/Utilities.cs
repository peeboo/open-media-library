using System;
using System.IO;
using System.Diagnostics;
using System.Data;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ComponentModel;

namespace OMLEngine
{
    #region Global Enumerators
    /// <summary>
    /// Enumerator for Sex
    /// </summary>
    public enum Sex
    {
        Male,
        Female
    };

    /// <summary>
    /// Enumerator for SourceDatabases
    /// </summary>
    public enum SourceDatabase
    {
        OML,
        MyMovies,
        DVDProfiler,
        MovieCollectorz
    };   

    /// <summary>
    /// Enumerator for Video Fromats
    /// </summary>
    public enum VideoFormat
    {
        // DO NOT MODIFY ORDER, INSERT IN THE MIDDLE, OR REMOVE ENTRIES, JUST ADD TO THE END!
        // All items in this list MUST be uppercase

        ASF, // WMV style
        AVC, // AVC H264
        AVI, // DivX, Xvid, etc
        B5T, // BlindWrite image
        B6T, // BlindWrite image
        BIN, // using an image loader lib and load/play this as a DVD
        BLURAY, // detect which drive supports this and request the disc
        BWT, // BlindWrite image
        CCD, // CloneCD image
        CDI, // DiscJuggler Image
        CUE, // cue sheet
        DVD, // detect which drive supports this and request the disc
        DVRMS, // MPG
        H264, // AVC OR MP4
        HDDVD, // detect which drive supports this and request the disc
        IFO, // Online DVD
        IMG, // using an image loader lib and load/play this as a DVD
        ISO, // Standard ISO image
        ISZ, // Compressed ISO image
        MDF, // using an image loader lib and load/play this as a DVD
        MDS, // Media Descriptor file
        MKV, // Likely h264
        MOV, // Quicktime
        MPG,
        MPEG,
        MP4, // DivX, AVC, or H264
        NRG, // Nero image
        OFFLINEBLURAY, // detect which drive supports this and request the disc
        OFFLINEDVD, // detect which drive supports this and request the disc
        OFFLINEHDDVD, // detect which drive supports this and request the disc
        OGM, // Similar to MKV
        PDI, // Instant CD/DVD image
        TS, // MPEG2
        UIF,
        UNKNOWN,
        URL, // this is used for online content (such as streaming trailers)
        WMV,
        VOB, // MPEG2
        WVX, // wtf is this?
        ASX, // like WPL
        WPL, // playlist file?
        WTV, // new dvr format in vista (introduced in the tv pack 2008)
        M2TS, // mpeg2 transport stream (moved, since it got inserted in the middle, and all new types have to be inserted at the end)

        ALL, // meaning all format types - used for setting video format to external player
    };

    /// <summary>
    /// Enumerator for various TitleCollection errors
    /// </summary>
    public enum TITLE_COLLECTION_STATUS
    {
        TC_OK,
        TC_TITLE_ALREADY_EXISTS,
        TC_TITLE_NAME_FORMAT_RELEASE_ALREADY_EXISTS,
        TC_TITLE_DOES_NOT_EXIST
    };
    #endregion

    public static class Utilities
    {
        private static FileStream Log;

        private static Random random;
        /// <summary>
        /// Static list of methods that ALL plugins must define
        /// </summary>
        public static string[] RequiredMethods = new string[4] {
            "get_GetTitles",
            "get_TotalRowsAdded",
            "get_GetDescription",
            "get_GetAuthor"
        };

        /// <summary>
        /// Loads data from plugin classes and creates a List of Title objects
        /// </summary>
        /// <param name="importerClassType">Type of importer class</param>
        /// <returns>IList of Title objects</returns>
        public static IList<Title> ImportData(Type importerClassType)
        {
            List<Title> titles;
            if (importerClassType.IsClass)
            {
                try
                {
                    object obj = Activator.CreateInstance(importerClassType);

                    MethodInfo mi = importerClassType.GetMethod("GetTotalRowsAdded");
                    int totalTitles = (int)mi.Invoke(obj, null);
                    if (totalTitles > 0) {
                        mi = importerClassType.GetMethod("GetTitles");
                        titles = (List<Title>)mi.Invoke(obj, null);

                        if (titles != null)
                        {
                            foreach (Title title in titles)
                            {
                                Utilities.DebugLine("[Utilities] Would add title" + title.Name);
                            }
                        }
                        return titles;
                    }
                }
                catch (Exception e)
                {
                    Utilities.DebugLine("[Utilities] Importer Error: " + e.Message);
                }
            }
            return null;
        }

        /// <summary>
        /// Scans the plugins directory for all possible plugins
        /// and validates each one before returns a list of all assemblies
        /// that passed validation.
        /// </summary>
        /// <returns>A List of plugin names</returns>
        public static List<string> getPossiblePlugins()
        {
            List<string> plugins = new List<string>();
            if (FileSystemWalker.PluginsDirExists)
            {
                string[] files = Directory.GetFiles(FileSystemWalker.PluginsDirectory);
                foreach (string possible_file in files)
                {
                    if (!possible_file.Contains("OMLSDK"))
                        plugins.Add(possible_file);
                }
            }
            return plugins;
        }

        /*public static string FileSearchPattern()
        {
            return "*.asf,*.avc,*.avi,*.bin,*.cue,*.dvr-ms,*.h264,*.img,*.iso,*.mdf,*.mkv" +
                   "*.mov,*.mpg,*.mpeg,*.mp4,*.ogm,*.ts,*.wmv,*.vob,video_ts,*.wtv";
        }*/
         
        /// <summary>
        /// Loads all valid plugins into memory
        /// </summary>
        /// <returns>A List of the Type objects representing all valid plugins found</returns>
        public static List<Type> LoadAssemblies()
        {
            List<Type> validPlugins = new List<Type>();
            Assembly asm = null;
            List<string> possible_plugins = getPossiblePlugins();
            foreach (string posPlugin in possible_plugins)
            {
                asm = Assembly.LoadFile(posPlugin);
                Type[] types = asm.GetTypes();
                foreach (Type type in types)
                {
                    if (ValidatePlugin(type))
                    {
                        validPlugins.Add(type);
                    }
                }
            }
            return validPlugins;
        }

        /// <summary>
        /// Checks a given plugin Type and ensures that it contains
        /// the required methods for use.
        /// </summary>
        /// <param name="type">Type of the plugin to be validated</param>
        /// <returns>True on successful validation</returns>
        public static bool ValidatePlugin(Type type)
        {
            if (type.IsClass)
            {
                try
                {
                    object obj = Activator.CreateInstance(type);
                    MethodInfo[] methods = type.GetMethods();

                    foreach (string required_meth in RequiredMethods)
                    {
                        if (!ContainsMethod(methods, required_meth))
                            return false;
                    }
                }
                catch (Exception e)
                {
                    Utilities.DebugLine("[Utilities] Failed to validate Importer: " +
                                    type +
                                    " with error: " +
                                    e.Message);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Validates or creates required directories for the proper function
        /// of the Open Media Library application
        /// </summary>
        /// <returns>True on success</returns>
        public static bool RawSetup()
        {
            if (!FileSystemWalker.RootDirExists)
                FileSystemWalker.createRootDirectory();

            if (!FileSystemWalker.PluginsDirExists)
                FileSystemWalker.createPluginsDirectory();

            if (!FileSystemWalker.PublicRootDirExists)
                FileSystemWalker.createPublicRootDirectory();

            if (!FileSystemWalker.ImageDirExists)
                FileSystemWalker.createImageDirectory();

            if (!FileSystemWalker.LogDirExists)
                FileSystemWalker.createLogDirectory();

            if (!FileSystemWalker.TranscodeBufferDirExists)
                FileSystemWalker.createTranscodeBufferDirectory();

            if (!FileSystemWalker.TempPlayListDirExists)
                FileSystemWalker.createTempPlayListDirectory();

            if (!FileSystemWalker.MainBackDropDirExists)
                FileSystemWalker.createMainBackDropDirectory();
            
            if (!FileSystemWalker.FanArtDirectoryExists)
                FileSystemWalker.createFanArtDirectory();

            return true;
        }

        /// <summary>
        /// Given a list of available methods and the method to search for,
        /// scan through all the available methods looking for the requested
        /// method.
        /// </summary>
        /// <param name="methods">List of methods to scan through</param>
        /// <param name="required_method">name of method to find</param>
        /// <returns>True on success</returns>
        private static bool ContainsMethod(MethodInfo[] methods, string required_method)
        {
            foreach (MethodInfo mi in methods)
            {
                if (mi.Name == required_method)
                    return true;
            }
            return false;
        }

        public static int NewRandomNumber()
        {
            if (Utilities.random == null)
            {
                DebugLine("[Utilities] Initializing Random number Generator");
                Utilities.random = new Random(DateTime.Now.Millisecond);
            }

            int rand_num = random.Next();
            DebugLine("[Utilities] Random Number Generated: " + rand_num.ToString());
            return rand_num;
        }

        public static bool HasDaemonTools()
        {
            string daemontools_path = OMLEngine.Properties.Settings.Default.MountingToolPath;
            if (daemontools_path != null && daemontools_path.Length > 0)
                return true;

            return false;
        }

        public static DriveInfo DriveInfoForDrive(string VirtualDiscDrive)
        {
            if (VirtualDiscDrive == null || VirtualDiscDrive.Length < 1)
                return null;

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                string driveName = drive.Name.Substring(0, 1);
                if (driveName.ToUpper().Substring(0, 1).CompareTo(VirtualDiscDrive.ToUpper()) == 0)
                {
                    return drive;
                }
            }
            return null;
        }

        public static void UnmountVirtualDrive(int VirtualDiscDriveNumber)
        {
            string mount_util_path = OMLEngine.Properties.Settings.Default.MountingToolPath;

            Process cmd = new Process();
            cmd.StartInfo.FileName = "\"" + mount_util_path + "\"";
            cmd.StartInfo.Arguments = @"-unmount " + VirtualDiscDriveNumber.ToString();
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.UseShellExecute = false;

            cmd.Start();
            Thread.Sleep(10);
        }

        public static void DebugLine(string msg, params object[] paramArray)
        {
            try
            {
                if (Log == null)
                {
                    try
                    {
                        string file = Path.Combine(OMLEngine.FileSystemWalker.LogDirectory, string.Format("{0}-debug.txt", Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName)));
                        if (Directory.Exists(OMLEngine.FileSystemWalker.LogDirectory) == false)
                            Directory.CreateDirectory(OMLEngine.FileSystemWalker.LogDirectory);

                        bool tooLarge = ( File.Exists(file) && (new FileInfo(file)).Length > 1000000);

                        Log = new FileStream(file, File.Exists(file) && !tooLarge ? FileMode.Append : FileMode.Create);
                        Trace.Listeners.Add(new TextWriterTraceListener(Log, "debug.txt"));
                        Trace.AutoFlush = true;
                        Trace.WriteLine(new string('=', 80));
                        Trace.TraceInformation(DateTime.Now.ToString() + " OML Version: 0.21b ({0}, PID:{1})", File.GetLastWriteTime(typeof(Utilities).Assembly.Location), Process.GetCurrentProcess().Id);
                    }
                    catch
                    { }
                }
                string prefix = string.Format("{0} [{1}#{2}], ", DateTime.Now, Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.Name);
                Trace.TraceInformation(prefix + msg, paramArray);
            }
            catch
            {
            }
        }

        // Image resizing methods taken from a public post on forums.msdn.microsoft.com
        // Written by user: PeacError on Wed. Nov 1st, 2006
        // Link: http://forums.msdn.microsoft.com/en-US/csharpgeneral/thread/33d0acc4-bf4c-475b-9b43-0fa1093c1e19/
        public static Image ScaleImageByPercentage(Image img, double percent)
        {
            double fractionalPercentage = (percent / 100.0);
            int outputWidth = (int)(img.Width * fractionalPercentage);
            int outputHeight = (int)(img.Height * fractionalPercentage);

            return ScaleImage(img, outputWidth, outputHeight);
        }

        public static Image ScaleImage(Image img, int outputWidth, int outputHeight)
        {
            Bitmap outputImage = new Bitmap(outputWidth, outputHeight, img.PixelFormat);
            outputImage.SetResolution(img.HorizontalResolution, img.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(outputImage))
            {
                graphics.CompositingQuality = CompositingQuality.Default;
                graphics.InterpolationMode = InterpolationMode.High;
                graphics.SmoothingMode = SmoothingMode.Default;
                graphics.DrawImage(img,
                                   new Rectangle(0, 0, outputWidth, outputHeight),
                                   new Rectangle(0, 0, img.Width, img.Height),
                                   GraphicsUnit.Pixel);

                graphics.Dispose();
            }

            return outputImage;
        }

        public static Image ScaleImageByHeight(Image img, int height)
        {
            if (img != null)
            {
                double fractionalPercentage = ((double)height / (double)img.Height);
                int outputWidth = (int)(img.Width * fractionalPercentage);
                int outputHeight = height;

                return ScaleImage(img, outputWidth, outputHeight);
            }
            else
            {
                return img;
            }
        }

        public static Image ScaleImageByWidth(Image img, int width)
        {
            if (img != null)
            {
                double fractionalPercentage = ((double)width / (double)img.Width);
                int outputWidth = width;
                int outputHeight = (int)(img.Height * fractionalPercentage);

                return ScaleImage(img, outputWidth, outputHeight);
            }
            else
            {
                return null;
            }
        }

        public static Image ScaleImageDownTillFits(Image img, Size size)
        {
            Image ret = img;
            bool bFound = false;

            if (img != null && (img.Width > size.Width) || (img.Height > size.Height))
            {
                for (double percent = 100; percent > 0; percent--)
                {
                    double fractionalPercentage = (percent / 100.0);
                    int outputWidth = (int)(img.Width * fractionalPercentage);
                    int outputHeight = (int)(img.Height * fractionalPercentage);

                    if ((outputWidth < size.Width) && (outputHeight < size.Height))
                    {
                        bFound = true;
                        ret = ScaleImage(img, outputWidth, outputHeight);
                        break;
                    }
                }

                if (!bFound)
                {
                    ret = ScaleImage(img, size.Width, size.Height);
                }
            }
            return ret;
        }

        /*
        public static Size ResolutionOfVideoFile(string fileName)
        {
            Size size = new Size(0, 0);
            Utilities.DebugLine("Determining Resolution of: " + fileName);
            Microsoft.DirectX.AudioVideoPlayback.Video video;

            if (File.Exists(fileName))
            {
                video = Microsoft.DirectX.AudioVideoPlayback.Video.FromFile(fileName);
                if (video != null)
                {
                    Utilities.DebugLine("Resolution found: " + video.DefaultSize.Width + "x" + video.DefaultSize.Height);
                    size = video.DefaultSize;
                }
            }
            return size;
        }
        */

        [DllImport("shell32.dll", EntryPoint = "#680", CharSet = CharSet.Unicode)]
        public static extern bool IsUserAnAdmin();

        public static bool IsUACActive()
        {
            return !IsUserAnAdmin();
        }

        public static Image ReadImageFromFile(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    using (FileStream fs = new FileStream(fileName, FileMode.Open))
                    {
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, (int)fs.Length);
                        using (MemoryStream ms = new MemoryStream(buffer))
                        {
                            Bitmap bmp1 = new Bitmap(ms);
                            Bitmap bmp2 = new Bitmap(bmp1.Width, bmp1.Height, bmp1.PixelFormat);
                            Graphics g = Graphics.FromImage(bmp2);
                            GraphicsUnit pageUnit = new GraphicsUnit();
                            g.DrawImage(bmp1, bmp2.GetBounds(ref pageUnit));


                            return bmp2;
                        }
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        public delegate void WorkImpersonatedDelegate();

        public static bool DoWorkImpersonated(WorkImpersonatedDelegate StuffToDo)
        {
            // Call LogonUser to get a token for the user
            IntPtr _Token = IntPtr.Zero;
            string username = Properties.Settings.Default.ImpersonationUsername;
            string password = Properties.Settings.Default.ImpersonationPassword;
            bool loggedOn = LogonUser(
                username,
                System.Environment.UserDomainName,
                password,
                LOGON32_LOGON_NETWORK,
                LOGON32_PROVIDER_DEFAULT,
                ref _Token);
            if (!loggedOn)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            // Begin impersonating the user
            WindowsImpersonationContext impersonationContext = null;
            try
            {
                WindowsIdentity.Impersonate(_Token);
                StuffToDo();
            }
            finally
            {
                // Clean up
                CloseHandle(_Token);
                if (impersonationContext != null)
                    impersonationContext.Undo();
            }
            return false;
        }

        public enum SECURITY_IMPERSONATION_LEVEL : int
        {
            SecurityAnonymous = 0,
            SecurityIdentification = 1,
            SecurityImpersonation = 2,
            SecurityDelegation = 3
        }

        private const int LOGON32_LOGON_INTERACTIVE = 2,
                          LOGON32_LOGON_NETWORK = 3,
                          LOGON32_PROVIDER_WINNT50 = 3,
                          LOGON32_PROVIDER_DEFAULT = 0,
                          LOGON32_LOGON_BATCH = 4,
                          LOGON32_LOGON_SERVICE = 5;

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool LogonUser(string pszUsername, string pszDomain, string pszPassword,
        int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool ImpersonateLoggedOnUser(IntPtr phToken);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool RevertToSelf();

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool DuplicateToken(IntPtr ExistingTokenHandle,

        int SECURITY_IMPERSONATION_LEVEL, ref IntPtr DuplicateTokenHandle);

        //private bool ImpersonateUser(string UserName, string Password, string DomainName)
        //{
        //    try
        //    {
        //        WindowsIdentity _WinIdent;
        //        IntPtr _Token = IntPtr.Zero,
        //        _DuplicateToken = IntPtr.Zero;
        //        int l_token1;
        //        bool results = LogonUser(UserName, DomainName, Password, LOGON32_LOGON_NETWORK, LOGON32_PROVIDER_DEFAULT, ref _Token);
        //        if (results == false)
        //        {
        //            Win32Exception _ex = new Win32Exception(Marshal.GetLastWin32Error());
        //            Console.WriteLine(_ex.Message + "\r\n" + _ex.ErrorCode + "\r\n" + _ex.StackTrace);
        //            return false;
        //        }

        //        if (!DuplicateToken(_Token, (int)SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation, ref _DuplicateToken))
        //        {
        //            Win32Exception _ex = new Win32Exception(Marshal.GetLastWin32Error());
        //            Console.WriteLine(_ex.Message + "\r\n" + _ex.ErrorCode + "\r\n" + _ex.StackTrace);
        //            return false;
        //        }

        //        _WinIdent = new WindowsIdentity(_DuplicateToken);
        //        m_ImpersonationContext = _WinIdent.Impersonate();
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
    }
}
