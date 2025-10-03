using OpenTK.Mathematics;
using TriangleNet;
using TriangleNet.Data;

namespace TestEarthworkVolumes
{
    public class Volumes
    {
        public Volumes() { }
        public double aboveGround;
        public double belowGround;
    }

    public class Surface2SurfaceVolume
    {
        Mesh mesh_undisturbedSurface;
        Mesh mesh_proposedSurface;
        public  Surface2SurfaceVolume(Mesh existing, Mesh proposed)
        {
            mesh_undisturbedSurface = existing;
            mesh_proposedSurface = proposed;
        }

        public Volumes ComputeVolumes()
        {
            Volumes volumes = new Volumes();
            //compute the boundary for the proposed surface mesh
            //add points that lie within the boundary to each surface
            //this create identical triangles within the boundary for both surfaces
            //compute the volume of each triangular prism 
            //compute the flat area of the triangle * the average of the three heights





            return volumes;
        }
 
        public double CalculateTriangleArea(Vector3 pointA, Vector3 pointB, Vector3 pointC)
        {
            Vector3 side1 = pointB - pointA;
            Vector3 side2 = pointC - pointA;
            Vector3 crossProduct = Vector3.Cross(side1, side2);
            return 0.5f * crossProduct.Length;
        }

        public   Vector3 ProjectPointOntoTriangle(Vector3 point, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            // Calculate the normal vector of the triangle
            Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1);
            normal = Vector3.Normalize(normal);

            // Calculate the vector from a vertex of the triangle to the point
            Vector3 vectorToP = point - v1;

            // Project the vector onto the normal vector
            float projectionLength = Vector3.Dot(vectorToP, normal);
            Vector3 projectionVector = projectionLength * normal;

            // Subtract the projection to get the projected point on the triangle plane
            return point - projectionVector;
        }

        public List<Vertex> MeshGetConvexHull(Mesh mesh)
        {
            return mesh.GetConvexHull();
        }



    }
}

