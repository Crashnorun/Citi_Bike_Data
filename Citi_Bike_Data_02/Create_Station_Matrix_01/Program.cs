using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Create_Station_Matrix_01
{
    class Program
    {
        private static string FileName = @"C:\Users\cportelli\Documents\Personal\GitHub\Citi_Bike_Data\Citi_Bike_Data_02\201306-citibike-tripdata\201306-citibike-tripdata.csv";
        private static List<int> UniqueStationIDs = new List<int>();
        private static SortedDictionary<int, SortedDictionary<int, int>> Matrix = new SortedDictionary<int, SortedDictionary<int, int>>();

        static void Main(string[] args)
        {
            FindUniqueStations();
            UniqueStationIDs.Sort();

            foreach (int num in UniqueStationIDs)
                Console.WriteLine(num.ToString());


            // create 2D list of stations (matrix)
            for (int i = 0; i < UniqueStationIDs.Count; i++)
            {

            }

            // populate matrix
            Console.ReadKey();
        }


        // find unique station ID's
        private static void FindUniqueStations()
        {
            // first row is headers
            // column 4 is start station ID
            // column 8 is end station ID

            using (StreamReader reader = new StreamReader(FileName))                                // get csv
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();                                                // read line
                    string[] vals = line.Split(',');                                                // split values

                    int num;
                    if (int.TryParse(vals[3], out num))                                             // parse integer
                        if (!UniqueStationIDs.Contains(num)) UniqueStationIDs.Add(num);             // add unique value

                    if (int.TryParse(vals[7], out num))                                             // parse integer
                        if (!UniqueStationIDs.Contains(num)) UniqueStationIDs.Add(num);             // add unique value
                }
            }
        }


        private static void CreateMatrix()
        {
            int count = File.ReadLines(FileName).Count();
            int startId, endId;

            // get start station id
            // get end station id

            using (StreamReader reader = new StreamReader(FileName))                                // get csv
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();                                                // read line
                    string[] vals = line.Split(',');                                                // split values

                    if (int.TryParse(vals[3], out startId))                                         // parse integer
                        if (int.TryParse(vals[7], out endId))                                       // parse integer
                        {
                            if (!Matrix.ContainsKey(startId))                                       // if the startId doesn't exist in the dictionary
                            {
                                Matrix.Add(startId, new SortedDictionary<int, int>() { { endId, 1 } });     // add the first item
                            }
                            else
                            {
                                if (!Matrix[startId].Values.Contains(endId))                         // if the endId already exists
                                {
                               //     Matrix[startId].Values.(new SortedDictionary<int, int> { { endId, 1 } });     // add new one
                                }
                                else
                                {
                                    
                                    // add it's value
                                }
                            }
                        }

                }
            }

        }

    }
}
