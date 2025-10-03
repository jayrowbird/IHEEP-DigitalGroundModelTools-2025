using TriangleNet.Data;

namespace HeepWare.Contouring
{
    public class MathLib
    {
        public static readonly double NO_Z = 0.001010010001;
        public static readonly double NEARZERO = 0.0000001;
        internal static bool debug = false;

        public static Vertex InterpXY(Vertex pnt1, Vertex pnt2, double Z)
        {
            double alpha = 0.0;
            double x = 0;
            double y = 0;
            double z = 0;

            /* if pnt1.Z is larger than pnt2.Z */
            if (pnt1.Z >= pnt2.Z)
            {
                if (Math.Abs(pnt1.Z - pnt2.Z) < NEARZERO)
                {  /* High and low are equal */
                    Console.Write("Error: divide by zero problem in interpxy\n");
                    Console.Write("zhigh and zlow are equal \n");
                    return new Vertex(-1.0, -1.0, -1.0);
                }
                alpha = (Z - pnt2.Z) / (pnt1.Z - pnt2.Z);

                /* compute x */
                if (pnt1.X >= pnt2.X)
                {
                    /*   alpha  low     high  */
                    x = lerp(alpha, pnt2.X, pnt1.X);
                }
                else
                {                   /*   alpha  low     high  */
                    x = lerp(1.00 - alpha, pnt1.X, pnt2.X);
                }

                /* compute y */
                if (pnt1.Y >= pnt2.Y)
                {
                    /*   alpha  low     high  */
                    y = lerp(alpha, pnt2.Y, pnt1.Y);
                }
                else
                {                   /*   alpha  low     high  */
                    y = lerp(1.00 - alpha, pnt1.Y, pnt2.Y);
                }
            }
            else  /* pnt2.Z > pnt1.Z  these can not be equal from above test */
            {
                alpha = (Z - pnt1.Z) / (pnt2.Z - pnt1.Z);
                /* compute x */
                if (pnt2.X >= pnt1.X)
                {
                    /*   alpha  low     high  */
                    x = lerp(alpha, pnt1.X, pnt2.X);
                }
                else
                {                   /*   alpha  low     high  */
                    x = lerp(1.00 - alpha, pnt2.X, pnt1.X);
                }
                /* compute y */
                if (pnt2.Y >= pnt1.Y)
                {
                    /*   alpha  low     high  */
                    y = lerp(alpha, pnt1.Y, pnt2.Y);
                }
                else
                {                   /*   alpha  low     high  */
                    y = lerp(1.00 - alpha, pnt2.Y, pnt1.Y);
                }
            }
            z = Z;

            return new Vertex(x, y, z);
        }

        // sorted smallest to largest
        internal static Vertex[] SortTrianglePntsByZ(Triangle tri)
        {
            if (debug)
                Console.WriteLine("Contours:sortTrianglePntsByZ ");

            List<Vertex> vertices = tri.GetVertices();
            vertices = vertices.OrderBy(x => x.Z).ToList();
            return vertices.ToArray();
        }

        public static bool isBadVertex(Vertex pt)
        {
            bool result = false;

            if (debug)
                Console.WriteLine("Contours:isBadVertex ");

            if (pt.X == -1.0 && pt.Y == -1.0 && pt.Z == MathLib.NO_Z)
            {
                result = true;
            }
            return result;
        }

        internal static double lerp(double alpha, double low, double high)
        {
            return (low + ((high - low) * alpha));
        }

        protected double PntCalcDirection(double x1, double y1, double x2, double y2)
        {
            double angle_r = 0.0;
            double XDiff = 0.0;
            double YDiff = 0.0;

            XDiff = (x2 - x1);
            YDiff = (y2 - y1);

            if (XDiff != 0.0)
            {
                angle_r = Math.Atan((YDiff) / XDiff);
            }
            else if (YDiff > 0.0)
            {
                angle_r = Math.PI / 2.0;
            }
            else if (YDiff < 0.0)
            {
                angle_r = 3.0 * Math.PI / 2.0;
            }
            else
            {
                angle_r = 0.0;
                return (angle_r);
            }

            if (XDiff < 0.0)
                angle_r = angle_r + Math.PI;

            if (angle_r < 0.0)
                angle_r = angle_r + Math.PI * 2.0;

            return (angle_r);
        }
    }
}