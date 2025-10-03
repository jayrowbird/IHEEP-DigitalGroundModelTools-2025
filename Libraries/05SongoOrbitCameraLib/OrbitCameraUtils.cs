using OpenTK.Mathematics;
using System.Diagnostics;
using System.Text;

namespace Songo.OrbitCamera
{
    public class OrbitCameraUtils
    {
        public static string PrintMatrix4(Matrix4 m)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("[{0,10}{1,10}{2,10}{3,10}]",
                                        m.M11, m.M12, m.M13, m.M14));
            sb.AppendLine(string.Format("[{0,10}{1,10}{2,10}{3,10}]",
                                        m.M21, m.M22, m.M23, m.M24));
            sb.AppendLine(string.Format("[{0,10}{1,10}{2,10}{3,10}]",
                                        m.M31, m.M32, m.M33, m.M34));
            sb.AppendLine(string.Format("[{0,10}{1,10}{2,10}{3,10}]",
                                        m.M41, m.M42, m.M43, m.M44));

            return sb.ToString();
        }
        ///////////////////////////////////////////////////////////////////////////////
        // display info messages
        ///////////////////////////////////////////////////////////////////////////////
        void showInfo(OrbitCamera orbitCamera)
        {

            // print camera info
            Console.WriteLine("     Angle: ", orbitCamera.getAngle());

            Console.WriteLine("Quaternion: ", orbitCamera.getQuaternion());

            Console.WriteLine("  Position: ", orbitCamera.getPosition());

            Console.WriteLine("    Target: ", orbitCamera.getTarget());

            Console.WriteLine("   Left Axis: ", orbitCamera.getLeftAxis());

            Console.WriteLine("     Up Axis: ", orbitCamera.getUpAxis());

            Console.WriteLine("Forward Axis: ", orbitCamera.getForwardAxis());

            Console.WriteLine("Use arrow keys to shift camera.");

            Console.WriteLine("Use +/- keys to move forward/backward.");
        }

        public void TimerStart()
        {
            timer.Start();
        }

        public long TimerElapsedMilliseconds()
        {
           return timer.ElapsedMilliseconds;
        }

        ///////////////////////////////////////////////////////////////////////////////
        // display frame rates
        ///////////////////////////////////////////////////////////////////////////////
        private Stopwatch timer = new Stopwatch();
        private int count = 0;
        private string fps = "0.0 FPS";
        private void showFPS()
        {
            //static Timer timer;
            //static int count = 0;
            //static std::string fps = "0.0 FPS";
            double elapsedTime = 0.0;

            ++count;
            // update fps every second
            elapsedTime = timer.ElapsedMilliseconds / 1000.0;
            if (elapsedTime >= 1.0)
            {
                timer.Stop();
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("{0:0.0} {1}", (count / elapsedTime), " FPS"));

                fps = sb.ToString();
                count = 0;                      // reset counter
                timer.Start();                  // restart timer
            }
            Console.WriteLine("==> {0}", fps);
        } 
    }
}

//Console.WriteLine("{0,10}{1,10}{2,10}{3,10}{4,10}",
//  customer[DisplayPos],
//  sales_figures[DisplayPos],
//  fee_payable[DisplayPos],
//  seventy_percent_value,
//  thirty_percent_value);

//inline std::ostream & operator <<(std::ostream& os, const Matrix4& m)
//{
//    os << std::fixed << std::setprecision(5);
//    os << "[" << std::setw(10) << m[0] << " " << std::setw(10) << m[4] << " " << std::setw(10) << m[8] << " " << std::setw(10) << m[12] << "]\n"
//       << "[" << std::setw(10) << m[1] << " " << std::setw(10) << m[5] << " " << std::setw(10) << m[9] << " " << std::setw(10) << m[13] << "]\n"
//       << "[" << std::setw(10) << m[2] << " " << std::setw(10) << m[6] << " " << std::setw(10) << m[10] << " " << std::setw(10) << m[14] << "]\n"
//       << "[" << std::setw(10) << m[3] << " " << std::setw(10) << m[7] << " " << std::setw(10) << m[11] << " " << std::setw(10) << m[15] << "]\n";
//    os << std::resetiosflags(std::ios_base::fixed | std::ios_base::floatfield);
//    return os;
//}
