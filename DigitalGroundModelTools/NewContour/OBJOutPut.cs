using System.Text;
using TriangleNet.Data;

namespace HeepWare.Contouring
{
    public class OBJOutPut
    {
        private static readonly bool debug = false;
        /// <summary>
        /// input is a string containing many obj contour string data
        /// that all start with 1
        /// </summary>
        /// <returns></returns>
        public static string MergeOBJContourData(string allcontours)
        {
            StringBuilder sb = new StringBuilder();
            int p1 = 0;
            int p2 = 0;
            int TotalVertexcount = 0;
            int vertexcount = 0;
            char previousLine = '\0';
            string[] lines = allcontours.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim().StartsWith("v"))
                {
                    //IF THE CURRENT line is a vertex line and the previous line is a line definition line
                    // update the total vertex count and reset the current vertex count
                    if (previousLine == 'l')
                    {
                        TotalVertexcount += vertexcount;
                        vertexcount = 0;
                    }
                    vertexcount++;
                    previousLine = 'v';
                }
                else if (lines[i].Trim().StartsWith("l"))
                {
                    string[] items = lines[i].Split(' ');
                    if(previousLine == 'v')
                    {
                        p1 = 1;p2 = 2;
                    }                    
                    
                    lines[i] = string.Format("l {0} {1} \n", p1+ TotalVertexcount, p2 + TotalVertexcount);
                    p1 = p2;
                    p2++;
                    previousLine = 'l';
                }
                sb.AppendLine(lines[i].Trim());
                //if(i == 33582)
                //{
                //    string t = "";
                //}
            }

            return sb.ToString();


        }
        public static String Triangles(List<Triangle> triList)
        {
            if (triList == null || triList.Count < 1) return "";
            StringBuilder sb = new StringBuilder("# test triangles in OBJ file format\n");
            int i = 0;
            int NumberOfTriangles = 0;

            NumberOfTriangles = triList.Count;

            int vertexcount = 0;

            for (i = 0; i < NumberOfTriangles; i += 3)
            {
                Triangle tri = triList[i];

                for (int j = 0; j < 3; j++)
                {
                    Vertex v = tri.GetVertex(j);
                    sb.AppendLine(string.Format("v {0} {1} {2}", v.X, v.Y, v.Z));
                    vertexcount++;
                }
            }
            sb.AppendLine();


            for (int l = 1; l <= vertexcount; l += 3)
            {
                sb.AppendLine(string.Format("l {0} {1} {2} {3}", l, (l + 1), (l + 2), l));
            }
            return sb.ToString();
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

                sbindexes.AppendLine(string.Format("l {0} {1}", index, index + 1));
                sbindexes.AppendLine(string.Format("l {0} {1}", index + 1, index + 2));
                sbindexes.AppendLine(string.Format("l {0} {1}", index + 2, index));
                index += 3;
            } // for 
            return sb.ToString() + sbindexes.ToString();
        }

         

        public static String ContourLineStrings(ContourV2 contour)
        {
            if (contour == null ) return "";
            StringBuilder sb = new StringBuilder("# test contour lines in OBJ file format\n\n");
            
            int pointcount = 0;
            //A contour can have many line strings
            for (int tt = 0; tt < contour.linestrings.Count; tt++)
            {
                LineString linestring = contour.linestrings[tt];
                for (int i = 0; i < linestring.Count; i++)
                {
                    sb.AppendLine(string.Format("v {0} {1} {2}", linestring[i].X, linestring[i].Y, linestring[i].Z));
                }
                sb.AppendLine();


                for (int l = 1; l < linestring.Count; l++)
                {
                    sb.AppendLine(string.Format("l {0} {1}", l + pointcount, (l + pointcount + 1)));
                }
                sb.AppendLine();
                pointcount = linestring.Count;
            }

            //no contour points found so return empty string
            if (sb.Length < "# test contour lines in OBJ file format\n".Length + 10)
            {
                sb.Clear();
            }
            return sb.ToString();
        }
    }
}
