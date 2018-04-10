using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;
using System.Windows.Forms;                                                                     // used for displaying exceptions
using System.Net;                       // used to download zip files
using System.IO;                        // used to create directory
using System.IO.Compression;            // used for unzipping files

namespace Citi_Bike_Data_01
{
    public class cls_DataBase_Helper
    {
        /// <summary>
        /// Get the list of table names be retrieving the FileNames column in the first table
        /// </summary>
        /// <param name="window"> Main dilaog window </param>
        /// <param name="connectionString"> Connection string to the database </param>
        /// <returns> List of strings containing the table names </returns>
        public static List<string> GetTableNames(MainWindow window, string connectionString)
        {
            List<string> tableNames = new List<string>();                                       // empty list for the names
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))          // create connection to DB
                {
                    string SqlQuery = "SELECT FileName FROM FileNames";                         // DB query
                    SqlCommand cmd = new SqlCommand(SqlQuery, connection);                      // create command
                    connection.Open();                                                          // open connection to DB
                    SqlDataReader reader = cmd.ExecuteReader();                                 // execute read command
                    while (reader.Read())
                    {
                        tableNames.Add(reader["FileName"].ToString());                          // read all the rows
                    }
                    connection.Close();                                                         // close connection
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not get table names from DB" + Environment.NewLine + ex.Message.ToString());
            }
            return tableNames;                                                                  // return list of names
        }


        /// <summary>
        /// Get the list of table names by retrieving the name of each table
        /// </summary>
        /// <param name="connectionString"> Connection string ot the database </param>
        /// <param name="window"> Main dialog window </param>
        /// <returns> List of strings containing the table names </returns>
        public static List<string> GetTableNames(string connectionString, MainWindow window)
        {
            List<string> tableNames = new List<string>();                                       // empty list for the names

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))          // create connection to DB
                {
                    connection.Open();                                                          // open connection to DB
                    DataTable table = connection.GetSchema("Tables");                           // get the table
                    connection.Close();                                                         // close connection

                    foreach (DataRow r in table.Rows)
                    {
                        tableNames.Add(r[2].ToString());                                        // save each table name
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not get table names from DB" + Environment.NewLine + ex.Message.ToString());
            }
            return tableNames;                                                                  // return list of names
        }


        /// <summary>
        /// Adds the names of the new datasets to the first table in the DB
        /// </summary>
        /// <reference>Does table exist:https://stackoverflow.com/questions/2923452/c-sharp-sql-create-table-if-it-doesnt-already-exist </reference>
        /// <reference>Create connection to database: https://stackoverflow.com/questions/12220865/connecting-to-local-sql-server-database-using-c-sharp </reference>
        /// <reference>https://stackoverflow.com/questions/6671067/retrieve-list-of-tables-from-specific-database-on-server-c-sharp</reference>
        public static void AddFileNamesToTable(string connectionString, List<string> FileNames)
        {
            string tableName = "FileNames";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                int count = 2;
                connection.Open();
                foreach (string fileName in FileNames)
                {
                    string commandText = "INSERT " + tableName +
                        " (FileName, TableNumber)" +
                        " VALUES ('" + fileName + "', " + count + ")";

                    SqlCommand cmd = new SqlCommand(commandText, connection);
                    cmd.ExecuteNonQuery();
                    count++;
                }
                connection.Close();
            }
        }


        /// <summary>
        /// Download unique files from Amazon server
        /// </summary>
        /// <reverence>download file - https://msdn.microsoft.com/en-us/library/ez801hhe(v=vs.110).aspx </reverence>
        /// <reference> unzip file - https://stackoverflow.com/questions/22604941/how-can-i-unzip-a-file-to-a-net-memory-stream </reference>
        /// <param name="UniqueFileNames"> List of files names to be downloaded</param>
        /// <param name="XMLWebAddress"> The URL where the XML data can be downloaded</param>
        public static void DownloadFiles(string ConnectionString, List<string> UniqueFileNames, string XMLWebAddress)
        {
            string assemblyFolderPath = cls_Helper.GetExecutingAssemblyPath();                  // get assembly folder path
            string destinationFolder = cls_Helper.GetDataFolder(assemblyFolderPath);            // get folder path where data will be stored

            using (WebClient client = new WebClient())
            {
                foreach (string uniqueName in UniqueFileNames)                                  // go through all the new file names
                {
                    //web address format: https://s3.amazonaws.com/tripdata/201307-201402-citibike-tripdata.zip
                    string webaddress = XMLWebAddress + uniqueName;                             // get the url of the new zip file name
                    string destinationFile = destinationFolder + @"\" + uniqueName;             // create full file name path
                    //string tempFolderPath = Path.GetTempPath();                               // temporary folder path
                    client.DownloadFile(webaddress, destinationFile);                           // download the file
                    using (FileStream fileStream = File.OpenRead(destinationFile))              // stream zip file
                    {
                        using (ZipArchive unzippedFile = new ZipArchive(fileStream, ZipArchiveMode.Read))  // unzip file
                        {
                            AddZipToDB(ConnectionString, unzippedFile, uniqueName);             // add the zip file name, and csv file names to the DB

                            foreach (ZipArchiveEntry entry in unzippedFile.Entries)             // go through each of the files in the zip file
                            {
                                DataTable csvTable = GetDataTableFromCSVFile(entry, destinationFolder + @"\" + entry.FullName); // convert csv into DataTable
                                AddDataTableToDataBase(csvTable, ConnectionString);
                                // add new table to DB
                            }
                        }
                    }
                }
            }
        }
        //-----------------------------------------------------------------------


        /// <summary>
        /// This will take a csv file and covert it to a data table
        /// </summary>
        /// <reference> CSV to datatable = https://stackoverflow.com/questions/20759302/upload-csv-file-to-sql-server </reference>
        /// <param name="csvFilePath"> Full file path where CSV file is located </param>
        /// <returns> Returns a datatable from a CSV file </returns>
        public static DataTable GetDataTableFromCSVFile(ZipArchiveEntry entry, string csvFilePath)
        {
            string fileName = entry.FullName.Split('.')[0];                                     // get the file name without file extension
            DataTable csvData = new DataTable(fileName.ToUpper());                              // create new data table with table name = file name

            try
            {
                using (Stream stream = entry.Open())                                            // open the csv file
                {
                    StreamReader reader = new StreamReader(stream);
                    string[] columnNames = reader.ReadLine().Replace("\"", "").Split(',');       // read the column headers
                    DataColumn[] columns = new DataColumn[columnNames.Length];

                    for (int i = 0; i < columns.Length; i++)
                        columns[i] = new DataColumn(columnNames[i].ToUpper(), typeof(string));  // set column names and data type

                    csvData.Columns.AddRange(columns);                                          // add columns to data table
                    while (!reader.EndOfStream)
                        csvData.Rows.Add(reader.ReadLine().Split(','));                         // add data to data table
                }
            }
            catch (Exception ex)
            {
                Debug.Print("Could not create DataTable from CSV Stream" + Environment.NewLine + ex.Message.ToString() +
                    Environment.NewLine + ex.StackTrace.ToString());
            }
            return csvData;
        }
        //-----------------------------------------------------------------------


        /// <summary>
        /// 
        /// </summary>
        /// <reference> https://www.morgantechspace.com/2014/04/Insert-DataTable-into-SQL-Table-in-C-Sharp.html </reference>
        /// <param name="csvTable"> Datatable to insert into database </param>
        /// <param name="connectionString"> Database connection string </param>
        public static void AddDataTableToDataBase(DataTable csvTable, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))               // create a connection
            {
                string query = "Create Table " + csvTable.TableName + "(";                      // begin format of query string
                for (int i = 0; i < csvTable.Columns.Count; i++)
                    query += (i < csvTable.Columns.Count - 1) ? csvTable.Columns[i].ColumnName + " string, " :
                        csvTable.Columns[i].ColumnName + " string)";                            // add column names to query string

                connection.Open();                                                              // open connection to DB
                SqlCommand command = new SqlCommand(query, connection);                         // create sql command
                command.ExecuteNonQuery();                                                      // execute command string

                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connection))
                {
                    sqlBulkCopy.DestinationTableName = csvTable.TableName;
                    foreach (DataColumn col in csvTable.Columns)                                // bulk copy each column
                        sqlBulkCopy.ColumnMappings.Add(col.ToString(), col.ToString());
                    sqlBulkCopy.WriteToServer(csvTable);
                }
            }
        }
        //-----------------------------------------------------------------------


        /// <summary>
        /// Identifies if a Zip file entry exists in the DB
        /// Identifies if the CSV file entries exist in the DB
        /// </summary>
        /// <param name="connectionString"> Connection string to DB </param>
        /// <param name="unzippedFile"> UnZipped File </param>
        /// <param name="zipFileName"> Zip File Name </param>
        public static void AddZipToDB(string connectionString, ZipArchive unzippedFile, string zipFileName)
        {
            string csvNames = "";
            for (int i = 0; i < unzippedFile.Entries.Count; i++)
            {
                csvNames += unzippedFile.Entries[i].Name.Split('.')[0];                         // format name
                if (i != unzippedFile.Entries.Count - 1)
                    csvNames += " , ";
            }

            using (SqlConnection connection = new SqlConnection(connectionString))              // create connection to DB
            {
                connection.Open();                                                              // open connection to DB
                //string commandString = "SELECT FileName FROM FileNames WHERE EXISTS (" + zipFileName + ")";
                string commandString = "SELECT COUNT (*) FROM FileNames WHERE EXISTS (" + zipFileName + ")";
                SqlCommand command = new SqlCommand(commandString, connection);
                SqlDataReader reader = command.ExecuteReader();                                 // check if zip file exits in db
                if (reader.FieldCount <= 0)                                                     // if entry doesn't exist, add it
                {
                    commandString = "INSERT INTO FileNames (FileName, CSVs) VALUES (" + zipFileName + csvNames + ")";            // add zip file name to db
                    command.CommandText = commandString;
                    command.ExecuteNonQuery();
                }
                else                                                                            // if entry exists
                {
                    commandString = "SELECT CSVs FROM FileNames WHERE EXISTS (" + zipFileName + ")";    // get csv names
                    command.CommandText = commandString;
                    reader = command.ExecuteReader();
                    if (reader.FieldCount <= 0)                                                   // if the csv file names don't exist
                    {
                        commandString = "UPDATE FileNames SET CSVs = " + csvNames + "WHERE FileNames = " + zipFileName;
                        command.CommandText = commandString;
                        command.ExecuteNonQuery();
                    }
                    else                                                                        // if the csv file names do exist
                    {
                        string value = (string)command.ExecuteScalar();                         // get value from DB
                        if (string.Compare(value, csvNames) != 0)
                            commandString = "UPDATE FileNames SET CSVs = " + csvNames + "WHERE FileNames = " + zipFileName;
                    }
                }
                connection.Close();                                                         // close connection
            }
        }
        //-----------------------------------------------------------------------



        // add table
        static void AddTable(string connectionString, string tableName = "", List<string> columNames = null)
        {

        }


    }       // close class
}           // close namespace
