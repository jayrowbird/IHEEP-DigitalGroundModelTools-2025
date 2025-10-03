using System.Diagnostics.Contracts;
using TriangleNet.Data;

namespace TestEarthworkVolumes
{
    public class TriangleMathVertex
    {
        public const double NEARZERO = 0.00001;

        /// <summary>
        /// Calculate the cross (vector) product of two vectors.
        /// </summary>
        /// <param name="left">First operand.</param>
        /// <param name="right">Second operand.</param>
        /// <returns>The cross product of the two inputs.</returns>
        [Pure]
        public static Vertex Cross(Vertex left, Vertex right)
        {
            Cross(in left, in right, out Vertex result);
            return result;
        }

        /// <summary>
        /// Calculate the cross (vector) product of two vectors.
        /// </summary>
        /// <param name="left">First operand.</param>
        /// <param name="right">Second operand.</param>
        /// <param name="result">The cross product of the two inputs.</param>
        public static void Cross(in Vertex left, in Vertex right, out Vertex result)
        {
            double X = (left.Y * right.Z) - (left.Z * right.Y);
            double Y = (left.Z * right.X) - (left.X * right.Z);
            double Z = (left.X * right.Y) - (left.Y * right.X);

            result = new Vertex(X, Y, Z);
        }


        /// <summary>
        /// Calculate the dot (scalar) product of two vectors.
        /// </summary>
        /// <param name="left">First operand.</param>
        /// <param name="right">Second operand.</param>
        /// <returns>The dot product of the two inputs.</returns>
        [Pure]
        public static double Dot(Vertex left, Vertex right)
        {
            return (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z);
        }

        /// <summary>
        /// Calculate the dot (scalar) product of two vectors.
        /// </summary>
        /// <param name="left">First operand.</param>
        /// <param name="right">Second operand.</param>
        /// <param name="result">The dot product of the two inputs.</param>
        public static void Dot(in Vertex left, in Vertex right, out double result)
        {
            result = (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z);
        }

        /// <summary>
        /// Subtract one Vector from another.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <returns>Result of subtraction.</returns>
        [Pure]
        public static Vertex Subtract(Vertex a, Vertex b)
        {
            Subtract(in a, in b, out a);
            return a;
        }

        /// <summary>
        /// Subtract one Vector from another.
        /// </summary>
        /// <param name="a">First operand.</param>
        /// <param name="b">Second operand.</param>
        /// <param name="result">Result of subtraction.</param>
        public static void Subtract(in Vertex a, in Vertex b, out Vertex result)
        {
            double X = a.X - b.X;
            double Y = a.Y - b.Y;
            double Z = a.Z - b.Z;
            result = new Vertex(X, Y, Z);
            return;
        }

        /// <summary>
        /// Multiplies a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <returns>Result of the operation.</returns>
        [Pure]
        public static Vertex Multiply(Vertex vector, double scale)
        {
            Multiply(in vector, scale, out vector);
            return vector;
        }

        /// <summary>
        /// Multiplies a vector by a scalar.
        /// </summary>
        /// <param name="vector">Left operand.</param>
        /// <param name="scale">Right operand.</param>
        /// <param name="result">Result of the operation.</param>
        public static void Multiply(in Vertex vector, double scale, out Vertex result)
        {
            double X = vector.X * scale;
            double Y = vector.Y * scale;
            double Z = vector.Z * scale;

              result = new Vertex(X, Y, Z);
        }

        /// <summary>
        /// Scales the Vector3 to unit length.
        /// </summary>
        public static Vertex Normalize(Vertex v)
        {
            double Length = Math.Sqrt((v.X * v.X) + (v.Y * v.Y) + (v.Z * v.Z));
            double scale = 1.0f / Length;
            double x = v.X * scale;
            double y = v.Y * scale;
            double z = v.Z * scale;

            return new Vertex(x, y, z);
        }
        // Equation of a plane constants
        // private double A = 0.0; //Ax
        // private double B = 0.0; //By
        // private double C = 0.0; //CZ
        // private double D = 0.0; //D = A + B + C 
        public static Vertex ComputeTriangleNormal(Vertex p1, Vertex p2, Vertex p3, bool makeUnitVector)
        {
            Vertex normalVector;
            //find first vector from point a to point p2
            Vertex u = new Vertex(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            //find second vector from point p1 to point p3
            Vertex v = new Vertex(p3.X - p1.X, p3.Y - p1.Y, p3.Z - p1.Z);
            // find the normal vector to the triangle by taking the cross product of vector u and v
            normalVector = Cross(u, v);
            Console.WriteLine("Normal vector to this triangle = " + normalVector.ToString());

            //if makeUnitVector is true return a unit vector
            if (makeUnitVector)
            {
                normalVector = Normalize(normalVector);
            }
            return normalVector;
        }

        public static double triArea(Vertex p1, Vertex p2, Vertex p3)
        {
            double area2 = (p1.X - p3.X) * (p2.Y - p3.Y) - (p1.Y - p3.Y) * (p2.X - p3.X);
            if (area2 < 0.0)
            {
                area2 *= -1.0;
            }
            return area2 / 2.00;
        }


        // Twice the area of the triangle
        private static double area2(Vertex pa, Vertex pb, Vertex pc)
        {
            return (pa.X - pc.X) * (pb.Y - pc.Y) - (pa.Y - pc.Y) * (pb.X - pc.X);
        }


        protected static Vertex centroid3D(Vertex p1, Vertex p2, Vertex p3)
        {
            double X = p1.X + p2.X + p3.X;
            double Y = p1.Y + p2.Y + p3.Y;
            double Z = p1.Z + p2.Z + p3.Z;

            X /= 3;
            Y /= 3;
            Z /= 3;

            return new Vertex(X, Y, Z);
        }

        // test to see if triangle point are in COunter clockwise order
        protected static bool isCCW(Vertex p1, Vertex p2, Vertex p3)
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
        private static List<Vertex> makeCCW(Vertex p1, Vertex p2, Vertex p3)
        {
            List<Vertex> pts = new List<Vertex>();
            Vertex temp;
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
        public static bool insideTriangle(Vertex p, Vertex p1, Vertex p2, Vertex p3) // abc is assumed to be counter-clockwise
        { // on the triangle or in the triangle
            if (isCCW(p1, p2, p3) == false)
            {   // swap a and p2 to make the triangle formed ccw
                List<Vertex> pts = makeCCW(p1, p2, p3);
                p1 = pts[0];
                p2 = pts[1];
                p3 = pts[2];
            }
            return (area2(p1, p2, p) >= -NEARZERO &&
                   area2(p2, p3, p) >= -NEARZERO &&
                   area2(p3, p1, p) >= -NEARZERO);
        }



        private static double equationOfTrianglePlane(Vertex p1, Vertex p2, Vertex p3)
        {
            // Use the point Normal form to build the equation of the plane
            //Equation in the form of Ax + By + Cz + D = 0
            Vertex normalVector = ComputeTriangleNormal(p1, p2, p3, true);


            // p1 is p1 point on the triangle which also makes it a point on the plane
            double D = (normalVector.X * p1.X + normalVector.Y * p1.Y + normalVector.Z * p1.Z);
            return D;
        }

        // What is the distance the given point from the plane
        // Triangle points are p1, p2, p3
        public static double point2Plane(Vertex pt, Vertex p1, Vertex p2, Vertex p3)
        {
            double result = -1.0;
            Vertex normalUnitVector;
            Vertex normalVector = ComputeTriangleNormal(p1, p2, p3, false);
            double D = equationOfTrianglePlane(p1, p2, p3);

            //normalUnitVector = normalize(normalVector) ;
            //System.out.println("p1 =  "+normalVector.X+"  p2 =  "+normalVector.Y+"  p3 =  "+normalVector.Z+" D =  "+D+"\n") ; 
            //p0 is a point on the plane
            //A(x-po.X) + B(y-po.Y) + C(z-po.Z) = 0
            Console.WriteLine("normalVector.X*pt.X = " + normalVector.X * pt.X);
            Console.WriteLine("normalVector.Y*pt.Y = " + normalVector.Y * pt.Y);
            Console.WriteLine("normalVector.Z*pt.Z = " + normalVector.Z * pt.Z);
            Console.WriteLine("D = " + D);

            result = normalVector.X * (p1.X - pt.X) +
                     normalVector.Y * (p1.Y - pt.Y) +
                     normalVector.Z * (p1.Z - pt.Z);

            normalUnitVector = Normalize(normalVector);
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
        public static double? elevation(Vertex pt, Vertex p1, Vertex p2, Vertex p3)
        {// compute the elevation of any point in or on the triangle 
            double? z = null;
            Vertex normalVector = ComputeTriangleNormal(p1, p2, p3, false);
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

        public static Vertex ProjectPointOnTrianglePlane(Vertex point, Vertex v1, Vertex v2, Vertex v3)
        {
            // Calculate the normal vector of the triangle plane
            Vertex normal = Cross(Subtract(v2 , v1), Subtract( v3 , v1));
            normal = Normalize(normal);

            // Calculate the projection of the point onto the plane
            double dot = Dot(Subtract(point , v1), normal);
            return Subtract(point , Multiply(normal , dot));
        }
    }
}
