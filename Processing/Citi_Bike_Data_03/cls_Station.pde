// location - lat long
// location - x, y
// location name
// location address
// number of trips to / from location
// location circle size
// location color / opacity

// TO DO:
// need to link the number of trips to / from location to the radius of the circle
// need to transform lat long to coordinates 

class Station {

  PVector Location, Coords;                  // station coords, lat, long
  Float Longitude, Latitude;                 // lat, long
  String Name, Address;                      // station name, address
  int StationID;                             // station ID
  int Count;                                 // number of trips
  color FillColor;
  float Radius;

  Station() { 
    Count = 1;
    FillColor = color(255, 100, 0, 0);
    ellipseMode(CENTER);
  }

  Station (float latitude, float longitude ) {
    Location = new PVector(latitude, longitude);
    Latitude = latitude;
    Longitude = longitude;
    Count = 1;
    FillColor = color(255, 0, 255);
    ellipseMode(CENTER);
  }

  void Render() {
    fill(FillColor);
    noStroke();
    ellipse(Coords.x, Coords.y, Radius, Radius);
  }
}