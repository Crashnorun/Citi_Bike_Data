﻿using System;
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

namespace Citi_Bike_Data_02.UI
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        #region ----PROPERTIES

        public XDocument XMLDocument;

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
                }
                catch (SqlException ex)
                {
                    lbl_Status_01.Content = "Unable to establish connection to DB" + ex.Message;
                }
            }
        }


        /// <summary>
        /// Download XML file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_DlXML_Click(object sender, RoutedEventArgs e)
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
                XMLDocument = XDocument.Parse(strXML);                                          // parse the string into XML format
                XMLDocument.Save(localPath + "\\XMLDoc.xml");                                   // save the xml doc
                lbl_Status_01.Content = "Downloaded XML Data";
            }

            if (XMLDocument != null)
            {
                ExtractXipFileList();
            }
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

        // extract zip files from xml document

        private void ExtractXipFileList()
        {
          

            var obj = XMLDocument.Descendants().Where(n => n.Name == "Key");
            //XmlNodeList elemList = XMLDocument.Nodes();
            //IEnumerable<XNode> nodes = XMLDocument.Nodes();

            //IEnumerable < XElement > xmlEles = XMLDocument.Elements();
            //IEnumerable<XNode> nodes = xmlEles.Nodes();

            //XmlNodeList nodes = XMLDocument.GetElementsByTagName("Item");

            //foreach (XmlNode child in node.SelectNodes("property"))
            //{
            //    if (child.Name == "property")
            //    {
            //        doSomethingElse()
            //        }
            //}


            //for (int i = 0; i < nodes.Count(); i++)
            //{
            //    var obj = nodes.ElementAt(i).ToString();
            //    System.Diagnostics.Debug.Print(obj.ToString());

            //    //string attrVal = elemList[i].Attributes["SuperString"].Value;

            //}




        }

        #endregion
    }
}
