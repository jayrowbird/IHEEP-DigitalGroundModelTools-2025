// Ignore Spelling: Framebuffer Colorbuffer Renderbuffer

using MyOpenTK;
using OpenTK.Graphics.OpenGL4;

namespace HeepWare.ModelPicking.Framebuffer
{
    public class Framebuffer4ModelPicking
    {
        private int framebufferId = -1;

        private int textureModelColorbufferHandle = -1;
        private int textureModelColorbufferUnit = -1;
        private int textureColorbuffer4IdHandle = -1;
        private int rbo = -1;

        private int windowWidth = -1;
        private int windowHeight = -1;

        private ShaderSeparateFiles? screenShader = null;

        private int screenQuadVAO = -1;
        private int quadVBO = -1;

        /// <summary>
        /// Release GPU Memory used by the model frame picking frame buffer
        /// </summary>
        public void CleanupGPUMemory()
        {
            if (screenShader != null)
            {
                GL.DeleteShader(screenShader.Handle); MY.GLCall();
                screenShader = null;
            }

            if (textureModelColorbufferHandle > -1)
            {
                GL.DeleteTexture(textureModelColorbufferHandle); MY.GLCall();
                textureModelColorbufferHandle = -1;
            }
            if (textureModelColorbufferUnit > -1)
            {
                //GL.DeleteTexture(textureModelColorbufferUnit);
                textureModelColorbufferUnit = -1;
            }
            if (textureColorbuffer4IdHandle > -1)
            {
                GL.DeleteTexture(textureColorbuffer4IdHandle); MY.GLCall();
                textureColorbuffer4IdHandle = -1;
            }
            if (rbo > -1)
            {
                GL.DeleteRenderbuffer(rbo); MY.GLCall();
                rbo = -1;
            } 
            if (screenQuadVAO > -1)
            {
                GL.DeleteVertexArray(screenQuadVAO); MY.GLCall();
                screenQuadVAO = -1;
            }
            if (quadVBO > -1)
            {
                GL.DeleteBuffer(quadVBO); MY.GLCall();
                quadVBO = -1;
            }
            if (framebufferId > -1)
            {
                GL.DeleteFramebuffer(framebufferId); MY.GLCall();
                framebufferId = -1;
            }
        }

        public void SetWindowSize(int screenWidth, int screenHeight)
        {
            windowWidth = screenWidth;
            windowHeight = screenHeight;
            LoadFrameBuffer(screenWidth, screenHeight);
        }

        public int GetFramebufferId()
        {
            return framebufferId;
        }

        public int GetTextureModelColorbufferHandle()
        {
            return textureModelColorbufferHandle;
        }

        public int GetTextureModelColorbufferUnit()
        {
            return textureModelColorbufferUnit;
        }

        public int GetTextureColorbuffer4IdHandle()
        {
            return textureColorbuffer4IdHandle;
        }

        public int GetRenderbuffer4DepthIdHandle()
        {
            return rbo;
        }

        public int GetScreenShaderId()
        {
            if (screenShader != null)
            {
                return screenShader.Handle;
            }
            return -1;
        }

        public int GetScreenQuadVAOId()
        {
            return screenQuadVAO;
        }

        public int LoadScreenQuadShader()
        {
            screenShader = new ShaderSeparateFiles("Shaders\\5.1.framebuffers_screen.vs", "Shaders\\5.1.framebuffers_screen.fs");
            //screenShader.Bind();
            return screenShader.Handle;
        }

        public void LoadFrameBuffer(int screenWidth, int screenHeight)
        {
            // build and compile shaders
            // -------------------------
            windowWidth = screenWidth;
            windowHeight = screenHeight;

            if (screenShader == null)
            {
                LoadScreenQuadShader();
            }
            if (screenShader != null)
            {
                screenShader.Bind();
            }


            //Change this moving forward need a way to get the next available texture unit
            textureModelColorbufferUnit = 4;


            // frame buffer configuration
            // -------------------------
            framebufferId = GL.GenFramebuffer();
            Console.WriteLine("framebufferId {0}", framebufferId);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferId);
            // create a color attachment texture
            textureModelColorbufferHandle = GL.GenTexture();
            Console.WriteLine("textureModelColorbuffer {0}", textureModelColorbufferHandle);
            GL.ActiveTexture(TextureUnit.Texture0 + textureModelColorbufferUnit);
            GL.BindTexture(TextureTarget.Texture2D, textureModelColorbufferHandle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, windowWidth, windowHeight, 0, PixelFormat.Rgb, PixelType.UnsignedByte, 0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, textureModelColorbufferHandle, 0);

            // create a model Id color attachment 
            //Utils::AttachColorTexture(m_ColorAttachments[i], m_Specification.Samples, GL_R32I, GL_RED_INTEGER, m_Specification.Width, m_Specification.Height, i);
            GL.ActiveTexture(TextureUnit.Texture0 + 4 + 1);
            textureColorbuffer4IdHandle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureColorbuffer4IdHandle);
            Console.WriteLine("textureColorbuffer4Id {0}", textureColorbuffer4IdHandle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R32i, windowWidth, windowHeight, 0, PixelFormat.RedInteger, PixelType.UnsignedByte, 0);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment1, TextureTarget.Texture2D, textureColorbuffer4IdHandle, 0);

            // create a render buffer object for depth and stencil attachment (we won't be sampling these)
            int rbo = GL.GenRenderbuffer();
            Console.WriteLine("rbo {0}", rbo);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, windowWidth, windowHeight); // use a single renderbuffer object for both a depth AND stencil buffer.
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, rbo); // now actually attach it

            GL.ReadBuffer(ReadBufferMode.ColorAttachment1);

            //make 2 color attachments writable
            GL.DrawBuffers(2,
                new[]
                {
                    DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1
                });
            // now that we actually created the framebuffer and added all attachments we want to check if it is actually complete now
            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                Console.WriteLine("ERROR::FRAMEBUFFER:: Framebuffer is not complete!");

            //Bind default frame buffer for screen drawing
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            if (screenQuadVAO < 0)
            {
                LoadScreenQuad();
            }
        }

        public int LoadScreenQuad()
        {
            float[] quadVertices = QuadVertices();

            // screen quad VAO
            screenQuadVAO = GL.GenVertexArray();
            quadVBO = GL.GenBuffer();

            GL.BindVertexArray(screenQuadVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, quadVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Length * sizeof(float), quadVertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), (2 * sizeof(float)));

            GL.BindVertexArray(0);

            return screenQuadVAO;
        }

        public float[] QuadVertices()
        {
            float[] quadVertices = { // vertex attributes for a quad that fills the entire screen in Normalized Device Coordinates.
                                    // positions   // texCoords
                                    -1.0f,  1.0f,  0.0f, 1.0f,
                                    -1.0f, -1.0f,  0.0f, 0.0f,
                                     1.0f, -1.0f,  1.0f, 0.0f,

                                    -1.0f,  1.0f,  0.0f, 1.0f,
                                     1.0f, -1.0f,  1.0f, 0.0f,
                                     1.0f,  1.0f,  1.0f, 1.0f
                                };
            return quadVertices;
        }
    }
}
