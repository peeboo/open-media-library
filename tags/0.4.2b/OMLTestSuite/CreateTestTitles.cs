using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMLEngine.FileSystem;
using OMLEngine.Settings;
using OMLEngine;


namespace OMLTestSuite
{
    class CreateTestTitles
    {
        int totalfilms;
        int totaltvprograms;
        int seriesperprogram;
        int episodesperseries;

        //int filmno;
        public void CreateTitles()
        {
            Console.WriteLine("Creating titles");
            totalfilms = 100;
            totaltvprograms = 100;
            seriesperprogram = 5;
            episodesperseries = 8;

            // Test to create a title withou type specified
            Title ttt1 = new Title();
            ttt1.Name = "Test without titletype";
            TitleCollectionManager.AddTitle(ttt1);

            Title ttmovies = new Title();
            ttmovies.Name = "Movies";
            ttmovies.TitleType = TitleTypes.Collection | TitleTypes.Root;
            TitleCollectionManager.AddTitle(ttmovies);

            Title tttv = new Title();
            tttv.Name = "TV";
            tttv.TitleType = TitleTypes.TVShow | TitleTypes.Root;
            TitleCollectionManager.AddTitle(tttv);

            for (int i = 0; i < totalfilms; i++)
            {
                CreateFilm(ttmovies.Id, i);
            }

            for (int i = 0; i < totaltvprograms; i++)
            {
                CreateTVProgram(tttv.Id, i);
            }
            Console.WriteLine("Job Done. Press enter");
            Console.ReadKey();

        }

        public void CreateFilm(int parent, int no)
        {
            Title tt = new Title();
            tt.Name = "Film " + no.ToString();
            tt.TitleType = TitleTypes.Movie;
            tt.ParentTitleId = parent;
            TitleCollectionManager.AddTitle(tt);
        }

        public void CreateTVProgram(int parent, int programno)
        {
            Title tt = new Title();
            tt.Name = "TV P" + programno.ToString();
            tt.TitleType = TitleTypes.TVShow;
            tt.ParentTitleId = parent;
            TitleCollectionManager.AddTitle(tt);

            for (int i = 0; i < seriesperprogram; i++)
            {
                CreateTVSeries(tt.Id, programno, i);
            }
        }

        public void CreateTVSeries(int parent, int programno, int seriesno)
        {
            Title tt = new Title();
            tt.Name = "TV P" + programno.ToString() + " S" + seriesno.ToString();
            tt.TitleType = TitleTypes.Season;
            tt.ParentTitleId = parent;
            TitleCollectionManager.AddTitle(tt);

            for (int i = 0; i < episodesperseries; i++)
            {
                CreateTVEpisode(tt.Id, programno, seriesno, i);
            }
        }

        public void CreateTVEpisode(int parent, int programno, int seriesno, int episodeno)
        {
            Title tt = new Title();
            tt.Name = "TV P" + programno.ToString() + " S" + seriesno.ToString() + " E" + episodeno.ToString();
            tt.TitleType = TitleTypes.Episode;
            tt.ParentTitleId = parent;
            TitleCollectionManager.AddTitle(tt);
        }
    }
}
