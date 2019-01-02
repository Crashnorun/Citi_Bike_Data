using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Data;

namespace Citi_Bike_Data_02.Helper
{
    /*
     * Download ZIP file from url
     * Unzip File - return list of csv file names in the zip file
     * Read CSV
     * Delete file
     */

    /* 
     * Create function that creates a Datatable
     *      Match columns in datatable with columns in CSV file
     *      Need a datatable schema -> convert headers accordingly
     *      Convert csv data into data table
     * Populate data into DB
     */

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
        public static bool UnZIPFile(string FileName, out List<string> FilePaths)
        {
            string path = Environment.CurrentDirectory + "\\" + FileName;
            List<string> fileNames = new List<string>();

            using (ZipArchive archive = ZipFile.OpenRead(path))                                 // open zip file
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    // if entries contain .csv and is not a macos file
                    if (entry.FullName.EndsWith(".csv", StringComparison.CurrentCultureIgnoreCase) && !entry.FullName.Contains("MACOS"))
                    {
                        try
                        {
                            string filePath = Path.Combine(Environment.CurrentDirectory, entry.FullName);

                            // the zip files contain a csv file and a macOS csv file, we only need one of them.
                            if (!fileNames.Contains(entry.FullName) && !File.Exists(filePath))
                            {
                                fileNames.Add(entry.FullName);                                      // save file name
                                entry.ExtractToFile(filePath);                                      // extract csv
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.Print(ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                            FilePaths = null;
                            return false;
                        }
                    }
                }
                FilePaths = fileNames;
                return true;
            }
        }

        /// <summary>
        /// Read teh CSV file line by line
        /// </summary>
        /// <reference>https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-read-from-a-text-file</reference>
        /// <param name="FilePath"></param>
        /// <returns>List of strings </returns>
        public static List<string> ReadCSVFile(string FilePath)
        {
            List<string> csvData = new List<string>();
            csvData = File.ReadAllLines(FilePath).ToList();
            return csvData;
        }

        /// <summary>
        /// Delete file from a directory
        /// </summary>
        /// <param name="DirectoryPath"></param>
        /// <param name="fileName"></param>
        public static void DeleteFile(string DirectoryPath, string fileName)
        {
            try
            {
                File.Delete(DirectoryPath + "\\" + fileName);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message + Environment.NewLine + ex.StackTrace.ToString());
            }
        }

        public static DataTable CreateDataTableFromCSV(string FilePath)
        {
            Dictionary<string, Type> DBSchema = new Dictionary<string, Type>();
            DBSchema = HelperDB.GetTableSchema(Properties.Resources.TableTrips);                // get the db table schema

            string fileName = Path.GetFileNameWithoutExtension(FilePath);                       // get the file name
            DataTable dt = new DataTable(fileName);                                             // create new data table

            foreach (string key in DBSchema.Keys)
                dt.Columns.Add(key, DBSchema[key]);                                             // create columns in dt from db
            dt.Columns.RemoveAt(0);                                                             // remove Unique ID column

            List<string> csvFile = new List<string>();
            csvFile = ReadCSVFile(FilePath);                                                    // get csv data

            List<string> headerRow = new List<string>();
            headerRow = csvFile[0].ToLower().Replace("\"", "")
                .Replace(" ", "").Split(',').ToList();                                          // get header rows, remove spaces and quotes

            for (int i = 1; i < csvFile.Count; i++)                                             // skip the header row
            {
                DataRow dr = dt.NewRow();                                                       // will hold the ordered data
                List<string> trip = csvFile[i].Replace("\"", "").Split(',').ToList();           // remove quotes, split row into columns

                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName.ToLower() == "date")
                        dr[col.ColumnName] = ExtractDateTimeFromFileName(fileName);
                    else
                    {
                        int index = headerRow.FindIndex(x => x.ToLower().Equals(col.ColumnName.Replace("\"", "").ToLower())); // find the matching column
                        if (index > -1)
                        {
                            switch (headerRow[index])
                            {
                                case "tripduration":
                                case "startstationid":
                                case "endstationid":
                                case "bikeid":
                                case "gender":
                                    dr[col.ColumnName] = Convert.ToInt32(trip[index]);
                                    break;
                                case "starttime":
                                case "stoptime":
                                    dr[col.ColumnName] = Convert.ToDateTime(trip[index]);
                                    break;
                                case "startstationlatitude":
                                case "startstationlongitude":
                                case "endstationlatitude":
                                case "endstationlongitude":
                                    dr[col.ColumnName] = Convert.ToDouble(trip[index]);
                                    break;
                                default:
                                    dr[col.ColumnName] = trip[index];
                                    break;
                            }
                        }
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }


        public static DateTime ExtractDateTimeFromFileName(string FileName)
        {
            string NewStr = "";
            foreach (char c in FileName)
            {
                if (Char.IsNumber(c))                                   // keep only the numbers
                    NewStr += c;
            }

            int year = Convert.ToInt32(NewStr.Substring(0, 4));         // first 4 characters are the year
            int month = Convert.ToInt32(NewStr.Substring(4, 2));        // last 2 characters are the month
            return new DateTime(year, month, 1);
        }
    }
}
