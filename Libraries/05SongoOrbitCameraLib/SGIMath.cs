using OpenTK.Mathematics;

namespace Songo.OrbitCamera.SGI.Maths
{

    public enum AnimationMode  // enum
    {
        LINEAR = 0,
        EASE_IN,
        EASE_IN2,       // using circle
        EASE_OUT,
        EASE_OUT2,      // using circle
        EASE_IN_OUT,
        EASE_IN_OUT2,   // using circle
        BOUNCE,
        ELASTIC
    };

    public class SGIMath
    {
        public const float DEG2RAD = 3.141593f / 180;
        public const float MOVE_SPEED = 3.0f;             // units per sec
        public const float MOVE_ACCEL = 10.0f;            // units per squared sec

        const bool debugflag = false;
        //const float TRACKBALLSIZE = (0.8f);

        public SGIMath() { }

        public float sqrtf(float v)
        {
            return (float)Math.Sqrt((double)v);
        }

        public float fabs(float v)
        {
            return (float)Math.Abs((double)v);
        }

        public float atan2f(float v, float v1)
        {
            return (float)Math.Atan2((double)v, (double)v1);
        }

        public float asinf(float v)
        {
            return (float)Math.Asin((double)v);
        }

        public float sinf(float v)
        {
            return (float)Math.Sin((double)v);
        }

        public float cosf(float v)
        {
            return (float)Math.Cos((double)v);
        }

        public float acosf(float v)
        {
            return (float)Math.Acos((double)v);
        }

        internal void vzero(out float[] v)
        {
            v = new float[3];
            v[0] = 0.0f;
            v[1] = 0.0f;
            v[2] = 0.0f;
        }
        internal void vset(out float[] v, float x, float y, float z)
        {
            v = new float[3];
            v[0] = x;
            v[1] = y;
            v[2] = z;
        }

        internal void vsub(float[] src1, float[] src2, float[] dst)
        {
            dst[0] = src1[0] - src2[0];
            dst[1] = src1[1] - src2[1];
            dst[2] = src1[2] - src2[2];
        }

        void vcopy(float[] v1, float[] v2)
        {
            int i;
            for (i = 0; i < 3; i++)
                v2[i] = v1[i];
        }
        internal void vcross(float[] v1, float[] v2, float[] cross)
        {
            float[] temp = new float[3];

            temp[0] = (v1[1] * v2[2]) - (v1[2] * v2[1]);
            temp[1] = (v1[2] * v2[0]) - (v1[0] * v2[2]);
            temp[2] = (v1[0] * v2[1]) - (v1[1] * v2[0]);
            vcopy(temp, cross);
        }

        internal float vlength(float[] v)
        {
            return sqrtf(v[0] * v[0] + v[1] * v[1] + v[2] * v[2]);
        }

        void vscale(float[] v, float div)
        {
            v[0] *= div;
            v[1] *= div;
            v[2] *= div;
        }

        void vnormal(float[] v)
        {
            vscale(v, 1.0f / vlength(v));
        }

        float vdot(float[] v1, float[] v2)
        {
            return v1[0] * v2[0] + v1[1] * v2[1] + v1[2] * v2[2];
        }

        void vadd(float[] src1, float[] src2, float[] dst)
        {
            dst[0] = src1[0] + src2[0];
            dst[1] = src1[1] + src2[1];
            dst[2] = src1[2] + src2[2];
        }

        internal Matrix4 getMatrix(Quaternion q)
        {
            // NOTE: assume the quaternion is unit length
            // compute common values
            float x2 = q.X + q.X;
            float y2 = q.Y + q.Y;
            float z2 = q.Z + q.Z;
            float xx2 = q.X * x2;
            float xy2 = q.X * y2;
            float xz2 = q.X * z2;
            float yy2 = q.Y * y2;
            float yz2 = q.Y * z2;
            float zz2 = q.Z * z2;
            float sx2 = q.W * x2;
            float sy2 = q.W * y2;
            float sz2 = q.W * z2;

            // build 4x4 matrix (column-major) and return
            return new Matrix4(1 - (yy2 + zz2), xy2 + sz2, xz2 - sy2, 0, // column 0
                           xy2 - sz2, 1 - (xx2 + zz2), yz2 + sx2, 0, // column 1
                           xz2 + sy2, yz2 - sx2, 1 - (xx2 + yy2), 0, // column 2
                           0, 0, 0, 1);// column 3

            // for non-unit quaternion
            // ss+xx-yy-zz, 2xy+2sz,     2xz-2sy,     0
            // 2xy-2sz,     ss-xx+yy-zz, 2yz-2sx,     0
            // 2xz+2sy,     2yz+2sx,     ss-xx-yy+zz, 0
            // 0,           0,           0,           1
        }

        private bool Vector3sEqual(Vector3 lhs, Vector3 rhs, float epsilon)
        {
            return fabs(lhs.X - rhs.X) < epsilon && fabs(lhs.Y - rhs.Y) < epsilon && fabs(lhs.Z - rhs.Z) < epsilon;
        }

        // find quaternion from 2D rotation angle (ax, ay)
        private Quaternion getQuaternion(Vector2 angles)
        {
            Quaternion qx = QuaternionFromVector(new Vector3(1, 0, 0), angles.X);   // rotate along X
            Quaternion qy = QuaternionFromVector(new Vector3(0, 1, 0), angles.Y);   // rotate along Y
            return qx * qy; // order: y->x
        }

        // find quaternion from 3D rotation angle (ax, ay, az)
        internal Quaternion getQuaternion(Vector3 angles)
        {
            Quaternion qx = QuaternionFromVector(new Vector3(1, 0, 0), angles.X);   // rotate along X
            Quaternion qy = QuaternionFromVector(new Vector3(0, 1, 0), angles.Y);   // rotate along Y
            Quaternion qz = QuaternionFromVector(new Vector3(0, 0, 1), angles.Z);   // rotate along Z
            return qx * qy * qz;    // order: z->y->x
        }

        public Quaternion QuaternionFromVector(Vector3 axis, float angle)
        {
            Quaternion q = new Quaternion();
            // use only half angle because of double multiplication, qpq*,
            // q at the front and its conjugate at the back
            Vector3 v = axis;
            v.Normalize();                  // convert to unit vector
            float sine = sinf(angle);       // angle is radian
            q.W = cosf(angle);
            q.X = v.X * sine;
            q.Y = v.Y * sine;
            q.Z = v.Z * sine;

            return q;
        }

        // find quaternion for rotating from v1 to v2
        internal Quaternion getQuaternion(Vector3 v1, Vector3 v2)
        {
            const float EPSILON = 0.001f;
            float HALF_PI = acosf(-1f) * 0.5f;

            // if two vectors are equal return the vector with 0 rotation
            //if (v1.equal(v2, EPSILON))
            if (Vector3sEqual(v1, v2, EPSILON))
            {
                return new Quaternion(v1, 0);
            }
            // if two vectors are opposite return a perpendicular vector with 180 angle
            // else if (v1.equal(-v2, EPSILON))
            else if (Vector3sEqual(v1, -v2, EPSILON))
            {
                Vector3 v_ = Vector3.Zero;
                if (v1.X > -EPSILON && v1.X < EPSILON)         // if x ~= 0
                    v_ = new Vector3(1, 0, 0);
                else if (v1.Y > -EPSILON && v1.Y < EPSILON)    // if y ~= 0
                    v_ = new Vector3(0, 1, 0);
                else                                        // if z ~= 0
                    v_ = new Vector3(0, 0, 1);
                return new Quaternion(v_, HALF_PI);
            }

            Vector3 u1 = v1;                    // convert to normal vector
            Vector3 u2 = v2;
            u1.Normalize();
            u2.Normalize();

            //Vector3 v = u1.cross(u2);           // compute rotation axis
            Vector3 v = Vector3.Cross(u1, u2);           // compute rotation axis
            float angle = acosf(Vector3.Dot(u1, u2));    // rotation angle
            return new Quaternion(v, angle * 0.5f); // half angle
        }



        /*
        *  Given an axis and angle, compute quaternion.
        */
        internal void axis_to_quat(float[] a, float phi, float[] q)
        {
            vnormal(a);
            vcopy(a, q);
            vscale(q, sinf(phi / 2.0f));
            q[3] = cosf(phi / 2.0f);
        }

        /*
         * Project an x,y pair onto a sphere of radius r OR a hyperbolic sheet
         * if we are away from the center of the sphere.
         */
        internal float tb_project_to_sphere(float r, float x, float y)
        {
            float d, t, z;

            d = sqrtf(x * x + y * y);
            if (d < r * 0.70710678118654752440f)
            {    /* Inside sphere */
                z = sqrtf(r * r - d * d);
            }
            else
            {           /* On hyperbola */
                t = r / 1.41421356237309504880f;
                z = t * t / d;
            }
            return z;
        }

        // interpolate from one point to the other
        // - "alpha" param is interpolation value (0 ~ 1)
        // - "mode" param is animation mode
        // - return new vector after interpolation

        internal float interpolate(float from, float to, float alpha, AnimationMode mode)
        {
            //const float PI = 3.141593f;
            //const float HALF_PI = 3.141593f * 0.5f;

            // recompute alpha based on animation mode
            if (mode == AnimationMode.EASE_IN)
            {
                //@@alpha = 1 - cosf(HALF_PI * alpha);
                // with cubic function
                alpha = alpha * alpha * alpha;
            }
            else if (mode == AnimationMode.EASE_IN2)
            {
                alpha = 1 - sqrtf(1 - alpha * alpha);
            }
            else if (mode == AnimationMode.EASE_OUT)
            {
                //@@alpha = sinf(HALF_PI * alpha);
                // with cubic function
                float beta = 1 - alpha;
                alpha = 1 - beta * beta * beta;
            }
            else if (mode == AnimationMode.EASE_OUT2)
            {
                alpha = sqrtf(1 - (1 - alpha) * (1 - alpha));
            }
            else if (mode == AnimationMode.EASE_IN_OUT)
            {
                //@@alpha = 0.5f * (1 - cosf(PI * alpha));
                // with cubic function
                float beta = 1 - alpha;
                float scale = 4.0f;     // 0.5 / (0.5^3)
                if (alpha < 0.5f)
                    alpha = alpha * alpha * alpha * scale;
                else
                    alpha = 1 - (beta * beta * beta * scale);
            }
            else if (mode == AnimationMode.EASE_IN_OUT2)
            {
                if (alpha < 0.5f)
                    alpha = 0.5f * (1 - sqrtf(1 - alpha * alpha));
                else
                    alpha = 0.5f * sqrtf(1 - (1 - alpha) * (1 - alpha)) + 0.5f;
            }
            else if (mode == AnimationMode.BOUNCE)
            {
            }
            else if (mode == AnimationMode.ELASTIC)
            {
            }

            return from + alpha * (to - from);
        }

        internal Vector3 interpolate(Vector3 from, Vector3 to, float alpha, AnimationMode mode)
        {
            //const float PI = 3.141593f;
            //const float HALF_PI = 3.141593f * 0.5f;

            // recompute alpha based on animation mode
            if (mode == AnimationMode.EASE_IN)
            {
                //@@alpha = 1 - cosf(HALF_PI * alpha);
                // with cubic function
                alpha = alpha * alpha * alpha;
            }
            else if (mode == AnimationMode.EASE_IN2)
            {
                alpha = 1 - sqrtf(1 - alpha * alpha);
            }
            else if (mode == AnimationMode.EASE_OUT)
            {
                //@@alpha = sinf(HALF_PI * alpha);
                // with cubic function
                float beta = 1 - alpha;
                alpha = 1 - beta * beta * beta;
            }
            else if (mode == AnimationMode.EASE_OUT2)
            {
                alpha = sqrtf(1 - (1 - alpha) * (1 - alpha));
            }
            else if (mode == AnimationMode.EASE_IN_OUT)
            {
                //@@alpha = 0.5f * (1 - cosf(PI * alpha));
                // with cubic function
                float beta = 1 - alpha;
                float scale = 4.0f;     // 0.5 / (0.5^3)
                if (alpha < 0.5f)
                    alpha = alpha * alpha * alpha * scale;
                else
                    alpha = 1 - (beta * beta * beta * scale);
            }
            else if (mode == AnimationMode.EASE_IN_OUT2)
            {
                if (alpha < 0.5f)
                    alpha = 0.5f * (1 - sqrtf(1 - alpha * alpha));
                else
                    alpha = 0.5f * sqrtf(1 - (1 - alpha) * (1 - alpha)) + 0.5f;
            }
            else if (mode == AnimationMode.BOUNCE)
            {
            }
            else if (mode == AnimationMode.ELASTIC)
            {
            }

            return from + alpha * (to - from);
        }



        ///////////////////////////////////////////////////////////////////////////////
        // spherical linear interpolation between 2 3D vectors
        // alpha value should be 0 ~ 1
        // NOTE: If angle between 2 vectors are 180, the rotation axis cannot be
        // determined.
        ///////////////////////////////////////////////////////////////////////////////
        internal Vector3 slerp(Vector3 from, Vector3 to, float alpha, AnimationMode mode)
        {
            // re-compute alpha
            float t = interpolate(0.0f, 1.0f, alpha, mode);

            // determine the angle between
            //@@ FIXME: handle if angle is ~180 degree
            //float dot = from.dot(to);
            //float cosine = from.dot(to) / (from.Length * to.Length);
            float cosine = Vector3.Dot(from, to) / (from.Length * to.Length);
            float angle = acosf(cosine);
            float invSine = 1.0f / sinf(angle);

            // compute the scale factors
            float scale1 = sinf((1 - t) * angle) * invSine;
            float scale2 = sinf(t * angle) * invSine;

            // compute slerp-ed vector
            return scale1 * from + scale2 * to;
        }



        ///////////////////////////////////////////////////////////////////////////////
        // spherical linear interpolation between 2 quaternions
        // the alpha should be 0 ~ 1
        // assume the quaternions have unit length.
        // NOTE: If angle between 2 vectors are 180, the rotation axis cannot be
        // determined.
        ///////////////////////////////////////////////////////////////////////////////
        internal Quaternion slerp(Quaternion from, Quaternion to, float alpha, AnimationMode mode)
        {
            // re-compute alpha
            float t = interpolate(0.0f, 1.0f, alpha, mode);

            float dot = from.W * to.W + from.X * to.X + from.Y * to.Y + from.Z * to.Z;

            // if 2 quaternions are close (angle ~= 0), then use lerp
            if (1 - dot < 0.001f)
            {
                return (from + (to - from) * t);
            }
            // if angle is ~180 degree, then the rotation axis is undefined
            // try to find any rotation axis in this case
            else if (fabs(1 + dot) < 0.001f) // dot ~= -1
            {
                Vector3 up, v1, v2;
                v1 = new Vector3(from.X, from.Y, from.Z);
                v1.Normalize();
                if (fabs(from.X) < 0.001f)
                    up = new Vector3(1, 0, 0);
                else
                    up = new Vector3(0, 1, 0);
                //v2 = v1.cross(up); // orthonormal to v1
                v2 = Vector3.Cross(v1, up); // orthonormal to v1
                v2.Normalize();
                //std::cout << v2 << std::endl;

                // referenced from Jonathan Blow's Understanding Slerp
                float angle_ = acosf(dot) * t;
                Vector3 v3 = v1 * cosf(angle_) + v2 * sinf(angle_);
                return new Quaternion(0, v3.X, v3.Y, v3.Z);
            }

            // determine the angle between
            float angle = acosf(dot);
            float invSine = 1.0f / sqrtf(1 - dot * dot);  // = 1 / sin(angle)

            // compute the scale factors
            float scale1 = sinf((1 - t) * angle) * invSine;
            float scale2 = sinf(t * angle) * invSine;

            return (from * scale1 + to * scale2);
        }



        ///////////////////////////////////////////////////////////////////////////////
        // accelerate / de-accelerate speed
        // === PARAMS ===
        //  isMoving: accelerate if true, de-accelerate if false
        //     speed: the current speed per sec
        //  maxSpeed: maximum speed per sec (positive or negative)
        //     accel: acceleration per sec squared (always positive)
        // deltaTime: frame time in second
        ///////////////////////////////////////////////////////////////////////////////
        internal float accelerate(bool isMoving, float speed, float maxSpeed, float accel, float deltaTime)
        {
            // determine direction
            float sign;
            if (maxSpeed > 0)
                sign = 1;
            else
                sign = -1;

            // accelerating
            if (isMoving)
            {
                speed += sign * accel * deltaTime;
                if ((sign * speed) > (sign * maxSpeed))
                    speed = maxSpeed;
            }
            // de-accelerating
            else
            {
                speed -= sign * accel * deltaTime;
                if ((sign * speed) < 0)
                    speed = 0;
            }

            return speed;
        }




        /*
         * Given two rotations, e1 and e2, expressed as quaternion rotations,
         * figure out the equivalent single rotation and stuff it into dest.
         *
         * This routine also normalizes the result every RENORMCOUNT times it is
         * called, to keep error from creeping in.
         *
         * NOTE: This routine is written so that q1 or q2 may be the same
         * as dest (or each other).
         */

        const int RENORMCOUNT = 97;

        void negate_quat(float[] q, out float[] nq)
        {
            nq = new float[4];
            nq[0] = -q[0];
            nq[1] = -q[1];
            nq[2] = -q[2];
            nq[3] = q[3];
        }

        void add_quats(float[] q1, float[] q2, out float[] dest)
        {
            int count = 0;
            dest = new float[4];
            float[] t1 = new float[4];
            float[] t2 = new float[4];
            float[] t3 = new float[4];
            float[] tf = new float[4];

            if (debugflag)
            {
                Console.WriteLine("q1 = {0}  {1}  {2}  {3} \n", q1[0], q1[1], q1[2], q1[3]);
                Console.WriteLine("q2 = {0}  {1}  {2}  {3} \n", q2[0], q2[1], q2[2], q2[3]);
            }

            vcopy(q1, t1);
            vscale(t1, q2[3]);

            vcopy(q2, t2);
            vscale(t2, q1[3]);

            vcross(q2, q1, t3);
            vadd(t1, t2, tf);
            vadd(t3, tf, tf);
            tf[3] = q1[3] * q2[3] - vdot(q1, q2);

            if (debugflag)
            {
                Console.WriteLine("tf = {0}  {1}  {2}  {3} \n", tf[0], tf[1], tf[2], tf[3]);
            }

            dest[0] = tf[0];
            dest[1] = tf[1];
            dest[2] = tf[2];
            dest[3] = tf[3];

            if (++count > RENORMCOUNT)
            {
                count = 0;
                normalize_quat(dest);
            }
        }

        /*
         * Quaternions always obey:  a^2 + b^2 + c^2 + d^2 = 1.0
         * If they don't add up to 1.0, dividing by their magnitudes will
         * renormalize them.
         *
         * Note: See the following for more information on quaternions:
         *
         * - Shoemake, K., Animating rotation with quaternion curves, Computer
         *   Graphics 19, No 3 (Proc. SIGGRAPH'85), 245-254, 1985.
         * - Pletinckx, D., Quaternion calculus as a basic tool in computer
         *   graphics, The Visual Computer 5, 2-13, 1989.
         */
        void normalize_quat(float[] q)
        {
            int i;
            float mag;

            mag = sqrtf(q[0] * q[0] + q[1] * q[1] + q[2] * q[2] + q[3] * q[3]);
            for (i = 0; i < 4; i++) q[i] /= mag;
        }
    }
}
