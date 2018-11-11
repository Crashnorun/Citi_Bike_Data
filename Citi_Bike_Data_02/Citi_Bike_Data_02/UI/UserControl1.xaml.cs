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

namespace Citi_Bike_Data_02.UI
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();

            lbl_Status_01.Content = string.Empty;
            Color col = (Color)ColorConverter.ConvertFromString(Properties.Resources.CitiBikeColorHEX);         // get color from properties
            SolidColorBrush fill = new SolidColorBrush(col);                                                    // create a solid brush
            this.progressBar1.Foreground = fill;                                                                // apply brush to progress bar
        }

        private void btn_CreateDB_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(Properties.Resources.ConnectionString))
            {
                try
                {
                    conn.Open();
                    lbl_Status_01.Content = "Established connection to DB";
                }
                catch (SqlException ex)
                {
                    lbl_Status_01.Content = "Unable to establish connection to DB" + ex.Message;
                }
            }
        }

        private void btn_DlXML_Click(object sender, RoutedEventArgs e)
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;                         // path in URI format
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);                                     // remove prefix
            string localPath = System.IO.Path.GetDirectoryName(path);                           // get actual folder path
            lbl_Status_01.Content = localPath;

            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += ProgressBarChanged;
                wc.DownloadFileAsync(new System.Uri(Properties.Resources.URLXML), localPath);
            }
        }

        void ProgressBarChanged(object sender, EventArgs e)
        {
            try
            {
                DownloadProgressChangedEventArgs ev = (DownloadProgressChangedEventArgs)e;
                progressBar1.Value = ev.ProgressPercentage;
            } catch (Exception ex)
            {
                lbl_Status_01.Content = ex.Message;
            }
            
        }
    }
}
