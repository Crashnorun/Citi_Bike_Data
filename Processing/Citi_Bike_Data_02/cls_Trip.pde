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

class Trip {

  Station StartStation, EndStation;          // start and end stations
  float TripDuration;                        // trip duration in seconds
  int StartHour, EndHour;                    // start and end hour
  int StartMin, EndMin;                      // start and end minute
  int Gender;                                // Gender 0 = Unknown, 1 = Male, 2 = Female
  int BikeID;                                // Bike ID

  Trip() {
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
}