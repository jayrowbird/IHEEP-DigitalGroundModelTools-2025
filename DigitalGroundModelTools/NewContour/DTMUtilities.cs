using System.Text;
using TriangleNet.Data;

namespace HeepWare.Contouring
{
    public class Output
    {
        private static bool debug = false;

        public Output()
        {
            ;
        }

        public static double toDegrees(double value)
        {
            return (180.00 / Math.PI) * value;
        } 

        // LandXML file format for surfaces the point list portion
        public static String writeXMLPoints(List<Vertex> pntList)
        {   //<P id="1">1524.000000 1524.000000 188.6986</P>
            StringBuilder sb = new StringBuilder("<Pnts>\r\n");

            Vertex p = null;
            for (int i = 0; i < pntList.Count; i++)
            {
                //filter out the triangles that include the containing traingle
                //point index values for the containing triangle are -100 -200 -300
                p = pntList[i];
                if (p != null)
                {
                    sb.Append("\t<P id=\"" + (i + 1) + "\">" + p.Y + "  " + p.X + "  " + p.Z + " </P>\r\n");
                }
            }
            sb.Append("</Pnts>\r\n");
            return sb.ToString();
        }

        // LandXML file format for surfaces the triangle faces portion
        public static String writeXMLFaces(List<Triangle> ltriList)
        {   //<F>53 47 48</F>
            StringBuilder sb = new StringBuilder("<Faces>\r\n");

            Triangle tri = null;
            for (int i = 0; i < ltriList.Count; i++)
            {
                //filter out the triangles that include the containing traingle
                //point index values for the containing triangle are -100 -200 -300 

                tri = ltriList[i];
                if (tri != null && tri.GetVertex(0) != null && tri.GetVertex(1) != null && tri.GetVertex(2) != null)
                {
                    sb.Append("\t<F> " + (tri.GetVertex(0).ID) + "  " + (tri.GetVertex(1).ID) + "  " + (tri.GetVertex(2).ID) + " </F>\r\n");
                }
            }
            sb.Append("</Faces>\r\n");
            return sb.ToString();
        } 

        //---------------------------------------------------------------------------
        public static String writeOBJTriangleShapes(List<Triangle> triList)
        {
            return OBJOutPut.TriangleShapes(triList);
        }
        public static String writeOBJContourLineStrings(ContourV2 contour)
        {
            return OBJOutPut.ContourLineStrings(contour);
        }
        public static String writeOBJTriangles(List<Triangle> triList)
        {
            return OBJOutPut.Triangles(triList);
        } 
    }
}