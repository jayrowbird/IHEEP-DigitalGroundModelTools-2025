// Ignore Spelling: Framebuffer


using HeepWare.Contouring;
using HeepWare.ModelPicking.Framebuffer;
using HeepWare.OBJ.Mesh.Data;
using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Songo.OrbitCamera;
using Songo.OrbitCamera.SGI.Maths;
using System.Diagnostics;
using System.Runtime.InteropServices;
using TriangleNet;
using TriangleNet.Data;
using TriangleNet.Geometry;
using TriangleNet.IO;
using Keys = OpenTK.Windowing.GraphicsLibraryFramework.Keys;

namespace OpenTKDTMContours
{
    public class WinOpenTKNGIViewer
    {
        private readonly bool DEBUGLOG = false;
        private GLControl glControl;
        private System.Windows.Forms.TextBox LogTextBox;
        private Form1 mainWindowForm;

        // The glControl did not trigger key board events
        // so i chose to use the glcontrol native input interface  
        private INativeInput? _nativeInput = null;

        // Below the Opengl api debug call back delegate 
        private static DebugProc DebugMessageDelegate = OnDebugMessage;

        private System.Windows.Forms.Timer rendererTimer;
        private Stopwatch rendererClock;

        private int screenWidth;
        private int screenHeight;

        //Added for orbit camera
        private SGIMath sgimath = new SGIMath();
        // global variables

        private bool mouseRightDown = false;
        private bool mouseLeftDown = false;
        private bool plusKeyDown = false;
        private bool minusKeyDown = false;
        //shift key down mouse click selects element
        private bool shiftKeyDown = false;
        private bool leftKeyDown = false;
        private bool rightKeyDown = false;
        private bool upKeyDown = false;
        private bool downKeyDown = false;
        private float mouseX, mouseY = float.MinValue;
        private float mouseDownX, mouseDownY = float.MinValue;
        private int drawMode;

        //Camera variables
        private Vector3 cameraAngle;
        private OrbitCamera? orbitCamera;
        private Quaternion cameraQuat;

        private bool DEBUGFLAG = false;
        List<MeshObject>? meshObjs;


        FastNGIRenderer? fastNGIRenderer = null;
        private Mesh? existingGroundmesh = null;
        private Mesh? proposedGroundmesh = null;

        private int currentModelId = -1;
        private int shaderId = -1;

        private GroundRender? existingGroundRender = null;
        private GroundRender? proposedGroundRender = null;
        private GroundRender? prismsGroundRender = null;
        private GroundRender? modelsRender = null;

        public WinOpenTKNGIViewer(Form mainWindowForm_)
        {
            mainWindowForm = (Form1)mainWindowForm_;
            LogTextBox = mainWindowForm.LogTextBox;
            glControl = mainWindowForm.glControl;

            //Set initial OpenTK viewer height and width
            screenWidth = glControl.Width;
            screenHeight = glControl.Height;

            //// must use the native input controls because the win forms
            //// input events do work for the game engine window

            InitNativeInputs();

            //Game Clock running continually
            rendererClock = new Stopwatch();
            rendererClock.Start();

            //Render event generator
            rendererTimer = new System.Windows.Forms.Timer();
            rendererTimer.Tick += (sender, e) => { glControl.Invalidate(); };
            rendererTimer.Interval = 50;
            rendererTimer.Start();

            glControl.Paint += GlControl_Paint;
            glControl.Resize += GlControl_Resize;

            // Log any focus changes for debugging.
            if (true)
            {
                glControl.GotFocus += (sender, e) =>
                 { if (true) Log("Focus in"); };
                glControl.LostFocus += (sender, e) =>
                {
                    if (DEBUGLOG) Log("Focus out");
                };
            }

            ShaderSeparateFiles shader = new ShaderSeparateFiles("Shaders\\OffScreenFramebuffer\\TestVertex.shader", "Shaders\\OffScreenFramebuffer\\TestFragment.shader");
            shader.Bind();
            shaderId = shader.Handle;

            //fastNGIRenderer = new FastNGIRenderer(mainWindowForm );
            //orbitCamera = fastNGIRenderer.GetOrbitCamera();

            GL.DebugMessageCallback(DebugMessageDelegate, IntPtr.Zero);
            GL.Enable(EnableCap.DebugOutput);
        }

        private void EnableComputeVolumes()
        {
            if (existingGroundmesh != null && proposedGroundmesh != null)
            {
                mainWindowForm.computeVolumesToolStripMenuItem.Enabled = true;
            }
            else
            {
                mainWindowForm.computeVolumesToolStripMenuItem.Enabled = false;
            }
        }
         

        public void ComputeExistingGroundContours()
        {
            if (existingGroundmesh != null)
            {
               if( existingGroundmesh.Triangles.Count < 1)
                {
                    //existingGroundmesh.Triangulate();
                }
                    ContoursV2 contours = new ContoursV2(5, 1, existingGroundmesh.Triangles);
                 
                Color4 color = Color4.BlueViolet; color.A = 0.25f;
                existingGroundRender?.LoadContoursLineStrings(contours.GetAllLineStrings(), color);
            }
        }

        public void ComputeProposedGroundContours()
        {
            if (proposedGroundmesh != null)
            {
                ContoursV2 contours = new ContoursV2(5, 1, proposedGroundmesh.Triangles);
                
                Color4 color = Color4.DarkOrange; color.A = 0.25f;
                proposedGroundRender.LoadContoursLineStrings(contours.GetAllLineStrings(), color);
            }
        }

        public  void ComputeVolumes()
        {
            if (existingGroundmesh == null || proposedGroundmesh == null) return  ;

            LocateResult findresult;
            Triangle foundTri = new Triangle();

            double TotalVolume = 0.0;
            double TotalArea = 0.0;
            //Store the geometry for the prismoidal volume shapes
            List<List<Vertex>> prisms = new List<List<Vertex>>();
            for (int n = 0; n < proposedGroundmesh.Triangles.Count; n++)
            {
                Triangle tri1 = proposedGroundmesh.Triangles.ElementAt(n);
                double? zlength = 0;
                //find the zlength for each of the existing triangle points
                List<Vertex> vertices = new List<Vertex>();
                for (int m = 0; m < 3; m++)
                {
                    Vertex v = tri1.GetVertex(m);
                    vertices.Add(v);
                    findresult = existingGroundmesh.FindContainingTriangle(v, ref foundTri);
                    if (findresult == LocateResult.Outside)
                    {
                        Console.WriteLine("point outside mesh bounds");
                        continue;
                    }
                    // point projected to the triangle plane is returned
                    Vertex v12 = TriangleMathVertex.ProjectPointOnTrianglePlane(v, foundTri.GetVertex(0), foundTri.GetVertex(1), foundTri.GetVertex(2));
                    vertices.Add(v12);
                    double z12 = Math.Abs(v12.Z - v.Z);
                    zlength += z12;
                }
                prisms.Add(vertices);

                zlength /= 3;
                List<Vertex> pts1 = tri1.GetVertices();
                //Compute the area of the triangle perpendicular to the z or elevation axis
                double area = TriangleMathVertex.triArea(pts1[0], pts1[1], pts1[2]);
                //triangle prism is the triangle area * the average of the 3 heights at the triangle points
                TotalVolume += area * (double)zlength;
                TotalArea += area;
            }
            prismsGroundRender = new GroundRender("Prisms", prisms, shaderId);

            fastNGIRenderer = new FastNGIRenderer(mainWindowForm, shaderId, existingGroundRender, proposedGroundRender, null, prismsGroundRender);

            orbitCamera = fastNGIRenderer.GetOrbitCamera();
            EnableComputeVolumes(); 
        }

        public void ShowExistingTriangles(bool value)
        {
            fastNGIRenderer?.ShowExistingTriangles(value);
        }
        public void ShowExistingPoints(bool value)
        {
            fastNGIRenderer?.ShowExistingPoints(value);
        }
        public void ShowExistingContours(bool value)
        {
            fastNGIRenderer?.ShowExistingContours(value);
        }

        public void ShowProposedTriangles(bool value)
        {
            fastNGIRenderer?.ShowProposedTriangles(value);
        }
        public void ShowProposedPoints(bool value)
        {
            fastNGIRenderer?.ShowProposedPoints(value);
        }
        public void ShowProposedContours(bool value)
        {
            fastNGIRenderer?.ShowProposedContours(value);
        }

        public void LoadExistingGroundFromFile(string filename)
        {
            //Load data for Delaunay triangulation
            List<ITriangle> triangles;
            InputGeometry geometry;

            FileReader.ReadOBJFile(filename, out geometry, out triangles);

            if (geometry != null && geometry.Count > 0 && triangles != null && triangles.Count > 0)
            {
                existingGroundmesh = new Mesh();
                //existingGroundmesh.Load(geometry, triangles);
                existingGroundmesh.Triangulate(geometry);
                
                //Draw triangles
            }
            else if (geometry != null && geometry.Count > 0)
            {
                existingGroundmesh = new Mesh();
                existingGroundmesh.Triangulate(geometry); 
            }
            else
            {
                throw new Exception(string.Format("Failed reading existing ground file '{0}'", filename));
            }
            //--------------------------------------------------------------------------
            //load data for rendering 
            FastOBJFileNGILoad? fastOBJFileNGILoad = new FastOBJFileNGILoad(Color4.DarkGreen, Color4.Yellow, Color4.DarkOliveGreen);
            List<RenderPrimitiveType> renderPrimitiveTypes = fastOBJFileNGILoad.LoadFile(filename);

            existingGroundRender = new GroundRender("Existing Ground", fastOBJFileNGILoad, shaderId);
            fastOBJFileNGILoad = null;  //free the wave front file loader memory

            fastNGIRenderer = new FastNGIRenderer(mainWindowForm, shaderId, existingGroundRender, proposedGroundRender, null, prismsGroundRender);
            
            fastNGIRenderer?.LoadExistingGroundTriangles(existingGroundmesh);

            orbitCamera = fastNGIRenderer.GetOrbitCamera();
            EnableComputeVolumes();
        }


        public void LoadProposedGroundFromFile(string filename)
        {
            //Load data for Delaunay triangulation
            List<ITriangle> triangles;
            InputGeometry geometry;

            FileReader.ReadOBJFile(filename, out geometry, out triangles);

            if (geometry != null && geometry.Count > 0 && triangles != null && triangles.Count > 0)
            {
                proposedGroundmesh = new Mesh();
                //proposedGroundmesh.Load(geometry, triangles);
                proposedGroundmesh.Triangulate(geometry);
            }
            else if (geometry != null && geometry.Count > 0)
            {
                proposedGroundmesh = new Mesh();
                proposedGroundmesh.Triangulate(geometry);
            }
            else
            {
                throw new Exception(string.Format("Failed reading proposed ground file '{0}'", filename));
            }
            //--------------------------------------------------------------------------
            //load data for rendering 
            FastOBJFileNGILoad? fastOBJFileNGILoad = new FastOBJFileNGILoad(Color4.DarkBlue, Color4.Yellow, Color4.DarkTurquoise);
            List<RenderPrimitiveType> renderPrimitiveTypes = fastOBJFileNGILoad.LoadFile(filename);

            proposedGroundRender = new GroundRender("Proposed Ground", fastOBJFileNGILoad, shaderId);
            fastOBJFileNGILoad = null;  //free the wave front file loader memory

            fastNGIRenderer = new FastNGIRenderer(mainWindowForm, shaderId, existingGroundRender, proposedGroundRender, null, prismsGroundRender);

            fastNGIRenderer?.LoadProposedGroundTriangles(proposedGroundmesh);

            orbitCamera = fastNGIRenderer.GetOrbitCamera();
            EnableComputeVolumes();
        }

        public void UseOrthometricCamera()
        {
            fastNGIRenderer = new FastNGIRenderer(mainWindowForm, shaderId, existingGroundRender, proposedGroundRender, null, prismsGroundRender);

            orbitCamera = fastNGIRenderer.GetOrbitCamera();
            EnableComputeVolumes();
        }


        public void LoadModelsFromFile(string filename)
        {
            //--------------------------------------------------------------------------
            //load data for rendering 
            FastOBJFileNGILoad fastOBJFileNGILoad = new FastOBJFileNGILoad();
            List<RenderPrimitiveType> renderPrimitiveTypes = fastOBJFileNGILoad.LoadFile(filename);
            if (fastNGIRenderer != null)
            {
                //fastNGIRenderer = new FastNGIRenderer(mainWindowForm, shaderId, existingGroundRender, proposedGroundRender, modelsRender);
            }
        }
         

        public void SelectCurrentModel()
        {
            if (fastNGIRenderer != null)
            {
                SelectCurrentModel();
                currentModelId = fastNGIRenderer.GetSelectedModelId();
            }
        }


        private void InitNativeInputs()
        {
            glControl.Focus();

            _nativeInput = glControl.EnableNativeInput();

            _nativeInput.KeyDown += _nativeInput_KeyDown;
            _nativeInput.KeyUp += _nativeInput_KeyUp;

            _nativeInput.MouseDown += _nativeInput_MouseDown;
            _nativeInput.MouseUp += _nativeInput_MouseUp;

            _nativeInput.MouseWheel += _nativeInput_MouseWheel;

            _nativeInput.MouseMove += _nativeInput_MouseMove;

            glControl.KeyDown += GlControl_KeyDown;
        }

        private void GlControl_KeyDown(object? sender, KeyEventArgs e)
        {
            throw new NotImplementedException();
        } 

        private void _nativeInput_MouseWheel(MouseWheelEventArgs obj)
        {
            OnMouseWheelGL(obj);
        }

        private void _nativeInput_KeyUp(KeyboardKeyEventArgs obj)
        {
            OnKeyUpGL(obj);
        }

        protected void _nativeInput_KeyDown(KeyboardKeyEventArgs obj)
        {
            if (DEBUGLOG) Log($"  Key Down");

            OnKeyDownGL(obj);
        }

        private void _nativeInput_MouseUp(MouseButtonEventArgs obj)
        {
            OnMouseUpGL(obj);
        }

        private void _nativeInput_MouseDown(MouseButtonEventArgs obj)
        {
            OnMouseDownGL(obj);
        }

        private void _nativeInput_MouseMove(MouseMoveEventArgs obj)
        {
            OnMouseMoveGL(obj);
        }

        protected void OnResizeGL(ResizeEventArgs e)
        {
            //base.OnResize(e); 
            if (fastNGIRenderer != null)
                fastNGIRenderer.OnResizeGL(e);
            return;
        }
        protected void OnRenderFrameGL(double time) //time in milliseconds
        {
            if (fastNGIRenderer == null) return;
            fastNGIRenderer.OnRenderFrameGL(time);
            return;
        }

        protected void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            //base.OnFramebufferResize(e);
            if (fastNGIRenderer != null)
                fastNGIRenderer.OnFramebufferResize(e);
            return;
        }

        protected void OnKeyUpGL(KeyboardKeyEventArgs e)
        {
            //base.OnKeyUp(e);

            if (e.Key == Keys.Equal || e.Key == Keys.KeyPadAdd)
            {
                plusKeyDown = false;
                if (orbitCamera != null)
                {
                    orbitCamera.stopForward();
                    if (DEBUGFLAG) Console.WriteLine("main::orbitCamera.stopForward");
                }
                else
                {
                    throw new Exception("Orbit Camera is null");
                }
            }
            else if (e.Key == Keys.LeftShift || e.Key == Keys.RightShift)
            {
                shiftKeyDown = false;
                return;
            }
            else if (e.Key == Keys.Minus)
            {
                minusKeyDown = false;
                if (orbitCamera != null)
                {
                    orbitCamera.stopForward();
                    if (DEBUGFLAG) Console.WriteLine("main::orbitCamera.stopForward");
                }
                else
                {
                    throw new Exception("Orbit Camera is null");
                }
            }
        }
        protected void OnKeyDownGL(KeyboardKeyEventArgs e)
        {
            //base.OnKeyDown(e);

            if (e.Key == Keys.Escape)
            {
                //this.Close();
                return;
            }
            else if (e.Key == Keys.LeftShift || e.Key == Keys.RightShift)
            {
                shiftKeyDown = true;
                return;
            }
            else if (e.Key == Keys.Space)
            {
                return;
            }
            else if (e.Key == Keys.R)
            {
                if (orbitCamera != null)
                {
                    orbitCamera.resetCamera();
                }
                else
                {
                    throw new Exception("Orbit Camera is null");
                }
                return;
            }
            else if (e.Key == Keys.D)
            {
                drawMode = ++drawMode % 3;
                if (orbitCamera != null)
                {
                    orbitCamera.SetDrawingMode(drawMode);
                }
                else
                {
                    throw new Exception("Orbit Camera is null");
                }
                return;
            }
            else if (e.Key == Keys.Equal || e.Key == Keys.KeyPadAdd)
            {
                //camera.moveForward( 1.0f);
                //camera.moveForward( 1.0f, 0.5f, Gil::EASE_OUT);
                if (!plusKeyDown)
                {
                    plusKeyDown = true;
                    if (orbitCamera != null)
                    {
                        orbitCamera.startForward(SGIMath.MOVE_SPEED, SGIMath.MOVE_ACCEL);
                        if (DEBUGFLAG) Console.WriteLine("main::orbitCamera.startForward");
                    }
                    else
                    {
                        throw new Exception("Orbit Camera is null");
                    }
                }
                return;
            }
            else if (e.Key == Keys.Minus)
            {
                //camera.moveForward( 1.0f);
                //camera.moveForward( 1.0f, 0.5f, Gil::EASE_OUT);
                if (!minusKeyDown)
                {
                    minusKeyDown = true;
                    if (orbitCamera != null)
                    {
                        orbitCamera.startForward(-SGIMath.MOVE_SPEED, SGIMath.MOVE_ACCEL);
                    }
                    else
                    {
                        throw new Exception("Orbit Camera is null");
                    }
                }
                return;
            }
        }

        protected void OnMouseWheelGL(MouseWheelEventArgs e)
        {
            if (e.Offset.Y > 0)
            {
                if (orbitCamera != null)
                {
                    if (DEBUGFLAG) Console.WriteLine(" {0}  e.Offset.Y > 0", e.OffsetY);
                    orbitCamera.moveForwardPercent(-.15f, .5f, AnimationMode.EASE_OUT);
                }
                else
                {
                    return;
                    throw new Exception("Orbit Camera is null");
                }
            }
            else if (e.Offset.Y < 0)
            {
                if (orbitCamera != null)
                {
                    if (DEBUGFLAG) Console.WriteLine(" {0}  e.Offset.Y < 0", e.OffsetY);
                    orbitCamera.moveForwardPercent(.15f, .5f, AnimationMode.EASE_OUT);
                }
                else
                {
                    return;
                    throw new Exception("Orbit Camera is null");
                }
            }
        }
        protected void OnMouseUpGL(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                this.mouseLeftDown = false;
            }
            else if (e.Button == MouseButton.Right)
            {
                this.mouseRightDown = false;
            }
        }











        // Create a timer with a two second interval.
        System.Windows.Forms.Timer? aTimer;
        // Hook up the Elapsed event for the timer. 



        private int ClickCount = 0;
        private int TickCount = 0;
        private OpenTK.Windowing.Common.MouseButtonEventArgs? refArgs;
        protected void OnMouseDownGL(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Right)
            {
                OnMouseDownSingleGL(e);
            }

            ClickCount++;
            if (ClickCount == 1)
            {
                refArgs = e;
                aTimer = new System.Windows.Forms.Timer();
                aTimer.Tick += ATimer_Tick;
                aTimer.Interval = (int)(SystemInformation.DoubleClickTime * 0.8);
                aTimer.Enabled = true;
            }
            Console.WriteLine("Click count {0}", ClickCount);
        }

        private void ATimer_Tick(object? sender, EventArgs e)
        {
            Console.WriteLine("Tick count {0}", TickCount++);
            Console.WriteLine("Click count {0}", ClickCount);

            if (TickCount > 0)
            {
                if (ClickCount < 2)
                {
                    if ((Control.MouseButtons & MouseButtons.Left) != 0)
                    {
                        //mouse button must be down
                        // do nothing if a single click is detected

                        Console.WriteLine("Click count {0}", ClickCount);
                        OnMouseDownSingleGL((OpenTK.Windowing.Common.MouseButtonEventArgs)refArgs); // Implement your single-click logic
                    }
                }
                else
                {
                    Console.WriteLine("Click count {0}", ClickCount);
                    OnMouseDownDoubleGL((OpenTK.Windowing.Common.MouseButtonEventArgs)refArgs); // Implement your double-click logic
                }
                //Cleanup below
                refArgs = null;
                TickCount = 0;
                ClickCount = 0;
                if (aTimer != null)
                {
                    aTimer.Tick -= ATimer_Tick;
                    aTimer = null;
                }
            }
        }

        protected void OnMouseDownDoubleGL(MouseButtonEventArgs e)
        {
            if (fastNGIRenderer != null)
            {
                fastNGIRenderer.SelectCurrentModel();
            }
        }

        protected void OnMouseDownSingleGL(MouseButtonEventArgs e)
        {
            //element selection
            if (shiftKeyDown == true)
            {
                //tell the render to select the current element
                fastNGIRenderer!.SelectCurrentModel();
                return;
            }
            //Convert cursor to mouse position relative to the GLControl

            //Vector2 mouse = glControl.GetMouseState().Position;
            System.Drawing.Point mpt = Control.MousePosition;
            mpt = glControl.PointToClient(mpt);
            //mpt.Y = glControl.Height - mpt.Y;

            Log($"Mouse Down position {mpt.ToString()}");

            //float x = mouse.X;
            //float y = mouse.Y;

            float x = mpt.X;
            float y = mpt.Y;

            mouseX = x;
            mouseY = y;

            //if (button == GLUT_LEFT_BUTTON)
            if (e.Button == MouseButton.Left)
            {
                if (e.IsPressed)
                {
                    if (orbitCamera != null)
                    {
                        cameraAngle = orbitCamera.getAngle(); // get current camera angle
                        cameraQuat = orbitCamera.getQuaternion();

                        mouseDownX = x;
                        mouseDownY = y;
                        mouseLeftDown = true;
                    }
                    else
                    {
                        throw new Exception("Orbit Camera is null");
                    }
                }
            }
            else
            {
                this.mouseLeftDown = false;
            }

            if (e.Button == MouseButton.Right)
            {
                if (e.IsPressed)
                {
                    mouseDownX = x;
                    mouseDownY = y;
                    this.mouseRightDown = true;
                }
            }
            else
            {
                this.mouseRightDown = false;
            }
        }

        protected void OnMouseMoveGL(MouseMoveEventArgs e)
        {
            const float SCALE_ANGLE = 0.2f;
            const float SCALE_SHIFT = 0.2f;
            float prevX = mouseX;
            float prevY = mouseY;

            //Vector2 mouse = glControl.GetMouseState().Position;
            System.Drawing.Point mpt = System.Windows.Forms.Control.MousePosition;
            mpt = glControl.PointToClient(mpt);

            float x = mpt.X;
            float y = mpt.Y;

            if (DEBUGLOG) Log($"glControl Mouse Position : {x}, {y}");
            if (true) Log($"Form Mouse Position : {e.X}, {e.Y}");

            mouseX = x;
            mouseY = y;

            if (this.mouseLeftDown)
            {
                Vector3 delta;
                delta.Y = (mouseX - mouseDownX) * SCALE_ANGLE;
                delta.X = (mouseY - mouseDownY) * SCALE_ANGLE;

                // re-compute camera matrix
                /*
                //@@ using Euler angles
                Vector3 angle;
                angle.x = cameraAngle.x + delta.x;
                angle.y = cameraAngle.y - delta.y;  // must negate for camera
                //camera.rotateTo(angle);
                camera.rotateTo(angle, 0.5f, Gil::EASE_OUT);
                */

                //@@ using quaternion
                Quaternion qx = sgimath.QuaternionFromVector(new Vector3(1, 0, 0), delta.X * SGIMath.DEG2RAD * 0.5f);   // rotate along X-axis
                Quaternion qy = sgimath.QuaternionFromVector(new Vector3(0, 1, 0), delta.Y * SGIMath.DEG2RAD * 0.5f);   // rotate along Y-axis
                Quaternion q = qx * qy * cameraQuat;
                //Quaternion q = Quaternion::getQuaternion(delta * DEG2RAD * 0.5f); // quat from angles
                //q *= cameraQuat;
                //camera.rotateTo(q);
                if (orbitCamera != null)
                {
                    orbitCamera.rotateTo(q, 0.5f, AnimationMode.EASE_OUT);
                }
                else
                {
                    throw new Exception("Orbit Camera is null");
                }
                /*
                //@@ using delta angle
                //camera.rotate(delta);
                //camera.rotate(delta, 0.5f, Gil::EASE_OUT);
                */
            }
            if (this.mouseRightDown)
            {
                Vector2 delta;
                delta.X = (mouseX - prevX) * SCALE_SHIFT;
                delta.Y = (mouseY - prevY) * SCALE_SHIFT;
                if (orbitCamera != null)
                {
                    orbitCamera.shift(delta, 0.5f, AnimationMode.EASE_OUT);
                }
                else
                {
                    throw new Exception("Orbit Camera is null");
                }
            }
        }
        private void Log(string message)
        {
            LogTextBox.AppendText(message + "\r\n");
        }

        private void GlControl_Resize(object? sender, EventArgs e)
        {
            glControl.MakeCurrent();
            OnResizeGL(new ResizeEventArgs(glControl.ClientSize.Width, glControl.ClientSize.Height));
        }

        private void GlControl_Paint(object? sender, PaintEventArgs e)
        {
            glControl.MakeCurrent();
            //Convert milliseconds to seconds
            OnRenderFrameGL(rendererClock.ElapsedMilliseconds);
        }

        private static void OnDebugMessage(
                DebugSource source,     // Source of the debugging message.
                DebugType type,         // Type of the debugging message.
                int id,                 // ID associated with the message.
                DebugSeverity severity, // Severity of the message.
                int length,             // Length of the string in pMessage.
                IntPtr pMessage,        // Pointer to message string.
                IntPtr pUserParam)      // The pointer you gave to OpenGL, explained later.
        {
            // In order to access the string pointed to by pMessage, you can use Marshal
            // class to copy its contents to a C# string without unsafe code. You can
            // also use the new function Marshal.PtrToStringUTF8 since .NET Core 1.1.
            string message = Marshal.PtrToStringAnsi(pMessage, length);

            // The rest of the function is up to you to implement, however a debug output
            // is always useful.
            Console.WriteLine("[{0} source={1} type={2} id={3}] {4}", severity, source, type, id, message);

            // Potentially, you may want to throw from the function for certain severity
            // messages.
            if (type == DebugType.DebugTypeError)
            {
                throw new Exception(message);
            }
        }
    }
}
