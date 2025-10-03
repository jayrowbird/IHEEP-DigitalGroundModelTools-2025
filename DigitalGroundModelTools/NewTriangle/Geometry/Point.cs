// -----------------------------------------------------------------------
// <copyright file="Point.cs" company="">
// Triangle.NET code by Christian Woltering, http://triangle.codeplex.com/
// </copyright>
// -----------------------------------------------------------------------

namespace TriangleNet.Geometry
{
    using System;

    /// <summary>
    /// Represents a 2D or 2.5D point.
    /// </summary>
    public class Point : IComparable<Point>, IEquatable<Point>
    {
        // Change this if you require a different precision.
        private const int Tolerance = 100000;
        // Magic FNV values. Do not change these.
        private const long FNV32Init = 0x811c9dc5;
        private const long FNV32Prime = 0x01000193;

        private int hashcode = -99;

        internal int id;
        internal double x;
        internal double y;
        internal double z;
        internal int mark;
        internal double[] attributes;

        public Point()
            : this(0, 0, 0, 0)
        {
        }
        public Point(double x, double y, double z)
            : this(x, y, z, 0)
        {
        }
        public Point(double x, double y)
            : this(x, y, 0)
        {            
        }

        public Point(double x, double y, int mark)
        {
            this.x = x;
            this.y = y;
            this.mark = mark;
            GetHashCode();
        }
        public Point(double x, double y, double z, int mark)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.mark = mark;
            GetHashCode();
        }

        #region Public properties

        /// <summary>
        /// Gets the vertex id.
        /// </summary>
        public int ID
        {
            get { return this.id; }
        }

        /// <summary>
        /// Gets the vertex x coordinate.
        /// </summary>
        public double X
        {
            get { return this.x; }
        }

        /// <summary>
        /// Gets the vertex y coordinate.
        /// </summary>
        public double Y
        {
            get { return this.y; }
        }

        /// <summary>
        /// Gets the vertex y coordinate.
        /// </summary>
        public double Z
        {
            get { return this.z; }
        }

        /// <summary>
        /// Gets the vertex boundary mark.
        /// </summary>
        public int Boundary
        {
            get { return this.mark; }
        }

        /// <summary>
        /// Gets the vertex attributes (may be null).
        /// </summary>
        public double[] Attributes
        {
            get { return this.attributes; }
        }

        #endregion

        #region Operator overloading / overriding Equals

        // Compare "Guidelines for Overriding Equals() and Operator =="
        // http://msdn.microsoft.com/en-us/library/ms173147.aspx

        public static bool operator ==(Point a, Point b)
        {
            // If both are null, or both are same instance, return true.
            if (Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            Point p = obj as Point;

            if ((object)p == null)
            {
                return false;
            }

            return (x == p.x) && (y == p.y) && (z == p.z);
        }

        public bool Equals(Point p)
        {
            // If vertex is null return false.
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (x == p.x) && (y == p.y) && (z == p.z);
        }

        #endregion

        public int CompareTo(Point other)
        {
            if (x == other.x && y == other.y && z == other.z)
            {
                return 0;
            }

            return (x < other.x || (x == other.x && y < other.y && z < other.z)) ? -1 : 1;
        }

        public override int GetHashCode()
        {
            //try to improve performance by only computing hashcode once
            if (hashcode > 0) return hashcode;

            long rv = FNV32Init;
            rv ^= (long)(Math.Round(x * Tolerance));
            rv *= FNV32Prime;
            rv ^= (long)(Math.Round(y * Tolerance));
            rv *= FNV32Prime;
            rv ^= (long)(Math.Round(z * Tolerance));
            rv *= FNV32Prime;
            hashcode = rv.GetHashCode();

            return hashcode;
        }

        public override string ToString()
        {
            return String.Format("Point [{3} : {0},{1},{2}]", x, y, z, GetHashCode());
        }
    }
}
