using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;                // used to create directory
using System.IO;                        // used to create directory
using System.Xml;                       // used to extract xml data
using System.Net;                       // used to download zip files
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Citi_Bike_Data_01.Classes;

namespace Citi_Bike_Data_01
{
    public class cls_Helper
    {
        /// <summary>
        /// Calculate if the date is a holiday or not
        /// </summary>
        /// <param name="date">DateTime</param>
        /// <reference>https://www.codeproject.com/Tips/1168428/US-Federal-Holidays-Csharp</reference>
        /// <returns>TRUE if date is a holiday, FALSE if date is not a holiday</returns>
        public static bool IsHoliday(DateTime date)
        {
            int nthWeekDay = (int)(Math.Ceiling((double)date.Day / 7.0d));
            DayOfWeek dayName = date.DayOfWeek;
            bool isThursday = dayName == DayOfWeek.Thursday;
            bool isFriday = dayName == DayOfWeek.Friday;
            bool isMonday = dayName == DayOfWeek.Monday;
            bool isWeekend = dayName == DayOfWeek.Saturday || dayName == DayOfWeek.Sunday;

            // New Years Day (Jan 1, or preceding Friday/following Monday if weekend)
            if ((date.Month == 12 && date.Day == 31 && isFriday) ||
                (date.Month == 1 && date.Day == 1 && !isWeekend) ||
                (date.Month == 1 && date.Day == 2 && isMonday)) return true;

            // MLK day (3rd monday in January)
            if (date.Month == 1 && isMonday && nthWeekDay == 3) return true;

            // President’s Day (3rd Monday in February)
            if (date.Month == 2 && isMonday && nthWeekDay == 3) return true;

            // Memorial Day (Last Monday in May)
            if (date.Month == 5 && isMonday && date.AddDays(7).Month == 6) return true;

            // Independence Day (July 4, or preceding Friday/following Monday if weekend)
            if ((date.Month == 7 && date.Day == 3 && isFriday) ||
                (date.Month == 7 && date.Day == 4 && !isWeekend) ||
                (date.Month == 7 && date.Day == 5 && isMonday)) return true;

            // Labor Day (1st Monday in September)
            if (date.Month == 9 && isMonday && nthWeekDay == 1) return true;

            // Columbus Day (2nd Monday in October)
            if (date.Month == 10 && isMonday && nthWeekDay == 2) return true;

            // Veteran’s Day (November 11, or preceding Friday/following Monday if weekend))
            if ((date.Month == 11 && date.Day == 10 && isFriday) ||
                (date.Month == 11 && date.Day == 11 && !isWeekend) ||
                (date.Month == 11 && date.Day == 12 && isMonday)) return true;

            // Thanksgiving Day (4th Thursday in November)
            if ((date.Month == 11 && isThursday && nthWeekDay == 4) ||
                (date.Month == 11 && isFriday && nthWeekDay == 5)) return true;

            // Christmas Day (December 25, or preceding Friday/following Monday if weekend))
            if ((date.Month == 12 && date.Day == 24 && isFriday) ||
                (date.Month == 12 && date.Day == 25 && !isWeekend) ||
                (date.Month == 12 && date.Day == 26 && isMonday)) return true;

            return false;
        }
        //-----------------------------------------------------------------------

        /*
         * download files
         * unzip files
         * save file names to data base
         */

        /// <summary>
        /// 
        /// </summary>
        /// <reference>Does table exist:https://stackoverflow.com/questions/2923452/c-sharp-sql-create-table-if-it-doesnt-already-exist </reference>
        /// <reference>Create connection to database: https://stackoverflow.com/questions/12220865/connecting-to-local-sql-server-database-using-c-sharp </reference>
        /// <reference>https://stackoverflow.com/questions/6671067/retrieve-list-of-tables-from-specific-database-on-server-c-sharp</reference>
        /// <returns></returns>
        public static bool AddFileNamesToTable(List<string> FileNames)
        {
            string tableName = "FileNames";
            SqlConnection connection = new SqlConnection(@"Data Source = (LocalDB)\MSSQLLocalDB;" +
                @"AttachDbFilename = E:\_My Stuff\Projects\Citi Bike Data\Citi_Bike_Data_01\Citi_Bike_Data_01\Citi_Bike_Data_01.mdf;" +
                "Integrated Security = True");
            
            connection.Open();

            //DataTable schema = connection.GetSchema("Tables");
            //List<string> TableNames = new List<string>();
            //foreach (DataRow row in schema.Rows)
            //{
            //    TableNames.Add(row[2].ToString());
            //}

            int count = 2;
            foreach (string fileName in FileNames)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT " + tableName + 
                    " (FileName, TableNumber)" +
                    " VALUES ('" + fileName + "', " + count + ")";
                cmd.Connection = connection;
                cmd.ExecuteNonQuery();
                count++;
            }
            connection.Close();

           /* DataTable schema = connection.GetSchema("Tables");
            List<string> TableNames = new List<string>();
            foreach(DataRow row in schema.Rows)
            {
                TableNames.Add(row[2].ToString());
            }*/

/*            using (SqlCommand cmd = connection.CreateCommand())
            {
                DataTable schema = connection.GetSchema("Tables");
                if (schema.Rows.Count == 0)
                {
                    DataColumn column;
                    DataRow row;

                    cls_File_Info fileinfo = new cls_File_Info("testFile", DateTime.Today);
                    System.Reflection.PropertyInfo[] properties = typeof(cls_File_Info).GetProperties();            // get a list of properties in the class

                    for (int i = 0; i < typeof(cls_File_Info).GetProperties().Count(); i++)                         // for each property add a column
                    {
                        column = new DataColumn();
                        column.DataType = System.Type.GetType(properties[i].GetType().ToString());                  // specify column type
                        column.ColumnName = properties[i].Name;                                                     // specify column name
                        schema.Columns.Add(column);                                                                 // add column
                    }

                    row = schema.NewRow();                                                                          // create new row
                    foreach (var prop in fileinfo.GetType().GetProperties())
                    {
                        row[prop.Name] = prop.GetValue(fileinfo);
                        // row[i] = fileinfo..properties[i];

                    }
                    schema.Rows.Add(row);
                }
            }
            */
           // connection.Close();
            return false;
        }


    }

    public static class cls_Sync_Data
    {
        public static List<string> FileNamesWeb;
        public static List<string> FileNamesLocal;
        public static List<string> FileNamesNew;
        public static string DestinationFolder;

        /// <summary>
        /// Get the executing assembly folder path. This path will be used to save the .zip files locally
        /// </summary>
        /// <returns>String - Return the folder path of the executing assembly</returns>
        public static string GetExecutingAssemblyPath()
        {
            // find assembly directory
            Assembly localAssembly = Assembly.GetExecutingAssembly();           // get the local assembly
            string codeBase = localAssembly.CodeBase;                           // get the code base
            UriBuilder uri = new UriBuilder(codeBase);                          // create new URI
            string path = Uri.UnescapeDataString(uri.Path);                     // get path from URI
            string folderPath = System.IO.Path.GetDirectoryName(path);          // get the folder path
            return folderPath;
        }
        //-----------------------------------------------------------------------

        /// <summary>
        /// Check if the data folder exists. Create it if it doesn't exist
        /// </summary>
        /// <param name="FolderPath">Assembly folder path as string</param>
        /// <returns>String - Data folder path as string</returns>
        public static string GetDataFolder(string FolderPath)
        {
            string dataFolder = @"\Original Data Files";
            if (Directory.Exists(FolderPath + dataFolder))          // check if the directory exists
            {
                return FolderPath + dataFolder;                     // retrun directory
            }
            else                                                    // if the directory doesn't exist
            {
                DirectoryInfo directoryInfo = Directory.CreateDirectory(FolderPath + dataFolder);       // create the directory
                DestinationFolder = directoryInfo.FullName;         // get the directory name
                return DestinationFolder;                           // return directory
            }
        }
        //-----------------------------------------------------------------------

        /// <summary>
        /// <reference>https://stackoverflow.com/questions/124492/c-sharp-httpwebrequest-command-to-get-directory-listing</reference>
        /// <reference>https://stackoverflow.com/questions/7496913/how-to-load-xml-from-url-on-xmldocument</reference>
        /// </summary>
        public static List<string> GetListOfFileNames(string XMLWebAddress)
        {
            FileNamesWeb = new List<string>();
            string xmlStr;   

            using (var wc = new WebClient())
            {
                xmlStr = wc.DownloadString(XMLWebAddress);              // get the xml from the website
            }
            XmlDocument doc = new XmlDocument();                        // create new xml document
            doc.LoadXml(xmlStr);                                        // load string into document

            XmlNodeList nodeList = doc.GetElementsByTagName("Key");     // search document for KEY

            foreach (XmlNode node in nodeList)                          // go through each node
            {
                if (node.InnerXml.Contains(".zip"))                     // if the name contains a .zip
                    FileNamesWeb.Add(node.InnerXml);                       // save file name
            }
            // create a txt
            #region OPTION 02
            /* XmlTextReader reader = new XmlTextReader(XMLWebAddress);            //https://support.microsoft.com/en-us/help/307643/how-to-read-xml-data-from-a-url-by-using-visual-c
             while (reader.Read())
             {
                 switch (reader.NodeType)
                 {
                     case XmlNodeType.Element:
                         if(reader.Name == "Key")
                         {
                             fileNames.Add(reader.Value);
                         }
                         //Debug.Print( reader.Name);
                         break;
                     case XmlNodeType.Text:
                        // Debug.Print(reader.Value);
                         break;
                     case XmlNodeType.EndElement:
                         Debug.Print(  reader.Name);
                         break;
                 }
             }*/
            #endregion

            #region OPTION 03
            /*HttpWebRequest request = (HttpWebRequest)WebRequest.Create(WebAddress);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string html = reader.ReadToEnd();
                    Regex regex = new Regex(WebAddress);
                    regex = new Regex("<a href=\".*\">(?<name>.*)</a>");
                    MatchCollection matches = regex.Matches(html);
                    if (matches.Count > 0)
                    {
                        foreach (Match match in matches)
                        {
                            if (match.Success)
                            {
                                Debug.Print(match.Groups["name"].ToString());
                            }
                        }
                    }
                }
            }*/
            #endregion
            return FileNamesWeb;
        }
        //-----------------------------------------------------------------------

        /// <summary>
        /// get the unique names, and download new files
        /// </summary>
        /// <param name="FileNamesWeb">List of file names listed on the amazon website</param>
        /// <param name="FileNamesLocal">List of files names stored locally</param>
        /// <param name="XMLWebAddress">The URL where the XML data can be downloaded</param>
        /// <returns>A list of unique file names. If list is lenth=0, then no new file names</returns>
        public static List<string> CompareFileNames(List<string> FileNamesLocal, string XMLWebAddress)
        {
            List<string> FileNames = new List<string>();
            List<string> UniqueFileNames = new List<string>();

            foreach (string name in FileNamesWeb)                                           // go through all the file names listed in the xml on the web
            {
                string tempName = FileNamesLocal.FirstOrDefault(x => x.Contains(name));     // compare for unique file names
                if (string.IsNullOrEmpty(tempName))
                {
                    UniqueFileNames.Add(name);                                              // save unique names
                }
                else
                {
                    string fName = System.IO.Path.GetFileName(tempName);
                    FileNames.Add(fName);
                }
            }

            FileNamesWeb = UniqueFileNames;
            if (UniqueFileNames.Count > 0)
                DownloadFiles(UniqueFileNames, XMLWebAddress);                              // download unique names
            return UniqueFileNames;                                                         // return the list of unique file names
        }
        //-----------------------------------------------------------------------

        /// <summary>
        /// Download unique files from Amazon server
        /// </summary>
        /// <reverence>download file - https://msdn.microsoft.com/en-us/library/ez801hhe(v=vs.110).aspx</reverence>
        /// <param name="UniqueFileNames">List of files names to be downloaded</param>
        /// <param name="XMLWebAddress">The URL where the XML data can be downloaded</param>
        private static void DownloadFiles(List<string> UniqueFileNames, string XMLWebAddress)
        {
            string webaddress, destinationFile;

            using (var client = new WebClient())
            {
                foreach (string uniqueName in UniqueFileNames)                                  // go through all the new file names
                {
                    //https://s3.amazonaws.com/tripdata/201307-201402-citibike-tripdata.zip
                    webaddress = XMLWebAddress + uniqueName;                                    // get the url of the new file name
                    destinationFile = DestinationFolder + @"\" + uniqueName;                    // create full file name path
                    client.DownloadFile(webaddress, destinationFile);                           // download the file
                }
            }
        }
        //-----------------------------------------------------------------------
    }
}
