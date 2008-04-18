using System;
using System.Data;
using System.Collections;

namespace OMLEngine
{
    public interface IDataImporter
    {
        DataSet getDataSet();
        IList getCrew();
        IList getActors(DataRow dataRow);
    }
}
