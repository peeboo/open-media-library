using System;
using System.Collections.Generic;
using OMLEngine;

namespace AmazonMetaData2
{
    class AmazonSearchResult
    {
        public AmazonSearchResult(Title[] dvdList, int totalPages, int totalItems)
        {
            m_DVDList = dvdList;
            m_TotalPages = totalPages;
            m_TotalItems = totalItems;
        }

        // public properties
        public Title[] DVDList { get { return m_DVDList; } }
        public int TotalPages { get { return m_TotalPages; } }
        public int TotalItems { get { return m_TotalItems; } }

        // private data
        private int m_TotalItems;
        private int m_TotalPages;
        Title[] m_DVDList;
    }
}
