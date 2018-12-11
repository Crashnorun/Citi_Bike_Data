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

namespace Citi_Bike_Data_02.HelperDB
{
    static class HelperDB
    {
        static string DBFilePath;
        static string DBConnectionString;

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
                "FILENAME = " + dbFilePath + ".mdf)";
            //string creationCmd = "CREATE DATABASE " + DBName + " ON " +                             // SQL Create DB Command
            //   "(NAME = " + DBName + ", " +
            //   "FILENAME = '" + dbFilePath + ".mdf')";

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

                    using (SqlCommand command = new SqlCommand("If not exists (" + TableName + ")" +
                        "begin create table " + TableName + "(" + columnString + "); end", conn))
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
        /// </summary>
        /// <reference>https://stackoverflow.com/questions/816566/how-do-you-get-the-current-project-directory-from-c-sharp-code-when-creating-a-c</reference>
        /// <returns>False = DB does not exist, or the file path were the db is located</returns>
        public static string CheckIfDBExists(string DBName)
        {
            string path = GetExecutingAssemblyPath();                                           // get the executing assembly path
            string dbPath = path + "\\" + DBName + ".mdf";                                      // create db RELEASE file path string

            if (File.Exists(dbPath))                                                            // check if DB exists in release mode
            {
                DBFilePath = dbPath;                                                            // if db exists, save it's full path
                                                                                                //AddValueToResources("DBConnectionString",
                                                                                                //    @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = " + DBFilePath + "; Integrated security = True; database = master;");
                AddValueToResources("DBConnectionString",
                  @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = " + DBFilePath + "; Integrated security = True;");
                return dbPath;                                                                  // return full path
            }
            else
            {
                dbPath = Environment.CurrentDirectory + "\\" + DBName + ".mdf";                 // create db DEBUG file path string
                if (File.Exists(dbPath))                                                        // in debug mode
                {
                    DBFilePath = dbPath;
                    return dbPath;
                }
                else
                {
                    DBFilePath = string.Empty;
                    return false.ToString().ToLower();
                }
            }
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
                    SqlCommand TableCheck = new SqlCommand(command, conn);                      // create query command to check if table exists

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


        public static void CreateCSVTable()
        {
            // check if table exists
            bool test = CheckIfTableExists(Properties.Resources.ConnectionStringBase, Properties.Resources.DBName,
               Properties.Resources.TableCSVFileName);

            if (!test)                                                                          // if table doesn't exist
            {
                string message = string.Empty;

                Dictionary<string, Type> ColumnNames = new Dictionary<string, Type>();
                Classes.cls_CSVFile csvFile = new Classes.cls_CSVFile();
                ColumnNames = csvFile.GetType().GetProperties().ToDictionary(prop => prop.Name, prop => prop.PropertyType);                     // convert object properties to dictionary

                CreateNewTable(Properties.Resources.TableCSVFileName, Properties.Resources.ConnectionStringBase, ColumnNames, ref message);     // create table
            }
        }

        public static int GetNumberOfTables(string ConnectionString)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                try
                {
                    if (conn.State != System.Data.ConnectionState.Open)                         // check if it's open
                        conn.Open();
                    DataTable schema = conn.GetSchema("Tables");
                    conn.Close();
                    return schema.Rows.Count;
                }
                catch (Exception ex)
                {
                    conn.Close();
                    Debug.Print("Could not get the number of tables in the DB" + Environment.NewLine + ex.Message);
                }
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


    }           // close class
}               // close namespace
