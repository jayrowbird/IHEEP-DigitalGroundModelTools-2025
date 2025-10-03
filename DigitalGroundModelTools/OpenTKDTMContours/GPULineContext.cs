// Ignore Spelling: Vertices gpu

using HeepWare.ModelPicking.Framebuffer;
using MyOpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SongoOrbitCamera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKDTMContours
{
    
    public class GPULineContext
    {
        private int vertexArrayId = -1;
        private int vertexBufferId = -1;
        private int indexBufferId = -1;

        public void CleanUpGPUMemory()
        {
            if (vertexBufferId > 0)
            {
                GL.DeleteBuffer(vertexBufferId); MY.GLCall();
                vertexBufferId = -1;
            }

            if (indexBufferId > 0)
            {
                GL.DeleteVertexArray(indexBufferId); MY.GLCall();
                indexBufferId = -1;
            }

            if (vertexArrayId > 0)
            {
                GL.DeleteVertexArray(vertexArrayId); MY.GLCall();
                vertexArrayId = -1;
            }
        }
        public int GetVertexBufferId()
        {
            return vertexBufferId;
        }

        public int GetVertexArrayId()
        {
            return vertexArrayId;
        }

        public int GetIndexBufferId()
        {
            return indexBufferId;
        }
        public void InitializeGPUContext(int shaderId, List<VertexPosColorId> gpuVertices, List<uint> gpuIndexBuffer)
        {
            // the opengl context must be complete before calling any GL. methods  

            GL.Enable(EnableCap.DebugOutput);

            //shader = new ShaderSeparateFiles("Shaders\\OffScreenFramebuffer\\TestVertex.shader", "Shaders\\OffScreenFramebuffer\\TestFragment.shader");
            //shader.Bind();
            //shaderID = shader.Handle;

            //highLightModelVertexArrayId = MakeHighlightModel(shaderID, meshObjs[0]);

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
    }
}
