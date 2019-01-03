using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citi_Bike_Data_01.Classes
{
    /// <summary>
    /// Contains station data
    /// Station ID = Int
    /// Statin Name = String
    /// Station Latitude = Double
    /// Station Longitude = Double
    /// </summary>
    public class cls_Station
    {
        #region PROPERTIES
        public int StationID
        {
            get { return stationID; }
            set { stationID = value; }
        }
       
        public string StationName
        {
            get { return stationName; }
            set { stationName = value; }
        }

        public double StationLatitude
        {
            get { return stationLatitude; }
            set { stationLatitude = value; }
        }

        public double StationLongitude
        {
            get { return stationLongitude; }
            set { stationLongitude = value; }
        }

        private int stationID;
        private string stationName;
        private double stationLatitude;
        private double stationLongitude;
        #endregion

        public cls_Station()
        {
        }

        public cls_Station(int stationID, string stationName, double stationLatitude, double stationLongitude)
        {
            this.stationID = stationID;
            this.stationName = stationName;
            this.stationLatitude = stationLatitude;
            this.stationLongitude = stationLongitude;
        }
    }
}
