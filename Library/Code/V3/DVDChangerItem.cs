using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Xml.Serialization;
using Microsoft.MediaCenter.UI;
using System.Threading;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using System.Security;
using System.Reflection;
using System.Net;

namespace Library.Code.V3
{
    public class DVDChangerItem : GalleryItem
    {
        public void PlayMovie()
        {
            OMLEngine.Disk disk = new OMLEngine.Disk();
            disk.Name = this.Description;
            disk.Format = OMLEngine.VideoFormat.DVD;
            disc.Load();

            disk.Path = string.Format("{0}\\Video_TS", disc.DrivePath);//get the path from the changer...
            var ms = new OMLEngine.MediaSource(disk);
            //ms.OnSave += new Action<MediaSource>(ms_OnSave);
            IPlayMovie moviePlayer = MoviePlayerFactory.CreateMoviePlayer(ms);
            moviePlayer.PlayMovie();
        }


        private string GetNodeInnerText(XmlDocument doc, string path)
        {
            string retString = null;
            try
            {
                XmlNode node = doc.SelectSingleNode(path);
                if (node != null)
                    retString = node.InnerText;
            }
            catch
            {
            }
            return retString;
        }

        private void AddComplete(IAsyncResult itfAR)
        {
            DvdInfo dvd = DvdInfo.EndGetDvdInfo(itfAR);
            this.DefaultImage = new Image(dvd.LargeCoverUrl);
            this.SimpleVideoFormat = "DVD";
            if (!string.IsNullOrEmpty(dvd.ReleaseDate))
                this.MetadataTop = dvd.ReleaseDate;
            else
                this.MetadataTop = "";

            this.ItemId = 1;
            //string dvdStarRating = GetNodeInnerText(dvdDoc, "/METADATA/MDR-DVD/title/providerRating");//dvdDoc.SelectSingleNode("/METADATA/MDR-DVD/title/providerRating").InnerText;
            //if(string.IsNullOrEmpty(dvdStarRating))
            //    dvdStarRating="0";
            ////string starRating = Convert.ToString(Math.Round((Convert.ToDouble(dvdStarRating) * 0.8), MidpointRounding.AwayFromZero));
            //this.StarRating = dvdStarRating;
            string extendedMetadata = string.Empty;

            this.Metadata = dvd.MPAARating;
            if (string.IsNullOrEmpty(dvd.MPAARating))
                this.Metadata = "Not Rated";

            if (!string.IsNullOrEmpty(dvd.Duration))
                this.Metadata += string.Format(", {0} minutes", dvd.Duration);

            this.Tagline = dvd.Synopsis;

            this.Invoked += delegate(object sender, EventArgs args)
            {

                //to test v3
                Library.Code.V3.DetailsPage page = new Library.Code.V3.DetailsPage();
                //DataRow movieData = this.GetMovieData(movieId);

                page.Description = "movie details";
                page.Title = this.Description;
                page.Summary = dvd.Synopsis;
                page.Background = this.DefaultImage;
                page.Details = new Library.Code.V3.ExtendedDetails();

                page.Details.CastArray = new ArrayListDataSet();

                if (!string.IsNullOrEmpty(dvd.Director))
                {
                    int directorCount = 0;
                    string[] directors = dvd.Director.Replace("; ",";").Split(new char[] { ';' });
                    foreach (string director in directors)
                    {
                        Library.Code.V3.CastCommand directorCommand = new Library.Code.V3.CastCommand();
                        directorCommand.Role = " ";
                        directorCommand.Description = director;
                        if (directorCount == 0)
                        {
                            directorCommand.CastType = "TitleAndDesc";
                            directorCommand.GroupTitle = "DIRECTOR";
                            directorCommand.ActorId = 1;//not sure how this works in oml
                        }
                        else
                            directorCommand.CastType = "Desc";
                        page.Details.CastArray.Add(directorCommand);
                        directorCount++;
                    }

                    page.Details.Director = string.Format("Directed By: {0}", dvd.Director);
                }
                if (!string.IsNullOrEmpty(dvd.LeadPerformer))
                {
                    string[] cast = dvd.LeadPerformer.Replace("; ", ";").Split(new char[] { ';' });
                    int actorCount = 0;
                    foreach (string actor in cast)
                    {
                        Library.Code.V3.CastCommand actorCommand = new Library.Code.V3.CastCommand();
                        actorCommand.Description = actor;
                        actorCommand.ActorId = 1;//not sure how this works in oml
                        //actorCommand.Role = kvp.Value;//not sure about role here...
                        if (actorCount == 0)
                        {
                            //add the title "CAST"
                            actorCommand.CastType = "TitleAndDesc";
                            actorCommand.GroupTitle = "CAST";
                        }
                        else
                            actorCommand.CastType = "Desc";

                        page.Details.CastArray.Add(actorCommand);
                        actorCount++;
                    }

                 page.Details.Cast = string.Format("Cast Info: {0}", dvd.LeadPerformer);

                }
                //if (this.StarRating != "0")
                //    page.Details.StarRating = new Image(string.Format("resx://Library/Library.Resources/V3_Controls_Common_Stars_{0}", starRating));

                //strip invalid dates
                if (!string.IsNullOrEmpty(dvd.ReleaseDate))
                    page.Details.YearString = dvd.ReleaseDate;

                page.Details.Studio = dvd.Studio;


                page.Details.GenreRatingandRuntime = dvd.Genre;

                //fixes double spacing issues
                if(!string.IsNullOrEmpty(dvd.Synopsis))
                    page.Details.Summary = dvd.Synopsis.Replace("\r\n", "\n");
                page.Commands = new ArrayListDataSet(page);
                //default play command
                Command playCmd = new Command();
                playCmd.Description = "Play";
                playCmd.Invoked += new EventHandler(playCmd_Invoked);
                page.Commands.Add(playCmd);

                if (page.Details.CastArray.Count > 0)
                {
                    foreach (Library.Code.V3.CastCommand actor in page.Details.CastArray)
                    {
                        //actor.Invoked += new EventHandler(actor_Invoked);
                        //TitleCollection titles = new TitleCollection();
                        OMLEngine.TitleFilter personFilter = new OMLEngine.TitleFilter(OMLEngine.TitleFilterType.Person, actor.Description);
                        OMLEngine.TitleCollectionManager.GetFilteredTitles(new List<OMLEngine.TitleFilter>() { personFilter });
                    }

                    Command command = new Command();
                    command.Description = "Cast + More";

                    command.Invoked += delegate(object castSender, EventArgs castArgs)
                    {
                        Dictionary<string, object> castProperties = new Dictionary<string, object>();
                        castProperties["Page"] = page;
                        castProperties["Application"] = Library.OMLApplication.Current;
                        Library.OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_DetailsPageCastCrew", castProperties);
                    };

                    page.Commands.Add(command);
                }

                Dictionary<string, object> properties = new Dictionary<string, object>();
                properties["Page"] = page;
                properties["Application"] = Library.OMLApplication.Current;

                Library.OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_DetailsPage", properties);
            };

        }

        void playCmd_Invoked(object sender, EventArgs e)
        {
            this.PlayMovie();
        }
        public DVDChangerItem(DiscDataEx disc, IModelItem owner)
            : base(owner)
        {
            this.disc = disc;
            if (!string.IsNullOrEmpty(disc.Title))
                this.Description = disc.Title;
            else
                this.Description = disc.VolumeLabel;
                     
            //slow load the rest
            DvdInfo.BeginGetDvdInfo(disc.DiscId, new AsyncCallback(AddComplete), null); 
        }

        private DiscDataEx disc;
        private static void AppendSeparatedValue(ref string text, string value, string seperator)
        {
            if (String.IsNullOrEmpty(text))
                text = value;
            else
                text = string.Format("{0}{1}{2}", text, seperator, value);
        }

        private static string MetadataURL(bool bExtended)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("http://go.microsoft.com/fwlink/?LinkId=");
            string str = "68164";//<--extended//68162 MetaDataRedirector(bExtended);
            builder.Append(str);
            builder.AppendFormat("&clcid={0}", "0x409");
            builder.AppendFormat("&locale={0}&geoId=XX", CultureInfo.CurrentUICulture.LCID);
            builder.AppendFormat("&clientType={0}", "MCE");
            string str2 = "5.0";//(mCERegKey != null) ? (mCERegKey.GetValue("Ident", "5.0") as string) : "5.0";
            builder.AppendFormat("&clientVersion={0}", str2);
            builder.AppendFormat("&clientID={0}", 0x4d2);
            builder.Append("&DVDID=");
            return builder.ToString();
        }

        //changer test
        private DvdInfo GetDVDInfo(string DiscId)
        {
            //should not be hardcoded...
            //string URL = string.Format("http://metaservices.windowsmedia.com/pas_dvd_B/template/GetMDRDVDByCRC.xml?{0}", DiscId.Replace("|", ""));
            string URL = MetadataURL(true) + DiscId;
            System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(URL);
            webRequest.Method = "GET";
            webRequest.UserAgent = "MCML/2006";
            webRequest.ServicePoint.Expect100Continue = true;
            webRequest.Headers.Add("Accept-Language", "en-US");
            webRequest.KeepAlive = false;

            System.Net.HttpWebResponse webResponse = this.GetResponse(webRequest, 3);
            System.Xml.XmlDocument dvdDoc = new System.Xml.XmlDocument();
            

            XmlSerializer s = new XmlSerializer(typeof(DvdInfo));
            DvdInfo info = (DvdInfo)s.Deserialize(webResponse.GetResponseStream());
            info.DvdId = DiscId;
            //dvdDoc.Load(webResponse.GetResponseStream());
            webResponse.Close();
            return info;
        }

        private System.Net.HttpWebResponse GetResponse(System.Net.HttpWebRequest request, int retries)
        {
            try
            {
                request.Timeout = 1200000;
                return (System.Net.HttpWebResponse)request.GetResponse();
            }
            catch (Exception exception4)
            {
                if (retries < 1)
                {
                    throw exception4;
                }
                retries = retries - 1;
                return this.GetResponse(request, retries);
            }

        }
    }

    public class XmlSerializationReaderDvdInfo : XmlSerializationReader
    {
        // Fields
        private string id1_METADATA;
        private string id10_dvdTitle;
        private string id11_studio;
        private string id12_leadPerformer;
        private string id13_director;
        private string id14_MPAARating;
        private string id15_language;
        private string id16_releaseDate;
        private string id17_genre;
        private string id18_largeCoverParams;
        private string id19_smallCoverParams;
        private string id2_Item;
        private string id20_dataProvider;
        private string id21_duration;
        private string id22_title;
        private string id23_Title;
        private string id24_titleNum;
        private string id25_titleTitle;
        private string id26_synopsis;
        private string id27_chapter;
        private string id28_Chapter;
        private string id29_chapterNum;
        private string id3_DvdInfo;
        private string id30_chapterTitle;
        private string id4_MDRDVD;
        private string id5_NeedsAttribution;
        private string id6_DvdId;
        private string id7_MdrDvd;
        private string id8_MetadataExpires;
        private string id9_version;

        // Methods
        protected override void InitCallbacks()
        {
        }

        protected override void InitIDs()
        {
            this.id25_titleTitle = base.Reader.NameTable.Add("titleTitle");
            this.id11_studio = base.Reader.NameTable.Add("studio");
            this.id10_dvdTitle = base.Reader.NameTable.Add("dvdTitle");
            this.id3_DvdInfo = base.Reader.NameTable.Add("DvdInfo");
            this.id21_duration = base.Reader.NameTable.Add("duration");
            this.id19_smallCoverParams = base.Reader.NameTable.Add("smallCoverParams");
            //this.id16_releaseDate = base.Reader.NameTable.Add("releaseDate");
            this.id16_releaseDate = base.Reader.NameTable.Add("movieReleaseYear");
            this.id29_chapterNum = base.Reader.NameTable.Add("chapterNum");
            this.id6_DvdId = base.Reader.NameTable.Add("DvdId");
            this.id13_director = base.Reader.NameTable.Add("director");
            this.id30_chapterTitle = base.Reader.NameTable.Add("chapterTitle");
            this.id20_dataProvider = base.Reader.NameTable.Add("dataProvider");
            this.id14_MPAARating = base.Reader.NameTable.Add("MPAARating");
            this.id26_synopsis = base.Reader.NameTable.Add("synopsis");
            this.id23_Title = base.Reader.NameTable.Add("Title");
            this.id24_titleNum = base.Reader.NameTable.Add("titleNum");
            this.id2_Item = base.Reader.NameTable.Add("");
            this.id22_title = base.Reader.NameTable.Add("title");
            this.id8_MetadataExpires = base.Reader.NameTable.Add("MetadataExpires");
            this.id7_MdrDvd = base.Reader.NameTable.Add("MdrDvd");
            this.id28_Chapter = base.Reader.NameTable.Add("Chapter");
            this.id17_genre = base.Reader.NameTable.Add("genre");
            this.id9_version = base.Reader.NameTable.Add("version");
            this.id18_largeCoverParams = base.Reader.NameTable.Add("largeCoverParams");
            this.id4_MDRDVD = base.Reader.NameTable.Add("MDR-DVD");
            this.id15_language = base.Reader.NameTable.Add("language");
            this.id27_chapter = base.Reader.NameTable.Add("chapter");
            this.id1_METADATA = base.Reader.NameTable.Add("METADATA");
            this.id5_NeedsAttribution = base.Reader.NameTable.Add("NeedsAttribution");
            this.id12_leadPerformer = base.Reader.NameTable.Add("leadPerformer");
        }

        private Chapter Read2_Chapter(bool checkType)
        {
            Chapter chapter;
            XmlQualifiedName type = checkType ? base.GetXsiType() : null;
            if ((checkType && (type != null)) && ((type.Name != this.id28_Chapter) || (type.Namespace != this.id2_Item)))
            {
                throw base.CreateUnknownTypeException(type);
            }
            try
            {
                chapter = (Chapter)Activator.CreateInstance(typeof(Chapter), BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new object[0], null);
            }
            catch (MissingMethodException)
            {
                throw base.CreateInaccessibleConstructorException("global::MediaCenter.DVD.Chapter");
            }
            catch (SecurityException)
            {
                throw base.CreateCtorHasSecurityException("global::MediaCenter.DVD.Chapter");
            }
            bool[] flagArray = new bool[2];
            while (base.Reader.MoveToNextAttribute())
            {
                if (!base.IsXmlnsAttribute(base.Reader.Name))
                {
                    base.UnknownNode(chapter);
                }
            }
            base.Reader.MoveToElement();
            if (base.Reader.IsEmptyElement)
            {
                base.Reader.Skip();
                return chapter;
            }
            base.Reader.ReadStartElement();
            base.Reader.MoveToContent();
            int whileIterations = 0;
            int readerCount = base.ReaderCount;
            while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
            {
                if (base.Reader.NodeType == XmlNodeType.Element)
                {
                    if ((!flagArray[0] && (base.Reader.LocalName == this.id29_chapterNum)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        chapter.ChapterNumber = base.Reader.ReadElementString();
                        flagArray[0] = true;
                    }
                    else if ((!flagArray[1] && (base.Reader.LocalName == this.id30_chapterTitle)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        chapter.Name = base.Reader.ReadElementString();
                        flagArray[1] = true;
                    }
                    else
                    {
                        base.UnknownNode(chapter, ":chapterNum, :chapterTitle");
                    }
                }
                else
                {
                    base.UnknownNode(chapter, ":chapterNum, :chapterTitle");
                }
                base.Reader.MoveToContent();
                base.CheckReaderCount(ref whileIterations, ref readerCount);
            }
            base.ReadEndElement();
            return chapter;
        }

        private Title Read3_Title(bool checkType)
        {
            Title title;
            XmlQualifiedName type = checkType ? base.GetXsiType() : null;
            if ((checkType && (type != null)) && ((type.Name != this.id23_Title) || (type.Namespace != this.id2_Item)))
            {
                throw base.CreateUnknownTypeException(type);
            }
            try
            {
                title = (Title)Activator.CreateInstance(typeof(Title), BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new object[0], null);
            }
            catch (MissingMethodException)
            {
                throw base.CreateInaccessibleConstructorException("global::MediaCenter.DVD.Title");
            }
            catch (SecurityException)
            {
                throw base.CreateCtorHasSecurityException("global::MediaCenter.DVD.Title");
            }
            Chapter[] a = null;
            int length = 0;
            bool[] flagArray = new bool[9];
            while (base.Reader.MoveToNextAttribute())
            {
                if (!base.IsXmlnsAttribute(base.Reader.Name))
                {
                    base.UnknownNode(title);
                }
            }
            base.Reader.MoveToElement();
            if (base.Reader.IsEmptyElement)
            {
                base.Reader.Skip();
                title.Chapters = (Chapter[])base.ShrinkArray(a, length, typeof(Chapter), true);
                return title;
            }
            base.Reader.ReadStartElement();
            base.Reader.MoveToContent();
            int whileIterations = 0;
            int readerCount = base.ReaderCount;
            while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
            {
                if (base.Reader.NodeType == XmlNodeType.Element)
                {
                    if ((!flagArray[0] && (base.Reader.LocalName == this.id24_titleNum)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        title.TitleNumber = base.Reader.ReadElementString();
                        flagArray[0] = true;
                    }
                    else if ((!flagArray[1] && (base.Reader.LocalName == this.id25_titleTitle)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        title.Name = base.Reader.ReadElementString();
                        flagArray[1] = true;
                    }
                    else if ((!flagArray[2] && (base.Reader.LocalName == this.id11_studio)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        title.Studio = base.Reader.ReadElementString();
                        flagArray[2] = true;
                    }
                    else if ((!flagArray[3] && (base.Reader.LocalName == this.id13_director)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        title.Director = base.Reader.ReadElementString();
                        flagArray[3] = true;
                    }
                    else if ((!flagArray[4] && (base.Reader.LocalName == this.id12_leadPerformer)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        title.LeadPerformer = base.Reader.ReadElementString();
                        flagArray[4] = true;
                    }
                    else if ((!flagArray[5] && (base.Reader.LocalName == this.id14_MPAARating)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        title.MPAARating = base.Reader.ReadElementString();
                        flagArray[5] = true;
                    }
                    else if ((!flagArray[6] && (base.Reader.LocalName == this.id17_genre)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        title.Genre = base.Reader.ReadElementString();
                        flagArray[6] = true;
                    }
                    else if ((!flagArray[7] && (base.Reader.LocalName == this.id26_synopsis)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        title.Synopsis = base.Reader.ReadElementString();
                        flagArray[7] = true;
                    }
                    else if ((base.Reader.LocalName == this.id27_chapter) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        a = (Chapter[])base.EnsureArrayIndex(a, length, typeof(Chapter));
                        a[length++] = this.Read2_Chapter(true);
                    }
                    else
                    {
                        base.UnknownNode(title, ":titleNum, :titleTitle, :studio, :director, :leadPerformer, :MPAARating, :genre, :synopsis, :chapter");
                    }
                }
                else
                {
                    base.UnknownNode(title, ":titleNum, :titleTitle, :studio, :director, :leadPerformer, :MPAARating, :genre, :synopsis, :chapter");
                }
                base.Reader.MoveToContent();
                base.CheckReaderCount(ref whileIterations, ref readerCount);
            }
            title.Chapters = (Chapter[])base.ShrinkArray(a, length, typeof(Chapter), true);
            base.ReadEndElement();
            return title;
        }

        private MdrDvd Read4_MdrDvd(bool checkType)
        {
            MdrDvd dvd;
            XmlQualifiedName type = checkType ? base.GetXsiType() : null;
            if ((checkType && (type != null)) && ((type.Name != this.id7_MdrDvd) || (type.Namespace != this.id2_Item)))
            {
                throw base.CreateUnknownTypeException(type);
            }
            try
            {
                dvd = (MdrDvd)Activator.CreateInstance(typeof(MdrDvd), BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new object[0], null);
            }
            catch (MissingMethodException)
            {
                throw base.CreateInaccessibleConstructorException("global::MediaCenter.DVD.MdrDvd");
            }
            catch (SecurityException)
            {
                throw base.CreateCtorHasSecurityException("global::MediaCenter.DVD.MdrDvd");
            }
            Title[] a = null;
            int length = 0;
            bool[] flagArray = new bool[15];
            while (base.Reader.MoveToNextAttribute())
            {
                if (!base.IsXmlnsAttribute(base.Reader.Name))
                {
                    base.UnknownNode(dvd);
                }
            }
            base.Reader.MoveToElement();
            if (base.Reader.IsEmptyElement)
            {
                base.Reader.Skip();
                dvd.Titles = (Title[])base.ShrinkArray(a, length, typeof(Title), true);
                return dvd;
            }
            base.Reader.ReadStartElement();
            base.Reader.MoveToContent();
            int whileIterations = 0;
            int readerCount = base.ReaderCount;
            while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
            {
                if (base.Reader.NodeType == XmlNodeType.Element)
                {
                    if ((!flagArray[0] && (base.Reader.LocalName == this.id8_MetadataExpires)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        dvd.MetadataExpires = XmlSerializationReader.ToDateTime(base.Reader.ReadElementString());
                        flagArray[0] = true;
                    }
                    else if ((!flagArray[1] && (base.Reader.LocalName == this.id9_version)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        dvd.Version = base.Reader.ReadElementString();
                        flagArray[1] = true;
                    }
                    else if ((!flagArray[2] && (base.Reader.LocalName == this.id10_dvdTitle)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        dvd.Name = base.Reader.ReadElementString();
                        flagArray[2] = true;
                    }
                    else if ((!flagArray[3] && (base.Reader.LocalName == this.id11_studio)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        dvd.Studio = base.Reader.ReadElementString();
                        flagArray[3] = true;
                    }
                    else if ((!flagArray[4] && (base.Reader.LocalName == this.id12_leadPerformer)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        dvd.LeadPerformer = base.Reader.ReadElementString();
                        flagArray[4] = true;
                    }
                    else if ((!flagArray[5] && (base.Reader.LocalName == this.id13_director)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        dvd.Director = base.Reader.ReadElementString();
                        flagArray[5] = true;
                    }
                    else if ((!flagArray[6] && (base.Reader.LocalName == this.id14_MPAARating)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        dvd.MPAARating = base.Reader.ReadElementString();
                        flagArray[6] = true;
                    }
                    else if ((!flagArray[7] && (base.Reader.LocalName == this.id15_language)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        dvd.Language = base.Reader.ReadElementString();
                        flagArray[7] = true;
                    }
                    else if ((!flagArray[8] && (base.Reader.LocalName == this.id16_releaseDate)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        dvd.ReleaseDate = base.Reader.ReadElementString();
                        flagArray[8] = true;
                    }
                    else if ((!flagArray[9] && (base.Reader.LocalName == this.id17_genre)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        dvd.Genre = base.Reader.ReadElementString();
                        flagArray[9] = true;
                    }
                    else if ((!flagArray[10] && (base.Reader.LocalName == this.id18_largeCoverParams)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        dvd.LargeCoverUrl = base.Reader.ReadElementString();
                        flagArray[10] = true;
                    }
                    else if ((!flagArray[11] && (base.Reader.LocalName == this.id19_smallCoverParams)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        dvd.SmallCoverUrl = base.Reader.ReadElementString();
                        flagArray[11] = true;
                    }
                    else if ((!flagArray[12] && (base.Reader.LocalName == this.id20_dataProvider)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        dvd.DataProvider = base.Reader.ReadElementString();
                        flagArray[12] = true;
                    }
                    else if ((!flagArray[13] && (base.Reader.LocalName == this.id21_duration)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        dvd.Duration = base.Reader.ReadElementString();
                        flagArray[13] = true;
                    }
                    else if ((base.Reader.LocalName == this.id22_title) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        a = (Title[])base.EnsureArrayIndex(a, length, typeof(Title));
                        a[length++] = this.Read3_Title(true);
                    }
                    else
                    {
                        base.UnknownNode(dvd, ":MetadataExpires, :version, :dvdTitle, :studio, :leadPerformer, :director, :MPAARating, :language, :releaseDate, :genre, :largeCoverParams, :smallCoverParams, :dataProvider, :duration, :title");
                    }
                }
                else
                {
                    base.UnknownNode(dvd, ":MetadataExpires, :version, :dvdTitle, :studio, :leadPerformer, :director, :MPAARating, :language, :releaseDate, :genre, :largeCoverParams, :smallCoverParams, :dataProvider, :duration, :title");
                }
                base.Reader.MoveToContent();
                base.CheckReaderCount(ref whileIterations, ref readerCount);
            }
            dvd.Titles = (Title[])base.ShrinkArray(a, length, typeof(Title), true);
            base.ReadEndElement();
            return dvd;
        }

        private DvdInfo Read5_DvdInfo(bool isNullable, bool checkType)
        {
            XmlQualifiedName type = checkType ? base.GetXsiType() : null;
            bool flag = false;
            if (isNullable)
            {
                flag = base.ReadNull();
            }
            if ((checkType && (type != null)) && ((type.Name != this.id3_DvdInfo) || (type.Namespace != this.id2_Item)))
            {
                throw base.CreateUnknownTypeException(type);
            }
            if (flag)
            {
                return null;
            }
            DvdInfo o = new DvdInfo();
            bool[] flagArray = new bool[3];
            while (base.Reader.MoveToNextAttribute())
            {
                if (!base.IsXmlnsAttribute(base.Reader.Name))
                {
                    base.UnknownNode(o);
                }
            }
            base.Reader.MoveToElement();
            if (base.Reader.IsEmptyElement)
            {
                base.Reader.Skip();
                return o;
            }
            base.Reader.ReadStartElement();
            base.Reader.MoveToContent();
            int whileIterations = 0;
            int readerCount = base.ReaderCount;
            while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
            {
                if (base.Reader.NodeType == XmlNodeType.Element)
                {
                    if ((!flagArray[0] && (base.Reader.LocalName == this.id4_MDRDVD)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.MdrDvd = this.Read4_MdrDvd(true);
                        flagArray[0] = true;
                    }
                    else if ((!flagArray[1] && (base.Reader.LocalName == this.id5_NeedsAttribution)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.NeedsAttribution = XmlConvert.ToBoolean(base.Reader.ReadElementString());
                        flagArray[1] = true;
                    }
                    else if ((!flagArray[2] && (base.Reader.LocalName == this.id6_DvdId)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.DvdId = base.Reader.ReadElementString();
                        flagArray[2] = true;
                    }
                    else
                    {
                        base.UnknownNode(o, ":MDR-DVD, :NeedsAttribution, :DvdId");
                    }
                }
                else
                {
                    base.UnknownNode(o, ":MDR-DVD, :NeedsAttribution, :DvdId");
                }
                base.Reader.MoveToContent();
                base.CheckReaderCount(ref whileIterations, ref readerCount);
            }
            base.ReadEndElement();
            return o;
        }

        public object Read6_METADATA()
        {
            base.Reader.MoveToContent();
            if (base.Reader.NodeType == XmlNodeType.Element)
            {
                if ((base.Reader.LocalName != this.id1_METADATA) || (base.Reader.NamespaceURI != this.id2_Item))
                {
                    throw base.CreateUnknownNodeException();
                }
                return this.Read5_DvdInfo(true, true);
            }
            base.UnknownNode(null, ":METADATA");
            return null;
        }

        [StructLayout(LayoutKind.Sequential), ComVisible(false)]
        public struct Chapter
        {
            [XmlElement("chapterNum")]
            public string ChapterNumber;
            [XmlElement("chapterTitle")]
            public string Name;
        }

        [StructLayout(LayoutKind.Sequential), ComVisible(false)]
        public struct Title
        {
            [XmlElement("titleNum")]
            public string TitleNumber;
            [XmlElement("titleTitle")]
            public string Name;
            [XmlElement("studio")]
            public string Studio;
            [XmlElement("director")]
            public string Director;
            [XmlElement("leadPerformer")]
            public string LeadPerformer;
            [XmlElement("MPAARating")]
            public string MPAARating;
            [XmlElement("genre")]
            public string Genre;
            [XmlElement("synopsis")]
            public string Synopsis;
            [XmlElement("chapter")]
            public Chapter[] Chapters;
        }

        [StructLayout(LayoutKind.Sequential), ComVisible(false)]
        public struct MdrDvd
        {
            [XmlElement("MetadataExpires")]
            public DateTime MetadataExpires;
            [XmlElement("version")]
            public string Version;
            [XmlElement("dvdTitle")]
            public string Name;
            [XmlElement("studio")]
            public string Studio;
            [XmlElement("leadPerformer")]
            public string LeadPerformer;
            [XmlElement("director")]
            public string Director;
            [XmlElement("MPAARating")]
            public string MPAARating;
            [XmlElement("language")]
            public string Language;
            [XmlElement("releaseDate")]
            public string ReleaseDate;
            [XmlElement("genre")]
            public string Genre;
            [XmlElement("largeCoverParams")]
            public string LargeCoverUrl;
            [XmlElement("smallCoverParams")]
            public string SmallCoverUrl;
            [XmlElement("dataProvider")]
            public string DataProvider;
            [XmlElement("duration")]
            public string Duration;
            [XmlElement("title")]
            public Title[] Titles;
        }
    }



    [XmlRoot("METADATA"), ComVisible(false)]
    public class DvdInfo
    {
        // Fields
        private string _DvdId;
        private static bool createCoverPath = true;
        [XmlElement("MDR-DVD")]
        public Library.Code.V3.XmlSerializationReaderDvdInfo.MdrDvd MdrDvd;
        [XmlElement("NeedsAttribution")]
        public bool NeedsAttribution;
        private const string relativeCachePath = @"Microsoft\eHome";
        private const string relativeCoverCachePath = "DvdCoverCache";
        private const string relativeFileCachePath = "DvdInfoCache";
        private static SortedList s_currentRequests = new SortedList();
        private const string s_enhancedWebCache = ".enhanced.web.xml";
        private const double s_lifetimeForCacheObjects = 90.0;
        private static object s_lock = new object();
        private static string s_sBasePath = null;
        private const string s_webCache = ".web.xml";

        private static string MetadataURL(bool bExtended)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("http://go.microsoft.com/fwlink/?LinkId=");
            string str = "68164";//<--extended//68162 MetaDataRedirector(bExtended);
            builder.Append(str);
            builder.AppendFormat("&clcid={0}", "0x409");
            builder.AppendFormat("&locale={0}&geoId=XX", CultureInfo.CurrentUICulture.LCID);
            builder.AppendFormat("&clientType={0}", "MCE");
            string str2 = "5.0";//(mCERegKey != null) ? (mCERegKey.GetValue("Ident", "5.0") as string) : "5.0";
            builder.AppendFormat("&clientVersion={0}", str2);
            builder.AppendFormat("&clientID={0}", 0x4d2);
            builder.Append("&DVDID=");
            return builder.ToString();
        }

        // Methods
        private static IAsyncResult _BeginGetDvdInfo(DvdInfoRequestData dvdInfoRequestData, string dvdId, AsyncCallback callback, object callbackData, bool bUseEnhancedMetadata)
        {
            if (dvdInfoRequestData == null)
            {
                //if (WorkOffline)
                //{
                //    return null;
                //}
                if (string.IsNullOrEmpty(dvdId))
                {
                    return null;
                }
                dvdInfoRequestData = new DvdInfoRequestData(dvdId);
                if (!bUseEnhancedMetadata)
                {
                    s_currentRequests[dvdId] = dvdInfoRequestData;
                }
                //string strUrl = MetadataURLForDisc(dvdId, bUseEnhancedMetadata);
                string strUrl = MetadataURL(bUseEnhancedMetadata)+dvdId;//.Replace("|","");//string.Format(metMetadataURLForDisc(dvdId, bUseEnhancedMetadata);
                uint cbMaxSize = 0x19000;
                AsyncCallback callback2 = bUseEnhancedMetadata ? new AsyncCallback(DvdInfo.InetStreamCallbackEx) : new AsyncCallback(DvdInfo.InetStreamCallback);
                bool flag = false;
                try
                {
                    InetStream.BeginRead(strUrl, callback2, dvdInfoRequestData, cbMaxSize);
                }
                catch (WebException exception)
                {
                    flag = true;
                    if (dvdInfoRequestData != null)
                    {
                        dvdInfoRequestData.Exception = exception;
                    }
                }
                catch (InvalidOperationException exception2)
                {
                    flag = true;
                    if (dvdInfoRequestData != null)
                    {
                        dvdInfoRequestData.Exception = exception2;
                    }
                }
                if (flag)
                {
                    CompleteTheRequest(dvdInfoRequestData, true, true);
                }
            }
            DvdInfoCallback dvdInfoCallback = new DvdInfoCallback(callback, callbackData);
            if (dvdInfoRequestData.IsCompleted)
            {
                return IssueCallback(dvdInfoRequestData, dvdInfoCallback, true);
            }
            if (callback != null)
            {
                dvdInfoRequestData.Callbacks.Add(dvdInfoCallback);
            }
            return new DvdInfoAsyncResult(dvdInfoRequestData, dvdInfoCallback.CallbackData, false);
        }

        private static void _InetStreamCallback(IAsyncResult ar, bool bCacheResults)
        {
            DvdInfoRequestData dvdInfoRequestData = null;
            try
            {
                dvdInfoRequestData = (DvdInfoRequestData)ar.AsyncState;
                using (Stream stream = InetStream.EndRead(ar))
                {
                    dvdInfoRequestData.DvdInfo = LoadFromWeb(dvdInfoRequestData.DvdId, stream, bCacheResults);
                }
                CompleteTheRequest(dvdInfoRequestData, false, false);
            }
            catch (WebException exception)
            {
                if (dvdInfoRequestData == null)
                {
                    throw exception;
                }
                dvdInfoRequestData.Exception = exception;
                CompleteTheRequest(dvdInfoRequestData, true, false);
            }
            catch (SystemException exception2)
            {
                //if (eDebug.FilterException("DvdInfo::InetStreamCallback", eDebug.ExceptionType.Ignored, exception2))
                //{
                //    throw;
                //}
                if (dvdInfoRequestData == null)
                {
                    throw exception2;
                }
                dvdInfoRequestData.Exception = exception2;
                CompleteTheRequest(dvdInfoRequestData, true, false);
            }
        }

        public static IAsyncResult BeginGetDvdInfo(string dvdId, AsyncCallback callback, object callbackData)
        {
            lock (s_lock)
            {
                DvdInfoRequestData dvdInfoRequestData = (DvdInfoRequestData)s_currentRequests[dvdId];
                if (dvdInfoRequestData == null)
                {
                    DvdInfo dvdInfo = LoadFromCache(dvdId);
                    if (IsValid(dvdInfo))
                    {
                        dvdInfoRequestData = new DvdInfoRequestData(dvdId);
                        s_currentRequests[dvdId] = dvdInfoRequestData;
                        dvdInfoRequestData.DvdInfo = dvdInfo;
                        dvdInfoRequestData.IsCompleted = true;
                    }
                }
                return _BeginGetDvdInfo(dvdInfoRequestData, dvdId, callback, callbackData, false);
            }
        }

        public static IAsyncResult BeginGetDvdInfoEx(string dvdId, AsyncCallback callback, object callbackData)
        {
            lock (s_lock)
            {
                return _BeginGetDvdInfo(null, dvdId, callback, callbackData, true);
            }
        }

        private static void CompleteTheRequest(DvdInfoRequestData dvdInfoRequestData, bool fRemove, bool synchronous)
        {
            lock (s_lock)
            {
                dvdInfoRequestData.IsCompleted = true;
                dvdInfoRequestData.AsyncEvent.Set();
                for (int i = 0; i < dvdInfoRequestData.Callbacks.Count; i++)
                {
                    DvdInfoCallback dvdInfoCallback = (DvdInfoCallback)dvdInfoRequestData.Callbacks[i];
                    IssueCallback(dvdInfoRequestData, dvdInfoCallback, synchronous);
                }
                dvdInfoRequestData.Callbacks.Clear();
                if (fRemove && (dvdInfoRequestData.DvdId != null))
                {
                    s_currentRequests.Remove(dvdInfoRequestData.DvdId);
                }
            }
        }

        private static DvdInfo CreateFromXML(string dvdId, Stream stream)
        {
            XmlSerializer serializer = new DvdInfoSerializer();
            try
            {
                DvdInfo info = (DvdInfo)serializer.Deserialize(stream);
                info.DvdId = dvdId;
                return info;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public static DvdInfo EndGetDvdInfo(IAsyncResult ar)
        {
            if (ar == null)
            {
                throw new ArgumentNullException("ar");
            }
            DvdInfoAsyncResult result = ar as DvdInfoAsyncResult;
            if (result == null)
            {
                throw new ArgumentException("ar must be of type DvdInfoAsyncResult");
            }
            DvdInfoRequestData requestData = result.RequestData;
            if (requestData.Exception != null)
            {
                throw requestData.Exception;
            }
            return requestData.DvdInfo;
        }

        public void EnsureImageIsCached()
        {
            string coverCachePath = GetCoverCachePath(this.MdrDvd.LargeCoverUrl);
            if (!File.Exists(coverCachePath))
            {
                string largeCoverUrl = this.LargeCoverUrl;
                if (!string.IsNullOrEmpty(largeCoverUrl))
                {
                    WebResponse response = null;
                    Stream stream = null;
                    try
                    {
                        WebRequest request = WebRequest.Create(largeCoverUrl);
                        response = (request != null) ? request.GetResponse() : null;
                        stream = (response != null) ? response.GetResponseStream() : null;
                        if (stream != null)
                        {
                            new System.Drawing.Bitmap(stream).Save(coverCachePath);
                        }
                    }
                    catch (WebException)
                    {
                    }
                    catch (Exception exception)
                    {
                        //if (eDebug.FilterException("DvdInfo::EnsureImageIsCached", exception))
                        //{
                        //    throw;
                        //}
                    }
                    finally
                    {
                        if (response != null)
                        {
                            response.Close();
                            response = null;
                        }
                        if (stream != null)
                        {
                            stream.Close();
                            stream = null;
                        }
                    }
                }
            }
        }

        private string FinalImagePath(string strBasePath)
        {
            if (string.IsNullOrEmpty(strBasePath))
            {
                return null;
            }
            if (Path.IsPathRooted(strBasePath))
            {
                return strBasePath;
            }
            string coverCachePath = GetCoverCachePath(strBasePath);
            if (File.Exists(coverCachePath))
            {
                return coverCachePath;
            }
            return ("http://images.windowsmedia.com/" + "dvdcover/" + strBasePath);
        }

        public static string GetCoverCachePath(string coverName)
        {
            if (createCoverPath)
            {
                createCoverPath = false;
                string path = Path.Combine(BasePath, "DvdCoverCache");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            return GetPath("DvdCoverCache", coverName, null);
        }

        public static string GetFileCachePath(string dvdId)
        {
            return GetPath("DvdInfoCache", dvdId, ".xml");
        }

        public static string SafeFileName(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                char newChar = '-';
                str = str.Replace('\\', newChar);
                str = str.Replace('/', newChar);
                str = str.Replace(':', newChar);
                str = str.Replace('*', newChar);
                str = str.Replace('?', newChar);
                str = str.Replace('"', newChar);
                str = str.Replace('<', newChar);
                str = str.Replace('>', newChar);
                str = str.Replace('|', newChar);
                str = str.Replace('%', newChar);
            }
            return str;
        }



        private static string GetPath(string relPath, string file, string ext)
        {
            file = SafeFileName(file);
            if (!string.IsNullOrEmpty(file))
            {
                try
                {
                    return Path.Combine(Path.Combine(BasePath, relPath), file + ext);
                }
                catch (SystemException)
                {
                }
            }
            return null;
        }

        private static void InetStreamCallback(IAsyncResult ar)
        {
            _InetStreamCallback(ar, true);
        }

        private static void InetStreamCallbackEx(IAsyncResult ar)
        {
            _InetStreamCallback(ar, false);
        }

        private static DvdInfoAsyncResult IssueCallback(DvdInfoRequestData dvdInfoRequestData, DvdInfoCallback dvdInfoCallback, bool completedSynchronously)
        {
            DvdInfoAsyncResult ar = new DvdInfoAsyncResult(dvdInfoRequestData, dvdInfoCallback.CallbackData, completedSynchronously);
            if (dvdInfoCallback.Callback != null)
            {
                if (completedSynchronously)
                {
                    if (dvdInfoCallback != null)
                    {
                        dvdInfoCallback.Callback(ar);
                    }
                    return ar;
                }
                //TODO: rewire callback
                //Application.DeferredInvoke(dvdInfoCallback.Callback, new object[] { ar });
            }
            return ar;
        }

        private static bool IsValid(DvdInfo dvdInfo)
        {
            return IsValid(dvdInfo, false);
        }

        private static bool IsValid(DvdInfo dvdInfo, bool bCheckExpiration)
        {
            if (dvdInfo == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(dvdInfo.Name))
            {
                return false;
            }
            if (bCheckExpiration && (DateTime.Now.CompareTo(dvdInfo.MdrDvd.MetadataExpires) > 0))
            {
                return false;
            }
            return true;
        }

        private static DvdInfo LoadFromCache(string dvdId)
        {
            FileStream stream;
            if (dvdId == null)
            {
                throw new ArgumentNullException("dvdId");
            }
            dvdId = dvdId.Trim();
            string fileCachePath = GetFileCachePath(dvdId);
            if ((fileCachePath == null) || !File.Exists(fileCachePath))
            {
                return null;
            }
            try
            {
                stream = new FileStream(fileCachePath, FileMode.Open);
            }
            catch (FileNotFoundException exception)
            {
                //eDebug.DumpExceptionToLog("DvdInfo::LoadFromCache FileNotFoundException even after File.Exists check passed", eDebug.ExceptionType.Recovered, exception);
                return null;
            }
            catch (IOException exception2)
            {
                //eDebug.DumpExceptionToLog("DvdInfo::LoadFromCache IOException even after File.Exists check passed", eDebug.ExceptionType.Recovered, exception2);
                return null;
            }
            catch (UnauthorizedAccessException exception3)
            {
                //eDebug.DumpExceptionToLog("DvdInfo::LoadFromCache UnauthorizedAccessException even though cache should be in user's profile", eDebug.ExceptionType.Recovered, exception3);
                return null;
            }
            DvdInfo info = CreateFromXML(dvdId, stream);
            stream.Close();
            return info;
        }

        private static DvdInfo LoadFromWeb(string dvdId, Stream stream, bool bCacheResults)
        {
            if (dvdId == null)
            {
                throw new ArgumentNullException("dvdId");
            }
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            DvdInfo dvdInfo = CreateFromXML(dvdId, stream);
            if (dvdInfo != null)
            {
                if (IsValid(dvdInfo, false))
                {
                    dvdInfo.NeedsAttribution = true;
                    dvdInfo.MdrDvd.MetadataExpires = DateTime.Now.AddDays(90.0);
                }
                if (bCacheResults)
                {
                    dvdInfo.Save();
                }
            }
            return dvdInfo;
        }

        public void Save()
        {
            try
            {
                string fileCachePath = GetFileCachePath(this.DvdId);
                if (fileCachePath != null)
                {
                    string directoryName = Path.GetDirectoryName(fileCachePath);
                    if (!Directory.Exists(directoryName))
                    {
                        Directory.CreateDirectory(directoryName);
                    }
                    using (FileStream stream = new FileStream(fileCachePath, FileMode.Create))
                    {
                        new DvdInfoSerializer().Serialize((Stream)stream, this);
                    }
                }
            }
            catch (Exception exception)
            {
                //if (eDebug.FilterException("DvdInfo::Save", exception))
                //{
                //    throw;
                //}
            }
        }

        // Properties
        private static string BasePath
        {
            get
            {
                if (string.IsNullOrEmpty(s_sBasePath))
                {
                    s_sBasePath = "DvdCoverCache";// DVDSettings.Settings.AltCacheLocation;
                    if (!string.IsNullOrEmpty(s_sBasePath))
                    {
                        return s_sBasePath;
                    }
                    s_sBasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Microsoft\eHome");
                }
                return s_sBasePath;
            }
        }

        [XmlIgnore]
        public string Director
        {
            get
            {
                return this.MdrDvd.Director;
            }
        }

        [XmlIgnore]
        public string Duration
        {
            get
            {
                return this.MdrDvd.Duration;
            }
        }

        public string DvdId
        {
            get
            {
                return this._DvdId;
            }
            set
            {
                this._DvdId = value;
            }
        }

        [XmlIgnore]
        public string Genre
        {
            get
            {
                return this.MdrDvd.Genre;
            }
        }

        [XmlIgnore]
        public string LargeCoverUrl
        {
            get
            {
                return this.FinalImagePath(this.MdrDvd.LargeCoverUrl);
            }
        }

        [XmlIgnore]
        public string LeadPerformer
        {
            get
            {
                return this.MdrDvd.LeadPerformer;
            }
        }

        [XmlIgnore]
        public string MPAARating
        {
            get
            {
                return this.MdrDvd.MPAARating;
            }
        }

        [XmlIgnore]
        public string Name
        {
            get
            {
                return this.MdrDvd.Name;
            }
        }

        [XmlIgnore]
        public string ReleaseDate
        {
            get
            {
                return this.MdrDvd.ReleaseDate;
            }
        }

        [XmlIgnore]
        public string SmallCoverUrl
        {
            get
            {
                return this.FinalImagePath(this.MdrDvd.SmallCoverUrl);
            }
        }

        [XmlIgnore]
        public string Studio
        {
            get
            {
                return this.MdrDvd.Studio;
            }
        }

        public string Synopsis
        {
            get
            {
                if ((this.MdrDvd.Titles != null) && (this.MdrDvd.Titles.Length != 0))
                {
                    return this.MdrDvd.Titles[0].Synopsis;
                }
                return null;
            }
        }

        [XmlIgnore]
        public Library.Code.V3.XmlSerializationReaderDvdInfo.Title[] Titles
        {
            get
            {
                return this.MdrDvd.Titles;
            }
        }

        //internal static bool WorkOffline
        //{
        //    get
        //    {
        //        return PageBasedUCPService.GlobalConfig.workoffline;
        //    }
        //    set
        //    {
        //        PageBasedUCPService.GlobalConfig.workoffline = value;
        //    }
        //}

        // Nested Types
        private sealed class DvdInfoAsyncResult : IAsyncResult
        {
            // Fields
            private object _callbackData;
            private bool _completedSynchronously;
            private DvdInfo.DvdInfoRequestData _requestData;

            // Methods
            public DvdInfoAsyncResult(DvdInfo.DvdInfoRequestData requestData, object callbackData, bool completedSynchronously)
            {
                this._requestData = requestData;
                this._callbackData = callbackData;
                this._completedSynchronously = completedSynchronously;
            }

            // Properties
            public object AsyncState
            {
                get
                {
                    return this._callbackData;
                }
            }

            public WaitHandle AsyncWaitHandle
            {
                get
                {
                    return this._requestData.AsyncEvent;
                }
            }

            public bool CompletedSynchronously
            {
                get
                {
                    return this._completedSynchronously;
                }
            }

            public bool IsCompleted
            {
                get
                {
                    return this._requestData.IsCompleted;
                }
            }

            public DvdInfo.DvdInfoRequestData RequestData
            {
                get
                {
                    return this._requestData;
                }
            }
        }

        private sealed class DvdInfoCallback
        {
            // Fields
            private AsyncCallback _callback;
            private object _callbackData;

            // Methods
            public DvdInfoCallback(AsyncCallback callback, object callbackData)
            {
                this._callback = callback;
                this._callbackData = callbackData;
            }

            // Properties
            public AsyncCallback Callback
            {
                get
                {
                    return this._callback;
                }
            }

            public object CallbackData
            {
                get
                {
                    return this._callbackData;
                }
            }
        }

        private sealed class DvdInfoRequestData
        {
            // Fields
            public ManualResetEvent AsyncEvent;
            public ArrayList Callbacks = new ArrayList();
            public string DvdId;
            public DvdInfo DvdInfo;
            public Exception Exception;
            public bool IsCompleted;

            // Methods
            public DvdInfoRequestData(string dvdId)
            {
                this.DvdId = dvdId;
                this.AsyncEvent = new ManualResetEvent(false);
            }

            ~DvdInfoRequestData()
            {
                this.AsyncEvent.Close();
            }
        }
    }

    public sealed class InetStream
    {
        // Fields
        private static ArrayList s_HostProxyInfo = new ArrayList();

        // Methods
        public static IAsyncResult BeginRead(WebRequest req, AsyncCallback callback, object callbackData, uint cbMaxSize)
        {
            HttpWebRequest request = req as HttpWebRequest;
            if (request != null)
            {
                request.KeepAlive = false;
            }
            InetAsyncResult state = new InetAsyncResult(callback, callbackData, cbMaxSize)
            {
                _req = req
            };
            HostProxyInfo hostProxyInfo = null;
            lock (s_HostProxyInfo)
            {
                hostProxyInfo = GetHostProxyInfo(state._req.RequestUri);
            }
            if ((hostProxyInfo != null) && (hostProxyInfo.Proxy != null))
            {
                state._req.Proxy = hostProxyInfo.Proxy;
            }
            else
            {
                state._req.Proxy = null;
            }
            state._req.BeginGetResponse(new AsyncCallback(InetStream.RespCallback), state);
            return state;
        }

        public static IAsyncResult BeginRead(string strUrl, AsyncCallback callback, object callbackData, uint cbMaxSize)
        {
            return BeginRead(WebRequest.Create(strUrl), callback, callbackData, cbMaxSize);
        }

        private static void CompleteTheRead(InetAsyncResult iar)
        {
            if ((iar._responseStream != null) && ((iar._exception == null) || !(iar._exception is InetStreamOverflowException)))
            {
                iar._responseStream.Close();
            }
            iar._localStream.Position = 0L;
            iar._isCompleted = true;
            iar._asyncEvent.Set();
            if (iar._callback != null)
            {
                iar._callback(iar);
            }
        }

        public static Stream EndRead(IAsyncResult ar)
        {
            InetAsyncResult result = (InetAsyncResult)ar;
            if (result._exception != null)
            {
                throw result._exception;
            }
            Stream stream = null;
            if (result.IsCompleted)
            {
                stream = result._localStream;
                result._localStream = null;
            }
            return stream;
        }
        [DllImport("ehepgnet.dll", CharSet = CharSet.Unicode)]
        public static extern bool GetProxyForUrl(string wszUrl, StringBuilder strProxyName, int cchStrLen);

        private static HostProxyInfo FindHostProxyInfo(Uri requestUri)
        {
            HostProxyInfo hostProxyInfo = null;
            try
            {
                hostProxyInfo = GetHostProxyInfo(requestUri);
                if (hostProxyInfo == null)
                {
                    StringBuilder strProxyName = new StringBuilder(0x1000);
                    if (!GetProxyForUrl(requestUri.ToString(), strProxyName, strProxyName.Capacity) || (strProxyName.Length <= 0))
                    {
                        return hostProxyInfo;
                    }
                    hostProxyInfo = new HostProxyInfo(requestUri.Host, strProxyName.ToString());
                    s_HostProxyInfo.Add(hostProxyInfo);
                }
            }
            catch (Exception exception)
            {
                //if (eDebug.FilterException("InetStream::RespCallback", eDebug.ExceptionType.Ignored, exception))
                //{
                //    throw;
                //}
                hostProxyInfo = null;
            }
            return hostProxyInfo;
        }

        private static HostProxyInfo GetHostProxyInfo(Uri requestUri)
        {
            HostProxyInfo info = null;
            try
            {
                string host = requestUri.Host;
                if (host != null)
                {
                    foreach (HostProxyInfo info2 in s_HostProxyInfo)
                    {
                        if (string.Compare(info2.Host, host, true, CultureInfo.InvariantCulture) == 0)
                        {
                            info = info2;
                        }
                    }
                }
                return info;
            }
            catch (Exception exception)
            {
                //if (eDebug.FilterException("InetStream::RespCallback", eDebug.ExceptionType.Ignored, exception))
                //{
                //    throw;
                //}
                //info = null;
            }
            return info;
        }

        private static void ReadCallBack(IAsyncResult ar)
        {
            InetAsyncResult state = null;
            try
            {
                state = (InetAsyncResult)ar.AsyncState;
                Stream stream = state._responseStream;
                bool flag = false;
                bool flag2 = false;
                int num = stream.EndRead(ar);
                int count = 0;
                if (num > 0)
                {
                    count = num;
                    if ((state._cbMaxSize > 0) && ((state._cbCurrSize + count) > state._cbMaxSize))
                    {
                        count = (int)(state._cbMaxSize - state._cbCurrSize);
                        flag2 = true;
                    }
                }
                if (count > 0)
                {
                    state._localStream.Write(state._bufferRead, 0, count);
                    state._cbCurrSize += (uint)count;
                    if ((state._cbMaxSize == 0) || (state._cbCurrSize < state._cbMaxSize))
                    {
                        flag = true;
                    }
                }
                if (flag2)
                {
                    throw new InetStreamOverflowException();
                }
                if (flag)
                {
                    stream.BeginRead(state._bufferRead, 0, 0x400, new AsyncCallback(InetStream.ReadCallBack), state);
                }
                else
                {
                    CompleteTheRead(state);
                }
            }
            catch (Exception exception)
            {
                //if (eDebug.FilterException("InetStream::ReadCallback", exception))
                //{
                //    throw;
                //}
                if (state == null)
                {
                    throw exception;
                }
                state._exception = exception;
                CompleteTheRead(state);
            }
        }

        public static Stream ReadInetStream(WebRequest req, uint cbMaxSize)
        {
            InetAsyncResult result = (InetAsyncResult)BeginRead(req, null, null, cbMaxSize);
            result.AsyncWaitHandle.WaitOne();
            if (result._exception != null)
            {
                throw result._exception;
            }
            return result._localStream;
        }

        public static Stream ReadInetStream(string strUrl, uint cbMaxSize)
        {
            return ReadInetStream(WebRequest.Create(strUrl), cbMaxSize);
        }

        private static void RespCallback(IAsyncResult ar)
        {
            InetAsyncResult state = null;
            try
            {
                state = (InetAsyncResult)ar.AsyncState;
                state._responseStream = state._req.EndGetResponse(ar).GetResponseStream();
                state._responseStream.BeginRead(state._bufferRead, 0, 0x400, new AsyncCallback(InetStream.ReadCallBack), state);
                if (state._hostProxyInfo != null)
                {
                    lock (s_HostProxyInfo)
                    {
                        state._hostProxyInfo.Proxy = new WebProxy(state._req.Proxy.GetProxy(state._req.RequestUri));
                    }
                }
            }
            catch (WebException exception)
            {
                //eDebug.DumpExceptionToLog("InetStream::RespCallback", eDebug.ExceptionType.NotHandled, exception, false);
                bool flag = false;
                if (exception.Status == WebExceptionStatus.NameResolutionFailure)
                {
                    if (state._hostProxyInfo == null)
                    {
                        lock (s_HostProxyInfo)
                        {
                            state._hostProxyInfo = FindHostProxyInfo(state._req.RequestUri);
                        }
                    }
                    WebProxy proxy = null;
                    if (state._hostProxyInfo != null)
                    {
                        proxy = state._hostProxyInfo.Proxy;
                    }
                    if (proxy == null)
                    {
                        string nextProxyServer = state.GetNextProxyServer();
                        if ((nextProxyServer != null) && (nextProxyServer.Length > 0))
                        {
                            proxy = new WebProxy(nextProxyServer, true);
                        }
                    }
                    if (proxy != null)
                    {
                        state._req = WebRequest.Create(state._req.RequestUri.ToString());
                        state._req.Proxy = proxy;
                        state._req.BeginGetResponse(new AsyncCallback(InetStream.RespCallback), state);
                        flag = true;
                    }
                }
                if (!flag)
                {
                    if (state == null)
                    {
                        throw exception;
                    }
                    state._exception = exception;
                    CompleteTheRead(state);
                }
            }
            catch (Exception exception2)
            {
                //if (eDebug.FilterException("InetStream::RespCallback", exception2))
                //{
                //    throw;
                //}
                if (state == null)
                {
                    throw exception2;
                }
                state._exception = exception2;
                CompleteTheRead(state);
            }
        }

        // Nested Types
        public sealed class HostProxyInfo
        {
            // Fields
            private string[] _proxyServers;
            private string _strHostName;
            private WebProxy _webProxy;
            public const int c_MaxProxyServersString = 0x1000;

            // Methods
            public HostProxyInfo(string strHostName, string proxyServersString)
            {
                this._strHostName = strHostName;
                if (proxyServersString != null)
                {
                    this._proxyServers = proxyServersString.Split(new char[] { ';' });
                }
            }

            public string GetProxyServer(int iIndex)
            {
                string str = null;
                if (((this._proxyServers != null) && (iIndex >= 0)) && (iIndex < this._proxyServers.Length))
                {
                    str = this._proxyServers[iIndex];
                }
                return str;
            }

            // Properties
            public int Count
            {
                get
                {
                    int length = 0;
                    if (this._proxyServers != null)
                    {
                        length = this._proxyServers.Length;
                    }
                    return length;
                }
            }

            public string Host
            {
                get
                {
                    return this._strHostName;
                }
            }

            public WebProxy Proxy
            {
                get
                {
                    return this._webProxy;
                }
                set
                {
                    this._webProxy = value;
                }
            }
        }

        public sealed class InetAsyncResult : IAsyncResult
        {
            // Fields
            public ManualResetEvent _asyncEvent;
            public byte[] _bufferRead = new byte[0x400];
            public AsyncCallback _callback;
            public object _callbackData;
            public uint _cbCurrSize;
            public uint _cbMaxSize;
            public Exception _exception;
            public int _hostProxyIndex;
            public InetStream.HostProxyInfo _hostProxyInfo;
            public bool _isCompleted;
            public Stream _localStream;
            public WebRequest _req;
            public Stream _responseStream;
            public const int BUFFER_SIZE = 0x400;

            // Methods
            public InetAsyncResult(AsyncCallback callback, object callbackData, uint cbMaxSize)
            {
                this._callback = callback;
                this._callbackData = callbackData;
                this._asyncEvent = new ManualResetEvent(false);
                this._localStream = new MemoryStream();
                this._cbMaxSize = cbMaxSize;
            }

            ~InetAsyncResult()
            {
                this._asyncEvent.Close();
            }

            public string GetNextProxyServer()
            {
                string proxyServer = null;
                if (this._hostProxyInfo != null)
                {
                    while ((proxyServer == null) && (this._hostProxyIndex < this._hostProxyInfo.Count))
                    {
                        proxyServer = this._hostProxyInfo.GetProxyServer(this._hostProxyIndex++);
                    }
                }
                return proxyServer;
            }

            // Properties
            public object AsyncState
            {
                get
                {
                    return this._callbackData;
                }
            }

            public WaitHandle AsyncWaitHandle
            {
                get
                {
                    return this._asyncEvent;
                }
            }

            public bool CompletedSynchronously
            {
                get
                {
                    return false;
                }
            }

            public bool IsCompleted
            {
                get
                {
                    return this._isCompleted;
                }
            }
        }
    }

    public sealed class InetStreamOverflowException : Exception
    {
    }

    public sealed class DvdInfoSerializer : DvdInfoXmlSerializerBase
    {
        // Methods
        public override bool CanDeserialize(XmlReader xmlReader)
        {
            return xmlReader.IsStartElement("METADATA", "");
        }

        protected override object Deserialize(XmlSerializationReader reader)
        {
            return ((XmlSerializationReaderDvdInfo)reader).Read6_METADATA();
        }

        protected override void Serialize(object objectToSerialize, XmlSerializationWriter writer)
        {
            ((XmlSerializationWriterDvdInfo)writer).Write6_METADATA(objectToSerialize);
        }
    }


    public abstract class DvdInfoXmlSerializerBase : XmlSerializer
    {
        // Methods
        protected DvdInfoXmlSerializerBase()
        {
        }

        protected override XmlSerializationReader CreateReader()
        {
            return new XmlSerializationReaderDvdInfo();
        }

        protected override XmlSerializationWriter CreateWriter()
        {
            return new XmlSerializationWriterDvdInfo();
        }
    }

    public class XmlSerializationWriterDvdInfo : XmlSerializationWriter
    {
        // Methods
        protected override void InitCallbacks()
        {
        }

        private void Write2_Chapter(string n, string ns, Library.Code.V3.XmlSerializationReaderDvdInfo.Chapter o, bool needType)
        {
            if (!needType && (o.GetType() != typeof(Library.Code.V3.XmlSerializationReaderDvdInfo.Chapter)))
            {
                throw base.CreateUnknownTypeException(o);
            }
            base.WriteStartElement(n, ns, o, false, null);
            if (needType)
            {
                base.WriteXsiType("Chapter", "");
            }
            base.WriteElementString("chapterNum", "", o.ChapterNumber);
            base.WriteElementString("chapterTitle", "", o.Name);
            base.WriteEndElement(o);
        }

        private void Write3_Title(string n, string ns, Library.Code.V3.XmlSerializationReaderDvdInfo.Title o, bool needType)
        {
            if (!needType && (o.GetType() != typeof(Library.Code.V3.XmlSerializationReaderDvdInfo.Title)))
            {
                throw base.CreateUnknownTypeException(o);
            }
            base.WriteStartElement(n, ns, o, false, null);
            if (needType)
            {
                base.WriteXsiType("Title", "");
            }
            base.WriteElementString("titleNum", "", o.TitleNumber);
            base.WriteElementString("titleTitle", "", o.Name);
            base.WriteElementString("studio", "", o.Studio);
            base.WriteElementString("director", "", o.Director);
            base.WriteElementString("leadPerformer", "", o.LeadPerformer);
            base.WriteElementString("MPAARating", "", o.MPAARating);
            base.WriteElementString("genre", "", o.Genre);
            base.WriteElementString("synopsis", "", o.Synopsis);
            Library.Code.V3.XmlSerializationReaderDvdInfo.Chapter[] chapters = o.Chapters;
            if (chapters != null)
            {
                for (int i = 0; i < chapters.Length; i++)
                {
                    this.Write2_Chapter("chapter", "", chapters[i], false);
                }
            }
            base.WriteEndElement(o);
        }

        private void Write4_MdrDvd(string n, string ns, Library.Code.V3.XmlSerializationReaderDvdInfo.MdrDvd o, bool needType)
        {
            if (!needType && (o.GetType() != typeof(Library.Code.V3.XmlSerializationReaderDvdInfo.MdrDvd)))
            {
                throw base.CreateUnknownTypeException(o);
            }
            base.WriteStartElement(n, ns, o, false, null);
            if (needType)
            {
                base.WriteXsiType("MdrDvd", "");
            }
            base.WriteElementStringRaw("MetadataExpires", "", XmlSerializationWriter.FromDateTime(o.MetadataExpires));
            base.WriteElementString("version", "", o.Version);
            base.WriteElementString("dvdTitle", "", o.Name);
            base.WriteElementString("studio", "", o.Studio);
            base.WriteElementString("leadPerformer", "", o.LeadPerformer);
            base.WriteElementString("director", "", o.Director);
            base.WriteElementString("MPAARating", "", o.MPAARating);
            base.WriteElementString("language", "", o.Language);
            base.WriteElementString("releaseDate", "", o.ReleaseDate);
            base.WriteElementString("genre", "", o.Genre);
            base.WriteElementString("largeCoverParams", "", o.LargeCoverUrl);
            base.WriteElementString("smallCoverParams", "", o.SmallCoverUrl);
            base.WriteElementString("dataProvider", "", o.DataProvider);
            base.WriteElementString("duration", "", o.Duration);
            Library.Code.V3.XmlSerializationReaderDvdInfo.Title[] titles = o.Titles;
            if (titles != null)
            {
                for (int i = 0; i < titles.Length; i++)
                {
                    this.Write3_Title("title", "", titles[i], false);
                }
            }
            base.WriteEndElement(o);
        }

        private void Write5_DvdInfo(string n, string ns, DvdInfo o, bool isNullable, bool needType)
        {
            if (o == null)
            {
                if (isNullable)
                {
                    base.WriteNullTagLiteral(n, ns);
                }
            }
            else
            {
                if (!needType && (o.GetType() != typeof(DvdInfo)))
                {
                    throw base.CreateUnknownTypeException(o);
                }
                base.WriteStartElement(n, ns, o, false, null);
                if (needType)
                {
                    base.WriteXsiType("DvdInfo", "");
                }
                this.Write4_MdrDvd("MDR-DVD", "", o.MdrDvd, false);
                base.WriteElementStringRaw("NeedsAttribution", "", XmlConvert.ToString(o.NeedsAttribution));
                base.WriteElementString("DvdId", "", o.DvdId);
                base.WriteEndElement(o);
            }
        }

        public void Write6_METADATA(object o)
        {
            base.WriteStartDocument();
            if (o == null)
            {
                base.WriteNullTagLiteral("METADATA", "");
            }
            else
            {
                base.TopLevelElement();
                this.Write5_DvdInfo("METADATA", "", (DvdInfo)o, true, false);
            }
        }
    }
 
}
