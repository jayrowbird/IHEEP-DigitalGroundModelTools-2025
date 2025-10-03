// Ignore Spelling: Utils meshobjects

using FileFormatWavefront.Model;
using HeepWare.GPU.VertexBuffer.Data;
using HeepWare.OBJ.Mesh.Data;
using MyOpenTK;
using OpenTK.Mathematics;
using System.Text;

namespace HeepWare.Mesh.Utilities
{
    public class MeshUtils
    {
        private static int colorIndex = 0;

        public static Color4 GetRandomColor()
        {
            Color4 color;
            switch (colorIndex++)
            {
                case 0:
                    color = Color4.White; break;

                case 1:
                    color = Color4.Black; break;
                case 2:
                    color = Color4.BlueViolet; break;
                case 3:
                    color = Color4.LimeGreen; break;
                case 4:
                    color = Color4.Pink; break;
                case 5:
                    color = Color4.Orchid; break;
                case 6:
                    color = Color4.DarkCyan; break;
                case 7:
                    color = Color4.AliceBlue; break;
                case 8:
                    color = Color4.Teal; break;
                case 9:
                    color = Color4.OliveDrab; break;
                case 10:
                    color = Color4.DarkGray; break;
                default:
                    colorIndex = 0;
                    color = Color4.DarkMagenta; break;
            }
            color.A = 0.15f;
            return color;
        }

        ///// <summary>
        ///// Create GPU vertex buffer data from mesh object data
        ///// </summary>
        ///// <param name="meshObj"></param>
        ///// <returns></returns>
        ///// <exception cref="Exception"></exception>
        //internal List<VertexObj> LoadVertexBuffer(MeshObject meshObj)
        //{
        //    List< VertexObj > vertices = new List< VertexObj >();

        //    if (meshObj.faceDataType == FaceDataType.PositionOny)
        //    {
        //        for (int i = 0; i < meshObj.position.Count; i++)
        //        {
        //            if (meshObj.material != null && meshObj.material.Diffuse != Material.NoColor)
        //            {
        //                vertices.Add(new VertexObj()
        //                {
        //                    id = meshObj.GetId(),
        //                    position = meshObj.position[(int)meshObj.posIndexes[i]],
        //                    color = meshObj.material.Diffuse
        //                });
        //            }
        //            else
        //            {
        //                vertices.Add(new VertexObj()
        //                {
        //                    id = meshObj.GetId(),
        //                    position = meshObj.position[(int)meshObj.posIndexes[i]]
        //                });
        //            }
        //        }
        //    }
        //    else if (meshObj.faceDataType == FaceDataType.PositionTextureCoord)
        //    {
        //        for (int i = 0; i < meshObj.position.Count; i++)
        //        {
        //            if (meshObj.material != null && meshObj.material.Diffuse != Material.NoColor)
        //            {
        //                vertices.Add(new VertexObj()
        //                {
        //                    id = meshObj.GetId(),
        //                    position = meshObj.position[(int)meshObj.posIndexes[i]],
        //                    texCoord = meshObj.textureCoord[(int)meshObj.uvIndexes[i]],
        //                    color = meshObj.material.Diffuse
        //                });
        //            }
        //            else
        //            {
        //                vertices.Add(new VertexObj()
        //                {
        //                    id = meshObj.GetId(),
        //                    position = meshObj.position[(int)meshObj.posIndexes[i]],
        //                    texCoord = meshObj.textureCoord[(int)meshObj.uvIndexes[i]],
        //                });
        //            }
        //        }
        //    }
        //    else if (meshObj.faceDataType == FaceDataType.PositionNormal)
        //    {
        //        for (int i = 0; i < meshObj.position.Count; i++)
        //        {
        //            if (meshObj.material != null && meshObj.material.Diffuse != Material.NoColor)
        //            {
        //                vertices.Add(new VertexObj()
        //                {
        //                    id = meshObj.GetId(),
        //                    position = meshObj.position[(int)meshObj.posIndexes[i]],
        //                    normal = meshObj.normal[(int)meshObj.normIndexes[i]],
        //                    color = meshObj.material.Diffuse
        //                });
        //            }
        //            else
        //            {
        //                vertices.Add(new VertexObj()
        //                {
        //                    id = meshObj.GetId(),
        //                    position = meshObj.position[(int)meshObj.posIndexes[i]],
        //                    normal = meshObj.normal[(int)meshObj.normIndexes[i]],
        //                });
        //            }
        //        }
        //    }
        //    else if (meshObj.faceDataType == FaceDataType.PositionTextureCoordNormal)
        //    {
        //        for (int i = 0; i < meshObj.position.Count; i++)
        //        {
        //            if (meshObj.material != null && meshObj.material.Diffuse != Material.NoColor)
        //            {
        //                vertices.Add(new VertexObj()
        //                {
        //                    id = meshObj.GetId(),
        //                    position = meshObj.position[(int)meshObj.posIndexes[i]],
        //                    normal = meshObj.normal[(int)meshObj.normIndexes[i]],
        //                    texCoord = meshObj.textureCoord[(int)meshObj.uvIndexes[i]],
        //                    color = meshObj.material.Diffuse
        //                });
        //            }
        //            else
        //            {
        //                vertices.Add(new VertexObj()
        //                {
        //                    id = meshObj.GetId(),
        //                    position = meshObj.position[(int)meshObj.posIndexes[i]],
        //                    normal = meshObj.normal[(int)meshObj.normIndexes[i]],
        //                    texCoord = meshObj.textureCoord[(int)meshObj.uvIndexes[i]],
        //                });
        //            }
        //        }
        //    }
        //    else
        //    {
        //        throw new Exception(string.Format("no data for model {0} was added to the vertex buffer", meshObj.GetName()));
        //    }
        //    return vertices; 
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void ComputeBoundingBox2(List<VertexPosColorId> gpuVertices, ref Vector3 center, ref Vector3 min, ref Vector3 max)
        {
            min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            Vector3 tmp;
            for (int n = 0; n < gpuVertices.Count; n++)
            {
                tmp = new Vector3(gpuVertices[n].position.X, gpuVertices[n].position.Y, gpuVertices[n].position.Z);

                min.X = Math.Min(min.X, tmp.X);
                min.Y = Math.Min(min.Y, tmp.Y);
                min.Z = Math.Min(min.Z, tmp.Z);

                max.X = Math.Max(max.X, tmp.X);
                max.Y = Math.Max(max.Y, tmp.Y);
                max.Z = Math.Max(max.Z, tmp.Z);
            }

            center.X = (min.X + max.X) / 2.0f;
            center.Y = (min.Y + max.Y) / 2.0f;
            center.Z = (min.Z + max.Z) / 2.0f;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void ComputeBoundingBox(List<Vector3> vertices, ref Vector3 center, ref Vector3 min, ref Vector3 max)
        {
            if (vertices == null || vertices.Count < 3)
            {
                throw new Exception(" vertices's  list is null or empty, data has not been loaded");
            }

            min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            Vector3 tmp;
            for (int i = 0; i < vertices.Count; i++)
            {
                tmp = new Vector3(vertices[i].X, vertices[i].Y, vertices[i].Z);

                min.X = Math.Min(min.X, tmp.X);
                min.Y = Math.Min(min.Y, tmp.Y);
                min.Z = Math.Min(min.Z, tmp.Z);

                max.X = Math.Max(max.X, tmp.X);
                max.Y = Math.Max(max.Y, tmp.Y);
                max.Z = Math.Max(max.Z, tmp.Z);
            }

            center.X = (min.X + max.X) / 2.0f;
            center.Y = (min.Y + max.Y) / 2.0f;
            center.Z = (min.Z + max.Z) / 2.0f;
        }

        //public static bool Write2OBJFile(string fullPath, List<MeshObject> meshobjects)
        //{
        //    bool result = false;
        //    StringBuilder sb = new StringBuilder();

        //    if (string.IsNullOrEmpty(fullPath) || meshobjects == null || meshobjects.Count < 1)
        //    {
        //        return result;
        //    }

        //    int positionCount = 0;
        //    int normalCount = 0;
        //    int textCoordCount = 0;

        //    for (int i = 0; i < meshobjects.Count; i++)
        //    {
        //        sb.AppendLine(string.Format("\ng {0}\n", meshobjects[i].GetName()));
        //        sb.AppendLine("\n# Positions");
        //        for (int p = 0; p < meshobjects[i].position.Count; p++)
        //        {
        //            sb.AppendLine(string.Format("v {0} {1} {2}", meshobjects[i].position[p].X, meshobjects[i].position[p].Y, meshobjects[i].position[p].Z));
        //        }
        //        if (meshobjects[i].textureCoord.Count > 0)
        //        {
        //            sb.AppendLine("\n# Texture coordinates");
        //            for (int t = 0; t < meshobjects[i].textureCoord.Count; t++)
        //            {
        //                sb.AppendLine(string.Format("vt {0} {1}  ", meshobjects[i].textureCoord[t].X, meshobjects[i].textureCoord[t].Y));
        //            }
        //        }
        //        if (meshobjects[i].normal.Count > 0)
        //        {
        //            sb.AppendLine("\n# Normals");
        //            for (int n = 0; n < meshobjects[i].normal.Count; n++)
        //            {
        //                sb.AppendLine(string.Format("vn {0} {1} {2}", meshobjects[i].normal[n].X, meshobjects[i].normal[n].Y, meshobjects[i].normal[n].Z));
        //            }
        //        }

        //        sb.AppendLine("\n# Face indexes");
        //        sb.AppendLine(string.Format("usemtl {0}\n", meshobjects[i].materialName));
        //        for (int f = 0; f < meshobjects[i].filePositionIndexes.Count; f += 3)
        //        {
        //            sb.Append("f");
        //            for (int t = 0; t < 3; t++)
        //            {
        //                if ((f + t) < meshobjects[i].filePositionIndexes.Count)
        //                {
        //                    sb.Append(string.Format(" {0}", meshobjects[i].filePositionIndexes[f + t] + 1 + positionCount));
        //                    if (meshobjects[i].fileTextureCoordIndexes.Count > 0)
        //                    {
        //                        if ((f + t) < meshobjects[i].fileTextureCoordIndexes.Count)
        //                        {
        //                            sb.Append(string.Format("/{0}", meshobjects[i].fileTextureCoordIndexes[f + t] + 1 + textCoordCount));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        sb.Append(string.Format("/"));
        //                    }
        //                    if (meshobjects[i].fileNormalIndexes.Count > 0)
        //                    {
        //                        if ((f + t) < meshobjects[i].fileNormalIndexes.Count)
        //                        {
        //                            sb.Append(string.Format("/{0}", meshobjects[i].fileNormalIndexes[f + t] + 1 + normalCount));
        //                        }
        //                    }
        //                    sb.Append(' ');
        //                }
        //            }
        //            sb.AppendLine();
        //        }
        //        positionCount += meshobjects[i].position.Count;
        //        normalCount += meshobjects[i].normal.Count;
        //        textCoordCount += meshobjects[i].textureCoord.Count;
        //    }
        //    File.WriteAllText(fullPath, sb.ToString());
        //    return result;
        //}

        //public static bool Write2OBJFIle(string fullpath, MeshObject mesh)
        //{
        //    bool result = false;
        //    StringBuilder sb = new StringBuilder();

        //    if (string.IsNullOrEmpty(fullpath) || mesh == null)
        //    {
        //        return result;
        //    }

        //    int positionCount = 0;
        //    int normalCount = 0;
        //    int textCoordCount = 0;


        //    sb.AppendLine(string.Format("\ng {0}\n", mesh.GetName()));
        //    sb.AppendLine("\n# Positions");
        //    for (int p = 0; p < mesh.position.Count; p++)
        //    {
        //        sb.AppendLine(string.Format("v {0} {1} {2}", mesh.position[p].X, mesh.position[p].Y, mesh.position[p].Z));
        //    }
        //    if (mesh.textureCoord.Count > 0)
        //    {
        //        sb.AppendLine("\n# Texture coordinates");
        //        for (int t = 0; t < mesh.textureCoord.Count; t++)
        //        {
        //            sb.AppendLine(string.Format("vt {0} {1}  ", mesh.textureCoord[t].X, mesh.textureCoord[t].Y));
        //        }
        //    }
        //    if (mesh.normal.Count > 0)
        //    {
        //        sb.AppendLine("\n# Normals");
        //        for (int n = 0; n < mesh.normal.Count; n++)
        //        {
        //            sb.AppendLine(string.Format("vn {0} {1} {2}", mesh.normal[n].X, mesh.normal[n].Y, mesh.normal[n].Z));
        //        }
        //    }

        //    sb.AppendLine("\n# Face indexes");
        //    sb.AppendLine(string.Format("usemtl {0}\n", mesh.materialName));
        //    for (int f = 0; f < mesh.filePositionIndexes.Count; f += 3)
        //    {
        //        sb.Append("f");
        //        for (int t = 0; t < 3; t++)
        //        {
        //            if ((f + t) < mesh.filePositionIndexes.Count)
        //            {
        //                sb.Append(string.Format(" {0}", mesh.filePositionIndexes[f + t] + 1 + positionCount));
        //                if (mesh.fileTextureCoordIndexes.Count > 0)
        //                {
        //                    if ((f + t) < mesh.fileTextureCoordIndexes.Count)
        //                    {
        //                        sb.Append(string.Format("/{0}", mesh.fileTextureCoordIndexes[f + t] + 1 + textCoordCount));
        //                    }
        //                }
        //                else
        //                {
        //                    sb.Append(string.Format("/"));
        //                }
        //                if (mesh.fileNormalIndexes.Count > 0)
        //                {
        //                    if ((f + t) < mesh.fileNormalIndexes.Count)
        //                    {
        //                        sb.Append(string.Format("/{0}", mesh.fileNormalIndexes[f + t] + 1 + normalCount));
        //                    }
        //                }
        //                sb.Append(' ');
        //            }
        //        }
        //        sb.AppendLine();
        //    }
        //    positionCount += mesh.position.Count;
        //    normalCount += mesh.normal.Count;
        //    textCoordCount += mesh.textureCoord.Count;

        //    File.WriteAllText(fullpath, sb.ToString());
        //    return result;
        //}

        public static bool Write2OBJFile(string fullPath, MeshObject mesh)
        {
            File.WriteAllText(fullPath, mesh.ToOBJFormat());
            return true;
        }
    }
}
