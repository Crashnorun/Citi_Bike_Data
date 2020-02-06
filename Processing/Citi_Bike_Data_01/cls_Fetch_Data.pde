

class FetchData {

  int startYear;
  int endYear;
  int startMonth;
  int endMonth;
  String currentDataName;
  int currentDataNum;

  String urlPrefix;
  String urlSuffix;
  String urlStr;

  byte[] bytes;

  //----Constructor----
  FetchData() {
    startYear = 2013;                                        // set global properties
    endYear = 2016;

    startMonth = 7;
    endMonth = 9;

    if (startMonth < 10) {                                   // set the name of the data set
      currentDataName = startYear + "0" + startMonth;
    } else { 
      currentDataName =  Integer.toString(startYear) + Integer.toString(startMonth);
    }
    currentDataNum = Integer.parseInt(currentDataName);      // convert name to string

    urlPrefix = "https://s3.amazonaws.com/tripdata/";        // set url
    urlSuffix = "-citibike-tripdata.zip";
  }
  //----Constructor----


  void LocateZipFile() {
    println("Fetching URL: " + urlStr);

    try {
      int start = millis();                                  // start timer
      bytes = loadBytes(urlStr);                             // ping url
      int end = millis();                                    // end timer
      println("Time lapsed: " + (end-start)/1000 + " Seconds");
      println("Byte Array Length: " + bytes.length);

      InputStream is = new ByteArrayInputStream(bytes);      // convert byte array to input stream
      ZipInputStream zis = new ZipInputStream(is);           // get the zip file
      ZipEntry ze = zis.getNextEntry();                      // get the item in the zip file
      byte[] bs = new byte[1024];                            // create new byte array
      int len;

      while ((len = zis.read(bs)) > 0) {                     // read the bytes in the csv file
        String str = new String(bs);                         // convert bytes to string
      }
      zis.close();                                           // close the zip stream
    }                                                        // close try
    catch(Exception ex) {
      println("Something happened fetching data");
      println(ex.getMessage());
    }                                                        // close catch
  }


  void ConstructURL() {
    urlStr = urlPrefix + currentDataNum + urlSuffix;        // construct URL string
  }
}      // close class