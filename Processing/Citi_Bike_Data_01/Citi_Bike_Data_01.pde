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
String fileName;
//----GLOBAL VARIABLES----

void setup() {

  sketchPath = sketchPath("");                        // get the sketch file path
  dataPath = dataPath("");                            // get the data file path

  urlStr = urlPrefix + fileNum + urlSuffix;          // construct url
  byte[] b;

  println("Pinging URL: " + urlStr);
  try {
    int start = millis();                              // start timer
    b = loadBytes(urlStr);                             // ping url
    int end = millis();                                // end timer
    println("Time lapsed: " + (end-start)/1000 + " Seconds");
    println("Byte Array Length: " + b.length);

    //println("Saving File : " + fileNum);
    //fileName = dataPath + "\\" + fileNum + ".zip";
    //saveBytes(fileName, b);                             // save zip file
    //UnZipFile(fileName);

    InputStream is = new ByteArrayInputStream(b);    // convert byte array to input stream
    ZipInputStream zis = new ZipInputStream(is);    // get the zip file
    ZipEntry ze = zis.getNextEntry();                // get the item in the zip file
    byte[] bs = new byte[1024];                        // create new byte array
    zis.read(bs);                                      // read the bytes in the csv file
    String str = new String(bs);                        // convert bytes to string
    println(str);                                        // print string
    
  } 
  catch (Exception ex) {
    println("Something happened");
    println(ex.getMessage());
  }

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