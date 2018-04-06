﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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


using Amazon.S3.Model;
using Amazon.S3;
using Amazon.S3.IO;

namespace ToDo
{
    /*
     * Check for new data
     *  go to amazon site and check the number of files against the number of local files
     * Get the data from amazon site
     *  download to local folder
     *-> Clean data
     *  unzip files and check for missing data
     *  report missing data to a dialog
     * Store data in a Mongo DB  
     */
}

namespace References
{
    /*
     * Unzip files into memory: https://stackoverflow.com/questions/22604941/how-can-i-unzip-a-file-to-a-net-memory-stream
     * Amazon sdk: http://docs.aws.amazon.com/sdkfornet/v3/apidocs/Index.html
     * Trip data xml: https://s3.amazonaws.com/tripdata/
     * Trip data zip: https://s3.amazonaws.com/tripdata/index.html
     * https://stackoverflow.com/questions/22604941/how-can-i-unzip-a-file-to-a-net-memory-stream
     */
}

namespace Notes
{
    /*
     * Birthdate can be NULL, blank, or\N
     * Stattion Lat and Long may be 0 0
     * 
     */
}



namespace Citi_Bike_Data_01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region ----GLOBAL VARIABLES ----
        public string WebAddress
        {
            get { return "https://s3.amazonaws.com/tripdata/index.html"; }      // human readable site
        }

        public string XMLWebAddress
        {
            get { return "https://s3.amazonaws.com/tripdata/"; }                // site with xml data
        }

        public string DestinationFolder
        {
            //get { return @"D:\_My Stuff\Projects\Citi Bike Data\Data Files\Original Data Files\"; }
            get { return destinationFolder; }
        }

        public List<string> FileNamesWeb
        {
            get { return fileNamesWeb; }                // file names located on the web
        }

        public List<string> FileNamesLocal
        {
            get { return fileNamesLocal; }              // file names located on local drive
        }

        public List<string> FileNamesNew
        {
            get { return fileNamesNew; }                // new file names
        }

        public string awsAccessKeyId
        {
            get { return "AKIAIXYFT43AM7LCA25Q"; }
        }

        public string aswSecretAccessKey
        {
            get { return "FXide7xruZFhv2sMzEQ13qh8CvXPNvzWZoRVdf0f"; }
        }

        private List<string> fileNamesWeb;
        private List<string> fileNamesLocal;
        private List<string> fileNamesNew;
        private string destinationFolder;
        #endregion


        public MainWindow()
        {
            InitializeComponent();
            fileNamesWeb = new List<string>();
            fileNamesLocal = new List<string>();
            fileNamesNew = new List<string>();

            string AssemblyFolderPath = cls_Sync_Data.GetExecutingAssemblyPath();               // get assembly folder path
            destinationFolder = cls_Sync_Data.GetDataFolder(AssemblyFolderPath);                // get folder path where data will be stored
            fileNamesLocal.AddRange(Directory.GetFiles(destinationFolder));                     // get the files in the local folder
            fileNamesWeb = cls_Sync_Data.GetListOfFileNames(XMLWebAddress);                     // get the number of files on amazon
            cls_Sync_Data.DestinationFolder = destinationFolder;
            fileNamesNew = cls_Sync_Data.CompareFileNames(FileNamesLocal, XMLWebAddress);       // compare files and download new ones

            cls_Helper.AddFileNamesToTable(fileNamesWeb);
            // if new files are downloaded, unzip them
            // get all new files and check / clean data
            // put data into DB
        }

        // download the citi bike data files
        private void button_Click(object sender, RoutedEventArgs e)
        {

        }           // close button click event


        /// <summary>
        /// <reference>https://stackoverflow.com/questions/33545065/amazon-s3-how-to-get-a-list-of-folders-in-the-bucket</reference>
        /// <reverence>http://docs.aws.amazon.com/sdkfornet1/latest/apidocs/html/T_Amazon_S3_Model_ListObjectsRequest.htm</reverence>
        /// </summary>
        private void AmazonVersion()
        {

            string BucketName = WebAddress;
            AmazonS3Client s3Client = new AmazonS3Client(awsAccessKeyId, aswSecretAccessKey, Amazon.RegionEndpoint.USEast1);
            //S3DirectoryInfo dir = new S3DirectoryInfo(s3Client, "doc/2006-03-01/");
            //S3DirectoryInfo dir = new S3DirectoryInfo(s3Client, "tripdata/");
            //bool exists = dir.Exists;

            //foreach(IS3FileSystemInfo file in dir.GetFileSystemInfos())
            //{
            //    Debug.Print(file.Name);                             //https://stackoverflow.com/questions/39225114/c-sharp-list-all-files-with-filename-under-a-amazon-s3-folder
            //}

            /*    IAmazonS3 client;
                ListBucketsResponse res = client.ListBuckets();

                using (client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1))
                {
                    ListObjectsRequest request = new ListObjectsRequest
                    {
                        BucketName = WebAddress
                    };

                    do
                    {
                        ListObjectsResponse response = client.ListObjects(request);

                        IEnumerable<S3Object> folders = response.S3Objects.Where(x => x.Key.EndsWith(@"/") && x.Size == 0);
                        List<S3Object> objs = folders.ToList();

                        if (response.IsTruncated)
                        {
                            request.Marker = response.NextMarker;
                        }
                        else
                        {
                            request = null;
                        }
                    } while (request != null);
                }*/
        }

    }               // close class
}                   // close namespace
