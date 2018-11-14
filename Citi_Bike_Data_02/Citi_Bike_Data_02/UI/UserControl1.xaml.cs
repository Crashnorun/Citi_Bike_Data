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

namespace Citi_Bike_Data_02.UI
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        #region ----PROPERTIES----

        public XDocument XMLDocument;
        public List<string> ZIPFileNamesOnline
        {
            get { return ExtractZipFileList(); }
        }

        #endregion


        #region ----FIELDS----




        #endregion


        #region ----UI----

        public UserControl1()
        {
            InitializeComponent();

            lbl_Status_01.Content = string.Empty;
            Color col = (Color)ColorConverter.ConvertFromString(Properties.Resources.CitiBikeColorHEX);         // get color from properties
            SolidColorBrush fill = new SolidColorBrush(col);                                                    // create a solid brush
            this.progressBar1.Foreground = fill;                                                                // apply brush to progress bar
        }

        /// <summary>
        /// Test connection to DB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_CreateDB_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(Properties.Resources.ConnectionString))
            {
                try
                {
                    conn.Open();                                                                // open connection to db
                    lbl_Status_01.Content = "Established connection to DB";
#if DEBUG
                    Debug.Print("Established DB Connection");
#endif
                }
                catch (SqlException ex)
                {
                    lbl_Status_01.Content = "Unable to establish connection to DB" + ex.Message;
#if DEBUG
                    Debug.Print("CANNOT Established DB Connection");
#endif
                }
                if (conn != null && conn.State == System.Data.ConnectionState.Open)         
                    conn.Close();                                                               // close connection
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

            GetZIPFileNamesFromDB();
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
        /// </summary>
        private void DownloadXMLFile()
        {
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
                    return;
                }

                XMLDocument = XDocument.Parse(strXML);                                          // parse the string into XML format
                XMLDocument.Save(localPath + "\\XMLDoc.xml");                                   // save the xml doc
                lbl_Status_01.Content = "Downloaded XML Data";
            }

            if (XMLDocument != null)
                ExtractZipFileList();

            this.progressBar1.Value = 0;                                                        // reset progress bar (I think)
        }


        /// <summary>
        /// Extract the ZIP file names from the XML document
        /// </summary>
        /// <returns>List of ZIP file names</returns>
        private List<string> ExtractZipFileList()
        {
            //var tempFileNames = from keys in XMLDocument.Descendants().ToList()                     // from all the nodes in the xml file
            //                    where keys.Name.LocalName == "Key" && !keys.Value.Contains("JC")    // select the child nodes with zip files, excluding JC
            //                    select keys.Value;

            var tempFileNames = from keys in XMLDocument.Descendants().ToList()                     // from all the nodes in the xml file
                                where keys.Value.Contains(".zip") && !keys.Value.Contains("JC")    // select the child nodes with zip files, excluding JC
                                select keys.Value;

            List<string> zipFileNamesOnline = new List<string>();
            zipFileNamesOnline = tempFileNames.ToList();

#if DEBUG
            Debug.Print("Number of ZIP files in XML File: " + zipFileNamesOnline.Count);
#endif
            return zipFileNamesOnline;
        }


        // get file names from DB
        private List<string> GetZIPFileNamesFromDB()
        {
            using (SqlConnection conn = new SqlConnection(Properties.Resources.ConnectionString))
            {
                List<string> tempZipFileNamesOnline = ZIPFileNamesOnline;

                try
                {
                    lbl_Status_01.Content = "Reading DB ZIP File Names";
                    string sql = "select * from " + Properties.Resources.TableZIPFileName;      // create query string
                    conn.Open();                                                                // open connection to db
                    SqlCommand sqlCommand = new SqlCommand(sql, conn);                          // sql command
                    SqlDataReader reader = sqlCommand.ExecuteReader();

                    Debug.Print(reader.FieldCount.ToString());

                    if (reader.HasRows)                                                         // if there is data
                    {
                        while (reader.Read())
                        {
                            Debug.Print(reader["ZIPFileName"].ToString());
                            // compare lists
                        }
                    }
                    reader.Close();

                    SqlCommand AddZIPFileNamesCommand = new SqlCommand("Insert into " + Properties.Resources.TableZIPFileName +
                        " (Id, ZIPFileName) Values (@Id, @ZIPFileName)", conn);

                    Debug.Print(AddZIPFileNamesCommand.Parameters.Count.ToString());

                    //for (int i = 0; i < tempZipFileNamesOnline.Count; i++)                      // add ZIP file names to DB
                    //{
                    //    AddZIPFileNamesCommand.Parameters[0].Value = i;
                    //    AddZIPFileNamesCommand.Parameters[1].Value = tempZipFileNamesOnline[i];

                    //    AddZIPFileNamesCommand.ExecuteNonQuery();
                    //}
                    
                    AddZIPFileNamesCommand.Parameters.Clear();
                    conn.Close();
#if DEBUG
                    Debug.Print("Writinging DB ZIP File Names");
#endif
                }
                catch (SqlException ex)
                {
                    if (conn != null && conn.State ==System.Data.ConnectionState.Open)
                        conn.Close();

                    lbl_Status_01.Content = "CANNOT Read / Write DB ZIP File Names" + ex.Message;
#if DEBUG
                    Debug.Print("CANNOT Read / Write DB ZIP File Names " + Environment.NewLine +
                        ex.Message);
#endif
                }
            }

            return null;
        }

        #endregion


    }
}
