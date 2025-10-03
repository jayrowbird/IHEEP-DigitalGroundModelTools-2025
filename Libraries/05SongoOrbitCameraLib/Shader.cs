using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Diagnostics;

namespace SongoOrbitCamera
{
    public class Shader
    {
        int ID;
        string name;
        // constructor generates the shader on the fly
        // ------------------------------------------------------------------------
        public Shader(string vertexPath, string fragmentPath)
        {
            // 1. retrieve the vertex/fragment source code from filePath
            int vertex, fragment;

            name = Path.GetFileNameWithoutExtension(vertexPath);

            string vShaderCode = File.ReadAllText(vertexPath);
            string fShaderCode = File.ReadAllText(fragmentPath); 

            // vertex shader
            vertex = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertex, vShaderCode);
            GL.CompileShader(vertex);
            GL.GetShader(vertex, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                var infoLog = GL.GetShaderInfoLog(vertex);
                throw new Exception($"Error occurred whilst compiling Shader({vertex}).\n\n{infoLog}");
            }
            checkCompileErrors(vertex, "VERTEX");
            // fragment Shader
            fragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragment, fShaderCode);
            GL.CompileShader(fragment);
            GL.GetShader(fragment, ShaderParameter.CompileStatus, out var code2);
            if (code2 != (int)All.True)
            {
                var infoLog = GL.GetShaderInfoLog(fragment);
                throw new Exception($"Error occurred whilst compiling Shader({fragment}).\n\n{infoLog}");
            }
            checkCompileErrors(fragment, "FRAGMENT");
            // shader Program
            ID = GL.CreateProgram();
            GL.AttachShader(ID, vertex);
            Console.WriteLine(GL.GetShaderInfoLog(vertex));
            GL.AttachShader(ID, fragment);
            Console.WriteLine(GL.GetShaderInfoLog(fragment));
            GL.LinkProgram(ID);
            checkCompileErrors(ID, "PROGRAM");
            // delete the shaders as they're linked into our program now and no longer necessary
            GL.DetachShader(ID, vertex);
            GL.DeleteShader(vertex);
            GL.DetachShader(ID, fragment);
            GL.DeleteShader(fragment);
            var programLog = GL.GetProgramInfoLog(ID);
            if (!string.IsNullOrEmpty(programLog)) throw new Exception(programLog);
            GL.UseProgram(0);
        }
        // activate the shader
        // ------------------------------------------------------------------------
       internal void Bind()
        { 
            GL.UseProgram(ID);
        }
        // utility uniform functions
        // ------------------------------------------------------------------------
        internal void setBool(string name, bool value)
        {
            GL.Uniform1(GL.GetUniformLocation(ID, name), value == true ? 0 : 1);
        }
        // ------------------------------------------------------------------------
        internal void setInt(string name, int value)
        {
            int loc = GL.GetUniformLocation(ID, name);
            GL.Uniform1(loc, value);
        }
        // ------------------------------------------------------------------------
        internal void setFloat(string name, float value)
        {
            GL.Uniform1(GL.GetUniformLocation(ID, name), value);
        }
        // ------------------------------------------------------------------------
        internal void setVec2(string name, Vector2 value)
        {
            GL.Uniform2(GL.GetUniformLocation(ID, name), value.X, value.Y);
        }
        internal void setVec2(string name, float x, float y)
        {
            GL.Uniform2(GL.GetUniformLocation(ID, name), x, y);
        }
        // ------------------------------------------------------------------------
        internal void setVec3(string name, Vector3 value)
        {
            GL.Uniform3(GL.GetUniformLocation(ID, name), value.X, value.Y, value.Z);
        }
        internal void setVec3(string name, float x, float y, float z)
        {
            GL.Uniform3(GL.GetUniformLocation(ID, name), x, y, z);
        }
        // ------------------------------------------------------------------------
        internal void setVec4(string name, Vector4 value)
        {
            GL.Uniform4(GL.GetUniformLocation(ID, name), value.X, value.Y, value.Z, value.W);
        }
        internal void setVec4(string name, float x, float y, float z, float w)
        {
            GL.Uniform4(GL.GetUniformLocation(ID, name), x, y, z, w);
        }
        // ------------------------------------------------------------------------
        internal void setMat2(string name, Matrix2 mat)
        {
            GL.UniformMatrix2(GL.GetUniformLocation(ID, name), false, ref mat);
        }
        // ------------------------------------------------------------------------
        internal void setMat3(string name, Matrix3 mat)
        {
            GL.UniformMatrix3(GL.GetUniformLocation(ID, name), false, ref mat);
        }
        // ------------------------------------------------------------------------
        internal void setMat4(string name, Matrix4 mat)
        {
            GL.UniformMatrix4(GL.GetUniformLocation(ID, name), false, ref mat);
        }


        // utility function for checking shader compilation/linking errors.
        // ------------------------------------------------------------------------
        private void checkCompileErrors(int shader, string type)
        {

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
                    var info = GL.GetProgramInfoLog(ID);
                    Debug.WriteLine($"GL.LinkProgram had info log [{name}]:\n{info}");

                }
            }
        }
    }
}
