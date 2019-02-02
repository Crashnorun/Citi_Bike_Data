using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Data;
using System.Collections;
using System.Resources;
using System.Data.SqlClient;

namespace Citi_Bike_Data_02.Helper
{
    /*
     * Create new DB
     * Create new Table
     * Get Executing Assembly Path
     * Check If DB exists
     * Find DB Location
     * Check if table exists
     * Convert to SQL Types
     * Create ZIP Table
     * Create CSV Table
     * Get Number of Tables
     * Add value to resources file
     * Get DB Table Schema to a dictionary 
     * Get DB Table to a DataTable
     * Add a DataTable To DB Table 
     */

    /*
     * Maximum DB Sizes: https://stackoverflow.com/questions/759244/sql-server-the-maximum-number-of-rows-in-table
     */

    
    static class HelperDB
    {
        static string DBFilePath;
        static string DBConnectionString;
        //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Charlie\Documents\GitHub\Citi_Bike_Data\Citi_Bike_Data_02\Citi_Bike_Data_02\CitiBikeData.mdf;Integrated Security=True";

        /// <summary>
        /// Create a new DB in the location of the executing assembly
        /// </summary>
        /// <param name="DBName">Required DB File name</param>
        /// <reference>https://support.microsoft.com/en-us/help/307283/how-to-create-a-sql-server-database-programmatically-by-using-ado-net</reference>
        /// <returns>DB connection string on success; Null on failure</returns>
        public static string CreateNewDB(string DBName, ref string message)
        {
            #region ---NOTES----

            // Executing assembly path => C:\Users\Charlie\Documents\GitHub\Citi_Bike_Data\Citi_Bike_Data_02\Citi_Bike_Data_02\bin\Debug
            // DB Connection String when db is manually created => Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Charlie\Documents\GitHub\Citi_Bike_Data\Citi_Bike_Data_02\Citi_Bike_Data_02\CitiBikeData.mdf;Integrated Security=True;Connect Timeout=30
            // DB Connection string when db is programatacally created =>"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\cportelli\Documents\Personal\GitHub\Citi_Bike_Data\Citi_Bike_Data_02\Citi_Bike_Data_02\bin\\Debug\CitiBikeData.mdf;Integrated security=True;database=master;"
            //string connectionStringBase = "Data Source=(LocalDB)\\MSSQLLocalDB; database=master; Integrated security=True;";

            #endregion

            string connectionString = string.Empty;
            string dbFilePath = Environment.CurrentDirectory + "\\" + DBName;                               // construct new DB file path
                                                                                                            //string creationCmd = "CREATE DATABASE " + DBName + ";";                                       // SQL Create DB Command
            string creationCmd = "CREATE DATABASE " + DBName + " ON PRIMARY " +                             // SQL Create DB Command
                "(NAME = " + DBName + ", " +
                "FILENAME = '" + dbFilePath + ".mdf')";

            using (SqlConnection conn = new SqlConnection(Properties.Resources.ConnectionStringBase))       // create connection
            {
                if (conn.State == System.Data.ConnectionState.Open)                                         // check if it's open
                    conn.Close();

                SqlCommand creationCMD = new SqlCommand(creationCmd, conn);                                 // SQL command

                try
                {
                    conn.Open();                                                                            // open connection
                    int num = creationCMD.ExecuteNonQuery();                                                // execute sql command
                    Debug.Print("DB creation return value: " + num.ToString());
                    message = "DB creation return value: " + num.ToString();
                    connectionString = Properties.Resources.ConnectionStringBase + "AttachDbFilename=" + dbFilePath + ".mdf;";
                    //AddValueToResources("DBConnectionString",
                    //    @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = " + DBFilePath + "; Integrated security = True; database = master;");
                    AddValueToResources("DBConnectionString",
                  @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = " + DBFilePath + "; Integrated security = True;");
                    DBConnectionString = connectionString;                                                  // save DB connection string
                    DBFilePath = dbFilePath;                                                                // save DB file path
                }
                catch (System.Exception ex)
                {
                    Debug.Print("Cannot create DB " + Environment.NewLine + ex.Message);
                    message = "Cannot create DB " + Environment.NewLine + ex.Message;
                }

                conn.Close();
            }
            return connectionString;
        }


        public static void CreateNewTable(string TableName, string ConnectionString, Dictionary<string, Type> ColumnNames, ref string message)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    if (conn.State != System.Data.ConnectionState.Open)                         // check if it's open
                        conn.Open();

                    string columnString = string.Empty;
                    Dictionary<string, string> SQLColumns = ConvertToSQLTypes(ColumnNames);

                    foreach (string name in SQLColumns.Keys)
                        columnString += name + " " + SQLColumns[name].ToLower() + ",";        // compile column names and data types
                    columnString = columnString.TrimEnd(',');

                    string com = "CREATE TABLE[dbo]." + TableName + "( " +
                                " [Id] INT NOT NULL PRIMARY KEY, [ZIPFileName] TEXT NULL, " +
                                "[CSVFileNames] TEXT NULL, [FullFolderPath] TEXT NULL )";

                    //using (SqlCommand command = new SqlCommand("IF NOT EXISTS (" + TableName + ")" +
                    //    "BEGIN CREATE TABLE " + TableName + "(" + columnString + "); END", conn))
                    using (SqlCommand command = new SqlCommand(com, conn))
                    {
                        command.ExecuteNonQuery();
                        Debug.Print("Added " + TableName + " table to DB" + Environment.NewLine);
                        message = "Added" + TableName + " table to DB";
                    }
                }
                catch (Exception ex)
                {
                    Debug.Print("Could not add " + TableName + " table to DB" + Environment.NewLine + ex.Message);
                    message = "Could not add " + TableName + " table to DB" + Environment.NewLine + ex.Message;
                }
            }
        }


        /// <summary>
        /// Find the executing assembly folder path 
        /// </summary>
        /// <returns>Executing assembly folder path</returns>
        public static string GetExecutingAssemblyPath()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;                         // path in URI format
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);                                     // remove prefix
            string localPath = System.IO.Path.GetDirectoryName(path);                           // get actual folder path
            return localPath;
            //C:\Users\Charlie\Documents\GitHub\Citi_Bike_Data\Citi_Bike_Data_02\Citi_Bike_Data_02\bin\Debug
        }


        /// <summary>
        /// Check if the DB exists at executing assembly
        /// Return false (as string) if DB does not exist
        /// Return the DB file path if found
        /// </summary>
        /// <reference>https://stackoverflow.com/questions/816566/how-do-you-get-the-current-project-directory-from-c-sharp-code-when-creating-a-c</reference>
        /// <returns>False = DB does not exist, or the file path were the db is located</returns>
        public static string CheckIfDBExists(string DBName)
        {
            //string path = GetExecutingAssemblyPath();                                           // get the executing assembly path
            //string dbPath = path + "\\" + DBName + ".mdf";                                      // create db file path string
            string dbPath = Environment.CurrentDirectory + "\\" + DBName + ".mdf";                   // create db file path string

            if (File.Exists(dbPath))                                                            // check if DB file exists
            {
                DBFilePath = dbPath;                                                            // if db exists, save it's full path
                                                                                                //AddValueToResources("DBConnectionString", @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = " + DBFilePath + "; Integrated security = True; database = master;");
                AddValueToResources("DBConnectionString",
                  @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = " + DBFilePath + "; Integrated security = True;");
                return dbPath;                                                                  // return full path
            }
            else
            {
                try
                {
                    using (var connection = new SqlConnection(Properties.Resources.ConnectionStringBase))
                    {
                        using (var command = new SqlCommand($"SELECT db_id('{Properties.Resources.DBName}')", connection))
                        {
                            connection.Open();
                            bool obj = (command.ExecuteScalar() != DBNull.Value);
                            if (obj)                                                            // DB exists
                            {

                                DBFilePath = FindDBLocation();
                                AddValueToResources("DBConnectionString",
                                        @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = " + DBFilePath + "; Integrated security = True;");
                                return dbPath;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    DBFilePath = string.Empty;
                    return false.ToString().ToLower();
                }
                DBFilePath = string.Empty;
                return false.ToString().ToLower();
            }
        }

        /// <summary>
        /// Find the file location of the DB
        /// </summary>
        /// <returns>Return the file path of the DB</returns>
        public static string FindDBLocation()
        {
            //https://stackoverflow.com/questions/8146207/how-to-get-current-connected-database-file-path
            using (SqlConnection conn = new SqlConnection(Properties.Resources.ConnectionString))          // create a base connection
            {
                using (SqlCommand GetDataFile = new SqlCommand())
                {
                    GetDataFile.Connection = conn;
                    GetDataFile.CommandText = "SELECT physical_name FROM sys.database_files WHERE type = 0";
                    try
                    {
                        if(conn.State != ConnectionState.Open)
                            conn.Open();
                        string DBFile = (string)GetDataFile.ExecuteScalar();                                    // get db file location
                        conn.Close();
                        Debug.Print("DB Location: " + DBFile);
                        return DBFile;
                    }
                    catch (Exception ex)
                    {
                        Debug.Print(ex.Message);
                        conn.Dispose();
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// Check if a table exists in a DB
        /// </summary>
        /// <param name="DBName">DB Name</param>
        /// <param name="TableName">Table Name</param>
        /// <returns>FALSE if table does not exist, TRUE if tabel does exist</returns>
        /// <reference>https://www.codeproject.com/Questions/1191245/Check-SQL-table-exist-or-not-in-Csharp</reference>
        public static bool CheckIfTableExists(string ConnectionString, string DBName, string TableName)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                if (conn.State != System.Data.ConnectionState.Open)                             // check if it's open
                    conn.Open();
                try
                {
                    string command = "SELECT CASE WHEN OBJECT_ID('" + DBName + "." + TableName + "' , 'U') IS NOT NULL THEN 1 ELSE 0 END";     // create query string to check if table exists
                    using (SqlCommand TableCheck = new SqlCommand(command, conn))                      // create query command to check if table exists
                    {
                        int x = Convert.ToInt32(TableCheck.ExecuteScalar());                        // execute query
                        conn.Close();                                                               // close connection

                        if (x == 1)
                        {
                            Debug.Print("Table exists: " + TableName);
                            return true;
                        }
                        else
                        {
                            Debug.Print("Table doesn't exist: " + TableName);
                            return false;
                        }
                    }
   
                }
                catch (Exception ex)
                {
                    conn.Close();                                                               // close connection
                    Debug.Print("Couldn't not identify if " + TableName + " exists in DB" +
                        Environment.NewLine + ex.Message);
                }
                return false;
            }
        }

        /// <summary>
        /// Convert dictionary of C# types to dictionary of SQL types
        /// </summary>
        /// <param name="ColumnNames">Dictionary containing column names (as string) and C# variable types (as Type)</param>
        /// <returns>Dictionary containing column names (as string) and SQL variable types (as string)</returns>
        public static Dictionary<string, string> ConvertToSQLTypes(Dictionary<string, Type> ColumnNames)
        {
            Dictionary<string, string> NewColumNames = new Dictionary<string, string>();        // create new dictonary

            foreach (KeyValuePair<string, Type> pair in ColumnNames)
            {
                if (pair.Value == typeof(string))
                {
                    NewColumNames.Add(pair.Key, "text");
                }
                else if (pair.Value == typeof(int))
                {
                    NewColumNames.Add(pair.Key, "int");
                }
                else if (pair.Value == typeof(double))
                {
                    NewColumNames.Add(pair.Key, "double");
                }
                else if (pair.Value == typeof(DateTime))
                {
                    NewColumNames.Add(pair.Key, "datetime");
                }
                else if (pair.Value == typeof(bool))
                {
                    NewColumNames.Add(pair.Key, "text");
                }
                else
                {
                    NewColumNames.Add(pair.Key, "text");
                }
            }
            return NewColumNames;
        }

        public static void CreateZIPTable(string ConnectionString)
        {
            // check if table exists
            bool test = CheckIfTableExists(ConnectionString, Properties.Resources.DBName,
               Properties.Resources.TableZIPFileName);

            if (!test)                                                                          // if table doesn't exist
            {
                string message = string.Empty;

                Dictionary<string, Type> ColumnNames = new Dictionary<string, Type>();
                Classes.cls_ZIPFile zipFile = new Classes.cls_ZIPFile();                        // create temporary object
                ColumnNames = zipFile.GetType().GetProperties().ToDictionary(prop => prop.Name, prop => prop.PropertyType);                     // convert object properties to dictionary

                CreateNewTable(Properties.Resources.TableZIPFileName, ConnectionString, ColumnNames, ref message);     // create table
            }
        }

        public static void CreateCSVTable(string ConnectionString)
        {
            // check if table exists
            bool test = CheckIfTableExists(ConnectionString, Properties.Resources.DBName,
               Properties.Resources.TableCSVFileName);

            if (!test)                                                                          // if table doesn't exist
            {
                string message = string.Empty;

                Dictionary<string, Type> ColumnNames = new Dictionary<string, Type>();
                Classes.cls_CSVFile csvFile = new Classes.cls_CSVFile();
                ColumnNames = csvFile.GetType().GetProperties().ToDictionary(prop => prop.Name, prop => prop.PropertyType);                     // convert object properties to dictionary

                CreateNewTable(Properties.Resources.TableCSVFileName, ConnectionString, ColumnNames, ref message);     // create table
            }
        }

        public static int GetNumberOfTables(string ConnectionString)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    if (conn.State != ConnectionState.Open)                         // check if it's open
                        conn.Open();
                    DataTable schema = conn.GetSchema("Tables");
                    return schema.Rows.Count;
                }
                catch (Exception ex)
                {
                    Debug.Print("Could not get the number of tables in the DB" + Environment.NewLine + ex.Message);
                }
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return 0;
        }


        /// <summary>
        /// Add a value to the resoureces file
        /// </summary>
        /// <param name="Key">Key | Value Name</param>
        /// <param name="Value">Value</param>
        public static void AddValueToResources(string Key, string Value)
        {
            using (ResXResourceWriter resxresourcewriter = new ResXResourceWriter(@".\CitiBikeResources.resx"))
            {
                resxresourcewriter.AddResource(Key, Value);
            }
        }

        /// <summary>
        /// Get a DB Table Schema
        /// </summary>
        /// <reference>https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqldatareader?view=netframework-4.7.2</reference>
        /// <param name="TableName"></param>
        public static Dictionary<string, Type> GetTableSchema(string TableName)
        {
            Debug.Print("GETTING DB TABLE: " + TableName + " SCHEMA");
            Dictionary<string, Type> TableSchema = new Dictionary<string, Type>();
            using (SqlConnection conn = new SqlConnection(Properties.Resources.ConnectionString))
            {
                string commandText = "SELECT * FROM " + TableName;
                using (SqlCommand command = new SqlCommand(commandText, conn))
                {
                    try
                    {
                        if (conn.State != ConnectionState.Open)
                            conn.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            for (int i = 0; i < reader.VisibleFieldCount; i++)
                                TableSchema.Add(reader.GetName(i), reader.GetFieldType(i));
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Print(ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                        return null;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                }
            }
            return TableSchema;
        }

        /// <summary>
        /// Copy a SQL table into a DataTable
        /// </summary>
        /// <reference>https://stackoverflow.com/questions/6073382/read-sql-table-into-c-sharp-datatable</reference>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static DataTable GetDBTable(string TableName)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(Properties.Resources.ConnectionString))
            {
                string commandText = "SELECT * FROM " + TableName;
                using (SqlCommand command = new SqlCommand(commandText, conn))
                {
                    try
                    {
                        if (conn.State != ConnectionState.Open)
                            conn.Open();

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Print(ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                        return null;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// Add a Datatable to the end of a table in the DB
        /// </summary>
        /// <param name="TableName">Table name</param>
        /// <param name="dataTable">Data table</param>
        public static void AddDataTableToDBTable(string TableName, DataTable dataTable)
        {
            Debug.Print("ADDING DATATABLE: " + dataTable.TableName + " INTO DB TABLE: " + TableName);
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Charlie\Documents\GitHub\Citi_Bike_Data\Citi_Bike_Data_02\Citi_Bike_Data_02\CitiBikeData.mdf;Integrated Security=True";
            using (SqlConnection conn = new SqlConnection(Properties.Resources.ConnectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                    {
                        //foreach (DataColumn col in dataTable.Columns)
                        // bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);        // copy the column mapping
                        bulkCopy.DestinationTableName = TableName;                              // set destination
                        bulkCopy.WriteToServer(dataTable);                                      // write to DB
                    }
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                }
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        /// <summary>
        /// Get the last Unique Id from a table in the DB
        /// </summary>
        /// <param name="TableName">Table name</param>
        /// <returns>-1 = failure, 0 = no entries, integer = last Unique Id</returns>
        public static int GetLastTableID(string TableName)
        {
            Debug.Print("GETTING LAST UNIQUE ID FROM TABLE: " + TableName);
            int num = -1;                                                       // default -1 value
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Charlie\Documents\GitHub\Citi_Bike_Data\Citi_Bike_Data_02\Citi_Bike_Data_02\CitiBikeData.mdf;Integrated Security=True";
            using (SqlConnection conn = new SqlConnection(Properties.Resources.ConnectionString))
            {
                string commandText = "SELECT MAX (Id) FROM " + TableName;           // select max Id from table
                using (SqlCommand command = new SqlCommand(commandText, conn))
                {
                    try
                    {
                        if (conn.State != ConnectionState.Open)
                            conn.Open();

                        var obj = command.ExecuteScalar();
                        if (obj == System.DBNull.Value)                                 // if the return value is null
                            num = 0;                                                    // return 0
                        else
                            num = Convert.ToInt32(obj);                                 // convert value to int
                    }
                    catch (Exception ex)
                    {
                        Debug.Print(ex.Message + Environment.NewLine + ex.StackTrace.ToString());
                    }
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }
            Debug.Print(num.ToString() + " ENTERIES IN TABLE: " + TableName);

            return num;
        }


        public static void DeleteRows()
        {
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Charlie\Documents\GitHub\Citi_Bike_Data\Citi_Bike_Data_02\Citi_Bike_Data_02\CitiBikeData.mdf;Integrated Security=True";
            using (SqlConnection conn = new SqlConnection(Properties.Resources.ConnectionString))
            {
                conn.Open();

                string sql = @"DELETE FROM Trips;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    int numRows = cmd.ExecuteNonQuery();

                    //sql = "ALTER DATABASE CitiBikeData SET RECOVERY SIMPLE;";
                    //cmd = new SqlCommand(sql, conn);
                    //cmd.ExecuteNonQuery();

                    //sql = "DBCC SHRINKDATABASE (CitiBikeData, 10);";
                    //cmd = new SqlCommand(sql, conn);
                    //cmd.ExecuteNonQuery();

                    //sql = "DBCC SHRINKDATAFILE (CitiBikeData.ldf, 10);";
                    //cmd = new SqlCommand(sql, conn);
                    //cmd.ExecuteNonQuery();

                    conn.Close();
                    Debug.Print("Number of rows deleted: " + numRows);
                }
            }
        }

        // https://social.msdn.microsoft.com/Forums/en-US/313fbd12-ee1f-4e34-b9c1-aeb4816b6060/dbcc-shrinkdatabase-truncateonly?forum=sqldatabaseengine
        public static void ShrinkDB()
        {
            Debug.Print("SHRINKING DB SIZE");
            string[] dirs = Directory.GetFiles(Environment.CurrentDirectory, "DB_Tests_01.mdf");

            if (dirs.Length == 0) return;

            long length = new System.IO.FileInfo(dirs[0]).Length;
            Debug.Print("Current DB Size: " + (double)(length / 1024f));

            string connectionString = Properties.Resources.ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.Open();
                //string sql = @"DBCC SHRINKDATABASE ('DB_Tests_01.mdf', 10, TRUNCATEONLY)";
                string sql = @"DBCC SHRINKFILE('" + Properties.Resources.DBName + "', 10)";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandTimeout = 0;
                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
            }

            length = new System.IO.FileInfo(dirs[0]).Length;
            Debug.Print("Current DB Size: " + (double)(length / 1024f));
        }

        public static void ShrinkLogs()
        {
            Debug.Print("SHRINKING LOG SIZE");
            string[] dirs = Directory.GetFiles(Environment.CurrentDirectory, Properties.Resources.DBName + "_log.ldf");
            if (dirs.Length == 0) return;
            long length = new System.IO.FileInfo(dirs[0]).Length;
            Debug.Print("Current Log Size: " + (double)(length / 1024f));

            using (SqlConnection conn = new SqlConnection(Properties.Resources.ConnectionString))
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                conn.Open();
                string sql = @"DBCC SHRINKFILE('" + Properties.Resources.DBName + "_log', 10)";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandTimeout = 0;
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }

            length = new System.IO.FileInfo(dirs[0]).Length;
            Debug.Print("Current Log Size: " + (double)(length / 1024f));
        }


    }           // close class
}               // close namespace
