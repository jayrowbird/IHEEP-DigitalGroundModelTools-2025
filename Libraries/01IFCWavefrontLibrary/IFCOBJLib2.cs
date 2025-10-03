// Ignore Spelling: gpu Vertices

using FileFormatWavefront;
using FileFormatWavefront.Model;
using HeepWare.GPU.VertexBuffer.Data;
using HeepWare.Mesh.Utilities;
using HeepWare.OBJ.Data;
using HeepWare.OBJ.Mesh.Data;
using OpenTK.Mathematics;
using System.Text;

namespace HeepWare.OBJ.IFC.Library;

public class IFCOBJLib2
{
    private readonly bool DEBUGFLAG = false;
    private List<MeshObject> meshObjects = new List<MeshObject>();
    private List<VertexObj> vertices = new List<VertexObj>();
    private List<int> indexes = new List<int>();

    private List<Material>? materials = new List<Material>();

    //ObjFile entire file lists of vertices's 
    List<Vector3> position = new List<Vector3>();
    List<Vector3> normal = new List<Vector3>();
    List<Vector2> uv = new List<Vector2>();

    private string? filename = null;

    public enum OBJLineStatus
    {
        Adding2Model,
        AddingFaceText,
        NewModel,
        Unknown
    }

    public IFCOBJLib2(string fileName)
    {
        filename = fileName;
    }

    public List<int> GetIndexes()
    {
        return indexes;
    }

    public List<MeshObject> GetMeshObjects()
    {
        return meshObjects;
    }

    public List<VertexObj> GetVertices()
    {
        return vertices;
    }

    public void WriteOBJFile(string filename, List<VertexObj> gpuVertices, List<int> gpuIndexBuffer)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var vertex in gpuVertices)
        {
            sb.AppendLine(string.Format("v {0} {1} {2}", vertex.position.X, vertex.position.Y, vertex.position.Z));
        }

        for (int i = 0; i < gpuIndexBuffer.Count; i += 3)
        {
            sb.AppendLine(string.Format("f {0} {1} {2}", gpuIndexBuffer[i] + 1, gpuIndexBuffer[i + 1] + 1, gpuIndexBuffer[i + 2] + 1));
        }

        File.WriteAllText(filename, sb.ToString());
    }

    private List<Material>? LoadOBJMaterialFile(List<string> lines, out string materialFilename, string filename)
    {
        string? materialPath = null;
        materialFilename = string.Empty;
        if (lines.Count > 0)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Trim().StartsWith("mtllib") == true)
                {
                    materialFilename = lines[i].Replace("mtllib", "").Trim();
                    string[] items = lines[i].Split([' '], StringSplitOptions.RemoveEmptyEntries);

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
            if (File.Exists(materialPath))
            {
                var fileLoadResult = FileFormatMtl.Load(materialPath, false);
                return fileLoadResult.Model;
            }
        }
        return null;
    }

    private void LoadVertexArrays(List<string> lines)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].StartsWith("v ") == true)
            {
                Vector3? v = MakeVector3(lines[i]);
                if (v != null) { position.Add((Vector3)v); }
            }
            else if (lines[i].StartsWith("vn") == true)
            {
                Vector3? v = MakeVector3(lines[i]);
                if (v != null) { normal.Add((Vector3)v); }
            }
            else if (lines[i].StartsWith("vt") == true)
            {
                Vector2? v = MakeVector2(lines[i]);
                if (v != null) { uv.Add((Vector2)v); }
            }
        }
    }

    public int LoadFile()
    {
        if (filename == null)
        {
            throw new Exception(string.Format("File not found {0} or not obj file converted from ifc", filename));
        }

        List<string> lines = File.ReadLines(filename).ToList();

        List<OBJMeshStrings> objMeshesAsStrings = new List<OBJMeshStrings>();
        OBJMeshStrings? objMmeshAsStrings = null;
        string materialFIlenmae = string.Empty;

        if (lines.Count > 0)
        {
            //Load Material file if one exists
            materials = LoadOBJMaterialFile(lines, out materialFilename, filename);

            LoadVertexArrays(lines);

            OBJLineStatus linestatus = OBJLineStatus.NewModel;
            for (int i = 0; i < lines.Count; i++)
            {
                lines[i] = lines[i].Trim();
                if (string.IsNullOrEmpty(lines[i])) continue;
                // if (lines[i].StartsWith("g") == true)
                if (linestatus == OBJLineStatus.NewModel)
                {
                    if (objMmeshAsStrings != null) objMeshesAsStrings.Add(objMmeshAsStrings);
                    objMmeshAsStrings = new OBJMeshStrings();
                    //Console.WriteLine("{0}  {1}", count++, lines[i]);
                    objMmeshAsStrings.lines.Add(lines[i]);
                    linestatus = OBJLineStatus.Adding2Model;
                }
                else
                {
                    if ((lines[i].StartsWith("g") || lines[i].StartsWith("o") || lines[i].StartsWith("v")) && linestatus == OBJLineStatus.AddingFaceText)
                    {
                        linestatus = OBJLineStatus.NewModel;
                        i--;
                    }
                    else
                    {
                        if (lines[i].StartsWith("f"))
                        {
                            linestatus = OBJLineStatus.AddingFaceText;
                        }
                        if (objMmeshAsStrings != null) objMmeshAsStrings.lines.Add(lines[i]);
                    }
                }
            }
            //store the last model in the file
            if (objMmeshAsStrings != null) objMeshesAsStrings.Add(objMmeshAsStrings);

            for (int i = 0; i < objMeshesAsStrings.Count; i++)
            {
                if (DEBUGFLAG) File.WriteAllLines(string.Format(@"Output\objMmeshAsStrings{0}.txt", i), objMeshesAsStrings[i].lines);

                MeshObject? meshobj = ProcessMesh(objMeshesAsStrings[i], materialFilename);
                if (meshobj != null)
                {
                    meshObjects.Add(meshobj);
                }
                if (meshobj != null)
                {
                    if (false) File.WriteAllText(string.Format(@"Output\objMmeshAsStrings{0}.obj", i), meshobj.ToOBJFormat());
                }
                else
                {
                    throw new Exception("meshobj is null");
                }
            }
        }
        return meshObjects.Count;
    }



    //private int colorIndex = 0;
    //private Color4 GetRandomColor()
    //{
    //    Color4 color;
    //    switch (colorIndex++)
    //    {
    //        case 0:
    //            color = Color4.White; break;

    //        case 1:
    //            color = Color4.Black; break;
    //        case 2:
    //            color = Color4.BlueViolet; break;
    //        case 3:
    //            color = Color4.LimeGreen; break;
    //        case 4:
    //            color = Color4.Pink; break;
    //        case 5:
    //            color = Color4.Orchid; break;
    //        case 6:
    //            color = Color4.DarkCyan; break;
    //        case 7:
    //            color = Color4.AliceBlue; break;
    //        case 8:
    //            color = Color4.Teal; break;
    //        case 9:
    //            color = Color4.OliveDrab; break;
    //        case 10:
    //            color = Color4.DarkGray; break;
    //        default:
    //            colorIndex = 0;
    //            color = Color4.DarkMagenta; break;
    //    }
    //    color.A = 0.15f;
    //    return color;
    //}

    /// <summary>
    /// Return the format of the wave front file face indexes
    /// </summary>
    /// <param name="f">
    /// string text read from the wave front file face </param>
    /// <param name="normalCount"></param>
    /// <returns></returns>
    private FaceDataType FindFaceDataType(string f, int normalCount)
    {
        FaceDataType faceDataType_ = FaceDataType.NA;
        string[] items = f.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (items[1].Contains("//"))
        {
            if (normalCount < 1)
            {
                faceDataType_ = FaceDataType.PositionOny;
            }
            else
            {
                //position and normal only
                faceDataType_ = FaceDataType.PositionNormal;
            }
        }
        else if (items[1].Contains("/") == false)
        {
            //position only
            faceDataType_ = FaceDataType.PositionOny;
        }
        else if (items[1].Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Length == 2)
        {
            string[] items_ = f.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            //position and texture coordinate only
            faceDataType_ = FaceDataType.PositionTextureCoord;
        }
        else
        {
            //position and texture coordinate and normals
            faceDataType_ = FaceDataType.PositionTextureCoordNormal;
        }
        return faceDataType_;
    }

    /// <summary>
    /// pass in a the obj file lines of text for each model in the obj file
    /// step 1 store position, normal, uv in lists
    /// </summary>
    /// <param name="mesh"></param> 
    string materialFilename = string.Empty;
    private MeshObject? ProcessMesh(OBJMeshStrings mesh, string materialFilename)
    {
        FaceDataType faceDataType = FaceDataType.NA;

        bool done = false;
        //New mesh each time the material changes

        List<Color4> diffuseColor = new List<Color4>();

        MeshObject? meshObj = new MeshObject();

        string meshGuid = string.Empty;
        string materialname = string.Empty;
        Material? material = null;

        // store the indexes read from the objfile,
        // needed to fix text lines with
        // more than 3 groups of indexes 
        List<int> pi = new List<int>();
        List<int> vi = new List<int>();
        List<int> ni = new List<int>();

        for (int i = 0; i < mesh.lines.Count; i++)
        {
            if (mesh.lines[i].StartsWith("g") == true || mesh.lines[i].StartsWith("o") == true)
            {
                if (mesh.lines[i].StartsWith("o") == true)
                {
                    meshObj.objectName = mesh.lines[i].Replace("o", "").Trim();
                }
                else
                {
                    meshObj.groupName = mesh.lines[i].Replace("g ", "").Trim();
                }
            }
            else if (mesh.lines[i].StartsWith("use") == true)
            {
                materialname = mesh.lines[i].Replace("usemtl", "").Trim();
                if (materials == null || materials.Count < 1) continue;

                string mat = mesh.lines[i].Trim();
                mat = mat.Replace("usemtl", "").Trim();
                material = materials.Where(x => x.Name == mat).SingleOrDefault();
            }
            else if (mesh.lines[i].StartsWith("f") == true)
            {
                faceDataType = FindFaceDataType(mesh.lines[i], normal.Count);
                //when first face is found add vertices's to the meshObj's vertex struct
                if (done == false)
                {
                    meshObj.faceDataType = faceDataType;
                    meshObj.materialFilename = materialFilename;
                    meshObj.material = material;
                    if (material != null)
                    {
                        meshObj.materialName = material.Name;
                        meshObj.color = material.Diffuse;
                        meshObj.color = MeshUtils.GetRandomColor();
                    }
                    else if (materialname != null)
                    {
                        meshObj.materialName = materialname;
                        meshObj.color = MeshUtils.GetRandomColor();
                    }
                    else
                    {
                        meshObj.materialName = "NA";
                        meshObj.color = MeshUtils.GetRandomColor();
                    }
                    meshObj.MakeId();
                }
                done = true;

                //--------------------------------------------------------------------
                //face format f v/vt/vn or f v//vn or f v or f v//

                //if face count greater than 3

                //triangle 1 => 1,2,3
                //triangle 2 => 1,3,4
                //triangle 3 => 1,4,5
                //triangle 4 => 1,5,6  

                //uint faceIndexCount = 0;
                pi.Clear();
                vi.Clear();
                ni.Clear();

                string[] items = mesh.lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (faceDataType == FaceDataType.PositionNormal)
                {
                    //position and normal only
                    for (int l = 1; l < items.Length; l++)
                    {
                        string[] itemsl = items[l].Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries);
                        pi.Add(int.Parse(itemsl[0]) - 1);
                        ni.Add(int.Parse(itemsl[1]) - 1);
                    }
                    if (pi.Count > 3)
                    {
                        pi = AddIndexes(pi);
                        ni = AddIndexes(ni);
                    }
                    for (int c = 0; c < pi.Count; c++)
                    {
                        meshObj.position.Add(position[pi[c]]);
                        meshObj.normal.Add(normal[ni[c]]);

                        if (DEBUGFLAG) Console.WriteLine("index {0} mat {1} name {2}", pi[c], material, meshGuid);
                    }
                }
                else if (faceDataType == FaceDataType.PositionOny)
                {
                    //position only
                    for (int l = 1; l < items.Length; l++)
                    {
                        // fix this issue f 1// 2// 3// by removing "//"
                        int pos = int.Parse(items[l].Replace("//", "")) - 1;
                        pi.Add(pos);
                    }
                    if (pi.Count > 3)
                    {
                        pi = AddIndexes(pi);
                    }
                    for (int c = 0; c < pi.Count; c++)
                    {
                        meshObj.position.Add(position[pi[c]]);

                        if (DEBUGFLAG) Console.WriteLine("index {0} mat {1} name {2}", pi[c], material, meshGuid);
                    }
                }
                else if (faceDataType == FaceDataType.PositionTextureCoord)
                {
                    string[] items_ = mesh.lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    //position and texture coordinate only
                    for (int l = 1; l < items_.Length; l++)
                    {
                        string[] itemsl = items_[l].Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                        pi.Add(int.Parse(itemsl[0]) - 1);
                        vi.Add(int.Parse(itemsl[1]) - 1);
                    }
                    if (pi.Count > 3)
                    {
                        pi = AddIndexes(pi);
                        vi = AddIndexes(vi);
                    }
                    for (int c = 0; c < pi.Count; c++)
                    {
                        meshObj.position.Add(position[pi[c]]);
                        meshObj.textureCoord.Add(uv[vi[c]]);

                        if (DEBUGFLAG) Console.WriteLine("index {0} mat {1} name {2}", pi[c], material, meshGuid);
                    }
                }
                else if (faceDataType == FaceDataType.PositionTextureCoordNormal)
                {
                    //position and texture coordinate and normals
                    string[] items2 = mesh.lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int l = 1; l < items2.Length; l++)
                    {
                        string[] itemsl = items2[l].Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                        pi.Add(int.Parse(itemsl[0]) - 1);
                        vi.Add(int.Parse(itemsl[1]) - 1);
                        ni.Add(int.Parse(itemsl[2]) - 1);
                    }
                    if (pi.Count > 3)
                    {
                        pi = AddIndexes(pi);
                        ni = AddIndexes(ni);
                        vi = AddIndexes(vi);
                    }
                    for (int c = 0; c < pi.Count; c++)
                    {
                        meshObj.position.Add(position[pi[c]]);
                        meshObj.normal.Add(normal[ni[c]]);
                        meshObj.textureCoord.Add(uv[vi[c]]);

                        if (DEBUGFLAG) Console.WriteLine("index {0} mat {1} name {2}", pi[c], material, meshGuid);
                    }
                }
                else
                {
                    throw new Exception("unknown face data type");
                }
            }
        }
        for (int n = 0; n < meshObj.position.Count; n++)
        {
            meshObj.faceIndexes.Add((uint)n);
        }

        if (DEBUGFLAG) Console.WriteLine("Mesh Object {0}", meshObj);
        if (DEBUGFLAG) Console.WriteLine(" mesh to obj \n{0}", meshObj.ToOBJFormat());
        return meshObj;
    }

    //private List<ChildMesh> ProcessChildMeshes(List<string> lines, FaceDataType faceDataType,
    //                                           int positionCount, int normalCount, int uvCount)
    //{
    //    List<ChildMesh> children = new List<ChildMesh>();
    //    ChildMesh? childMesh = null;
    //    List<int> posIndexes = new List<int>();
    //    List<int> normIndexes = new List<int>();
    //    List<int> uvIndexes = new List<int>();

    //    for (int i = 0; i < lines.Count; i++)
    //    {

    //        if (lines[i].StartsWith("use") == true)
    //        {
    //            childMesh = new ChildMesh();
    //            children.Add(childMesh);
    //            if (materials == null || materials.Count < 1) continue;

    //            string mat = lines[i].Trim();
    //            mat = mat.Replace("usemtl", "").Trim();
    //            Material material = materials.Where(x => x.Name == mat).SingleOrDefault();

    //            childMesh.material = material;
    //            childMesh.color = material.Diffuse;
    //        }
    //        else if (lines[i].StartsWith("f") == true)
    //        {
    //            //--------------------------------------------------------------------
    //            //face format f v/vt/vn or f v//vn or f v or f v//

    //            //if face count greater than 3

    //            //triangle 1 => 1,2,3
    //            //triangle 2 => 1,3,4
    //            //triangle 3 => 1,4,5
    //            //triangle 4 => 1,5,6 

    //            //need to address quads not triangles
    //            //--------------------------------------------------------------------
    //            //faceIndexes.Clear();
    //            posIndexes.Clear();
    //            normIndexes.Clear();
    //            uvIndexes.Clear();

    //            string[] items = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    //            if (faceDataType == FaceDataType.PositionNormal)
    //            {
    //                //position and normal only
    //                for (int l = 1; l < items.Length; l++)
    //                {
    //                    string[] itemsl = items[l].Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries);
    //                    int pos = int.Parse(itemsl[0]) - 1;

    //                    int norm = int.Parse(itemsl[1]) - 1;

    //                    //faceIndexes.Add(pos);
    //                    posIndexes.Add(pos - positionCount);
    //                    normIndexes.Add(norm - normalCount);
    //                }

    //                childMesh.normIndexes.AddRange(AddIndexes(normIndexes));
    //                childMesh.posIndexes.AddRange(AddIndexes(posIndexes));
    //            }
    //            else if (faceDataType == FaceDataType.PositionOny)
    //            {
    //                //position only
    //                for (int l = 1; l < items.Length; l++)
    //                {
    //                    // fix this issue f 1// 2// 3// by removing "//"
    //                    int pos = int.Parse(items[l].Replace("//", "")) - 1;

    //                    posIndexes.Add(pos - positionCount);
    //                }

    //                childMesh.posIndexes.AddRange(AddIndexes(posIndexes));
    //            }
    //            else if (faceDataType == FaceDataType.PositionTextureCoord)
    //            {
    //                string[] items_ = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

    //                //position and texture coordinate only
    //                for (int l = 1; l < items_.Length; l++)
    //                {
    //                    string[] itemsl = items_[l].Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
    //                    int pos = int.Parse(itemsl[0]) - 1;

    //                    posIndexes.Add(pos - positionCount);

    //                    int vt = int.Parse(itemsl[1]) - 1;

    //                    uvIndexes.Add(vt - uvCount);

    //                    //faceIndexes.Add(pos);
    //                }

    //                childMesh.uvIndexes.AddRange(AddIndexes(uvIndexes));
    //                childMesh.posIndexes.AddRange(AddIndexes(posIndexes));
    //            }
    //            else if (faceDataType == FaceDataType.PositionTextureCoordNormal)
    //            {
    //                //position and texture coordinate and normals
    //                string[] items2 = lines[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    //                for (int l = 1; l < items2.Length; l++)
    //                {
    //                    string[] itemsl = items2[l].Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
    //                    int pos = int.Parse(itemsl[0]) - 1;
    //                    posIndexes.Add(pos - positionCount);

    //                    int vt = int.Parse(itemsl[1]) - 1;
    //                    uvIndexes.Add(vt - uvCount);

    //                    int norm = int.Parse(itemsl[2]) - 1;
    //                    normIndexes.Add(norm - normalCount);

    //                    //if (DEBUGFLAG) Console.WriteLine("index {0} mat {1} name {2}", pos, material, meshGuid);
    //                }

    //                childMesh.normIndexes.AddRange(AddIndexes(normIndexes));
    //                childMesh.uvIndexes.AddRange(AddIndexes(uvIndexes));
    //                childMesh.posIndexes.AddRange(AddIndexes(posIndexes));
    //            }
    //        }
    //    }

    //    return children;
    //}




    /// <summary>
    /// if the input int list contains more than three indexes
    /// convert the indexes to triangle indexes
    /// if face count greater than 3
    ///
    ////triangle 1 => 1,2,3     3 indexes
    ////triangle 2 => 1,3,4     4 indexes
    ////triangle 3 => 1,4,5     5 indexes
    ////triangle 4 => 1,5,6     6 indexes
    ///
    /// </summary>
    /// <param name="fi"></param> 
    /// <returns> indexes for triangles </returns> 

    private List<int> AddIndexes(List<int> fi)
    {
        List<int> findex = new List<int>();
        int loopcount = 1 + fi.Count - 3;
        int j = 1;
        for (int i = 0; i < loopcount; i++)
        {

            findex.Add((int)fi[0]);


            findex.Add((int)fi[j]);


            findex.Add((int)fi[j + 1]);
            j++;
        }

        return findex;
    }

      
    /// <summary>
    /// Given a line of text from a wavefront file 
    /// convert it into a Vector3 object
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
    /// Given a line of text from a wavefront file 
    /// convert it into a Vector2 object
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

