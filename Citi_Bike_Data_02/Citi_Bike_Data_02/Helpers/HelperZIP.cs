﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;

namespace Citi_Bike_Data_02.Helper
{
    public static class HelperZIP
    {
        /// <summary>
        /// Download a ZIP file from the web and save it in the Environment.CurrentDirectory
        /// </summary>
        /// <reference>https://stackoverflow.com/questions/4769032/how-do-i-download-zip-file-in-c</reference>
        /// <param name="FileName">Filename to download</param>
        /// <returns>TRUE = success, FALSE = failure</returns>
        public static bool DownloadZIPFile(string FileName)
        {
            // Example url: https://s3.amazonaws.com/tripdata/201306-citibike-tripdata.zip
            // construct url
            string url = Properties.Resources.URLXML + "/" + FileName;
            string path = Environment.CurrentDirectory + "\\" + FileName;

            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile(url, path);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// Unzip file to Environment.CurrentDirectory
        /// </summary>
        /// <reference>https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.zipfile.extracttodirectory?view=netframework-4.7.2</reference>
        /// <reference>https://social.msdn.microsoft.com/Forums/en-US/c3be2f07-821e-482c-b9e5-0c8add5506e0/reading-contents-of-a-zip-file-within-a-zipfile-without-extracting?forum=csharpgeneral</refernce>
        /// <param name="FileName">File name to save</param>
        /// <param name="FilePath">File path where document(s) saved</param>
        public static bool UnZIPFile(string FileName, out string FilePath)
        {
            string path = Environment.CurrentDirectory + "\\" + FileName;

            using (ZipArchive archive = ZipFile.OpenRead(path))
            {
                foreach(ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase))
                    {
                        FilePath = Path.Combine(Environment.CurrentDirectory, entry.FullName);
                        entry.ExtractToFile(FilePath);
                    }
                }
            }

            /*try
            {
                ZipFile.ExtractToDirectory(path, Environment.CurrentDirectory);
                FilePath = Environment.CurrentDirectory + "\\" + FileName;
                return true;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                FilePath = string.Empty;
                return false;
            } */
        }


        /// <summary>
        /// Gets the list of files from a directory
        /// </summary>
        /// <reference>https://docs.microsoft.com/en-us/dotnet/api/system.io.directory.getfiles?view=netframework-4.7.2</reference>
        /// <param name="DirectoryPath"> Directory Path </param>
        /// <param name="Files"> List of filenames found in directory </param>
        /// <returns> Number of files found, -1 on error </returns>
        public static int NumberOfUnzipedFiles(string DirectoryPath, out List<string> Files)
        {
            try
            {
                List<string> files = Directory.GetFiles(DirectoryPath).ToList();
                Files = files;
                return files.Count;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Files = null;
                return -1;
            }
        }

        public static List<string> ReadCSVFiles(string FilePath)
        {
            List<string> csvData = new List<string>();

            using (StreamReader reader = new StreamReader(FilePath))
            {
                while (!reader.EndOfStream)
                {
                    csvData.Add(reader.ReadLine());
                }
            }
            return csvData;
        }


        public static void DeleteCSVFile(string DirectoryPath, string fileName)
        {
            try
            {
                File.Delete(DirectoryPath + "\\" + fileName);
            } catch(Exception ex)
            {
                Debug.Print(ex.Message + Environment.NewLine + ex.StackTrace.ToString());
            }
        }

        // open new directory
        // read csv files
        // read documents in zip file
        // save documents in zip file
        // 
    }
}
