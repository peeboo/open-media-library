using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;

namespace OMLSDK
{
    public class OMLDataSet : DataSet
    {
        public OMLDataSet() : base()
        {
            DataTable table = new DataTable("Titles");
            DataColumn colName = new DataColumn();
            colName.DataType = typeof(string);
            colName.ColumnName = "Name";
            table.Columns.Add(colName);

            DataColumn colFileLocation = new DataColumn();
            colFileLocation.DataType = typeof(string);
            colFileLocation.ColumnName = "FileLocation";
            table.Columns.Add(colFileLocation);

            DataColumn colDescription = new DataColumn();
            colDescription.DataType = typeof(string);
            colDescription.ColumnName = "Description";
            table.Columns.Add(colDescription);

            DataColumn colItemId = new DataColumn();
            colItemId.DataType = typeof(int);
            colItemId.ColumnName = "ItemId";
            table.Columns.Add(colItemId);

            DataColumn colFrontBoxartPath = new DataColumn();
            colFrontBoxartPath.DataType = typeof(string);
            colFrontBoxartPath.ColumnName = "FrontBoxartPath";
            table.Columns.Add(colFrontBoxartPath);

            DataColumn colBackBoxartPath = new DataColumn();
            colBackBoxartPath.DataType = typeof(string);
            colBackBoxartPath.ColumnName = "BackBoxartPath";
            table.Columns.Add(colBackBoxartPath);

            DataColumn colRuntime = new DataColumn();
            colRuntime.DataType = typeof(int);
            colRuntime.ColumnName = "Runtime";
            table.Columns.Add(colRuntime);

            DataColumn colMPAARating = new DataColumn();
            colMPAARating.DataType = typeof(string);
            colMPAARating.ColumnName = "MPAARating";
            table.Columns.Add(colMPAARating);

            DataColumn colSynopsis = new DataColumn();
            colSynopsis.DataType = typeof(string);
            colSynopsis.ColumnName = "Synopsis";
            table.Columns.Add(colSynopsis);

            DataColumn colDistributor = new DataColumn();
            colDistributor.DataType = typeof(string);
            colDistributor.ColumnName = "Distributor";
            table.Columns.Add(colDistributor);

            DataColumn colCountry = new DataColumn();
            colCountry.DataType = typeof(string);
            colCountry.ColumnName = "Country";
            table.Columns.Add(colCountry);

            DataColumn colWebsite = new DataColumn();
            colWebsite.DataType = typeof(string);
            colWebsite.ColumnName = "Website";
            table.Columns.Add(colWebsite);

            DataColumn colReleaseDate = new DataColumn();
            colReleaseDate.DataType = typeof(DateTime);
            colReleaseDate.ColumnName = "ReleaseDate";
            table.Columns.Add(colReleaseDate);

            DataColumn colDateAdded = new DataColumn();
            colDateAdded.DataType = typeof(DateTime);
            colDateAdded.ColumnName = "DateAdded";
            table.Columns.Add(colDateAdded);

            DataColumn colSource = new DataColumn();
            colSource.DataType = typeof(string);
            colSource.ColumnName = "Source";
            table.Columns.Add(colSource);

            DataColumn colActors = new DataColumn();
            colActors.DataType = typeof(List<string>);
            colActors.ColumnName = "Actors";
            table.Columns.Add(colActors);

            DataColumn colCrew = new DataColumn();
            colCrew.DataType = typeof(List<string>);
            colCrew.ColumnName = "Crew";
            table.Columns.Add(colCrew);

            Tables.Add(table);
        }
        public DataRow NewRow()
        {
            return this.Tables[0].NewRow();
        }
        public void AddRow(DataRow row)
        {
            this.Tables[0].Rows.Add(row);
        }
        public ArrayList GetColumnNames()
        {
            ArrayList column_names = new ArrayList();
            DataColumnCollection dcc = this.Tables[0].Columns;
            foreach (DataColumn col in dcc)
            {
                column_names.Add(col.ColumnName);
            }
            return column_names;
        }
    }
}
