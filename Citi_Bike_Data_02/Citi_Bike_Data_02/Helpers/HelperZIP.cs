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
using Citi_Bike_Data_01.Classes;

namespace Citi_Bike_Data_02.Helper
{
    /*
     * Download ZIP file from url
     * Unzip File - return list of csv file names in the zip file
     * Read CSV
     * Delete file
     * Create DataTable From CSV
     * ExtractDateTimeFromFileName
     */

    /* 
     * Need to add Unique ID Key column to data table
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
            Debug.Print("DOWNLOADING ZIP FILE: " + FileName);

            // construct url, Example url: https://s3.amazonaws.com/tripdata/201306-citibike-tripdata.zip
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
            Debug.Print("EXTRACTING CSV FILES");
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
            Debug.Print("DELETING FILE: " + fileName);
            try
            {
                File.Delete(DirectoryPath + "\\" + fileName);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message + Environment.NewLine + ex.StackTrace.ToString());
            }
        }

        /// <summary>
        /// Create DataTable from CSV file
        /// </summary>
        /// <param name="FilePath">File path where the csv file is saved</param>
        /// <param name="StartingUniqueId"></param>
        /// <returns>DataTable from the CSV file</returns>
        public static DataTable CreateDataTableFromCSV(string FilePath, int StartingUniqueId, Dictionary<string, Type> DBSchema, ref List<cls_Station> Stations)
        {
            Debug.Print("CONVERTING CSV: " + FilePath + " INTO DATATABLE");

            string fileName = Path.GetFileNameWithoutExtension(FilePath);                       // get the file name
            DataTable dt = CreateDatatableFromSchema(DBSchema, fileName);                       // create new data table

            List<string> csvFile = new List<string>();
            csvFile = ReadCSVFile(FilePath);                                                    // get csv data

            List<string> headerRow = new List<string>();
            headerRow = csvFile[0].ToLower().Replace("\"", "")
                .Replace(" ", "").Split(',').ToList();                                          // get header rows, remove spaces and quotes

            for (int i = 1; i < csvFile.Count; i++)                                             // skip the header row
            {
                DataRow dr = dt.NewRow();                                                       // will hold the ordered data
                List<string> trip = csvFile[i].Replace("\"", "").Split(',').ToList();           // remove quotes, split row into columns

                AddStationToList(trip, headerRow, ref Stations);                                // add unique stations to list 

                foreach (DataColumn col in dt.Columns)
                {
                    if (col.ColumnName.ToLower() == "id")
                        dr[col.ColumnName] = StartingUniqueId + i - 1;
                    else if (col.ColumnName.ToLower() == "date")
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
                                    if (trip[index].ToUpper() == "NULL")
                                        dr[col.ColumnName] = -1;                // insert -1 for nulls
                                    else
                                        dr[col.ColumnName] = Convert.ToInt32(trip[index]);
                                    break;
                                case "starttime":
                                case "stoptime":
                                    if (trip[index].ToUpper() == "NULL")
                                        dr[col.ColumnName] = new DateTime();
                                    else
                                        dr[col.ColumnName] = Convert.ToDateTime(trip[index]);
                                    break;
                                //case "startstationlatitude":
                                //case "startstationlongitude":
                                //case "endstationlatitude":
                                //case "endstationlongitude":
                                //if (trip[index].ToUpper() == "NULL")
                                //    dr[col.ColumnName] = -1;
                                //else
                                //    dr[col.ColumnName] = Convert.ToDouble(trip[index]);
                                //break;
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

        public static DataTable CreateDatatableFromSchema(Dictionary<string, Type> Schema, string TableName)
        {
            DataTable dt = new DataTable(TableName);                                            // create new data table
            foreach (string key in Schema.Keys)
                dt.Columns.Add(key, Schema[key]);                                               // create columns in dt from db
            return dt;
        }

        /// <summary>
        /// Extract start and end stations from the TRIP.
        /// Add start and end stations to the list of Stations if they do not exsits 
        /// </summary>
        /// <param name="Trip">List of strings identifying a trip</param>
        /// <param name="HeaderRow">Header row from CSV file</param>
        /// <param name="Stations">List of unique stations</param>
        public static void AddStationToList(List<string> Trip, List<string> HeaderRow, ref List<cls_Station> Stations)
        {
            cls_Station StartStation = new cls_Station();
            cls_Station EndStation = new cls_Station();

            for (int i = 0; i < Trip.Count; i++)                    // extract start and end stations from trip
            {
                int tempNum = -1;
                switch (HeaderRow[i].ToLower())
                {
                    case "startstationid":
                        int.TryParse(Trip[i], out tempNum);
                        StartStation.StationID = tempNum;
                        break;
                    case "startstationname":
                        StartStation.StationName = Trip[i];
                        break;
                    case "startstationlatitude":
                        if (Trip[i].ToUpper() == "NULL") StartStation.StationLatitude = 0;
                        else StartStation.StationLatitude = Convert.ToDouble(Trip[i]);
                        break;
                    case "startstationlongitude":
                        if (Trip[i].ToUpper() == "NULL") StartStation.StationLongitude = 0;
                        else StartStation.StationLongitude = Convert.ToDouble(Trip[i]);
                        break;
                    case "endstationid":
                        int.TryParse(Trip[i], out tempNum);
                        EndStation.StationID = tempNum;
                        break;
                    case "endstationname":
                        EndStation.StationName = Trip[i];
                        break;
                    case "endstationlatitude":
                        if (Trip[i].ToUpper() == "NULL") EndStation.StationLatitude = 0;
                        else EndStation.StationLatitude = Convert.ToDouble(Trip[i]);
                        break;
                    case "endstationlongitude":
                        if (Trip[i].ToUpper() == "NULL") EndStation.StationLongitude = 0;
                        else EndStation.StationLongitude = Convert.ToDouble(Trip[i]);
                        break;
                }
            }

            if (Stations.FindIndex(x => x.StationID == StartStation.StationID) == -1)
                Stations.Add(StartStation);                                 // add start station

            if (Stations.FindIndex(x => x.StationID == EndStation.StationID) == -1)
                Stations.Add(EndStation);                                   // add end station
        }


        /// <summary>
        /// Extract the date from the CSV file name to a DateTime
        /// </summary>
        /// <param name="FileName">CSV file name</param>
        /// <returns>DateTime</returns>
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
