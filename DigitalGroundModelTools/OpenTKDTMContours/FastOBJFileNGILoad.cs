using FileFormatWavefront;
using FileFormatWavefront.Model;
using HeepWare.IFC.Catalog;
using HeepWare.Mesh.Utilities;
using HeepWare.OBJ.Data;
using HeepWare.OBJ.Mesh.Data;
using MyOpenTK;
using OpenTK.Mathematics;


namespace OpenTKDTMContours
{
    public class VertexPosColorId_
    {
        public Vector3 position;
        public Color4 color;
        public int id;
    }
    public class FastOBJFileNGILoad
    {
        private IFCCatalog ifcCatalog = IFCCatalog.Instance;
        private string materialFilename = string.Empty;
        private List<VertexPosColorId_> tmpVertexbuffer = new List<VertexPosColorId_>();
        private List<VertexPosColorId> gpuPointVertexbuffer = new List<VertexPosColorId>();
        private List<VertexPosColorId> gpuLineVertexbuffer = new List<VertexPosColorId>();
        private List<VertexPosColorId> gpuModelVertexbuffer = new List<VertexPosColorId>();
        private List<uint> gpuPointIndexBuffer = new List<uint>();
        private List<uint> gpuLineIndexBuffer = new List<uint>();
        private List<uint> gpuModelIndexBuffer = new List<uint>();

        private List<Vector3> position = new List<Vector3>();
        private List<Vector3> normal = new List<Vector3>();
        private List<Vector2> texturecoord = new List<Vector2>();

        private List<Material>? materials = new List<Material>();
        private Material? material = null;

        private Color4? meshcolor = null;
        private Color4? pointColor = null;
        private Color4? lineColor = null;

        private List<RenderPrimitiveType> renderPrimitiveTypes = new List<RenderPrimitiveType>();


        public List<RenderPrimitiveType> GetRenderPrimitiveTypes()
        {
            return renderPrimitiveTypes;
        }

        public bool HasPoints()
        {
            return (gpuPointVertexbuffer.Count > 1);
        }
        public bool HasLines()
        {
            return (gpuLineVertexbuffer.Count > 1);
        }
        public bool HasModels()
        {
            return (gpuModelVertexbuffer.Count > 1);
        }

        public List<uint> GetGPUPointIndexBuffer()
        {
            return gpuPointIndexBuffer;
        }
        public List<uint> GetGPULineIndexBuffer()
        {
            return gpuLineIndexBuffer;
        }
        public List<uint> GetGPUModelIndexBuffer()
        {
            return gpuModelIndexBuffer;
        }

        public List<VertexPosColorId> GetGPUPointVertexBuffer()
        {
            return gpuPointVertexbuffer;
        }

        public List<Vector3> GetPositions()
        {
            return position;
        }

        public List<VertexPosColorId> GetGPULineVertexBuffer()
        {
            return gpuLineVertexbuffer;
        }

        public List<VertexPosColorId> GetGPUModelVertexBuffer()
        {
            return gpuModelVertexbuffer;
        }

        public FastOBJFileNGILoad(Color4? pointColor_ = null, Color4? lineColor_ = null, Color4? meshColor_ = null)
        {
            if (pointColor_ != null)
            { 
                this.pointColor = pointColor_;
            }
            else
            {
                this.pointColor = Color4.DarkGreen;
            }

            if (lineColor_ != null)
            {
                this.lineColor = lineColor_;
            }
            else
            {
                this.lineColor = Color4.DarkCyan;
            }

            if (meshColor_ != null)
            {
                this.meshcolor = meshColor_;
            }
            else
            {
                this.meshcolor = MeshUtils.GetRandomColor();
            } 
        }

        public List<RenderPrimitiveType> LoadFile(string filename)
        {
            if (filename == null)
            {
                throw new Exception(string.Format("File not found {0} or not obj file converted from ifc", filename));
            }
            //let the calling program know what type of data we read in


            List<string> lines = File.ReadLines(filename).ToList();

            List<OBJMeshStrings> objMeshesAsStrings = new List<OBJMeshStrings>();

            int meshid = -3;
            string meshname = string.Empty;
            Color4? theColor = null;

            if (lines.Count > 0)
            {
                //Load Material file if one exists
                materials = LoadOBJMaterialFile(lines, filename);

                for (int i = 0; i < lines.Count; i++)
                {
                    lines[i] = lines[i].Trim();

                    if (lines[i].StartsWith("g") == true || lines[i].StartsWith("o") == true)
                    {
                        meshname = lines[i].Replace("g ", "").Replace("o ", "").Trim();
                        meshid = ifcCatalog.Add(meshname);
                    }
                    else if (lines[i].StartsWith("mtllib ") == true)
                    {
                        materialFilename = lines[i].Trim().Replace("mtlib ", "");
                    }
                    else if (lines[i].StartsWith("v ") == true)
                    {

                        Vector3? v = MakeVector3(lines[i]);
                        if (v != null)
                        {  //if meshid has not been set by the time we reach the vertices section of the wavefront file set it here
                            if (meshid < 1)
                            {
                                string guid = string.Format("Elev:{0}_{1}", ((Vector3)v).Z, Guid.NewGuid());
                                ifcCatalog.Add(guid);
                                meshid = ifcCatalog.GetID(guid);
                                theColor = MeshUtils.GetRandomColor();
                            }
                            position.Add((Vector3)v);
                            tmpVertexbuffer.Add(new VertexPosColorId_() { position = (Vector3)v, color = (Color4)theColor, id = meshid });
                        }

                    }
                    else if (lines[i].StartsWith("vn") == true)
                    {
                        Vector3? v = MakeVector3(lines[i]);
                        if (v != null) { normal.Add((Vector3)v); }
                    }
                    else if (lines[i].StartsWith("vt") == true)
                    {
                        Vector2? v = MakeVector2(lines[i]);
                        if (v != null) { texturecoord.Add((Vector2)v); }
                    }
                    else if (lines[i].StartsWith("use") == true)
                    {
                        string mat = lines[i].Trim();
                        mat = mat.Replace("usemtl", "").Trim();
                        if (materials != null)
                        {
                            material = materials.Where(x => x.Name == mat).SingleOrDefault();
                            if (material != null)
                            {
                                Color4 color = material.Diffuse;                                 
                                color.A = 0.75f;
                                meshcolor = color;
                            }
                            else
                            {
                                meshcolor = MeshUtils.GetRandomColor();
                            }
                        }
                        else
                        {
                            Color4 color = MeshUtils.GetRandomColor();
                            color.A = 0.05f;
                            meshcolor = color; 
                        }
                    }
                    else if (lines[i].StartsWith("f") == true)
                    {
                        string line = lines[i].Replace("//", ",").Replace("/", ",");
                        string[] items = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        List<int> indexes = new List<int>();

                        //position only
                        for (int p = 1; p < items.Length; p++)
                        {
                            string[] itms = items[p].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                            indexes.Add(int.Parse(itms[0]) - 1);
                        }
                        List<uint> pindexes = AddIndexes(indexes);
                        for (int n = 0; n < pindexes.Count; n++)
                        {
                            int p = (int)pindexes[n];
                            tmpVertexbuffer[p].id = meshid;

                            tmpVertexbuffer[p].color = (Color4)meshcolor;
                        }
                        gpuModelIndexBuffer.AddRange(pindexes);
                    }
                    else if (lines[i].StartsWith("p") == true)
                    {
                        string[] items = lines[i].Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                         
                        for (int p = 1; p < items.Length; p++)
                        {
                            gpuPointIndexBuffer.Add(uint.Parse(items[p]) - 1);
                            tmpVertexbuffer[p].id = meshid;

                            tmpVertexbuffer[p].color = (Color4)meshcolor;
                        }
                    }
                    else if (lines[i].StartsWith("l") == true)
                    {
                        meshid = -3;
                        meshname = string.Empty;

                        string[] items = lines[i].Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        for (int l = 1; l < items.Length; l++)
                        {
                            gpuLineIndexBuffer.Add(uint.Parse(items[l]) - 1);
                        }
                    } 
                }
            }

            //if(gpuPointVertexbuffer.Count < 1)  
            //    {
            //    for (int i = 0; i < tmpVertexbuffer.Count; i++)
            //    {
            //        gpuPointVertexbuffer.Add(new VertexPosColorId()
            //        {
            //            position = tmpVertexbuffer[i].position,
            //            color = tmpVertexbuffer[i].color,
            //            id = tmpVertexbuffer[i].id
            //        });
            //        gpuPointIndexBuffer.Add((uint)i);
            //    }
            //}

             // assume the obj file will have points
                renderPrimitiveTypes.Add(RenderPrimitiveType.PointType);
            
            if (gpuLineIndexBuffer != null && gpuLineIndexBuffer.Count > 0)
            {
                renderPrimitiveTypes.Add(RenderPrimitiveType.LineType);
            }
            if (gpuModelIndexBuffer != null && gpuModelIndexBuffer.Count > 0)
            {
                renderPrimitiveTypes.Add(RenderPrimitiveType.MeshType);
            }
            if (renderPrimitiveTypes.Contains(RenderPrimitiveType.PointType) == true)
            {
                for (int i = 0; i < tmpVertexbuffer.Count; i++)
                {
                    Color4 pcolor = Color4.DarkSeaGreen;
                    gpuPointVertexbuffer.Add(new VertexPosColorId() { position = tmpVertexbuffer[i].position, color = pcolor, id = i + 1 });
                }
                if(gpuPointIndexBuffer.Count < 1)
                {
                    for (int i = 0; i < tmpVertexbuffer.Count; i++)
                    {
                        gpuPointIndexBuffer.Add((uint)i);
                    }
                }
            }
            if (renderPrimitiveTypes.Contains(RenderPrimitiveType.LineType) == true)
            {

                for (int i = 0; i < tmpVertexbuffer.Count; i++)
                {
                    gpuLineVertexbuffer.Add(new VertexPosColorId()
                    {
                        position = tmpVertexbuffer[i].position,
                        color = tmpVertexbuffer[i].color,
                        id = tmpVertexbuffer[i].id
                    });
                }
            }

            if (renderPrimitiveTypes.Contains(RenderPrimitiveType.MeshType) == true)
            {
                for (int i = 0; i < tmpVertexbuffer.Count; i++)
                {
                    gpuModelVertexbuffer.Add(new VertexPosColorId() { position = tmpVertexbuffer[i].position, color = tmpVertexbuffer[i].color, id = tmpVertexbuffer[i].id });
                }
            }

            return renderPrimitiveTypes;
        }

        private List<Material>? LoadOBJMaterialFile(List<string> lines, string filename)
        {
            string? materialPath = null;
            if (lines.Count > 0)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Trim().StartsWith("mtllib") == true)
                    {
                        string[] items = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (items.Length == 2)
                        {
                            DirectoryInfo dirInfo = new DirectoryInfo(filename);
                            if (dirInfo.Parent != null == true && dirInfo.Parent.Exists)
                            {
                                materialPath = dirInfo.Parent.FullName;
                                materialPath = Path.Combine(materialPath, items[1]);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
            if (materialPath != null)
            {
                var fileLoadResult = FileFormatMtl.Load(materialPath, false);
                return fileLoadResult.Model;
            }
            return null;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fi"></param> 
        private List<uint> AddIndexes(List<int> fi)
        {
            List<uint> findex = new List<uint>();
            int loopcount = 1 + fi.Count - 3;
            int j = 1;
            for (int i = 0; i < loopcount; i++)
            {

                findex.Add((uint)fi[0]);


                findex.Add((uint)fi[j]);


                findex.Add((uint)fi[j + 1]);
                j++;
            }

            return findex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private Vector3? MakeVector3(string line)
        {
            string[] items = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length == 4)
            {
                return new Vector3(float.Parse(items[1]), float.Parse(items[2]), float.Parse(items[3]));
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private Vector2? MakeVector2(string line)
        {
            string[] items = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length == 3)
            {
                return new Vector2(float.Parse(items[1]), float.Parse(items[2]));
            }
            return null;
        }
    }
}
