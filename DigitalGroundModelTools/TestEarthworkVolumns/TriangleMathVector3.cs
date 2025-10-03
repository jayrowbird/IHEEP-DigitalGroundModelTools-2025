using OpenTK.Mathematics;

namespace TestEarthworkVolumes
{
    public class TriangleMathVector3
    {
        public const double NEARZERO = 0.00001;

        // Equation of a plane constants
        // private double A = 0.0; //Ax
        // private double B = 0.0; //By
        // private double C = 0.0; //CZ
        // private double D = 0.0; //D = A + B + C 

        public static Vector3 ComputeTriangleNormal(Vector3 p1, Vector3 p2, Vector3 p3, bool makeUnitVector)
        {
            Vector3 normalVector;
            //find first vector from point a to point p2
            Vector3 u = new Vector3(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            //find second vector from point p1 to point p3
            Vector3 v = new Vector3(p3.X - p1.X, p3.Y - p1.Y, p3.Z - p1.Z);
            // find the normal vector to the triangle by taking the cross product of vector u and v
            normalVector = Vector3.Cross(u, v);
            Console.WriteLine("Normal vector to this triangle = " + normalVector.ToString());

            //if makeUnitVector is true return a unit vector
            if (makeUnitVector)
            {
                Vector3.Normalize(normalVector);
            }
            return normalVector;
        }
         

        public static double triArea(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            double area2 = (p1.X - p3.X) * (p2.Y - p3.Y) - (p1.Y - p3.Y) * (p2.X - p3.X);
            if (area2 < 0.0)
            {
                area2 *= -1.0;
            }
            return area2 / 2.00;
        }

        // Twice the area of the triangle
        private static double area2(Vector3 pa, Vector3 pb, Vector3 pc)
        {
            return (pa.X - pc.X) * (pb.Y - pc.Y) - (pa.Y - pc.Y) * (pb.X - pc.X);
        }
        
        protected static Vector3 centroid3D(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 cg = Vector3.Zero;
            cg.X = p1.X + p2.X + p3.X;
            cg.Y = p1.Y + p2.Y + p3.Y;
            cg.Z = p1.Z + p2.Z + p3.Z;

            cg.X /= 3;
            cg.Y /= 3;
            cg.Z /= 3;

            return cg;
        }

        // test to see if triangle point are in COunter clockwise order
        protected static bool isCCW(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            bool result = true;
            if (area2(p1, p2, p3) < 0.0)
            {
                result = false;
            }
            return result;
        } 
         

        // Change point order to ensure the order is counterclockwise
        // p1 is index 0
        // p2 is index 1
        // p3 is index 3
        private static List<Vector3> makeCCW(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            List<Vector3> pts = new List<Vector3>();
            Vector3 temp = Vector3.Zero;
            if (area2(p1, p2, p3) < 0.0)
            {
                temp = p1;
                p1 = p2;
                p2 = p1;
            }
            pts.Add(p1); pts.Add(p2); pts.Add(p3);
            return pts;
        } 

        // Test if a Point is inside the triangle
        // triangle point are p1, p2, p3
        // p is the point tested for inside
        public static bool insideTriangle(Vector3 p, Vector3 p1, Vector3 p2, Vector3 p3) // abc is assumed to be counter-clockwise
        { // on the triangle or in the triangle
            if (isCCW(p1, p2, p3) == false)
            {   // swap a and p2 to make the triangle formed ccw
                List<Vector3> pts = makeCCW(p1, p2, p3);
                p1 = pts[0];
                p2 = pts[1];
                p3 = pts[2];
            }
            return (area2(p1, p2, p) >= -NEARZERO &&
                   area2(p2, p3, p) >= -NEARZERO &&
                   area2(p3, p1, p) >= -NEARZERO);
        } 

        private static double equationOfTrianglePlane(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            // Use the point Normal form to build the equation of the plane
            //Equation in the form of Ax + By + Cz + D = 0
            Vector3 normalVector = ComputeTriangleNormal(p1, p2, p3, true);


            // p1 is p1 point on the triangle which also makes it a point on the plane
            double D = (normalVector.X * p1.X + normalVector.Y * p1.Y + normalVector.Z * p1.Z);
            return D;
        }
 

        // What is the distance the given point from the plane
        // Triangle points are p1, p2, p3
        public static double point2Plane(Vector3 pt, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            double result = -1.0;
            Vector3 normalUnitVector;
            Vector3 normalVector = ComputeTriangleNormal(p1, p2, p3, false);
            double D = equationOfTrianglePlane(p1, p2, p3);

            //normalUnitVector = normalize(normalVector) ;
            //System.out.println("p1 =  "+normalVector.X+"  p2 =  "+normalVector.Y+"  p3 =  "+normalVector.Z+" D =  "+D+"\n") ; 
            //po is a point on the plane
            //A(x-po.X) + B(y-po.Y) + C(z-po.Z) = 0
            Console.WriteLine("normalVector.X*pt.X = " + normalVector.X * pt.X);
            Console.WriteLine("normalVector.Y*pt.Y = " + normalVector.Y * pt.Y);
            Console.WriteLine("normalVector.Z*pt.Z = " + normalVector.Z * pt.Z);
            Console.WriteLine("D = " + D);

            result = normalVector.X * (p1.X - pt.X) +
                     normalVector.Y * (p1.Y - pt.Y) +
                     normalVector.Z * (p1.Z - pt.Z);

            normalUnitVector = Vector3.Normalize(normalVector);
            Console.WriteLine("normalUnitVector = " + normalUnitVector);

            result = normalUnitVector.X * (p1.X - pt.X) +
                     normalUnitVector.Y * (p1.Y - pt.Y) +
                     normalUnitVector.Z * (p1.Z - pt.Z);

            //try this 
            //result = normalVector.X*(pt.X) + normalVector.Y*(pt.Y) + normalVector.Z*(pt.Z) + D ;
            //result = result / Math.sqrt(normalVector.X*normalVector.X + normalVector.Y*normalVector.Y + normalVector.Z*normalVector.Z) ;
            return result;
        }

        // Triangle points are p1, p2, p3
        public static double? elevation(Vector3 pt, Vector3 p1, Vector3 p2, Vector3 p3)
        {// compute the elevation of any point in or on the triangle 
            double? z = null;
            Vector3 normalVector = ComputeTriangleNormal(p1, p2, p3, false);
            double D = equationOfTrianglePlane(p1, p2, p3);

            if (insideTriangle(pt, p1, p2, p3) == true)
            {
                z = (D - normalVector.X * pt.X - normalVector.Y * pt.Y) / normalVector.Z;
            }
            else
            {
                Console.WriteLine("Point pt is not on or in triangle");
            }
            return z;
        }

        public static Vector3 ProjectPointOnTrianglePlane(Vector3 point, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            // Calculate the normal vector of the triangle plane
            Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1);
            normal = Vector3.Normalize(normal);

            // Calculate the projection of the point onto the plane
            float dot = Vector3.Dot(point - v1, normal);
            return point - dot * normal;
        }
    }
}
