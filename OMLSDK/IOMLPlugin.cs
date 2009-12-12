using System;
using System.Collections.Generic;

namespace OMLSDK
{
    public interface IOMLPlugin
    {
        IList<OMLSDKTitle> GetTitles();
        OMLSDKTitle newTitle();
        void AddTitle(OMLSDKTitle newTitle);
        bool ValidateTitle(OMLSDKTitle title_to_validate);
        bool IsSupportedFormat(string file_extension);
        bool Load(string filename);
        void DoWork(string[] thework);
        void ProcessDir(string fPath);
        void ProcessFiles(string[] sFiles);
        void ProcessFile(string file);
        bool IsSingleFileImporter();
        string SetupDescription();
    }
}
