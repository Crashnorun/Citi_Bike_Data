// TO DO:
// create class of location
// create class of trip
// should I make a class to display the time on the lower left corner??
// should I make a helper class to transform coordinates??

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
//----GLOBAL VARIABLES----

void setup() {
  sketchPath = sketchPath("");                        // get the sketch file path
  dataPath = dataPath("");                            // get the data file path

  FetchData fData = new FetchData();
  fData.ConstructURL();
  fData.LocateZipFile();
  //fData.ReadDataFromFile();
  fData.ReadDataFromURL();

  println("----DONE----");
}
//---------------------------------------------------


void draw() {
}
//---------------------------------------------------


// LOOK INTO READING THE DATA WITHOUT SAVING THE ZIP FILE
void UnZipFile(String file) {
  int count = 0;

  File f = new File(file);                              // load file
  if (f.exists()) {                                     // check if file exists

    try {
      ZipInputStream zis = new ZipInputStream(new FileInputStream(file));    // get the zip file
      ZipEntry ze;

      // http://www.mkyong.com/java/how-to-decompress-files-from-a-zip-file/
      while ((ze = zis.getNextEntry()) != null)        // if there is a file in the zip file
      {
        String name = ze.getName();                    // get the file name
        println("Name is: " + name);

        File newFile = new File(dataPath + "\\" + name);        // create file in the diretory
        FileOutputStream fos = new FileOutputStream(newFile);   // create an output stream
        int len;
        byte[] b = new byte[1024];

        while ((len = zis.read(b)) > 0) {                        // read the data
          String str = new String(b);
          if (count < 10) {
            println(str);
          }
          count++;
          fos.write(b, 0, len);                                  // write the data to the file
          //println("data item: " + b);
        }
      }
      zis.close();                                              // close the zip stream
    }
    catch(FileNotFoundException ex) {
      println("File Not Found Exception: " + ex.getMessage());
    }
    catch(IOException ex) {
      println("File IO Exception: " + ex.getMessage());
    }
  }
}
//---------------------------------------------------