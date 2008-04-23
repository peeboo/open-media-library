using System;
using System.Data;
using System.Collections.Generic;

namespace OMLSDK
{
    public sealed class OMLDataSet : DataSet
    {
        public OMLDataSet()
        {
            DataTable tab = new DataTable("Titles");

            DataColumn colName = new DataColumn();
            colName.DataType = typeof(string);
            colName.ColumnName = "Name";
            tab.Columns.Add(colName);

            DataColumn colFileLocation = new DataColumn();
            colFileLocation.DataType = typeof(string);
            colFileLocation.ColumnName = "FileLocation";
            tab.Columns.Add(colFileLocation);

            DataColumn colDescription = new DataColumn();
            colDescription.DataType = typeof(string);
            colDescription.ColumnName = "Description";
            tab.Columns.Add(colDescription);

            DataColumn colItemId = new DataColumn();
            colItemId.DataType = typeof(int);
            colItemId.ColumnName = "ItemId";
            tab.Columns.Add(colItemId);

            DataColumn colFrontBoxartPath = new DataColumn();
            colFrontBoxartPath.DataType = typeof(string);
            colFrontBoxartPath.ColumnName = "FrontBoxartPath";
            tab.Columns.Add(colFrontBoxartPath);

            DataColumn colBackBoxartPath = new DataColumn();
            colBackBoxartPath.DataType = typeof(string);
            colBackBoxartPath.ColumnName = "BackBoxartPath";
            tab.Columns.Add(colBackBoxartPath);

            DataColumn colRuntime = new DataColumn();
            colRuntime.DataType = typeof(int);
            colRuntime.ColumnName = "Runtime";
            tab.Columns.Add(colRuntime);

            DataColumn colMPAARating = new DataColumn();
            colMPAARating.DataType = typeof(string);
            colMPAARating.ColumnName = "MPAARating";
            tab.Columns.Add(colMPAARating);

            DataColumn colSynopsis = new DataColumn();
            colSynopsis.DataType = typeof(string);
            colSynopsis.ColumnName = "Synopsis";
            tab.Columns.Add(colSynopsis);

            DataColumn colDistributor = new DataColumn();
            colDistributor.DataType = typeof(string);
            colDistributor.ColumnName = "Distributor";
            tab.Columns.Add(colDistributor);

            DataColumn colCountry = new DataColumn();
            colCountry.DataType = typeof(string);
            colCountry.ColumnName = "Country";
            tab.Columns.Add(colCountry);

            DataColumn colWebsite = new DataColumn();
            colWebsite.DataType = typeof(string);
            colWebsite.ColumnName = "Website";
            tab.Columns.Add(colWebsite);

            DataColumn colReleaseDate = new DataColumn();
            colReleaseDate.DataType = typeof(DateTime);
            colReleaseDate.ColumnName = "ReleaseDate";
            tab.Columns.Add(colReleaseDate);

            DataColumn colDateAdded = new DataColumn();
            colDateAdded.DataType = typeof(DateTime);
            colDateAdded.ColumnName = "DateAdded";
            tab.Columns.Add(colDateAdded);

            DataColumn colSource = new DataColumn();
            colSource.DataType = typeof(string);
            colSource.ColumnName = "Source";
            tab.Columns.Add(colSource);

            DataColumn colActors = new DataColumn();
            colActors.DataType = typeof(List<string>);
            colActors.ColumnName = "Actors";
            tab.Columns.Add(colActors);

            DataColumn colCrew = new DataColumn();
            colCrew.DataType = typeof(List<string>);
            colCrew.ColumnName = "Crew";
            tab.Columns.Add(colCrew);
        }
        DataColumn col = new DataColumn();
        #region properties
        private List<string> _directors;
        private List<string> _writers;
        private List<string> _producers;
        private List<string> _sound_formats;
        private List<string> _language_formats;
        private List<string> _genres;
        #endregion


    }
}
