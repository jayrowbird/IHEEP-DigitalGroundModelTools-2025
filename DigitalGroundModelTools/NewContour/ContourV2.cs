using System.Text;
using TriangleNet.Data;
using static HeepWare.Contouring.BoundBox3D;

namespace HeepWare.Contouring
{
    public class ContourV2 //implements Comparator
    {
        private readonly bool debug = false;
        // the elevation for this contour 

        private ContourID contourId;
        internal double elevation;
        internal BoundingBox3D? range = null;

        internal List<LineString> linestrings = new List<LineString>();

        internal int colorindex;

        //list of labels for this contour
        //internal List<ContourLabel>  contourLabels= null;

        // distance between contour labels
        private double labelDistance = 50;

        // debug variables below

        internal bool logFlag = false;
        // protected LogFile logFile = null;

        // Contour constructor
        public ContourV2(ContourID id)
        {
            contourId = id;
            elevation = contourId.elev;
        }


        //public bool IsMajorContour()
        //{
        //    return (elevation % contours.majorInterval == 0);
        //}

        public int GetColorIndex()
        {
            return colorindex;
        }

        public double getElevation()
        {
            return elevation;
        }

        public void setLogFileFlag()
        {
            logFlag = true;
        }

        public void setLabelDistance(double distance)
        {
            labelDistance = distance;
        }

        public double getLabelDistance()
        {
            return labelDistance;
        }

        public void SetLineStrings(List<LineString> lsrts)
        {
            if (lsrts != null)
            {
                linestrings.AddRange(lsrts);
                range = ComputeBoundingBox();
            }
            return;
        }

        public List<LineString> GetLineStrings()
        {
            return linestrings;
        }

        private BoundingBox3D ComputeBoundingBox()
        {
            BoundingBox3D bbox = new BoundingBox3D();
            if (linestrings != null && linestrings.Count > 0)
            {

                Vertex tmp;
                for (int p = 0; p < linestrings.Count; p++)
                {
                    LineString lineString = linestrings[p];
                    for (int m = 0; m < lineString.Count; m++)
                    {
                        tmp = lineString[m];
                        bbox.MinX = Math.Min(bbox.MinX, tmp.X);
                        bbox.MinY = Math.Min(bbox.MinY, tmp.Y);
                        bbox.MinZ = Math.Min(bbox.MinZ, tmp.Z);

                        bbox.MaxX = Math.Max(bbox.MaxX, tmp.X);
                        bbox.MaxY = Math.Max(bbox.MaxY, tmp.Y);
                        bbox.MaxZ = Math.Max(bbox.MaxZ, tmp.Z);
                    }
                }
            }
            return bbox; 
        } 
        internal bool makeLabels(double labelDistance)
        {
            if (debug) Console.WriteLine("Contour:makeLabels:Entered:Elevation : " + elevation);
            bool result = false;
             
            if (debug) Console.WriteLine("Contour:makeLabels:Exicted ");
            return result;
        }
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("Contour: " + elevation + "\n");
            sb.Append("BoundingBox = " + range + "\n");
            sb.Append("Line strings store = " + linestrings.Count + "\n");

            return sb.ToString();
        }
    }
}

