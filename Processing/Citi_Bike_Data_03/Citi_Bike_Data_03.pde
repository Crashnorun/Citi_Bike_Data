// TO DO: //<>//
// make a class to display the time on the lower left corner
// need to make bottons for twitter and weather

//----IMPORTS----
import java.io.ByteArrayInputStream;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.util.zip.ZipEntry;
import java.util.zip.ZipInputStream;
//----IMPORTS----

//----GLOBAL VARIABLES----
String urlStr;
String urlPrefix = "https://s3.amazonaws.com/tripdata/";
String urlSuffix = "-citibike-tripdata.zip";
int fileNum = 201307;

String sketchPath;                      
String dataPath;     

Trip[] trips;
//----GLOBAL VARIABLES----

void setup() {

  FetchData fData = new FetchData();
  fData.ConstructURL();                    // construct the url where the data will be pulled from
  fData.LocateZipFile();                   // locate the ZIP file on the web and get bytes
  fData.trips = trips;
  //fData.ReadDataFromFile();                // save the zip file on disk 
  fData.ReadDataFromURL();                 // save the zip file data in an array??


  println("----DONE----");
}
//---------------------------------------------------


void draw() {
}
//---------------------------------------------------