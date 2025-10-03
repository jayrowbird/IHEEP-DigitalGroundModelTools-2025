using TriangleNet.Data;

namespace HeepWare.Contouring 
{
    internal class BoundBox3D
    {
        public struct BoundingBox3D
        {
            internal double MinX { get; set; } = float.MaxValue;
            internal double MinY { get; set; } = float.MaxValue;
            internal double MinZ { get; set; } = float.MaxValue;
            internal double MaxX { get; set; } = float.MinValue;
            internal double MaxY { get; set; } = float.MinValue;
            internal double MaxZ { get; set; } = float.MinValue;

            public BoundingBox3D()
            {

            }

            public BoundingBox3D(double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
            {
                MinX = minX;
                MinY = minY;
                MinZ = minZ;
                MaxX = maxX;
                MaxY = maxY;
                MaxZ = maxZ;
            }

            public BoundingBox3D(Vertex min, Vertex max)
            {
                MinX = min.X;
                MinY = min.Y;
                MinZ = min.Z;
                MaxX = max.X;
                MaxY = max.Y;
                MaxZ = max.Z;
            }

            // Example: Check for intersection with another BoundingBox2D
            public bool Intersects(BoundingBox3D other)
            {
                return MaxX >= other.MinX && MinX <= other.MaxX && 
                       MaxY >= other.MinY && MinY <= other.MaxY && 
                       MaxZ >= other.MinZ && MinZ >= other.MaxZ ;
            }
        }
    }
}
