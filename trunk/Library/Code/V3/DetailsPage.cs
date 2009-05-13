using System;
using System.Collections;

using Microsoft.MediaCenter.UI;
using System.Text;
using System.Threading;
using System.Data;
using System.Collections.Generic;


namespace Library.Code.V3
{
    /// <summary>
    /// This object contains the standard set of information displayed in the 
    /// details page UI.
    /// </summary>
    public class DetailsPage : BaseModelItem
    {

        public void FastLoadData()
        {
            if (detailsLoaded == false)
            {
                //int UniqueId = ((DataSetHelpers.DetailsCommand)this.commands[0]).ItemId;
                //Image slowImage = ImageHelper.LoadImage(imagePath);
                this.detailsLoaded = true;
                //this.details = SingleRecordHelper.LoadExtendedDataForID(UniqueId);

                //should we add cast info?
                if (this.details.CastArray.Count > 0)
                {
                    foreach (CastCommand actor in this.details.CastArray)
                    {
                        actor.Invoked += new EventHandler(actor_Invoked);
                    }
                    //DataSetHelpers.DetailsCommand command = new DataSetHelpers.DetailsCommand(this, "Cast + More");
                    //command.Type = 0;

                    //command.Invoked += new EventHandler(command_Invoked);// += delegate(object sender, EventArgs e)
                    //this.Commands.Add(command);
                }
                //DataSetHelpers.DetailsCommand similarCommand = new DataSetHelpers.DetailsCommand(this, "Similar Titles");
                //similarCommand.Type = 0;
                //similarCommand.Invoked += new EventHandler(similarCommand_Invoked);
                //this.Commands.Add(similarCommand);
            }
        }

        void similarCommand_Invoked(object sender, EventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
            //int UniqueId = ((DataSetHelpers.DetailsCommand)this.commands[0]).ItemId;
            //Application.Current.GoToSimilarSeries(this);
        }

        void actor_Invoked(object sender, EventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
            //MyMovies actorMovies=new MyMovies(((CastCommand)sender).ActorId, SubGalleryType.Actor);
            //Application.Current.GoToActorSeries((CastCommand)sender);
        }

        void command_Invoked(object sender, EventArgs e)
        {
            //Application.Current.GoToDetailsCast(this);
            //throw new Exception("The method or operation is not implemented.");
        }
        
        /// <summary>
        /// The primary title of the object.
        /// </summary>
        public string Title
        {
            get { return title; }
            set
            {
                if (title != value)
                {
                    title = value;
                    FirePropertyChanged("Title");
                }
            }
        }

        /// <summary>
        /// A multiline summary of the object.
        /// </summary>
        public string Summary
        {
            get { return summary; }
            set
            {
                if (summary != value)
                {
                    summary = value;
                    FirePropertyChanged("Summary");
                }
            }
        }

        /// <summary>
        /// A list of actions that can be performed on this object.
        /// This list should only contain objects of type Command.
        /// </summary>
        public IList Commands
        {
            get { return commands; }
            set
            {
                if (commands != value)
                {
                    commands = value;
                    FirePropertyChanged("Commands");
                }
            }
        }

        /// <summary>
        /// Additional minor metadata about this object.
        /// </summary>
        public string Metadata
        {
            get { return metadata; }
            set
            {
                if (metadata != value)
                {
                    metadata = value;
                    FirePropertyChanged("Metadata");
                }
            }
        }

        /// <summary>
        /// A fullscreen image to display in the background.
        /// </summary>
        public Image Background
        {
            get { return backgroundImage; }
            set
            {
                if (backgroundImage != value)
                {
                    backgroundImage = value;
                    FirePropertyChanged("Background");
                }
            }
        }


        private string title;
        private string summary;
        private IList commands;
        private string metadata;
        private Image backgroundImage;

        //new
        private bool detailsLoaded = false;
        private ExtendedDetails details = new ExtendedDetails();
        public ExtendedDetails Details
        {
            get { return details; }
            set
            {
                if (details != value)
                {
                    details = value;
                    FirePropertyChanged("Details");
                }
            }
        }
    }

    public class CastCommand : Command
    {
        public CastCommand()
            : base()
        {
        }

        public CastCommand(DetailsPage page, string description)
            : base(page, description, null)
        {
        }

        public string CastType
        {
            get { return castType; }
            set { castType = value; }
        }
        private string castType = "Default";

        public string Actor
        {
            get { return actor; }
            set { actor = value; }
        }

        public string Role
        {
            get { return role; }
            set { role = value; }
        }

        public string GroupTitle
        {
            get { return groupTitle; }
            set { groupTitle = value; }
        }

        public int ActorId
        {
            get { return actorId; }
            set { actorId = value; }
        }

        private int actorId;
        private string actor;
        private string role;
        private string groupTitle;
    }

    public class ExtendedDetails
    {
        public List<Image> FanArtImages = null;
        //new
        private string summary = "";
        public string Summary
        {
            get { return summary; }
            set
            {
                if (summary != value)
                {
                    summary = value;
                    //FirePropertyChanged("Summary");
                }
            }
        }

        private Image starRating;
        public Image StarRating
        {
            get { return starRating; }
            set
            {
                if (starRating != value)
                {
                    starRating = value;
                }
            }
        }

        private Image fanArt;
        public Image FanArt
        {
            get { return fanArt; }
            set
            {
                if (fanArt != value)
                {
                    fanArt = value;
                }
            }
        }
        private string yearString = string.Empty;
        public string YearString
        {
            get { return yearString; }
            set
            {
                if (yearString != value)
                {
                    yearString = value;
                    //FirePropertyChanged("YearString");
                }
            }
        }

        private string genreRatingandRuntime = string.Empty;
        public string GenreRatingandRuntime
        {
            get { return genreRatingandRuntime; }
            set
            {
                if (genreRatingandRuntime != value)
                {
                    genreRatingandRuntime = value;
                    //FirePropertyChanged("GenreRatingandRuntime");
                }
            }
        }

        private string studio = string.Empty;
        public string Studio
        {
            get { return studio; }
            set
            {
                if (studio != value)
                {
                    studio = value;
                    //FirePropertyChanged("Studio");
                }
            }
        }

        private string director = string.Empty;
        public string Director
        {
            get { return director; }
            set
            {
                if (director != value)
                {
                    director = value;
                    //FirePropertyChanged("Director");
                }
            }
        }

        private string cast = string.Empty;
        public string Cast
        {
            get { return cast; }
            set
            {
                if (cast != value)
                {
                    cast = value;
                    //FirePropertyChanged("Cast");
                }
            }
        }

        private ArrayListDataSet castArray = new ArrayListDataSet();
        public ArrayListDataSet CastArray
        {
            get { return castArray; }
            set
            {
                if (castArray != value)
                    castArray = value;
            }
        }
    }

    public class SingleRecordHelper
    {
        public static ExtendedDetails LoadExtendedDataForID(int id)
        {
            ExtendedDetails retDetails = new ExtendedDetails();
            return retDetails;
        }

    }
}
