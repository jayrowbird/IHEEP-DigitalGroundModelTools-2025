// -----------------------------------------------------------------------
// <copyright file="TriangleFormat.cs" company="">
// Triangle.NET code by Christian Woltering, http://triangle.codeplex.com/
// </copyright>
// -----------------------------------------------------------------------

namespace TriangleNet.IO
{
    using System;
    using System.Collections.Generic;
    using TriangleNet.Geometry;
    using System.IO;

    /// <summary>
    /// Implements geometry and mesh file formats of the original Triangle code.
    /// </summary>
    public class WaveFrontOBJFormat : IGeometryFormat, IMeshFormat
    {
        public Mesh Import(string filename)
        {
            string ext = Path.GetExtension(filename);

            if (ext == ".obj")
            {
                List<ITriangle> triangles;
                InputGeometry geometry;

                FileReader.ReadOBJFile(filename, out geometry, out triangles);

                if (geometry != null && triangles != null)
                {
                    Mesh mesh = new Mesh();
                    mesh.Load(geometry, triangles);

                    return mesh;
                }
            }

            throw new NotSupportedException("Could not load '" + filename + "' file.");
        }

        public void Write(Mesh mesh, string filename)
        {
            FileWriter.WriteWaveFrontOBJFile(mesh, Path.ChangeExtension(filename, ".obj"));            
        }

        public InputGeometry Read(string filename)
        {
            string ext = Path.GetExtension(filename);

            if (ext == ".obj")
            {
                return FileReader.Read(filename);
            }
            throw new NotSupportedException("File format '" + ext + "' not supported.");
        }
    }
}
