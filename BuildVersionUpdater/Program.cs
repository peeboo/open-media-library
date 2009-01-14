using System;
using System.IO;

namespace BuildVersionUpdater
{
    class Program
    {
        const string BASE_DIR         = @"..\..\..\Library";
        const string SETUPX86FILE     = @"Setup\Setup.wxs";
        const string SETUPX64FILE     = @"Setupx64\Setup.wxs";
        const string REGISTRATIONFILE = @"Registration.xml";
        const string ASSEMBLYFILE     = @"Properties\AssemblyInfo.cs";
        static void Main(string[] args)
        {
            // first get the latest version from the assembly file
            string version_number = string.Empty;

            string assemblyFileText = File.ReadAllText(Path.Combine(BASE_DIR, ASSEMBLYFILE));
            if (assemblyFileText.Contains("assembly: AssemblyVersion"))
            {
                int startLoc = assemblyFileText.IndexOf("assembly: AssemblyVersion(\"");
                if (startLoc > 0)
                {
                    startLoc += "assembly: AssemblyVersion(\"".Length;
                    version_number = assemblyFileText.Substring(startLoc, 7);
                    if (string.IsNullOrEmpty(version_number))
                    {
                        Console.WriteLine("ERROR reading current assembly version from Assembly file: {0}",
                            Path.Combine(BASE_DIR, ASSEMBLYFILE));
                        return;
                    }
                }

                if (assemblyFileText.Contains("assembly: AssemblyFileVersion"))
                {
                    int fileStartLoc = assemblyFileText.IndexOf("assembly: AssemblyFileVersion(\"");
                    if (fileStartLoc > 0)
                    {
                        fileStartLoc += "assembly: AssemblyFileVersion(\"".Length;
                        string fileVersionNumber = assemblyFileText.Substring(fileStartLoc, 7);
                        if (string.IsNullOrEmpty(fileVersionNumber))
                        {
                            Console.WriteLine("ERROR reading current file version from Assembly file: {0}",
                                Path.Combine(BASE_DIR, ASSEMBLYFILE));
                            return;
                        }

                        if (version_number.CompareTo(fileVersionNumber) != 0)
                        {
                            Console.WriteLine("ERROR the assembly version and the file version of the Assembly file do NOT match");
                            return;
                        }

                        // the assembly file looks good, both assemblyversion and fileversion are the same
                        if (!FixSetupX86File(version_number))
                        {
                        }

                        if (!FixSetupX64File(version_number))
                        {
                        }

                        if (!FixRegistrationFile(version_number))
                        {
                        }
                        Console.WriteLine("Success, all required files updated.");
                    }
                }
            }
        }

        private static bool FixSetupX86File(string version_number)
        {
            string[] setup86lines = File.ReadAllLines(Path.Combine(BASE_DIR, SETUPX86FILE));
            string newX86file = Path.GetTempFileName();
            TextWriter x86TxtWriter = new StreamWriter(newX86file);
            foreach (string x86line in setup86lines)
            {
                if (x86line.Contains("Property_ProductVersion = \""))
                {
                    int x86loc = x86line.IndexOf("Property_ProductVersion = \"");
                    x86loc += "Property_ProductVersion = \"".Length;
                    string x86stub_version = x86line.Substring(x86loc, 7);
                    string newX86line = x86line.Replace(x86stub_version, version_number);
                    x86TxtWriter.WriteLine(newX86line);
                }
                else
                {
                    x86TxtWriter.WriteLine(x86line);
                }
            }
            if (x86TxtWriter != null)
                x86TxtWriter.Close();

            string tmpX86FileName = Path.GetTempFileName();
            File.Delete(tmpX86FileName);
            try
            {
                File.Move(Path.Combine(BASE_DIR, SETUPX86FILE), tmpX86FileName);
                File.Move(newX86file, Path.Combine(BASE_DIR, SETUPX86FILE));
            } catch {
                if (File.Exists(tmpX86FileName))
                {
                    File.Move(tmpX86FileName, Path.Combine(BASE_DIR, SETUPX86FILE));
                    Console.WriteLine("ERROR: Failed to edit {0}", SETUPX86FILE);
                    return false;
                }
            }
            return true;
        }

        private static bool FixSetupX64File(string version_number)
        {
            string[] setup64lines = File.ReadAllLines(Path.Combine(BASE_DIR, SETUPX64FILE));
            string newX64file = Path.GetTempFileName();
            TextWriter x64TxtWriter = new StreamWriter(newX64file);
            foreach (string x64line in setup64lines)
            {
                if (x64line.Contains("Property_ProductVersion = \""))
                {
                    int x64loc = x64line.IndexOf("Property_ProductVersion = \"");
                    x64loc += "Property_ProductVersion = \"".Length;
                    string x64stub_version = x64line.Substring(x64loc, 7);
                    string newX64line = x64line.Replace(x64stub_version, version_number);
                    x64TxtWriter.WriteLine(newX64line);
                }
                else
                {
                    x64TxtWriter.WriteLine(x64line);
                }
            }
            if (x64TxtWriter != null)
                x64TxtWriter.Close();

            string tmpX64FileName = Path.GetTempFileName();
            File.Delete(tmpX64FileName);
            try
            {
                File.Move(Path.Combine(BASE_DIR, SETUPX64FILE), tmpX64FileName);
                File.Move(newX64file, Path.Combine(BASE_DIR, SETUPX64FILE));
            }
            catch
            {
                if (File.Exists(tmpX64FileName))
                {
                    File.Move(tmpX64FileName, Path.Combine(BASE_DIR, SETUPX64FILE));
                    Console.WriteLine("ERROR: Failed to edit {0}", SETUPX64FILE);
                    return false;
                }
            }
            return true;
        }

        private static bool FixRegistrationFile(string version_number)
        {
            string[] reglines = File.ReadAllLines(Path.Combine(BASE_DIR, REGISTRATIONFILE));
            string newRegfile = Path.GetTempFileName();
            TextWriter regTxtWriter = new StreamWriter(newRegfile);
            foreach (string regline in reglines)
            {
                if (regline.Contains("Version="))
                {
                    int regloc = regline.IndexOf("Version=");
                    regloc += "Version=".Length;
                    string regstub_version = regline.Substring(regloc, 7);
                    string newregline = regline.Replace(regstub_version, version_number);
                    regTxtWriter.WriteLine(newregline);
                }
                else
                {
                    regTxtWriter.WriteLine(regline);
                }
            }
            if (regTxtWriter != null)
                regTxtWriter.Close();

            string tmpregFileName = Path.GetTempFileName();
            File.Delete(tmpregFileName);
            try
            {
                File.Move(Path.Combine(BASE_DIR, REGISTRATIONFILE), tmpregFileName);
                File.Move(newRegfile, Path.Combine(BASE_DIR, REGISTRATIONFILE));
            }
            catch
            {
                if (File.Exists(tmpregFileName))
                {
                    File.Move(tmpregFileName, Path.Combine(BASE_DIR, REGISTRATIONFILE));
                    Console.WriteLine("ERROR: Failed to edit {0}", REGISTRATIONFILE);
                    return false;
                }
            }
            return true;
        }
    }
}
