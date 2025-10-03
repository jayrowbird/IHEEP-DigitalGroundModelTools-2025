// Ignore Spelling: vertices Coord

using FileFormatWavefront.Model;
using HeepWare.GPU.VertexBuffer.Data;
using HeepWare.IFC.Catalog;
using HeepWare.OBJ.Data;
using OpenTK.Mathematics;
using System.Text;

namespace HeepWare.OBJ.Mesh.Data
{
    public enum FaceDataType
    {
        PositionOny,
        PositionNormal,
        PositionTextureCoord,
        PositionTextureCoordNormal,
        NA
    }

    public class ChildMesh
    {
        public Material? material = null;
        //public int id = -1;
        public Color4 color;

        //public List<uint> filePositionIndexes = new List<uint>();
        //public List<uint> fileNormalIndexes = new List<uint>();
        //public List<uint> fileTextureCoordIndexes = new List<uint>();
    }

    public enum RenderPrimitiveType
    {
        PointType,
        LineType,
        MeshType,
        NA
    }

    public class MeshObject
    {
        private IFCCatalog ifcCatalog = IFCCatalog.Instance;
        //name should be set to groupname first, objectname is groupname is not found
        private RenderPrimitiveType primitiveType = RenderPrimitiveType.MeshType; 
        public string groupName = string.Empty;
        public string objectName = string.Empty;
        public string? materialName = null;
        public string? materialFilename = null;
        public Material? material = null;
        private int id = -1;
        public Color4 color;

        //remove the 2 lines below after fixing issue with winforms
        public List<VertexObj> vertices = new List<VertexObj>();
        public List<uint> faceIndexes = new List<uint>();

        public FaceDataType faceDataType = FaceDataType.NA;

        public List<Vector3> position = new List<Vector3>();
        public List<Vector3> normal = new List<Vector3>();
        public List<Vector2> textureCoord = new List<Vector2>();

        //public List<uint> filePositionIndexes = new List<uint>();
        //public List<uint> fileNormalIndexes = new List<uint>();
        //public List<uint> fileTextureCoordIndexes = new List<uint>();

        public List<ChildMesh> childMeshes = new List<ChildMesh>();

        private BBox bbox;
        private Vector3 center;
        private bool BBOXCOMPUTED = false;

        public BBox GetBBox()
        {
            if (BBOXCOMPUTED == false)
            {
                ComputerBoundngBox();
            }

            return bbox;
        }

        public int GetId()
        {
            if (id == -1)
            {
                MakeId();
            }
            return id;
        }

        public Vector3 GetCenter()
        {
            if (BBOXCOMPUTED == false)
            {
                ComputerBoundngBox();
            }

            return center;
        }

        public string GetName()
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                return groupName;
            }
            else if (!string.IsNullOrEmpty(objectName))
            {
                return objectName;
            }
            return "No name";
        }

        /// <summary>
        /// Set the mesh object => id <= after all possible obj names have been set
        /// </summary>
        public void MakeId()
        {
            if (!string.IsNullOrEmpty(groupName))
            {
                id = ifcCatalog.GetID(groupName);
                if (id == -1)
                {
                    id = ifcCatalog.Add(groupName);
                }
            }
            else if (!string.IsNullOrEmpty(objectName))
            {
                id = ifcCatalog.GetID(objectName);
                if (id == -1)
                {
                    id = ifcCatalog.Add(objectName);
                }
            }
            else
            {
                id = ifcCatalog.GetID("Unknown model name");
                if (id == -1)
                {
                    id = ifcCatalog.Add("Unknown model name");
                }
                //throw new Exception("Mesh id could not be set");
            }
        }

        private void ComputerBoundngBox()
        {
            bbox.min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            bbox.max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            for (int i = 0; i < position.Count; i++)
            {
                bbox.min.X = Math.Min(bbox.min.X, position[i].X);
                bbox.min.Y = Math.Min(bbox.min.Y, position[i].Y);
                bbox.min.Z = Math.Min(bbox.min.Z, position[i].Z);

                bbox.max.X = Math.Max(bbox.max.X, position[i].X);
                bbox.max.Y = Math.Max(bbox.max.Y, position[i].Y);
                bbox.max.Z = Math.Max(bbox.max.Z, position[i].Z);
            }
            center.X = (bbox.min.X + bbox.max.X) / 2.0f;
            center.Y = (bbox.min.Y + bbox.max.Y) / 2.0f;
            center.Z = (bbox.min.Z + bbox.max.Z) / 2.0f;
            BBOXCOMPUTED = true;
        }

        public string ToOBJFormat()
        { 
            StringBuilder sb = new StringBuilder();
            if (primitiveType == RenderPrimitiveType.MeshType)
            {
                if (groupName != null)
                {
                    sb.AppendLine(string.Format("g {0}", groupName));
                }

                if (!string.IsNullOrEmpty(objectName))
                {
                    sb.AppendLine(string.Format("o {0}", objectName));
                }

                if (id > -1)
                {
                    sb.AppendLine(string.Format("# => id {0} <=", GetId()));
                }

                if (!string.IsNullOrEmpty(materialFilename))
                {
                    sb.AppendLine(string.Format("mtllib {0}", materialFilename));
                }

                if (materialName != null)
                {
                    sb.AppendLine(string.Format("usemtl {0}", materialName));
                }

                sb.AppendLine("\n# Positions");
                for (int p = 0; p < position.Count; p++)
                {
                    sb.AppendLine(string.Format("v {0} {1} {2}", position[p].X, position[p].Y, position[p].Z));
                }

                sb.AppendLine("\n# Texture coordinates");
                for (int vt = 0; vt < textureCoord.Count; vt++)
                {
                    sb.AppendLine(string.Format("vt {0} {1}", textureCoord[vt].X, textureCoord[vt].Y));
                }

                sb.AppendLine("\n# Normals");
                for (int n = 0; n < normal.Count; n++)
                {
                    sb.AppendLine(string.Format("vn {0} {1} {2}", normal[n].X, normal[n].Y, normal[n].Z));
                }

                if (position.Count > 0)
                {
                    sb.AppendLine("\n# Face indexes");

                    if (normal.Count < 1 && textureCoord.Count < 1)
                    {
                        for (int f = 0; f < position.Count; f++)
                        {
                            if (f % 3 == 0)
                            {
                                sb.AppendLine();
                                sb.Append("f ");
                            }
                            sb.Append(string.Format("{0} ", f + 1));
                        }
                    }

                    else if (normal.Count > 0 && textureCoord.Count < 1)
                    {
                        for (int f = 0; f < position.Count; f++)
                        {
                            if (f % 3 == 0)
                            {
                                sb.AppendLine();
                                sb.Append("f ");
                            }
                            sb.Append(string.Format("{0}//{1} ", f + 1, f + 1));
                        }
                    }

                    else if (normal.Count < 1 && textureCoord.Count > 0)
                    {
                        for (int f = 0; f < position.Count; f++)
                        {
                            if (f % 3 == 0)
                            {
                                sb.AppendLine();
                                sb.Append("f ");
                            }
                            sb.Append(string.Format("{0}/{1} ", f + 1, f + 1));
                        }
                    }

                    else if (normal.Count > 0 && textureCoord.Count > 0)
                    {
                        for (int f = 0; f < position.Count; f++)
                        {
                            if (f % 3 == 0)
                            {
                                sb.AppendLine();
                                sb.Append("f ");
                            }
                            sb.Append(string.Format("{0}/{1}/{2} ", f + 1, f + 1, f + 1));
                        }
                    }



                }
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("MeshObj name:[{0}] id:[{1}] color:[{2}]", GetName(), id, color));
            sb.AppendLine(string.Format("[{0}], [Center:[{1}]]", bbox, center));
            sb.AppendLine(string.Format("Face Data Type: [{0}]", faceDataType.ToString()));
            if (material == null)
            {
                sb.AppendLine("material is null");
            }
            else
            {
                if (!string.IsNullOrEmpty(materialFilename))
                    sb.AppendLine(string.Format("material filename :[{0}]", materialFilename.ToString()));
                sb.AppendLine(string.Format("material :[{0}]", material.ToString()));
            }
            sb.AppendLine("\n-- Positions --");
            sb.Append($"[{0,5}] ");
            int j = 0;
            for (int i = 0; i < position.Count; i++)
            {
                sb.Append($" [{position[i],32}] "); j++;
                if (j % 3 == 0 && i < position.Count - 1)
                {
                    sb.AppendLine();
                    sb.Append($"[{i + 1,5}] ");
                    j = 0;
                }
            }
            if (normal.Count > 0)
            {
                sb.AppendLine("\n-- Normals --");
                sb.Append($"[{0,5}] ");
                j = 0;
                for (int i = 0; i < normal.Count; i++)
                {
                    sb.Append($" [{normal[i],32}] "); j++;
                    if (j % 3 == 0 && i < normal.Count - 1)
                    {
                        sb.AppendLine();
                        sb.Append($"[{i + 1,5}] ");
                        j = 0;
                    }
                }
            }
            if (textureCoord.Count > 0)
            {
                sb.AppendLine("\n-- Texture Coordinates --");
                sb.Append($"[{0,5}] ");
                j = 0;
                for (int i = 0; i < textureCoord.Count; i++)
                {
                    sb.Append($" [{textureCoord[i],32}] "); j++;
                    if (j % 3 == 0 && i < textureCoord.Count - 1)
                    {
                        sb.AppendLine();
                        sb.Append($"[{i + 1,5}] ");
                        j = 0;
                    }
                }
            }
            //sb.AppendLine("\n-- Position Indexes --");
            //sb.Append($"[{0,2}] ");
            //j = 0;
            //for (int i = 0; i < posIndexes.Count; i++)
            //{
            //    sb.Append($" [{posIndexes[i],2}] "); j++;
            //    if (j % 12 == 0 && i < posIndexes.Count - 1)
            //    {
            //        sb.AppendLine();
            //        sb.Append($"[{i + 1,2}] ");
            //        j = 0;
            //    }
            //}

            //($"{customer[DisplayPos],10}" +
            //      $"{salesFigures[DisplayPos],10}" +
            //      $"{feePayable[DisplayPos],10}" +
            //      $"{seventyPercentValue,10}" +
            //      $"{thirtyPercentValue,10}");

            return sb.ToString();
        }

        //public void Dispose()
        //{
        //    for(int i = vertices.Count-1; i >= 0; i--)
        //    {
        //        vertices.Remove(vertices[i]);
        //    }
        //    faceIndexes.Clear();
        //}
    }
}
