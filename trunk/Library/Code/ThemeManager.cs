using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Globalization;
using Library.Code.V3;
using System.Security;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using System.ComponentModel;
using System.Xml.XPath;
using System.Security.Cryptography;

namespace Library
{
    class ThemeManager
    {
        private static XmlDocument XmlDocumentFromByteArray(byte[] docArray)
        {
            string xmlString = System.Text.Encoding.Default.GetString(docArray);
            XmlDocument doc = new XmlDocument();
            doc.Load(new StringReader(xmlString));
            return doc;
        }

        private static void updateMCMLResource(string eHResourceName, byte[] libResource, string saveFilePath)
        {
            try
            {
                DllResource eHRes = (DllResource)DllResources.Instance.GetResource(eHResourceName, null, null);
                eHRes.Open();
                XmlDocument eHDoc = XmlDocumentFromByteArray(eHRes.ByteArray);
                XmlDocument libraryDoc = XmlDocumentFromByteArray(libResource);

                foreach (XmlNode node in libraryDoc.SelectNodes("//*[@Name]"))
                {
                    XmlNode eHNode = eHDoc.SelectSingleNode(string.Format("//*[@Name='{0}']", node.Attributes["Name"].Value));
                    if (eHNode != null)
                    {
                        foreach (XmlAttribute attrib in node.Attributes)
                        {
                            attrib.Value = eHNode.Attributes[attrib.Name].Value;
                        }
                    }
                }
                File.WriteAllText(saveFilePath, libraryDoc.OuterXml);
            }
            catch (Exception ex)
            {
                //TODO:log it
                //we write our known resource here
                File.WriteAllBytes(saveFilePath, libResource);
                throw (ex);
            }
        }

        static public void CreateThemeFiles()
        {
            string basePath = OMLEngine.FileSystemWalker.PublicRootDirectory;
            string themePath = Path.Combine(basePath, "Themes");   

            if (!Directory.Exists(basePath))
            {
                try
                {
                    Directory.CreateDirectory(basePath);
                }
                catch
                {
                    //TODO: log it
                }
            }
            if (!Directory.Exists(themePath))
            {
                try
                {
                    Directory.CreateDirectory(themePath);
                }
                catch
                {
                    //TODO: log it
                }
            }

            try
            {
                //update our mcml files
                //we store a hash of Microsoft.MediaCenter.Shell.dll and if it doesn't match we regen
                //omlsettings would not be applicable because this is machine specific
                if (!File.Exists(Path.Combine(themePath, "Fonts.mcml")) || !File.Exists(Path.Combine(themePath, "FontNames.mcml")) || !File.Exists(Path.Combine(themePath, "Colors.mcml")) || string.IsNullOrEmpty(Properties.Settings.Default.MicrosoftMediaCenterShellHash) || ComputeFileHash(Path.Combine(OMLEngine.FileSystemWalker.eHomeDirectory, "Microsoft.MediaCenter.Shell.dll")) != Properties.Settings.Default.MicrosoftMediaCenterShellHash)
                {
                    updateMCMLResource("Microsoft.MediaCenter.Shell!Colors.mcml", Resources.V3_Controls_Common_Colors, Path.Combine(themePath, "Colors.mcml"));
                    updateMCMLResource("Microsoft.MediaCenter.Shell!Fonts.mcml", Resources.V3_Controls_Common_Fonts, Path.Combine(themePath, "Fonts.mcml"));
                    updateMCMLResource("Microsoft.MediaCenter.Shell!FontNames.mcml", Resources.V3_Controls_Common_FontNames, Path.Combine(themePath, "FontNames.mcml"));
                    Properties.Settings.Default.MicrosoftMediaCenterShellHash = ComputeFileHash(Path.Combine(OMLEngine.FileSystemWalker.eHomeDirectory, "Microsoft.MediaCenter.Shell.dll"));
                    Properties.Settings.Default.Save();
                }
                
            }
            catch (Exception ex)
            {
                //TODO: log it
            }

        }

        private static string ComputeFileHash(string file)
        {
            //SHA1Managed managed = new SHA1Managed(); 
            MD5CryptoServiceProvider unmanaged = new MD5CryptoServiceProvider(); 
            //managed.ComputeHash(buffer, 0, buffer.Length);
            //return System.Text.Encoding.Default.GetString(unmanaged.ComputeHash(File.OpenRead(Path.Combine(OMLEngine.FileSystemWalker.eHomeDirectory,""))));
            return Convert.ToBase64String(unmanaged.ComputeHash(File.OpenRead(file)));
            //foreach (byte b in bs)
            //{
            //    s.Append(b.ToString("x2").ToLower());
            //}
            //string password = s.ToString();
            //return password;

        }

        //private static byte[] ComputeFileHash(string fileName)
        //{
        //    using (var stream = File.OpenRead(fileName)) return System.Security.Cryptography.MD5.Create().ComputeHash(stream);
        //}

    }

    //internal class ResXResources : Library.Code.V3.IResourceProvider
    //{
    //    // Fields
    //    private HybridDictionary m_dictResXManagers = new HybridDictionary(true);
    //    private static ResXResources s_instance = new ResXResources();

    //    // Methods
    //    private ResXResources()
    //    {
    //    }

    //    public Library.Code.V3.Resource GetResource(string stResource, string stUri, IDictionary dictUriData)
    //    {
    //        string str;
    //        string str2;
    //        string str3;
    //        string str4;
    //        Library.Code.V3.Resource resource = null;
    //        ParseResource(stResource, out str, out str2, out str3, out str4);
    //        if (str != null)
    //        {
    //            ResourceManager manager = this.GetResourceXManager(str, str2, str3);
    //            if ((manager != null) && ResourceExists(manager, str4))
    //            {
    //                resource = new ResXResource(manager, str, str4);
    //            }
    //        }
    //        return resource;
    //    }

    //    private ResourceManager GetResourceXManager(string stCacheKey, string stAssembly, string stBase)
    //    {
    //        ResourceManager manager = (ResourceManager)this.m_dictResXManagers[stCacheKey];
    //        if (manager == null)
    //        {
    //            AssemblyName name = null;
    //            try
    //            {
    //                name = new AssemblyName(stAssembly);
    //            }
    //            catch (COMException)
    //            {
    //                return null;
    //            }
    //            //Assembly assembly = MarkupSystem.FindAssembly(name);
    //            Assembly assembly = FindAssembly(name);
    //            if (assembly == null)
    //            {
    //                return null;
    //            }
    //            manager = new ResourceManager(stBase, assembly);
    //            this.m_dictResXManagers[stCacheKey] = manager;
    //        }
    //        return manager;
    //    }

    //    internal static Assembly FindAssembly(AssemblyName name)
    //    {
    //        string s_stAssemblyRedirect = null;
    //        Assembly wellKnownAssembly = null;
    //        string stWeakName = name.Name;
    //        //wellKnownAssembly = GetWellKnownAssembly(stWeakName);
    //        if (wellKnownAssembly == null)
    //        {
    //            //if (MarkupVisibility.IsCodelessLockdown)
    //            //{
    //            //    return null;
    //            //}
    //            if (Library.Code.V3.InvariantString.EqualsI(name.Name, "ehres"))
    //            {
    //                return null;
    //            }
    //            if (s_stAssemblyRedirect != null)
    //            {
    //                string path = Path.Combine(s_stAssemblyRedirect, stWeakName + ".dll");
    //                if (File.Exists(path))
    //                {
    //                    try
    //                    {
    //                        wellKnownAssembly = Assembly.LoadFrom(path);
    //                        if (wellKnownAssembly != null)
    //                        {
    //                            return wellKnownAssembly;
    //                        }
    //                    }
    //                    catch (FileLoadException)
    //                    {
    //                    }
    //                    catch (BadImageFormatException)
    //                    {
    //                    }
    //                }
    //            }
    //            try
    //            {
    //                wellKnownAssembly = Assembly.Load(name);
    //            }
    //            catch (FileLoadException)
    //            {
    //            }
    //            catch (BadImageFormatException)
    //            {
    //            }
    //            catch (FileNotFoundException)
    //            {
    //            }
    //        }
    //        return wellKnownAssembly;
    //    }



    //    internal static void ParseResource(string stResource, out string stHost, out string stAssembly, out string stBase, out string stIdentifier)
    //    {
    //        stHost = null;
    //        stAssembly = null;
    //        stBase = null;
    //        stIdentifier = null;
    //        string[] strArray = stResource.Split(new char[] { '/' });
    //        if (strArray.Length == 3)
    //        {
    //            stAssembly = strArray[0];
    //            stBase = strArray[1];
    //            stIdentifier = strArray[2];
    //            stHost = stAssembly + "/" + stBase;
    //        }
    //    }

    //    public static bool ResourceExists(ResourceManager manager, string stIdentifier)
    //    {
    //        bool flag = true;
    //        object obj2 = null;
    //        try
    //        {
    //            obj2 = manager.GetObject(stIdentifier, null);
    //        }
    //        catch (MissingManifestResourceException)
    //        {
    //            flag = false;
    //        }
    //        finally
    //        {
    //            IDisposable disposable = obj2 as IDisposable;
    //            if (disposable != null)
    //            {
    //                disposable.Dispose();
    //            }
    //        }
    //        return flag;
    //    }

    //    // Properties
    //    public static ResXResources Instance
    //    {
    //        get
    //        {
    //            return s_instance;
    //        }
    //    }
    //}

    //internal class ResXResource : Library.Code.V3.Resource
    //{
    //    // Fields
    //    private Exception m_exception;
    //    private ResourceManager m_manager;
    //    private Library.Code.V3.IntPtrFromManaged m_pDataUnmanagedView;
    //    private string m_stHost;
    //    private string m_stIdentifier;
    //    private Stream m_streamTransient;

    //    // Methods
    //    internal ResXResource(ResourceManager manager, string stHost, string stIdentifier)
    //    {
    //        this.m_manager = manager;
    //        this.m_stHost = stHost;
    //        this.m_stIdentifier = stIdentifier;
    //    }

    //    public override void Close()
    //    {
    //        if (this.m_pDataUnmanagedView != null)
    //        {
    //            this.m_pDataUnmanagedView.Dispose();
    //            this.m_pDataUnmanagedView = null;
    //        }
    //        if (this.m_streamTransient != null)
    //        {
    //            this.m_streamTransient.Close();
    //            this.m_streamTransient = null;
    //        }
    //    }

    //    public object GetRaw()
    //    {
    //        object obj2 = null;
    //        try
    //        {
    //            obj2 = this.m_manager.GetObject(this.m_stIdentifier, null);
    //        }
    //        catch (MissingManifestResourceException)
    //        {
    //        }
    //        return obj2;
    //    }

    //    public override Library.Code.V3.ResourceStatus Open()
    //    {
    //        try
    //        {
    //            object obj2 = this.m_manager.GetObject(this.m_stIdentifier, null);
    //            this.m_streamTransient = obj2 as Stream;
    //            if (this.m_streamTransient == null)
    //            {
    //                if (obj2 is byte[])
    //                {
    //                    this.m_streamTransient = new MemoryStream((byte[])obj2);
    //                }
    //                else if (obj2 is Image)
    //                {
    //                    this.m_streamTransient = new MemoryStream();
    //                    Image image = (Image)obj2;
    //                    image.Save(this.m_streamTransient, image.RawFormat);
    //                }
    //            }
    //        }
    //        catch (MissingManifestResourceException exception)
    //        {
    //            //eDebug.DumpExceptionToLog("ResXResource.Open", eDebug.ExceptionType.Recovered, exception);
    //            this.m_exception = exception;
    //        }
    //        if (this.m_streamTransient == null)
    //        {
    //            return Library.Code.V3.ResourceStatus.Error;
    //        }
    //        return Library.Code.V3.ResourceStatus.Available;
    //    }

    //    public override string ToString()
    //    {
    //        return (this.m_stHost.ToLower(CultureInfo.InvariantCulture) + "/" + this.m_stIdentifier.ToLower(CultureInfo.InvariantCulture));
    //    }

    //    // Properties
    //    public override IntPtr Buffer
    //    {
    //        get
    //        {
    //            if (this.m_streamTransient == null)
    //            {
    //                return IntPtr.Zero;
    //            }
    //            if (this.m_pDataUnmanagedView == null)
    //            {
    //                this.m_pDataUnmanagedView = Library.Code.V3.Resource.IntPtrFromByteArray(this.ByteArray);
    //            }
    //            return this.m_pDataUnmanagedView.Buffer;
    //        }
    //    }

    //    public override byte[] ByteArray
    //    {
    //        get
    //        {
    //            byte[] buffer = null;
    //            if (this.m_streamTransient != null)
    //            {
    //                buffer = Library.Code.V3.Resource.ByteArrayFromStream(this.m_streamTransient);
    //            }
    //            return buffer;
    //        }
    //    }

    //    public override string Host
    //    {
    //        get
    //        {
    //            return this.m_stHost;
    //        }
    //    }

    //    public override string Identifier
    //    {
    //        get
    //        {
    //            return this.m_stIdentifier;
    //        }
    //    }

    //    public override int Length
    //    {
    //        get
    //        {
    //            if (this.m_streamTransient != null)
    //            {
    //                return (int)this.m_streamTransient.Length;
    //            }
    //            return 0;
    //        }
    //    }

    //    public override Stream Stream
    //    {
    //        get
    //        {
    //            return this.m_streamTransient;
    //        }
    //    }

    //    public override Exception UnderlyingErrorFromOpen
    //    {
    //        get
    //        {
    //            return this.m_exception;
    //        }
    //    }
    //}

    internal sealed class ResourceManager
    {
        // Fields
        private HybridDictionary m_dictSources = new HybridDictionary();
        internal const string ProtocolSeparator = "://";
        private static ResourceManager s_instance = new ResourceManager();

        // Methods
        private ResourceManager()
        {
        }

        public Library.Code.V3.Resource GetResource(string stUri)
        {
            return this.GetResource(stUri, null);
        }

        public Library.Code.V3.Resource GetResource(string stUri, IDictionary dictUriData)
        {
            Library.Code.V3.Resource resource = null;
            string str;
            string str2;
            ParseUri(stUri, out str, out str2);
            if (str == null)
            {
                return null;
            }
            Library.Code.V3.IResourceProvider source = (Library.Code.V3.IResourceProvider)this.m_dictSources[str];
            //if (!MarkupVisibility.IsPermittedResourceProvider(source))
            //{
            //    source = null;
            //}
            if (source != null)
            {
                resource = source.GetResource(str2, stUri, dictUriData);
            }
            return resource;
        }

        public static void ParseUri(string stUri, out string stScheme, out string stResource)
        {
            int index = stUri.IndexOf("://");
            if (index > 0)
            {
                stScheme = stUri.Substring(0, index);
                stResource = stUri.Substring(index + "://".Length);
            }
            else
            {
                stScheme = null;
                stResource = stUri;
            }
        }

        public void RegisterSource(string stScheme, Library.Code.V3.IResourceProvider source)
        {
            this.m_dictSources[stScheme] = source;
        }

        public void UnregisterSource(string stScheme)
        {
            this.m_dictSources.Remove(stScheme);
        }

        // Properties
        public static ResourceManager Instance
        {
            get
            {
                return s_instance;
            }
        }
    }

    internal class DllResources : IResourceProvider
    {
        // Fields
        private HybridDictionary m_dictDlls = new HybridDictionary(true);
        private static DllResources s_instance = new DllResources();

        // Methods
        private DllResources()
        {
        }

        public Win32Api.HINSTANCE GetHInstance(string stDllName)
        {
            string str;
            return this.GetHInstance(stDllName, out str);
        }

        public Win32Api.HINSTANCE GetHInstance(string stDllName, out string stDllFullName)
        {
            Win32Api.HINSTANCE hInstance;
            stDllFullName = null;
            if (this.m_dictDlls.Contains(stDllName))
            {
                DllEntry entry = (DllEntry)this.m_dictDlls[stDllName];
                hInstance = entry.HInstance;
                stDllFullName = entry.FullName;
                return hInstance;
            }
            string stModuleName = null;
            AssemblyName name = null;
            try
            {
                name = new AssemblyName(stDllName);
            }
            catch (COMException)
            {
                return Win32Api.HINSTANCE.NULL;
            }
            //Assembly assembly = MarkupSystem.FindAssembly(name);
            //if (assembly != null)
            //{
            //    stModuleName = assembly.Location;
            //    stDllFullName = assembly.FullName;
            //}
            //else
            //{
            stModuleName = stDllName + ".dll";
            stDllFullName = stDllName;
            //}
            hInstance = Win32Api.LoadLibraryEx(stModuleName, Win32Api.HANDLE.NULL, 2);
            if (hInstance != Win32Api.HINSTANCE.NULL)
            {
                DllEntry entry2 = new DllEntry
                {
                    HInstance = hInstance,
                    FullName = stDllFullName
                };
                this.m_dictDlls[stDllName] = entry2;
            }
            return hInstance;
        }

        public Resource GetResource(Win32Api.HINSTANCE hinst, string stHost, string stIdentifier)
        {
            Resource resource = null;
            if (hinst != Win32Api.HINSTANCE.NULL)
            {
                //if (MarkupSystem.RedirectionPrefix != null)
                //{
                //    string str = MarkupSystem.RedirectionPrefix + stIdentifier;
                //    if (ResourceExists(hinst, str))
                //    {
                //        resource = new DllResource(hinst, stHost, str);
                //    }
                //}
                if ((resource == null) && ResourceExists(hinst, stIdentifier))
                {
                    resource = new DllResource(hinst, stHost, stIdentifier);
                }
            }
            return resource;
        }

        public Resource GetResource(string stResource, string stUri, IDictionary dictUriData)
        {
            string str;
            string str2;
            Resource resource = null;
            ParseResource(stResource, out str, out str2);
            if (str != null)
            {
                string str3;
                Win32Api.HINSTANCE hInstance = this.GetHInstance(str, out str3);
                resource = this.GetResource(hInstance, str3, str2);
            }
            return resource;
        }

        internal static void ParseResource(string stResource, out string stHost, out string stIdentifier)
        {
            string[] strArray = stResource.Split(new char[] { '!' });
            if (strArray.Length == 2)
            {
                stHost = strArray[0];
                stIdentifier = strArray[1];
            }
            else
            {
                stHost = null;
                stIdentifier = strArray[0];
            }
        }

        public static bool ResourceExists(Win32Api.HINSTANCE hinst, string stIdentifier)
        {
            bool flag = Win32Api.FindResource(hinst.h, stIdentifier, (IntPtr)10) != IntPtr.Zero;
            if (!flag)
            {
                flag = Win32Api.FindResource(hinst.h, stIdentifier, (IntPtr)0x17) != IntPtr.Zero;
            }
            return flag;
        }

        // Properties
        public static DllResources Instance
        {
            get
            {
                return s_instance;
            }
        }

        // Nested Types
        private class DllEntry
        {
            // Fields
            public string FullName;
            public Win32Api.HINSTANCE HInstance;
        }
    }

    internal class DllResource : Resource
    {
        // Fields
        private Win32Api.HINSTANCE m_hinst;
        private IntPtr m_hResData;
        private IntPtr m_hResInfo;
        private int m_nLength;
        private IntPtr m_pDataTransient;
        private string m_stHost;
        private string m_stIdentifier;
        private Stream m_streamManagedView;

        // Methods
        internal DllResource(Win32Api.HINSTANCE hinst, string stHost, string stIdentifier)
        {
            this.m_hinst = hinst;
            this.m_stHost = stHost;
            this.m_stIdentifier = stIdentifier;
        }

        public override void Close()
        {
            if (this.m_streamManagedView != null)
            {
                this.m_streamManagedView.Close();
                this.m_streamManagedView = null;
            }
            this.m_pDataTransient = IntPtr.Zero;
            this.m_nLength = 0;
        }

        public override ResourceStatus Open()
        {
            if (this.m_hResInfo == IntPtr.Zero)
            {
                this.m_hResInfo = Win32Api.FindResource(this.m_hinst.h, this.m_stIdentifier, (IntPtr)10);
                if (this.m_hResInfo == IntPtr.Zero)
                {
                    this.m_hResInfo = Win32Api.FindResource(this.m_hinst.h, this.m_stIdentifier, (IntPtr)0x17);
                }
                if (this.m_hResInfo == IntPtr.Zero)
                {
                    goto Label_00EE;
                }
                this.m_hResData = Win32Api.LoadResource(this.m_hinst.h, this.m_hResInfo);
                if (this.m_hResData == IntPtr.Zero)
                {
                    goto Label_00EE;
                }
            }
            this.m_pDataTransient = Win32Api.LockResource(this.m_hResData);
            if (!(this.m_pDataTransient == IntPtr.Zero))
            {
                this.m_nLength = Win32Api.SizeofResource(this.m_hinst.h, this.m_hResInfo);
                return ResourceStatus.Available;
            }
        Label_00EE:
            return ResourceStatus.Error;
        }

        public override string ToString()
        {
            return (this.m_hinst.GetHashCode() + "|" + this.m_stIdentifier.ToLower(CultureInfo.InvariantCulture));
        }

        // Properties
        public override IntPtr Buffer
        {
            get
            {
                return this.m_pDataTransient;
            }
        }

        public override byte[] ByteArray
        {
            get
            {
                return Resource.ByteArrayFromIntPtr(this.m_pDataTransient, this.m_nLength);
            }
        }

        public override string Host
        {
            get
            {
                return this.m_stHost;
            }
        }

        public override string Identifier
        {
            get
            {
                return this.m_stIdentifier;
            }
        }

        public override int Length
        {
            get
            {
                return this.m_nLength;
            }
        }

        public override Stream Stream
        {
            get
            {
                if (this.m_streamManagedView == null)
                {
                    this.m_streamManagedView = Resource.StreamFromIntPtr(this.m_pDataTransient, (long)this.m_nLength);
                }
                return this.m_streamManagedView;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct HRESULT
    {
        public int hr;
        public HRESULT(int hr)
        {
            this.hr = hr;
        }

        public static bool operator ==(HRESULT hrA, HRESULT hrB)
        {
            return (hrA.hr == hrB.hr);
        }

        public static bool operator !=(HRESULT hrA, HRESULT hrB)
        {
            return (hrA.hr != hrB.hr);
        }

        public override bool Equals(object oCompare)
        {
            return ((oCompare is HRESULT) && (this.hr == ((HRESULT)oCompare).hr));
        }

        public override int GetHashCode()
        {
            return this.hr;
        }

        public override string ToString()
        {
            return ("hr:" + this.hr.ToString("X", CultureInfo.InvariantCulture));
        }

        public bool IsError()
        {
            return (this.hr < 0);
        }

        public bool IsSuccess()
        {
            return (this.hr >= 0);
        }

        public void HandleError()
        {
            if (this.IsError())
            {
                Marshal.ThrowExceptionForHR(this.hr);
            }
        }

        public int Int
        {
            get
            {
                return this.hr;
            }
        }
    }

    [SuppressUnmanagedCodeSecurity]
    internal sealed class Win32Api
    {
        // Fields
        public const int ABS_DOWNDISABLED = 8;
        public const int ABS_DOWNHOT = 6;
        public const int ABS_DOWNNORMAL = 5;
        public const int ABS_DOWNPRESSED = 7;
        public const int ABS_LEFTDISABLED = 12;
        public const int ABS_LEFTHOT = 10;
        public const int ABS_LEFTNORMAL = 9;
        public const int ABS_LEFTPRESSED = 11;
        public const int ABS_RIGHTDISABLED = 0x10;
        public const int ABS_RIGHTHOT = 14;
        public const int ABS_RIGHTNORMAL = 13;
        public const int ABS_RIGHTPRESSED = 15;
        public const int ABS_UPDISABLED = 4;
        public const int ABS_UPHOT = 2;
        public const int ABS_UPNORMAL = 1;
        public const int ABS_UPPRESSED = 3;
        public const byte AC_SRC_ALPHA = 1;
        public const byte AC_SRC_OVER = 0;
        public const int ALTERNATE = 1;
        public const int ANTIALIASED_QUALITY = 4;
        public const uint APPCOMMAND_BROWSER_BACKWARD = 1;
        public const uint APPCOMMAND_MEDIA_FAST_FORWARD = 0x31;
        public const uint APPCOMMAND_MEDIA_NEXTTRACK = 11;
        public const uint APPCOMMAND_MEDIA_PAUSE = 0x2f;
        public const uint APPCOMMAND_MEDIA_PLAY = 0x2e;
        public const uint APPCOMMAND_MEDIA_PLAY_PAUSE = 14;
        public const uint APPCOMMAND_MEDIA_PREVIOUSTRACK = 12;
        public const uint APPCOMMAND_MEDIA_RECORD = 0x30;
        public const uint APPCOMMAND_MEDIA_REWIND = 50;
        public const uint APPCOMMAND_MEDIA_STOP = 13;
        public const uint APPCOMMAND_VOLUME_DOWN = 9;
        public const uint APPCOMMAND_VOLUME_MUTE = 8;
        public const uint APPCOMMAND_VOLUME_UP = 10;
        public const uint BDR_INNER = 12;
        public const uint BDR_OUTER = 3;
        public const uint BDR_RAISED = 5;
        public const uint BDR_RAISEDINNER = 4;
        public const uint BDR_RAISEDOUTER = 1;
        public const uint BDR_SUNKEN = 10;
        public const uint BDR_SUNKENINNER = 8;
        public const uint BDR_SUNKENOUTER = 2;
        public const uint BF_ADJUST = 0x2000;
        public const uint BF_BOTTOM = 8;
        public const uint BF_BOTTOMLEFT = 9;
        public const uint BF_BOTTOMRIGHT = 12;
        public const uint BF_DIAGONAL = 0x10;
        public const uint BF_DIAGONAL_ENDBOTTOMLEFT = 0x19;
        public const uint BF_DIAGONAL_ENDBOTTOMRIGHT = 0x1c;
        public const uint BF_DIAGONAL_ENDTOPLEFT = 0x13;
        public const uint BF_DIAGONAL_ENDTOPRIGHT = 0x16;
        public const uint BF_FLAT = 0x4000;
        public const uint BF_LEFT = 1;
        public const uint BF_MIDDLE = 0x800;
        public const uint BF_MONO = 0x8000;
        public const uint BF_RECT = 15;
        public const uint BF_RIGHT = 4;
        public const uint BF_SOFT = 0x1000;
        public const uint BF_TOP = 2;
        public const uint BF_TOPLEFT = 3;
        public const uint BF_TOPRIGHT = 6;
        public const int BITSPIXEL = 12;
        public const int BLACK_BRUSH = 4;
        public const int BP_CHECKBOX = 4;
        public const int BP_GROUPBOX = 3;
        public const int BP_PUSHBUTTON = 1;
        public const int BP_RADIOBUTTON = 2;
        public const int BP_USERBUTTON = 5;
        public const uint CALLBACK_CHUNK_FINISHED = 0;
        public const uint CALLBACK_STREAM_SWITCH = 1;
        public const int CBS_CHECKEDDISABLED = 8;
        public const int CBS_CHECKEDHOT = 6;
        public const int CBS_CHECKEDNORMAL = 5;
        public const int CBS_CHECKEDPRESSED = 7;
        public const int CBS_MIXEDDISABLED = 12;
        public const int CBS_MIXEDHOT = 10;
        public const int CBS_MIXEDNORMAL = 9;
        public const int CBS_MIXEDPRESSED = 11;
        public const int CBS_UNCHECKEDDISABLED = 4;
        public const int CBS_UNCHECKEDHOT = 2;
        public const int CBS_UNCHECKEDNORMAL = 1;
        public const int CBS_UNCHECKEDPRESSED = 3;
        public const int CCERR_CHOOSECOLORCODES = 0x5000;
        public const int CDERR_DIALOGFAILURE = 0xffff;
        public const int CDERR_FINDRESFAILURE = 6;
        public const int CDERR_GENERALCODES = 0;
        public const int CDERR_INITIALIZATION = 2;
        public const int CDERR_LOADRESFAILURE = 7;
        public const int CDERR_LOADSTRFAILURE = 5;
        public const int CDERR_LOCKRESFAILURE = 8;
        public const int CDERR_MEMALLOCFAILURE = 9;
        public const int CDERR_MEMLOCKFAILURE = 10;
        public const int CDERR_NOHINSTANCE = 4;
        public const int CDERR_NOHOOK = 11;
        public const int CDERR_NOTEMPLATE = 3;
        public const int CDERR_REGISTERMSGFAIL = 12;
        public const int CDERR_STRUCTSIZE = 1;
        public const uint CDS_FULLSCREEN = 4;
        public const uint CDS_GLOBAL = 8;
        public const uint CDS_NORESET = 0x10000000;
        public const uint CDS_RESET = 0x40000000;
        public const uint CDS_SET_PRIMARY = 0x10;
        public const uint CDS_TEST = 2;
        public const uint CDS_UPDATEREGISTRY = 1;
        public const uint CDS_VIDEOPARAMETERS = 0x20;
        public const int CFERR_CHOOSEFONTCODES = 0x2000;
        public const int CFERR_MAXLESSTHANMIN = 0x2002;
        public const int CFERR_NOFONTS = 0x2001;
        public const int CLIP_DEFAULT_PRECIS = 0;
        public const int COLOR_3DDKSHADOW = 0x15;
        public const int COLOR_3DFACE = 15;
        public const int COLOR_3DHIGHLIGHT = 20;
        public const int COLOR_3DHILIGHT = 20;
        public const int COLOR_3DLIGHT = 0x16;
        public const int COLOR_3DSHADOW = 0x10;
        public const int COLOR_ACTIVEBORDER = 10;
        public const int COLOR_ACTIVECAPTION = 2;
        public const int COLOR_APPWORKSPACE = 12;
        public const int COLOR_BACKGROUND = 1;
        public const int COLOR_BTNFACE = 15;
        public const int COLOR_BTNHIGHLIGHT = 20;
        public const int COLOR_BTNHILIGHT = 20;
        public const int COLOR_BTNSHADOW = 0x10;
        public const int COLOR_BTNTEXT = 0x12;
        public const int COLOR_CAPTIONTEXT = 9;
        public const int COLOR_DESKTOP = 1;
        public const int COLOR_GRADIENTACTIVECAPTION = 0x1b;
        public const int COLOR_GRADIENTINACTIVECAPTION = 0x1c;
        public const int COLOR_GRAYTEXT = 0x11;
        public const int COLOR_HIGHLIGHT = 13;
        public const int COLOR_HIGHLIGHTTEXT = 14;
        public const int COLOR_HOTLIGHT = 0x1a;
        public const int COLOR_INACTIVEBORDER = 11;
        public const int COLOR_INACTIVECAPTION = 3;
        public const int COLOR_INACTIVECAPTIONTEXT = 0x13;
        public const int COLOR_INFOBK = 0x18;
        public const int COLOR_INFOTEXT = 0x17;
        public const int COLOR_MENU = 4;
        public const int COLOR_MENUBAR = 30;
        public const int COLOR_MENUHILIGHT = 0x1d;
        public const int COLOR_MENUTEXT = 7;
        public const int COLOR_SCROLLBAR = 0;
        public const int COLOR_WINDOW = 5;
        public const int COLOR_WINDOWFRAME = 6;
        public const int COLOR_WINDOWTEXT = 8;
        public const int COMPLEXREGION = 3;
        public const uint COPY_FILE_ALLOW_DECRYPTED_DESTINATION = 8;
        public const uint COPY_FILE_FAIL_IF_EXISTS = 1;
        public const uint COPY_FILE_OPEN_SOURCE_FOR_WRITE = 4;
        public const uint COPY_FILE_RESTARTABLE = 2;
        public const uint CS_BYTEALIGNCLIENT = 0x1000;
        public const uint CS_BYTEALIGNWINDOW = 0x2000;
        public const uint CS_CLASSDC = 0x40;
        public const uint CS_DBLCLKS = 8;
        public const uint CS_DROPSHADOW = 0x20000;
        public const uint CS_GLOBALCLASS = 0x4000;
        public const uint CS_HREDRAW = 2;
        public const uint CS_IME = 0x10000;
        public const uint CS_NOCLOSE = 0x200;
        public const uint CS_OWNDC = 0x20;
        public const uint CS_PARENTDC = 0x80;
        public const uint CS_SAVEBITS = 0x800;
        public const uint CS_VREDRAW = 1;
        public const int CSIDL_COMMON_APPDATA = 0x23;
        public const int CSIDL_COMMON_DOCUMENTS = 0x2e;
        public const int CSIDL_COMMON_VIDEO = 0x37;
        public const int CSIDL_FLAG_CREATE = 0x8000;
        public const int CW_USEDEFAULT = 0x8000;
        public const uint D3DLIGHT_DIRECTIONAL = 3;
        public const uint D3DLIGHT_POINT = 1;
        public static readonly float D3DLIGHT_RANGE_MAX = ((float)Math.Sqrt(3.4028234663852886E+38));
        public const uint D3DLIGHT_SPOT = 2;
        public const int DBT_CUSTOMEVENT = 0x8006;
        public const int DBT_DEVICEARRIVAL = 0x8000;
        public const int DBT_DEVICEQUERYREMOVE = 0x8001;
        public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        public const int DBT_DEVICEREMOVEPENDING = 0x8003;
        public const int DBT_DEVICETYPESPECIFIC = 0x8005;
        public const int DBT_DEVTYP_DEVICEINTERFACE = 5;
        public const int DBT_DEVTYP_DEVNODE = 1;
        public const int DBT_DEVTYP_HANDLE = 6;
        public const int DBT_DEVTYP_NET = 4;
        public const int DBT_DEVTYP_OEM = 0;
        public const int DBT_DEVTYP_PORT = 3;
        public const int DBT_DEVTYP_VOLUME = 2;
        public const ushort DBTF_MEDIA = 1;
        public const uint DCX_CACHE = 2;
        public const uint DCX_CLIPCHILDREN = 8;
        public const uint DCX_CLIPSIBLINGS = 0x10;
        public const uint DCX_EXCLUDERGN = 0x40;
        public const uint DCX_EXCLUDEUPDATE = 0x100;
        public const uint DCX_INTERSECTRGN = 0x80;
        public const uint DCX_INTERSECTUPDATE = 0x200;
        public const uint DCX_LOCKWINDOWUPDATE = 0x400;
        public const uint DCX_NORESETATTRS = 4;
        public const uint DCX_PARENTCLIP = 0x20;
        public const uint DCX_USESTYLE = 0x10000;
        public const uint DCX_VALIDATE = 0x200000;
        public const uint DCX_WINDOW = 1;
        public const int DEFAULT_CHARSET = 1;
        public const int DEFAULT_PITCH = 0;
        public const uint DELETE = 0x10000;
        public const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;
        public const int DEVICE_NOTIFY_SERVICE_HANDLE = 1;
        public const int DEVICE_NOTIFY_WINDOW_HANDLE = 0;
        public const uint DFC_BUTTON = 4;
        public const uint DFC_CAPTION = 1;
        public const uint DFC_MENU = 2;
        public const uint DFC_POPUPMENU = 5;
        public const uint DFC_SCROLL = 3;
        public const uint DFCS_ADJUSTRECT = 0x2000;
        public const uint DFCS_BUTTON3STATE = 8;
        public const uint DFCS_BUTTONCHECK = 0;
        public const uint DFCS_BUTTONPUSH = 0x10;
        public const uint DFCS_BUTTONRADIO = 4;
        public const uint DFCS_BUTTONRADIOIMAGE = 1;
        public const uint DFCS_BUTTONRADIOMASK = 2;
        public const uint DFCS_CAPTIONCLOSE = 0;
        public const uint DFCS_CAPTIONHELP = 4;
        public const uint DFCS_CAPTIONMAX = 2;
        public const uint DFCS_CAPTIONMIN = 1;
        public const uint DFCS_CAPTIONRESTORE = 3;
        public const uint DFCS_CHECKED = 0x400;
        public const uint DFCS_FLAT = 0x4000;
        public const uint DFCS_HOT = 0x1000;
        public const uint DFCS_INACTIVE = 0x100;
        public const uint DFCS_MENUARROW = 0;
        public const uint DFCS_MENUARROWRIGHT = 4;
        public const uint DFCS_MENUBULLET = 2;
        public const uint DFCS_MENUCHECK = 1;
        public const uint DFCS_MONO = 0x8000;
        public const uint DFCS_PUSHED = 0x200;
        public const uint DFCS_SCROLLCOMBOBOX = 5;
        public const uint DFCS_SCROLLDOWN = 1;
        public const uint DFCS_SCROLLLEFT = 2;
        public const uint DFCS_SCROLLRIGHT = 3;
        public const uint DFCS_SCROLLSIZEGRIP = 8;
        public const uint DFCS_SCROLLSIZEGRIPRIGHT = 0x10;
        public const uint DFCS_SCROLLUP = 0;
        public const uint DFCS_TRANSPARENT = 0x800;
        public const uint DI_COMPAT = 4;
        public const uint DI_DEFAULTSIZE = 8;
        public const uint DI_IMAGE = 2;
        public const uint DI_MASK = 1;
        public const uint DI_NOMIRROR = 0x10;
        public const uint DI_NORMAL = 3;
        public const int DISP_CHANGE_BADDUALVIEW = -6;
        public const int DISP_CHANGE_BADFLAGS = -4;
        public const int DISP_CHANGE_BADMODE = -2;
        public const int DISP_CHANGE_BADPARAM = -5;
        public const int DISP_CHANGE_FAILED = -1;
        public const int DISP_CHANGE_NOTUPDATED = -3;
        public const int DISP_CHANGE_RESTART = 1;
        public const int DISP_CHANGE_SUCCESSFUL = 0;
        public const uint DISPLAY_DEVICE_ATTACHED_TO_DESKTOP = 1;
        public const uint DISPLAY_DEVICE_PRIMARY_DEVICE = 4;
        public const int DKGRAY_BRUSH = 3;
        public const uint DM_INTERLACED = 2;
        public const uint DMDO_180 = 2;
        public const uint DMDO_270 = 3;
        public const uint DMDO_90 = 1;
        public const uint DMDO_DEFAULT = 0;
        public const uint DONT_RESOLVE_DLL_REFERENCES = 1;
        public const uint DRIVE_CDROM = 5;
        public const uint DRIVE_FIXED = 3;
        public const uint DRIVE_NO_ROOT_DIR = 1;
        public const uint DRIVE_RAMDISK = 6;
        public const uint DRIVE_REMOTE = 4;
        public const uint DRIVE_REMOVABLE = 2;
        public const uint DRIVE_UNKNOWN = 0;
        public const uint DS_TIMESERV_REQUIRED = 0x800;
        public const int DSSPEAKER_5POINT1 = 6;
        public const int DSSPEAKER_7POINT1 = 7;
        public const int DSSPEAKER_DIRECTOUT = 0;
        public const int DSSPEAKER_HEADPHONE = 1;
        public const int DSSPEAKER_MONO = 2;
        public const int DSSPEAKER_QUAD = 3;
        public const int DSSPEAKER_STEREO = 4;
        public const int DSSPEAKER_SURROUND = 5;
        public const int DT_BOTTOM = 8;
        public const int DT_CALCRECT = 0x400;
        public const int DT_CENTER = 1;
        public const int DT_EDITCONTROL = 0x2000;
        public const int DT_END_ELLIPSIS = 0x8000;
        public const int DT_EXPANDTABS = 0x40;
        public const int DT_EXTERNALLEADING = 0x200;
        public const int DT_HIDEPREFIX = 0x100000;
        public const int DT_INTERNAL = 0x1000;
        public const int DT_LEFT = 0;
        public const int DT_MODIFYSTRING = 0x10000;
        public const int DT_NOCLIP = 0x100;
        public const int DT_NOFULLWIDTHCHARBREAK = 0x80000;
        public const int DT_NOPREFIX = 0x800;
        public const int DT_PATH_ELLIPSIS = 0x4000;
        public const int DT_PREFIXONLY = 0x200000;
        public const int DT_RIGHT = 2;
        public const int DT_RTLREADING = 0x20000;
        public const int DT_SINGLELINE = 0x20;
        public const int DT_TABSTOP = 0x80;
        public const int DT_TOP = 0;
        public const int DT_VCENTER = 4;
        public const int DT_WORD_ELLIPSIS = 0x40000;
        public const int DT_WORDBREAK = 0x10;
        public const uint DUPLICATE_CLOSE_SOURCE = 1;
        public const uint DUPLICATE_SAME_ACCESS = 2;
        public const uint EDGE_BUMP = 9;
        public const uint EDGE_ETCHED = 6;
        public const uint EDGE_RAISED = 5;
        public const uint EDGE_SUNKEN = 10;
        public const uint ENUM_CURRENT_SETTINGS = uint.MaxValue;
        public const uint ENUM_REGISTRY_SETTINGS = 0xfffffffe;
        public const int ERROR = 0;
        public const uint ERROR_ACCESS_DENIED = 5;
        public const uint ERROR_CALL_NOT_IMPLEMENTED = 120;
        public const uint ERROR_FILE_NOT_FOUND = 2;
        public const uint ERROR_INVALID_HANDLE = 6;
        public const uint ERROR_INVALID_PARAMETER = 0x57;
        public const uint ERROR_MORE_DATA = 0xea;
        public const uint ERROR_NO_MATCH = 0x491;
        public const uint ERROR_NOT_ENOUGH_MEMORY = 8;
        public const uint ERROR_NOT_FOUND = 0x490;
        public const uint ERROR_NOT_SUPPORTED = 50;
        public const uint ERROR_OUTOFMEMORY = 14;
        public const uint ERROR_PATH_NOT_FOUND = 3;
        public const uint ERROR_SERVICE_ALREADY_RUNNING = 0x420;
        public const uint ERROR_SERVICE_DATABASE_LOCKED = 0x41f;
        public const uint ERROR_SET_NOT_FOUND = 0x492;
        public const uint ERROR_SUCCESS = 0;
        public const uint ES_AWAYMODE_REQUIRED = 0x40;
        public const uint ES_CONTINUOUS = 0x80000000;
        public const uint ES_DISPLAY_REQUIRED = 2;
        public const uint ES_SYSTEM_REQUIRED = 1;
        public const uint ES_USER_PRESENT = 4;
        public const int ETO_CLIPPED = 4;
        public const int ETO_GLYPH_INDEX = 0x10;
        public const int ETO_IGNORELANGUAGE = 0x1000;
        public const int ETO_NUMERICSLATIN = 0x800;
        public const int ETO_NUMERICSLOCAL = 0x400;
        public const int ETO_OPAQUE = 2;
        public const int ETO_PDY = 0x2000;
        public const int ETO_RTLREADING = 0x80;
        public const uint EVENT_ALL_ACCESS = 0x1f0003;
        public const uint EVENT_MODIFY_STATE = 2;
        public const int FF_DONTCARE = 0;
        public const uint FILE_APPEND_DATA = 4;
        public const uint FILE_ATTRIBUTE_ARCHIVE = 0x20;
        public const uint FILE_ATTRIBUTE_COMPRESSED = 0x800;
        public const uint FILE_ATTRIBUTE_DEVICE = 0x40;
        public const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;
        public const uint FILE_ATTRIBUTE_ENCRYPTED = 0x4000;
        public const uint FILE_ATTRIBUTE_HIDDEN = 2;
        public const uint FILE_ATTRIBUTE_NORMAL = 0x80;
        public const uint FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x2000;
        public const uint FILE_ATTRIBUTE_OFFLINE = 0x1000;
        public const uint FILE_ATTRIBUTE_READONLY = 1;
        public const uint FILE_ATTRIBUTE_REPARSE_POINT = 0x400;
        public const uint FILE_ATTRIBUTE_SPARSE_FILE = 0x200;
        public const uint FILE_ATTRIBUTE_SYSTEM = 4;
        public const uint FILE_ATTRIBUTE_TEMPORARY = 0x100;
        public const uint FILE_EXECUTE = 0x20;
        public const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        public const uint FILE_READ_ATTRIBUTES = 0x80;
        public const uint FILE_SHARE_DELETE = 4;
        public const uint FILE_SHARE_READ = 1;
        public const uint FILE_SHARE_WRITE = 2;
        public const uint FILE_WRITE_ATTRIBUTES = 0x100;
        public const uint FILE_WRITE_DATA = 2;
        public const uint FILE_WRITE_EA = 0x10;
        private const float FLT_MAX = float.MaxValue;
        public const int FNERR_BUFFERTOOSMALL = 0x3003;
        public const int FNERR_FILENAMECODES = 0x3000;
        public const int FNERR_INVALIDFILENAME = 0x3002;
        public const int FNERR_SUBCLASSFAILURE = 0x3001;
        public static Guid FOLDERID_Profile = new Guid("5E6C858F-0E22-4760-9AFE-EA3317B67173");
        public static Guid FOLDERID_Public = new Guid("DFDF76A2-C82A-4D63-906A-5644AC457385");
        public const uint FR_NOT_ENUM = 0x20;
        public const uint FR_PRIVATE = 0x10;
        public const int FRERR_BUFFERLENGTHZERO = 0x4001;
        public const int FRERR_FINDREPLACECODES = 0x4000;
        public const int FW_BOLD = 700;
        public const int FW_NORMAL = 400;
        public const int GA_PARENT = 1;
        public const int GA_ROOT = 2;
        public const int GA_ROOTOWNER = 3;
        public const int GCL_HICON = -14;
        public const uint GDI_ERROR = uint.MaxValue;
        public const uint GENERIC_EXECUTE = 0x20000000;
        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const int GM_ADVANCED = 2;
        public const int GM_COMPATIBLE = 1;
        public const int GRAY_BRUSH = 2;
        public static readonly Guid GUID_BATTERY_DISCHARGE_FLAGS_1 = new Guid(0xbcded951, 0x187b, 0x4d05, 0xbc, 0xcc, 0xf7, 0xe5, 0x19, 0x60, 0xc2, 0x58);
        public static readonly Guid GUID_BATTERY_DISCHARGE_LEVEL_1 = new Guid(0x8183ba9a, 0xe910, 0x48da, 0x87, 0x69, 20, 0xae, 0x6d, 0xc1, 0x17, 10);
        public static readonly Guid GUID_BATTERY_PERCENTAGE_REMAINING = new Guid(0xa7ad8041, 0xb45a, 0x4cae, 0x87, 0xa3, 0xee, 0xcb, 180, 0x68, 0xa9, 0xe1);
        public static readonly Guid GUID_DEVICE_BATTERY = new Guid(0x72631e54, 0x78a4, 0x11d0, 0xbc, 0xf7, 0, 170, 0, 0xb7, 0xb3, 0x2a);
        public static readonly Guid GUID_SYSTEM_AWAYMODE = new Guid(0x98a7f580, 0x1f7, 0x48aa, 0x9c, 15, 0x44, 0x35, 0x2c, 0x29, 0xe5, 0xc0);
        public const int GWL_EXSTYLE = -20;
        public const int GWL_HINSTANCE = -6;
        public const int GWL_HWNDPARENT = -8;
        public const int GWL_ID = -12;
        public const int GWL_STYLE = -16;
        public const int GWL_USERDATA = -21;
        public const int GWL_WNDPROC = -4;
        public const int HALFTONE = 4;
        public const int HCBT_ACTIVATE = 5;
        public const int HCBT_CLICKSKIPPED = 6;
        public const int HCBT_CREATEWND = 3;
        public const int HCBT_DESTROYWND = 4;
        public const int HCBT_KEYSKIPPED = 7;
        public const int HCBT_MINMAX = 1;
        public const int HCBT_MOVESIZE = 0;
        public const int HCBT_QS = 2;
        public const int HCBT_SETFOCUS = 9;
        public const int HCBT_SYSCOMMAND = 8;
        public const int HCF_HIGHCONTRASTON = 1;
        public static readonly int HH_FTS_DEFAULT_PROXIMITY = -1;
        public const int HOLLOW_BRUSH = 5;
        public const uint HSHELL_APPCOMMAND = 12;
        public const int ICON_BIG = 1;
        public const int ICON_SMALL = 0;
        public const int IDABORT = 3;
        public const int IDC_APPSTARTING = 0x7f26;
        public const int IDC_ARROW = 0x7f00;
        public const int IDC_HAND = 0x7f89;
        public const int IDC_SIZENESW = 0x7f83;
        public const int IDC_SIZENS = 0x7f85;
        public const int IDC_SIZENWSE = 0x7f82;
        public const int IDC_SIZEWE = 0x7f84;
        public const int IDC_WAIT = 0x7f02;
        public const int IDCANCEL = 2;
        public const int IDCLOSE = 8;
        public const int IDCONTINUE = 11;
        public const int IDHELP = 9;
        public const int IDIGNORE = 5;
        public const int IDNO = 7;
        public const int IDOK = 1;
        public const int IDRETRY = 4;
        public const int IDTIMEOUT = 0x7d00;
        public const int IDTRYAGAIN = 10;
        public const int IDYES = 6;
        public const uint IMAGE_BITMAP = 0;
        public const uint IMAGE_CURSOR = 2;
        public const uint IMAGE_ENHMETAFILE = 3;
        public const uint IMAGE_ICON = 1;
        public const uint INFINITE = uint.MaxValue;
        public const int INPUT_KEYBOARD = 1;
        public static IntPtr INVALID_HANDLE_VALUE = ((IntPtr)(-1));
        public const uint IOCTL_WMI_SET_MARK = 0x2200a4;
        public const int KEY_CREATE_LINK = 0x20;
        public const int KEY_CREATE_SUB_KEY = 4;
        public const int KEY_ENUMERATE_SUB_KEYS = 8;
        public const int KEY_NOTIFY = 0x10;
        public const int KEY_QUERY_VALUE = 1;
        public const int KEY_SET_VALUE = 2;
        public const int KEY_WOW64_32KEY = 0x200;
        public const int KEY_WOW64_64KEY = 0x100;
        public const int KEY_WOW64_RES = 0x300;
        public const int KEYEVENTF_EXTENDEDKEY = 1;
        public const int KEYEVENTF_KEYUP = 2;
        public const int KEYEVENTF_SCANCODE = 8;
        public const int KEYEVENTF_UNICODE = 4;
        public const int KF_FLAG_CREATE = 0x8000;
        public const int KF_FLAG_DEFAULT_PATH = 0x400;
        public const int KF_FLAG_DONT_UNEXPAND = 0x2000;
        public const int KF_FLAG_DONT_VERIFY = 0x4000;
        public const int KF_FLAG_INIT = 0x800;
        public const int KF_FLAG_NO_ALIAS = 0x1000;
        public const int KF_FLAG_NOT_PARENT_RELATIVE = 0x200;
        public const int LAYOUT_LTR = 0;
        public const int LAYOUT_RTL = 1;
        public const uint LCMAP_FULLWIDTH = 0x800000;
        public const uint LCMAP_HALFWIDTH = 0x400000;
        public const uint LCMAP_HIRAGANA = 0x100000;
        public const uint LCMAP_KATAKANA = 0x200000;
        public const uint LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x10;
        public const uint LOAD_LIBRARY_AS_DATAFILE = 2;
        public const uint LOAD_WITH_ALTERED_SEARCH_PATH = 8;
        public const int LOGPIXELSX = 0x58;
        public const int LOGPIXELSY = 90;
        public const uint LR_COLOR = 2;
        public const uint LR_COLORDELETEORG = 8;
        public const uint LR_COLORRETURNORG = 4;
        public const uint LR_COPYFROMRESOURCE = 0x4000;
        public const uint LR_CREATEDIBSECTION = 0x2000;
        public const uint LR_DEFAULTCOLOR = 0;
        public const uint LR_DEFAULTSIZE = 0x40;
        public const uint LR_LOADFROMFILE = 0x10;
        public const uint LR_LOADMAP3DCOLORS = 0x1000;
        public const uint LR_LOADTRANSPARENT = 0x20;
        public const uint LR_MONOCHROME = 1;
        public const uint LR_SHARED = 0x8000;
        public const uint LR_VGACOLOR = 0x80;
        public const int LSFW_LOCK = 1;
        public const int LSFW_UNLOCK = 2;
        public const int LTGRAY_BRUSH = 1;
        public const int MAX_PATH = 260;
        public const uint MB_ABORTRETRYIGNORE = 2;
        public const uint MB_APPLMODAL = 0;
        public const uint MB_CANCELTRYCONTINUE = 6;
        public const uint MB_DEFAULT_DESKTOP_ONLY = 0x20000;
        public const uint MB_DEFBUTTON1 = 0;
        public const uint MB_DEFBUTTON2 = 0x100;
        public const uint MB_DEFBUTTON3 = 0x200;
        public const uint MB_DEFBUTTON4 = 0x300;
        public const uint MB_HELP = 0x4000;
        public const uint MB_ICONASTERISK = 0x40;
        public const uint MB_ICONERROR = 0x10;
        public const uint MB_ICONEXCLAMATION = 0x30;
        public const uint MB_ICONHAND = 0x10;
        public const uint MB_ICONINFORMATION = 0x40;
        public const uint MB_ICONQUESTION = 0x20;
        public const uint MB_ICONSTOP = 0x10;
        public const uint MB_ICONWARNING = 0x30;
        public const uint MB_NOFOCUS = 0x8000;
        public const uint MB_OK = 0;
        public const uint MB_OKCANCEL = 1;
        public const uint MB_RETRYCANCEL = 5;
        public const uint MB_RIGHT = 0x80000;
        public const uint MB_RTLREADING = 0x100000;
        public const uint MB_SETFOREGROUND = 0x10000;
        public const uint MB_SYSTEMMODAL = 0x1000;
        public const uint MB_TASKMODAL = 0x2000;
        public const uint MB_TOPMOST = 0x40000;
        public const uint MB_USERICON = 0x80;
        public const uint MB_YESNO = 4;
        public const uint MB_YESNOCANCEL = 3;
        public const uint MF_APPEND = 0x100;
        public const uint MF_BITMAP = 4;
        public const uint MF_BYCOMMAND = 0;
        public const uint MF_BYPOSITION = 0x400;
        public const uint MF_CHANGE = 0x80;
        public const uint MF_CHECKED = 8;
        public const uint MF_DEFAULT = 0x1000;
        public const uint MF_DELETE = 0x200;
        public const uint MF_DISABLED = 2;
        public const uint MF_ENABLED = 0;
        public const uint MF_GRAYED = 1;
        public const uint MF_HELP = 0x4000;
        public const uint MF_HILITE = 0x80;
        public const uint MF_INSERT = 0;
        public const uint MF_MENUBARBREAK = 0x20;
        public const uint MF_MENUBREAK = 0x40;
        public const uint MF_OWNERDRAW = 0x100;
        public const uint MF_POPUP = 0x10;
        public const uint MF_REMOVE = 0x1000;
        public const uint MF_RIGHTJUSTIFY = 0x4000;
        public const uint MF_SEPARATOR = 0x800;
        public const uint MF_STRING = 0;
        public const uint MF_SYSMENU = 0x2000;
        public const uint MF_UNCHECKED = 0;
        public const uint MF_UNHILITE = 0;
        public const uint MF_USECHECKBITMAPS = 0x200;
        public const uint MIM_APPLYTOSUBMENUS = 0x80000000;
        public const uint MIM_BACKGROUND = 2;
        public const uint MIM_HELPID = 4;
        public const uint MIM_MAXHEIGHT = 1;
        public const uint MIM_MENUDATA = 8;
        public const uint MIM_STYLE = 0x10;
        public const int MK_CONTROL = 8;
        public const int MK_LBUTTON = 1;
        public const int MK_MBUTTON = 0x10;
        public const int MK_RBUTTON = 2;
        public const int MK_SHIFT = 4;
        public const int MK_XBUTTON1 = 0x20;
        public const int MK_XBUTTON2 = 0x40;
        public const int MM_MIXM_CONTROL_CHANGE = 0x3d1;
        public const int MM_MIXM_LINE_CHANGE = 0x3d0;
        public const uint MONITOR_DEFAULTTONEAREST = 2;
        public const uint MONITOR_DEFAULTTONULL = 0;
        public const uint MONITOR_DEFAULTTOPRIMARY = 1;
        public const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        public const int MOUSEEVENTF_LEFTDOWN = 2;
        public const int MOUSEEVENTF_LEFTUP = 4;
        public const int MOUSEEVENTF_MIDDLEDOWN = 0x20;
        public const int MOUSEEVENTF_MIDDLEUP = 0x40;
        public const int MOUSEEVENTF_MOVE = 1;
        public const int MOUSEEVENTF_RIGHTDOWN = 8;
        public const int MOUSEEVENTF_RIGHTUP = 0x10;
        public const int MOUSEEVENTF_VIRTUALDESK = 0x4000;
        public const int MOUSEEVENTF_WHEEL = 0x800;
        public const int MOUSEEVENTF_XDOWN = 0x80;
        public const int MOUSEEVENTF_XUP = 0x100;
        public const uint MOVEFILE_DELAY_UNTIL_REBOOT = 4;
        public const uint MWT_IDENTITY = 1;
        public const uint MWT_LEFTMULTIPLY = 2;
        public const uint MWT_RIGHTMULTIPLY = 3;
        public const uint NERR_Success = 0;
        public const int NETWORK_ALIVE_LAN = 1;
        public const uint NOTIFY_FOR_THIS_SESSION = 0;
        public const int NULL_BRUSH = 5;
        public const int NULLREGION = 1;
        public const int OFN_ALLOWMULTISELECT = 0x200;
        public const int OFN_CREATEPROMPT = 0x2000;
        public const int OFN_DONTADDTORECENT = 0x2000000;
        public const int OFN_ENABLEHOOK = 0x20;
        public const int OFN_ENABLEINCLUDENOTIFY = 0x400000;
        public const int OFN_ENABLESIZING = 0x800000;
        public const int OFN_ENABLETEMPLATE = 0x40;
        public const int OFN_ENABLETEMPLATEHANDLE = 0x80;
        public const int OFN_EX_NOPLACESBAR = 1;
        public const int OFN_EXPLORER = 0x80000;
        public const int OFN_EXTENSIONDIFFERENT = 0x400;
        public const int OFN_FILEMUSTEXIST = 0x1000;
        public const int OFN_FORCESHOWHIDDEN = 0x10000000;
        public const int OFN_HIDEREADONLY = 4;
        public const int OFN_LONGNAMES = 0x200000;
        public const int OFN_NOCHANGEDIR = 8;
        public const int OFN_NODEREFERENCELINKS = 0x100000;
        public const int OFN_NOLONGNAMES = 0x40000;
        public const int OFN_NONETWORKBUTTON = 0x20000;
        public const int OFN_NOREADONLYRETURN = 0x8000;
        public const int OFN_NOTESTFILECREATE = 0x10000;
        public const int OFN_NOVALIDATE = 0x100;
        public const int OFN_OVERWRITEPROMPT = 2;
        public const int OFN_PATHMUSTEXIST = 0x800;
        public const int OFN_READONLY = 1;
        public const int OFN_SHAREAWARE = 0x4000;
        public const int OFN_SHOWHELP = 0x10;
        public const int OPAQUE = 2;
        public const uint OPEN_ALWAYS = 4;
        public const uint OPEN_EXISTING = 3;
        public const int OUT_DEFAULT_PRECIS = 0;
        public const int PBS_DEFAULTED = 5;
        public const int PBS_DISABLED = 4;
        public const int PBS_HOT = 2;
        public const int PBS_NORMAL = 1;
        public const int PBS_PRESSED = 3;
        public const uint PBT_APMBATTERYLOW = 9;
        public const uint PBT_APMOEMEVENT = 11;
        public const uint PBT_APMPOWERSTATUSCHANGE = 10;
        public const uint PBT_APMQUERYSUSPEND = 0;
        public const uint PBT_APMQUERYSUSPENDFAILED = 2;
        public const uint PBT_APMRESUMEAUTOMATIC = 0x12;
        public const uint PBT_APMRESUMECRITICAL = 6;
        public const uint PBT_APMRESUMESUSPEND = 7;
        public const uint PBT_APMSUSPEND = 4;
        public const uint PBT_POWERSETTINGCHANGE = 0x8013;
        public const uint PBTF_APMRESUMEFROMFAILURE = 1;
        public const int PDERR_CREATEICFAILURE = 0x100a;
        public const int PDERR_DEFAULTDIFFERENT = 0x100c;
        public const int PDERR_DNDMMISMATCH = 0x1009;
        public const int PDERR_GETDEVMODEFAIL = 0x1005;
        public const int PDERR_INITFAILURE = 0x1006;
        public const int PDERR_LOADDRVFAILURE = 0x1004;
        public const int PDERR_NODEFAULTPRN = 0x1008;
        public const int PDERR_NODEVICES = 0x1007;
        public const int PDERR_PARSEFAILURE = 0x1002;
        public const int PDERR_PRINTERCODES = 0x1000;
        public const int PDERR_PRINTERNOTFOUND = 0x100b;
        public const int PDERR_RETDEFFAILURE = 0x1003;
        public const int PDERR_SETUPFAILURE = 0x1001;
        public const int PLANES = 14;
        public const uint PM_NOREMOVE = 0;
        public const uint PM_NOYIELD = 2;
        public const uint PM_REMOVE = 1;
        public const ushort PROCESSOR_ARCHITECTURE_AMD64 = 9;
        public const ushort PROCESSOR_ARCHITECTURE_IA32_ON_WIN64 = 10;
        public const ushort PROCESSOR_ARCHITECTURE_IA64 = 6;
        public const ushort PROCESSOR_ARCHITECTURE_INTEL = 0;
        public const ushort PROCESSOR_ARCHITECTURE_UNKNOWN = 0xffff;
        public const uint PROGRESS_CANCEL = 1;
        public const uint PROGRESS_CONTINUE = 0;
        public const uint PROGRESS_QUIET = 3;
        public const uint PROGRESS_STOP = 2;
        public const int RBS_CHECKEDDISABLED = 8;
        public const int RBS_CHECKEDHOT = 6;
        public const int RBS_CHECKEDNORMAL = 5;
        public const int RBS_CHECKEDPRESSED = 7;
        public const int RBS_UNCHECKEDDISABLED = 4;
        public const int RBS_UNCHECKEDHOT = 2;
        public const int RBS_UNCHECKEDNORMAL = 1;
        public const int RBS_UNCHECKEDPRESSED = 3;
        public const int RDW_ALLCHILDREN = 0x80;
        public const int RDW_ERASE = 4;
        public const int RDW_ERASENOW = 0x200;
        public const int RDW_FRAME = 0x400;
        public const int RDW_INTERNALPAINT = 2;
        public const int RDW_INVALIDATE = 1;
        public const int RDW_NOCHILDREN = 0x40;
        public const int RDW_NOERASE = 0x20;
        public const int RDW_NOFRAME = 0x800;
        public const int RDW_NOINTERNALPAINT = 0x10;
        public const int RDW_UPDATENOW = 0x100;
        public const int RDW_VALIDATE = 8;
        public const uint READ_CONTROL = 0x20000;
        public const int REG_NOTIFY_CHANGE_ATTRIBUTES = 2;
        public const int REG_NOTIFY_CHANGE_LAST_SET = 4;
        public const int REG_NOTIFY_CHANGE_NAME = 1;
        public const int REG_NOTIFY_CHANGE_SECURITY = 8;
        public const uint REG_SZ = 1;
        public const int RGN_AND = 1;
        public const int RGN_COPY = 5;
        public const int RGN_DIFF = 4;
        public const int RGN_OR = 2;
        public const int RGN_XOR = 3;
        public const int RT_ACCELERATOR = 9;
        public const int RT_BITMAP = 2;
        public const int RT_CURSOR = 1;
        public const int RT_DIALOG = 5;
        public const int RT_FONT = 8;
        public const int RT_FONTDIR = 7;
        public const int RT_HTML = 0x17;
        public const int RT_ICON = 3;
        public const int RT_MENU = 4;
        public const int RT_RCDATA = 10;
        public const int RT_STRING = 6;
        private static readonly string[] s_rgsMessageNames = new string[0];
        public const int SB_BOTTOM = 7;
        public const int SB_CONST_ALPHA = 1;
        public const int SB_CTL = 2;
        public const int SB_ENDSCROLL = 8;
        public const int SB_HORZ = 0;
        public const int SB_LEFT = 6;
        public const int SB_LINEDOWN = 1;
        public const int SB_LINELEFT = 0;
        public const int SB_LINERIGHT = 1;
        public const int SB_LINEUP = 0;
        public const int SB_PAGEDOWN = 3;
        public const int SB_PAGELEFT = 2;
        public const int SB_PAGERIGHT = 3;
        public const int SB_PAGEUP = 2;
        public const int SB_RIGHT = 7;
        public const int SB_THUMBPOSITION = 4;
        public const int SB_THUMBTRACK = 5;
        public const int SB_TOP = 6;
        public const int SB_VERT = 1;
        public const int SBP_ARROWBTN = 1;
        public const int SBP_GRIPPERHORZ = 8;
        public const int SBP_GRIPPERVERT = 9;
        public const int SBP_LOWERTRACKHORZ = 4;
        public const int SBP_LOWERTRACKVERT = 6;
        public const int SBP_SIZEBOX = 10;
        public const int SBP_THUMBBTNHORZ = 2;
        public const int SBP_THUMBBTNVERT = 3;
        public const int SBP_UPPERTRACKHORZ = 5;
        public const int SBP_UPPERTRACKVERT = 7;
        public const uint SC_ARRANGE = 0xf110;
        public const uint SC_CLOSE = 0xf060;
        public const uint SC_CONTEXTHELP = 0xf180;
        public const uint SC_DEFAULT = 0xf160;
        public const uint SC_HOTKEY = 0xf150;
        public const uint SC_HSCROLL = 0xf080;
        public const uint SC_KEYMENU = 0xf100;
        public const uint SC_MAXIMIZE = 0xf030;
        public const uint SC_MINIMIZE = 0xf020;
        public const uint SC_MONITORPOWER = 0xf170;
        public const uint SC_MOUSEMENU = 0xf090;
        public const uint SC_MOVE = 0xf010;
        public const uint SC_NEXTWINDOW = 0xf040;
        public const uint SC_PREVWINDOW = 0xf050;
        public const uint SC_RESTORE = 0xf120;
        public const uint SC_SCREENSAVE = 0xf140;
        public const uint SC_SIZE = 0xf000;
        public const uint SC_TASKLIST = 0xf130;
        public const uint SC_VSCROLL = 0xf070;
        public const int SCRBS_DISABLED = 4;
        public const int SCRBS_HOT = 2;
        public const int SCRBS_NORMAL = 1;
        public const int SCRBS_PRESSED = 3;
        public const uint SERVICE_CONFIG_DELAYED_AUTO_START_INFO = 3;
        public const uint SERVICE_CONFIG_DESCRIPTION = 1;
        public const uint SERVICE_CONFIG_FAILURE_ACTIONS = 2;
        public const uint SERVICE_CONFIG_FAILURE_ACTIONS_FLAG = 4;
        public const uint SERVICE_CONFIG_PRESHUTDOWN_INFO = 7;
        public const uint SERVICE_CONFIG_REQUIRED_PRIVILEGES_INFO = 6;
        public const uint SERVICE_CONFIG_SERVICE_SID_INFO = 5;
        public const uint SERVICE_CONTROL_PARAMCHANGE = 6;
        public const uint SERVICE_NO_CHANGE = uint.MaxValue;
        public const int SHADEBLENDCAPS = 120;
        public const int SHGFP_TYPE_CURRENT = 0;
        public const int SIF_DISABLENOSCROLL = 8;
        public const int SIF_PAGE = 2;
        public const int SIF_POS = 4;
        public const int SIF_RANGE = 1;
        public const int SIF_TRACKPOS = 0x10;
        public const int SIMPLEREGION = 2;
        public const uint SIZEOF_DISPLAY_DEVICE = 840;
        public const int SM_ARRANGE = 0x38;
        public const int SM_CLEANBOOT = 0x43;
        public const int SM_CMETRICS = 0x56;
        public const int SM_CMONITORS = 80;
        public const int SM_CMOUSEBUTTONS = 0x2b;
        public const int SM_CXBORDER = 5;
        public const int SM_CXCURSOR = 13;
        public const int SM_CXDLGFRAME = 7;
        public const int SM_CXDOUBLECLK = 0x24;
        public const int SM_CXDRAG = 0x44;
        public const int SM_CXEDGE = 0x2d;
        public const int SM_CXFIXEDFRAME = 7;
        public const int SM_CXFOCUSBORDER = 0x53;
        public const int SM_CXFRAME = 0x20;
        public const int SM_CXFULLSCREEN = 0x10;
        public const int SM_CXHSCROLL = 0x15;
        public const int SM_CXHTHUMB = 10;
        public const int SM_CXICON = 11;
        public const int SM_CXICONSPACING = 0x26;
        public const int SM_CXMAXIMIZED = 0x3d;
        public const int SM_CXMAXTRACK = 0x3b;
        public const int SM_CXMENUCHECK = 0x47;
        public const int SM_CXMENUSIZE = 0x36;
        public const int SM_CXMIN = 0x1c;
        public const int SM_CXMINIMIZED = 0x39;
        public const int SM_CXMINSPACING = 0x2f;
        public const int SM_CXMINTRACK = 0x22;
        public const int SM_CXSCREEN = 0;
        public const int SM_CXSIZE = 30;
        public const int SM_CXSIZEFRAME = 0x20;
        public const int SM_CXSMICON = 0x31;
        public const int SM_CXSMSIZE = 0x34;
        public const int SM_CXVIRTUALSCREEN = 0x4e;
        public const int SM_CXVSCROLL = 2;
        public const int SM_CYBORDER = 6;
        public const int SM_CYCAPTION = 4;
        public const int SM_CYCURSOR = 14;
        public const int SM_CYDLGFRAME = 8;
        public const int SM_CYDOUBLECLK = 0x25;
        public const int SM_CYDRAG = 0x45;
        public const int SM_CYEDGE = 0x2e;
        public const int SM_CYFIXEDFRAME = 8;
        public const int SM_CYFOCUSBORDER = 0x54;
        public const int SM_CYFRAME = 0x21;
        public const int SM_CYFULLSCREEN = 0x11;
        public const int SM_CYHSCROLL = 3;
        public const int SM_CYICON = 12;
        public const int SM_CYICONSPACING = 0x27;
        public const int SM_CYKANJIWINDOW = 0x12;
        public const int SM_CYMAXIMIZED = 0x3e;
        public const int SM_CYMAXTRACK = 60;
        public const int SM_CYMENU = 15;
        public const int SM_CYMENUCHECK = 0x48;
        public const int SM_CYMENUSIZE = 0x37;
        public const int SM_CYMIN = 0x1d;
        public const int SM_CYMINIMIZED = 0x3a;
        public const int SM_CYMINSPACING = 0x30;
        public const int SM_CYMINTRACK = 0x23;
        public const int SM_CYSCREEN = 1;
        public const int SM_CYSIZE = 0x1f;
        public const int SM_CYSIZEFRAME = 0x21;
        public const int SM_CYSMCAPTION = 0x33;
        public const int SM_CYSMICON = 50;
        public const int SM_CYSMSIZE = 0x35;
        public const int SM_CYVIRTUALSCREEN = 0x4f;
        public const int SM_CYVSCROLL = 20;
        public const int SM_CYVTHUMB = 9;
        public const int SM_DBCSENABLED = 0x2a;
        public const int SM_DEBUG = 0x16;
        public const int SM_IMMENABLED = 0x52;
        public const int SM_MEDIACENTER = 0x57;
        public const int SM_MENUDROPALIGNMENT = 40;
        public const int SM_MIDEASTENABLED = 0x4a;
        public const int SM_MOUSEPRESENT = 0x13;
        public const int SM_MOUSEWHEELPRESENT = 0x4b;
        public const int SM_NETWORK = 0x3f;
        public const int SM_PENWINDOWS = 0x29;
        public const int SM_REMOTESESSION = 0x1000;
        public const int SM_RESERVED1 = 0x18;
        public const int SM_RESERVED2 = 0x19;
        public const int SM_RESERVED3 = 0x1a;
        public const int SM_RESERVED4 = 0x1b;
        public const int SM_SAMEDISPLAYFORMAT = 0x51;
        public const int SM_SECURE = 0x2c;
        public const int SM_SHOWSOUNDS = 70;
        public const int SM_SHUTTINGDOWN = 0x2000;
        public const int SM_SLOWMACHINE = 0x49;
        public const int SM_SWAPBUTTON = 0x17;
        public const int SM_XVIRTUALSCREEN = 0x4c;
        public const int SM_YVIRTUALSCREEN = 0x4d;
        public const int SMTO_ABORTIFHUNG = 2;
        public const int SMTO_BLOCK = 1;
        public const int SMTO_NORMAL = 0;
        public const uint SND_ALIAS = 0x10000;
        public const uint SND_ALIAS_ID = 0x110000;
        public const uint SND_APPLICATION = 0x80;
        public const uint SND_ASYNC = 1;
        public const uint SND_FILENAME = 0x20000;
        public const uint SND_LOOP = 8;
        public const uint SND_MEMORY = 4;
        public const uint SND_NODEFAULT = 2;
        public const uint SND_NOSTOP = 0x10;
        public const uint SND_NOWAIT = 0x2000;
        public const uint SND_PURGE = 0x40;
        public const uint SND_RESOURCE = 0x40004;
        public const uint SND_SYNC = 0;
        public const int SPI_GETHIGHCONTRAST = 0x42;
        public const int SPI_GETMENUANIMATION = 0x1002;
        public const int SPI_GETMENUFADE = 0x1012;
        public const int SPI_GETSCREENSAVEACTIVE = 0x10;
        public const int SPI_GETSCREENSAVERRUNNING = 0x72;
        public const int SPI_GETSCREENSAVESECURE = 0x76;
        public const int SPI_GETSCREENSAVETIMEOUT = 14;
        public const int SPI_SETSCREENSAVEACTIVE = 0x11;
        public const int SPI_SETSCREENSAVERRUNNING = 0x61;
        public const int SPI_SETSCREENSAVESECURE = 0x77;
        public const int SPI_SETSCREENSAVETIMEOUT = 15;
        public const int SPIF_SENDWININICHANGE = 2;
        public static uint SRCCOPY = 0xcc0020;
        public const uint STANDARD_RIGHTS_READ = 0x20000;
        public const uint STANDARD_RIGHTS_WRITE = 0x20000;
        public const uint STATUS_ABANDONED_WAIT_0 = 0x80;
        public const int SW_FORCEMINIMIZE = 11;
        public const int SW_HIDE = 0;
        public const int SW_MAXIMIZE = 3;
        public const int SW_MINIMIZE = 6;
        public const int SW_NORMAL = 1;
        public const int SW_RESTORE = 9;
        public const int SW_SHOW = 5;
        public const int SW_SHOWDEFAULT = 10;
        public const int SW_SHOWMAXIMIZED = 3;
        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_SHOWMINNOACTIVE = 7;
        public const int SW_SHOWNA = 8;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int SW_SHOWNORMAL = 1;
        public const int SWP_ASYNCWINDOWPOS = 0x4000;
        public const int SWP_DEFERERASE = 0x2000;
        public const int SWP_DRAWFRAME = 0x20;
        public const int SWP_FRAMECHANGED = 0x20;
        public const int SWP_HIDEWINDOW = 0x80;
        public const int SWP_NOACTIVATE = 0x10;
        public const int SWP_NOCOPYBITS = 0x100;
        public const int SWP_NOMOVE = 2;
        public const int SWP_NOOWNERZORDER = 0x200;
        public const int SWP_NOREDRAW = 8;
        public const int SWP_NOREPOSITION = 0x200;
        public const int SWP_NOSENDCHANGING = 0x400;
        public const int SWP_NOSIZE = 1;
        public const int SWP_NOZORDER = 4;
        public const int SWP_SHOWWINDOW = 0x40;
        public const uint SYNCHRONIZE = 0x100000;
        public const int SZB_LEFTALIGN = 2;
        public const int SZB_RIGHTALIGN = 1;
        public const uint TA_BASELINE = 0x18;
        public const uint TA_BOTTOM = 8;
        public const uint TA_CENTER = 6;
        public const uint TA_LEFT = 0;
        public const uint TA_RIGHT = 2;
        public const uint TA_RTLREADING = 0x100;
        public const uint TA_TOP = 0;
        public const uint TME_CANCEL = 0x80000000;
        public const uint TME_HOVER = 1;
        public const uint TME_LEAVE = 2;
        public const uint TME_NONCLIENT = 0x10;
        public const uint TME_QUERY = 0x40000000;
        public const uint TMPF_DEVICE = 8;
        public const uint TMPF_FIXED_PITCH = 1;
        public const uint TMPF_TRUETYPE = 4;
        public const uint TMPF_VECTOR = 2;
        public const int TP_BUTTON = 1;
        public const int TP_DROPDOWNBUTTON = 2;
        public const int TP_SEPARATOR = 5;
        public const int TP_SEPARATORVERT = 6;
        public const int TP_SPLITBUTTON = 3;
        public const int TP_SPLITBUTTONDROPDOWN = 4;
        public const int TRANSPARENT = 1;
        public const int TS_CHECKED = 5;
        public const int TS_DISABLED = 4;
        public const int TS_HOT = 2;
        public const int TS_HOTCHECKED = 6;
        public const int TS_NORMAL = 1;
        public const int TS_PRESSED = 3;
        public const int VK_0 = 0x30;
        public const int VK_1 = 0x31;
        public const int VK_2 = 50;
        public const int VK_3 = 0x33;
        public const int VK_4 = 0x34;
        public const int VK_5 = 0x35;
        public const int VK_6 = 0x36;
        public const int VK_7 = 0x37;
        public const int VK_8 = 0x38;
        public const int VK_9 = 0x39;
        public const int VK_A = 0x41;
        public const int VK_ACCEPT = 30;
        public const int VK_ADD = 0x6b;
        public const int VK_APPS = 0x5d;
        public const int VK_ATTN = 0xf6;
        public const int VK_B = 0x42;
        public const int VK_BACK = 8;
        public const int VK_BROWSER_BACK = 0xa6;
        public const int VK_BROWSER_FAVORITES = 0xab;
        public const int VK_BROWSER_FORWARD = 0xa7;
        public const int VK_BROWSER_HOME = 0xac;
        public const int VK_BROWSER_REFRESH = 0xa8;
        public const int VK_BROWSER_SEARCH = 170;
        public const int VK_BROWSER_STOP = 0xa9;
        public const int VK_C = 0x43;
        public const int VK_CANCEL = 3;
        public const int VK_CAPITAL = 20;
        public const int VK_CLEAR = 12;
        public const int VK_CONTROL = 0x11;
        public const int VK_CONVERT = 0x1c;
        public const int VK_CRSEL = 0xf7;
        public const int VK_D = 0x44;
        public const int VK_DECIMAL = 110;
        public const int VK_DELETE = 0x2e;
        public const int VK_DIVIDE = 0x6f;
        public const int VK_DOWN = 40;
        public const int VK_E = 0x45;
        public const int VK_END = 0x23;
        public const int VK_EREOF = 0xf9;
        public const int VK_ESCAPE = 0x1b;
        public const int VK_EXECUTE = 0x2b;
        public const int VK_EXSEL = 0xf8;
        public const int VK_F = 70;
        public const int VK_F1 = 0x70;
        public const int VK_F10 = 0x79;
        public const int VK_F11 = 0x7a;
        public const int VK_F12 = 0x7b;
        public const int VK_F13 = 0x7c;
        public const int VK_F14 = 0x7d;
        public const int VK_F15 = 0x7e;
        public const int VK_F16 = 0x7f;
        public const int VK_F17 = 0x80;
        public const int VK_F18 = 0x81;
        public const int VK_F19 = 130;
        public const int VK_F2 = 0x71;
        public const int VK_F20 = 0x83;
        public const int VK_F21 = 0x84;
        public const int VK_F22 = 0x85;
        public const int VK_F23 = 0x86;
        public const int VK_F24 = 0x87;
        public const int VK_F3 = 0x72;
        public const int VK_F4 = 0x73;
        public const int VK_F5 = 0x74;
        public const int VK_F6 = 0x75;
        public const int VK_F7 = 0x76;
        public const int VK_F8 = 0x77;
        public const int VK_F9 = 120;
        public const int VK_FINAL = 0x18;
        public const int VK_G = 0x47;
        public const int VK_H = 0x48;
        public const int VK_HANGEUL = 0x15;
        public const int VK_HANGUL = 0x15;
        public const int VK_HANJA = 0x19;
        public const int VK_HELP = 0x2f;
        public const int VK_HOME = 0x24;
        public const int VK_I = 0x49;
        public const int VK_INSERT = 0x2d;
        public const int VK_J = 0x4a;
        public const int VK_JUNJA = 0x17;
        public const int VK_K = 0x4b;
        public const int VK_KANA = 0x15;
        public const int VK_KANJI = 0x19;
        public const int VK_L = 0x4c;
        public const int VK_LAUNCH_APP1 = 0xb6;
        public const int VK_LAUNCH_APP2 = 0xb7;
        public const int VK_LAUNCH_MAIL = 180;
        public const int VK_LAUNCH_MEDIA_SELECT = 0xb5;
        public const int VK_LBUTTON = 1;
        public const int VK_LCONTROL = 0xa2;
        public const int VK_LEFT = 0x25;
        public const int VK_LMENU = 0xa4;
        public const int VK_LSHIFT = 160;
        public const int VK_LWIN = 0x5b;
        public const int VK_M = 0x4d;
        public const int VK_MBUTTON = 4;
        public const int VK_MEDIA_NEXT_TRACK = 0xb0;
        public const int VK_MEDIA_PLAY_PAUSE = 0xb3;
        public const int VK_MEDIA_PREV_TRACK = 0xb1;
        public const int VK_MEDIA_STOP = 0xb2;
        public const int VK_MENU = 0x12;
        public const int VK_MODECHANGE = 0x1f;
        public const int VK_MULTIPLY = 0x6a;
        public const int VK_N = 0x4e;
        public const int VK_NEXT = 0x22;
        public const int VK_NONAME = 0xfc;
        public const int VK_NONCONVERT = 0x1d;
        public const int VK_NUMLOCK = 0x90;
        public const int VK_NUMPAD0 = 0x60;
        public const int VK_NUMPAD1 = 0x61;
        public const int VK_NUMPAD2 = 0x62;
        public const int VK_NUMPAD3 = 0x63;
        public const int VK_NUMPAD4 = 100;
        public const int VK_NUMPAD5 = 0x65;
        public const int VK_NUMPAD6 = 0x66;
        public const int VK_NUMPAD7 = 0x67;
        public const int VK_NUMPAD8 = 0x68;
        public const int VK_NUMPAD9 = 0x69;
        public const int VK_O = 0x4f;
        public const int VK_OEM_CLEAR = 0xfe;
        public const int VK_P = 80;
        public const int VK_PA1 = 0xfd;
        public const int VK_PACKET = 0xe7;
        public const int VK_PAUSE = 0x13;
        public const int VK_PLAY = 250;
        public const int VK_POWER = 0x5e;
        public const int VK_PRINT = 0x2a;
        public const int VK_PRIOR = 0x21;
        public const int VK_PROCESSKEY = 0xe5;
        public const int VK_Q = 0x51;
        public const int VK_R = 0x52;
        public const int VK_RBUTTON = 2;
        public const int VK_RCONTROL = 0xa3;
        public const int VK_RETURN = 13;
        public const int VK_RIGHT = 0x27;
        public const int VK_RMENU = 0xa5;
        public const int VK_RSHIFT = 0xa1;
        public const int VK_RWIN = 0x5c;
        public const int VK_S = 0x53;
        public const int VK_SCROLL = 0x91;
        public const int VK_SELECT = 0x29;
        public const int VK_SEPARATOR = 0x6c;
        public const int VK_SHIFT = 0x10;
        public const int VK_SLEEP = 0x5f;
        public const int VK_SNAPSHOT = 0x2c;
        public const int VK_SPACE = 0x20;
        public const int VK_SUBTRACT = 0x6d;
        public const int VK_T = 0x54;
        public const int VK_TAB = 9;
        public const int VK_U = 0x55;
        public const int VK_UP = 0x26;
        public const int VK_V = 0x56;
        public const int VK_VOLUME_DOWN = 0xae;
        public const int VK_VOLUME_MUTE = 0xad;
        public const int VK_VOLUME_UP = 0xaf;
        public const int VK_W = 0x57;
        public const int VK_X = 0x58;
        public const int VK_XBUTTON1 = 5;
        public const int VK_XBUTTON2 = 6;
        public const int VK_Y = 0x59;
        public const int VK_Z = 90;
        public const int VK_ZOOM = 0xfb;
        public const uint W32TIME_CONFIG_MANUAL_PEER_LIST = 1;
        public const int WA_ACTIVE = 1;
        public const int WA_CLICKACTIVE = 2;
        public const int WA_INACTIVE = 0;
        public const uint WAIT_ABANDONED = 0x80;
        public const uint WAIT_OBJECT_0 = 0;
        public const uint WAIT_TIMEOUT = 0x102;
        public const int WHITE_BRUSH = 0;
        public const int WINDING = 2;
        public const uint WM_ACTIVATE = 6;
        public const uint WM_ACTIVATEOWNEDWINDOW = 0x408;
        public const uint WM_APPCOMMAND = 0x319;
        public const uint WM_CHAR = 0x102;
        public const uint WM_CLOSE = 0x10;
        public const uint WM_COMMAND = 0x111;
        public const uint WM_COPYDATA = 0x4a;
        public const uint WM_CREATE = 1;
        public const uint WM_DESTROY = 2;
        public const uint WM_DEVICECHANGE = 0x219;
        public const uint WM_DISPLAYCHANGE = 0x7e;
        public const uint WM_ENABLE = 10;
        public const uint WM_ENDSESSION = 0x16;
        public const uint WM_ERASEBKGND = 20;
        public const uint WM_EXITSIZEMOVE = 0x232;
        public const uint WM_GETICON = 0x7f;
        public const uint WM_GETMINMAXINFO = 0x24;
        public const uint WM_GETOBJECT = 0x3d;
        public const uint WM_GETTEXT = 13;
        public const uint WM_GETTEXTLENGTH = 14;
        public const uint WM_HSCROLL = 0x114;
        public const uint WM_INPUT = 0xff;
        public const uint WM_KEYDOWN = 0x100;
        public const uint WM_KEYFIRST = 0x100;
        public const uint WM_KEYLAST = 0x109;
        public const uint WM_KEYUP = 0x101;
        public const uint WM_KILLFOCUS = 8;
        public const uint WM_LBUTTONDBLCLK = 0x203;
        public const uint WM_LBUTTONDOWN = 0x201;
        public const uint WM_LBUTTONUP = 0x202;
        public const uint WM_MBUTTONDBLCLK = 0x209;
        public const uint WM_MBUTTONDOWN = 0x207;
        public const uint WM_MBUTTONUP = 520;
        public const uint WM_MOUSEFIRST = 0x200;
        public const uint WM_MOUSEHOVER = 0x2a1;
        public const uint WM_MOUSELAST = 0x20d;
        public const uint WM_MOUSELEAVE = 0x2a3;
        public const uint WM_MOUSEMOVE = 0x200;
        public const uint WM_MOUSEWHEEL = 0x20a;
        public const uint WM_MOVE = 3;
        public const uint WM_NCCREATE = 0x81;
        public const uint WM_NCDESTROY = 130;
        public const uint WM_NULL = 0;
        public const uint WM_PAINT = 15;
        public const uint WM_POWERBROADCAST = 0x218;
        public const uint WM_QUERYENDSESSION = 0x11;
        public const uint WM_QUIT = 0x12;
        public const uint WM_RBUTTONDBLCLK = 0x206;
        public const uint WM_RBUTTONDOWN = 0x204;
        public const uint WM_RBUTTONUP = 0x205;
        public const uint WM_SETCURSOR = 0x20;
        public const uint WM_SETFOCUS = 7;
        public const uint WM_SETFONT = 0x30;
        public const uint WM_SETICON = 0x80;
        public const uint WM_SETREDRAW = 11;
        public const uint WM_SETTEXT = 12;
        public const uint WM_SETTINGCHANGE = 0x1a;
        public const uint WM_SHOWWINDOW = 0x18;
        public const uint WM_SIZE = 5;
        public const uint WM_SYSCHAR = 0x106;
        public const uint WM_SYSCOMMAND = 0x112;
        public const uint WM_SYSKEYDOWN = 260;
        public const uint WM_SYSKEYUP = 0x105;
        public const uint WM_TIMECHANGE = 30;
        public const uint WM_TIMER = 0x113;
        public const uint WM_USER = 0x400;
        public const uint WM_VSCROLL = 0x115;
        public const uint WM_WINDOWPOSCHANGED = 0x47;
        public const uint WM_WTSSESSION_CHANGE = 0x2b1;
        public const uint WM_XBUTTONDBLCLK = 0x20d;
        public const uint WM_XBUTTONDOWN = 0x20b;
        public const uint WM_XBUTTONUP = 0x20c;
        public const uint WPF_RESTORETOMAXIMIZED = 3;
        public const uint WS_BORDER = 0x800000;
        public const uint WS_CAPTION = 0xc00000;
        public const uint WS_CHILD = 0x40000000;
        public const uint WS_CLIPCHILDREN = 0x2000000;
        public const uint WS_CLIPSIBLINGS = 0x4000000;
        public const uint WS_DLGFRAME = 0x400000;
        public const uint WS_EX_ACCEPTFILES = 0x10;
        public const uint WS_EX_APPWINDOW = 0x40000;
        public const uint WS_EX_CLIENTEDGE = 0x200;
        public const uint WS_EX_COMPOSITED = 0x2000000;
        public const uint WS_EX_CONTEXTHELP = 0x400;
        public const uint WS_EX_CONTROLPARENT = 0x10000;
        public const uint WS_EX_DLGMODALFRAME = 1;
        public const uint WS_EX_LAYERED = 0x80000;
        public const uint WS_EX_LAYOUTRTL = 0x400000;
        public const uint WS_EX_LEFT = 0;
        public const uint WS_EX_LEFTSCROLLBAR = 0x4000;
        public const uint WS_EX_LTRREADING = 0;
        public const uint WS_EX_MDICHILD = 0x40;
        public const uint WS_EX_NOACTIVATE = 0x8000000;
        public const uint WS_EX_NOINHERITLAYOUT = 0x100000;
        public const uint WS_EX_NOPARENTNOTIFY = 4;
        public const uint WS_EX_OVERLAPPEDWINDOW = 0x300;
        public const uint WS_EX_PALETTEWINDOW = 0x188;
        public const uint WS_EX_RIGHT = 0x1000;
        public const uint WS_EX_RIGHTSCROLLBAR = 0;
        public const uint WS_EX_RTLREADING = 0x2000;
        public const uint WS_EX_STATICEDGE = 0x20000;
        public const uint WS_EX_TOOLWINDOW = 0x80;
        public const uint WS_EX_TOPMOST = 8;
        public const uint WS_EX_TRANSPARENT = 0x20;
        public const uint WS_EX_WINDOWEDGE = 0x100;
        public const uint WS_HSCROLL = 0x100000;
        public const uint WS_MAXIMIZE = 0x1000000;
        public const uint WS_MAXIMIZEBOX = 0x10000;
        public const uint WS_MINIMIZE = 0x20000000;
        public const uint WS_MINIMIZEBOX = 0x20000;
        public const uint WS_OVERLAPPED = 0;
        public const uint WS_OVERLAPPEDWINDOW = 0xcf0000;
        public const uint WS_POPUP = 0x80000000;
        public const uint WS_SYSMENU = 0x80000;
        public const uint WS_THICKFRAME = 0x40000;
        public const uint WS_VISIBLE = 0x10000000;
        public const uint WS_VSCROLL = 0x200000;
        public const uint WTS_CONSOLE_CONNECT = 1;
        public const uint WTS_CONSOLE_DISCONNECT = 2;
        public const uint WTS_REMOTE_CONNECT = 3;
        public const uint WTS_REMOTE_DISCONNECT = 4;
        public const uint WTS_SESSION_LOCK = 7;
        public const uint WTS_SESSION_LOGOFF = 6;
        public const uint WTS_SESSION_LOGON = 5;
        public const uint WTS_SESSION_UNLOCK = 8;
        public const int XBUTTON1 = 1;
        public const int XBUTTON2 = 2;

        // Methods
        static Win32Api()
        {
            InitMessageDump();
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern uint AddFontResourceEx(string stFileName, uint flags, IntPtr lpReserved);
        [DllImport("user32.dll")]
        public static extern bool AdjustWindowRectEx([In, Out] ref RECT rect, uint nStyle, bool bMenu, uint nExStyle);
        [DllImport("msimg32.dll")]
        public static extern bool AlphaBlend(HDC hdcDest, int xDest, int yDest, int cxDest, int cyDest, HDC hdcSrc, int xSrc, int ySrc, int cxSrc, int cySrc, [In] BLENDFUNCTION bf);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool AppendMenu(HMENU hmenu, uint uFlags, UIntPtr uIDNewItem, string lpNewItem);
        public static COLORREF ARGB(byte a, byte r, byte g, byte b)
        {
            return new COLORREF((uint)((((a << 0x18) | (b << 0x10)) | (g << 8)) | r));
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool AttachThreadInput(uint dwThreadToAttach, uint dwThreadToAttachTo, bool fAttach);
        [DllImport("Kernel32.dll")]
        public static extern bool Beep(uint dwFreq, uint dwDuration);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern HANDLE BeginDeferWindowPos(int nNumWindows);
        [DllImport("user32.dll")]
        public static extern HDC BeginPaint(HWND hwnd, [In, Out] ref PAINTSTRUCT ps);
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(HDC hdcDest, int xDest, int yDest, int cxDest, int cyDest, HDC hdcSrc, int xSrc, int ySrc, uint nCmd);
        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CallWindowProc(IntPtr proc, HWND hwnd, uint uMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int ChangeDisplaySettingsEx(string deviceName, [In] ref DEVMODE devmode, IntPtr hwndReserved, uint dwFlags, IntPtr param);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int ChangeDisplaySettingsEx(string deviceName, [In] IntPtr devmode, IntPtr hwndReserved, uint dwFlags, IntPtr param);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool ChangeServiceConfig(SafeServiceHandle handle, uint serviceType, SERVICE_START_TYPE startType, uint errorControl, string binaryPathName, string loadOrderGroup, IntPtr tagId, string dependencies, string serviceStartName, string password, string displayName);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool ChangeServiceConfig2(SafeServiceHandle handle, uint dwInfoLevel, IntPtr pInfo);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool CheckMenuItem(HMENU hMenu, uint uIDEnableItem, uint uCheck);
        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(HWND hwnd, [In, Out] ref POINT pt);
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(HANDLE hObject);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CloseServiceHandle(SafeServiceHandle handle);
        [DllImport("gdi32.dll")]
        public static extern int CombineRgn(HGDIOBJ hrgnDest, HRGN hrgnSrc1, HRGN hrgnSrc2, int nCombineMode);
        [DllImport("Comdlg32.dll", CharSet = CharSet.Auto)]
        public static extern int CommDlgExtendedError();
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int CompareString(int Locale, int dwCmpFlags, StringBuilder lpString1, int cchCount1, StringBuilder lpString2, int cchCount2);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool ControlService(SafeServiceHandle hService, uint control, out SERVICE_STATUS serviceStatus);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CopyFile(string existingFile, string newFile, bool failIfExists);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CopyFileEx(string existingFile, string newFile, CopyProgressRoutine routine, IntPtr data, IntPtr zero, uint flags);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern HBITMAP CreateCompatibleBitmap(HDC hdc, int cx, int cy);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern HDC CreateCompatibleDC(HDC hdc);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CreateDirectory(string directory, IntPtr zero);
        [DllImport("kernel32.dll")]
        public static extern HANDLE CreateEvent(IntPtr lpSecurityAttributes, bool bManualReset, bool bInitialState, string lpName);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern HFONT CreateFontIndirect([In, MarshalAs(UnmanagedType.LPStruct)] LOGFONT plf);
        [DllImport("user32.dll")]
        public static extern HMENU CreateMenu();
        [DllImport("kernel32.dll")]
        public static extern HANDLE CreateMutex(IntPtr lpMutexAttributes, bool bInitialOwner, string lpName);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern HBRUSH CreatePatternBrush(HBITMAP hbmp);
        [DllImport("user32.dll")]
        public static extern HMENU CreatePopupMenu();
        [DllImport("gdi32.dll")]
        public static extern HRGN CreateRectRgn(int nLeft, int nTop, int nRight, int nBottom);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern HBRUSH CreateSolidBrush(COLORREF crColor);
        [DllImport("user32.dll", EntryPoint = "CreateWindowExW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern HWND CreateWindowEx(uint dwExStyle, IntPtr classAtom, string stTitle, uint dwStyle, int x, int y, int w, int h, HWND hwndParent, HMENU hmenu, HINSTANCE hinst, [MarshalAs(UnmanagedType.AsAny)] object pvParam);
        [DllImport("user32.dll", EntryPoint = "CreateWindowExW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern HWND CreateWindowEx(uint dwExStyle, string stClassName, string stTitle, uint dwStyle, int x, int y, int w, int h, HWND hwndParent, HMENU hmenu, HINSTANCE hinst, [MarshalAs(UnmanagedType.AsAny)] object pvParam);
        public static DateTime DateTimeFromSystemTime(SYSTEMTIME sysTime)
        {
            return new DateTime(sysTime.wYear, sysTime.wMonth, sysTime.wDay, sysTime.wHour, sysTime.wMinute, sysTime.wSecond, sysTime.wMilliseconds);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool DeferWindowPos(HANDLE hWndPosInfo, HWND hWnd, HWND hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll", EntryPoint = "DefWindowProcW", CharSet = CharSet.Unicode)]
        public static extern IntPtr DefWindowProc(HWND hwnd, uint uMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll")]
        public static extern void DeleteCriticalSection(CRITICAL_SECTION lpCriticalSection);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern bool DeleteDC(HDC hdc);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool DeleteFile(string file);
        [DllImport("user32.dll")]
        public static extern bool DeleteMenu(HMENU hmenu, uint uPosition, uint uFlags);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(HGDIOBJ hobj);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool DestroyIcon(HICON hIcon);
        [DllImport("user32.dll")]
        public static extern bool DestroyMenu(HMENU hmenu);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool DestroyWindow(HWND hwnd);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern unsafe bool DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, void* lpInBuffer, int nInBufferSize, void* lpOutBuffer, int nOutBufferSize, int* lpBytesReturned, IntPtr lpOverlapped);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr DispatchMessage(ref MSG msg);
        [DllImport("user32.dll")]
        public static extern bool DrawEdge(HDC hdc, [In, Out] ref RECT rc, uint nEdge, uint nFlags);
        [DllImport("user32.dll")]
        public static extern bool DrawFocusRect(HDC hdc, [In] ref RECT rc);
        [DllImport("user32.dll")]
        public static extern bool DrawFrameControl(HDC hdc, ref RECT rc, uint type, uint state);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool DrawIcon(HDC hdc, int x, int y, HICON hicon);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool DrawIconEx(HDC hdc, int x, int y, HICON hicon, int cx, int cy, uint istep, HGDIOBJ hbr, uint flags);
        public static bool DrawText(HDC hdc, string stText, ref RECT rcFill, uint nFormat)
        {
            return DrawText(hdc, stText, stText.Length, ref rcFill, nFormat);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool DrawText(HDC hdc, string stText, int cch, ref RECT rcFill, uint nFormat);
        [DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
        public static extern uint DsGetDcName(IntPtr computerName, IntPtr domainName, IntPtr domainGuid, IntPtr siteName, uint flags, out IntPtr domainControllerInfo);
        public static string DumpMessage(uint uMsg)
        {
            string str = DumpMessageWorker(uMsg);
            if (str == null)
            {
                str = "UNKNOWN MESSAGE";
            }
            return str;
        }

        private static string DumpMessageWorker(uint uMsg)
        {
            InitMessageDump();
            if (uMsg >= s_rgsMessageNames.Length)
            {
                return null;
            }
            string str = s_rgsMessageNames[uMsg];
            if (str != null)
            {
                return str;
            }
            uint index = uMsg;
            while (uMsg > 0)
            {
                index--;
                str = s_rgsMessageNames[index];
                if (str != null)
                {
                    break;
                }
            }
            if (str == null)
            {
                return null;
            }
            return InvariantString.Format("{0} + {1}", new object[] { str, uMsg - index });
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool DuplicateHandle(IntPtr hSourceProcess, IntPtr hSourceHandle, IntPtr hTargetProcess, ref IntPtr hTargetHandle, uint dwDesiredAccess, bool inheritable, uint dwOptions);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool EnableMenuItem(HMENU hMenu, uint uIDEnableItem, uint uEnable);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool EnableWindow(HWND hWnd, bool bEnable);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool EndDeferWindowPos(HANDLE hWndPosInfo);
        [DllImport("user32.dll")]
        public static extern bool EndPaint(HWND hwnd, [In, Out] ref PAINTSTRUCT ps);
        [DllImport("kernel32.dll")]
        public static extern void EnterCriticalSection(CRITICAL_SECTION lpCriticalSection);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool EnumDisplayDevices(string deviceName, uint iDevNum, [In, Out] ref DISPLAY_DEVICE dispDevice, uint dwFlags);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern bool EnumDisplaySettingsEx(string deviceName, uint iModeNum, [In, Out] ref DEVMODE devmode, uint dwFlags);
        [DllImport("kernel32.dll", EntryPoint = "EnumResourceNamesW", CharSet = CharSet.Unicode)]
        public static extern bool EnumResourceNames(IntPtr hinst, IntPtr nType, ENUMRESNAMEPROC callback, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool EnumSystemGeoID(GEOCLASS geoClass, int parentGeoId, GeoEnumProc callback);
        [DllImport("ntdll.dll")]
        internal static extern uint EtwNotificationRegister(ref Guid guid, uint type, EtwNotificationCallback callback, IntPtr context, out long regHandle);
        [DllImport("ntdll.dll")]
        internal static extern uint EtwNotificationUnregister(long regHandle, out IntPtr context);
        [DllImport("Shell32.dll", CharSet = CharSet.Auto)]
        public static extern HICON ExtractIcon(HINSTANCE hinst, string stExeFileName, int nIconIndex);
        [DllImport("Shell32.dll", CharSet = CharSet.Auto)]
        public static extern uint ExtractIconEx(string stExeFileName, int nIconIndex, ref HICON phiconLarge, ref HICON phiconSmall, uint nIcons);
        [DllImport("gdi32.dll")]
        public static extern int ExtSelectClipRgn(HDC hdc, HGDIOBJ hrgn, int nCombineMode);
        public static bool ExtTextOut(HDC hdc, int x, int y, uint fuOptions, [In] ref RECT rcFill, string strText)
        {
            return ExtTextOut(hdc, x, y, fuOptions, ref rcFill, strText, strText.Length, new IntPtr(0));
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool ExtTextOut(HDC hdc, int x, int y, uint fuOptions, [In] ref RECT rcFill, string stText, int cch, IntPtr pdx);
        [DllImport("user32.dll")]
        public static extern bool FillRect(HDC hdc, [In] ref RECT rcFill, HBRUSH hbrFill);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FindClose(HANDLE hFindFile);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern HANDLE FindFirstFile(string pFileName, [In, Out] WIN32_FIND_DATA pFindFileData);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool FindNextFile(HANDLE hndFindFile, [In, Out, MarshalAs(UnmanagedType.LPStruct)] WIN32_FIND_DATA lpFindFileData);
        [DllImport("kernel32.dll", EntryPoint = "FindResourceW", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindResource(IntPtr hinst, IntPtr resource, IntPtr type);
        [DllImport("kernel32.dll", EntryPoint = "FindResourceW", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindResource(IntPtr hinst, string resource, IntPtr type);
        [DllImport("user32.dll", EntryPoint = "FindWindowW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern HWND FindWindow(string strClassName, string strWindowName);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool FreeLibrary(HINSTANCE hinst);
        public static string GET_RESOURCE_NAME(IntPtr value)
        {
            if (IS_INTRESOURCE(value))
            {
                return value.ToString();
            }
            return Marshal.PtrToStringUni(value);
        }

        public static int GET_X_LPARAM(IntPtr lParam)
        {
            return (short)LOWORD(lParam);
        }

        public static int GET_Y_LPARAM(IntPtr lParam)
        {
            return (short)HIWORD(lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern HWND GetAncestor(HWND hwnd, int gaFlags);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern short GetAsyncKeyState(int nVirtKey);
        [DllImport("gdi32.dll")]
        public static extern bool GetBrushOrgEx(HDC hdc, out POINT pt);
        public static int GetBValue(COLORREF cr)
        {
            return (int)((cr.cr & 0xff0000) >> 0x10);
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool GetCharABCWidths(HDC hdc, uint chFirst, uint chLast, out ABC abc);
        [DllImport("user32.dll")]
        public static extern int GetClassLong(HWND hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "GetClassNameW", CharSet = CharSet.Unicode)]
        public static extern int GetClassName(HWND hWnd, StringBuilder text, int nMax);
        [DllImport("user32.dll")]
        public static extern bool GetClientRect(HWND hwnd, [In, Out] ref RECT rc);
        [DllImport("gdi32.dll")]
        public static extern int GetClipRgn(HDC hdc, HGDIOBJ hrgn);
        public static string GetComputerName()
        {
            StringBuilder text = new StringBuilder(0xff);
            int capacity = text.Capacity;
            GetComputerNameExW(1, text, ref capacity);
            return text.ToString();
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern bool GetComputerNameExW(int nNameType, StringBuilder text, ref int nMax);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern HANDLE GetCurrentProcess();
        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();
        [DllImport("user32.dll")]
        public static extern int GetCursorPos(out POINT pt);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern HDC GetDC(HWND hwnd);
        [DllImport("user32.dll")]
        public static extern HDC GetDCEx(HWND hwnd, HGDIOBJ hrgnClip, uint nFlags);
        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(HDC hDC, int nIndex);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailableToCaller, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);
        [DllImport("kernel32.dll")]
        public static extern uint GetDriveType(string s);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern uint GetFileAttributes(string name);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetFileAttributesEx(string name, int fileInfoLevel, ref WIN32_FILE_ATTRIBUTE_DATA lpFileInformation);
        [DllImport("user32.dll")]
        public static extern HWND GetFocus();
        [DllImport("user32.dll")]
        public static extern HWND GetForegroundWindow();
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetGeoInfo(int geoId, GEOINFO geoType, StringBuilder sb, uint cch, ushort langid);
        public static int GetGValue(COLORREF cr)
        {
            return (int)((cr.cr & 0xff00) >> 8);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetIconInfo(HICON hIcon, out ICONINFO iconinfo);
        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetKeyboardLayout(int dwLayout);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetKeyboardState(byte[] keystate);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern short GetKeyState(int nVirtKey);
        [Obsolete("Marshal.GetLastWin32Error with matching PInvoke DllImport SetLastError=true must be used instead of GetLastError")]
        public static uint GetLastError()
        {
            return 0;
        }

        [DllImport("user32.dll")]
        public static extern HMENU GetMenu(HWND hwnd);
        [DllImport("user32.dll")]
        public static extern bool GetMenuInfo(HMENU hmenu, [In, Out] ref MENUINFO mi);
        [DllImport("user32.dll")]
        public static extern int GetMenuItemCount(HMENU hmenu);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetMessage(ref MSG msg, HWND hwnd, uint nMsgFilterMin, uint nMsgFilterMax);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetModuleFileName(IntPtr hModule, StringBuilder buffer, int length);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern HINSTANCE GetModuleHandle(string stModuleName);
        [DllImport("user32.dll")]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, [In, Out] ref MONITORINFOEX miex);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern int GetObject(HBITMAP hbmp, int cb, ref BITMAP bm);
        [DllImport("Comdlg32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out] ref OPENFILENAME ofn);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern HWND GetParent(HWND hWndChild);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern COLORREF GetPixel(HDC hdc, int x, int y);
        [DllImport("Kernel32.dll")]
        public static extern int GetPrivateProfileString(string strSectionName, string strKeyName, string strDefaultValue, StringBuilder strReturnValue, uint cbSize, string strFileName);
        [DllImport("user32.dll")]
        public static extern bool GetProcessDefaultLayout(out int pdwDefaultLayout);
        public static int GetRValue(COLORREF cr)
        {
            return (((int)cr.cr) & 0xff);
        }

        [DllImport("Comdlg32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetSaveFileName([In, Out] ref OPENFILENAME ofn);
        [DllImport("user32.dll")]
        public static extern bool GetScrollInfo(HWND hwnd, int fnBar, [In, Out] ref SCROLLINFO lpsi);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern void GetStartupInfo([In, Out] STARTUPINFO info);
        [DllImport("gdi32.dll")]
        public static extern HGDIOBJ GetStockObject(int nObject);
        [DllImport("user32.dll")]
        public static extern HMENU GetSubMenu(HMENU hmenu, int nPos);
        [DllImport("user32.dll")]
        public static extern COLORREF GetSysColor(int idxColor);
        public static string GetSystemDirectory()
        {
            StringBuilder sbDir = new StringBuilder(260);
            while (true)
            {
                uint systemDirectory = GetSystemDirectory(sbDir, (uint)sbDir.Capacity);
                if (systemDirectory <= sbDir.Capacity)
                {
                    if (systemDirectory == 0)
                    {
                        return null;
                    }
                    return sbDir.ToString();
                }
                sbDir.EnsureCapacity((int)systemDirectory);
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern uint GetSystemDirectory(StringBuilder sbDir, uint cbSize);
        [DllImport("kernel32.dll")]
        public static extern void GetSystemInfo(out SYSTEM_INFO info);
        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int metric);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern uint GetTempFileName(string path, string prefix, uint unique, StringBuilder buffer);
        public static string GetTemporaryFile(string prefix)
        {
            StringBuilder sb = new StringBuilder(260);
            if (GetTempPath(sb.Capacity, sb) > 0)
            {
                StringBuilder buffer = new StringBuilder(260);
                if (GetTempFileName(sb.ToString(), prefix, 0, buffer) != 0)
                {
                    return buffer.ToString();
                }
            }
            return null;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern uint GetTempPath(int length, StringBuilder sb);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern uint GetTextAlign(HDC hdc);
        public static bool GetTextExtentPoint32(HDC hdc, string stText, out SIZE size)
        {
            return GetTextExtentPoint32(hdc, stText, stText.Length, out size);
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetTextExtentPoint32(HDC hdc, string stText, int cch, out SIZE size);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetTextMetrics(HDC hdc, out TEXTMETRIC lptm);
        [DllImport("user32.dll")]
        public static extern bool GetUpdateRect(HWND hwnd, [In, Out] ref RECT rc, bool fErase);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetUserGeoID(GEOCLASS geoClass);
        [DllImport("kernel32.dll", EntryPoint = "GetVolumeInformationW", CharSet = CharSet.Unicode)]
        public static extern bool GetVolumeInformation(string s, StringBuilder sb, int cch, IntPtr serial, IntPtr len, IntPtr flag, IntPtr fsname, int fsnamelen);
        [DllImport("user32.dll")]
        public static extern HWND GetWindow(HWND hWnd, GetWindow_Cmd uCmd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowLong(HWND hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", CharSet = CharSet.Auto)]
        public static extern WNDPROC GetWindowLongWNDPROC(HWND hWnd, int nIndex);
        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(HWND hwnd, [In, Out] WINDOWPLACEMENT wndpl);
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(HWND hwnd, [In, Out] ref RECT rc);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern uint GetWindowsDirectory(StringBuilder sbDir, uint cbSize);
        [DllImport("user32.dll", EntryPoint = "GetWindowTextW", CharSet = CharSet.Unicode)]
        public static extern int GetWindowText(HWND hWnd, StringBuilder text, int nMax);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int GetWindowTextLength(HWND hwnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint GetWindowThreadProcessId(HWND hWnd, out uint dwProcessId);
        [DllImport("gdi32.dll")]
        public static extern bool GetWorldTransform(HDC hdc, out XFORM xf);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern ATOM GlobalAddAtom(string atomName);
        [DllImport("kernel32.dll")]
        public static extern ATOM GlobalDeleteAtom(ATOM a);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern uint GlobalGetAtomName(ATOM atom, StringBuilder strAtomName, int cbSize);
        [DllImport("kernel32.dll")]
        public static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);
        public static short HISHORT(int i)
        {
            return (short)((i >> 0x10) & 0xffff);
        }

        public static short HISHORT(IntPtr i)
        {
            return HISHORT((int)i);
        }

        public static int HIWORD(IntPtr lParam)
        {
            return (((int)lParam) >> 0x10);
        }

        [DllImport("Hhctrl.ocx")]
        public static extern int HtmlHelp(IntPtr hwndCaller, string pszFile, int uCommand, HH_AKLINK dwData);
        [DllImport("Hhctrl.ocx")]
        public static extern int HtmlHelp(IntPtr hwndCaller, string pszFile, int uCommand, HH_FTS_QUERY dwData);
        [DllImport("Hhctrl.ocx")]
        public static extern int HtmlHelp(IntPtr hwndCaller, string pszFile, int uCommand, HH_POPUP dwData);
        [DllImport("Hhctrl.ocx")]
        public static extern int HtmlHelp(IntPtr hwndCaller, string pszFile, int uCommand, int dwData);
        [DllImport("Hhctrl.ocx")]
        public static extern int HtmlHelp(IntPtr hwndCaller, string pszFile, int uCommand, string dwData);
        [DllImport("kernel32.dll")]
        public static extern void InitializeCriticalSection(CRITICAL_SECTION lpCriticalSection);
        private static void InitMessageDump()
        {
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool InsertMenu(HMENU hmenu, uint uPosition, uint uFlags, UIntPtr uIDNewItem, string lpNewItem);
        [DllImport("gdi32.dll")]
        public static extern int IntersectClipRect(HDC hdc, int nLeft, int nTop, int nRight, int nBottom);
        [DllImport("user32.dll")]
        public static extern bool InvalidateRect(HWND hwnd, ref RECT rc, bool fErase);
        [DllImport("user32.dll")]
        public static extern bool InvalidateRect(HWND hwnd, IntPtr p, bool fErase);
        public static bool IS_INTRESOURCE(IntPtr value)
        {
            if (((int)value) > 0xffff)
            {
                return false;
            }
            return true;
        }

        [DllImport("user32.dll")]
        public static extern bool IsChild(HWND hWndParent, HWND hwnd);
        [DllImport("sensapi.dll")]
        public static extern bool IsNetworkAlive(ref int flags);
        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern bool IsUserAnAdmin();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool IsWindow(HWND hwnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool IsWindowEnabled(HWND hwnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool IsWindowVisible(HWND hwnd);
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte code, byte scan, int flags, IntPtr dwExtraInfo);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool KillTimer(HWND hwnd, UIntPtr idTimer);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern int LCMapString(int Locale, uint dwMapFlags, StringBuilder lpSrcStr, int cchSrc, StringBuilder lpDestStr, int cchDest);
        [DllImport("kernel32.dll")]
        public static extern void LeaveCriticalSection(CRITICAL_SECTION lpCriticalSection);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern HCURSOR LoadCursor(HINSTANCE hinstance, IntPtr nCursorName);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern HCURSOR LoadCursor(HINSTANCE hinstance, string stCursorName);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr LoadImage(HINSTANCE hinst, IntPtr id, uint nType, int cxDesired, int cyDesired, uint nFlags);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr LoadImage(HINSTANCE hinst, string stName, uint nType, int cxDesired, int cyDesired, uint nFlags);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern HINSTANCE LoadLibrary(string stModuleName);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern HINSTANCE LoadLibraryEx(string stModuleName, HANDLE hFile, uint dwFlags);
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadResource(IntPtr hinst, IntPtr i);
        public static string LoadString(HINSTANCE hInstance, uint uID)
        {
            return LoadString(hInstance, uID, 0x100000);
        }

        public static string LoadString(HINSTANCE hInstance, uint uID, int maxLength)
        {
            StringBuilder lpBuffer = new StringBuilder(0x100);
            int num = 0;
            while (((num = LoadString(hInstance, uID, lpBuffer, lpBuffer.Capacity)) >= (lpBuffer.Capacity - 1)) && (lpBuffer.Capacity <= maxLength))
            {
                lpBuffer.Capacity *= 2;
            }
            if (num == 0)
            {
                return null;
            }
            return lpBuffer.ToString();
        }

        [DllImport("user32.dll", EntryPoint = "LoadStringW", CharSet = CharSet.Unicode)]
        public static extern int LoadString(HINSTANCE hInstance, uint uID, [Out] StringBuilder lpBuffer, int nBufferMax);
        [DllImport("user32.dll", EntryPoint = "LoadStringW", CharSet = CharSet.Unicode)]
        public static extern unsafe int LoadStringFast(HINSTANCE hInstance, uint uID, IntPtr* lppBuffer, int nBufferMax);
        [DllImport("kernel32.dll")]
        public static extern IntPtr LockResource(IntPtr i);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr LockServiceDatabase(SafeServiceHandle hSCManager);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool LockSetForegroundWindow(uint uLockCode);
        [DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(HWND hwnd);
        public static short LOSHORT(int i)
        {
            return (short)(i & 0xffff);
        }

        public static short LOSHORT(IntPtr i)
        {
            return LOSHORT((int)i);
        }

        public static int LOWORD(IntPtr lParam)
        {
            return (((int)lParam) & 0xffff);
        }

        public static int MAKELPARAM(int wLow, int wHigh)
        {
            return ((wLow & 0xffff) | (wHigh << 0x10));
        }

        [DllImport("user32.dll")]
        public static extern bool MapWindowPoints(HWND hwnd, HWND hwndParent, [In, Out] ref RECT rc, uint nCount);
        [DllImport("user32.dll")]
        public static extern bool MessageBeep(uint type);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MessageBox(HWND hwnd, string wzText, string wzTitle, uint mb);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MessageBoxEx(HWND hWnd, string text, string caption, uint uType, short wLanguageId);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool ModifyMenu(HMENU hmenu, uint uPosition, uint uFlags, UIntPtr uIDNewItem, string lpNewItem);
        [DllImport("gdi32.dll")]
        public static extern bool ModifyWorldTransform(HDC hdc, ref XFORM xf, uint nMode);
        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromRect([In] ref RECT rc, uint flags);
        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(HWND hWnd, uint flags);
        [DllImport("user32.dll")]
        public static extern void mouse_event(int flags, int dx, int dy, int dwData, IntPtr dwExtraInfo);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool MoveFileEx(string from, string to, uint flags);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool MoveWindow(HWND hwnd, int x, int y, int width, int height, bool bRepaint);
        [DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
        public static extern uint NetApiBufferFree(IntPtr buffer);
        [DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
        public static extern uint NetGetJoinInformation(IntPtr server, out IntPtr nameBuffer, out NETSETUP_JOIN_STATUS joinStatus);
        [DllImport("gdi32.dll")]
        public static extern int OffsetRgn(HGDIOBJ hrgn, int xOffset, int yOffset);
        [DllImport("kernel32.dll")]
        public static extern HANDLE OpenEvent(uint nDesiredAccess, bool bInheritTable, string lpName);
        [DllImport("kernel32.dll")]
        public static extern HANDLE OpenMutex(int dwDesiredAccess, bool bInheritHandle, string lpName);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern SafeServiceHandle OpenSCManager(string machineName, string databaseName, SC_MANAGER_ACCESS_TYPE desiredAccess);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern SafeServiceHandle OpenService(SafeServiceHandle hSCManager, string serviceName, SERVICE_ACCESS_TYPE desiredAccess);
        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int PathCommonPrefix(string strPath1, string strPath2, StringBuilder sbResult);
        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern bool PathCompactPathExW(StringBuilder sb, string strFilename, int uiMaxChars, uint dwReserved);
        [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
        public static extern int PathGetDriveNumber([In] string pszPath);
        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern bool PathIsNetworkPath(string strPath);
        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern bool PathIsRoot(string strPath);
        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern bool PathIsSameRoot(string strPath1, string strPath2);
        [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
        public static extern int PathParseIconLocation([In, Out] string pszIconFile);
        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern bool PathRemoveBackslash(StringBuilder sb);
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern bool PlaySound(IntPtr pData, IntPtr hMod, uint fdwSound);
        [DllImport("winmm.dll", SetLastError = true)]
        public static extern bool PlaySound(string pszSound, IntPtr hMod, uint fdwSound);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool PostMessage(HWND hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern void PostQuitMessage(int nExitCode);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool PostThreadMessage(uint dwThreadId, uint uMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("ole32.dll")]
        public static extern HRESULT PropVariantClear(ref PROPVARIANT p);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool QueryPerformanceCounter(ref long pPerformanceCounter);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool QueryPerformanceFrequency(ref long pPerformanceFrequency);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool QueryServiceConfig(SafeServiceHandle handle, IntPtr pBuffer, uint bufferSize, out uint bytesNeeded);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool QueryServiceLockStatus(SafeServiceHandle hSCManager, IntPtr lpLockStatus, uint cbBufSize, out uint pcbBytesNeeded);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool QueryServiceStatus(SafeServiceHandle handle, out SERVICE_STATUS serviceStatus);
        [DllImport("gdi32.dll")]
        public static extern bool Rectangle(HDC hdc, int nLeft, int nTop, int nRight, int nBottom);
        [DllImport("user32.dll")]
        public static extern bool RedrawWindow(HWND hwnd, ref RECT rc, HGDIOBJ hrgn, uint nFlags);
        [DllImport("user32.dll")]
        public static extern bool RedrawWindow(HWND hwnd, IntPtr p, HGDIOBJ hrgn, uint nFlags);
        [DllImport("advapi32.dll")]
        public static extern int RegCloseKey(HKEY hkey);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern uint RegisterApplicationRestart(string strCommandLine, uint restartFlags);
        [DllImport("user32.dll", EntryPoint = "RegisterClassExW", CharSet = CharSet.Unicode)]
        public static extern ATOM RegisterClassEx(WNDCLASSEX wc);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr RegisterDeviceNotification(HWND hRecipient, DEV_BROADCAST_HDR NotificationFilter, uint Flags);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern HPOWERNOTIFY RegisterPowerSettingNotification(IntPtr hRecipient, ref Guid PowerSettingGuid, uint Flags);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int RegisterWaitForSingleObject(out IntPtr phNewWaitObject, IntPtr hObject, WaitOrTimerCallBackDelegate Callback, IntPtr Context, uint dwMilliseconds, uint dwFlags);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint RegisterWindowMessage(string name);
        [DllImport("advapi32.dll")]
        public static extern int RegNotifyChangeKeyValue(HKEY hkey, bool bWatchSubtree, int dwNotifyFilter, HANDLE hEvent, bool fAsynchronous);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
        public static extern int RegOpenKeyExW(HKEY hkey, string szSubKey, int dwOptions, int samDesired, ref HKEY phkResult);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(HWND hwnd, HDC hdc);
        [DllImport("kernel32.dll")]
        public static extern bool ReleaseMutex(HANDLE hMutex);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool RemoveDirectory(string directory);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern uint RemoveFontResourceEx(string stFileName, uint flags, IntPtr lpReserved);
        [DllImport("user32.dll")]
        public static extern bool RemoveMenu(HMENU hmenu, uint uPosition, uint uFlags);
        [DllImport("kernel32.dll")]
        public static extern bool ResetEvent(HANDLE hEvent);
        public static COLORREF RGB(byte r, byte g, byte b)
        {
            return new COLORREF((uint)(((b << 0x10) | (g << 8)) | r));
        }

        public static COLORREF RGB(int r, int g, int b)
        {
            return RGB((byte)r, (byte)g, (byte)b);
        }

        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(HWND hwnd, [In, Out] ref POINT pt);
        [DllImport("gdi32.dll")]
        public static extern HGDIOBJ SelectObject(HDC hdc, HGDIOBJ hobj);
        [DllImport("user32.dll")]
        public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessage(HWND hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr SendMessageTimeout(HWND hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, uint flags, uint timeout, out uint result);
        [DllImport("gdi32.dll")]
        public static extern COLORREF SetBkColor(HDC hdc, COLORREF crBack);
        [DllImport("gdi32.dll")]
        public static extern int SetBkMode(HDC hdc, int iBkMode);
        [DllImport("gdi32.dll")]
        public static extern bool SetBrushOrgEx(HDC hdc, int xOrg, int yOrg, out POINT pt);
        [DllImport("user32.dll")]
        public static extern HWND SetCapture(HWND hWnd);
        [DllImport("user32.dll")]
        public static extern int SetClassLong(HWND hWnd, int nIndex, int dwNewLong);
        [DllImport("kernel32.dll")]
        public static extern uint SetErrorMode(uint uMode);
        [DllImport("kernel32.dll")]
        public static extern bool SetEvent(HANDLE hEvent);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern uint SetFileAttributes(string name, uint val);
        [DllImport("user32.dll")]
        public static extern HWND SetFocus(HWND hWnd);
        [DllImport("gdi32.dll")]
        public static extern int SetGraphicsMode(HDC hdc, int nMode);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SetKeyboardState(byte[] keystate);
        [DllImport("user32.dll")]
        public static extern bool SetMenu(HWND hwnd, HMENU hmenu);
        [DllImport("user32.dll")]
        public static extern bool SetMenuInfo(HMENU hmenu, [In] ref MENUINFO mi);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern HWND SetParent(HWND hWndChild, HWND hWndParent);
        [DllImport("user32.dll")]
        public static extern bool SetProcessDefaultLayout(int pdwDefaultLayout);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetProcessShutdownParameters(uint dwLevel, uint dwFlags);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetProcessWorkingSetSize(HANDLE hProcess, IntPtr min, IntPtr max);
        [DllImport("user32.dll")]
        public static extern bool SetScrollInfo(HWND hwnd, int fnBar, [In] ref SCROLLINFO lpsi, bool fRedraw);
        [DllImport("gdi32.dll")]
        public static extern int SetStretchBltMode(HDC hdc, int nMode);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern uint SetTextAlign(HDC hdc, uint nMode);
        [DllImport("gdi32.dll")]
        public static extern int SetTextCharacterExtra(HDC hdc, int nCharExtra);
        [DllImport("gdi32.dll")]
        public static extern COLORREF SetTextColor(HDC hdc, COLORREF crText);
        [DllImport("kernel32.dll")]
        public static extern uint SetThreadExecutionState(uint esFlags);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern UIntPtr SetTimer(HWND hwnd, UIntPtr idTimer, uint uElapse, TIMERPROC proc);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern UIntPtr SetTimer(HWND hwnd, UIntPtr idTimer, uint uElapse, IntPtr timerproc);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern uint SetWindowLong(HWND hWnd, int nIndex, uint dwNewLong);
        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Unicode)]
        public static extern IntPtr SetWindowLong32(HWND hWnd, int nIndex, IntPtr newLong);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Unicode)]
        public static extern IntPtr SetWindowLong64(HWND hWnd, int nIndex, IntPtr newLong);
        public static IntPtr SetWindowLongPtr(HWND hWnd, int nIndex, WNDPROC wndProc)
        {
            return SetWindowLongPtr(hWnd, nIndex, Marshal.GetFunctionPointerForDelegate(wndProc));
        }

        public static IntPtr SetWindowLongPtr(HWND hWnd, int nIndex, IntPtr newLong)
        {
            if (IntPtr.Size == 4)
            {
                return SetWindowLong32(hWnd, nIndex, newLong);
            }
            return SetWindowLong64(hWnd, nIndex, newLong);
        }

        [DllImport("user32.dll")]
        public static extern bool SetWindowPlacement(HWND hWnd, WINDOWPLACEMENT wndpl);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetWindowPos(HWND hWnd, HWND hwndZAfter, int x, int y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")]
        public static extern bool SetWindowRgn(HWND hwnd, HGDIOBJ hrgn, bool bRedraw);
        [DllImport("user32.dll")]
        public static extern bool SetWindowRgn(HWND hwnd, HRGN hrgn, bool bRedraw);
        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(HookType hook, HookProc callback, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", EntryPoint = "SetWindowTextW", CharSet = CharSet.Unicode)]
        public static extern bool SetWindowText(HWND hWnd, string text);
        [DllImport("gdi32.dll")]
        public static extern bool SetWorldTransform(HDC hdc, ref XFORM xf);
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        public static extern void SHChangeNotify(uint ev, uint flags, string path, string other);
        [DllImport("Shell32.dll")]
        public static extern IntPtr ShellExecute(HWND hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);
        [DllImport("shell32.dll", SetLastError = true)]
        public static extern int SHGetFolderPath(IntPtr handle, int folder, IntPtr token, int flags, StringBuilder path);
        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SHGetFolderPathEx(ref Guid rfid, int dwFlags, IntPtr token, StringBuilder Path, int cchPath);
        [DllImport("user32.dll")]
        public static extern int ShowCursor(bool fShow);
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(HWND hwnd, int nCmdShow);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool ShutdownBlockReasonCreate(HWND hWnd, string pwszReason);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool ShutdownBlockReasonDestroy(HWND hWnd);
        [DllImport("kernel32.dll")]
        public static extern int SizeofResource(IntPtr hinst, IntPtr i);
        [DllImport("Kernel32.dll")]
        public static extern void Sleep(int ms);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool StartService(SafeServiceHandle hService, uint numArgs, string[] args);
        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int StrCmpLogicalW(string st1, string st2);
        [DllImport("gdi32.dll")]
        public static extern bool StretchBlt(HDC hdcDest, int xDest, int yDest, int cxDest, int cyDest, HDC hdcSrc, int xSrc, int ySrc, int cxSrc, int cySrc, uint nCmd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool SystemParametersInfo(int nAction, int nParam, ref HIGHCONTRAST_I rc, int nUpdate);
        [DllImport("user32.dll")]
        public static extern bool SystemParametersInfo(int uiAction, int uiParam, [In, Out] ref IntPtr pParam, int nWinIni);
        public static SYSTEMTIME SystemTimeFromDateTime(DateTime dt)
        {
            return new SYSTEMTIME { wYear = Convert.ToInt16(dt.Year), wMonth = Convert.ToInt16(dt.Month), wDayOfWeek = (short)dt.DayOfWeek, wDay = Convert.ToInt16(dt.Day), wHour = Convert.ToInt16(dt.Hour), wMinute = Convert.ToInt16(dt.Minute), wSecond = Convert.ToInt16(dt.Second), wMilliseconds = Convert.ToInt16(dt.Millisecond) };
        }

        [DllImport("Kernel32.dll")]
        public static extern bool TerminateProcess(HANDLE hProcess, uint uExitCode);
        public static bool TextOut(HDC hdc, int x, int y, string stText)
        {
            return TextOut(hdc, x, y, stText, stText.Length);
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern bool TextOut(HDC hdc, int x, int y, string stText, int cch);
        [DllImport("kernel32.dll")]
        public static extern uint TlsAlloc();
        [DllImport("kernel32.dll")]
        public static extern bool TlsFree(uint idxSlot);
        [DllImport("kernel32.dll")]
        public static extern UIntPtr TlsGetValue(uint idxSlot);
        [DllImport("kernel32.dll")]
        public static extern bool TlsSetValue(uint idxSlot, [In, Out] UIntPtr nNewValue);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int ToUnicode(int nVirtKey, int nScanCode, byte[] keystate, StringBuilder text, int cch, int flags);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool TrackMouseEvent(TRACKMOUSEEVENT tme);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool TranslateMessage(ref MSG msg);
        [DllImport("msimg32.dll")]
        public static extern bool TransparentBlt(HDC hdcDest, int xDest, int yDest, int cxDest, int cyDest, HDC hdcSrc, int xSrc, int ySrc, int cxSrc, int cySrc, COLORREF crTransparent);
        [DllImport("kernel32.dll")]
        public static extern int TryEnterCriticalSection(CRITICAL_SECTION lpCriticalSection);
        [DllImport("user32.dll")]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool UnlockServiceDatabase(IntPtr ScLoc);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool UnregisterClass(string hlpClass, HINSTANCE hInstance);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern HPOWERNOTIFY UnregisterPowerSettingNotification(HPOWERNOTIFY Handle);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool UnregisterWait(IntPtr hWaitHandle);
        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(HWND hwnd);
        [DllImport("user32.dll")]
        public static extern bool ValidateRect(HWND hwnd, IntPtr p, bool fErase);
        [DllImport("user32.dll")]
        public static extern bool ValidateRect(HWND hwnd, ref RECT rc, bool fErase);
        [DllImport("w32time.dll", CharSet = CharSet.Unicode)]
        public static extern uint W32TimeSetConfig(uint property, uint type, byte[] config, uint size);
        [DllImport("kernel32.dll")]
        public static extern uint WaitForSingleObject(IntPtr handle, uint dwMilliseconds);
        [DllImport("mpr.dll", EntryPoint = "WNetGetConnectionW", CharSet = CharSet.Unicode)]
        public static extern uint WNetGetConnection(string strDrive, StringBuilder sb, ref uint cch);
        [DllImport("wtsapi32.dll")]
        public static extern bool WTSRegisterSessionNotification(HWND hWnd, uint uiFlags);
        [DllImport("wtsapi32.dll")]
        public static extern bool WTSUnRegisterSessionNotification(HWND hWnd);

        // Properties
        public static string WindowsDirectory
        {
            get
            {
                StringBuilder sbDir = new StringBuilder(260);
                while (true)
                {
                    uint windowsDirectory = GetWindowsDirectory(sbDir, (uint)sbDir.Capacity);
                    if (windowsDirectory <= sbDir.Capacity)
                    {
                        if (windowsDirectory == 0)
                        {
                            return null;
                        }
                        return sbDir.ToString();
                    }
                    sbDir.EnsureCapacity((int)windowsDirectory);
                }
            }
        }

        // Nested Types
        [StructLayout(LayoutKind.Sequential)]
        public struct ABC
        {
            public int abcA;
            public uint abcB;
            public int abcC;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ATOM
        {
            public ushort h;
            public static explicit operator Win32Api.ATOM(ushort h)
            {
                return ToAtom(h);
            }

            public static Win32Api.ATOM ToAtom(ushort h)
            {
                return new Win32Api.ATOM { h = h };
            }

            public IntPtr GetIntPtr()
            {
                return (IntPtr)this.h;
            }

            public static Win32Api.ATOM NULL
            {
                get
                {
                    return new Win32Api.ATOM { h = 0 };
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAP
        {
            public int bmType;
            public int bmWidth;
            public int bmHeight;
            public int bmWidthBytes;
            public ushort bmPlanes;
            public ushort bmBitsPixel;
            public int bmBits;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CAUB
        {
            public uint c;
            public IntPtr array;
        }

        public enum CBTHookResult
        {
            AllowOperation,
            BlockOperation
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COLORREF
        {
            public uint cr;
            public COLORREF(uint cr)
            {
                this.cr = cr;
            }

            public COLORREF(Win32Api.COLORREF cr)
            {
                this.cr = cr.cr;
            }
        }

        private enum ComputerName
        {
            NetBIOS,
            DnsHostname,
            DnsDomain,
            DnsFullyQualified,
            PhysicalNetBIOS,
            PhysicalDnsHostname,
            PhysicalDnsDomain,
            PhysicalDnsFullyQualified,
            Max
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
            public static Win32Api.COPYDATASTRUCT Empty
            {
                get
                {
                    return new Win32Api.COPYDATASTRUCT { dwData = IntPtr.Zero, cbData = 0, lpData = IntPtr.Zero };
                }
            }
            public bool IsEmpty()
            {
                return (((IntPtr.Zero == this.dwData) && (this.cbData == 0)) && (IntPtr.Zero == this.lpData));
            }

            public unsafe byte[] GetData()
            {
                if ((this.cbData <= 0) || (IntPtr.Zero == this.lpData))
                {
                    return null;
                }
                byte[] buffer = null;
                try
                {
                    byte* lpData = (byte*)this.lpData;
                    buffer = new byte[this.cbData];
                    for (int i = 0; i < this.cbData; i++)
                    {
                        buffer[i] = lpData[i];
                    }
                }
                catch (OutOfMemoryException)
                {
                    buffer = null;
                }
                return buffer;
            }
        }

        public delegate uint CopyProgressRoutine(long TotalSize, long TotalBytes, long streamsize, long streambytes, uint stream, uint reason, IntPtr srcHandle, IntPtr destHandle, IntPtr data);

        [StructLayout(LayoutKind.Sequential)]
        public class CRITICAL_SECTION
        {
            public IntPtr DebugInfo;
            public long LockCount;
            public long RecursionCount;
            public int OwningThread;
            public int LockSemaphore;
            public IntPtr SpinCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_DEVICEINTERFACE
        {
            public uint dbcc_size;
            public uint dbcc_devicetype;
            public uint dbcc_reserved;
            public Guid dbcc_classguid;
            public byte dbcc_name;
        }

        [StructLayout(LayoutKind.Sequential), ComVisible(false)]
        public class DEV_BROADCAST_HDR
        {
            public uint dbcv_size;
            public uint dbcv_devicetype;
            public uint dbcv_reserved;
        }

        [StructLayout(LayoutKind.Sequential), ComVisible(false)]
        public class DEV_BROADCAST_VOLUME : Win32Api.DEV_BROADCAST_HDR
        {
            public uint dbcv_unitmask;
            public ushort dbcv_flags;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4)]
        public struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public ushort dmSpecVersion;
            public ushort dmDriverVersion;
            public ushort dmSize;
            public ushort dmDriverExtra;
            public uint dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public uint dmDisplayOrientation;
            public uint dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public ushort dmLogPixels;
            public uint dmBitsPerPel;
            public uint dmPelsWidth;
            public uint dmPelsHeight;
            public uint dmDisplayFlags;
            public uint dmDisplayFrequency;
            public uint dmICMMethod;
            public uint dmICMIntent;
            public uint dmMediaType;
            public uint dmDitherType;
            public uint dmReserved1;
            public uint dmReserved2;
            public uint dmPanningWidth;
            public uint dmPanningHeight;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 8)]
        public struct DISPLAY_DEVICE
        {
            public uint cb;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string DeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x80)]
            public string DeviceString;
            public uint StateFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x80)]
            public string DeviceID;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x80)]
            public string DeviceKey;
        }

        public delegate bool EnumChildrenCallback(IntPtr hwnd, IntPtr lParam);

        public delegate bool ENUMRESNAMEPROC(IntPtr hinst, IntPtr lpszType, IntPtr lpszName, IntPtr lParam);

        internal delegate uint EtwNotificationCallback(ref Win32Api.EtwNotificationHeader NotificationHeader, IntPtr Context);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct EtwNotificationHeader
        {
            public Win32Api.EtwNotificationType NotificationType;
            public uint NotificationSize;
            public uint Offset;
            public bool ReplyRequested;
            public uint Reserved1;
            public uint ReplyOrNotifeeCount;
            public ulong Reserved2;
            public uint TargetPID;
            public uint SourcePID;
            public Guid DestinationGuid;
            public Guid SourceGuid;
        }

        internal enum EtwNotificationType : uint
        {
            EtwNotificationTypeAudio = 6,
            EtwNotificationTypeCredentialUI = 9,
            EtwNotificationTypeEnable = 3,
            EtwNotificationTypeLegacyEnable = 2,
            EtwNotificationTypeMax = 10,
            EtwNotificationTypeNoReply = 1,
            EtwNotificationTypePerflib = 5,
            EtwNotificationTypePrivateLogger = 4,
            EtwNotificationTypeReserved = 8,
            EtwNotificationTypeSession = 7
        }

        public abstract class FileDialog
        {
            // Fields
            private string filename = string.Empty;
            private string filter = string.Empty;
            private const int MaxBufferSize = 0x2000;
            protected Win32Api.OPENFILENAME ofn = new Win32Api.OPENFILENAME();
            private bool unicode;

            // Methods
            public FileDialog()
            {
                this.ofn.lStructSize = Marshal.SizeOf(this.ofn);
                this.ofn.hInstance = Win32Api.GetModuleHandle(null);
                this.ofn.lpstrCustomFilter = IntPtr.Zero;
                this.ofn.nMaxCustFilter = 0;
                this.ofn.nFilterIndex = 0;
                this.ofn.nMaxFile = 0x2000;
                this.ofn.lpstrFileTitle = IntPtr.Zero;
                this.ofn.nMaxFileTitle = 0;
                this.ofn.lpstrDefExt = null;
                this.ofn.lCustData = IntPtr.Zero;
                this.ofn.lpfnHook = null;
                this.ofn.lpTemplateName = null;
                this.ofn.pvReserved = IntPtr.Zero;
                this.unicode = Marshal.SystemDefaultCharSize != 1;
                this.ofn.lpstrFile = Marshal.AllocCoTaskMem(this.unicode ? 0x4000 : 0x2000);
                this.ofn.hwndOwner = Win32Api.HWND.NULL;
                this.ofn.lpstrFilter = null;
                this.ofn.lpstrInitialDir = null;
                this.ofn.lpstrTitle = null;
                this.ofn.Flags = 0;
                this.ofn.FlagsEx = 0;
            }

            public bool DoModal()
            {
                this.LocalToStruct();
                bool flag = this.RunDialog();
                if (!flag && ((Win32Api.CommDlgExtendedError() & 0x3002) != 0))
                {
                    this.FileName = string.Empty;
                    this.LocalToStruct();
                    flag = this.RunDialog();
                }
                if (flag)
                {
                    this.StructToLocal();
                }
                return flag;
            }

            private void LocalToStruct()
            {
                if ((this.Filter == null) || (this.Filter == string.Empty))
                {
                    this.ofn.lpstrFilter = null;
                }
                else
                {
                    this.ofn.lpstrFilter = this.Filter.Replace('|', '\0');
                }
                if (this.unicode)
                {
                    char[] destination = new char[0x2000];
                    int count = Math.Min(this.filename.Length, destination.Length - 1);
                    this.filename.CopyTo(0, destination, 0, count);
                    destination[count] = '\0';
                    Marshal.Copy(destination, 0, this.ofn.lpstrFile, destination.Length);
                }
                else
                {
                    byte[] bytes = Encoding.Default.GetBytes(this.filename);
                    byte[] destinationArray = new byte[0x2000];
                    int length = Math.Min(bytes.Length, destinationArray.Length - 1);
                    Array.Copy(bytes, 0, destinationArray, 0, length);
                    destinationArray[length] = 0;
                    Marshal.Copy(destinationArray, 0, this.ofn.lpstrFile, destinationArray.Length);
                }
            }

            protected abstract bool RunDialog();
            private void StructToLocal()
            {
                if (this.unicode)
                {
                    char[] destination = new char[0x2000];
                    Marshal.Copy(this.ofn.lpstrFile, destination, 0, 0x2000);
                    int index = 0;
                    while ((index < destination.Length) && (destination[index] != '\0'))
                    {
                        index++;
                    }
                    this.filename = new string(destination, 0, index);
                }
                else
                {
                    byte[] buffer = new byte[0x2000];
                    Marshal.Copy(this.ofn.lpstrFile, buffer, 0, 0x2000);
                    int num2 = 0;
                    while ((num2 < buffer.Length) && (buffer[num2] != 0))
                    {
                        num2++;
                    }
                    this.filename = Encoding.Default.GetString(buffer, 0, num2);
                }
            }

            // Properties
            public string FileName
            {
                get
                {
                    return this.filename;
                }
                set
                {
                    this.filename = value;
                }
            }

            public string Filter
            {
                get
                {
                    return this.filter;
                }
                set
                {
                    this.filter = value;
                }
            }

            public int Flags
            {
                get
                {
                    return this.ofn.Flags;
                }
                set
                {
                    this.ofn.Flags = value;
                }
            }

            public int FlagsEx
            {
                get
                {
                    return this.ofn.FlagsEx;
                }
                set
                {
                    this.ofn.FlagsEx = value;
                }
            }

            public string InitialDirectory
            {
                get
                {
                    return this.ofn.lpstrInitialDir;
                }
                set
                {
                    this.ofn.lpstrInitialDir = value;
                }
            }

            public Win32Api.HWND Owner
            {
                get
                {
                    return this.ofn.hwndOwner;
                }
                set
                {
                    this.ofn.hwndOwner = value;
                }
            }

            public string Title
            {
                get
                {
                    return this.ofn.lpstrTitle;
                }
                set
                {
                    this.ofn.lpstrTitle = value;
                }
            }
        }

        public enum GEOCLASS
        {
            GEOCLASS_NATION = 0x10
        }

        public delegate bool GeoEnumProc(int geoId);

        [Flags]
        public enum GEOINFO
        {
            GEO_FRIENDLYNAME = 8,
            GEO_ISO2 = 4,
            None = 0
        }

        public enum GetWindow_Cmd : uint
        {
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6,
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct HANDLE
        {
            public IntPtr h;
            public static explicit operator Win32Api.HANDLE(IntPtr h)
            {
                return new Win32Api.HANDLE { h = h };
            }

            public bool IsValid
            {
                get
                {
                    return (this.h != Win32Api.INVALID_HANDLE_VALUE);
                }
            }
            public static Win32Api.HANDLE NULL
            {
                get
                {
                    return new Win32Api.HANDLE { h = IntPtr.Zero };
                }
            }
            public static Win32Api.HANDLE INVALID
            {
                get
                {
                    return new Win32Api.HANDLE { h = Win32Api.INVALID_HANDLE_VALUE };
                }
            }
            public static bool operator ==(Win32Api.HANDLE hl, Win32Api.HANDLE hr)
            {
                return (hl.h == hr.h);
            }

            public static bool operator !=(Win32Api.HANDLE hl, Win32Api.HANDLE hr)
            {
                return (hl.h != hr.h);
            }

            public override bool Equals(object oCompare)
            {
                return (this.h == ((IntPtr)oCompare));
            }

            public override int GetHashCode()
            {
                return (int)this.h;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HBITMAP
        {
            public IntPtr h;
            public static implicit operator Win32Api.HGDIOBJ(Win32Api.HBITMAP hbmp)
            {
                return (Win32Api.HGDIOBJ)hbmp.h;
            }

            public static explicit operator Win32Api.HBITMAP(Win32Api.HGDIOBJ hobj)
            {
                return (Win32Api.HBITMAP)hobj.h;
            }

            public static explicit operator Win32Api.HBITMAP(IntPtr h)
            {
                return new Win32Api.HBITMAP { h = h };
            }

            public static Win32Api.HBITMAP NULL
            {
                get
                {
                    return new Win32Api.HBITMAP { h = IntPtr.Zero };
                }
            }
            public static bool operator ==(Win32Api.HBITMAP hl, Win32Api.HBITMAP hr)
            {
                return (hl.h == hr.h);
            }

            public static bool operator !=(Win32Api.HBITMAP hl, Win32Api.HBITMAP hr)
            {
                return (hl.h != hr.h);
            }

            public override bool Equals(object oCompare)
            {
                return (this.h == ((IntPtr)oCompare));
            }

            public override int GetHashCode()
            {
                return (int)this.h;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HBRUSH
        {
            public IntPtr h;
            public static implicit operator Win32Api.HGDIOBJ(Win32Api.HBRUSH hbr)
            {
                return (Win32Api.HGDIOBJ)hbr.h;
            }

            public static explicit operator Win32Api.HBRUSH(Win32Api.HGDIOBJ hobj)
            {
                return (Win32Api.HBRUSH)hobj.h;
            }

            public static explicit operator Win32Api.HBRUSH(IntPtr h)
            {
                return new Win32Api.HBRUSH { h = h };
            }

            public static Win32Api.HBRUSH NULL
            {
                get
                {
                    return new Win32Api.HBRUSH { h = IntPtr.Zero };
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HCURSOR
        {
            public IntPtr h;
            public static explicit operator Win32Api.HCURSOR(IntPtr h)
            {
                return new Win32Api.HCURSOR { h = h };
            }

            public static Win32Api.HCURSOR NULL
            {
                get
                {
                    return new Win32Api.HCURSOR { h = IntPtr.Zero };
                }
            }
            public static bool operator ==(Win32Api.HCURSOR hl, Win32Api.HCURSOR hr)
            {
                return (hl.h == hr.h);
            }

            public static bool operator !=(Win32Api.HCURSOR hl, Win32Api.HCURSOR hr)
            {
                return (hl.h != hr.h);
            }

            public override bool Equals(object oCompare)
            {
                return (this.h == ((IntPtr)oCompare));
            }

            public override int GetHashCode()
            {
                return (int)this.h;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HDC
        {
            public IntPtr h;
            public static explicit operator Win32Api.HDC(IntPtr h)
            {
                return new Win32Api.HDC { h = h };
            }

            public static Win32Api.HDC NULL
            {
                get
                {
                    return new Win32Api.HDC { h = IntPtr.Zero };
                }
            }
            public static bool operator ==(Win32Api.HDC hl, Win32Api.HDC hr)
            {
                return (hl.h == hr.h);
            }

            public static bool operator !=(Win32Api.HDC hl, Win32Api.HDC hr)
            {
                return (hl.h != hr.h);
            }

            public override bool Equals(object oCompare)
            {
                return (this.h == ((IntPtr)oCompare));
            }

            public override int GetHashCode()
            {
                return (int)this.h;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HFONT
        {
            public IntPtr h;
            public static implicit operator Win32Api.HGDIOBJ(Win32Api.HFONT hfnt)
            {
                return (Win32Api.HGDIOBJ)hfnt.h;
            }

            public static explicit operator Win32Api.HFONT(Win32Api.HGDIOBJ hobj)
            {
                return (Win32Api.HFONT)hobj.h;
            }

            public static explicit operator Win32Api.HFONT(IntPtr h)
            {
                return new Win32Api.HFONT { h = h };
            }

            public static Win32Api.HFONT NULL
            {
                get
                {
                    return new Win32Api.HFONT { h = IntPtr.Zero };
                }
            }
            public static bool operator ==(Win32Api.HFONT hl, Win32Api.HFONT hr)
            {
                return (hl.h == hr.h);
            }

            public static bool operator !=(Win32Api.HFONT hl, Win32Api.HFONT hr)
            {
                return (hl.h != hr.h);
            }

            public override bool Equals(object oCompare)
            {
                return (this.h == ((IntPtr)oCompare));
            }

            public override int GetHashCode()
            {
                return (int)this.h;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HGDIOBJ
        {
            public IntPtr h;
            public static explicit operator Win32Api.HGDIOBJ(IntPtr h)
            {
                return new Win32Api.HGDIOBJ { h = h };
            }

            public static Win32Api.HGDIOBJ NULL
            {
                get
                {
                    return new Win32Api.HGDIOBJ { h = IntPtr.Zero };
                }
            }
            public static bool operator ==(Win32Api.HGDIOBJ hl, Win32Api.HGDIOBJ hr)
            {
                return (hl.h == hr.h);
            }

            public static bool operator !=(Win32Api.HGDIOBJ hl, Win32Api.HGDIOBJ hr)
            {
                return (hl.h != hr.h);
            }

            public override bool Equals(object oCompare)
            {
                return (this.h == ((IntPtr)oCompare));
            }

            public override int GetHashCode()
            {
                return (int)this.h;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class HH_AKLINK
        {
            public int cbStruct = Marshal.SizeOf(typeof(Win32Api.HH_AKLINK));
            public bool fReserved;
            public string pszKeywords;
            public string pszUrl;
            public string pszMsgText;
            public string pszMsgTitle;
            public string pszWindow;
            public bool fIndexOnFail;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class HH_FTS_QUERY
        {
            public int cbStruct = Marshal.SizeOf(typeof(Win32Api.HH_FTS_QUERY));
            public bool fUniCodeStrings;
            public string pszSearchQuery;
            public int iProximity = Win32Api.HH_FTS_DEFAULT_PROXIMITY;
            public bool fStemmedSearch;
            public bool fTitleOnly;
            public bool fExecute = true;
            public string pszWindow;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class HH_POPUP
        {
            public int cbStruct = Marshal.SizeOf(typeof(Win32Api.HH_POPUP));
            public IntPtr hinst = IntPtr.Zero;
            public int idString;
            public string pszText;
            public Win32Api.POINT pt = new Win32Api.POINT();
            public int clrForeground = -1;
            public int clrBackground = -1;
            public Win32Api.RECT rcMargins = Win32Api.RECT.FromXYWH(-1, -1, -1, -1);
            public string pszFont;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HICON
        {
            public IntPtr h;
            public static explicit operator Win32Api.HICON(IntPtr h)
            {
                return new Win32Api.HICON { h = h };
            }

            public static implicit operator IntPtr(Win32Api.HICON h)
            {
                return h.h;
            }

            public static Win32Api.HICON NULL
            {
                get
                {
                    return new Win32Api.HICON { h = IntPtr.Zero };
                }
            }
            public static bool operator ==(Win32Api.HICON hl, Win32Api.HICON hr)
            {
                return (hl.h == hr.h);
            }

            public static bool operator !=(Win32Api.HICON hl, Win32Api.HICON hr)
            {
                return (hl.h != hr.h);
            }

            public override bool Equals(object oCompare)
            {
                return (this.h == ((IntPtr)oCompare));
            }

            public override int GetHashCode()
            {
                return (int)this.h;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct HIGHCONTRAST_I
        {
            public int cbSize;
            public int dwFlags;
            public IntPtr lpszDefaultScheme;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HINSTANCE
        {
            public IntPtr h;
            public static explicit operator Win32Api.HINSTANCE(IntPtr h)
            {
                return new Win32Api.HINSTANCE { h = h };
            }

            public static Win32Api.HINSTANCE NULL
            {
                get
                {
                    return new Win32Api.HINSTANCE { h = IntPtr.Zero };
                }
            }
            public static bool operator ==(Win32Api.HINSTANCE hl, Win32Api.HINSTANCE hr)
            {
                return (hl.h == hr.h);
            }

            public static bool operator !=(Win32Api.HINSTANCE hl, Win32Api.HINSTANCE hr)
            {
                return (hl.h != hr.h);
            }

            public override bool Equals(object oCompare)
            {
                return (this.h == ((IntPtr)oCompare));
            }

            public override int GetHashCode()
            {
                return (int)this.h;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HKEY
        {
            public UIntPtr h;
            public static explicit operator Win32Api.HKEY(UIntPtr h)
            {
                return new Win32Api.HKEY { h = h };
            }

            public static Win32Api.HKEY NULL
            {
                get
                {
                    return new Win32Api.HKEY { h = UIntPtr.Zero };
                }
            }
            public static Win32Api.HKEY HKEY_CURRENT_USER
            {
                get
                {
                    Win32Api.HKEY hkey = new Win32Api.HKEY();
                    uint num = 0x80000001;
                    hkey.h = new UIntPtr(num);
                    return hkey;
                }
            }
            public static Win32Api.HKEY HKEY_LOCAL_MACHINE
            {
                get
                {
                    Win32Api.HKEY hkey = new Win32Api.HKEY();
                    uint num = 0x80000002;
                    hkey.h = new UIntPtr(num);
                    return hkey;
                }
            }
            public static bool operator ==(Win32Api.HKEY a, Win32Api.HKEY b)
            {
                return a.Equals(b);
            }

            public static bool operator !=(Win32Api.HKEY a, Win32Api.HKEY b)
            {
                return !a.Equals(b);
            }

            public override bool Equals(object obj)
            {
                return ((obj is Win32Api.HKEY) && ((Win32Api.HKEY)obj).h.Equals(this.h));
            }

            public override int GetHashCode()
            {
                return this.h.GetHashCode();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HMENU
        {
            public IntPtr h;
            public static explicit operator Win32Api.HMENU(IntPtr h)
            {
                return new Win32Api.HMENU { h = h };
            }

            public static implicit operator IntPtr(Win32Api.HMENU h)
            {
                return h.h;
            }

            public static Win32Api.HMENU NULL
            {
                get
                {
                    return new Win32Api.HMENU { h = IntPtr.Zero };
                }
            }
            public static bool operator ==(Win32Api.HMENU hl, Win32Api.HMENU hr)
            {
                return (hl.h == hr.h);
            }

            public static bool operator !=(Win32Api.HMENU hl, Win32Api.HMENU hr)
            {
                return (hl.h != hr.h);
            }

            public override bool Equals(object oCompare)
            {
                return (this.h == ((IntPtr)oCompare));
            }

            public override int GetHashCode()
            {
                return (int)this.h;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HMONITOR
        {
            public IntPtr h;
            public static explicit operator Win32Api.HMONITOR(IntPtr h)
            {
                return new Win32Api.HMONITOR { h = h };
            }

            public static Win32Api.HMONITOR NULL
            {
                get
                {
                    return new Win32Api.HMONITOR { h = IntPtr.Zero };
                }
            }
            public static bool operator ==(Win32Api.HMONITOR hl, Win32Api.HMONITOR hr)
            {
                return (hl.h == hr.h);
            }

            public static bool operator !=(Win32Api.HMONITOR hl, Win32Api.HMONITOR hr)
            {
                return (hl.h != hr.h);
            }

            public override bool Equals(object oCompare)
            {
                return (this.h == ((IntPtr)oCompare));
            }

            public override int GetHashCode()
            {
                return (int)this.h;
            }
        }

        public delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

        public enum HookType
        {
            WH_JOURNALRECORD,
            WH_JOURNALPLAYBACK,
            WH_KEYBOARD,
            WH_GETMESSAGE,
            WH_CALLWNDPROC,
            WH_CBT,
            WH_SYSMSGFILTER,
            WH_MOUSE,
            WH_HARDWARE,
            WH_DEBUG,
            WH_SHELL,
            WH_FOREGROUNDIDLE,
            WH_CALLWNDPROCRET,
            WH_KEYBOARD_LL,
            WH_MOUSE_LL
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HPEN
        {
            public IntPtr h;
            public static implicit operator Win32Api.HGDIOBJ(Win32Api.HPEN hpen)
            {
                return (Win32Api.HGDIOBJ)hpen.h;
            }

            public static explicit operator Win32Api.HPEN(Win32Api.HGDIOBJ hobj)
            {
                return (Win32Api.HPEN)hobj.h;
            }

            public static explicit operator Win32Api.HPEN(IntPtr h)
            {
                return new Win32Api.HPEN { h = h };
            }

            public static Win32Api.HPEN NULL
            {
                get
                {
                    return new Win32Api.HPEN { h = IntPtr.Zero };
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HPOWERNOTIFY
        {
            public IntPtr h;
            public static explicit operator Win32Api.HPOWERNOTIFY(IntPtr h)
            {
                return new Win32Api.HPOWERNOTIFY { h = h };
            }

            public static implicit operator IntPtr(Win32Api.HPOWERNOTIFY h)
            {
                return h.h;
            }

            public static Win32Api.HPOWERNOTIFY NULL
            {
                get
                {
                    return new Win32Api.HPOWERNOTIFY { h = IntPtr.Zero };
                }
            }
            public static bool operator ==(Win32Api.HPOWERNOTIFY hl, Win32Api.HPOWERNOTIFY hr)
            {
                return (hl.h == hr.h);
            }

            public static bool operator !=(Win32Api.HPOWERNOTIFY hl, Win32Api.HPOWERNOTIFY hr)
            {
                return (hl.h != hr.h);
            }

            public override bool Equals(object oCompare)
            {
                return (this.h == ((IntPtr)oCompare));
            }

            public override int GetHashCode()
            {
                return (int)this.h;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HRGN
        {
            public IntPtr h;
            public static explicit operator Win32Api.HRGN(IntPtr h)
            {
                return new Win32Api.HRGN { h = h };
            }

            public static Win32Api.HRGN NULL
            {
                get
                {
                    return new Win32Api.HRGN { h = IntPtr.Zero };
                }
            }
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct HWND
        {
            public IntPtr h;
            public static explicit operator Win32Api.HWND(IntPtr h)
            {
                return new Win32Api.HWND { h = h };
            }

            public static implicit operator IntPtr(Win32Api.HWND h)
            {
                return h.h;
            }

            public static Win32Api.HWND NULL
            {
                get
                {
                    return new Win32Api.HWND { h = IntPtr.Zero };
                }
            }
            public static Win32Api.HWND HWND_TOP
            {
                get
                {
                    return new Win32Api.HWND { h = new IntPtr(0) };
                }
            }
            public static Win32Api.HWND HWND_BOTTOM
            {
                get
                {
                    return new Win32Api.HWND { h = new IntPtr(1) };
                }
            }
            public static Win32Api.HWND HWND_TOPMOST
            {
                get
                {
                    return new Win32Api.HWND { h = new IntPtr(-1) };
                }
            }
            public static Win32Api.HWND HWND_NOTOPMOST
            {
                get
                {
                    return new Win32Api.HWND { h = new IntPtr(-2) };
                }
            }
            public static Win32Api.HWND HWND_MESSAGE
            {
                get
                {
                    return new Win32Api.HWND { h = new IntPtr(-3) };
                }
            }
            public static bool operator ==(Win32Api.HWND hl, Win32Api.HWND hr)
            {
                return (hl.h == hr.h);
            }

            public static bool operator !=(Win32Api.HWND hl, Win32Api.HWND hr)
            {
                return (hl.h != hr.h);
            }

            public override bool Equals(object oCompare)
            {
                return (this.h == ((IntPtr)oCompare));
            }

            public override int GetHashCode()
            {
                return (int)this.h;
            }

            public override string ToString()
            {
                if (this.h == IntPtr.Zero)
                {
                    return "NULL";
                }
                StringBuilder text = new StringBuilder(0xff);
                string str = "<unknown class>";
                if (Win32Api.GetClassName(this, text, text.Capacity) > 0)
                {
                    str = InvariantString.Format("cls=\"{0}\"", new object[] { text });
                }
                string str2 = null;
                if (Win32Api.GetWindowText(this, text, text.Capacity) > 0)
                {
                    str2 = InvariantString.Format(", txt=\"{0}\"", new object[] { text });
                }
                return InvariantString.Format("0x{0,0:X} ({1}{2})", new object[] { this.h, str, str2 });
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ICONINFO
        {
            public int fIcon;
            public uint xHotspot;
            public uint yHotspot;
            public Win32Api.HBITMAP hbmMask;
            public Win32Api.HBITMAP hbmColor;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public int type;
            public Win32Api.INPUTUNION data;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUTUNION
        {
            // Fields
            [FieldOffset(0)]
            public Win32Api.HARDWAREINPUT hi;
            [FieldOffset(0)]
            public Win32Api.KEYBDINPUT ki;
            [FieldOffset(0)]
            public Win32Api.MOUSEINPUT mi;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class LOGFONT
        {
            public const int LF_FACESIZE = 0x20;
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string lfFaceName;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct LOGFONTW_STRUCT
        {
            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public int lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public byte lfOutPrecision;
            public byte lfClipPrecision;
            public byte lfQuality;
            public byte lfPitchAndFamily;
            public FACENAME lfFaceName;
            // Nested Types
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct FACENAME
            {
                private char name00;
                private char name01;
                private char name02;
                private char name03;
                private char name04;
                private char name05;
                private char name06;
                private char name07;
                private char name08;
                private char name09;
                private char name10;
                private char name11;
                private char name12;
                private char name13;
                private char name14;
                private char name15;
                private char name16;
                private char name17;
                private char name18;
                private char name19;
                private char name20;
                private char name21;
                private char name22;
                private char name23;
                private char name24;
                private char name25;
                private char name26;
                private char name27;
                private char name28;
                private char name29;
                private char name30;
                private char name31;
            }
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public class MEMORYSTATUSEX
        {
            public int Length = Marshal.SizeOf(typeof(Win32Api.MEMORYSTATUSEX));
            public int MemoryLoad;
            public long TotalPhysical;
            public long AvailablePhysical;
            public long TotalPageFile;
            public long AvailablePageFile;
            public long TotalVirtual;
            public long AvailableVirtual;
            public long AvailableExtendedVirtual;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MENUINFO
        {
            public uint cbSize;
            public uint fMask;
            public uint dwStyle;
            public uint cyMax;
            public Win32Api.HGDIOBJ hbrBack;
            public uint dwContextHelpID;
            public UIntPtr dwMenuData;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MINMAXINFO
        {
            public Win32Api.POINT ptReserved;
            public Win32Api.POINT ptMaxSize;
            public Win32Api.POINT ptMaxPosition;
            public Win32Api.POINT ptMinTrackSize;
            public Win32Api.POINT ptMaxTrackSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MONITORINFOEX
        {
            public uint cbSize;
            public Win32Api.RECT rcMonitor;
            public Win32Api.RECT rcWork;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string deviceName;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public uint dx;
            public uint dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            public Win32Api.HWND hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public int pt_x;
            public int pt_y;
            public override string ToString()
            {
                return InvariantString.Format("{0} -> {1}, wp=0x{2,0:x} lp=0x{3,0:x}", new object[] { Win32Api.DumpMessage(this.message), this.hwnd, this.wParam, this.lParam });
            }
        }

        public enum NETSETUP_JOIN_STATUS
        {
            NetSetupUnknownStatus,
            NetSetupUnjoined,
            NetSetupWorkgroupName,
            NetSetupDomainName
        }

        public class OpenFileDialog : Win32Api.FileDialog
        {
            // Methods
            public OpenFileDialog()
            {
                base.Flags |= 0x1804;
            }

            protected override bool RunDialog()
            {
                return Win32Api.GetOpenFileName(ref this.ofn);
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        public struct OPENFILENAME
        {
            public int lStructSize;
            public Win32Api.HWND hwndOwner;
            public Win32Api.HINSTANCE hInstance;
            public string lpstrFilter;
            public IntPtr lpstrCustomFilter;
            public int nMaxCustFilter;
            public int nFilterIndex;
            public IntPtr lpstrFile;
            public int nMaxFile;
            public IntPtr lpstrFileTitle;
            public int nMaxFileTitle;
            public string lpstrInitialDir;
            public string lpstrTitle;
            public int Flags;
            public short nFileOffset;
            public short nFileExtension;
            public string lpstrDefExt;
            public IntPtr lCustData;
            public Win32Api.WNDPROC lpfnHook;
            public string lpTemplateName;
            public IntPtr pvReserved;
            public int dwReserved;
            public int FlagsEx;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PAINTSTRUCT
        {
            public Win32Api.HDC hdc;
            public int fErase;
            public int rcPaint_left;
            public int rcPaint_top;
            public int rcPaint_right;
            public int rcPaint_bottom;
            public int fRestore;
            public int fIncUpdate;
            public int reserved1;
            public int reserved2;
            public int reserved3;
            public int reserved4;
            public int reserved5;
            public int reserved6;
            public int reserved7;
            public int reserved8;
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POWERBROADCAST_SETTING
        {
            public Guid PowerSetting;
            public uint DataLength;
            public byte Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct PROPSPEC
        {
            // Fields
            [FieldOffset(0)]
            public uint kind;
            [FieldOffset(4)]
            public uint propid;
            [FieldOffset(4)]
            public IntPtr str;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct PROPSPEC64
        {
            // Fields
            [FieldOffset(0)]
            public uint kind;
            [FieldOffset(8)]
            public uint propid;
            [FieldOffset(8)]
            public IntPtr str;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct PROPVARIANT
        {
            // Fields
            [FieldOffset(8)]
            public Win32Api.CAUB byteVector;
            [FieldOffset(2)]
            public short pad;
            [FieldOffset(4)]
            public uint pad2;
            [FieldOffset(8)]
            public IntPtr t;
            [FieldOffset(8)]
            public ulong ul;
            [FieldOffset(0)]
            public short vt;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct QUERY_SERVICE_LOCK_STATUS
        {
            public int fIsLocked;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpLockOwner;
            public int dwLockDuration;
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public RECT(Win32Api.RECT rcSrc)
            {
                this.left = rcSrc.left;
                this.top = rcSrc.top;
                this.right = rcSrc.right;
                this.bottom = rcSrc.bottom;
            }

            public int Width
            {
                get
                {
                    return (this.right - this.left);
                }
                set
                {
                    this.right = this.left + value;
                }
            }
            public int Height
            {
                get
                {
                    return (this.bottom - this.top);
                }
                set
                {
                    this.bottom = this.top + value;
                }
            }
            public static Win32Api.RECT FromXYWH(int x, int y, int width, int height)
            {
                return new Win32Api.RECT(x, y, x + width, y + height);
            }
        }

        public class SafeEventHandle : SafeHandle
        {
            // Methods
            private SafeEventHandle()
                : base(IntPtr.Zero, true)
            {
            }

            [DllImport("kernel32.dll")]
            private static extern bool CloseHandle(IntPtr handle);
            [DllImport("kernel32.dll")]
            public static extern Win32Api.SafeEventHandle CreateEvent(IntPtr lpSecurityAttributes, bool bManualReset, bool bInitialState, string lpName);
            [DllImport("Kernel32.dll")]
            public static extern Win32Api.SafeEventHandle OpenEvent(uint dwDesiredAccess, bool bInheritHandle, string lpName);
            protected override bool ReleaseHandle()
            {
                bool flag = false;
                if (!this.IsInvalid)
                {
                    flag = CloseHandle(base.handle);
                }
                base.handle = IntPtr.Zero;
                return flag;
            }

            [DllImport("kernel32.dll")]
            private static extern bool SetEvent(IntPtr hEvent);
            public bool SetSafeEvent()
            {
                return SetEvent(base.handle);
            }

            // Properties
            public override bool IsInvalid
            {
                get
                {
                    if (!base.IsClosed)
                    {
                        return (base.handle == IntPtr.Zero);
                    }
                    return true;
                }
            }

            public SafeWaitHandle SafeWaitHandle
            {
                get
                {
                    return new SafeWaitHandle(base.handle, true);
                }
            }
        }

        public class SafeServiceHandle : SafeHandle
        {
            // Methods
            public SafeServiceHandle()
                : this(IntPtr.Zero, true)
            {
            }

            public SafeServiceHandle(IntPtr invalidHandleValue, bool ownsHandle)
                : base(invalidHandleValue, ownsHandle)
            {
            }

            [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            private static extern bool CloseServiceHandle(IntPtr handle);
            protected override bool ReleaseHandle()
            {
                return CloseServiceHandle(base.handle);
            }

            // Properties
            public override bool IsInvalid
            {
                get
                {
                    if (!base.IsClosed)
                    {
                        return (base.handle == IntPtr.Zero);
                    }
                    return true;
                }
            }
        }

        public class SaveFileDialog : Win32Api.FileDialog
        {
            // Methods
            public SaveFileDialog()
            {
                base.Flags |= 2;
            }

            protected override bool RunDialog()
            {
                return Win32Api.GetSaveFileName(ref this.ofn);
            }
        }

        [Flags]
        public enum SC_MANAGER_ACCESS_TYPE : uint
        {
            None = 0,
            SC_MANAGER_CONNECT = 1,
            SC_MANAGER_CREATE_SERVICE = 2,
            SC_MANAGER_ENUMERATE_SERVICE = 4,
            SC_MANAGER_LOCK = 8,
            SC_MANAGER_MODIFY_BOOT_CONFIG = 0x20,
            SC_MANAGER_QUERY_LOCK_STATUS = 0x10
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SCROLLINFO
        {
            public uint cbSize;
            public uint fMask;
            public int nMin;
            public int nMax;
            public uint nPage;
            public int nPos;
            public int nTrackPos;
        }

        [Flags]
        public enum SERVICE_ACCESS_TYPE : uint
        {
            None = 0,
            SERVICE_CHANGE_CONFIG = 2,
            SERVICE_ENUMERATE_DEPENDENTS = 8,
            SERVICE_INTERROGATE = 0x80,
            SERVICE_PAUSE_CONTINUE = 0x40,
            SERVICE_QUERY_CONFIG = 1,
            SERVICE_QUERY_STATUS = 4,
            SERVICE_START = 0x10,
            SERVICE_STOP = 0x20,
            SERVICE_USER_DEFINED_CONTROL = 0x100
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SERVICE_DELAYED_AUTO_START_INFO
        {
            public bool fDelayedAutostart;
        }

        public enum SERVICE_START_TYPE : uint
        {
            SERVICE_AUTO_START = 2,
            SERVICE_BOOT_START = 0,
            SERVICE_DEMAND_START = 3,
            SERVICE_DISABLED = 4,
            SERVICE_SYSTEM_START = 1
        }

        public enum SERVICE_STATE
        {
            SERVICE_CONTINUE_PENDING = 5,
            SERVICE_PAUSE_PENDING = 6,
            SERVICE_PAUSED = 7,
            SERVICE_RUNNING = 4,
            SERVICE_START_PENDING = 2,
            SERVICE_STOP_PENDING = 3,
            SERVICE_STOPPED = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SERVICE_STATUS
        {
            public uint serviceType;
            public Win32Api.SERVICE_STATE currentState;
            public uint controlsAccepted;
            public uint win32ExitCode;
            public uint serviceSpecificExitCode;
            public uint checkPoint;
            public uint waitHint;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SIZE
        {
            public int cx;
            public int cy;
            public SIZE(int cx, int cy)
            {
                this.cx = cx;
                this.cy = cy;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class STARTUPINFO
        {
            public uint cb = ((uint)Marshal.SizeOf(typeof(Win32Api.STARTUPINFO)));
            public IntPtr lpReserved = IntPtr.Zero;
            public IntPtr lpDesktop = IntPtr.Zero;
            public IntPtr lpTitle = IntPtr.Zero;
            public uint dwX;
            public uint dwY;
            public uint dwXSize;
            public uint dwYSize;
            public uint dwXCountChars;
            public uint dwYCountChars;
            public uint dwFillAttribute;
            public uint dwFlags;
            public ushort wShowWindow;
            public ushort cbReserved2;
            public IntPtr lpReserved2 = IntPtr.Zero;
            public Win32Api.HANDLE hStdInput;
            public Win32Api.HANDLE hStdOutput;
            public Win32Api.HANDLE hStdError;
        }

        [StructLayout(LayoutKind.Sequential), ComVisible(false)]
        public struct SYSTEM_INFO
        {
            public ushort wProcessorArchitecture;
            public ushort wReserved;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public IntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort wProcessorLevel;
            public ushort wProcessorRevision;
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
            public short Year
            {
                get
                {
                    return this.wYear;
                }
                set
                {
                    this.wYear = value;
                }
            }
            public short Month
            {
                get
                {
                    return this.wMonth;
                }
                set
                {
                    this.wMonth = value;
                }
            }
            public short DayOfWeek
            {
                get
                {
                    return this.wDayOfWeek;
                }
                set
                {
                    this.wDayOfWeek = value;
                }
            }
            public short Day
            {
                get
                {
                    return this.wDay;
                }
                set
                {
                    this.wDay = value;
                }
            }
            public short Hour
            {
                get
                {
                    return this.wHour;
                }
                set
                {
                    this.wHour = value;
                }
            }
            public short Minute
            {
                get
                {
                    return this.wMinute;
                }
                set
                {
                    this.wMinute = value;
                }
            }
            public short Second
            {
                get
                {
                    return this.wSecond;
                }
                set
                {
                    this.wSecond = value;
                }
            }
            public short Milliseconds
            {
                get
                {
                    return this.wMilliseconds;
                }
                set
                {
                    this.wMilliseconds = value;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct TEXTMETRIC
        {
            public uint tmHeight;
            public uint tmAscent;
            public uint tmDescent;
            public uint tmInternalLeading;
            public uint tmExternalLeading;
            public uint tmAveCharWidth;
            public uint tmMaxCharWidth;
            public uint tmWeight;
            public uint tmOverhang;
            public uint tmDigitizedAspectX;
            public uint tmDigitizedAspectY;
            public char tmFirstChar;
            public char tmLastChar;
            public char tmDefaultChar;
            public char tmBreakChar;
            public byte tmItalic;
            public byte tmUnderlined;
            public byte tmStruckOut;
            public byte tmPitchAndFamily;
            public byte tmCharSet;
        }

        public delegate void TIMERPROC(Win32Api.HWND hwnd, uint nMsg, UIntPtr idEvent, uint dwTime);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class TRACKMOUSEEVENT
        {
            public uint cbSize;
            public uint dwFlags;
            public Win32Api.HWND hwndTrack;
            public uint dwHoverTime;
        }

        public sealed class UnmanagedWaitHandle : WaitHandle
        {
            // Methods
            public UnmanagedWaitHandle(IntPtr handle)
            {
                Win32Api.HANDLE currentProcess = Win32Api.GetCurrentProcess();
                IntPtr zero = IntPtr.Zero;
                if (!Win32Api.DuplicateHandle(currentProcess.h, handle, currentProcess.h, ref zero, 0, false, 2))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                base.SafeWaitHandle = new SafeWaitHandle(zero, true);
            }
        }

        public delegate void WaitOrTimerCallBackDelegate(IntPtr lpParameter, byte f);

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct WIN32_FILE_ATTRIBUTE_DATA
        {
            public int fileAttributes;
            public uint ftCreationTimeLow;
            public uint ftCreationTimeHigh;
            public uint ftLastAccessTimeLow;
            public uint ftLastAccessTimeHigh;
            public uint ftLastWriteTimeLow;
            public uint ftLastWriteTimeHigh;
            public int fileSizeHigh;
            public int fileSizeLow;
        }

        [Serializable, StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class WIN32_FIND_DATA
        {
            public int dwFileAttributes;
            public uint ftCreationTime_dwLowDateTime;
            public uint ftCreationTime_dwHighDateTime;
            public uint ftLastAccessTime_dwLowDateTime;
            public uint ftLastAccessTime_dwHighDateTime;
            public uint ftLastWriteTime_dwLowDateTime;
            public uint ftLastWriteTime_dwHighDateTime;
            public int nFileSizeHigh;
            public int nFileSizeLow;
            public int dwReserved0;
            public int dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public class WINDOWPLACEMENT
        {
            public uint length = ((uint)Marshal.SizeOf(typeof(Win32Api.WINDOWPLACEMENT)));
            public uint flags;
            public int showCmd;
            public Win32Api.POINT ptMinPosition;
            public Win32Api.POINT ptMaxPosition;
            public Win32Api.RECT rcNormalPosition;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class WINDOWPOS
        {
            public Win32Api.HWND hwnd;
            public Win32Api.HWND hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public uint flags;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public class WNDCLASSEX
        {
            public uint cbSize;
            public uint style;
            public Win32Api.WNDPROC lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public Win32Api.HINSTANCE hInstance;
            public Win32Api.HICON hIcon;
            public Win32Api.HCURSOR hCursor;
            public Win32Api.HGDIOBJ hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
            public Win32Api.HICON hIconSm;
        }

        public delegate IntPtr WNDPROC(Win32Api.HWND hwnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct XFORM
        {
            public float eM11;
            public float eM12;
            public float eM21;
            public float eM22;
            public float eDx;
            public float eDy;
        }
    }
}
