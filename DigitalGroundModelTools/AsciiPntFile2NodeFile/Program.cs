using System;
using System.Collections.Generic;
using System.IO; 

namespace AsciiPntFile2NodeFile
{
    class Program
    {
        static void Main(string[] args)
        {
            List<String> strList = new List<string>();
            String filename = @"D:\Alien2016\00SBrown\TriangleNet\LasTools\SteveAscii.txt";
            String line;
            long count = 0;
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                using (StreamReader sr = new StreamReader(filename))
                {  

                //Read the first line of text
                line = sr.ReadLine();
                    //Do not change the first line
                strList.Add(line);

                    line = sr.ReadLine();
                    //Continue to read until you reach end of file
                    while (line != null)
                {
                    count++;
                    //write the lie to console window
                    line = "8" + " " + line;
                    if (count % 10000 == 0)
                        Console.Write("\r" + line);

                    strList.Add(line);
                    //Read the next line
                    line = sr.ReadLine();
                }
                Console.WriteLine();
                //close the file
                sr.Close();
            }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }

            if (strList.Count > 0)
            {
                try
                {
                    FileInfo fileinfo = new FileInfo(filename);
                    String wrFilename = fileinfo.FullName.Replace(fileinfo.Extension, ".node");
                    //Pass the file path and filename to the StreamWriter Constructor
                    using (StreamWriter sw = new StreamWriter(wrFilename, false))
                    {

                        for (int i = 0; i < strList.Count; i++)
                        {
                            //Write line to file
                            sw.WriteLine(strList[i]);
                            if (i % 10000 == 0)
                                Console.Write("\r" + strList[i]);
                        }


                        //Close the file
                        sw.Close();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
                finally
                {
                    Console.WriteLine("Executing finally block.");
                }
            }
            
        }
    }
}
