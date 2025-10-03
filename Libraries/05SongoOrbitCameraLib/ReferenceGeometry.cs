using MyOpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Songo.OrbitCamera.ReferenceGeom
{
    public struct RefVertex
    {
        public Vector3 position;
        public Color4 color;

        public RefVertex(Vector3 p, Color4 c)
        {
            position = p;
            color = c;
        }

        public static int SizeInBytes()
        {
            return 3 * 4 * sizeof(float);
        }
    }
    public struct RefVertex2
    {
        public Vector2 position;
        public Color4 color;

        public RefVertex2(Vector2 p, Color4 c)
        {
            position = p;
            color = c;
        }

        public static int SizeInBytes()
        {
            return 2 * 4 * sizeof(float);
        }
    }

    public struct RefVertexN
    {
        public Vector3 position;
        public Color4 color;
        public Vector3 normal;

        public RefVertexN(Vector3 p, Vector3 n, Color4 c)
        {
            position = p;
            normal = n;
            color = c;
        }
    }


    public class ReferenceGeometry
    {
        private int vboGrid = -1;
        private int vboModel = -1;
        private int vboFocalGuid = -1;
        
        /// <summary>
        /// 
        /// </summary>
        public void CleanUpGPUMemory()
        {
            if (vboGrid > -1)
            {
                GL.DeleteBuffer(vboGrid); 
                vboGrid = -1;
            }
            if (vboModel > -1)
            {
                GL.DeleteBuffer(vboModel);
                vboModel = -1;
            }
            if (vboFocalGuid > -1)
            {
                GL.DeleteBuffer(vboFocalGuid);
                vboFocalGuid = -1;
            }
        }
 
        public int GetFocalGuidVBO()
        {
            return vboFocalGuid;
        }
        public int LoadModelGeometry(int vaoModel, int shaderprogram, out int count)
        {
            vaoModel = GL.GenVertexArray();
            GL.BindVertexArray(vaoModel);

            //need to set point size and line width here while the vao is bound           

             vboModel = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboModel);

            List<RefVertexN> model = drawModel();

            GL.BufferData(BufferTarget.ArrayBuffer, model.Count * 10 * sizeof(float), model.ToArray(), BufferUsageHint.StaticDraw);

            count = model.Count;
            // Get the location of the attributes that enters in the vertex shader
            int position_attribute = GL.GetAttribLocation(shaderprogram, "position");
            // Specify how the data for position can be accessed
            GL.VertexAttribPointer(position_attribute, 3, VertexAttribPointerType.Float, false, 10 * sizeof(float), 0);
            GL.EnableVertexAttribArray(position_attribute);

            // Get the location of the attributes that enters in the vertex shader
            int color_attribute = GL.GetAttribLocation(shaderprogram, "color");
            // Specify how the data for position can be accessed
            GL.VertexAttribPointer(color_attribute, 4, VertexAttribPointerType.Float, false, 10 * sizeof(float), 4 * sizeof(float));
            GL.EnableVertexAttribArray(color_attribute);

            GL.BindVertexArray(0);

            return vaoModel;
        }

        public int LoadFocalGuidGeometry(int vaoFocalGuid,  int shader  ,float screenHeight, float screenWidth)
        {
            List<RefVertex> focalgeom = new List<RefVertex>();

            vaoFocalGuid = GL.GenVertexArray();
            GL.BindVertexArray(vaoFocalGuid);
            
            vboFocalGuid = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboFocalGuid);

            focalgeom = UpdateFocalGeom(screenWidth, screenHeight);

            GL.BufferData(BufferTarget.ArrayBuffer, focalgeom.Count * 7 * sizeof(float), 0, BufferUsageHint.DynamicDraw);

            GL.BufferSubData(BufferTarget.ArrayBuffer, 0, focalgeom.Count * 7 * sizeof(float), focalgeom.ToArray());

            // Get the location of the attributes that enters in the vertex shader
            int position_attribute = GL.GetAttribLocation(shader, "position");
            // Specify how the data for position can be accessed
            GL.VertexAttribPointer(position_attribute, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);
            GL.EnableVertexAttribArray(position_attribute);

            // Get the location of the attributes that enters in the vertex shader
            int color_attribute = GL.GetAttribLocation(shader, "color");
            // Specify how the data for position can be accessed
            GL.VertexAttribPointer(color_attribute, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(color_attribute);

            GL.BindVertexArray(0);

            return vaoFocalGuid;
        }

        public List<RefVertex> UpdateFocalGeom(float screenWidth, float screenHeight)
        {
            List<RefVertex> geom = new List<RefVertex>();
            geom.Clear();
            Color4 yellow = new Color4(1.0f, 1.0f, 0.0f, 1.0f);
            geom.Add(new RefVertex(new Vector3(0f + 10f, screenHeight * 0.5f, 0.0f), yellow));
            geom.Add(new RefVertex(new Vector3(screenWidth - 10f, screenHeight * 0.5f, 0.0f), yellow));
            geom.Add(new RefVertex(new Vector3(screenWidth * 0.5f, 0f + 10f, 0.0f), yellow));
            geom.Add(new RefVertex(new Vector3(screenWidth * 0.5f, screenHeight - 10f, 0.0f), yellow));

            geom.Add(new RefVertex(new Vector3(screenWidth * 0.5f, screenHeight * 0.5f, 0.0f), yellow));
 
            return geom;
        }

        public int LoadGridReferenceGeometry(int vaoGrid, int shaderprogram, out int count)
        {
            vaoGrid = GL.GenVertexArray();
            GL.BindVertexArray(vaoGrid);

            //need to set point size and line width here while the vao is bound

            vboGrid = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboGrid);

            List<RefVertex> axis = TestDrawAxisLines(12.0f);
            List<RefVertex> gridXZ = drawGridXZ(10.0f, 1.0f);
            List<RefVertex> gridXY = drawGridXY(10.0f, 1.0f);

            axis.AddRange(gridXZ);
            axis.AddRange(gridXY);
            GL.BufferData(BufferTarget.ArrayBuffer, axis.Count * 7 * sizeof(float), axis.ToArray(), BufferUsageHint.StaticDraw);

            count = axis.Count;

            // Get the location of the attributes that enters in the vertex shader
            int position_attribute = GL.GetAttribLocation(shaderprogram, "position");
            // Specify how the data for position can be accessed
            GL.VertexAttribPointer(position_attribute, 3, VertexAttribPointerType.Float, false, 7 * sizeof(float), 0);
            GL.EnableVertexAttribArray(position_attribute);

            // Get the location of the attributes that enters in the vertex shader
            int color_attribute = GL.GetAttribLocation(shaderprogram, "color");
            // Specify how the data for position can be accessed
            GL.VertexAttribPointer(color_attribute, 4, VertexAttribPointerType.Float, false, 7 * sizeof(float), 4 * sizeof(float));
            GL.EnableVertexAttribArray(color_attribute);

            GL.BindVertexArray(0);

            return vaoGrid;
        }


        ///////////////////////////////////////////////////////////////////////////////
        // draw a grid on XZ-plane
        ///////////////////////////////////////////////////////////////////////////////
        internal List<RefVertex> drawGridXZ(float size, float step)
        {
            // 20x20 grid 
            List<RefVertex> refGeom = new List<RefVertex>();

            Color4 color = new Color4(0.5f, 0.5f, 0.5f, 0.5f);
            for (float i = step; i <= size; i += step)
            {
                refGeom.Add(new RefVertex(new Vector3(-size, 0, i), color));   // lines parallel to X-axis
                refGeom.Add(new RefVertex(new Vector3(size, 0, i), color));
                refGeom.Add(new RefVertex(new Vector3(-size, 0, -i), color));   // lines parallel to X-axis
                refGeom.Add(new RefVertex(new Vector3(size, 0, -i), color));

                refGeom.Add(new RefVertex(new Vector3(i, 0, -size), color));  // lines parallel to Z-axis
                refGeom.Add(new RefVertex(new Vector3(i, 0, size), color));
                refGeom.Add(new RefVertex(new Vector3(-i, 0, -size), color));  // lines parallel to Z-axis
                refGeom.Add(new RefVertex(new Vector3(-i, 0, size), color));
            }

            // x-axis
            color = new Color4(1f, 0f, 0f, 0.9f);
            refGeom.Add(new RefVertex(new Vector3(-size, 0, 0), color));
            refGeom.Add(new RefVertex(new Vector3(size, 0, 0), color));

            // z-axis 
            color = new Color4(0f, 0f, 1f, 0.9f);
            refGeom.Add(new RefVertex(new Vector3(0, 0, -size), color));
            refGeom.Add(new RefVertex(new Vector3(0, 0, size), color));

            return refGeom;
        }



        ///////////////////////////////////////////////////////////////////////////////
        // draw a grid on XY-plane
        ///////////////////////////////////////////////////////////////////////////////
        internal List<RefVertex> drawGridXY(float size, float step)
        {
            List<RefVertex> refGeom = new List<RefVertex>();

            Color4 color = new Color4(0.5f, 0.5f, 0.5f, 0.5f);

            // 20x20 grid            
            for (float i = step; i <= size; i += step)
            {
                refGeom.Add(new RefVertex(new Vector3(-size, i, 0), color));   // lines parallel to X-axis
                refGeom.Add(new RefVertex(new Vector3(size, i, 0), color));
                refGeom.Add(new RefVertex(new Vector3(-size, -i, 0), color));   // lines parallel to X-axis
                refGeom.Add(new RefVertex(new Vector3(size, -i, 0), color));

                refGeom.Add(new RefVertex(new Vector3(i, -size, 0), color));   // lines parallel to Y-axis
                refGeom.Add(new RefVertex(new Vector3(i, size, 0), color));
                refGeom.Add(new RefVertex(new Vector3(-i, -size, 0), color));   // lines parallel to Y-axis
                refGeom.Add(new RefVertex(new Vector3(-i, size, 0), color));
            }

            // x-axis
            color = new Color4(1, 0, 0, 0.5f);
            refGeom.Add(new RefVertex(new Vector3(-size, 0, 0), color));
            refGeom.Add(new RefVertex(new Vector3(size, 0, 0), color));

            // y-axis
            color = new Color4(0, 0, 1, 0.5f);
            refGeom.Add(new RefVertex(new Vector3(0, -size, 0), color));
            refGeom.Add(new RefVertex(new Vector3(0, size, 0), color));
            return refGeom;
        }

        ///////////////////////////////////////////////////////////////////////////////
        // draw the local axis of an object
        ///////////////////////////////////////////////////////////////////////////////
        internal List<RefVertex> drawAxisLines(float size)
        {
            List<RefVertex> refGeom = new List<RefVertex>();

            Color4 color = new Color4(1, 0, 0, 1);
            // draw axis
            GL.LineWidth(5);
            // glBegin(GL_LINES);
            //glColor3f(1, 0, 0);
            refGeom.Add(new RefVertex(new Vector3(0, 0, 0), color));
            refGeom.Add(new RefVertex(new Vector3(size, 0, 0), color));
            color = new Color4(0, 1, 0, 1);
            //glColor3f(0, 1, 0);
            refGeom.Add(new RefVertex(new Vector3(0, 0, 0), color));
            refGeom.Add(new RefVertex(new Vector3(0, size, 0), color));
            color = new Color4(0, 0, 1, 1);
            //glColor3f(0, 0, 1);
            refGeom.Add(new RefVertex(new Vector3(0, 0, 0), color));
            refGeom.Add(new RefVertex(new Vector3(0, 0, size), color));

            return refGeom;
        }

        ///////////////////////////////////////////////////////////////////////////////
        // draw the local axis of an object
        ///////////////////////////////////////////////////////////////////////////////
        internal List<RefVertex> TestDrawAxisLines(float size)
        {
            List<RefVertex> refGeom = new List<RefVertex>();

            Color4 color = new Color4(1f, 0f, 0f, 1f);
            // draw axis
            GL.LineWidth(5);
            // glBegin(GL_LINES);
            //glColor3f(1, 0, 0);
            refGeom.Add(new RefVertex(new Vector3(0, 0, 0), color));
            refGeom.Add(new RefVertex(new Vector3(size, 0, 0), color));

            color = new Color4(1f, 1f, 1f, 1f);
            refGeom.Add(new RefVertex(new Vector3(0, 0, 0), color));
            refGeom.Add(new RefVertex(new Vector3(-size, 0, 0), color));

            color = new Color4(0f, 1f, 0f, 1f);
            ////glColor3f(0, 1, 0);
            refGeom.Add(new RefVertex(new Vector3(0, 0, 0), color));
            refGeom.Add(new RefVertex(new Vector3(0, size, 0), color));

            color = new Color4(1f, 1f, 1f, 1f);
            refGeom.Add(new RefVertex(new Vector3(0, 0, 0), color));
            refGeom.Add(new RefVertex(new Vector3(0, -size, 0), color));

            color = new Color4(0f, 0f, 1f, 1f);
            ////glColor3f(0, 0, 1);
            refGeom.Add(new RefVertex(new Vector3(0, 0, 0), color));
            refGeom.Add(new RefVertex(new Vector3(0, 0, size), color));

            color = new Color4(1f, 1f, 1f, 1f);
            refGeom.Add(new RefVertex(new Vector3(0, 0, 0), color));
            refGeom.Add(new RefVertex(new Vector3(0, 0, -size), color));

            return refGeom;
        }
        internal List<RefVertex> drawAxisPoints(float size)
        {
            List<RefVertex> refGeom = new List<RefVertex>();

            // draw arrows(actually big square dots)
            GL.PointSize(10);
            //glBegin(GL_POINTS);
            refGeom.Add(new RefVertex(new Vector3(size, 0, 0), new Color4(1, 0, 0, 1)));

            refGeom.Add(new RefVertex(new Vector3(0, size, 0), new Color4(0, 1, 0, 1)));

            refGeom.Add(new RefVertex(new Vector3(0, 0, size), new Color4(0, 0, 1, 1)));

            //glEnd();
            return refGeom;
        }



        ///////////////////////////////////////////////////////////////////////////////
        // draw a point with a given size
        ///////////////////////////////////////////////////////////////////////////////
        internal RefVertex drawCameraPoint(float pointsize, Vector3 cameraTarget)
        {
            GL.PointSize(pointsize);
            Color4 color = new Color4(1, 1, 0, 1);

            //Vector3 cameraTarget = camera.getTarget();
            //glBegin(GL_POINTS);
            Vector3 v = new Vector3(cameraTarget.X, cameraTarget.Y, cameraTarget.Z);
            //glEnd();
            return new RefVertex(v, color);
        }



        ///////////////////////////////////////////////////////////////////////////////
        // draw a model (tetrahedron)
        ///////////////////////////////////////////////////////////////////////////////
        internal List<RefVertexN> drawModel()
        {
            List<RefVertexN> modelGeom = new List<RefVertexN>();

            Color4 color = new Color4(1.0f, 0.604f, 0.8204f, 0.196f);
            //color = Color4.YellowGreen;
            // Neon green new Color4(1.0f, 0.172f,0.99f, 0.02f)
            color = new Color4(1.0f, 0.06f, 0.99f, 0.02f);
            //glBegin(GL_TRIANGLES);
            Vector3 normal = new Vector3(0.6667f, 0.6667f, 0.3334f);

            modelGeom.Add(new RefVertexN(new Vector3(1, 0, 0), normal, color));
            modelGeom.Add(new RefVertexN(new Vector3(0, 1, 0), normal, color));
            modelGeom.Add(new RefVertexN(new Vector3(0, 0, 2), normal, color));

            normal = new Vector3(-0.6667f, 0.6667f, 0.3334f);

            modelGeom.Add(new RefVertexN(new Vector3(-1, 0, 0), normal, color));
            modelGeom.Add(new RefVertexN(new Vector3(0, 0, 2), normal, color));
            modelGeom.Add(new RefVertexN(new Vector3(0, 1, 0), normal, color));

            normal = new Vector3(0, 0, -1);
            modelGeom.Add(new RefVertexN(new Vector3(1, 0, 0), normal, color));
            modelGeom.Add(new RefVertexN(new Vector3(0, 0, 2), normal, color));
            modelGeom.Add(new RefVertexN(new Vector3(-1, 0, 0), normal, color));

            normal = new Vector3(0, -1, 0);
            modelGeom.Add(new RefVertexN(new Vector3(1, 0, 0), normal, color));
            modelGeom.Add(new RefVertexN(new Vector3(-1, 0, 0), normal, color));
            modelGeom.Add(new RefVertexN(new Vector3(0, 1, 0), normal, color));

            return modelGeom;
        }
    }
}
