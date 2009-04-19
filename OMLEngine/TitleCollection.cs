using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.ServiceModel;
using OMLEngineService;
using System.Security.AccessControl;
using System.Runtime.InteropServices;

namespace OMLEngine
{       
    /// <summary>
    /// This class is used to de-serialize the old OML dat file so it can be inserted into the new SQL store
    /// </summary>
    public class LegacyTitleCollection 
    {
        private List<Title> _list = new List<Title>();        
        private string _database_filename;
                                           
        public IList<Title> Source
        {
            get { return _list.AsReadOnly(); }
        }

        public bool OMLDatExists
        {
            get { return File.Exists(_database_filename); }
        }

        public LegacyTitleCollection()
        {            
            Utilities.DebugLine("[TitleCollection] set _database_filename");
            _database_filename = Path.Combine(FileSystemWalker.PublicRootDirectory, @"oml.dat");
        }        

        /// <summary>
        /// Loads data from OML Database
        /// </summary>
        /// <returns>True on successful load</returns>
        public bool LoadTitleCollectionFromOML()
        {
            Utilities.DebugLine("[TitleCollection] Using OML database");
            Stream stm;
            try
            {
                if (File.Exists(_database_filename) == false)
                    return false;
                stm = File.OpenRead(_database_filename);
            }
            catch (Exception ex)
            {
                Utilities.DebugLine("[TitleCollection] Error loading title collection: " + ex.Message);
                return false;
            }

            if (stm.Length > 0)
            {
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    int numTitles = (int)bf.Deserialize(stm);
                    for (int i = 0; i < numTitles; i++)
                    {
                        Title t = (Title)bf.Deserialize(stm);
                        Utilities.DebugLine("[TitleCollection] Adding Title: "+t.Name);
                        _list.Add(t);  
                    }                    
                    stm.Close();
                    Utilities.DebugLine("[TitleCollection] Loaded: " + numTitles + " titles");
                    return true;
                }
                catch (Exception e)
                {
                    Utilities.DebugLine("[TitleCollection] Failed to load db file: " + e.Message);
                }
            }
            return false;
        }

        public void RenameDATCollection()
        {
            if (File.Exists(_database_filename))
                File.Move(_database_filename, _database_filename + ".bak");
        }               
    }    
}
