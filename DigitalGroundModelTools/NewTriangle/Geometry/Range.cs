using System.Text;
using System;


//import java.io.* ;
//import java.awt.* ;
//import java.util.ArrayList ;

/*
      Copy right S C Brown Nebraska
      Year 2002 
      month October       
*/
namespace TriangleNet.Geometry
{
    public class Range
    {
        internal double xHigh;
        internal double yHigh;
        internal double xLow;
        internal double yLow;

        internal double zHigh;
        internal double zLow;
        internal long elementId = 0;

        public Range(Point pt1, Point pt2)
        {
            // initialize the class variables
            init();
            update(pt1.x, pt1.y, pt1.z);
            update(pt2.x, pt2.y, pt2.z);
        }

        public Range(Point pt1)
        {
            xHigh = pt1.x; xLow = pt1.x;
            yHigh = pt1.y; yLow = pt1.y;
            zHigh = pt1.z; zLow = pt1.z;
        }

        public Range(Point pt1, Point pt2, Point pt3)
        {
            // initialize the class variables
            init();
            update(pt1.x, pt1.y, pt1.z);
            update(pt2.x, pt2.y, pt2.z);
            update(pt3.x, pt3.y, pt3.z);
        }

        public Range()
        {
            init();
        }

        private void init()
        {
            xHigh = -999999999.0;
            xLow = 999999999.0;
            yHigh = -999999999.0;
            yLow = 999999999.0;
            zHigh = -999999999.0;
            zLow = 999999999.0;
        }

        public Range(double x, double y)
        {
            xHigh = x; xLow = x;
            yHigh = y; yLow = y;
            zHigh = -999999999.0;
            zLow = 999999999.0;
        }

        public Range(double x, double y, double z)
        {
            xHigh = x; xLow = x;
            yHigh = y; yLow = y;
            zHigh = z; zLow = z;
        }

        public Range(double x1, double y1, double x2, double y2)
        {
            init();
            update(x1, y1);
            update(x2, y2);
        }

        public Range(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            init();
            update(x1, y1, z1);
            update(x2, y2, z2);
        }


        public double getxHigh()
        {
            return xHigh;
        }

        public double getxLow()
        {
            return xLow;
        }

        public double getyHigh()
        {
            return yHigh;
        }

        public double getyLow()
        {
            return yLow;
        }

        public double getzHigh()
        {
            return zHigh;
        }

        public double getzLow()
        {
            return zLow;
        }

        public Point getCentroid2D()
        {
            Point p = new Point((xHigh + xLow) / 2.0, (yHigh + yLow) / 2.0);
            return p;
        }

        public Point getCentroid3D()
        {
            Point p = new Point((xHigh + xLow) / 2.0, (yHigh + yLow) / 2.0, (zHigh + zLow) / 2.0);
            return p;
        }

        public Boolean doesIntersect2D(Range nr)
        {
            Boolean result = true;

            if (this.yHigh < nr.yLow ||
                this.yLow > nr.yHigh ||
                this.xLow > nr.xHigh ||
                this.xHigh < nr.xLow)
            {
                result = false;
            }

            return result;
        }

        public Boolean doesIntersect3D(Range nr)
        {
            Boolean result = false;
            if (doesIntersect2D(nr) == true)
            {
                if (this.zHigh < nr.zLow ||
                    this.zLow > nr.zHigh)
                {
                    result = false;
                }
            }
            else
            {
                result = true;
            }
            return result;
        }

        public Boolean isPointInRange2D(Point pt)
        {
            Boolean result = false;
            if (pt.x >= xLow && pt.x <= xHigh)
            {
                if (pt.y >= yLow && pt.y <= yHigh)
                {
                    result = true;
                }
            }
            return result;
        }




        public Boolean isPointInRange3D(Point pt)
        {
            Boolean result = false;
            if (isPointInRange2D(pt) == true)
            {
                if (pt.z >= zLow && pt.z <= zHigh)
                {
                    result = true;
                }
            }

            return result;
        }

        public void print()
        {
            Console.WriteLine(ToString());
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("Range high:");
            sb.Append(" X " + xHigh);
            sb.Append(" Y " + yHigh);
            sb.Append(" Z " + zHigh);

            sb.Append(" ,Range low: ");
            sb.Append(" X " + xLow);
            sb.Append(" Y " + yLow);
            sb.Append(",Z " + zLow);
            sb.Append("\n");

            return sb.ToString();
        }

        private void update_x(double x)
        {
            if (x > xHigh)
            {
                xHigh = x;
            }

            if (x < xLow)
            {
                xLow = x;
            }
        }

        private void update_y(double y)
        {
            if (y > yHigh)
            {
                yHigh = y;
            }

            if (y < yLow)
            {
                yLow = y;
            }
        }

        private void update_z(double z)
        {
            if (z > zHigh)
            {
                zHigh = z;
            }

            if (z < zLow)
            {
                zLow = z;
            }
        }

        public void update(double x1, double y1, double x2, double y2)
        {
            update_x(x1);
            update_x(x2);
            update_y(y1);
            update_y(y2);
        }

        public void update(double x, double y)
        {
            update_x(x);
            update_y(y);
        }

        public void update(double x, double y, double z)
        {
            update_x(x);
            update_y(y);
            update_z(z);
        }

        public void update(Point pt)
        {
            update_x(pt.x);
            update_y(pt.y);
            update_z(pt.z);  // remember z is checked if it equals Point.NO_Z in update_z
        }


        public void update(Range nr)
        {
            update_x(nr.xHigh);
            update_x(nr.xLow);
            update_y(nr.yHigh);
            update_y(nr.yLow);
            update_z(nr.zHigh);
            update_z(nr.zLow);
        }

        public static void main(String[] args)
        {
            Range test = null;

            Console.WriteLine("\nTest range: constructor  Range() ;");
            test = new Range();
            test.print();

            Console.WriteLine("\nTest range: constructor  Range(1.0, 2.0) ;");
            test = new Range(1.0, 2.0);
            test.print();

            Console.WriteLine("\nTest range: constructor  Range(1.0, 20.0, 300.0) ;");
            test = new Range(1.0, 20.0, 300.0);
            test.print();

            Console.WriteLine("\nTest range: constructor  Range(1.0, 200.0, 3.0, 600.0) ;");
            test = new Range(1.0, 200.0, 3.0, 600.0);
            test.print();

            Console.WriteLine("\nTest range: constructor  Range(1.0, 20.0, 9000.0, 3.0, 66.0, 8340.0) ;");
            test = new Range(1.0, 20.0, 9000.0, 3.0, 66.0, 8340.0);
            test.print();

            Console.WriteLine("\nTest range: constructor  Range(new Point(1.0, 20.0)) ;");
            test = new Range(new Point(1.0, 20.0));
            test.print();

            Console.WriteLine("\nTest range: constructor  Range(new Point(1.0, 20.0, 500.0)) ;");
            test = new Range(new Point(1.0, 20.0, 500.0));
            test.print();

            Console.WriteLine("\nTest range: constructor  Range(new Point(1.0, 2.0), new Point()) ;");
            test = new Range(new Point(1.0, 2.0), new Point());
            test.print();

            Console.WriteLine("\nTest range: constructor  Range(new Point(1.0, 2.0), new Point()) ;");
            test = new Range(new Point(1.0, 2.0, -1200.00), new Point());
            test.print();
        }
    }
}

