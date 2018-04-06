using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citi_Bike_Data_01.Classes
{
   public class cls_File_Info
    {
        #region PROPERTIES
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public DateTime FileDate
        {
            get { return fileDate; }
            set { fileDate = value; }
        }

        public DateTime DownloadDate
        {
            get { return downloadDate; }
            set { downloadDate = value; }
        }

        public int NumberOfTrips
        {
            get { return numberOfTrips; }
            set{ numberOfTrips = value; }
        }

        public double FileSize
        {
            get { return fileSize; }
            set { fileSize = value; }
        }

        private string fileName;
        private DateTime fileDate;
        private DateTime downloadDate;
        private int numberOfTrips;              // number of trips in the file
        private double fileSize;                // file size
        #endregion

        public  cls_File_Info()
        {
        }

        public cls_File_Info(string fileName, DateTime fileDate)
        {
            this.fileDate = fileDate;
            this.FileName = FileName;
        }
    }
}
