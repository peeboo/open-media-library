using System;
using System.Data;
using System.Collections;

namespace OMLSDK
{
    public interface IDataImporter
    {
        OMLDataSet GetOmlDataSet();
    }
}
