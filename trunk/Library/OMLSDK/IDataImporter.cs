using System;
using System.Data;
using System.Collections;

namespace Library.OMLSDK
{
    public interface IDataImporter
    {
        OMLDataset getDataSet();
    }
}
