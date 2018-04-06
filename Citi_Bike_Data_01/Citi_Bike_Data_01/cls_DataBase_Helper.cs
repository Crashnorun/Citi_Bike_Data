using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

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

            using (SqlConnection connection = new SqlConnection(connectionString))              // create connection to DB
            {
                string SqlQuery = "SELECT FileName FROM FileNames";                             // DB query
                SqlCommand cmd = new SqlCommand(SqlQuery, connection);                          // create command
                connection.Open();                                                              // open connection to DB
                SqlDataReader reader = cmd.ExecuteReader();                                     // execute read command
                while (reader.Read())
                {
                    tableNames.Add(reader["FileName"].ToString());                              // read all the rows
                }
                connection.Close();                                                             // close connection
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

            using (SqlConnection connection = new SqlConnection(connectionString))              // create connection to DB
            {
                connection.Open();                                                              // open connection to DB
                DataTable table = connection.GetSchema("Tables");                               // get the table
                connection.Close();                                                             // close connection

                foreach(DataRow r in table.Rows)
                {
                    tableNames.Add(r[2].ToString());                                            // save each table name
                }
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

    }
}
