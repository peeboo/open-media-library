using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using OMLEngine;
using OMLSDK;

namespace DVDProfilerPlugin
{
    // [CLSCompliant(true)] // this requires CLSCompliant assemby attribute
    public class DVDProfilerImporter : OMLPlugin
    {
        private static readonly Regex filepathRegex = new Regex(@"\[filepath((\s+disc\s*=\s*(?'discNumber'\d+)(?'side'[ab])?))?\s*\](?'path'[^\[]+)\[\s*\/\s*filepath\s*\]", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex removeFormattingRegex = new Regex(@"<\/?(i|b)>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private static readonly Regex removeExtraLinebreaksRegex = new Regex(@"(\s*(\r|\n)\s*)+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private List<TagDefinition> tagDefinitions = new List<TagDefinition>();
        private List<Exception> initializationErrors = new List<Exception>();

        string imagesPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="DVDProfilerImporter"/> class.
        /// </summary>
        public DVDProfilerImporter()
        {
            InitializeSettings(null);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="DVDProfilerImporter"/> class for use in UnitTesting.
        /// </summary>
        /// <param name="settingsXml">An <see cref="XmlTextReader"/> containing the settings to use.</param>
        internal DVDProfilerImporter(XmlTextReader settingsXml)
        {
            InitializeSettings(settingsXml);
        }

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

        public override void ProcessFile(string file)
        {
            foreach (Exception ex in initializationErrors)
            {
                AddError(ex.Message);
            }
            using (XmlTextReader reader = new XmlTextReader(file))
            {
                var navigator = new XPathDocument(reader).CreateNavigator();
                foreach (XPathNavigator dvd in navigator.Select("/Collection/DVD|/DVD")) // Allow both collection file and single profile export
                {
                    HandleDvd(dvd);
                }
            }
        }

        private void HandleDvd(XPathNavigator dvdNavigator)
        {
            Title title = LoadTitle(dvdNavigator, true);
            GetImagesForNewTitle(title);
            if (ValidateTitle(title))
            {
                try { AddTitle(title); }
                catch (Exception e) { Trace.WriteLine("Error adding row: " + e.Message); }
            }
            else Trace.WriteLine("Error saving row");
        }

        public Title LoadTitle(XPathNavigator dvdNavigator, bool lookForDiskInfo)
        {
            Title title = new Title();
            VideoFormat videoFormat = VideoFormat.DVD;
            string notes = string.Empty;
            var discs = new Dictionary<string, string>(); // The key is on the form "1a", "10b", etc - the value is the description

            foreach (XPathNavigator dvdElement in dvdNavigator.SelectChildren(XPathNodeType.Element))
            {
                switch (dvdElement.Name)
                {
                    case "ID":
                        title.MetadataSourceID = dvdElement.Value;
                        break;
                    case "MediaTypes":
                        videoFormat = HandleMediaTypes(dvdElement);
                        break;
                    case "UPC":
                        title.UPC = dvdElement.Value;
                        break;
                    case "Title":
                        title.Name = dvdElement.Value;
                        break;
                    case "OriginalTitle":
                        title.OriginalName = dvdElement.Value;
                        break;
                    case "SortTitle":
                        title.SortName = dvdElement.Value;
                        break;
                    case "CountryOfOrigin":
                        title.CountryOfOrigin = dvdElement.Value;
                        break;
                    case "ProductionYear":
                        // Set to January first in the appropriate year in lack of better...
                        int releaseYear;
                        if (TryReadItemInt(dvdElement, out releaseYear))
                        {
                            title.ReleaseDate = new DateTime(releaseYear, 1, 1);
                        }
                        break;
                    case "RunningTime":
                        int runningTime;
                        if (TryReadItemInt(dvdElement, out runningTime)) title.Runtime = runningTime;
                        break;
                    case "Rating":
                        title.ParentalRating = dvdElement.Value;
                        break;
                    case "RatingDetails":
                        title.ParentalRatingReason = dvdElement.Value;
                        break;
                    case "Genres":
                        foreach (XPathNavigator genreNavigator in dvdElement.Select("Genre[. != '']"))
                        {
                            title.AddGenre(genreNavigator.Value);
                        }
                        break;
                    case "Format":
                        HandleFormat(title, dvdElement);
                        break;
                    case "Studios":
                        var studioNavigator = dvdElement.SelectSingleNode("Studio[. != '']");
                        if (studioNavigator != null) title.Studio = studioNavigator.Value;
                        break;
                    case "Audio":
                        HandleAudio(title, dvdElement);
                        break;
                    case "Subtitles":
                        foreach (XPathNavigator subtitleNavigator in dvdElement.SelectChildren("Subtitle", string.Empty))
                        {
                            title.AddSubtitle(subtitleNavigator.Value);
                        }
                        break;
                    case "Actors":
                        HandleActors(title, dvdElement);
                        break;
                    case "Credits":
                        HandleCredits(title, dvdElement);
                        break;
                    case "Review":
                        ReadFilmReview(title, dvdElement);
                        break;
                    case "Overview":
                        string synopsis = removeFormattingRegex.Replace(dvdElement.Value, string.Empty);
                        title.Synopsis = removeExtraLinebreaksRegex.Replace(synopsis, "\r\n").Trim();
                        break;
                    case "Discs":
                        discs = HandleDiscs(dvdElement);
                        break;
                    case "Notes":
                        notes = dvdElement.Value;
                        break;
                    case "PurchaseInfo":
                        HandlePurchaseInfo(title, dvdElement);
                        break;
                }
            }
            if (lookForDiskInfo)
                MergeDiscInfo(title, videoFormat, discs, notes);

            ApplyTags(title, dvdNavigator);
            return title;
        }

        private void ReadFilmReview(Title title, XPathNavigator reviewNavigator)
        {
            switch (reviewNavigator.GetAttribute("Film", String.Empty))
            {
                case "9": title.UserStarRating = 100; break;
                case "8": title.UserStarRating = 90; break;
                case "7": title.UserStarRating = 80; break;
                case "6": title.UserStarRating = 70; break;
                case "5": title.UserStarRating = 60; break;
                case "4": title.UserStarRating = 50; break;
                case "3": title.UserStarRating = 40; break;
                case "2": title.UserStarRating = 20; break; // Have to skip somewhere to avoid fractions :(
                case "1": title.UserStarRating = 10; break;
            }
        }

        //Collection/DVD/Discs/*
        private Dictionary<string, string> HandleDiscs(XPathNavigator discsNavigator)
        {
            var discs = new Dictionary<string, string>();
            int currentDiscId = 0;
            foreach (XPathNavigator descriptionNavigator in discsNavigator.Select("Disc/DescriptionSideA|Disc/DescriptionSideB"))
            {
                string description = descriptionNavigator.Value;
                string side = descriptionNavigator.Name.EndsWith("B") ? "b" : "a";
                if (side == "a") currentDiscId++;

                discs.Add(currentDiscId.ToString(CultureInfo.InvariantCulture) + side, description);
            }
            return discs;
        }


        // Collection/DVD/Actors/*
        private void HandleActors(Title title, XPathNavigator actorsNavigator)
        {
            foreach (XPathNavigator actorNavigator in actorsNavigator.SelectChildren("Actor", string.Empty))
            {
                string role = actorNavigator.GetAttribute("Role", string.Empty) ?? string.Empty;
                string fullName = ReadFullName(actorNavigator);
                if (!string.IsNullOrEmpty(fullName)) // Skip dividers
                {
                    if (title.ActingRoles.ContainsKey(fullName))
                    {
                        string currentRole = title.ActingRoles[fullName];
                        string newRole = currentRole;
                        if (string.IsNullOrEmpty(currentRole))
                        {
                            newRole = role;
                        }
                        else if (!currentRole.Contains(role))
                        {
                            newRole = currentRole + "/" + role;
                        }
                        title.ActingRoles[fullName] = newRole;
                    }
                    else
                    {
                        title.ActingRoles.Add(fullName, role);
                    }
                }
            }
        }

        // Collection/DVD/Credits/*
        private void HandleCredits(Title title, XPathNavigator creditsNavigator)
        {
            List<string> directorsAlreadyAdded = new List<string>();
            List<string> writersAlreadyAdded = new List<string>();
            List<string> producersAlreadyAdded = new List<string>();

            foreach (XPathNavigator creditNavigator in creditsNavigator.SelectChildren("Credit", string.Empty))
            {
                string creditType = creditNavigator.GetAttribute("CreditType", string.Empty);
                Person person = new Person(ReadFullName(creditNavigator));
                switch (creditType)
                {
                    case "Direction":
                        if (!directorsAlreadyAdded.Contains(person.full_name.ToUpperInvariant()))
                        {
                            title.Directors.Add(person);
                            directorsAlreadyAdded.Add(person.full_name.ToUpperInvariant());
                        }
                        break;
                    case "Writing":
                        if (!writersAlreadyAdded.Contains(person.full_name.ToUpperInvariant()))
                        {
                            title.Writers.Add(person);
                            writersAlreadyAdded.Add(person.full_name.ToUpperInvariant());
                        }
                        break;
                    case "Production":
                        if (!producersAlreadyAdded.Contains(person.full_name.ToUpperInvariant()))
                        {
                            title.Producers.Add(person.full_name);
                            producersAlreadyAdded.Add(person.full_name.ToUpperInvariant());
                        }
                        break;
                }
            }
        }


        // Collection/DVD/Audio/*
        private void HandleAudio(Title title, XPathNavigator audioTracksNavigator)
        {
            foreach (XPathNavigator audioTrackNavigator in audioTracksNavigator.SelectChildren("AudioTrack", string.Empty))
            {
                string content = null;
                string format = null;
                foreach (XPathNavigator audioTrackElementNavigator in audioTrackNavigator.SelectChildren(string.Empty, string.Empty))
                {
                    switch (audioTrackElementNavigator.Name)
                    {
                        case "AudioContent":
                            content = audioTrackElementNavigator.Value;
                            break;
                        case "AudioFormat":
                            format = audioTrackElementNavigator.Value;
                            break;
                    }
                }

                // Move both into content, leaving it null if none of them are set
                if (string.IsNullOrEmpty(content)) content = format;
                else if (!string.IsNullOrEmpty(format)) content += " (" + format + ")";

                if (!string.IsNullOrEmpty(content)) title.AddAudioTrack(content);
            }
        }

        // Collection/DVD/Format/*
        private void HandleFormat(Title title, XPathNavigator formatNavigator)
        {
            foreach (XPathNavigator formatChildElementNavigator in formatNavigator.SelectChildren(XPathNodeType.Element))
            {
                switch (formatChildElementNavigator.Name)
                {
                    case "FormatAspectRatio":
                        title.AspectRatio = formatChildElementNavigator.Value;
                        break;
                    case "FormatVideoStandard":
                        title.VideoStandard = formatChildElementNavigator.Value;
                        break;
                    case "FormatLetterBox":
                    case "FormatPanAndScan":
                    case "FormatFullFrame":
                        if (string.IsNullOrEmpty(title.AspectRatio))
                        {
                            title.AspectRatio = "1.33";
                        }
                        break;
                }
            }
        }

        // Collection/DVD/MediaTypes/*
        private VideoFormat HandleMediaTypes(XPathNavigator mediaTypesNavigator)
        {
            XPathNavigator selectedMedia = mediaTypesNavigator.SelectSingleNode("*[.='True']");
            if (selectedMedia != null)
            {
                switch (selectedMedia.Name)
                {
                    case "HDDVD":
                        return VideoFormat.HDDVD;
                    case "BluRay":
                        return VideoFormat.BLURAY;
                }
            }
            return VideoFormat.DVD;
        }

        // Collection/DVD/PurchaseInfo/*
        private void HandlePurchaseInfo(Title title, XPathNavigator dvdElement)
        {
            XPathNavigator purchaseDateNavigator = dvdElement.SelectSingleNode("PurchaseDate");
            if (purchaseDateNavigator != null)
            {
                DateTime purchaseDate;
                if (TryReadItemDate(purchaseDateNavigator, DateTimeKind.Local, out purchaseDate))
                {
                    title.DateAdded = purchaseDate;
                }
            }
        }


        private static bool TryReadItemDate(XPathItem item, DateTimeKind dateTimeKind, out DateTime date)
        {
            DateTimeStyles style = dateTimeKind == DateTimeKind.Local
                                       ? DateTimeStyles.AssumeLocal
                                       : DateTimeStyles.AssumeUniversal;
            string dateString = item.Value;
            bool result = DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, style | DateTimeStyles.NoCurrentDateDefault, out date);
            if (result && date.Year < 1900) // DVD Profiler tend to use 1899-12-30 for no date
            {
                date = DateTime.MinValue;
                result = false;
            }
            return result;
        }

        private static bool TryReadItemInt(XPathItem item, out int number)
        {
            return int.TryParse(item.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out number);
        }

        private static string ReadFullName(XPathNavigator personNavigator)
        {
            StringBuilder result = new StringBuilder();

            string firstName = personNavigator.GetAttribute("FirstName", string.Empty);
            if (!string.IsNullOrEmpty(firstName)) result.Append(firstName);

            string middleName = personNavigator.GetAttribute("MiddleName", string.Empty);
            if (!string.IsNullOrEmpty(middleName))
            {
                if (result.Length > 0) result.Append(" ");
                result.Append(middleName);
            }

            string lastName = personNavigator.GetAttribute("LastName", string.Empty);
            if (!string.IsNullOrEmpty(lastName))
            {
                if (result.Length > 0) result.Append(" ");
                result.Append(lastName);
            }

            string birthYear = personNavigator.GetAttribute("BirthYear", string.Empty);
            if (!string.IsNullOrEmpty(birthYear) && birthYear != "0")
            {
                if (result.Length > 0) result.Append(" ");
                result.AppendFormat("({0})", birthYear);
            }
            return result.ToString();
        }

        /// <summary>
        /// Merges the information from the Notes field with the DVD Profiler Disc data to create the Disk entries for the title
        /// </summary>
        private void MergeDiscInfo(Title title, VideoFormat dvdProfilerVideoFormat, Dictionary<string, string> discs, string notes)
        {
            if (string.IsNullOrEmpty(notes)) return;

            var matches = filepathRegex.Matches(notes);
            int lastDiskNumber = 0;
            foreach (Match m in matches)
            {
                string discNumberString = m.Groups["discNumber"].Value;
                int discNumber = Convert.ToInt32(string.IsNullOrEmpty(discNumberString) ? "0" : discNumberString,
                                                 CultureInfo.InvariantCulture);
                if (discNumber == 0) discNumber = lastDiskNumber + 1;
                string sideString = m.Groups["side"].Value;
                if (string.IsNullOrEmpty(sideString)) sideString = "a";

                string description;
                string discKey = discNumber.ToString(CultureInfo.InvariantCulture) + sideString.ToLowerInvariant();
                if (!discs.TryGetValue(discKey, out description))
                {
                    // TODO: I18N 
                    if (matches.Count == 1) description = "Movie";
                    else description = "Disc " + discKey;
                }
                string path = m.Groups["path"].Value;
                VideoFormat discVideoFormat = GetFormatFromExtension(Path.GetExtension(path), dvdProfilerVideoFormat);

                if (!string.IsNullOrEmpty(path))
                {
                    // validate that the path to the file/directory is valid
                    if (!File.Exists(path) && !Directory.Exists(path))
                    {
                        this.AddError("Invalid path \"" + path + "\" for movie \"" + title.Name + "\"");
                    }
                }

                title.AddDisk(new Disk(description, path, discVideoFormat));
                lastDiskNumber++;
            }
        }

        private void ApplyTags(Title title, XPathNavigator dvdNavigator)
        {
            foreach (TagDefinition tagDefinition in tagDefinitions)
            {
                bool match = dvdNavigator.SelectSingleNode(tagDefinition.XPath) != null;
                if (match && !string.IsNullOrEmpty(tagDefinition.Name)) title.Tags.Add(tagDefinition.Name);
                if (!match && !string.IsNullOrEmpty(tagDefinition.ExcludedName)) title.Tags.Add(tagDefinition.ExcludedName);
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

        public void GetImagesForNewTitle(Title newTitle)
        {
            string id = newTitle.MetadataSourceID;
            if (!string.IsNullOrEmpty(id))
            {
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

        private class TagDefinition
        {
            public string Name { get; set; }
            public string ExcludedName { get; set; }
            public XPathExpression XPath { get; set; }
        }

        private void InitializeSettings(XmlTextReader settingsXml)
        {
            XmlTextReader dvdProfilerSettings = null;
            bool disposeReader = false;
            try
            {
                if (settingsXml != null)
                {
                    dvdProfilerSettings = settingsXml;
                }
                else
                {
                    string path = Path.Combine(FileSystemWalker.PluginsDirectory, "DVDProfilerSettings.xml");
                    if (File.Exists(path))
                    {
                        disposeReader = true;
                        dvdProfilerSettings = new XmlTextReader(path);
                    }
                }


                if (dvdProfilerSettings != null)
                {
                    while (dvdProfilerSettings.Read())
                    {
                        if (dvdProfilerSettings.NodeType != XmlNodeType.Element) continue;
                        switch (dvdProfilerSettings.Name)
                        {
                            case "imagesPath":
                                imagesPath = (dvdProfilerSettings.ReadInnerXml());
                                break;
                            case "tag":
                                string tagName = dvdProfilerSettings.GetAttribute("name");
                                string tagExcludedName = dvdProfilerSettings.GetAttribute("excludedName");
                                string tagXPath = dvdProfilerSettings.GetAttribute("xpath");
                                if (string.IsNullOrEmpty(tagName) && string.IsNullOrEmpty(tagExcludedName))
                                {
                                    Utilities.DebugLine("[DVDProfilerImporter] Tag setting missing name");
                                    AddError("Tag missing name or excludeName attribute in DVDProfilerSettings.xml");
                                }
                                if (string.IsNullOrEmpty(tagXPath))
                                {
                                    Utilities.DebugLine("[DVDProfilerImporter] Tag setting missing xpath");
                                    AddError("Tag missing xpath attribute in DVDProfilerSettings.xml");
                                }
                                if (!string.IsNullOrEmpty(tagXPath))
                                {
                                    try
                                    {
                                        tagDefinitions.Add(new TagDefinition
                                                               {
                                                                   Name = tagName,
                                                                   ExcludedName = tagExcludedName,
                                                                   XPath = XPathExpression.Compile(tagXPath)
                                                               });
                                    }
                                    catch (ArgumentException e)
                                    {
                                        initializationErrors.Add(e);
                                    }
                                    catch (XPathException e)
                                    {
                                        initializationErrors.Add(e);
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Utilities.DebugLine("[DVDProfilerImporter] Unable to open DVDProfilerSettings.xml file: {0}",
                                    e.Message);
                initializationErrors.Add(e);
            }
            finally
            {
                if (disposeReader && dvdProfilerSettings != null) dvdProfilerSettings.Close();
            }
        }
    }
}
