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
    public class cls_Station : IEquatable<cls_Station>
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

        //public override bool Equals(object obj)
        //{
        //    cls_Station other = obj as cls_Station;
        //    return (this.StationID == other.StationID && this.StationName == other.StationName &&
        //      this.StationLatitude == other.StationLatitude && this.StationLongitude == other.StationLongitude);
        //}

        public bool Equals(cls_Station other)
        {
            return (this.StationID == other.StationID && this.StationName == other.StationName &&
              this.StationLatitude == other.StationLatitude && this.StationLongitude == other.StationLongitude);
        }
    }
}
