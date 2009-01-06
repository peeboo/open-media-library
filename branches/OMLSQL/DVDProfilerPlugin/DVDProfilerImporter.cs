using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using OMLEngine;
using OMLSDK;

namespace DVDProfilerPlugin
{
    // [CLSCompliant(true)] // this requires CLSCompliant assemby attribute
    public class DVDProfilerImporter : OMLPlugin
    {
        private Title currentTitle;
        private VideoFormat currentVideoFormat;
        private readonly List<List<string>> currentDiscTitles = new List<List<string>>(); // List of discs with a list of sides
        private string currentNotes;
        private readonly List<string> currentWritersAlreadyAdded = new List<string>();
        private readonly List<string> currentProducersAlreadyAdded = new List<string>();
        private static readonly Regex filepathRegex = new Regex(@"\[filepath((\s+disc\s*=\s*(?'discNumber'\d+)(?'side'[ab])?))?\s*\](?'path'[^\[]+)\[\s*\/\s*filepath\s*\]", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex removeFormattingRegex = new Regex(@"<\/?(i|b)>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private static readonly Regex removeExtraLinebreaksRegex = new Regex(@"(\s*(\r|\n)\s*)+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        string imagesPath;

        public override bool IsSingleFileImporter()
        {
            return true;
        }
        public override string SetupDescription()
        {
            return GetName() + @" will search for and import [" + DefaultFileToImport() + @"] files.";
        }
        public override string DefaultFileToImport()
        {
            return @"Collection.xml";
        }

        private static double MajorVersion = 0.9;
        private static double MinorVersion = 0.1;
        protected override double GetVersionMajor()
        {
            return MajorVersion;
        }

        protected override double GetVersionMinor()
        {
            return MinorVersion;
        }
        protected override string GetMenu()
        {
            return "DVD Profiler";
        }
        protected override string GetName()
        {
            return "DVDProfilerPlugin";
        }
        protected override string GetAuthor()
        {
            return "OML Development Team";
        }
        protected override string GetDescription()
        {
            return @"DVDProfiler xml file importer v" + Version;
        }

        private Title CurrentTitle
        {
            get
            {
                if (currentTitle == null) currentTitle = new Title();
                return currentTitle;
            }
        }

        public override void ProcessFile(string file)
        {
            using (XmlTextReader reader = new XmlTextReader(file))
            {
                reader.MoveToContent();
                switch (reader.LocalName)
                {
                    case "Collection":
                        ReadChildElements(reader, HandleDvd);
                        break;
                    case "DVD":
                        HandleDvd(reader);
                        break;
                    default:
                        throw new IOException("Unsupported root node: " + reader.LocalName);
                }
            }
        }

        private void HandleDvd(XmlTextReader reader)
        {
            Debug.Assert(reader.LocalName == "DVD", "Xml reader error - expected DVD node, got: " + reader.LocalName);
            ClearImportState();
            ReadChildElements(reader, HandleDvdElements);
            NextTitle();
        }

        private void HandleDvdElements(XmlTextReader reader)
        {
            switch (reader.LocalName)
            {
                case "DVD":
                    NextTitle();
                    break;
                case "ID":
                    CurrentTitle.MetadataSourceID = reader.ReadElementString();
                    break;
                case "MediaTypes":
                    ReadChildElements(reader, HandleMediaTypes);
                    break;
                case "UPC":
                    CurrentTitle.UPC = reader.ReadElementString();
                    break;
                case "Title":
                    CurrentTitle.Name = reader.ReadElementString();
                    break;
                case "OriginalTitle":
                    CurrentTitle.OriginalName = reader.ReadElementString();
                    break;
                case "SortTitle":
                    CurrentTitle.SortName = reader.ReadElementString();
                    break;
                case "CountryOfOrigin":
                    CurrentTitle.CountryOfOrigin = reader.ReadElementString();
                    break;
                case "ProductionYear":
                    // Set to January first in the appropriate year in lack of better...
                    int releaseYear;
                    if (TryReadElementInt(reader, out releaseYear)) CurrentTitle.ReleaseDate = new DateTime(releaseYear, 1, 1);
                    break;
                case "RunningTime":
                    int runningTime;
                    if (TryReadElementInt(reader, out runningTime)) CurrentTitle.Runtime = runningTime;
                    break;
                case "Rating":
                    CurrentTitle.ParentalRating = reader.ReadElementString();
                    break;
                case "RatingDetails":
                    CurrentTitle.ParentalRatingReason = reader.ReadElementString();
                    break;
                case "Genres":
                    ReadChildElements(reader, HandleGenres);
                    break;
                case "Format":
                    ReadChildElements(reader, HandleFormat);
                    break;
                case "Studios":
                    ReadChildElements(reader,
                          delegate
                          {
                              if (reader.LocalName == "Studio" && string.IsNullOrEmpty(CurrentTitle.Studio)) CurrentTitle.Studio = reader.ReadElementString();
                          });
                    break;
                case "Audio":
                    ReadChildElements(reader, HandleAudio);
                    break;
                case "Subtitles":
                    ReadChildElements(reader,
                          delegate
                          {
                              if (reader.LocalName == "Subtitle") CurrentTitle.AddSubtitle(reader.ReadElementString());
                          });
                    break;
                case "Actors":
                    ReadChildElements(reader, HandleActors);
                    break;
                case "Credits":
                    ReadChildElements(reader, HandleCredits);
                    break;
                case "Review":
                    ReadFilmReview(reader);
                    break;
                case "Overview":
                    string synopsis = removeFormattingRegex.Replace(reader.ReadElementString(), "");
                    CurrentTitle.Synopsis = removeExtraLinebreaksRegex.Replace(synopsis, "\r\n").Trim();
                    break;
                case "Discs":
                    ReadChildElements(reader, HandleDiscs);
                    break;
                case "Notes":
                    currentNotes = reader.ReadElementString();
                    break;
            }
        }

        //Collection/DVD/Genres/*
        private void HandleGenres(XmlReader reader)
        {
            if (reader.LocalName == "Genre")
            {
                string genre = reader.ReadElementString();
                if (!string.IsNullOrEmpty(genre)) CurrentTitle.AddGenre(genre);
            }
        }

        private void ReadFilmReview(XmlReader reader)
        {
            if (reader.MoveToAttribute("Film"))
            {
                switch (reader.Value)
                {
                    case "9": CurrentTitle.UserStarRating = 100; break;
                    case "8": CurrentTitle.UserStarRating = 90; break;
                    case "7": CurrentTitle.UserStarRating = 80; break;
                    case "6": CurrentTitle.UserStarRating = 70; break;
                    case "5": CurrentTitle.UserStarRating = 60; break;
                    case "4": CurrentTitle.UserStarRating = 50; break;
                    case "3": CurrentTitle.UserStarRating = 40; break;
                    case "2": CurrentTitle.UserStarRating = 20; break; // Have to skip somewhere to avoid fractions :(
                    case "1": CurrentTitle.UserStarRating = 10; break;
                }
            }
        }

        //Collection/DVD/Discs/*
        private void HandleDiscs(XmlTextReader reader)
        {
            if (reader.LocalName == "Disc")
            {
                var sideTitles = new List<string> { null, null };


                ReadChildElements(reader,
                    delegate
                    {
                        switch (reader.LocalName)
                        {
                            case "DescriptionSideA":
                                sideTitles[0] = reader.ReadElementString();
                                break;
                            case "DescriptionSideB":
                                sideTitles[1] = reader.ReadElementString();
                                break;
                        }

                    });

                currentDiscTitles.Add(sideTitles);
            }
        }


        // Collection/DVD/Actors/*
        private void HandleActors(XmlTextReader reader)
        {
            if (reader.LocalName == "Actor")
            {
                string role = "";
                if (reader.MoveToAttribute("Role")) role = reader.Value;
                string fullName = ReadFullName(reader);
                if (!string.IsNullOrEmpty(fullName)) // Skip dividers
                {
                    if (CurrentTitle.ActingRoles.ContainsKey(fullName))
                    {
                        string currentRole = CurrentTitle.ActingRoles[fullName];
                        string newRole = currentRole;
                        if (string.IsNullOrEmpty(currentRole))
                        {
                            newRole = role;
                        }
                        else if (!currentRole.Contains(role))
                        {
                            newRole = currentRole + "/" + role;
                        }
                        CurrentTitle.ActingRoles[fullName] = newRole;
                    }
                    else
                    {
                        CurrentTitle.ActingRoles.Add(fullName, role);
                    }
                }
            }
        }

        // Collection/DVD/Credits/*
        private void HandleCredits(XmlTextReader reader)
        {
            if (reader.LocalName == "Credit")
            {
                string creditType = null;
                if (reader.MoveToAttribute("CreditType")) creditType = reader.Value;
                Person person = new Person(ReadFullName(reader));
                switch (creditType)
                {
                    case "Direction":
                        CurrentTitle.Directors.Add(person);
                        break;
                    case "Writing":
                        if (!currentWritersAlreadyAdded.Contains(person.full_name))
                        {
                            CurrentTitle.Writers.Add(person);
                            currentWritersAlreadyAdded.Add(person.full_name);
                        }
                        break;
                    case "Production":
                        if (!currentProducersAlreadyAdded.Contains(person.full_name))
                        {
                            CurrentTitle.Producers.Add(person.full_name);
                            currentProducersAlreadyAdded.Add(person.full_name);
                        }
                        break;
                }
            }
        }


        // Collection/DVD/Audio/*
        private void HandleAudio(XmlTextReader reader)
        {
            if (reader.LocalName == "AudioTrack")
            {
                string content = null;
                string format = null;

                ReadChildElements(reader,
                    delegate
                    {
                        switch (reader.LocalName)
                        {
                            case "AudioContent":
                                content = reader.ReadElementString();
                                break;
                            case "AudioFormat":
                                format = reader.ReadElementString();
                                break;
                        }
                    });

                // Move both into content, leaving it null if none of them are set
                if (string.IsNullOrEmpty(content)) content = format;
                else if (!string.IsNullOrEmpty(format)) content += " (" + format + ")";

                if (!string.IsNullOrEmpty(content)) CurrentTitle.AddAudioTrack(content);
            }
        }

        // Collection/DVD/Format/*
        private void HandleFormat(XmlTextReader reader)
        {
            switch (reader.LocalName)
            {
                case "FormatAspectRatio":
                    CurrentTitle.AspectRatio = reader.ReadElementString();
                    break;
                case "FormatVideoStandard":
                    CurrentTitle.VideoStandard = reader.ReadElementString();
                    break;
                case "FormatLetterBox":
                case "FormatPanAndScan":
                case "FormatFullFrame":
                    if (string.IsNullOrEmpty(CurrentTitle.AspectRatio))
                    {
                        CurrentTitle.AspectRatio = "1.33";
                    }
                    break;
            }
        }

        // Collection/DVD/MediaTypes/*
        private void HandleMediaTypes(XmlTextReader reader)
        {
            string name = reader.LocalName;
            string value = reader.ReadElementString();
            if (string.Compare(value, "true", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                switch (name)
                {
                    case "HDDVD":
                        currentVideoFormat = VideoFormat.HDDVD;
                        break;
                    case "BluRay":
                        currentVideoFormat = VideoFormat.BLURAY;
                        break;
                }
            }
        }



        private static bool TryReadElementDate(XmlReader reader, DateTimeKind dateTimeKind, out DateTime date)
        {
            DateTimeStyles style = dateTimeKind == DateTimeKind.Local
                                       ? DateTimeStyles.AssumeLocal
                                       : DateTimeStyles.AssumeUniversal;
            string dateString = reader.ReadElementString();
            bool result = DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, style | DateTimeStyles.NoCurrentDateDefault, out date);
            if (result && date.Year < 1900) // DVD Profiler tend to use 1899-12-30 for no date
            {
                date = DateTime.MinValue;
                result = false;
            }
            return result;
        }

        private static bool TryReadElementInt(XmlReader reader, out int number)
        {
            return int.TryParse(reader.ReadElementString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out number);
        }

        private static string ReadFullName(XmlReader reader)
        {
            StringBuilder result = new StringBuilder();

            if (reader.MoveToAttribute("FirstName"))
            {
                if (!string.IsNullOrEmpty(reader.Value)) result.Append(reader.Value);
            }
            if (reader.MoveToAttribute("MiddleName"))
            {
                if (!string.IsNullOrEmpty(reader.Value))
                {
                    if (result.Length > 0) result.Append(" ");
                    result.Append(reader.Value);
                }
            }
            if (reader.MoveToAttribute("LastName"))
            {
                if (!string.IsNullOrEmpty(reader.Value))
                {
                    if (result.Length > 0) result.Append(" ");
                    result.Append(reader.Value);
                }
            }
            if (reader.MoveToAttribute("BirthYear"))
            {
                if (!string.IsNullOrEmpty(reader.Value) && reader.Value != "0")
                {
                    if (result.Length > 0) result.Append(" ");
                    result.AppendFormat("({0})", reader.Value);
                }
            }
            return result.ToString();
        }



        private delegate void ElementHandlerDelegate(XmlTextReader reader);


        private static void ReadChildElements(XmlTextReader reader, ElementHandlerDelegate elementHandler)
        {
            if (!reader.IsStartElement()) throw new NotSupportedException("Reading the child nodes of an XML node of type " + reader.NodeType + " is not supported.");
            int currentDepth = reader.Depth;
            reader.Read();
            reader.MoveToContent();
            if (currentDepth >= reader.Depth) return; // node had no child nodes


            while (!reader.EOF && reader.Depth > currentDepth)
            {
                int currentLine = reader.LineNumber;
                int currentCol = reader.LinePosition;


                if (reader.NodeType == XmlNodeType.Element && string.IsNullOrEmpty(reader.NamespaceURI))
                {
                    elementHandler(reader);
                }
                else
                {
                    reader.Skip();
                }


                // Advance the reader if the handler didn't do it
                if (currentLine == reader.LineNumber && currentCol == reader.LinePosition) reader.Skip();

                // Read back to the same level if the handler didn't do it
                while (reader.Depth > currentDepth + 1) reader.Skip();
            }
        }

        private void NextTitle()
        {
            if (currentTitle != null)
            {
                MergeDiscInfo();

                GetImagesForNewTitle(currentTitle);
                if (ValidateTitle(currentTitle))
                {
                    try { AddTitle(currentTitle); }
                    catch (Exception e) { Trace.WriteLine("Error adding row: " + e.Message); }
                }
                else Trace.WriteLine("Error saving row");
            }
        }

        /// <summary>
        /// Merges the information from the Notes field with the DVD Profiler Disc data to create the Disk entries for the title
        /// </summary>
        private void MergeDiscInfo()
        {
            if (string.IsNullOrEmpty(currentNotes)) return;

            var matches = filepathRegex.Matches(currentNotes);
            foreach (Match m in matches)
            {
                string discNumberString = m.Groups["discNumber"].Value;
                int discNumber = Convert.ToInt32(string.IsNullOrEmpty(discNumberString) ? "1" : discNumberString,
                                                 CultureInfo.InvariantCulture);
                string sideString = m.Groups["side"].Value;
                int side = 0;
                if (!string.IsNullOrEmpty(sideString) && sideString.ToLowerInvariant() == "b") side = 1;

                string title;
                if (currentDiscTitles.Count >= discNumber) title = currentDiscTitles[discNumber - 1][side];
                else
                {
                    // TODO: I18N 
                    if (matches.Count == 1) title = "Movie";
                    else title = "Disc " + discNumber + (side == 0 ? "a" : "b");
                }
                string path = m.Groups["path"].Value;
                currentVideoFormat = GetFormatFromExtension(Path.GetExtension(path), currentVideoFormat);

                if (!string.IsNullOrEmpty(path))
                {
                    // validate that the path to the file/directory is valid
                    if (!File.Exists(path) && !Directory.Exists(path))
                    {
                        this.AddError("Invalid path \"" + path + "\" for movie \"" + CurrentTitle.Name + "\"");
                    }
                }
                
                CurrentTitle.AddDisk(new Disk(title, path, currentVideoFormat));                
            }
        }

        private static VideoFormat GetFormatFromExtension(string extension, VideoFormat defaultFormat)
        {
            if (string.IsNullOrEmpty(extension)) return defaultFormat;
            extension = extension.TrimStart('.').ToLowerInvariant();

            foreach (VideoFormat format in Enum.GetValues(typeof(VideoFormat)))
            {
                if (Enum.GetName(typeof(VideoFormat), format).ToLowerInvariant() == extension)
                {
                    return format;
                }
            }
            return defaultFormat;
        }


        private void ClearImportState()
        {
            currentTitle = null;
            currentVideoFormat = VideoFormat.DVD;
            currentDiscTitles.Clear();
            currentNotes = null;
            currentWritersAlreadyAdded.Clear();
            currentProducersAlreadyAdded.Clear();
        }


        private void GetImagesForNewTitle(Title newTitle)
        {
            string id = newTitle.MetadataSourceID;
            if (!string.IsNullOrEmpty(id))
            {
                InitializeImagesPath();
                if (imagesPath != null && Directory.Exists(imagesPath))
                {
                    if (File.Exists(Path.Combine(imagesPath, string.Format("{0}f.{1}", id, @"jpg"))))
                    {
                        newTitle.FrontCoverPath = Path.Combine(imagesPath, string.Format("{0}f.{1}", id, @"jpg"));
                    }

                    if (File.Exists(Path.Combine(imagesPath, string.Format("{0}b.{1}", id, @"jpg"))))
                    {
                        newTitle.BackCoverPath = Path.Combine(imagesPath, string.Format("{0}b.{1}", id, @"jpg"));
                    }
                }
            }
        }

        private void InitializeImagesPath()
        {
            if (!string.IsNullOrEmpty(imagesPath)) return;

            XmlTextReader dvdProfilerSettings;
            if (File.Exists(Path.Combine(FileSystemWalker.PluginsDirectory, "DVDProfilerSettings.xml")))
            {
                try
                {
                    using (dvdProfilerSettings = new XmlTextReader(Path.Combine(FileSystemWalker.PluginsDirectory, "DVDProfilerSettings.xml")))
                    {

                        while (dvdProfilerSettings.Read())
                        {
                            if (dvdProfilerSettings.NodeType == XmlNodeType.Element &&
                                dvdProfilerSettings.Name == "imagesPath")
                            {
                                imagesPath = (dvdProfilerSettings.ReadInnerXml());
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(imagesPath)) Console.WriteLine("Missing Database location variable in DvdProfilerSettings.xml file, as a result images will not be imported");
                }
                catch (Exception e)
                {
                    Utilities.DebugLine("[DVDProfilerImporter] Unable to open DVDProfilerSettings.xml file: {0}", e.Message);
                }
            }
        }
    }
}
