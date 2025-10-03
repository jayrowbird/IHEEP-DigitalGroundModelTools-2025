using NewTriangle.IO;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Text;
using TriangleNet;
using TriangleNet.Data;
using TriangleNet.Geometry;
using TriangleNet.IO;

namespace TestEarthworkVolumes
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, Earthwork volumes World!");

            Program program = new Program();
            //program.MakeEarthworkTest();

            program.J5P0100Points2OBJ();

            List<List<Vertex>> prisms = program.MakeEarthworkWedgeTest2();

            program.WritePrism2Wavefront(prisms);


            //program.Make10x10WedgeSurface();

            //program.GCoppConvexHull();
            //program.GCoppConvexHull2();

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private void WritePrism2Wavefront(List<List<Vertex>> prisms)
        {
            int indexCount = 0;
            StringBuilder sb = new StringBuilder();
            string filename = "output\\Prisms.obj";

            for (int i = 0; i < prisms.Count; i++)
            {
                List<Vertex> prism = prisms[i];
                for (int n = 0; n < prism.Count; n++)
                {
                    sb.AppendLine(string.Format("v {0} {1} {2}", prism[n].X, prism[n].Y, prism[n].Z));
                }
                sb.AppendLine(string.Format("l {0} {1} ", 1 + indexCount, 2 + indexCount ));
                sb.AppendLine(string.Format("l {0} {1} ", 3 + indexCount, 4 + indexCount));
                sb.AppendLine(string.Format("l {0} {1} ", 5 + indexCount, 6 + indexCount));

                //sb.AppendLine(string.Format("f {0} {1} {2}", 1 + indexCount, 3 + indexCount, 5 + indexCount));
                //sb.AppendLine(string.Format("f {0} {1} {2}", 2 + indexCount, 4 + indexCount, 6 + indexCount));


                indexCount += 6;
                //Below display triangles
                if (false)
                {
                    sb.AppendLine(string.Format("f {0} {1} {2}", 1 + indexCount, 2 + indexCount, 3 + indexCount));
                    sb.AppendLine(string.Format("f {0} {1} {2}", 2 + indexCount, 4 + indexCount, 3 + indexCount));

                    sb.AppendLine(string.Format("f {0} {1} {2}", 3 + indexCount, 4 + indexCount, 5 + indexCount));
                    sb.AppendLine(string.Format("f {0} {1} {2}", 4 + indexCount, 6 + indexCount, 5 + indexCount));

                    sb.AppendLine(string.Format("f {0} {1} {2}", 5 + indexCount, 6 + indexCount, 1 + indexCount));
                    sb.AppendLine(string.Format("f {0} {1} {2}", 6 + indexCount, 2 + indexCount, 1 + indexCount));

                    sb.AppendLine(string.Format("f {0} {1} {2}", 1 + indexCount, 3 + indexCount, 5 + indexCount));
                    sb.AppendLine(string.Format("f {0} {1} {2}", 2 + indexCount, 4 + indexCount, 6 + indexCount));
                    indexCount += 6;
                }
            }
            File.WriteAllText(filename, sb.ToString());
        }


        private void GCoppConvexHull2()
        {
            StringBuilder sb = new StringBuilder();
            string filename = "TestData\\UstnJTestModel.pts";
            //See if we can get the convex hull for the proposed surface
            Behavior behavior = new Behavior();
            behavior.Convex = true;
            Mesh mesh = new Mesh(behavior);

            InputGeometry test = ReadTextXYZ.ReadTextXYZFile(filename);

            mesh.Triangulate(test);

            List<Vertex> hull = mesh.GetConvexHull();
            sb.Clear();
            for (int i = 0; i < hull.Count; i++)
            {
                sb.AppendLine(string.Format("v {0} {1} {2}", hull[i].X, hull[i].Y, hull[i].Z));
            }
            for (int i = 0; i < hull.Count - 1; i++)
            {
                sb.AppendLine(string.Format("l {0} {1} ", i + 1, i + 2));
            }

            File.WriteAllText("Output\\UstnJTestModelHull.obj", sb.ToString());

        }

        private void J5P0100Points2OBJ()
        {
            StringBuilder sb = new StringBuilder();
            string filename = "TestData\\J5P0100Points.node";
            //See if we can get the convex hull for the proposed surface
            Behavior behavior = new Behavior();
            behavior.Convex = true;
            Mesh mesh = new Mesh(behavior);

            mesh.Triangulate(filename);

            WaveFrontOBJFormat waveFrontOBJFormat = new WaveFrontOBJFormat();
            waveFrontOBJFormat.Write(mesh, "Output\\J5P0100PointsTris.obj");
        }

        private void GCoppConvexHull()
        {
            StringBuilder sb = new StringBuilder();
            string filename = "TestData\\J5P0100Points.node";
            //See if we can get the convex hull for the proposed surface
            Behavior behavior = new Behavior();
            behavior.Convex = true;
            Mesh mesh = new Mesh(behavior);

            mesh.Triangulate(filename);

            List<Vertex> hull = mesh.GetConvexHull();
            sb.Clear();
            for (int i = 0; i < hull.Count; i++)
            {
                sb.AppendLine(string.Format("v {0} {1} {2}", hull[i].X, hull[i].Y, hull[i].Z));
            }
            for (int i = 0; i < hull.Count - 1; i++)
            {
                sb.AppendLine(string.Format("l {0} {1} ", i + 1, i + 2));
            }

            File.WriteAllText("Output\\J5P0100PointsHull.obj", sb.ToString());

        }

        private void Make10x10WedgeSurface()
        {
            List<Vector3> pts = new List<Vector3>();
            float z = 0.0f;
            float maxc = 5.0f;
            float minc = -5.0f;
            pts.Add(new Vector3(minc, minc, 5.0f));
            pts.Add(new Vector3(maxc, maxc, 0.0f));
            pts.Add(new Vector3(maxc, minc, 0.0f));
            pts.Add(new Vector3(minc, maxc, 5.0f));

            float min = minc;
            float max = maxc;
            float x;
            float y;

            Random rand = new Random();
            float elev = 0.0f;
            for (int i = 0; i < 100; i++)
            {
                x = (float)rand.NextDouble() * (max - min) + min;
                y = (float)rand.NextDouble() * (max - min) + min;
                elev = ((maxc - x) / 10.0f) * 5.0f;
                pts.Add(new Vector3(x, y, elev));
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < pts.Count; i++)
            {
                sb.AppendLine(string.Format("{0} {1} {2}", pts[i].X, pts[i].Y, pts[i].Z));
            }

            string filename = "Output\\xyz_10x10xWedge.txt";

            File.WriteAllText(filename, sb.ToString());

            Behavior behavior = new Behavior();
            behavior.Convex = true;
            Mesh mesh = new Mesh(behavior);

            InputGeometry test = ReadTextXYZ.ReadTextXYZFile(filename);

            mesh.Triangulate(test);

            WaveFrontOBJFormat waveFrontOBJFormat = new WaveFrontOBJFormat();
            waveFrontOBJFormat.Write(mesh, "Output\\xyz_10x10xWedge.obj");

        }

        private List<List<Vertex>> MakeEarthworkWedgeTest2()
        {
            // existing mesh surface is natural ground 
            // proposed mesh surface is the design or stockpile surface
            string proposedSurfaceFilename = "Output\\xyz_10x10xWedge.txt";
            string existingSurfaceFilename = "Output\\xyz_20x20xz5.txt";

            InputGeometry proposed = ReadTextXYZ.ReadTextXYZFile(proposedSurfaceFilename);
            InputGeometry existing = ReadTextXYZ.ReadTextXYZFile(existingSurfaceFilename);

            Mesh meshProposed = new Mesh();
            meshProposed.Triangulate(proposed);

            Mesh meshExisting = new Mesh();
            meshExisting.Triangulate(existing);

            LocateResult findresult;
            Triangle foundTri = new Triangle();

            double TotalVolume = 0.0;
            double TotalArea = 0.0;

            List<List<Vertex>> prisms = new List<List<Vertex>>();
            for (int n = 0; n < meshProposed.Triangles.Count; n++)
            {
                Triangle tri1 = meshProposed.Triangles.ElementAt(n);
                double? zlength = 0;
                //find the zlength for each of the existing triangle points
                List<Vertex> vertices = new List<Vertex>();
                for (int m = 0; m < 3; m++)
                {
                    Vertex v = tri1.GetVertex(m);
                    vertices.Add(v);
                    findresult = meshExisting.FindContainingTriangle(v, ref foundTri);
                    if (findresult == LocateResult.Outside)
                    {
                        Console.WriteLine("point outside mesh bounds");
                        continue;
                    }
                    // point projected to the triangle plane is returned
                    Vertex v12 = TriangleMathVertex.ProjectPointOnTrianglePlane(v, foundTri.GetVertex(0), foundTri.GetVertex(1), foundTri.GetVertex(2));
                    vertices.Add(v12);
                    double z12 = Math.Abs(v12.Z - v.Z);
                    zlength += z12;
                }
                prisms.Add(vertices);

                zlength /= 3;
                List<Vertex> pts1 = tri1.GetVertices();
                //Compute the area of the triangle perpendicular to the z or elevation axis
                double area = TriangleMathVertex.triArea(pts1[0], pts1[1], pts1[2]);
                //triangle prism is the triangle area * the average of the 3 heights at the triangle points
                TotalVolume += area * (double)zlength;
                TotalArea += area;
            }
            return prisms;
        }

        private void MakeEarthworkTest()
        {
            List<Vector3> pts = new List<Vector3>();
            float z = 0.0f;
            float maxc = 5.0f;
            float minc = -5.0f;
            pts.Add(new Vector3(minc, minc, z));
            pts.Add(new Vector3(maxc, minc, z));
            pts.Add(new Vector3(maxc, maxc, z));
            pts.Add(new Vector3(minc, maxc, z));

            float min = minc;
            float max = maxc;
            float x;
            float y;

            Random rand = new Random();

            for (int i = 0; i < 100; i++)
            {
                x = (float)rand.NextDouble() * (max - min) + min;
                y = (float)rand.NextDouble() * (max - min) + min;
                pts.Add(new Vector3(x, y, z));
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < pts.Count; i++)
            {
                sb.AppendLine(string.Format("{0} {1} {2}", pts[i].X, pts[i].Y, pts[i].Z));
            }

            string filename = "Output\\xyz_10x10xz0.txt";

            File.WriteAllText(filename, sb.ToString());

            InputGeometry test = ReadTextXYZ.ReadTextXYZFile(filename);

            //See if we can get the convex hull for the proposed surface
            Behavior behavior = new Behavior();
            behavior.Convex = true;
            Mesh mesh = new Mesh(behavior);
            mesh.Triangulate(test);

            LocateResult findresult;
            Triangle foundTri = new Triangle();
            Vertex p1;
            Vertex p2;
            Vertex p3;
            double TotalVolume = 0.0;
            double TotalArea = 0.0;

            for (int n = 0; n < mesh.Triangles.Count; n++)
            {
                Triangle tri1 = mesh.Triangles.ElementAt(n);

                for (int m = 0; m < 3; m++)
                {
                    findresult = mesh.FindContainingTriangle(tri1.GetVertex(m), ref foundTri);
                    if (findresult == LocateResult.Outside)
                    {
                        Console.WriteLine("point outside mesh bounds");
                    }
                }
                List<Vertex> pts1 = tri1.GetVertices();
                double area = TriangleMathVector3.triArea(new Vector3((float)pts1[0].X, (float)pts1[0].Y, 0.0f),
                                                    new Vector3((float)pts1[1].X, (float)pts1[1].Y, 0.0f),
                                                    new Vector3((float)pts1[2].X, (float)pts1[2].Y, 0.0f));
                TotalVolume += area * 5.0;
                TotalArea += area;
            }

            //Find triangle in the mesh
            Triangle tri = new Triangle();
            findresult = mesh.FindContainingTriangle(new Point(0f, 0f, 20f), ref tri);


            FileWriter.WriteWaveFrontOBJFile(mesh, "Output\\xyz_10x10xz0.obj");
            List<Vertex> hull = mesh.GetConvexHull();
            sb.Clear();
            for (int i = 0; i < hull.Count; i++)
            {
                sb.AppendLine(string.Format("v {0} {1} {2}", hull[i].X, hull[i].Y, hull[i].Z));
            }
            for (int i = 0; i < hull.Count; i++)
            {
                sb.AppendLine(string.Format("l {0} {1} ", i + 1, i + 2));
            }

            File.WriteAllText("Output\\xyz_10x10xz0hull.obj", sb.ToString());
        }
    }
}
