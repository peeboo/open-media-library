using System;
using System.Collections.Generic;
using OMLEngine;

namespace OMLSDK
{
    public interface IOMLPlugin
    {
        string GetName();
        string GetDescription();
        string GetAuthor();
        int GetTotalTitlesAdded();
        List<Title> GetTitles();
        Title newTitle();
        void AddTitle(Title newTitle);
        bool ValidateTitle(Title title_to_validate);
        bool IsSupportedFormat(string file_extension);
        bool Load(string filename, bool ShouldCopyImages);
    }
}
