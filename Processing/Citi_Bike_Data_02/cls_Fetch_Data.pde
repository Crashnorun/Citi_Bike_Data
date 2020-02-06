// TO DO:
// find the length of the csv (number of rows)
// find a way to read all the lines in the csv in a structured manner
// find a way to indetify when you've reached the end of the csv
// load each line of the csv into an array of cls_Trip

class FetchData {

  int startYear, endYear, currentYear;
  int startMonth, endMonth, currentMonth;
  String currentDataName;
  int currentDataNum;

  String urlPrefix, urlSuffix;
  String urlStr;

  byte[] bytes;
  InputStream is;                                            // used to convert byte array to input stream
  ZipInputStream zis;                                        // used to convert the input stream to get the zip file
  ZipEntry ze;                                               // used to get the csv file from within the zip file

  //----Constructor----
  FetchData() {
    startYear = 2013;                                        // set global properties
    endYear = 2016;
    currentYear = 2013;

    startMonth = 7;
    endMonth = 9;
    currentMonth = 7;

    if (startMonth < 10) {                                   // set the name of the data set
      currentDataName = startYear + "0" + startMonth;
    } else { 
      currentDataName =  Integer.toString(startYear) + Integer.toString(startMonth);
    }
    currentDataNum = Integer.parseInt(currentDataName);      // convert name to number

    urlPrefix = "https://s3.amazonaws.com/tripdata/";        // set url
    urlSuffix = "-citibike-tripdata.zip";
  }
  //----Constructor----


  // Construct URL with urrent date
  void ConstructURL() {
    urlStr = urlPrefix + currentDataNum + urlSuffix;        // construct URL string
  }
  //---------------------------------------------------


  // based on a URL go and fetch the ZIP data
  // return TRUE on success
  // return FALSE on failure
  boolean LocateZipFile() {
    println("Fetching URL: " + urlStr);

    try {
      int start = millis();                                  // start timer
      bytes = loadBytes(urlStr);                             // ping url
      int end = millis();                                    // end timer
      println("Time lapsed: " + (end-start)/1000 + " Seconds");
      println("Byte Array Length: " + bytes.length);

      if (bytes == null || bytes.length ==0) {
        println("Returned bytes is empty, could not get ZIP file from URL");
        return false;
      } else {
        return true;
      }
    }                                                        // close try
    catch(Exception ex) {
      println("Something happened fetching data");
      println(ex.getMessage());
      return false;
    }                                                        // close catch
  }
  //---------------------------------------------------


  // read the csv data directly from the web
  void ReadDataFromURL() {
    try {
      is = new ByteArrayInputStream(bytes);                  // convert byte array to input stream
      zis = new ZipInputStream(is);                          // get the zip file

      ze = zis.getNextEntry();                               // get the item in the zip file
      println("CSV File Name is: " + ze.getName());
      byte[] bs = new byte[1024];                            // create new byte array
      int len;
     
      println("Reading data from: " + ze.getName());
      
      while ((len = zis.read(bs)) > 0) {                     // read the bytes in the csv file
        String str = new String(bs);                         // convert bytes to string
      }

      zis.close();                                           // close the zip stream
    }
    catch(IOException ex) {
      println("File IO Exception: " + ex.getMessage());
    }
  }
  //---------------------------------------------------


  // read the csv data from a saved file
  void ReadDataFromFile() {

    String sketchPath = sketchPath("");                        // get the sketch file path
    String dataPath = dataPath("");                            // get the data file path
    String fileName = dataPath + "\\" + currentDataName + ".zip";
    saveBytes(fileName, bytes);                                // save the downloaded bytes (the zip file)
    File f = new File(fileName);                               // load file

    if (f.exists()) {                                          // check if file exists
      try {
        zis = new ZipInputStream(new FileInputStream(fileName)); // get the zip file

        // http://www.mkyong.com/java/how-to-decompress-files-from-a-zip-file/
        while ((ze = zis.getNextEntry()) != null)              // if there is a file in the zip file
        {
          String name = ze.getName();                          // get the csv file name
          println("CSV File Name is: " + name);

          File newFile = new File(dataPath + "\\" + name);     // create CSV file in the diretory
          FileOutputStream fos = new FileOutputStream(newFile);   // create an output stream
          int len;
          byte[] b = new byte[1024];

          println("Writing data to file: " + name);

          while ((len = zis.read(b)) > 0) {                    // read the data
            fos.write(b, 0, len);                              // write the data to the file
          }
          fos.close();
        }
        zis.close();                                           // close the zip stream
      }
      catch(FileNotFoundException ex) {
        println("File Not Found Exception: " + ex.getMessage());
      }
      catch(IOException ex) {
        println("File IO Exception: " + ex.getMessage());
      }
    } else {
      println("Zip file does not exist");
    }
  }
  //---------------------------------------------------

  // Get the next date 
  void GetNextDate() {

    if (currentMonth == 12) {
      currentYear++;
      currentMonth = 1;
    } else {
      currentMonth++;
    }

    if (currentMonth< 10) {
      currentDataName = startYear + "0" + startMonth;
    } else {
      currentDataName =  Integer.toString(startYear) + Integer.toString(startMonth);
    }
    currentDataNum = Integer.parseInt(currentDataName);      // convert name to number
  }
  //---------------------------------------------------
}      // close class