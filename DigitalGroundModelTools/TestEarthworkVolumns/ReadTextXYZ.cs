using OpenTK.Mathematics;
using System.Globalization;
using TriangleNet.Geometry;

namespace NewTriangle.IO
{
    internal class ReadTextXYZ
    {
        public ReadTextXYZ() { }

        static NumberFormatInfo nfi = CultureInfo.InvariantCulture.NumberFormat;
        static int startIndex = 0;

        

        static bool TryReadLine(StreamReader reader, out string[] token)
        {
            token = null;

            if (reader.EndOfStream)
            {
                return false;
            }

            string line = reader.ReadLine().Trim();

            while (String.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
            {
                if (reader.EndOfStream)
                {
                    return false;
                }

                line = reader.ReadLine().Trim();
            }

            token = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            return true;
        }


        static Vector3 ReadVertex3D(string[] line)
        {
            //int parseCount = 3;
            float x = float.Parse(line[0], nfi);
            float y = float.Parse(line[1], nfi);
            float z = float.Parse(line[2], nfi);

            return new Vector3(x, y, z);
        }



        /// <summary>
        /// Read the vertices's from a file, which may be a .node or .poly file.
        /// </summary>
        /// <param name="nodefilename"></param>
        /// <param name="readElements"></param>
        public static InputGeometry ReadTextXYZFile(string xyzfilename )
        {
            InputGeometry data;
            //bool is3D = false;
            //startIndex = 0;

            string[] line;
            List<Vector3> pts = new List<Vector3>();

            using (StreamReader reader = new StreamReader(xyzfilename))
            { 
                while (TryReadLine(reader, out line))
                {
                    Vector3? pt = ReadVertex3D(line);
                    if (pt != null)
                    {
                        pts.Add((Vector3)pt );
                    } 
                } 
            }
            data = new InputGeometry(pts.Count);
            for (int i = 0; i < pts.Count; i++) 
            {
                data.AddPoint(pts[i].X, pts[i].Y, pts[i].Z);
            }
            return data;
        }
    }
}
