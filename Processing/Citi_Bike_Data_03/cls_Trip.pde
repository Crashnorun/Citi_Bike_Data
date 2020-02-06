// trip start time
// trip end time
// trip duration
// trip geneder
// line color
// line start pt (lat, long), (x,y)
// line end pt (lat, long), (x,y)
// line thickness
// line growing

//TO DO:
// need to remap the duration time to a stroke width
// need to control the growth of the line based on time and distance

class Trip {

  Station StartStation, EndStation;          // start and end stations
  float TripDuration;                        // trip duration in seconds
  int StartHour, EndHour;                    // start and end hour
  int StartMin, EndMin;                      // start and end minute
  int Gender;                                // Gender 0 = Unknown, 1 = Male, 2 = Female
  int BikeID;                                // Bike ID

  // Blank Constructor
  Trip() {
    StartStation = new Station();
    EndStation = new Station();
  }

  void PopulateTripData(IntDict columnValues, String[] values) {

    int index ;
    int time[];

    for (int i = 0; i < columnValues.size(); i++) {
      switch (columnValues.keyArray()[i]) {
      case "tripduration":
        index = columnValues.get("tripduration");
        TripDuration = float(values[index]);
        break;
      case "starttime":
        index = columnValues.get("starttime");
        time = ConvertTimeToInt(values[index]);
        StartHour = time[0];
        StartMin = time[1];
        break;
      case "endtime":
        index = columnValues.get("endtime");
        time = ConvertTimeToInt(values[index]);
        EndHour = time[0];
        EndMin = time[1];
        break;
      case "gender":
        index = columnValues.get("endtime");
        Gender = int(values[index]);
        break;
      case "bikeid":
        index = columnValues.get("bikeid");
        BikeID = int(values[index]);
        break;
      case "start station id":
        index = columnValues.get("start station id");
        StartStation.StationID = int(values[index]);
        break;
      case "start station name":
        index = columnValues.get("start station name");
        StartStation.Address = values[index];
        break;  
      case "start station latitude":
        index = columnValues.get("start station latitude");
        StartStation.Latitude = float(values[index]);
        break;
      case "start station longitude":
        index = columnValues.get("start station longitude");
        StartStation.Longitude = float(values[index]);
        break;
      case "end station id":
        index = columnValues.get("end station id");
        EndStation.StationID = int(values[index]);
        break;
      case "end station name":
        index = columnValues.get("end station name");
        EndStation.Address = values[index];
        break;  
      case "end station latitude":
        index = columnValues.get("end station latitude");
        EndStation.Latitude = float(values[index]);
        break;
      case "end station longitude":
        index = columnValues.get("end station longitude");
        EndStation.Longitude = float(values[index]);
        break;
      default:
        println("Something happend when converting string to trip data: cls_Trip: PopulateTripData");
        break;
      }
    }
  }

  // convert the String value of start and stop time to hour and minute integers
  // return an array of integers with two indices, hour & minutes
  int[] ConvertTimeToInt( String time) {

    int[] timeVals = new int[1];                            // empty array to hold the values
    String[] splitVals = splitTokens(time, " ");            // split the date-time by the space
    String[] splitTime = splitTokens(splitVals[1], ":");    // split the time by the colon
    timeVals[0] = int(splitTime[0]);                        // save the hour
    timeVals[1] = int(splitTime[1]);                        // save the minute
    return timeVals;                                        // return the values
  }

  void Render() {

    strokeWeight(1);
    color strokeColor;
    if (Gender ==0) {                        // unknown gender
      strokeColor = color(63, 191, 127);     // sea grean = unknown gender
    } else if (Gender == 1) {
      strokeColor = color(63, 191, 191);     // Tirquoise = male gender
    } else {
      strokeColor = color(255, 191, 191);    // Pink = female gender
    }
    stroke(strokeColor);
    line(StartStation.Coords.x, StartStation.Coords.y, EndStation.Coords.x, EndStation.Coords.y);
  }
  //---------------------------------------------------
}      // close class