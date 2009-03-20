using System;
using System.Collections;
using Microsoft.MediaCenter.UI;
using System.Text;
using System.Threading;
using System.Data;


namespace Library.Code.V3
{
    /// <summary>
    /// This object contains the standard set of information displayed in the 
    /// details page UI.
    /// </summary>
    public class MoviesSearchPage : ModelItem
    {
        public MoviesSearchPage()
            : base()
        {
            this.title = "search movies";
            this.Commands = new ArrayListDataSet();

            //search by title
            Command titleCmd = new Command();
            titleCmd.Description = "Title";
            this.Commands.Add(titleCmd);

            //search by keyword
            Command keywordCmd = new Command();
            keywordCmd.Description = "Keyword";
            this.Commands.Add(keywordCmd);

            //search by actor
            Command actorCmd = new Command();
            actorCmd.Description = "Actor";
            this.Commands.Add(actorCmd);

            //search by director
            Command directorCmd = new Command();
            directorCmd.Description = "Director";
            this.Commands.Add(directorCmd);
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

        private string title;
        private IList commands;

    }
}
