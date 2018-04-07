using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Citi_Bike_Data_01.Classes
{
    public class cls_Trip
    {
        #region PROPERTIES
        /// <summary>
        /// Duration calculated in SECONDS
        /// </summary>
        public int TripDuration
        {
            get { return tripDuration; }
            set { tripDuration = value; }
        }

        /// <summary>
        /// Format mm/dd/year 24hr:min
        /// </summary>
        public DateTime StartTime
        {
            get { return startTime; }
            set { startTime = value; }
        }

        /// <summary>
        /// Format mm/dd/year 24hr:min
        /// </summary>
        public DateTime StopTime
        {
            get { return stopTime; }
            set { stopTime = value; }
        }

        public cls_Station StartStation
        {
            get { return startStation; }
            set { startStation = value; }
        }

        public cls_Station EndStation
        {
            get { return endStation; }
            set { endStation = value; }
        }

        public int BikeID
        {
            get { return bikeID; }
            set { bikeID = value; }
        }

        public cls_User User
        {
            get { return user; }
            set { user = value; }
        }
        #endregion

        #region PRIVATE FIELDS
        private int tripDuration;
        private DateTime startTime;
        private DateTime stopTime;
        private cls_Station startStation;
        private cls_Station endStation;
        private int bikeID;
        private cls_User user;
        private cls_File_Info fileInfo;
        #endregion

        public cls_Trip()
        {
        }
    }
   
}
