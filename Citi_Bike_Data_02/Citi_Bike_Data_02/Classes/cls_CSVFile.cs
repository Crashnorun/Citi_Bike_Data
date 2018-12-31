using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citi_Bike_Data_02.Classes
{
    [Serializable]
    public class cls_CSVFile
    {

        #region ----PROPERTIES----

        public string CSVFileName;
        public string ZIPFileName;
        public string FullFolderPath;

        #endregion


        /// <summary>
        /// Blank constructor
        /// </summary>
        public cls_CSVFile() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="csvFileName">CSV file name</param>
        /// <param name="zipFileName">ZIP file name</param>
        public cls_CSVFile(string csvFileName, string zipFileName)
        {
            this.CSVFileName = csvFileName;
            this.ZIPFileName = zipFileName;
        }
    }
}
