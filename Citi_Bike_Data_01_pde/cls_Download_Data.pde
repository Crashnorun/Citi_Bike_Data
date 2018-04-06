

class cls_Download_Data {

  String URL = "https://s3.amazonaws.com/tripdata/index.html";
  ArrayList<String> FileNamesNY;
  ArrayList<String> FileNamesNJ;

  cls_Download_Data() {
    FileNamesNY = new ArrayList<String>();
    FileNamesNJ = new ArrayList<String>();
    PopulateNYFileNames();
  }// close constructor


  void DownLoadFiles() {
    // go to website
    // download each individual csv file
    
    for(int i = 0; i < FileNamesNY.size(); i++){
      URL url = new URL(URL);
      ReadableByteChannel rbc = Channels.newChannel(url.openStream());
      
      
    }
    
    
  }

  void PopulateNYFileNames() {
    FileNamesNY.add("201307-201402-citibike-tripdata.zip"); 
    FileNamesNY.add("201307-citibike-tripdata.zip");
    FileNamesNY.add("201308-citibike-tripdata.zip");
    FileNamesNY.add("201309-citibike-tripdata.zip");
    FileNamesNY.add("201310-citibike-tripdata.zip");
    FileNamesNY.add("201311-citibike-tripdata.zip");
    FileNamesNY.add("201312-citibike-tripdata.zip");

    FileNamesNY.add("201401-citibike-tripdata.zip");
    FileNamesNY.add("201402-citibike-tripdata.zip");
    FileNamesNY.add("201405-citibike-tripdata.zip");
    FileNamesNY.add("201406-citibike-tripdata.zip");
    FileNamesNY.add("201407-citibike-tripdata.zip");
    FileNamesNY.add("201408-citibike-tripdata.zip");
    FileNamesNY.add("201409-citibike-tripdata.zip");
    FileNamesNY.add("201410-citibike-tripdata.zip");
    FileNamesNY.add("201411-citibike-tripdata.zip");
    FileNamesNY.add("201412-citibike-tripdata.zip");

    FileNamesNY.add("201501-citibike-tripdata.zip");
    FileNamesNY.add("201502-citibike-tripdata.zip");
    FileNamesNY.add("201505-citibike-tripdata.zip");
    FileNamesNY.add("201506-citibike-tripdata.zip");
    FileNamesNY.add("201507-citibike-tripdata.zip");
    FileNamesNY.add("201508-citibike-tripdata.zip");
    FileNamesNY.add("201509-citibike-tripdata.zip");
    FileNamesNY.add("201510-citibike-tripdata.zip");
    FileNamesNY.add("201511-citibike-tripdata.zip");
    FileNamesNY.add("201512-citibike-tripdata.zip");

    FileNamesNY.add("201601-citibike-tripdata.zip");
    FileNamesNY.add("201602-citibike-tripdata.zip");
    FileNamesNY.add("201605-citibike-tripdata.zip");
    FileNamesNY.add("201606-citibike-tripdata.zip");
    FileNamesNY.add("201607-citibike-tripdata.zip");
    FileNamesNY.add("201608-citibike-tripdata.zip");
    FileNamesNY.add("201609-citibike-tripdata.zip");
    FileNamesNY.add("201610-citibike-tripdata.zip");
    FileNamesNY.add("201611-citibike-tripdata.zip");
    FileNamesNY.add("201612-citibike-tripdata.zip");

    FileNamesNY.add("201701-citibike-tripdata.zip");
    FileNamesNY.add("201702-citibike-tripdata.zip");
    FileNamesNY.add("201705-citibike-tripdata.zip");
    FileNamesNY.add("201706-citibike-tripdata.zip");
    FileNamesNY.add("201707-citibike-tripdata.zip");

    /*FileNamesNY.add("201708-citibike-tripdata.zip");
     FileNamesNY.add("201709-citibike-tripdata.zip");
     FileNamesNY.add("201710-citibike-tripdata.zip");
     FileNamesNY.add("201711-citibike-tripdata.zip");
     FileNamesNY.add("201712-citibike-tripdata.zip");*/
  }


  void PopulateFilesNJ() {
    FileNamesNJ.add("JC-201509-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201510-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201511-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201512-citibike-tripdata.csv.zip");

    FileNamesNJ.add("JC-201601-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201602-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201603-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201604-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201605-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201606-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201607-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201608-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201609-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201610-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201611-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201612-citibike-tripdata.csv.zip");

    FileNamesNJ.add("JC-201701-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201702-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201703-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201704-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201705-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201706-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201707-citibike-tripdata.csv.zip");
    
    /*FileNamesNJ.add("JC-201708-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201709-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201710-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201711-citibike-tripdata.csv.zip");
    FileNamesNJ.add("JC-201712-citibike-tripdata.csv.zip");*/
  }
}  // close class