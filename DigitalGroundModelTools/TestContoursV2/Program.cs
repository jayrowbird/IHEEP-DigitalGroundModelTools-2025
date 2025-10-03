using HeepWare.Contouring;
using System;
using System.Collections.Generic;
using System.Linq;
using TriangleNet;
using TriangleNet.IO;

namespace TestContoursV2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Program test = new Program();
            string filenameOnly = "J5P0100Points";
            //filenameOnly = "UstnJTestModel";
            test.Run(filenameOnly);

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private void Run(string filenameOnly)
        {
            string filename = string.Format(@"TestData\{0}.obj", filenameOnly);

            WaveFrontOBJFormat format = new WaveFrontOBJFormat();
            Mesh mesh = format.Import(filename);

            //Lets try to write the triangles as OBJ file
            String trianglestxt = Output.writeOBJTriangleShapes(mesh.Triangles.ToList());
            System.IO.File.WriteAllText(string.Format(@"OutputObj\{0}triangles.obj", filenameOnly), trianglestxt);


            Console.WriteLine("Mesh {0}", mesh); 
             
            ContoursV2 contours = new ContoursV2(5, 1, mesh.Triangles);
             
            List<ContourID> keys_ = contours.GetContourkeys();
            //string elevstr1 = null;
            //string lstring1 = null;
            //StringBuilder  allcontours = new StringBuilder();
            //for (int i = 0; i < keys_.Count; i++)
            //{
            //    ContourV2 contour1 = contours.GetContour(keys_[i]);

            //    contour1.Segments2LineString();
            //    elevstr1 = contour1.getElevation().ToString().Replace(".", "_");
            //    lstring1 = Output.writeOBJContourLineStrings(contour1);
            //    if (lstring1.Length > 5)
            //    {
            //        allcontours.AppendLine(lstring1);
            //        //System.IO.File.WriteAllText(@"OutputObj\UstnJTestModel_contour_" + elevstr1 + ".obj", lstring1);
            //    }
            //}
            //elevstr1 = "allContours";
            //string  lines = OBJOutPut.MergeOBJContourData(allcontours.ToString());
            //System.IO.File.WriteAllText(string.Format(@"OutputObj\{0}_contour_" + elevstr1 + ".obj",filenameOnly), lines);

            if (false)
            {
                //Contour contour1 = contours.GetContour(keys_[4]);
                //String contoursobj = Output.writeOBJContourLineSegements(contour1);
                //System.IO.File.WriteAllText(@"OutputObj\newcontour.obj", contoursobj);
            }

            //if (false)
            //{
            //    String trianglestxt1 = Output.writeDGNTriangleShapes(mesh.Triangles.ToList());

            //    System.IO.File.WriteAllText(@"D:\Copy2\010TriangleNet\TriangleNet\Triangle.NET\TestContours\Output\newtriangles.txt", trianglestxt1);
            //}

            //Contour contour172 = contours.GetContour4Elevation(172d);
            //string elevstr1 = contour172.getElevation().ToString().Replace(".", "_");

            //contour172.Segments2LineString();
            //string lstring1 = Output.writeDGNContourLineStrings(contour172);

            //System.IO.File.WriteAllText(string.Format(@"D:\Copy2\010TriangleNet\TriangleNet\Triangle.NET\TestContours\Output\newcontourElev{0}.txt", elevstr1), lstring1);




            if (false)
            {
                //List<String> keys = contours.GetContourkeys();
                //Contour contour = contours.GetContour(keys[4]);
                //string dgncontourtxt = null;
                //string allLsContours = null;
                //for (int i = 0; i < keys.Count; i++)
                //{
                //    contour = contours.GetContour(keys[i]);
                //    contour.computeContourLinearSegments(mesh.Triangles);
                //    dgncontourtxt += Output.writeContourDGNLines(contour);

                //    contour.Segments2LineString();
                //    string elevstr = contour.getElevation().ToString().Replace(".", "_");
                //    string lstring = Output.writeDGNContourLineStrings(contour);
                //    allLsContours += lstring;

                //    System.IO.File.WriteAllText(string.Format(@"D:\Copy2\010TriangleNet\TriangleNet\Triangle.NET\TestContours\Output\newcontourElev{0}.txt", elevstr), lstring);
                //}

                //System.IO.File.WriteAllText(string.Format(@"D:\Copy2\010TriangleNet\TriangleNet\Triangle.NET\TestContours\Output\newcontourAllLS.txt"), allLsContours);
                //System.IO.File.WriteAllText(@"D:\Copy2\010TriangleNet\TriangleNet\Triangle.NET\TestContours\Output\newcontour.txt", dgncontourtxt);
            }
        }
    }
}
