using MyOpenTK;
using OpenTK.Mathematics;
using System.Text;

namespace HeepWare.OBJ.Data
{
    public struct BBox
    {
        private static int id = 1000000;
        public BBox()
        {
            max = Vector3.NegativeInfinity;
            min = Vector3.PositiveInfinity;
        }
        public Vector3 max;
        public Vector3 min;

        public float[] GetMin()
        {
            return new float[3] { min.X, min.Y, min.Z };
        }
        public float[] GetMax()
        {
            return new float[3] { max.X, max.Y, max.Z };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>GPU aligned data</returns>
        public List<VertexPosColorId> GetGPUVertices()
        {
            Color4 bboxColor = GetColor();
            List<VertexPosColorId> gpuVertices = new List<VertexPosColorId>();
            gpuVertices.Add(new VertexPosColorId() { position = new Vector3(min[0], min[1], min[2]), color = bboxColor, id = BBox.id++ });
            gpuVertices.Add(new VertexPosColorId() { position = new Vector3(max[0], min[1], min[2]), color = bboxColor, id = BBox.id++ });
            gpuVertices.Add(new VertexPosColorId() { position = new Vector3(max[0], max[1], min[2]), color = bboxColor, id = BBox.id++ });
            gpuVertices.Add(new VertexPosColorId() { position = new Vector3(min[0], max[1], min[2]), color = bboxColor, id = BBox.id++ });
            gpuVertices.Add(new VertexPosColorId() { position = new Vector3(min[0], min[1], max[2]), color = bboxColor, id = BBox.id++ });
            gpuVertices.Add(new VertexPosColorId() { position = new Vector3(max[0], min[1], max[2]), color = bboxColor, id = BBox.id++ });
            gpuVertices.Add(new VertexPosColorId() { position = new Vector3(max[0], max[1], max[2]), color = bboxColor, id = BBox.id++ });
            gpuVertices.Add(new VertexPosColorId() { position = new Vector3(min[0], max[1], max[2]), color = bboxColor, id = BBox.id++ });

            return gpuVertices;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<uint> GetGPUIndexes()
        {
            List<uint> indexes = new List<uint>() { 0, 1, 2,
                                                    2, 3, 0,

                                                    4,5,6,
                                                    6,7,4,

                                                    0,1,5,
                                                    5,4,0,

                                                    3,2,6,
                                                    6,7,3,
            
                                                    0,4,7,
                                                    7,3,0,
            
                                                    1,5,6,
                                                    6,2,1};


            return indexes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Color4 GetColor()
        {
            Color4 color;
            float xdiff = max[0] - min[0];
            float ydiff = max[1] - min[1];
            float zdiff = max[2] - min[2];

            if (zdiff >= xdiff && zdiff >= ydiff) color = Color4.DarkBlue;
            else if (xdiff >= zdiff && xdiff >= ydiff) color = Color4.Yellow;
            else if (ydiff >= zdiff && ydiff >= xdiff) color = Color4.DarkGreen;
            else color = Color4.White;
            color.A = 0.25f;
            return color;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public string WriteBbox2WaveFrontFile(string filename)
        {
            // Write to the StringBuilder
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("v {0} {1} {2}\n", min[0], min[1], min[2]);
            sb.AppendFormat("v {0} {1} {2}\n", max[0], min[1], min[2]);
            sb.AppendFormat("v {0} {1} {2}\n", max[0], max[1], min[2]);
            sb.AppendFormat("v {0} {1} {2}\n", min[0], max[1], min[2]);
            sb.AppendFormat("v {0} {1} {2}\n", min[0], min[1], max[2]);
            sb.AppendFormat("v {0} {1} {2}\n", max[0], min[1], max[2]);
            sb.AppendFormat("v {0} {1} {2}\n", max[0], max[1], max[2]);
            sb.AppendFormat("v {0} {1} {2}\n", min[0], max[1], max[2]);

            sb.AppendLine("//front");
            sb.AppendLine("f 1 2 3 ");
            sb.AppendLine("f 3 4 1 ");
            sb.AppendLine("//back");
            sb.AppendLine("f 5 6 7 ");
            sb.AppendLine("f 7 8 5 ");
            sb.AppendLine("//bottom");
            sb.AppendLine("f 1 2 6 ");
            sb.AppendLine("f 6 5 1 ");
            sb.AppendLine("//top");
            sb.AppendLine("f 4 3 7 ");
            sb.AppendLine("f 7 8 4 ");
            sb.AppendLine("//left");
            sb.AppendLine("f 1 5 8 ");
            sb.AppendLine("f 8 4 1 ");
            sb.AppendLine("//right");
            sb.AppendLine("f 2 6 7 ");
            sb.AppendLine("f 7 3 2 ");

            // Write to the file
            return sb.ToString();
        }

        public void WriteBbox2WavefrontFile(string filename, string objText)
        {
            // Write to the file
            File.WriteAllText(filename, objText);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("Bounding Box max:[{0}] min:[{1}]", max.ToString(), min.ToString()));

            return sb.ToString();
        }
    }


}
