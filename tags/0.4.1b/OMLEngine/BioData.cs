using System;
using System.Collections.Generic;
using Dao = OMLEngine.Dao;
using System.Text;
using System.IO;
using System.Drawing;

namespace OMLEngine
{
    public class BioData
    {
        private Dao.BioData _bioData;

        string _imagePath;

        public BioData()
        {
            _bioData = new OMLEngine.Dao.BioData();

            // Set a default date to stop exception (must change type from smalldate in db)
            _bioData.DateOfBirth = new DateTime(1900, 1, 1);
        }

        public void ReloadBioData()
        {
            Dao.DBContext.Instance.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, _bioData);
        }

        internal BioData(OMLEngine.Dao.BioData bioData)
        {
            _bioData = bioData;
        }

        internal Dao.BioData DaoBioData
        {
            get { return _bioData; }
        }

        #region Properties
        public long Id
        {
            get { return _bioData.Id; }
            set { 
                _bioData.Id = value;
                ModifiedDate = DateTime.Now;
            }
        }

        public DateTime? DateOfBirth
        {
            get { return _bioData.DateOfBirth; }
            set { 
                _bioData.DateOfBirth = value;
                ModifiedDate = DateTime.Now;
            }
        }

        public string FullName
        {
            get { return _bioData.FullName; }
            set
            {
                _bioData.FullName = value;
                ModifiedDate = DateTime.Now;
            }
        }

        public string Biography
        {
            get { return _bioData.Biography; }
            set { 
                _bioData.Biography = value;
                ModifiedDate = DateTime.Now;
            }
        }

        public DateTime? ModifiedDate
        {
            get { return _bioData.ModifiedDate; }
            set { _bioData.ModifiedDate = value; }
        }


        public string ImagePath
        {
            get
            {
                if (_bioData.UpdatedImagePath != null)
                    return _bioData.UpdatedImagePath;

                if (_imagePath == null)
                {
                    string path = ImageManager.GetImagePathById(_bioData.PhotoID, ImageSize.Original);
                    
                    if (File.Exists(path))
                        _imagePath = path;
                }

                return _imagePath;
            }
            set
            {
                if (value.Length > 255)
                    throw new FormatException("ImagePath must be 255 characters or less.");

                _bioData.UpdatedImagePath = value;
                _imagePath = null;
                _bioData.ModifiedDate = DateTime.Now;
            }
        }
        /*public string ImagePath
        {
            get
            {
                if (_bioData.UpdatedImagePath != null)
                    return _bioData.UpdatedImagePath;

                if (_imagePath == null)
                {
                    _imagePath = Path.Combine(FileSystemWalker.ImageDirectory, "bio" + Id.ToString() + ".png");

                    if (!File.Exists(_imagePath))
                    {
                        // grab the image from the db
                        Image image = null;

                        try
                        {
                            image = Image.FromStream(new MemoryStream(_bioData.Photo.ToArray()));
                            image.Save(_imagePath);
                        }
                        catch
                        {
                            _imagePath = null;
                        }
                    }
                }

                return _imagePath;
            }
            set
            {
                if (value.Length > 255)
                    throw new FormatException("ImagePath must be 255 characters or less.");

                Image image = Image.FromFile(value);
                byte[] imageb = ImageManager.ImageToByteArray(image);
                _bioData.Photo = imageb;

                _bioData.UpdatedImagePath = value;
                _imagePath = null;
                _bioData.ModifiedDate = DateTime.Now;
            }
        }*/


        #endregion

        public override string ToString()
        {
            return _bioData.FullName;
        }
    }
}
