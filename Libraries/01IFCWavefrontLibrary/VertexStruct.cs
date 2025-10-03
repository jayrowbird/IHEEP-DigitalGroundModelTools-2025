using HeepWare.GPU.VertexBuffer.Data;
using OpenTK.Mathematics;

namespace MyOpenTK
{
    public class VertexStruct
    {
        public Vector3 position;
        public Vector3 normal;
        public Color4 color;
        public Vector2 textureCoord ;
        public float texIndex;
        //float tilingFactor;

        // Editor-only
        //int EntityID; 
    }

    public class ObjVertex
    {
        public Vector3? position;
        public Vector3? normal;
        public Color4? color;
        public Vector2? textureCoord;
        public float texIndex = float.MinValue;

        public string name = string.Empty;
        public string material = string.Empty;
        public int id = -1;
    }

    public struct VertexPos
    {
        public Vector3 position;
    }

    public struct VertexPosId
    {
        public Vector3 position;
        public int id;
    }

    public struct VertexPosColor
    {
        public Vector3 position;
        public Color4 color;
    }

    public struct VertexPosNormalColorId
    {
        public Vector3 position;
        public Vector3 normal;
        public Color4 color;
        public int id;
    }
    public struct VertexPosNormalUVColorId
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 uv;
        public Color4 color;
        public int id;
    }

    public struct VertexPosColorId
    {
        public Vector3 position;
        public Color4 color;
        public int id;
    }

    public class MapVertices
    {
        public List<VertexPosColorId> MapObjVertices(List<VertexObj> v)
        {
            List<VertexPosColorId> gpuv = new List<VertexPosColorId>();
            for (int i = 0; i < v.Count; i++)
            {
                gpuv.Add(new VertexPosColorId()
                {
                    position = v[i].position,
                    color = v[i].color,
                    id = (int)v[i].id
                });
            }
            return gpuv;
        }

        public List<uint> MapObjIndexes(List<int> ix)
        {
            List<uint> objIndexes = new List<uint>();
            for (int i = 0; i < ix.Count; i++)
            {
                objIndexes.Add((uint)ix[i]);
            }
            return objIndexes;
        }
    }
}