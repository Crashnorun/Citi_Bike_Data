using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.IO;


namespace Citi_Bike_Data_02.Classes
{
    [Serializable]
    public class cls_ZIPFile
    {

        #region ----PROPERTIES----

        public string ZIPFileName { get; set; }
        public List<string> CSVFileNames { get; set; }
        public string FullFolderPath { get; set; }

        #endregion


        /// <summary>
        /// Blank constructor
        /// </summary>
        public cls_ZIPFile() { }


        public cls_ZIPFile(string zipFileName)
        {
            this.ZIPFileName = zipFileName;
        }


        public cls_ZIPFile(string zipFileName, List<string> csvFileNames)
        {
            this.ZIPFileName = zipFileName;
            this.CSVFileNames = csvFileNames;
        }


        /// <summary>
        /// Extract CSV file names from ZIP file
        /// </summary>
        /// <param name="zipFilePath">ZIP File</param>
        /// <param name="destinationFolder">Destination folder where to extract files</param>
        /// <reference>https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.zipfile?view=netframework-4.7.2</reference>
        /// <returns>List of CSV file names</returns>
        public static List<string> UnZipFile(string zipFilePath, string destinationFolder = "")
        {
            List<string> FilePaths = new List<string>();                                        // store full file path
            List<string> FileNames = new List<string>();                                        // store file names

            if (destinationFolder == "")                                                        // if no destination folder is specified
                destinationFolder = HelperDB.HelperDB.GetExecutingAssemblyPath();               // get executing assembly folder
            else if (!Directory.Exists(destinationFolder))                                      // if folder does not exist
                Directory.CreateDirectory(destinationFolder);                                   // create folder

            ZipFile.ExtractToDirectory(zipFilePath, destinationFolder);                         // extract zip files
            FilePaths = Directory.GetFiles(destinationFolder).ToList();                         // get extracted files

            foreach (string filePath in FilePaths)                                              // save .CSV files
            {
                var folders = filePath.Split('\\');

                if (folders[folders.Length - 1].ToLower().Contains(".csv"))
                    FileNames.Add(folders[folders.Length - 1].ToLower());
            }

            return FileNames;
        }

    }

}
