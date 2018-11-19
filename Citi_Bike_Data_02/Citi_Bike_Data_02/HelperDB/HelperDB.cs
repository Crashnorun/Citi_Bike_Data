using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.IO;


namespace Citi_Bike_Data_02.HelperDB
{
    static class HelperDB
    {
        static string DBFilePath;
        static string DBConnectionString;

        /// <summary>
        /// Create a new DB
        /// </summary>
        /// <param name="DBName">Required DB File name</param>
        /// <returns>DB connection string on success; Null on failure</returns>
        public static string CreateNewDB(string DBName)
        {
            #region ---NOTES----
            // Executing assembly path => C:\Users\Charlie\Documents\GitHub\Citi_Bike_Data\Citi_Bike_Data_02\Citi_Bike_Data_02\bin\Debug
            // DB Connection String => Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Charlie\Documents\GitHub\Citi_Bike_Data\Citi_Bike_Data_02\Citi_Bike_Data_02\CitiBikeData.mdf;Integrated Security=True;Connect Timeout=30
            //string connectionStringBase = "Data Source=(LocalDB)\\MSSQLLocalDB; database=master; Integrated security=True;";
            #endregion

            #region ----NOTES ----
            //string creationString = "CREATE DATABASE IF NOT EXISTS " + DBName + ";";
            //string creationString = "CREATE DATABASE IF NOT EXISTS " + DBName + "; ON PRIMARY ";// +
            //string creationString = "CREATE DATABASE " + DBName + "; ON PRIMARY ";// +
            // "FILENAME = '" + path + ".mdf'";
            //"(NAME = " + DBName + ", " +
            //"FILENAME = '" + path + ".mdf')"; 
            //"SIZE = 2MB, MAXSIZE = 10MB, FILEGROWTH = 10%) " +
            //"LOG ON (NAME = MyDatabase_Log, " +
            //"FILENAME = 'C:\\MyDatabaseLog.ldf', " +
            //"SIZE = 1MB, " +
            //"MAXSIZE = 5MB, " +
            //"FILEGROWTH = 10%)";
            #endregion

            string connectionString = string.Empty;
            string dbFilePath = string.Empty;
#if DEBUG
            dbFilePath = Environment.CurrentDirectory + "\\" + DBName;                                      // construct new DB file path
#else
            dbFilePath = GetExecutingAssemblyPath() + "\\" + DBName;                                        // construct new DB file path
#endif
            string creationCmd = "CREATE DATABASE " + DBName + ";";                                         // SQL Create DB Command

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
                    connectionString = Properties.Resources.ConnectionStringBase + ";AttachDbFilename=" + dbFilePath + ".mdf;";
                    DBConnectionString = connectionString;
                    DBFilePath = dbFilePath;
                }
                catch (System.Exception ex)
                {
                    Debug.Print("Cannot create DB " + Environment.NewLine + ex.Message);

                }

                conn.Close();
            }
              
            return connectionString;
        }


        /// <summary>
        /// Find the executing assembly folder path 
        /// </summary>
        /// <returns>Executing assembly folder path</returns>
        private static string GetExecutingAssemblyPath()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;                         // path in URI format
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);                                     // remove prefix
            string localPath = System.IO.Path.GetDirectoryName(path);                           // get actual folder path
            return localPath;
            //C:\Users\Charlie\Documents\GitHub\Citi_Bike_Data\Citi_Bike_Data_02\Citi_Bike_Data_02\bin\Debug
        }


        /// <summary>
        /// Check if the DB exists. Function checks in two locations:
        /// 1. Relase configuration (at executing assembly)
        /// 2. Debug configuration (in project diretory)
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
                DBFilePath = dbPath;
                return dbPath;
            }
            else
            {
                dbPath = Environment.CurrentDirectory + "\\" + DBName + ".mdf";                 // create db DEBUG file path string
                if (File.Exists(dbPath))                                                            // in debug mode
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
    }
}
