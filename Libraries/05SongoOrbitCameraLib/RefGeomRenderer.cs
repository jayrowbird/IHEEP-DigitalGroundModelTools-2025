using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Songo.OrbitCamera;
using Songo.OrbitCamera.ReferenceGeom;
using System.Diagnostics;

namespace SongoOrbitCameraLib
{
    public class RefGeomRenderer
    {
        private int shaderprogram = -1;
        private int focalshaderprogram = -1;
        private int vaoGrid = -1;
        private int vaoModel = -1;
        private int vaoFocalGuid = -1;
        //private int vboFocalGuid = -1;
        private int linecount = 0;
        private int modelcount = 0;
        private ReferenceGeometry? referenceGeometry = new ReferenceGeometry();
        private Matrix4 projection;
        private Matrix4 ortho;
        private Matrix4 view;
        private Matrix4 focalview;
        private Matrix4 translation2ceneter;
        private Matrix4 translation2eye;
        private const float CAMERA_DISTANCE = 15.0f;
        //bool zooming = false;
        private float playTime;     // sec
        private OrbitCamera refOrbitCamera;
        int screenWidth;
        int screenHeight;
        public RefGeomRenderer(int screenWidth_, int screenHeight_,
                               OrbitCamera orbitCamera_)
        {
            screenWidth = screenWidth_;
            screenHeight = screenHeight_;
            refOrbitCamera = orbitCamera_;
        }

        /// <summary>
        ///  Need to release the GPU resources used by this guy
        /// </summary>
        public void CleanUpGPUMemory()
        {
            if(referenceGeometry != null)
            {
                referenceGeometry.CleanUpGPUMemory();
            }

            //Grid and Model shader program
            if (shaderprogram > -1)
            {
                GL.DeleteShader(shaderprogram);
                shaderprogram = -1;
            }
            // focal shader program displays the big yellow cross hairs
            if (focalshaderprogram > -1)
            {
                GL.DeleteShader(focalshaderprogram);
                focalshaderprogram = -1;
            } 

            linecount = 0;
            modelcount = 0;
            referenceGeometry = null;
        }
 
        public void OnRenderFrame()
        {
            // update times
            float elapsedTime = refOrbitCamera.GetTimerElapsed();
            float frameTime = elapsedTime - playTime;
            playTime = elapsedTime;
            // update camera
            refOrbitCamera.update(frameTime);
            GL.Viewport(0, 0, screenWidth, screenHeight);

            //EnableCap Depth 
            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest);

            //The main model render loop must clear the screen
            //keep below commented out
            //GL.ClearColor(new Color4(0.25f, 0.25f, 0.25f, 1.0f));
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            view = refOrbitCamera.getMatrix();
            //view  *= translation2ceneter;
            view = translation2ceneter * view;

            GL.LineWidth(1f);
            //Draw grids
            GL.UseProgram(shaderprogram);
            GL.BindVertexArray(vaoGrid); 

            GL.UniformMatrix4(GL.GetUniformLocation(shaderprogram, "projection"), false, ref projection);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderprogram, "view"), false, ref view);

            GL.DrawArrays(PrimitiveType.Lines, 0, linecount);
            

            //draw model 
            GL.UseProgram(shaderprogram);
            GL.BindVertexArray(vaoModel);

            GL.UniformMatrix4(GL.GetUniformLocation(shaderprogram, "projection"), false, ref projection);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderprogram, "view"), false, ref view);

            
            if(drawMode == 1)
                GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
            else if(drawMode == 2)
                GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Point);
            else
                GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            GL.DrawArrays(PrimitiveType.Triangles, 0, modelcount);

            // draw focal guide lines ===================
            GL.LineWidth(2f);
            GL.PointSize(10f);

            GL.UseProgram(focalshaderprogram);
            GL.BindVertexArray(vaoFocalGuid);

            GL.UniformMatrix4(GL.GetUniformLocation(focalshaderprogram, "projection"), false, ref ortho);
            GL.UniformMatrix4(GL.GetUniformLocation(focalshaderprogram, "view"), false, ref focalview);

            GL.DrawArrays(PrimitiveType.Lines, 0, 4);
            GL.DrawArrays(PrimitiveType.Points, 0, 5);

            //GL.LineWidth(1f);
            //GL.PointSize(1f);

            GL.UseProgram(0);
            GL.BindVertexArray(0);

            //Disable Depth test 
            GL.Disable(EnableCap.DepthTest);
        }

        public void OnResize(int width, int height)
        {
            screenWidth = width;
            screenHeight = height;
            //deal with minimized screen
            if (screenWidth <= screenHeight) return;
            //if ((this.ClientSize.X / this.ClientSize.Y) <= 1.00) return;
            GL.Viewport(0, 0, screenWidth, screenHeight);
            projection = Matrix4.CreatePerspectiveFieldOfView((float)(2.0f * Math.PI / 6.0f), (screenWidth / screenHeight), 0.01f, 10000);
            ortho = Matrix4.CreateOrthographic(screenWidth, screenHeight, 0.01f, 1000f);

            focalview = Matrix4.LookAt(new Vector3(screenWidth * 0.5f, screenHeight * 0.5f, 100), new Vector3(screenWidth * 0.5f, screenHeight * 0.5f, 0), new Vector3(0, 1, 0));

            //Yes this is needed to show the screen size cross hairs
            if (referenceGeometry != null)
            {
                GL.BindVertexArray(vaoFocalGuid);
                GL.BindBuffer(BufferTarget.ArrayBuffer, referenceGeometry.GetFocalGuidVBO());

                List<RefVertex> geom = referenceGeometry.UpdateFocalGeom((float)screenWidth, (float)screenHeight);
                GL.BufferSubData(BufferTarget.ArrayBuffer, 0, geom.Count * 7 * sizeof(float), geom.ToArray());

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindVertexArray(0);
            }
        }

        public void SetRefTarget(Vector3 target)
        {
            translation2ceneter = Matrix4.CreateTranslation(target);
        }

        public void SetRefEye(Vector3 eye)
        {
            translation2eye = Matrix4.CreateTranslation(eye);
        }

        public void Load()
        {
            if (referenceGeometry != null)
            {
                //orbitCameraUtils.TimerStart();
                playTime = 0;
                shaderprogram = create_program(@".\Shaders\LineVertexShader.txt", @".\Shaders\LineFragmentShader.txt");

                focalshaderprogram = create_program(@".\Shaders\AxisVertexShader.txt", @".\Shaders\AxisFragmentShader.txt");

                vaoGrid = referenceGeometry.LoadGridReferenceGeometry(vaoGrid, shaderprogram, out linecount);
                vaoModel = referenceGeometry.LoadModelGeometry(vaoModel, shaderprogram, out modelcount);

                vaoFocalGuid = referenceGeometry.LoadFocalGuidGeometry(vaoFocalGuid, focalshaderprogram, screenHeight, screenWidth);

                projection = Matrix4.CreatePerspectiveFieldOfView((float)(2.0f * Math.PI / 6.0f), (screenWidth / screenHeight), 0.01f, 10000);

                focalview = Matrix4.LookAt(new Vector3(screenWidth * 0.5f, screenHeight * 0.5f, 100), new Vector3(screenWidth * 0.5f, screenHeight * 0.5f, 0), new Vector3(0, 1, 0));

                ortho = Matrix4.CreateOrthographic((float)screenWidth, (float)screenHeight, 0.01f, 1000f);

                GL.BindVertexArray(0);
                GL.UseProgram(0);

                resetCamera();
                view = refOrbitCamera.getMatrix();
                GL.Enable(EnableCap.DepthTest);
                GL.Enable(EnableCap.ProgramPointSize);
                GL.PointSize(5f);
            }
        }

        // Create a program from two shaders
        internal int create_program(string path_vert_shader, string path_frag_shader)
        {
            // Load and compile the vertex and fragment shaders
            //int vertexShader = load_and_compile_shader(path_vert_shader, GL_VERTEX_SHADER);
            //int fragmentShader = load_and_compile_shader(path_frag_shader, GL_FRAGMENT_SHADER);

            string vShaderCode = File.ReadAllText(path_vert_shader);
            string fShaderCode = File.ReadAllText(path_frag_shader);

            // vertex shader
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vShaderCode);
            GL.CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fShaderCode);
            GL.CompileShader(fragmentShader);

            // Attach the above shader to a program
            int shaderprogram = GL.CreateProgram();
            GL.AttachShader(shaderprogram, vertexShader);
            GL.AttachShader(shaderprogram, fragmentShader);

            // Flag the shaders for deletion
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            // Link and use the program
            GL.LinkProgram(shaderprogram);
            checkCompileErrors(shaderprogram, "PROGRAM");
            GL.UseProgram(shaderprogram);

            var programLog = GL.GetProgramInfoLog(shaderprogram);
            if (!string.IsNullOrEmpty(programLog)) throw new Exception(programLog);

            return shaderprogram;
        }
        int drawMode = 0;
        internal void SetDrawingMode(int Mode)
        {
            drawMode = Mode;

            if (drawMode < 0)
                drawMode = 0;
            if (drawMode > 3)
                drawMode = 3;

            if (drawMode == 0)        // fill mode
            {
                GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
                GL.Enable(EnableCap.DepthTest);
                GL.Enable(EnableCap.CullFace);
            }
            else if (drawMode == 1)  // wireframe mode
            {
                GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
                GL.LineWidth(5f);
                GL.Enable(EnableCap.DepthTest);
                GL.Enable(EnableCap.CullFace);
            }
            else                    // point mode
            {
                GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Point);
                GL.PointSize(10f);
                GL.Enable(EnableCap.DepthTest);
                GL.Enable(EnableCap.CullFace);
            }

            return;
        }

        /////////////////////////////////////////////////////////////////////////////////
        //// reset camera to default
        /////////////////////////////////////////////////////////////////////////////////
        //internal void resetCamera(Vector3 initialPosition, Vector3 initialTarget)
        //{
        //    //Vector3 cameraPosition = new Vector3(10, 10, CAMERA_DISTANCE);
        //    //Vector3 cameraTarget = new Vector3(0, 0, 0);
        //    Vector3 cameraPosition = initialPosition;
        //    Vector3 cameraTarget = initialTarget;
        //    Vector3 cameraUp = new Vector3(0, 1, 0);

        //    refOrbitCamera.lookAt(cameraPosition, cameraTarget);
        //    //camera.lookAt(cameraPosition, cameraTarget, cameraUp);
        //    Console.WriteLine(OrbitCameraUtils.PrintMatrix4(refOrbitCamera.getMatrix()));
        //    Console.WriteLine("distance: {0}", refOrbitCamera.getDistance());
        //}

        ///////////////////////////////////////////////////////////////////////////////
        // reset camera to default
        ///////////////////////////////////////////////////////////////////////////////
        internal void resetCamera()
        {
            Vector3 cameraPosition = new Vector3(10, 10, CAMERA_DISTANCE);
            Vector3 cameraTarget = new Vector3(0, 0, 0);

            Vector3 cameraUp = new Vector3(0, 1, 0); 
            //camera.lookAt(cameraPosition, cameraTarget, cameraUp);
            Console.WriteLine(OrbitCameraUtils.PrintMatrix4(refOrbitCamera.getMatrix()));
            Console.WriteLine("distance: {0}", refOrbitCamera.getDistance());
        }


        // utility function for checking shader compilation/linking errors.
        // ------------------------------------------------------------------------
        private void checkCompileErrors(int shader, string type)
        {
            string name = "shader";

            if (type != "PROGRAM")
            {
                GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
                if (code != (int)All.True)
                {
                    var infoLog = GL.GetShaderInfoLog(shader);
                    throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
                }
            }
            else
            {
                GL.GetProgram(shader, GetProgramParameterName.LinkStatus, out var code);
                if (code != (int)All.True)
                {
                    var info = GL.GetProgramInfoLog(shader);
                    Debug.WriteLine($"GL.LinkProgram had info log [{name}]:\n{info}");

                }
            }
        }
    }
}
