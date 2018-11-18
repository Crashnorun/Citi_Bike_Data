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


        public static string CreateNewDB(string DBName)
        {
            // Executing assembly path => C:\Users\Charlie\Documents\GitHub\Citi_Bike_Data\Citi_Bike_Data_02\Citi_Bike_Data_02\bin\Debug
            // DB Connection String => Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Charlie\Documents\GitHub\Citi_Bike_Data\Citi_Bike_Data_02\Citi_Bike_Data_02\CitiBikeData.mdf;Integrated Security=True;Connect Timeout=30
            //string connectionStringBase = "Data Source=(LocalDB)\\MSSQLLocalDB; database=master; Integrated security=True;";
            //string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB; database=master; Integrated security=True;";

            string connectionString = string.Empty;
            SqlConnection conn = new SqlConnection(Properties.Resources.ConnectionStringBase);      // create connection
            if (conn.State == System.Data.ConnectionState.Open)                                     // check if it's open
                conn.Close();

            string path = GetExecutingAssemblyPath() + "\\" + DBName;                               // construct new DB file path
            //string creationString = "CREATE DATABASE IF NOT EXISTS " + DBName + ";";
            //string creationString = "CREATE DATABASE IF NOT EXISTS " + DBName + "; ON PRIMARY ";// +
            //string creationString = "CREATE DATABASE " + DBName + "; ON PRIMARY ";// +
            string creationString = "CREATE DATABASE " + DBName + ";";// +                          // SQL Create DB Command
                                                                      // "FILENAME = '" + path + ".mdf'";
                                                                      //"(NAME = " + DBName + ", " +
                                                                      //"FILENAME = '" + path + ".mdf')"; 
                                                                      //"SIZE = 2MB, MAXSIZE = 10MB, FILEGROWTH = 10%) " +
                                                                      //"LOG ON (NAME = MyDatabase_Log, " +
                                                                      //"FILENAME = 'C:\\MyDatabaseLog.ldf', " +
                                                                      //"SIZE = 1MB, " +
                                                                      //"MAXSIZE = 5MB, " +
                                                                      //"FILEGROWTH = 10%)";


            SqlCommand creationCMD = new SqlCommand(creationString, conn);                          // SQL command

            try
            {
                conn.Open();
                int num = creationCMD.ExecuteNonQuery();
                Debug.Print("DB creation return value: " + num.ToString());
                connectionString = Properties.Resources.ConnectionStringBase + ";AttachDbFilename=" + path + ".mdf;";
                DBConnectionString = connectionString;
                DBFilePath = path;
            }
            catch (System.Exception ex)
            {
                Debug.Print("Cannot create DB: " + Environment.NewLine + ex.Message);
            }

            conn.Close();
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
        /// Check if the DB exists. Need to check in two locations:
        /// 1. Relase configuration
        /// 2. Debug configuration
        /// Return false if DB does not exist
        /// </summary>
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
