using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Net;
using System.Reflection;
using System.Xml.Linq;
using System.Xml;
using System.Diagnostics;
using System.Data;
using System.Resources;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Collections;
using Citi_Bike_Data_01.Classes;


namespace Citi_Bike_Data_02.UI
{

    public partial class UserControl1 : UserControl
    {

        #region ----PROPERTIES----
        public XDocument XMLDocument;
        public List<string> ZIPFileNamesOnline = new List<string>();
        public Dictionary<int, string> ZIPFileNamesDB;
        public List<cls_Station> Stations = new List<cls_Station>();
        #endregion

        #region ----FIELDS----

        private string lblMessage = string.Empty;

        #endregion


        #region ----UI----

        /// <summary>
        /// Set progress bar color, create resources file to hold DB connection string
        /// </summary>
        /// <reference>https://docs.microsoft.com/en-us/dotnet/api/system.resources.resxresourcewriter?view=netframework-4.7.2</reference>
        ///<reference>https://docs.microsoft.com/en-us/dotnet/framework/resources/working-with-resx-files-programmatically</reference> 
        public UserControl1()
        {
            InitializeComponent();

            lbl_Status_01.Content = string.Empty;
            lbl_Status_01.ToolTip = lbl_Status_01.Content;
            Color col = (Color)ColorConverter.ConvertFromString(Properties.Resources.CitiBikeColorHEX);         // get color from properties
            SolidColorBrush fill = new SolidColorBrush(col);                                                    // create a solid brush
            this.progressBar1.Foreground = fill;                                                                // apply brush to progress bar

            // Create .resx file to hold the Connection string
            if (!System.IO.File.Exists(Helper.HelperDB.GetExecutingAssemblyPath() + "\\CitiBikeResources.resx"))
            {
                using (ResXResourceWriter resxresourcewriter = new ResXResourceWriter(@".\CitiBikeResources.resx"))
                {
                    resxresourcewriter.AddResource("DBConnectionString", "");
                }
            }
            btn_ConnectDB.IsEnabled = false;
            btn_CreateTable.IsEnabled = false;
            btn_DlXML.IsEnabled = false;
            btn_CompareZipFiles.IsEnabled = false;

            #region ----NOTES----
            //Hashtable resourceEntries = new Hashtable();

            //using (ResXResourceReader reader = new ResXResourceReader(@".\CitiBikeResources.resx"))
            //{
            //    foreach (DictionaryEntry d in reader)
            //    {
            //        if(d.Key.ToString() == "DBConnectionString")
            //        {
            //            if(d.Value.ToString() == "")
            //                resourceEntries.Add(d.Key, "Bob");
            //            else
            //                resourceEntries.Add(d.Key, "");
            //        }
            //    }
            //}

            //using (ResXResourceWriter resxresourcewriter = new ResXResourceWriter(@".\CitiBikeResources.resx"))
            //{
            //    foreach (DictionaryEntry d in resourceEntries)
            //    {
            //        resxresourcewriter.AddResource(d.Key.ToString(), d.Value.ToString());
            //    }
            //}
            #endregion

        }


        /// <summary>
        /// Test connection to DB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_CreateDB_Click(object sender, RoutedEventArgs e)
        {
            string DBFilePath = Helper.HelperDB.CheckIfDBExists(Properties.Resources.DBName);
            if (DBFilePath == false.ToString().ToLower())
            {
                Helper.HelperDB.CreateNewDB(Properties.Resources.DBName, ref lblMessage);                     // create new DB
                lbl_Status_01.Content = lblMessage;
            }
            else           // find the location of the db
            {
                ////https://stackoverflow.com/questions/8146207/how-to-get-current-connected-database-file-path
                //SqlConnection conn = new SqlConnection(Properties.Resources.ConnectionStringBase);          // get the db File location
                //SqlCommand GetDataFile = new SqlCommand();
                //GetDataFile.Connection = conn;
                //GetDataFile.CommandText = "SELECT physical_name FROM sys.database_files WHERE type = 0";
                //try
                //{
                //    conn.Open();
                //    string DBFile = (string)GetDataFile.ExecuteScalar();                                    // get db file location
                //    conn.Close();
                //    Debug.Print("DB Location: " + DBFile);
                //    this.txtBlock.Text = "DB Location: " + DBFile;
                //}
                //catch (Exception ex)
                //{
                //    conn.Dispose();
                //}
            }

            #region ----NOTES----
            //            using (SqlConnection conn = new SqlConnection(Properties.Resources.ConnectionStringDebug))
            //            {
            //                try
            //                {
            //                    conn.Open();                                                                // open connection to db
            //                    lbl_Status_01.Content = "Established connection to DB";
            //#if DEBUG
            //                    Debug.Print("Established DB Connection");
            //#endif
            //                }
            //                catch (SqlException ex)
            //                {
            //                    lbl_Status_01.Content = "Unable to establish connection to DB" + ex.Message;
            //                    txtBlock.Text = ex.Message;
            //                    txtBlock.TextWrapping = TextWrapping.Wrap;
            //#if DEBUG
            //                    Debug.Print("CANNOT Established DB Connection");
            //#endif
            //                }
            //                if (conn != null && conn.State == System.Data.ConnectionState.Open)
            //                    conn.Close();                                                               // close connection
            //            }
            #endregion
        }


        private void btn_CreateTable_Click(object sender, RoutedEventArgs e)
        {
            //ButtonAutomationPeer peer = new ButtonAutomationPeer(btn_ConnectDB);
            //IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            //invokeProv.Invoke();                    // click the "create db" button
            string ConnectionString = "";
            using (ResXResourceReader reader = new ResXResourceReader(@".\CitiBikeResources.resx"))
            {
                foreach (DictionaryEntry d in reader)
                {
                    if (d.Key.ToString() == "DBConnectionString")
                        ConnectionString = d.Value.ToString(); break;
                }
            }

            // check if table exist
            if (Helper.HelperDB.GetNumberOfTables(ConnectionString) == 0)
            {
                Helper.HelperDB.CreateZIPTable(ConnectionString);                     // create ZIP File table
                Helper.HelperDB.CreateCSVTable(ConnectionString);                     // create CSV File table
            }
        }


        /// <summary>
        /// Download XML file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_DlXML_Click(object sender, RoutedEventArgs e)
        {
            DownloadXMLFile();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_CompareZipFiles_Click(object sender, RoutedEventArgs e)
        {
            if (XMLDocument == null)
                DownloadXMLFile();

            ZIPFileNamesDB = GetZIPFileNamesFromDB();               // extract from db
            ZIPFileNamesOnline = ExtractZipFileList(XMLDocument);   // extract from xml file
            List<string> tempZIPFileNames = CompareZIPFileNames(ZIPFileNamesOnline, ZIPFileNamesDB);  // compare
            if (tempZIPFileNames.Count > 0)
                AddZIPFileNamesToDB(tempZIPFileNames);              // add new
            else
                lbl_Status_01.Content = "No new ZIP File Names to add";
        }

        private void btn_DLZIPFiles_Click(object sender, RoutedEventArgs e)
        {
            Helper.HelperDB.DeleteRows("StationTable");
            Helper.HelperDB.DeleteRows("Trips");
            Helper.HelperDB.DeleteRows("CSVFileName");
            Helper.HelperDB.ShrinkDB();
            Helper.HelperDB.ShrinkLogs();

            List<string> files = new List<string>();
            List<string> CSVData = new List<string>();

            if (XMLDocument == null)
                DownloadXMLFile();

            ExtractZipFileList(XMLDocument);                                                    // get the xml document with zip file names

            Dictionary<string, Type> DBSchema = new Dictionary<string, Type>();
            DBSchema = Helper.HelperDB.GetTableSchema(Properties.Resources.TableTrips);         // get the db table schema

            ZIPFileNamesOnline.RemoveRange(2, ZIPFileNamesOnline.Count - 3);
            foreach (string ZIPName in ZIPFileNamesOnline)
            {
                if (ZIPName == "201307-201402-citibike-tripdata.zip")
                    continue;

                if (!Helper.HelperZIP.DownloadZIPFile(ZIPName))                                 // download zip files
                    continue;

                if (!Helper.HelperZIP.UnZIPFile(ZIPName, out files))                            // unzip files
                    continue;

                for (int i = 0; i < files.Count; i++)
                {
                    int num = Helper.HelperDB.GetLastTableID(Properties.Resources.TableTrips);

                    DataTable dt = Helper.HelperZIP.CreateDataTableFromCSV(Environment.CurrentDirectory + "\\" + files[i], num + 1, DBSchema, ref Stations); //Convert csv to datatable

                    Helper.HelperDB.AddDataTableToDBTable(Properties.Resources.TableTrips, dt); // populate db with datatable

                    Helper.HelperDB.AddCSVFileNameToDB(ZIPName, System.IO.Path.GetFileNameWithoutExtension(files[i]));  //add csv file name to CSV file Table

                    Helper.HelperZIP.DeleteFile(Environment.CurrentDirectory, files[i]);        // delete CSV file
                }
                Helper.HelperZIP.DeleteFile(Environment.CurrentDirectory, ZIPName);             // delete ZIP file
                Helper.HelperDB.ShrinkLogs();
            }

            //add stations to db
            Helper.HelperDB.AddStationsToDB(Stations);
        }

        private void btn_GetSchema_Click(object sender, RoutedEventArgs e)
        {
            Helper.HelperDB.GetTableSchema("Trips");
        }

        void ProgressBarChanged(object sender, EventArgs e)
        {
            try
            {
                DownloadProgressChangedEventArgs ev = (DownloadProgressChangedEventArgs)e;
                progressBar1.Value = ev.ProgressPercentage;
            }
            catch (Exception ex)
            {
                lbl_Status_01.Content = ex.Message;
            }

        }

        #endregion


        #region ----FUNCTIONS----

        /// <summary>
        /// Download XML file from Amazon
        /// Save data into XMLDocument property
        /// </summary>
        private void DownloadXMLFile()
        {
            Debug.Print("DOWNLOADING XML FILE");
            // get executing assembly file path
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;                         // path in URI format
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);                                     // remove prefix
            string localPath = System.IO.Path.GetDirectoryName(path);                           // get actual folder path
            lbl_Status_01.Content = localPath;

            using (WebClient wc = new WebClient())
            {
                lbl_Status_01.Content = "Downloading XML Data";
                wc.DownloadProgressChanged += ProgressBarChanged;
                // wc.DownloadFileAsync(new System.Uri(Properties.Resources.URLXML), localPath + "\\XMLDatat.XML");
                string strXML = wc.DownloadString(Properties.Resources.URLXML);                 // get the xml string

                if (string.IsNullOrEmpty(strXML))                                               // if the xml doc is blank 
                {
                    this.lbl_Status_01.Content = "Cannot access URL for XML data";
                    return;                                                                     // exit function
                }

                XMLDocument = XDocument.Parse(strXML);                                          // parse the string into XML format
                XMLDocument.Save(localPath + "\\XMLDoc.xml");                                   // save the xml doc
                lbl_Status_01.Content = "Downloaded XML Data";
            }
            this.progressBar1.Value = 0;                                                        // reset progress bar (I think)
        }


        /// <summary>
        /// Extract the ZIP file names from the XML document
        /// </summary>
        /// <returns>List of ZIP file names</returns>
        private List<string> ExtractZipFileList(XDocument xmlDocument)
        {
            Debug.Print("EXTRACTING ZIP FILE NAMES FROM XML");
            //var tempFileNames = from keys in XMLDocument.Descendants().ToList()                     // from all the nodes in the xml file
            //                    where keys.Name.LocalName == "Key" && !keys.Value.Contains("JC")    // select the child nodes with zip files, excluding JC
            //                    select keys.Value;

            //var tempFileNames = from keys in xmlDocument.Descendants().ToList()                     // from all the nodes in the xml file
            //                    where keys.Value.Contains(".zip") && !keys.Value.Contains("JC")    // select the child nodes with zip files, excluding JC
            //                    select keys.Value;

            var tempFileNames = from key in xmlDocument.Descendants().ToList()                  // from all the nodes in the xml file
                                where key.Name.LocalName == "Key"                               // select the nodes with the name of KEY
                                where key.Value.Contains(".zip") && !key.Value.Contains("JC")   // select the ones with .zip in the name but ignore the ones with JC
                                select key.Value;

            List<string> zipFileNamesOnline = new List<string>();
            zipFileNamesOnline = tempFileNames.ToList();                                        // save values to list
            ZIPFileNamesOnline = zipFileNamesOnline;

            Debug.Print("Number of ZIP files in XML File: " + zipFileNamesOnline.Count);

            return zipFileNamesOnline;
        }


        /// <summary>
        /// Get list of ZIP file names from DB
        /// If list.count = 0, then there are no file names in the db table
        /// </summary>
        /// <reference>https://stackoverflow.com/questions/15737425/reading-data-from-sql-database-table-to-generic-collection</reference>
        /// <returns>List of ZIP File names in the DB. If list.count = 0, there are no file names in the DB</returns>
        private Dictionary<int, string> GetZIPFileNamesFromDB()
        {
            //string connectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Charlie\Documents\GitHub\Citi_Bike_Data\Citi_Bike_Data_02\Citi_Bike_Data_02\CitiBikeData.mdf; Integrated Security = True";
            //using (SqlConnection conn = new SqlConnection(connectionString))   // create a connection
            using (SqlConnection conn = new SqlConnection(Properties.Resources.ConnectionString))   // create a connection
            {
                Dictionary<int, string> zipFileNamesDB = new Dictionary<int, string>();
                try
                {
                    lbl_Status_01.Content = "Reading DB ZIP File Names";
                    string sql = "select * from " + Properties.Resources.TableZIPFileName;      // create query string
                    if (conn.State != ConnectionState.Open)
                        conn.Open();                                                                // open connection to db

                    using (SqlCommand sqlCommand = new SqlCommand(sql, conn))                          // sql command
                    {
                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            Debug.Print(reader.FieldCount.ToString());
                            lbl_Status_01.Content = "Number of ZIP File Names in DB: " + reader.FieldCount.ToString();

                            if (reader.HasRows)                                                         // if there is data
                            {
                                while (reader.Read())
                                {
                                    Debug.Print(reader["ZIPFileName"].ToString());
                                    zipFileNamesDB.Add(Convert.ToInt32(reader["Id"]), reader["ZIPFileName"].ToString());
                                }
                            }
                            reader.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Print("Cannot read ZIP File names from DB" + Environment.NewLine + ex.Message);
                    lbl_Status_01.Content = "Cannot read ZIP File names from DB" + Environment.NewLine + ex.Message;
                }
                if (zipFileNamesDB.Count > 0)
                    ZIPFileNamesDB = zipFileNamesDB;
                conn.Close();
                return zipFileNamesDB;
            }
        }


        /// <summary>
        /// Compare the ZIPFileNamesOnline with ZIPFileNamesDB and return unique / new names 
        /// Remove any names in the Online list that already exist in the DB
        /// </summary>
        /// <reference>https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlbulkcopy?redirectedfrom=MSDN&view=netframework-4.7.2</reference>
        /// <returns>Return only the list of unique names</returns>
        private List<string> CompareZIPFileNames(List<string> zipFileNamesOnline, Dictionary<int, string> zipFileNamesDB)
        {
            lbl_Status_01.Content = "Comparing ZIP File Names";
            List<string> tempZIPFileNames = new List<string>(zipFileNamesOnline);               // duplicate the list

            foreach (string name in zipFileNamesDB.Values)                                       // cycle throught he dictionary
            {
                if (tempZIPFileNames.Contains(name))                                            // if name exists
                    tempZIPFileNames.Remove(name);                                              // remove it
            }
            return tempZIPFileNames;                                                            // return unique names
        }


        /// <summary>
        /// Write new ZIP File Names to DB
        /// Bulk copy the new file names to the DB
        /// </summary>
        /// <param name="zipFileNames"></param>
        private void AddZIPFileNamesToDB(List<string> zipFileNames)
        {
            string connectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\Charlie\Documents\GitHub\Citi_Bike_Data\Citi_Bike_Data_02\Citi_Bike_Data_02\CitiBikeData.mdf; Integrated Security = True";
            // create a connection string
            //using (SqlConnection conn = new SqlConnection(Properties.Resources.ConnectionStringDebug))   // create a connection
            using (SqlConnection conn = new SqlConnection(connectionString))   // create a connection
            {
                conn.Open();
                lbl_Status_01.Content = "Writing ZIP File Names to DB: " + zipFileNames.Count;

                int numRows = 0;
                string sql = "select count (*) from " + Properties.Resources.TableZIPFileName;      // create query string
                using (SqlCommand sqlCommand = new SqlCommand(sql, conn))                           // sql command
                {
                    numRows = (int)sqlCommand.ExecuteScalar();                                      // get number of exesting rows in db
                    lbl_Status_01.Content = "Number of ZIP File Names in DB: " + numRows;
                    Debug.Print("Number of existing rows: " + numRows);
                }

                SqlCommand AddZIPFileNamesCommand = new SqlCommand("Insert into " + Properties.Resources.TableZIPFileName +
                                    " (Id, ZIPFileName) Values (@Id, @ZIPFileName)", conn);

                DataTable dt = new DataTable(Properties.Resources.TableZIPFileName);                // create a temporary datatable
                dt.Columns.Add("Id");                                                               // assign the same column names
                dt.Columns.Add("ZIPFileName");

                for (int i = 0; i < zipFileNames.Count; i++)                                        // add ZIP file names to data table
                    dt.Rows.Add(new object[] { i + numRows, zipFileNames[i] });

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                {
                    bulkCopy.DestinationTableName = Properties.Resources.TableZIPFileName;
                    lbl_Status_01.Content = "Added: " + dt.Rows.Count + " new ZIP file names to DB";
                    try
                    {
                        bulkCopy.WriteToServer(dt);                                                 // bulk copy data table to DB
                    }
                    catch (Exception ex)
                    {
                        lbl_Status_01.Content = "Unable to insert new ZIPFileNames into DB" + Environment.NewLine + ex.Message;
                        Debug.Print(ex.Message);
                    }
                }

                conn.Close();
                Debug.Print("Writinging DB ZIP File Names");
            }
        }


        #endregion
    }           // close class
}               // close namespace
