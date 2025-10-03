using HeepWare.Contouring;
using HeepWare.Mesh.Utilities;
using MyOpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using TriangleNet.Data;

namespace OpenTKDTMContours
{
    public class GroundRender
    {
        private FastOBJFileNGILoad fastOBJFileNGILoader;

        private static int id = 1;
        private string name;
        private Color4 color;
        private bool FILLMODE = false;
        private bool SHOWTRIANGLES = false;
        private bool SHOWPOINTS = false;
        private bool SHOWCONTOURS = false;

        private Vector3 groundCenter, groundMin, groundMax = Vector3.Zero;

        //Point data for rendering ground points
        //Point data gpu context
        private int pointVertexArrayId = -1;
        private int pointVertexBufferId = -1;
        private int pointIndexBufferId = -1;
        private List<VertexPosColorId> gpuPointVertices;
        private List<uint> gpuPointIndexBuffer;

        //Contour Data for rendering
        //Line data gpu context
        private int lineVertexArrayId = -1;
        private int lineVertexBufferId = -1;
        private int lineIndexBufferId = -1;
        private List<VertexPosColorId> gpuLineVertices;
        private List<uint> gpuLineIndexBuffer;

        //Contour Data for rendering
        //Model data gpu context
        private int modelVertexArrayId = -1;
        private int modelVertexBufferId = -1;
        private int modelIndexBufferId = -1;
        private List<VertexPosColorId> gpuModelVertices;
        private List<uint> gpuModelIndexBuffer;

        private MeshUtils meshUtils;
        private int shaderId = -1;

        public GroundRender(string name, FastOBJFileNGILoad fastOBJFileNGILoader, int shaderId)
        {
            meshUtils = new MeshUtils();
            this.shaderId = shaderId;
            this.name = name;

            if (fastOBJFileNGILoader != null)
            {
                if (fastOBJFileNGILoader.HasPoints() == true)
                {
                    LoadPoints(fastOBJFileNGILoader);

                }
                if (fastOBJFileNGILoader.HasLines() == true)
                {
                    LoadContours(fastOBJFileNGILoader);
                }
                if (fastOBJFileNGILoader.HasModels() == true)
                {
                    LoadModels(fastOBJFileNGILoader);
                }
            }
        }

        public GroundRender(string name, List<List<Vertex>>? prisms, int shaderId)
        {
            meshUtils = new MeshUtils();
            this.shaderId = shaderId;
            this.name = name;

            Prism2GPUDate(prisms);

            meshUtils.ComputeBoundingBox2(gpuLineVertices, ref groundCenter, ref groundMin, ref groundMax);

            InitializeGPUContext(shaderId,
              gpuLineVertices,
              gpuLineIndexBuffer,
              out lineVertexArrayId,
              out lineVertexBufferId,
              out lineIndexBufferId);

            InitializeGPUContext(shaderId,
             gpuModelVertices,
             gpuModelIndexBuffer,
             out modelVertexArrayId,
             out modelVertexBufferId,
             out modelIndexBufferId);

            FILLMODE = true;
        }

        private void Prism2GPUDate(List<List<Vertex>> prisms)
        {
            uint indexCount = 0;
            if (gpuLineVertices == null)
            {
                gpuLineVertices = new List<VertexPosColorId>();
            }
            else
            {
                gpuLineVertices.Clear();
            }
            if (gpuLineIndexBuffer == null)
            {
                gpuLineIndexBuffer = new List<uint>();
            }
            else
            {
                gpuLineIndexBuffer.Clear();
            }
            if (gpuModelVertices == null)
            {
                gpuModelVertices = new List<VertexPosColorId>();
            }
            else
            {
                gpuModelVertices.Clear();
            }
            if (gpuModelIndexBuffer == null)
            {
                gpuModelIndexBuffer = new List<uint>();
            }
            else
            {
                gpuModelIndexBuffer.Clear();
            }

            Color4 modelColor = Color4.Red; modelColor.A = 0.5f;
            Color4 lineColor = Color4.Yellow;
            for (int i = 0; i < prisms.Count; i++)
            {
                List<Vertex> prism = prisms[i];
                for (int n = 0; n < prism.Count; n++)
                {
                    gpuLineVertices.Add(new VertexPosColorId() { position = new Vector3((float)prism[n].X, (float)prism[n].Y, (float)prism[n].Z), color = lineColor, id = -3 });
                    gpuModelVertices.Add(new VertexPosColorId() { position = new Vector3((float)prism[n].X, (float)prism[n].Y, (float)prism[n].Z), color = modelColor, id = -4 });
                }
                gpuLineIndexBuffer.Add(0 + indexCount); gpuLineIndexBuffer.Add(1 + indexCount);
                gpuLineIndexBuffer.Add(2 + indexCount); gpuLineIndexBuffer.Add(3 + indexCount);
                gpuLineIndexBuffer.Add(4 + indexCount); gpuLineIndexBuffer.Add(5 + indexCount);

                gpuModelIndexBuffer.Add(0 + indexCount); gpuModelIndexBuffer.Add(2 + indexCount); gpuModelIndexBuffer.Add(4 + indexCount);
                gpuModelIndexBuffer.Add(1 + indexCount); gpuModelIndexBuffer.Add(3 + indexCount); gpuModelIndexBuffer.Add(5 + indexCount);



                //Below display triangles
                if (true)
                {
                    gpuModelIndexBuffer.Add(0 + indexCount); gpuModelIndexBuffer.Add(1 + indexCount); gpuModelIndexBuffer.Add(2 + indexCount);
                    gpuModelIndexBuffer.Add(1 + indexCount); gpuModelIndexBuffer.Add(3 + indexCount); gpuModelIndexBuffer.Add(2 + indexCount);

                    gpuModelIndexBuffer.Add(2 + indexCount); gpuModelIndexBuffer.Add(3 + indexCount); gpuModelIndexBuffer.Add(4 + indexCount);
                    gpuModelIndexBuffer.Add(3 + indexCount); gpuModelIndexBuffer.Add(5 + indexCount); gpuModelIndexBuffer.Add(4 + indexCount);

                    gpuModelIndexBuffer.Add(4 + indexCount); gpuModelIndexBuffer.Add(5 + indexCount); gpuModelIndexBuffer.Add(0 + indexCount);
                    gpuModelIndexBuffer.Add(5 + indexCount); gpuModelIndexBuffer.Add(1 + indexCount); gpuModelIndexBuffer.Add(0 + indexCount);

                    gpuModelIndexBuffer.Add(0 + indexCount); gpuModelIndexBuffer.Add(2 + indexCount); gpuModelIndexBuffer.Add(4 + indexCount);
                    gpuModelIndexBuffer.Add(1 + indexCount); gpuModelIndexBuffer.Add(3 + indexCount); gpuModelIndexBuffer.Add(5 + indexCount);
                }
                indexCount += 6;
            }

        }

        public Vector3 GetCenter()
        {
            return groundCenter;
        }
        public Vector3 GetMax()
        {
            return groundMax;
        }
        public Vector3 GetMin()
        {
            return groundMin;
        }


        public void ClearGPUMemory()
        {
            ClearPointGPUMemory();
            ClearLineGPUMemory();
            ClearModelGPUMemory();
        }

        private void ClearPointGPUMemory()
        {
            if (pointVertexArrayId > -1)
            {
                GL.DeleteVertexArray(pointVertexArrayId); MY.GLCall();
                pointVertexArrayId = -1;
            }
            if (pointIndexBufferId > -1)
            {
                GL.DeleteBuffer(pointIndexBufferId); MY.GLCall();
                pointIndexBufferId = -1;
            }
            if (pointVertexBufferId > -1)
            {
                GL.DeleteBuffer(pointVertexBufferId); MY.GLCall();
                pointVertexBufferId = -1;
            }
            gpuPointIndexBuffer.Clear();
            gpuPointVertices.Clear();
        }

        private void ClearLineGPUMemory()
        {
            if (lineVertexArrayId > -1)
            {
                GL.DeleteVertexArray(lineVertexArrayId); MY.GLCall();
                lineVertexArrayId = -1;
            }
            if (lineIndexBufferId > -1)
            {
                GL.DeleteBuffer(lineIndexBufferId); MY.GLCall();
                lineIndexBufferId = -1;
            }
            if (lineVertexBufferId > -1)
            {
                GL.DeleteBuffer(lineVertexBufferId); MY.GLCall();
                lineVertexBufferId = -1;
            }
            gpuLineIndexBuffer.Clear();
            gpuLineVertices.Clear();
        }

        private void ClearModelGPUMemory()
        {
            if (modelVertexArrayId > -1)
            {
                GL.DeleteVertexArray(modelVertexArrayId); MY.GLCall();
                modelVertexArrayId = -1;
            }
            if (modelIndexBufferId > -1)
            {
                GL.DeleteBuffer(modelIndexBufferId); MY.GLCall();
                modelIndexBufferId = -1;
            }
            if (modelVertexBufferId > -1)
            {
                GL.DeleteBuffer(modelVertexBufferId); MY.GLCall();
                modelVertexBufferId = -1;
            }
            gpuModelIndexBuffer.Clear();
            gpuModelVertices.Clear();
        }

        public void ShowTriangles(bool value)
        {
            SHOWTRIANGLES = value;
        }
        public void ShowPoints(bool value)
        {
            SHOWPOINTS = value;
        }
        public void ShowContours(bool value)
        {
            SHOWCONTOURS = value;
        }

        public Vector3 LoadPoints(FastOBJFileNGILoad fastOBJFileNGILoader)
        {
            gpuPointVertices = fastOBJFileNGILoader.GetGPUPointVertexBuffer();
            gpuPointIndexBuffer = fastOBJFileNGILoader.GetGPUPointIndexBuffer();

            if (gpuPointVertices == null || gpuPointIndexBuffer == null || gpuPointVertices.Count < 1 || gpuPointIndexBuffer.Count < 1)
            {
                throw new Exception("RenderPrimitiveType is Point data yet either the vertex buffer is empty of the index buffer is empty");
            }

            meshUtils.ComputeBoundingBox2(gpuPointVertices, ref groundCenter, ref groundMin, ref groundMax);

            InitializeGPUContext(shaderId,
              gpuPointVertices,
              gpuPointIndexBuffer,
              out pointVertexArrayId,
              out pointVertexBufferId,
              out pointIndexBufferId);

            return groundCenter;
        }

        public Vector3? LoadTriangles(TriangleNet.Mesh groundMesh, Color4 color)
        {
            if (groundMesh == null || groundMesh.Triangles == null) return null;
            if (gpuModelIndexBuffer == null) gpuModelIndexBuffer = new List<uint>();
            else gpuModelIndexBuffer.Clear();
            if (gpuModelVertices == null) gpuModelVertices = new List<VertexPosColorId>();
            else gpuModelVertices.Clear();

            Vertex vx;
            Vector3 v;

            for (int t = 0; t < groundMesh.Triangles.Count; t++)
            {
                vx = groundMesh.Triangles.ElementAt(t).GetVertex(0);
                v = new Vector3((float)vx.X, (float)vx.Y, (float)vx.Z);
                gpuModelVertices.Add(new VertexPosColorId() { position = v, color = color, id = t + 1 });
                vx = groundMesh.Triangles.ElementAt(t).GetVertex(1);
                v = new Vector3((float)vx.X, (float)vx.Y, (float)vx.Z);
                gpuModelVertices.Add(new VertexPosColorId() { position = v, color = color, id = t + 2 });
                vx = groundMesh.Triangles.ElementAt(t).GetVertex(2);
                v = new Vector3((float)vx.X, (float)vx.Y, (float)vx.Z);
                gpuModelVertices.Add(new VertexPosColorId() { position = v, color = color, id = t + 3 });
            }

            uint count = 0;
            for (int t = 0; t < groundMesh.Triangles.Count; t++)
            {
                gpuModelIndexBuffer.Add(count++);
                gpuModelIndexBuffer.Add(count++);
                gpuModelIndexBuffer.Add(count++);
            }

            if (gpuModelVertices == null || gpuModelIndexBuffer == null || gpuModelVertices.Count < 1 || gpuModelIndexBuffer.Count < 1)
            {
                throw new Exception("RenderPrimitiveType is Model data yet either the vertex buffer is empty of the index buffer is empty");
            }

            meshUtils.ComputeBoundingBox2(gpuModelVertices, ref groundCenter, ref groundMin, ref groundMax);

            InitializeGPUContext(shaderId,
              gpuModelVertices,
              gpuModelIndexBuffer,
              out modelVertexArrayId,
              out modelVertexBufferId,
              out modelIndexBufferId);

            return groundCenter;
        }

        public void LoadContoursLineStrings(List<LineString> linestrings, Color4 color)
        {
            if (linestrings == null) return;

            if (gpuLineVertices == null) gpuLineVertices = new List<VertexPosColorId>();
            if (gpuLineIndexBuffer == null) gpuLineIndexBuffer = new List<uint>(); 

            for (int tt = 0; tt < linestrings.Count; tt++)
            {
                LineString linestring = linestrings[tt];
                for (int l = 0; l < linestring.Count; l++)
                {
                    gpuLineVertices.Add(new VertexPosColorId()
                    {
                        position = new Vector3((float)linestring[l].X, (float)linestring[l].Y, (float)linestring[l].Z),
                        color = color,
                        id = 1
                    });
                    if (l == 0)
                    {
                        // draw line p0 to p1 ...
                        //add indexes after first vertex is added
                        continue;
                    }
                    else if (l > 0)
                    {
                        gpuLineIndexBuffer.Add((uint)(gpuLineVertices.Count - 2));
                        gpuLineIndexBuffer.Add((uint)(gpuLineVertices.Count - 1));
                    }
                }
            } 

            InitializeGPUContext(shaderId,
                                 gpuLineVertices,
                                 gpuLineIndexBuffer,
                                 out lineVertexArrayId,
                                 out lineVertexBufferId,
                                 out lineIndexBufferId);
        }


        public void LoadContours(FastOBJFileNGILoad fastOBJFileNGILoader)
        {
            gpuLineVertices = fastOBJFileNGILoader.GetGPULineVertexBuffer();
            gpuLineIndexBuffer = fastOBJFileNGILoader.GetGPULineIndexBuffer();
            if (gpuLineVertices == null || gpuLineIndexBuffer == null || gpuLineVertices.Count < 1 || gpuLineIndexBuffer.Count < 1)
            {
                throw new Exception("RenderPrimitiveType is Line data yet either the vertex buffer is empty of the index buffer is empty");
            }

            // Note: point centroid should be the same as the contour line centroid
            //meshUtils.ComputeBoundingBox2(gpuLineVertices, ref groundCenter, ref groundCenter, ref groundCenter);
            //GPUPointContext gpuPointContext = new GPUPointContext();
            InitializeGPUContext(shaderId,
              gpuLineVertices,
              gpuLineIndexBuffer,
              out lineVertexArrayId,
              out lineVertexBufferId,
              out lineIndexBufferId);
        }

        public void LoadModels(FastOBJFileNGILoad fastOBJFileNGILoader)
        {
            gpuModelVertices = fastOBJFileNGILoader.GetGPUModelVertexBuffer();
            gpuModelIndexBuffer = fastOBJFileNGILoader.GetGPUModelIndexBuffer();
            if (gpuModelVertices == null || gpuModelIndexBuffer == null || gpuModelVertices.Count < 1 || gpuModelIndexBuffer.Count < 1)
            {
                throw new Exception("RenderPrimitiveType is Model data yet either the vertex buffer is empty of the index buffer is empty");
            }

            // Note: point centroid should be the same as the contour line centroid
            //meshUtils.ComputeBoundingBox2(gpuLineVertices, ref groundCenter, ref groundCenter, ref groundCenter);
            //GPUPointContext gpuPointContext = new GPUPointContext();
            InitializeGPUContext(shaderId,
              gpuModelVertices,
              gpuModelIndexBuffer,
              out modelVertexArrayId,
              out modelVertexBufferId,
              out modelIndexBufferId);
        }

        public void InitializeGPUContext(int shaderId,
                                         List<VertexPosColorId> gpuVertices,
                                         List<uint> gpuIndexBuffer,
                                         out int vertexArrayId,
                                         out int vertexBufferId,
                                         out int indexBufferId)

        {
            // the opengl context must be complete before calling any GL. methods  
            GL.Enable(EnableCap.DebugOutput);

            GL.UseProgram(shaderId);

            //ArrayBuffer
            MY.GLCall(vertexArrayId = GL.GenVertexArray());
            GL.BindVertexArray(vertexArrayId); MY.GLCall();

            //VertexBuffer
            vertexBufferId = GL.GenBuffer(); MY.GLCall();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId); MY.GLCall();
            GL.BufferData(BufferTarget.ArrayBuffer, gpuVertices.Count * ((3 + 4) * sizeof(float) + (1 * sizeof(int))), gpuVertices.ToArray(), BufferUsageHint.StaticDraw); MY.GLCall();
            MY.GLCall();

            //IndexBuffer
            MY.GLCall(indexBufferId = GL.GenBuffer());
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferId); MY.GLCall();
            GL.BufferData(BufferTarget.ElementArrayBuffer, gpuIndexBuffer.Count * sizeof(UInt32), gpuIndexBuffer.ToArray(), BufferUsageHint.StaticDraw); MY.GLCall();
            MY.GLCall();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            //Color
            GL.EnableVertexAttribArray(1); MY.GLCall();
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, ((3 + 4) * sizeof(float) + (1 * sizeof(int))), 3 * sizeof(float));
            MY.GLCall();

            //position
            GL.EnableVertexAttribArray(0); MY.GLCall();
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, ((3 + 4) * sizeof(float) + (1 * sizeof(int))), 0);
            MY.GLCall();

            //Model id
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribIPointer(2, 1, VertexAttribIntegerType.Int, ((3 + 4) * sizeof(float) + (1 * sizeof(int))), 7 * sizeof(float));
            MY.GLCall();

            GL.BindVertexArray(0);
            GL.UseProgram(0);
        }

        Matrix4 view;
        int count = 0;
        public void RenderFrameGL(double time, //time in milliseconds 
                                   Matrix4 mvp)
        {

            // render
            // ------
            // bind to frame-buffer and draw scene as we normally would to color texture 
            //GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferId);
            //GL.Viewport(0, 0, screenWidth, screenHeight);
            //GL.Enable(EnableCap.DepthTest); // enable depth testing (is disabled for rendering screen-space quad)


            GL.UseProgram(shaderId);
            if (pointVertexArrayId > -1 && SHOWPOINTS == true)
            {
                GL.Disable(EnableCap.CullFace);
                GL.Disable(EnableCap.DepthTest);
                GL.PointSize(2);

                GL.BindVertexArray(pointVertexArrayId);

                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);
                GL.EnableVertexAttribArray(2);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, pointIndexBufferId);

                GL.UniformMatrix4(GL.GetUniformLocation(shaderId, "u_ViewProjection"), false, ref mvp);

                GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Point);
                GL.DrawElements(PrimitiveType.Points, gpuPointIndexBuffer.Count, DrawElementsType.UnsignedInt, 0); //MY.GLCall();
            }

            if (lineVertexArrayId > -1 && SHOWCONTOURS)
            {
                GL.Disable(EnableCap.CullFace);
                GL.Disable(EnableCap.DepthTest);

                GL.LineWidth(2);
                GL.BindVertexArray(lineVertexArrayId);

                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);
                GL.EnableVertexAttribArray(2);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, lineIndexBufferId);

                GL.UniformMatrix4(GL.GetUniformLocation(shaderId, "u_ViewProjection"), false, ref mvp);

                GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
                GL.DrawElements(PrimitiveType.Lines, gpuLineIndexBuffer.Count, DrawElementsType.UnsignedInt, 0); //MY.GLCall();
            }

            if (modelVertexArrayId > -1 && SHOWTRIANGLES == true)
            {
                GL.Enable(EnableCap.CullFace);
                GL.Enable(EnableCap.DepthTest);

                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

                GL.BindVertexArray(modelVertexArrayId);

                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);
                GL.EnableVertexAttribArray(2);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, modelIndexBufferId);

                GL.UniformMatrix4(GL.GetUniformLocation(shaderId, "u_ViewProjection"), false, ref mvp);

                if (FILLMODE)
                {
                    GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
                }
                else
                {
                    GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
                }
                GL.DrawElements(PrimitiveType.Triangles, gpuModelIndexBuffer.Count, DrawElementsType.UnsignedInt, 0); //MY.GLCall();
            }
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Disable(EnableCap.Blend);
        }
    }
}
