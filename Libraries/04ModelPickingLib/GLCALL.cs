using OpenTK.Graphics.OpenGL4;
using System.Text;

namespace MyOpenTK
{

    public class MY
    {
        public static  bool GL_FALSE = false;
        public static bool GL_TRUE = true;

        public static void GLCall<T>(T a)  where T : Type
        {           
            GLCheckError(); GLClearError();
        }
        public static void GLCall()
        {           
            GLCheckError(); GLClearError();
        } 
        public static void GLCall(object o)
        {            
            GLCheckError();GLClearError();
        }

        //public static void GLCall(func<System.Void, System.Void> )
        //{
        //    GLClearError();
        //    GLCheckError();
        //}

        static void GLClearError()
        {
            while (GL.GetError() != ErrorCode.NoError) ;
        }

        static bool GLCheckError()
        {
            ErrorCode glerror;
            while ((glerror = GL.GetError()) != ErrorCode.NoError)
            {
                StringBuilder cout = new StringBuilder();
                cout.Append("[OpenGL Error] ");
                switch (glerror)
                {
                    case ErrorCode.InvalidEnum:
                        cout.Append(ErrorCode.InvalidEnum.ToString() + "  : An unacceptable value is specified for an enumerated argument.");
                        break;
                    case ErrorCode.InvalidValue:
                        cout.Append(ErrorCode.InvalidValue.ToString() + " : A numeric argument is out of range.");
                        break;
                    case ErrorCode.InvalidOperation:
                        cout.Append(ErrorCode.InvalidOperation.ToString() + " : The specified operation is not allowed in the current state.");
                        break;
                    case ErrorCode.InvalidFramebufferOperation:
                        cout.Append(ErrorCode.InvalidFramebufferOperation.ToString() + " : The framebuffer object is not complete.");
                        break;
                    case ErrorCode.OutOfMemory:
                        cout.Append(ErrorCode.OutOfMemory.ToString() + " : There is not enough memory left to execute the command.");
                        break;
                    //case ErrorCode.InvalidFramebufferOperationExt:
                    //    cout.Append(ErrorCode.InvalidFramebufferOperationExt.ToString() + " : An attempt has been made to perform an operation that would cause an internal stack to overflow.");
                    //    break;
                    //case ErrorCode.InvalidFramebufferOperationOes:
                    //    cout.Append(ErrorCode.InvalidFramebufferOperationOes.ToString() + " : An attempt has been made to perform an operation that would cause an internal stack to overflow.");
                    //    break;
                    case ErrorCode.ContextLost:
                        cout.Append(ErrorCode.ContextLost.ToString() + " : An attempt has been made to perform an operation that would cause an internal stack to overflow.");
                        break;
                    case ErrorCode.TableTooLarge:
                        cout.Append(ErrorCode.TableTooLarge.ToString() + " : An attempt has been made to perform an operation that would cause an internal stack to overflow.");
                        break;
                    //case ErrorCode.TableTooLargeExt:
                    //    cout.Append(ErrorCode.TableTooLargeExt.ToString() + " : An attempt has been made to perform an operation that would cause an internal stack to overflow.");
                    //   break;

                    default:
                        cout.Append("Unrecognized error" + glerror.ToString());
                        break;
                }
                cout.AppendLine();
                Console.WriteLine(cout.ToString());

            }
            return true;
        }
    }
}

