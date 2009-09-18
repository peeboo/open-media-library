using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MediaCenter.UI;
using Microsoft.MediaCenter;
using OMLEngine.Settings;
using System.Collections;

namespace Library.Code.V3
{
    public class FirstRunConfigureStartMenu : IFirstRunItem
    {
        #region IFirstRunItem Members

        public string PageTitle { get; set; }
        public string PageInstructions { get; set; }

        public string SourceTemplate
        {
            get
            {
                return "resx://Library/Library.Resources/V3_FirstRunBackground#RadioContentHost";
            }
        }
        public bool FocusContent { get; set; }

        public void Commit()
        {
            if ((string)this.SelectedViewOptions.Chosen == strProgramLibrary || (string)this.SelectedViewOptions.Chosen == strCreateStrip)
            {
                //if there are any existing, clear them out
                while (helper.StartMenuItems.Count > 0)
                {
                    helper.DeleteStartMenuItem(helper.StartMenuItems[0]);
                }
                //foreach(StartMenuItem item in helper.StartMenuItems)
                //{
                //    helper.DeleteStartMenuItem(item);
                //}
            }

            if ((string)this.SelectedViewOptions.Chosen == strCreateStrip)
            {
                //we need to create the strip
                this.CreateDefaultStartMenuItems();
            }
        }

        #endregion

        private void CreateDefaultStartMenuItems()
        {
            //create movies
            StartMenuItem moviesItem = new StartMenuItem();
            moviesItem.Title = "movies";
            moviesItem.Context = Context.Movies;
            moviesItem.ExtendedContext = string.Empty;
            moviesItem.Description = "Movies";
            moviesItem.ImageUrl = Environment.SpecialFolder.ProgramFiles.ToString() + @"\Open Media Library\Application.png";
            this.helper.AddStartMenuItem(moviesItem);
            //create tv
            StartMenuItem tvItem = new StartMenuItem();
            tvItem.Title = "tv";
            tvItem.Context = Context.TV;
            tvItem.ExtendedContext = string.Empty;
            tvItem.Description = "TV";
            tvItem.ImageUrl = Environment.SpecialFolder.ProgramFiles.ToString() + @"\Open Media Library\Application.png";
            this.helper.AddStartMenuItem(tvItem);
            //create trailers
            StartMenuItem trailersItem = new StartMenuItem();
            trailersItem.Title = "trailers";
            trailersItem.Context = Context.Trailers;
            trailersItem.ExtendedContext = string.Empty;
            trailersItem.Description = "Trailers";
            trailersItem.ImageUrl = Environment.SpecialFolder.ProgramFiles.ToString() + @"\Open Media Library\Application.png";
            this.helper.AddStartMenuItem(trailersItem);
            //create search
            StartMenuItem searchItem = new StartMenuItem();
            searchItem.Title = "search";
            searchItem.Context = Context.Search;
            searchItem.ExtendedContext = string.Empty;
            searchItem.Description = "Search";
            searchItem.ImageUrl = Environment.SpecialFolder.ProgramFiles.ToString() + @"\Open Media Library\Application.png";
            this.helper.AddStartMenuItem(searchItem);
        }

        private StartMenuHelper helper;
        private Choice selectedViewOptions;
        public Choice SelectedViewOptions
        {
            get { return selectedViewOptions; }
            set { selectedViewOptions = value; }
        }

        private static string strKeepExisting = "Keep my existing Start Menu Configuration";
        private static string strCreateStrip = "Create an Open Media Library Start Menu Strip";
        private static string strProgramLibrary = "Access Open Media Library from Program Library";


        private FirstRun owner;
        public FirstRunConfigureStartMenu(FirstRun firstRun)
        {
            this.PageInstructions = "How would you like to access Open Media Library?";
            this.PageTitle = "Start Menu";
            this.owner = firstRun;
            this.owner.NextCommandEnabled.Value = true;

            string addinId = OMLApplication.AssemblyName;
            this.helper = new StartMenuHelper(addinId);

            this.selectedViewOptions = new Choice(this.owner);
            List<string> selectedViewList = new List<string>();
            
            if(this.helper.StartMenuItems.Count>0)
                selectedViewList.Add(strKeepExisting);
            selectedViewList.Add(strCreateStrip);
            selectedViewList.Add(strProgramLibrary);

            this.selectedViewOptions.Options = selectedViewList;
            this.selectedViewOptions.ChosenIndex = 0;

            this.selectedViewOptions.ChosenChanged += new EventHandler(selectedViewOptions_ChosenChanged);

            this.FocusContent = true;
        }

        void selectedViewOptions_ChosenChanged(object sender, EventArgs e)
        {
            this.owner.FireNext();   
        }
    }

    public class FirstRunCancelPage : IFirstRunItem
    {
        #region IFirstRunItem Members

        public string PageTitle { get; set; }
        public string PageInstructions { get; set; }
        public string SourceTemplate
        {
            get
            {
                return "resx://Library/Library.Resources/V3_FirstRunBackground#DefaultContentHost";
            }
        }
        public bool FocusContent { get; set; }

        public void Commit()
        {
        }
        #endregion

        public IFirstRunItem LastPage { get; set; }
        private FirstRun owner;
        public FirstRunCancelPage(FirstRun firstRun)
        {
            this.PageInstructions = "Any changes you have made will not be saved. If you would like to continue this wizard at a later time you can access OML from Program Library.\n\nSelect Finish to exit.";
            this.PageTitle = "Setup Canceled";
            this.owner = firstRun;
            this.owner.NextCommandEnabled.Value = false;
        }
    }

    public class FirstRunFinishPage : IFirstRunItem
    {
        #region IFirstRunItem Members

        public string PageTitle { get; set; }
        public string PageInstructions { get; set; }
        public string SourceTemplate
        {
            get
            {
                return "resx://Library/Library.Resources/V3_FirstRunBackground#DefaultContentHost";
            }
        }
        public bool FocusContent { get; set; }

        public void Commit()
        {
        }

        #endregion

        private FirstRun owner;
        public FirstRunFinishPage(FirstRun firstRun)
        {
            this.PageInstructions = "You have finished configuring Open Media Library. For these changes to take effect please restart Media Center.\n\nSelect Finish to exit.";
            this.PageTitle = "You Are Done!";
            this.owner = firstRun;
        }
    }

    public class FirstRunStartPage : IFirstRunItem
    {
        #region IFirstRunItem Members

        public string PageTitle { get; set; }
        public string PageInstructions { get; set; }
        public string SourceTemplate
        {
            get
            {
                return "resx://Library/Library.Resources/V3_FirstRunBackground#DefaultContentHost";
            }
        }
        public bool FocusContent { get; set; }

        public void Commit()
        {
        }

        #endregion

        private FirstRun owner;
        public FirstRunStartPage(FirstRun firstRun)
        {
            this.PageInstructions = "This wizard will help you configure Open Media Library to access your media library.\n\nSelect Next to get started.";
            this.PageTitle = "Welcome to Open Media Library";
            this.owner = firstRun;
        }
    }

    public interface IFirstRunItem
    {
        string PageTitle { get; set; }
        string PageInstructions { get; set; }
        string SourceTemplate { get; }
        bool FocusContent { get; set; }
        void Commit();
    }

    public class FirstRun : ModelItem
    {
        public void FireNext()
        {
            FirePropertyChanged("FocusNext");
        }
        public void FireFinish()
        {
            FirePropertyChanged("FocusFinish");
        }

        public bool FocusNext { get; set; }
        public bool FocusFinish { get; set; }
        public string FirstRunTitle { get; set; }
        private List<IFirstRunItem> pages;
        private IFirstRunItem currentPage;
        private IFirstRunItem cancelPage;
        public IFirstRunItem CurrentPage
        {
            get
            {
                if (goingForward == false && this.currentPage != this.pages[0] && this.currentPage != this.cancelPage)
                    this.currentPage = this.pages[this.pages.IndexOf(this.currentPage) - 1];
                else if (goingForward == false && this.currentPage == this.cancelPage)
                    this.currentPage = ((FirstRunCancelPage)this.cancelPage).LastPage;
                this.goingForward = false;
                return this.currentPage;
            }
        }

        //public bool CancelCommandEnabled { get; set; }
        //cancel is always true
        public bool CancelCommandVisible
        {
            get
            {
                if (this.currentPage == this.cancelPage)
                    return false;
                return true;
            }
        }
        private Command cancelCommand;
        public Command CancelCommand
        {
            get { return cancelCommand; }
            set
            {
                if (cancelCommand != value)
                {
                    cancelCommand = value;
                    FirePropertyChanged("CancelCommand");
                }
            }
        }

        public bool BackCommandVisible
        {
            get
            {

                if (this.currentPage == pages[0])
                    return false;
                return true;

            }
        }
        private Command backCommand;
        public Command BackCommand
        {
            get { return backCommand; }
            set
            {
                if (backCommand != value)
                {
                    backCommand = value;
                    FirePropertyChanged("BackCommand");
                }
            }
        }

        public bool NextCommandVisible
        {
            get
            {
                if (this.currentPage == pages[pages.Count-1] || this.currentPage==this.cancelPage)
                    return false;
                return true;

            }
        }
        private BooleanChoice nextCommandEnabled = new BooleanChoice();
        public BooleanChoice NextCommandEnabled
        {
            get
            {
                return this.nextCommandEnabled;
            }
            set
            {
                if (this.nextCommandEnabled != value)
                {
                    this.nextCommandEnabled = value;
                    FirePropertyChanged("NextCommandEnabled");
                }
            }
        }
        private Command nextCommand;
        public Command NextCommand
        {
            get { return nextCommand; }
            set
            {
                if (nextCommand != value)
                {
                    nextCommand = value;
                    FirePropertyChanged("NextCommand");
                }
            }
        }

        public bool FinishCommandVisible {
            get
            {
                if (this.currentPage == this.pages[this.pages.Count - 1] || this.currentPage == this.cancelPage)
                {
                    return true;
                }

                return false;
            }
        }
        private Command finishCommand;
        public Command FinishCommand
        {
            get { return finishCommand; }
            set
            {
                if (finishCommand != value)
                {
                    finishCommand = value;
                    FirePropertyChanged("FinishCommand");
                }
            }
        }
        public FirstRun()
        {
            this.cancelPage = new FirstRunCancelPage(this);
            this.pages = new List<Library.Code.V3.IFirstRunItem>();
            pages.Add(new FirstRunStartPage(this));
            pages.Add(new FirstRunConfigureStartMenu(this));
            pages.Add(new FirstRunFinishPage(this));
            

            this.FirstRunTitle = "Open Media Library Setup";
            this.currentPage = pages[0];

            this.backCommand = new Command(this);
            this.backCommand.Description = "Back";
            this.backCommand.Invoked+=new EventHandler(backCommand_Invoked);

            this.nextCommand = new Command(this);
            this.nextCommand.Description = "Next";
            this.nextCommand.Invoked += new EventHandler(nextCommand_Invoked);

            this.cancelCommand = new Command(this);
            this.cancelCommand.Description = "Cancel";
            this.cancelCommand.Invoked += new EventHandler(cancelCommand_Invoked);

            this.finishCommand = new Command(this);
            this.finishCommand.Description = "Finish";
            this.finishCommand.Invoked += new EventHandler(finishCommand_Invoked);
        }

        void finishCommand_Invoked(object sender, EventArgs e)
        {
            if (this.currentPage != this.cancelPage)
            {
                //we completed firstrun
                Properties.Settings.Default.CompletedFirstRun = true;
                Properties.Settings.Default.Save();
                //save stuff
                foreach (IFirstRunItem item in this.pages)
                {
                    item.Commit();
                }
            }
            //close
            OMLApplication.Current.Session.Close();
        }

        void cancelCommand_Invoked(object sender, EventArgs e)
        {
            ((FirstRunCancelPage)this.cancelPage).LastPage = this.currentPage;
            this.currentPage = this.cancelPage;
            Dictionary<string, object> properties = new Dictionary<string, object>();

            properties["Page"] = this;
            //properties["Application"] = OMLApplication.Current;

            this.goingForward = true;
            OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_FirstRunBackground", properties);
        }

        private bool goingForward = false;
        void nextCommand_Invoked(object sender, EventArgs e)
        {
            this.currentPage = this.pages[this.pages.IndexOf(this.currentPage) + 1];
            Dictionary<string, object> properties = new Dictionary<string, object>();

            properties["Page"] = this;
            //properties["Application"] = OMLApplication.Current;

            this.goingForward = true;
            OMLApplication.Current.Session.GoToPage("resx://Library/Library.Resources/V3_FirstRunBackground", properties);
        }

        void backCommand_Invoked(object sender, EventArgs e)
        {
            //we simply go back in the stack
            this.goingForward = false;
            if (this.currentPage != this.pages[0])
            {
                //this.currentPage = this.pages[this.pages.IndexOf(this.currentPage) - 1];
                OMLApplication.Current.Session.BackPage();
            }
            else
            {
                OMLApplication.Current.Session.Close();
            }
        }
    }
}
