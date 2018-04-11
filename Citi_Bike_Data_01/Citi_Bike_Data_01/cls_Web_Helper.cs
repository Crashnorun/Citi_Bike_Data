using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;

namespace Citi_Bike_Data_01
{
    public class cls_Web_Helper
    {
        /// <summary>
        /// Retrieves the list of available citi bike datasets on the amazon s3 bucket
        /// <reference>https://stackoverflow.com/questions/124492/c-sharp-httpwebrequest-command-to-get-directory-listing</reference>
        /// <reference>https://stackoverflow.com/questions/7496913/how-to-load-xml-from-url-on-xmldocument</reference>
        /// <param name="XMLWebAddress"> URL address where to locate the XML string</param>
        /// <returns> List of strings indicating the data set names</return>
        /// </summary>
        public static List<string> GetListOfFileNames(string XMLWebAddress)
        {
            List<string> fileNamesWeb = new List<string>();                                     // will hold all the file names from the amazon site
            string xmlStr;

            try
            {
                using (WebClient wc = new WebClient())
                {
                    xmlStr = wc.DownloadString(XMLWebAddress);                                      // get the xml from the website
                }
                XmlDocument doc = new XmlDocument();                                                // create new xml document
                doc.LoadXml(xmlStr);                                                                // load string into document

                XmlNodeList nodeList = doc.GetElementsByTagName("Key");                             // search document for KEY

                foreach (XmlNode node in nodeList)                                                  // go through each node
                {
                    if (node.InnerXml.Contains(".zip"))                                             // if the name contains a .zip
                    {                                             
                        string fileName = node.InnerXml.Split('.')[0];                              // get rid of file extension .zip
                        fileNamesWeb.Add(fileName);                                                 // save file name
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not get list of names from Amazon" + Environment.NewLine + ex.Message.ToString());
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
            return fileNamesWeb;
        }
        //-----------------------------------------------------------------------
    }
}
