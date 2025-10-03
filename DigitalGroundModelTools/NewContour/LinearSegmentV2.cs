using System.Text;
using TriangleNet.Data;
using TriangleNet.Geometry;

namespace HeepWare.Contouring
{
    public class LinearSegmentV2
    {
        internal BoundingBox range = null;
        internal Vertex p1;
        internal Vertex p2;
        internal Guid contourId;
        
        //internal bool debug = false;

        public LinearSegmentV2(Vertex ipb, Vertex ipe, Guid id)
        {
            contourId = id;
            //add the segment ordered by x coordinate
            if(ipb.GetHashCode() == ipe.GetHashCode())
            {
                //something is not correct
                string t = "";
            }
            if (ipb.Z <= ipe.Z) 
            { 
                p1 = ipb;
                p2 = ipe;
            }
            else
            {
                p1 = ipe;
                p2 = ipb;
            }
        }

        public bool reverse()
        {
            bool result = true;
            Vertex temp =  p1;
             p1 =  p2;
             p2 = temp;
            return result; 
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("LinearSegment: ");
            sb.Append("[ p1 :  " +  p1 + "  p2 :  " +  p2 + "]");
            return sb.ToString();
        }
    }
}
