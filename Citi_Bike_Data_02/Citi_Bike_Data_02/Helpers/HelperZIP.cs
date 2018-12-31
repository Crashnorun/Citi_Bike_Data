﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO.Compression;

namespace Citi_Bike_Data_02.Helpers
{
    public static class HelperZIP
    {
        /// <summary>
        /// Download a ZIP file from the web and save it in the Environment.CurrentDirectory
        /// </summary>
        /// <reference>https://stackoverflow.com/questions/4769032/how-do-i-download-zip-file-in-c</reference>
        /// <param name="FileName">Filename to download</param>
        /// <returns>TRUE = success, FALSE = failure</returns>
        static bool DownloadZIPFile(string FileName)
        {
            // Example url: https://s3.amazonaws.com/tripdata/201306-citibike-tripdata.zip
            // construct url
            string url = Properties.Resources.URLXML + "/" + FileName;
            string path = Environment.CurrentDirectory + "\\" + FileName;

            using (WebClient client = new WebClient())
            {
                try {
                    client.DownloadFile(url, path);
                    return true;
                }
                catch (Exception ex) {
                    Debug.Print(ex.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// Unzip file to Environment.CurrentDirectory
        /// </summary>
        /// <reference>https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.zipfile.extracttodirectory?view=netframework-4.7.2</reference>
        /// <param name="FileName">File name to save</param>
        /// <param name="FilePath">File path where document(s) saved</param>
        static bool UnZIPFile(string FileName, out string FilePath)
        {
            string path = Environment.CurrentDirectory + "\\" + FileName;
            try {
                ZipFile.ExtractToDirectory(path, Environment.CurrentDirectory);
                FilePath = Environment.CurrentDirectory + "\\" + FileName;
                return true;
            } catch(Exception ex)
            {
                Debug.Print(ex.Message);
                FilePath = string.Empty;
                return false;
            }
        }

        // open new directory
        // count number of files
        // get file names
        // read documents in zip file
        // save documents in zip file
        // 
    }
}