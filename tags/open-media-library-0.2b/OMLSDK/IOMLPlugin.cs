using System;
using System.Collections.Generic;
using OMLEngine;

namespace OMLSDK
{
    public interface IOMLPlugin
    {
        IList<Title> GetTitles();
        Title newTitle();
        void AddTitle(Title newTitle);
        bool ValidateTitle(Title title_to_validate);
        bool IsSupportedFormat(string file_extension);
        bool Load(string filename, bool ShouldCopyImages);
        void DoWork(string[] thework);
        void ProcessDir(string fPath);
        void ProcessFiles(string[] sFiles);
        void ProcessFile(string file);
        bool IsSingleFileImporter();
        string SetupDescription();
    }
}
