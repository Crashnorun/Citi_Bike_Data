using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

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

        static void UnZIPFile(string FileName)
        {

        }
        // unzip file
        // read documents in zip file
        // save documents in zip file
        // 
    }
}
