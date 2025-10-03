// Ignore Spelling: Vertices Framebuffer

using HeepWare.IFC.Catalog;
using HeepWare.Mesh.Utilities;
using HeepWare.ModelPicking.Framebuffer;
using HeepWare.OBJ.Mesh.Data;
using MyOpenTK;
using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Songo.OrbitCamera;
using System.Text;

namespace OpenTKDTMContours
{
    /// <summary>
    /// 
    /// </summary>
    public class FastNGIRenderer
    {
        bool DEBUGLOG = false;
        private readonly TextBox LogTextBox;

        private MeshUtils meshUtils;

        private bool USE_ORTHO_CAMERA = false;

        internal HighLightModelRenderer? highLightModelRenderer = null;
        private List<MeshObject>? models = null;

        //below is the mapping of the IFC guid and OpenGL render guid
        IFCCatalog ifcCatalog = IFCCatalog.Instance;

        //private ShaderSeparateFiles? shader;

        private GroundRender? refExistingGroundRender = null;
        private GroundRender? refProposedGroundRender = null;
        private GroundRender? refModelsRender = null;
        private GroundRender? refPrismsGroundRender = null;

        private OrbitCamera? orbitCamera = null;
        private Vector3 m_sceneCenter, m_sceneMin, m_sceneMax;

        private Matrix4 target;
        private Matrix4 projection;
        private Matrix4 mvp;

        private int framebufferId = -1;
        private int shaderId = -1;
        private int screenShaderID = -1;

        private int screenQuadVAO = -1;

        private int textureModelColorbuffer = -1;
        private int textureModelColorbufferUnit = -1;

        private int currentModelId = -2;

        private GLControl refGlControl;

        private int screenWidth;
        private int screenHeight;

        //-----------------------  
        //private float _z = -2.7f;
        private float playTime;     // sec
        //-----------------------

        //Moving Frame buffer creation to a class lib
        private Framebuffer4ModelPicking? framebuffer4ModelPicking = null;
        //-------------------------


        private Vector3 cameraPosition;
        private Vector3 targetPosition;

        private RenderPrimitiveType renderPrimitiveType = RenderPrimitiveType.NA;

        private FastOBJFileNGILoad? fastOBJFileNGILoad = null;
        private List<RenderPrimitiveType>? renderPrimitiveTypes = null;

        Form1 refMainWindowForm;
        public FastNGIRenderer(Form1 mainWindowForm_, int shaderID, GroundRender? existing, GroundRender? proposed, GroundRender? model, GroundRender? prisms ,bool useortho = false)
        {
            USE_ORTHO_CAMERA = useortho;
            meshUtils = new MeshUtils();
            // reference to main viewer form so we can up date the tile with fps
            refMainWindowForm = mainWindowForm_;
            shaderId = shaderID;
            //reference to the main viewer forms GLcontrol so we can handle resize events
            refGlControl = refMainWindowForm.glControl;
            // reference to the log text box panel so we can display error text messages
            LogTextBox = refMainWindowForm.LogTextBox;

            refExistingGroundRender = existing;
            refProposedGroundRender = proposed;
            refModelsRender = model;
            refPrismsGroundRender = prisms ;

            if (refExistingGroundRender != null)
            {
                m_sceneCenter = refExistingGroundRender.GetCenter();
                m_sceneMax = refExistingGroundRender.GetMax();
                m_sceneMin = refExistingGroundRender.GetMin();
            }

            if (refProposedGroundRender != null)
            {
                m_sceneCenter = refProposedGroundRender.GetCenter();
                m_sceneMax = refProposedGroundRender.GetMax();
                m_sceneMin = refProposedGroundRender.GetMin();
                //Vector3 max = refProposedGroundRender.GetMax();
                //Vector3 min = refProposedGroundRender.GetMin();
                //m_sceneMax.X = Math.Max(m_sceneMax.X, max.X);
                //m_sceneMax.Y = Math.Max(m_sceneMax.Y, max.Y);
                //m_sceneMax.Z = Math.Max(m_sceneMax.Z, max.Z);

                //m_sceneMin.X = Math.Min(m_sceneMin.X, max.X);
                //m_sceneMin.Y = Math.Min(m_sceneMin.Y, max.Y);
                //m_sceneMin.Z = Math.Min(m_sceneMin.Z, max.Z);

                //m_sceneCenter.X = (m_sceneMax.X + m_sceneMin.X) / 2.0f;
                //m_sceneCenter.Y = (m_sceneMax.Y + m_sceneMin.Y) / 2.0f;
                //m_sceneCenter.Z = (m_sceneMax.Z + m_sceneMin.Z) / 2.0f;
            }

            if (refModelsRender != null)
            {
                m_sceneCenter = refModelsRender.GetCenter();
                m_sceneMax = refModelsRender.GetMax();
                m_sceneMin = refModelsRender.GetMin();
                //Vector3 max = refModelsRender.GetMax();
                //Vector3 min = refModelsRender.GetMin();
                //m_sceneMax.X = Math.Max(m_sceneMax.X, max.X);
                //m_sceneMax.Y = Math.Max(m_sceneMax.Y, max.Y);
                //m_sceneMax.Z = Math.Max(m_sceneMax.Z, max.Z);

                //m_sceneMin.X = Math.Min(m_sceneMin.X, max.X);
                //m_sceneMin.Y = Math.Min(m_sceneMin.Y, max.Y);
                //m_sceneMin.Z = Math.Min(m_sceneMin.Z, max.Z);

                //m_sceneCenter.X = (m_sceneMax.X + m_sceneMin.X) / 2.0f;
                //m_sceneCenter.Y = (m_sceneMax.Y + m_sceneMin.Y) / 2.0f;
                //m_sceneCenter.Z = (m_sceneMax.Z + m_sceneMin.Z) / 2.0f;
            }

            if (refPrismsGroundRender != null)
            {
                m_sceneCenter = refPrismsGroundRender.GetCenter();
                m_sceneMax = refPrismsGroundRender.GetMax();
                m_sceneMin = refPrismsGroundRender.GetMin();
                //Vector3 max = refPrismsGroundRender.GetMax();
                //Vector3 min = refPrismsGroundRender.GetMin();
                //m_sceneMax.X = Math.Max(m_sceneMax.X, max.X);
                //m_sceneMax.Y = Math.Max(m_sceneMax.Y, max.Y);
                //m_sceneMax.Z = Math.Max(m_sceneMax.Z, max.Z);

                //m_sceneMin.X = Math.Min(m_sceneMin.X, max.X);
                //m_sceneMin.Y = Math.Min(m_sceneMin.Y, max.Y);
                //m_sceneMin.Z = Math.Min(m_sceneMin.Z, max.Z);

                //m_sceneCenter.X = (m_sceneMax.X + m_sceneMin.X) / 2.0f;
                //m_sceneCenter.Y = (m_sceneMax.Y + m_sceneMin.Y) / 2.0f;
                //m_sceneCenter.Z = (m_sceneMax.Z + m_sceneMin.Z) / 2.0f;
            }

            screenHeight = refGlControl.Height;
            screenWidth = refGlControl.Width; 

            // setup camera
            // setup frame buffer for model picking
            // load and compile shaders
            // create vertex buffers and index buffers
            // enable vertex attributes attributes
            // setup mvp matrices 

            RendererSetup();
            InitializeGPUContext();


        }

        /// <summary>
        /// 
        /// </summary>
        public void CleanUpGPUMemory()
        {
            if (framebuffer4ModelPicking != null)
            {
                framebuffer4ModelPicking.CleanupGPUMemory();
            }

            if (refExistingGroundRender != null)
            {
                refExistingGroundRender.ClearGPUMemory();
                refExistingGroundRender = null;
            }

            if (refProposedGroundRender != null)
            {
                refProposedGroundRender.ClearGPUMemory();
                refProposedGroundRender = null;
            }

            if (refModelsRender != null)
            {
                refModelsRender.ClearGPUMemory();
                refModelsRender = null;
            }

            if (models != null && models.Count > 0)
            {
                models.Clear();
            }

            //Release Screen Quad frame buffer resources
            if (textureModelColorbuffer > 0)
            {
                GL.DeleteTexture(textureModelColorbuffer); MY.GLCall();
            }
            if (textureModelColorbuffer > 0)
            {
                GL.DeleteTexture(textureModelColorbuffer); MY.GLCall();
            }
        }
          
        /// <summary>
        /// return a reference to the main orbit camera to the main form viewer
        /// this supports mouse camera events
        /// </summary>
        /// <returns></returns>
        public OrbitCamera? GetOrbitCamera()
        {
            return orbitCamera;
        }

        public int GetSelectedModelId()
        {
            return currentModelId;
        }

        public void SetModels(List<MeshObject> _models)
        {
            models = _models;
        }

        //bool toggle = true;
        public void SelectCurrentModel()
        {
            if (currentModelId != -1 && models != null)
            {
                MeshObject? model = models.Where(x => x.GetId() == currentModelId).FirstOrDefault();
                //load the selected model to the highlight render
                //leave the model highlighted until a new model is selected or currentmodel == -1
                Console.WriteLine("Select model id :: {0}", currentModelId);
                //for testing the shift to target function
                //targetPosition
                //Vector3 newTarget = new Vector3(0, 5, 1);
                if (model != null)
                {
                    Vector3 newTarget = model.GetCenter();
                    orbitCamera!.shiftTo(newTarget, 10.0f, Songo.OrbitCamera.SGI.Maths.AnimationMode.BOUNCE);
                    HighLightModel(model);
                }
            }
            else
            {
                Console.WriteLine("Clear highlight render by selecting no model with shift and mouse click");
            }
        }

        public void ShowExistingTriangles(bool value)
        {
            if(refExistingGroundRender !=  null)
            {
                refExistingGroundRender.ShowTriangles(value);
            }
        }
        public void ShowExistingPoints(bool value)
        {
            if (refExistingGroundRender != null)
            {
                refExistingGroundRender.ShowPoints(value);
            }
        }
        public void ShowExistingContours(bool value)
        {
            if (refExistingGroundRender != null)
            {
                refExistingGroundRender.ShowContours(value);
            }
        }

        public void ShowProposedTriangles(bool value)
        {
            if (refProposedGroundRender != null)
            {
                refProposedGroundRender.ShowTriangles(value);
            }
        }
        public void ShowProposedPoints(bool value)
        {
            if (refProposedGroundRender != null)
            {
                refProposedGroundRender.ShowPoints(value);
            }
        }
        public void ShowProposedContours(bool value)
        {
            if (refProposedGroundRender != null)
            {
                refProposedGroundRender.ShowContours(value);
            }
        }

        public void LoadExistingGroundTriangles(TriangleNet.Mesh existingMesh)
        {
            if (refExistingGroundRender != null)
            {
                Color4 color = Color4.DarkCyan;
                color.A = 0.25f;
                refExistingGroundRender.LoadTriangles(existingMesh, color);
            }
        }

        public void LoadProposedGroundTriangles(TriangleNet.Mesh proposedMesh)
        {
            if (refProposedGroundRender != null)
            {
                Color4 color = Color4.DarkOrange;
                color.A = 0.25f;
                refProposedGroundRender.LoadTriangles(proposedMesh, color);
            }
        }

        public void HighLightModel(MeshObject model)
        {
            highLightModelRenderer = new HighLightModelRenderer(shaderId, model);
        }

        public void DeleteHiHighLightModel()
        {
            if (highLightModelRenderer != null)
            {
                highLightModelRenderer = null;
            }
        }

        private void SetupOrthometricCamera()
        {
            // Define the dimensions of your projection volume
            float left = 0.0f;
            float right = refGlControl.ClientSize.Width; // e.g., width of your window
            float bottom = 0.0f;
            float top = refGlControl.ClientSize.Height;   // e.g., height of your window
            float zNear = 0.1f;   // Near clipping plane
            float zFar = 100000.0f;  // Far clipping plane

            // Create the orthographic projection matrix
            Matrix4 orthoProjectionMatrix = Matrix4.CreateOrthographicOffCenter(left, right, bottom, top, zNear, zFar);
            Vector3 targetPosition = new Vector3(m_sceneCenter.X, m_sceneCenter.Y, m_sceneMax.Z);
            mvp = target * projection;
        }


        internal void RendererSetup()
        {
            //Contours have the same elevation so this will not work
            //float eyeDistance = m_sceneMax.Z - m_sceneMin.Z;
            float zdiff = m_sceneMax.Z - m_sceneMin.Z;
            float xdiff = m_sceneMax.X - m_sceneMin.X;
            float ydiff = m_sceneMax.Y - m_sceneMin.Y;

            float eyeDistance = Math.Max(zdiff, xdiff);
            eyeDistance = Math.Max(eyeDistance, ydiff);

            //----------------------------------------------------------------------------------------------
            orbitCamera = new OrbitCamera(refGlControl.ClientSize.Width, refGlControl.ClientSize.Height, true);
            targetPosition = new Vector3(m_sceneCenter.X, m_sceneCenter.Y, m_sceneMax.Z);
            cameraPosition = new Vector3(m_sceneCenter.X, m_sceneCenter.Y, m_sceneMax.Z + eyeDistance);

            orbitCamera.lookAt(cameraPosition, targetPosition);

            //----------------------------------------------------------------------------------------------
            target = Matrix4.LookAt(cameraPosition, new Vector3(m_sceneCenter.X, m_sceneCenter.Y, m_sceneCenter.Z), new Vector3(0f, 1f, 0f));
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), (float)refGlControl.ClientSize.Width / (float)refGlControl.ClientSize.Height, 0.01f, 100000.0f);

            mvp = target * projection;

            if(USE_ORTHO_CAMERA == true)
            {
                SetupOrthometricCamera();
            }

            //----------------------------------------------------------------------------------------------

            framebuffer4ModelPicking = new Framebuffer4ModelPicking();
            screenQuadVAO = framebuffer4ModelPicking.LoadScreenQuad();
            screenShaderID = framebuffer4ModelPicking.LoadScreenQuadShader();

            // frame buffer configuration
            // -------------------------

            framebuffer4ModelPicking.LoadFrameBuffer(refGlControl.ClientSize.Width, refGlControl.ClientSize.Height);
            framebufferId = framebuffer4ModelPicking.GetFramebufferId();
            textureModelColorbuffer = framebuffer4ModelPicking.GetTextureModelColorbufferHandle();
            textureModelColorbufferUnit = framebuffer4ModelPicking.GetTextureModelColorbufferUnit();
        }

        protected void InitializeGPUContext()
        {
            // the opengl context must be complete before calling any GL. methods  

            GL.Enable(EnableCap.DebugOutput);
            target = Matrix4.LookAt(cameraPosition, new Vector3(m_sceneCenter.X, m_sceneCenter.Y, m_sceneCenter.Z), new Vector3(0f, 1f, 0f));
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), (float)refGlControl.ClientSize.Width / (float)refGlControl.ClientSize.Height, 0.01f, 100000.0f);

            mvp = target * projection;
        }

        internal void WriteGPUBuffers2File(string filename, List<VertexPosColorId> gpuVertices, List<uint> gpuIndexBuffer)
        {
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            StringBuilder sb = new StringBuilder();
            //write gpuVertices out to file
            for (int i = 0; i < gpuVertices.Count; i++)
            {
                sb.AppendLine(string.Format("v : [{0}], c : [{1}], i: [{2}]",
                    gpuVertices[i].position.ToString(),
                    gpuVertices[i].color.ToString(),
                    gpuVertices[i].id));
            }
            File.WriteAllText("GPU_Vertices_two", sb.ToString());

            StringBuilder sb1 = new StringBuilder();
            //write gpuVertices out to file
            for (int i = 0; i < gpuIndexBuffer.Count; i++)
            {
                if (i % 10 == 0)
                {
                    sb1.AppendLine();
                }
                sb1.Append(string.Format(" {0} ", gpuIndexBuffer[i]));
            }
            File.WriteAllText(filename, sb1.ToString());
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        }

        public void OnResizeGL(ResizeEventArgs e)
        {
            //base.OnResize(e); 

            if (e.Width < 50 || e.Height < 50) return;

            if (orbitCamera != null)
            {
                orbitCamera.OnResize(e.Width, e.Height);
            }

            screenHeight = e.Height;
            screenWidth = e.Width;

            GL.Viewport(0, 0, screenWidth, screenHeight); MY.GLCall();

            // frame buffer configuration
            // -------------------------
            // Must recreate the quad frame buffer after every screen resize

            if (framebuffer4ModelPicking != null)
            {
                framebuffer4ModelPicking.LoadFrameBuffer(screenWidth, screenHeight);
                framebufferId = framebuffer4ModelPicking.GetFramebufferId();
                textureModelColorbuffer = framebuffer4ModelPicking.GetTextureModelColorbufferHandle();
                textureModelColorbufferUnit = framebuffer4ModelPicking.GetTextureModelColorbufferUnit();
            }
        }

        Matrix4 view;
        int count = 0;
        public void OnRenderFrameGL(double time) //time in milliseconds
        {
            if (orbitCamera == null) return;

            time = time / 1000.00f;  // milliseconds to seconds
                                     //base.OnRenderFrame(e);

            if (DEBUGLOG) Log($"OnRenderFrameGL [delta time (sec)] {time},  count {count++}");

            // update times
            float elapsedTime = orbitCamera.GetTimerElapsed();
            float frameTime = elapsedTime - playTime;
            playTime = elapsedTime;

            if (DEBUGLOG) Log($"Frame time {frameTime}");
            if (DEBUGLOG) Log($"FPS: {1f / frameTime}");
            refMainWindowForm.Text = $"FPS: {1f / frameTime}";
            // update camera
            orbitCamera.update(frameTime);


            view = orbitCamera.getMatrix();
            mvp = view * projection;

            GL.Enable(EnableCap.DepthTest);
            GL.Viewport(0, 0, screenWidth, screenHeight);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);


            // render
            // ------
            // bind to frame-buffer and draw scene as we normally would to color texture 
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferId);
            GL.Viewport(0, 0, screenWidth, screenHeight);
            GL.Enable(EnableCap.DepthTest); // enable depth testing (is disabled for rendering screen-space quad)

            // make sure we clear the frame-buffer's content
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            int value = -1;
            //Clear the id color buffer attachment      
            GL.ClearBuffer(ClearBuffer.Color, 1, ref value);

            if (true)
            {
                //shader.use();
                GL.UseProgram(shaderId);

                if (refExistingGroundRender != null)
                {
                    refExistingGroundRender.RenderFrameGL(time, mvp);
                }
                if (refProposedGroundRender != null)
                {
                    refProposedGroundRender.RenderFrameGL(time, mvp);
                }
                if (refModelsRender != null)
                {
                    refModelsRender.RenderFrameGL(time, mvp);
                }
                if (refPrismsGroundRender!= null)
                {
                    refPrismsGroundRender.RenderFrameGL(time, mvp);
                    refPrismsGroundRender.ShowPoints(true);
                    refPrismsGroundRender.ShowContours(true);
                    refPrismsGroundRender.ShowTriangles(true); 
                }
            }


            // if there is a current highlight element
            if (highLightModelRenderer != null)
            {
                GL.Disable(EnableCap.DepthTest);
                GL.UseProgram(shaderId);
                // use the same shader for now
                GL.BindVertexArray(highLightModelRenderer.GetVertexArrayId());

                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);
                GL.EnableVertexAttribArray(2);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, highLightModelRenderer.GetGPUIndexBufferId());

                GL.UniformMatrix4(GL.GetUniformLocation(shaderId, "u_ViewProjection"), false, ref mvp);

                GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
                GL.DrawElements(PrimitiveType.Triangles, highLightModelRenderer.GetGPUIndexBufferCount(), DrawElementsType.UnsignedInt, 0); MY.GLCall();
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);


            // read the model that is under the cursor at this moment in time
            // write it out to the console window
            Point mpt = System.Windows.Forms.Control.MousePosition;
            mpt = refGlControl.PointToClient(mpt);
            Vector2 pos = new Vector2(mpt.X, mpt.Y);

            int pixelData = -1;
            pos.Y = screenHeight - pos.Y;
            GL.ReadPixels((int)pos.X, (int)pos.Y, 1, 1, PixelFormat.RedInteger, PixelType.Int, ref pixelData);

            if (pixelData != currentModelId)
            {
                string? guid = ifcCatalog!.GetGuid(pixelData);
                if (DEBUGLOG) Log(string.Format("Model id: {0} at pixel location [{1},{2}]", pixelData, pos.X, pos.Y));
                if (DEBUGLOG) Log(string.Format("Model id: {0} at pixel location [{1},{2}]", guid, pos.X, pos.Y));
                if (true) Console.WriteLine("Model id: {0} at pixel location [{1},{2}]", pixelData, pos.X, pos.Y);
                if (true) Console.WriteLine("Model id: {0} at pixel location [{1},{2}]", guid, pos.X, pos.Y);

                if (true) Log(string.Format("Model id: {0} at pixel location [{1},{2}]", pixelData, pos.X, pos.Y));
                if (true) Log(string.Format("Model id: {0} at pixel location [{1},{2}]", guid, pos.X, pos.Y));
                currentModelId = pixelData;
            }

            orbitCamera.OnRenderFrame();

            //Bind the default frame buffer for drawing the quad
            //now draw the off screen buffer/image to the screen
            //the off screen buffer is required to get fast model ids under the cursor at runtime

            // now bind back to default frame-buffer and draw a quad plane with the attached frame-buffer color texture
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Viewport(0, 0, screenWidth, screenHeight);
            GL.Disable(EnableCap.DepthTest); // disable depth test so screen-space quad isn't discarded due to depth test.

            GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);

            // clear all relevant buffers
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f); // set clear color to white (not really necessary actually, since we won't be able to see behind the quad anyways)
            GL.Clear(ClearBufferMask.ColorBufferBit);

            //screenShader.use();
            GL.UseProgram(screenShaderID);

            GL.BindVertexArray(screenQuadVAO);
            GL.ActiveTexture(TextureUnit.Texture0 + textureModelColorbufferUnit);
            GL.BindTexture(TextureTarget.Texture2D, textureModelColorbuffer);   // use the color attachment texture as the texture of the quad plane
            GL.Uniform1(GL.GetUniformLocation(screenShaderID, "screenTexture"), textureModelColorbufferUnit);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            refGlControl.SwapBuffers();
        }

        public void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            //base.OnFramebufferResize(e);
            screenWidth = e.Width;
            screenHeight = e.Height;

            GL.Viewport(0, 0, screenWidth, screenHeight);
        }

        private void Log(string message)
        {
            LogTextBox.AppendText(message + "\r\n");
        }
    }
}
