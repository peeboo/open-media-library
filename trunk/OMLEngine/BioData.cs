using System;
using System.Collections.Generic;
using Dao = OMLEngine.Dao;
using System.Text;

namespace OMLEngine
{
    public class BioData
    {
        private Dao.BioData _bioData;

        public BioData()
        {
            _bioData = new OMLEngine.Dao.BioData();
        }

        internal BioData(OMLEngine.Dao.BioData bioData)
        {
            _bioData = bioData;
        }

        public long Id
        {
            get { return _bioData.Id; }
            set { _bioData.Id = value; }
        }
        public DateTime? DateOfBirth
        {
            get { return _bioData.DateOfBirth; }
            set { _bioData.DateOfBirth = value; }
        }
        public string FullName
        {
            get { return _bioData.FullName; }
            set { _bioData.FullName = value; }
        }

        public override string ToString()
        {
            return _bioData.FullName;
        }
        /*public long Id
        {
            get { return _bioData.Photo; }
            set { _bioData.Id = value; }
        }*/

    }
}
