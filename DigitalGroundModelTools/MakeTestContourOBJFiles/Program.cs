using System.Numerics;
using System.Text;
using TriangleNet;
using TriangleNet.Data;

namespace MakeTestContourOBJFiles
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Vector3> pnts = new List<Vector3>();
            Console.WriteLine("Hello, World!");
            Console.WriteLine("Done");
            string fileFullName = @".\Files\UstnJTestModel.txt";
            //fileFullName = @".\Files\J5P0100Points.txt";
            fileFullName = @".\Files\J5P0100Points.node";
            LoadNodeFile(fileFullName);

            Console.ReadKey();

            string[] lines = File.ReadAllLines(fileFullName);

            for (int i = 0; i < lines.Length; i++)
            {
                if (string.IsNullOrEmpty(lines[i].Trim()) || lines[i].Contains("#"))
                {
                    continue;
                }

                pnts.Add((Vector3)MakeVector3(lines[i]));
            }

            if (false) MakeStandardOBJPointsFile(pnts, fileFullName);
            if (false) Make10PntsPerLineOBJPointsFile(pnts, fileFullName);


            Console.ReadKey();
        }

        private static void MakeStandardOBJPointsFile(List<Vector3> pnts, string fileFullName)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < pnts.Count; i++)
            {
                sb.AppendFormat("v {0} {1} {2} \n", pnts[i].X, pnts[i].Y, pnts[i].Z);
            }
            for (int i = 0; i < pnts.Count; i++)
            {
                sb.AppendFormat("p {0}\n", i + 1);
            }

            string filename = Path.GetFileName(fileFullName);
            string ext = Path.GetExtension(filename);
            filename = filename.Replace(ext, "Points.obj");

            File.WriteAllText(filename, sb.ToString());
        }

        private static void Make10PntsPerLineOBJPointsFile(List<Vector3> pnts, string fileFullName)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < pnts.Count; i++)
            {
                sb.AppendFormat("v {0} {1} {2} \n", pnts[i].X, pnts[i].Y, pnts[i].Z);
            }
            for (int i = 0; i < pnts.Count; i++)
            {
                if (i % 10 == 0)
                {
                    sb.AppendLine();
                    sb.AppendFormat("p {0}", i + 1);
                }
                else
                {
                    sb.AppendFormat(" {0} ", 1 + i);
                }
            }

            string filename = Path.GetFileName(fileFullName);
            string ext = Path.GetExtension(filename);
            filename = filename.Replace(ext, "10PointsPerline.obj");

            File.WriteAllText(filename, sb.ToString());
        }

        private static void Make10PntsPerLineOBJTrianglesFile(List<Vector3> pnts, string fileFullName)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < pnts.Count; i++)
            {
                sb.AppendFormat("v {0} {1} {2} \n", pnts[i].X, pnts[i].Y, pnts[i].Z);
            }
            for (int i = 0; i < pnts.Count; i++)
            {
                if (i % 10 == 0)
                {
                    sb.AppendLine();
                    sb.AppendFormat("p {0}", i + 1);
                }
                else
                {
                    sb.AppendFormat(" {0} ", 1 + i);
                }
            }

            string filename = Path.GetFileName(fileFullName);
            string ext = Path.GetExtension(filename);
            filename = filename.Replace(ext, "10PointsPerline.obj");

            File.WriteAllText(filename, sb.ToString());
        }

        private static void LoadNodeFile(string fileFullname)
        {
            string filename = Path.GetFileName(fileFullname);
            string ext = Path.GetExtension(filename);

            if (ext != ".node")
            {
                throw new Exception(string.Format("File must be a node file {0}", fileFullname));
            }

            Mesh mesh = new Mesh();
            mesh.Triangulate(fileFullname);

            String trianglestxt = TriangleShapes(mesh.Triangles.ToList());
            filename = filename.Replace(ext, "3TrianglesPerline.obj");
            File.WriteAllText(filename, trianglestxt);

            String text = TriangleOutline(mesh.Triangles.ToList());
              filename = Path.GetFileName(fileFullname);
            filename = filename.Replace(ext, "Triangles6Outline.obj");
            File.WriteAllText(filename, text);
        }

        public static String TriangleShapes(List<Triangle> triList)
        {
            if (triList == null || triList.Count < 1) return "";
            StringBuilder sb = new StringBuilder("#Mesh triangles in wavefront format\n");
            StringBuilder sbindexes = new StringBuilder();

            int NumberOfTriangles = triList.Count;

            int index = 1;

            for (int i = 0; i < NumberOfTriangles; i++)
            {
                Triangle tri = triList[i];
                /* if triangle is part of the bounding */
                /*
                if (tri.p1.z == Point3D.NO_Z || 
                    tri.p2.z == Point3D.NO_Z || 
                    tri.p3.z == Point3D.NO_Z)
                {
                   / * this triangle is part of the bounding triangle
                      and ther for should not be drawn
                   * /
                   //System.out.print("Triangle index with a z == Point3D.NO_Z ="+i+"\n");
                   //System.out.print("tri.p1 = \n");
                   //tri.p1.print();
                   //System.out.print("tri.p2 = \n");
                   //tri.p2.print();
                   //System.out.print("tri.p3 = \n");
                   //tri.p3.print();
                   continue ;
                }
                */

                sb.Append("v " + tri.GetVertex(0).X + " " + tri.GetVertex(0).Y + " " + tri.GetVertex(0).Z + "\n");
                sb.Append("v " + tri.GetVertex(1).X + " " + tri.GetVertex(1).Y + " " + tri.GetVertex(1).Z + "\n");
                sb.Append("v " + tri.GetVertex(2).X + " " + tri.GetVertex(2).Y + " " + tri.GetVertex(2).Z + "\n");

                sbindexes.AppendLine(string.Format("f {0} {1} {2}", index, index + 1, index + 2));

                index += 3;
            } // for 
            return sb.ToString() + sbindexes.ToString();
        }

        public static String TriangleOutline(List<Triangle> triList)
        {
            if (triList == null || triList.Count < 1) return "";
            StringBuilder sb = new StringBuilder("#Mesh triangles in wavefront format\n");
            StringBuilder sbindexes = new StringBuilder();

            int NumberOfTriangles = triList.Count;

            int index = 1;

            for (int i = 0; i < NumberOfTriangles; i++)
            {
                Triangle tri = triList[i];
                /* if triangle is part of the bounding */
                /*
                if (tri.p1.z == Point3D.NO_Z || 
                    tri.p2.z == Point3D.NO_Z || 
                    tri.p3.z == Point3D.NO_Z)
                {
                   / * this triangle is part of the bounding triangle
                      and ther for should not be drawn
                   * /
                   //System.out.print("Triangle index with a z == Point3D.NO_Z ="+i+"\n");
                   //System.out.print("tri.p1 = \n");
                   //tri.p1.print();
                   //System.out.print("tri.p2 = \n");
                   //tri.p2.print();
                   //System.out.print("tri.p3 = \n");
                   //tri.p3.print();
                   continue ;
                }
                */

                sb.Append("v " + tri.GetVertex(0).X + " " + tri.GetVertex(0).Y + " " + tri.GetVertex(0).Z + "\n");
                sb.Append("v " + tri.GetVertex(1).X + " " + tri.GetVertex(1).Y + " " + tri.GetVertex(1).Z + "\n");
                sb.Append("v " + tri.GetVertex(2).X + " " + tri.GetVertex(2).Y + " " + tri.GetVertex(2).Z + "\n");

                sbindexes.Append (string.Format("l {0} {1}", index, index + 1));
                sbindexes.Append (string.Format(" {0} {1}", index + 1, index + 2));
                sbindexes.AppendLine(string.Format(" {0} {1}", index + 2, index));
                index += 3;
            } // for 
            return sb.ToString() + sbindexes.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static Vector3? MakeVector3(string line)
        {
            string[] items = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length == 3)
            {
                return new Vector3(float.Parse(items[0]), float.Parse(items[1]), float.Parse(items[2]));
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static Vector2? MakeVector2(string line)
        {
            string[] items = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length == 3)
            {
                return new Vector2(float.Parse(items[1]), float.Parse(items[2]));
            }
            return null;
        }
    }
}
