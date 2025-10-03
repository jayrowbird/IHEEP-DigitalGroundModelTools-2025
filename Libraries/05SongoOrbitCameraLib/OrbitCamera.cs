///////////////////////////////////////////////////////////////////////////////
// OrbitCamera.h
// =============
// Orbital camera class for OpenGL
// Use lookAt() for initial positioning the camera, then call rotateTo() for
// orbital rotation, moveTo()/moveForward() to move camera position only and
// shiftTo() to move position and target together (panning)
//
// Dependencies: Vector2, Vector3, Matrix4, Quaternion, animUtils.h
//
//  AUTHOR: Song Ho Ahn (song.ahn@gmail.com)
// CREATED: 2011-12-02
// UPDATED: 2016-10-24
///////////////////////////////////////////////////////////////////////////////

// Ignore Spelling: Songo

using OpenTK.Mathematics;
using Songo.OrbitCamera.SGI.Maths;
using SongoOrbitCameraLib;

namespace Songo.OrbitCamera
{ 
    public class OrbitCamera
    {
        private bool DEBUGFLAG = false;
        // constants ==================================================================
        const float DEG2RAD = 3.141593f / 180.0f;
        const float RAD2DEG = 180.0f / 3.141593f;
        const float EPSILON = 0.00001f;

        SGIMath sgimath = new SGIMath();

        // static functions ===========================================================
        ///////////////////////////////////////////////////////////////////////////////
        // convert rotation angles (degree) to 4x4 matrix
        // NOTE: the angle is for orbit camera, so yaw angle must be reversed before
        // matrix computation.
        //
        // The order of rotation is Roll.Yaw.Pitch (Rx*Ry*Rz)
        // Rx: rotation about X-axis, pitch
        // Ry: rotation about Y-axis, yaw(heading)
        // Rz: rotation about Z-axis, roll
        //    Rx           Ry          Rz
        // |1  0   0| | Cy  0 Sy| |Cz -Sz 0|   | CyCz        -CySz         Sy  |
        // |0 Cx -Sx|*|  0  1  0|*|Sz  Cz 0| = | SxSyCz+CxSz -SxSySz+CxCz -SxCy|
        // |0 Sx  Cx| |-Sy  0 Cy| | 0   0 1|   |-CxSyCz+SxSz  CxSySz+SxCz  CxCy|
        ///////////////////////////////////////////////////////////////////////////////

        Matrix4 angleToMatrix(Vector3 angles)
        {
            float sx, sy, sz, cx, cy, cz, theta;
            Vector3 left, up, forward;

            // rotation angle about X-axis (pitch)
            theta = angle.X * DEG2RAD;
            sx = sgimath.sinf(theta);
            cx = sgimath.cosf(theta);

            // rotation angle about Y-axis (yaw)
            theta = -angle.Y * DEG2RAD;
            sy = sgimath.sinf(theta);
            cy = sgimath.cosf(theta);

            // rotation angle about Z-axis (roll)
            theta = angle.Z * DEG2RAD;
            sz = sgimath.sinf(theta);
            cz = sgimath.cosf(theta);

            // determine left axis
            left.X = cy * cz;
            left.Y = sx * sy * cz + cx * sz;
            left.Z = -cx * sy * cz + sx * sz;

            // determine up axis
            up.X = -cy * sz;
            up.Y = -sx * sy * sz + cx * cz;
            up.Z = cx * sy * sz + sx * cz;

            // determine forward axis
            forward.X = sy;
            forward.Y = -sx * cy;
            forward.Z = cx * cy;

            // construct rotation matrix
            Matrix4 matrix = Matrix4.Identity;
            //matrix.setColumn(0, left);
            matrix.Row0 = new Vector4(left, 1);
            //matrix.setColumn(1, up);
            matrix.Row1 = new Vector4(up, 1);
            //matrix.setColumn(2, forward);
            matrix.Row2 = new Vector4(forward, 1);

            return matrix;
        }
        ///////////////////////////////////////////////////////////////////////////////
        // retrieve angles in degree from rotation matrix, M = Rx*Ry*Rz
        // Rx: rotation about X-axis, pitch
        // Ry: rotation about Y-axis, yaw(heading)
        // Rz: rotation about Z-axis, roll
        //    Rx           Ry          Rz
        // |1  0   0| | Cy  0 Sy| |Cz -Sz 0|   | CyCz        -CySz         Sy  |
        // |0 Cx -Sx|*|  0  1  0|*|Sz  Cz 0| = | SxSyCz+CxSz -SxSySz+CxCz -SxCy|
        // |0 Sx  Cx| |-Sy  0 Cy| | 0   0 1|   |-CxSyCz+SxSz  CxSySz+SxCz  CxCy|
        //
        // Pitch: atan(-m[7] / m[8]) = atan(SxCy/CxCy)
        // Yaw  : asin(m[6])         = asin(Sy)
        // Roll : atan(-m[3] / m[0]) = atan(SzCy/CzCy)
        ///////////////////////////////////////////////////////////////////////////////

        Vector3 matrixToAngle(Matrix4 matrix)
        {
            Vector3 angle = MatrixGetAngle(matrix);

            // reverse yaw
            angle.Y = -angle.Y;
            angle.Z = angle.Z;
            return angle;
        }

        ///////////////////////////////////////////////////////////////////////////////
        // retrieve angles in degree from rotation matrix, M = Rx*Ry*Rz
        // Rx: rotation about X-axis, pitch
        // Ry: rotation about Y-axis, yaw(heading)
        // Rz: rotation about Z-axis, roll
        //    Rx           Ry          Rz
        // |1  0   0| | Cy  0 Sy| |Cz -Sz 0|   | CyCz        -CySz         Sy  |
        // |0 Cx -Sx|*|  0  1  0|*|Sz  Cz 0| = | SxSyCz+CxSz -SxSySz+CxCz -SxCy|
        // |0 Sx  Cx| |-Sy  0 Cy| | 0   0 1|   |-CxSyCz+SxSz  CxSySz+SxCz  CxCy|
        //
        // Pitch: atan(-m[9] / m[10]) = atan(SxCy/CxCy)
        // Yaw  : asin(m[8]) = asin(Sy)
        // Roll : atan(-m[4] / m[0]) = atan(SzCy/CzCy)
        ///////////////////////////////////////////////////////////////////////////////
        internal Vector3 MatrixGetAngle(Matrix4 matrix)
        {
            float pitch, yaw, roll;         // 3 angles

            // find yaw (around y-axis) first
            // NOTE: asin() returns -90~+90, so correct the angle range -180~+180
            // using z value of forward vector
            //yaw = RAD2DEG * asinf(m[8]);
            yaw = RAD2DEG * sgimath.asinf(matrix.M31);
            //if (m[10] < 0)
            if (matrix.M33 < 0)
            {
                if (yaw >= 0) yaw = 180.0f - yaw;
                else yaw = -180.0f - yaw;
            }

            // find roll (around z-axis) and pitch (around x-axis)
            // if forward vector is (1,0,0) or (-1,0,0), then m[0]=m[4]=m[9]=m[10]=0
            //if (m[0] > -EPSILON && m[0] < EPSILON)
            if (matrix.M11 > -EPSILON && matrix.M11 < EPSILON)
            {
                roll = 0;  //@@ assume roll=0
                //pitch = RAD2DEG * atan2f(m[1], m[5]);
                pitch = RAD2DEG * sgimath.atan2f(matrix.M12, matrix.M22);
            }
            else
            {
                //roll = RAD2DEG * atan2f(-m[4], m[0]);
                roll = RAD2DEG * sgimath.atan2f(-matrix.M21, matrix.M11);
                //pitch = RAD2DEG * atan2f(-m[9], m[10]);
                pitch = RAD2DEG * sgimath.atan2f(-matrix.M32, matrix.M33);
            }

            return new Vector3(pitch, yaw, roll);
        }


        //static Vector3 lookAtToAngle(const Vector3& position, const Vector3& target);

        // member variables
        Vector3 initialPosition = Vector3.NegativeInfinity;            // scenes initial position
        Vector3 initialTarget = Vector3.NegativeInfinity;              // scenes initial target

        Vector3 position;                   // camera position at world space
        Vector3 target;                     // camera focal(look at) position at world space
        float distance;                     // distance between position and target
        Vector3 angle;                      // angle in degree around the target (pitch, heading, roll)
        Matrix4 matrix;                     // 4x4 matrix combined rotation and translation
        Matrix4 matrixRotation;             // rotation only
        Quaternion quaternion;              // quaternion for rotations

        // for position movement
        Vector3 movingFrom;                 // camera starting position
        Vector3 movingTo;                   // camera destination position 
        Vector3 movingVector;               // normalized direction vector
        float movingTime;                   // animation elapsed time (sec)
        float movingDuration;               // animation duration (sec)
        bool moving;                        // flag to start/stop animation
        AnimationMode movingMode;           // interpolation mode

        // for target movement (shift)
        Vector3 shiftingFrom;               // camera starting target
        Vector3 shiftingTo;                 // camera destination target
        Vector3 shiftingVector;             // normalized direction vector
        float shiftingTime;                 // animation elapsed time (sec)
        float shiftingDuration;             // animation duration (sec)
        float shiftingSpeed;                // current velocity of shift vector
        float shiftingAccel;                // acceleration, units per second squared
        float shiftingMaxSpeed;             // max velocity of shift vector
        bool shifting;                      // flag to start/stop animation
        AnimationMode shiftingMode;         // interpolation mode

        // for forwarding using distance between position and target
        float forwardingFrom;               // starting distance
        float forwardingTo;                 // ending distance
        float forwardingTime;               // animation elapsed time (sec)
        float forwardingDuration;           // animation duration (sec)
        float forwardingSpeed;              // current velocity of moving forward
        float forwardingAccel;              // acceleration, units per second squared
        float forwardingMaxSpeed;           // max velocity of moving forward
        bool forwarding;                    // flag to start/stop forwarding
        AnimationMode forwardingMode;       // interpolation mode

        // for rotation
        Vector3 turningAngleFrom;           // starting angles
        Vector3 turningAngleTo;             // ending angles
        Quaternion turningQuaternionFrom;   // starting quaternion
        Quaternion turningQuaternionTo;     // ending quaternion
        float turningTime;                  // animation elapsed time (sec)
        float turningDuration;              // animation duration (sec)
        bool turning;                       // flag to start/stop rotation
        bool quaternionUsed;                // flag to use quaternion
        AnimationMode turningMode;          // interpolation mode


        // Added sb SB 2025
        //public double zoomDuration = 0.0;
        //public bool zooming = false;
        private int screenWidth;
        private int screenHeight;
        private RefGeomRenderer? refGeomRenderer = null;
        private OrbitCameraUtils? orbitCameraUtils = null;

        ~OrbitCamera()
        {
            refGeomRenderer = null;
            orbitCameraUtils = null;
        }

        public OrbitCamera(int width, int height, bool showReferenceGeometry)
        {
            screenWidth = width;
            screenHeight = height;
            orbitCameraUtils = new OrbitCameraUtils();
            orbitCameraUtils.TimerStart();

            if (showReferenceGeometry)
            {
                refGeomRenderer = new RefGeomRenderer(screenWidth, screenHeight, this);
                refGeomRenderer.Load();
            }

            movingTime = 0; movingDuration = 0; moving = false;
            shiftingTime = 0; shiftingDuration = 0; shiftingSpeed = 0;
            shiftingAccel = 0; shiftingMaxSpeed = 0; shifting = false;
            forwardingTime = 0; forwardingDuration = 0; forwardingSpeed = 0;
            forwardingAccel = 0; forwardingMaxSpeed = 0; forwarding = false;
            turningTime = 0; turningDuration = 0; turning = false;
            quaternionUsed = false;

            quaternion = new Quaternion(1, 0, 0, 0);
            matrix = Matrix4.Identity;
        }

        /// <summary>
        /// 
        /// </summary>
        public void CleanUpGPUMemory()
        {
            if(refGeomRenderer != null)
            {
                refGeomRenderer.CleanUpGPUMemory();
            }
            refGeomRenderer = null;
            orbitCameraUtils = null;
        }

        public void SetDrawingMode(int drawMode)
        {
            if (refGeomRenderer != null)
                refGeomRenderer.SetDrawingMode(drawMode);
        }

        public void resetCamera()
        {
            lookAt(initialPosition, initialTarget);
            //if (refGeomRenderer != null)
            //    refGeomRenderer.resetCamera(initialPosition, initialTarget);
        }
        public float GetTimerElapsed()
        {
            if(orbitCameraUtils != null)
            return orbitCameraUtils.TimerElapsedMilliseconds() / 1000.00f;
            else return 0;
        }

        public void OnRenderFrame()
        {
            if (refGeomRenderer != null)
            {
                refGeomRenderer.OnRenderFrame();
            }
        }

        /// <summary>
        /// On mouse wheel scroll add time to the zoom duration
        /// </summary>
        /// <param name="seconds"></param>
        //public void SetZoomDuration(double seconds)
        //{
        //    zoomDuration = orbitCameraUtils.TimerElapsedMilliseconds() / 1000.00 + seconds;
        //    zooming = true;
        //}

        //public void OnUpdateFrame()
        //{
        //    if (zoomDuration == 0.0 && zooming == true)
        //    {
        //        zoomDuration =  orbitCameraUtils.TimerElapsedMilliseconds() / 1000.00 + 1.2;
        //    }
        //    else if (zooming == true && zoomDuration <= orbitCameraUtils.TimerElapsedMilliseconds() / 1000.00)
        //    {//zoom using mouse wheel for 1.2 seconds
        //        stopForward();
        //        zoomDuration = 0.0;
        //        zooming = false;
        //    }
        //}
        public void OnResize(int width, int height)
        {
            if (refGeomRenderer != null)
            {
                refGeomRenderer.OnResize(width, height);
            }
        }


        internal OrbitCamera(Vector3 position, Vector3 target)
        {
            movingTime = 0; movingDuration = 0; moving = false;
            shiftingTime = 0; shiftingDuration = 0; shiftingSpeed = 0;
            shiftingAccel = 0; shiftingMaxSpeed = 0; shifting = false;
            forwardingTime = 0; forwardingDuration = 0; forwardingSpeed = 0;
            forwardingAccel = 0; forwardingMaxSpeed = 0; forwarding = false;
            turningTime = 0; turningDuration = 0; turning = false;
            quaternionUsed = false;

            quaternion = new Quaternion(1, 0, 0, 0);
            matrix = Matrix4.Identity;
            orbitCameraUtils = new OrbitCameraUtils();
            lookAt(position, target);
        } 
        ///////////////////////////////////////////////////////////////////////////////
        // update each frame, frame time is sec
        ///////////////////////////////////////////////////////////////////////////////
        //int test = 0;
        public void update(float frameTime)
        {
            //if(test++%1000 == 0)
            //{
            //    if (DEBUGFLAG) Console.WriteLine("{1} update forwarding is (0)", forwarding, test);
            //}

            if (moving)
                updateMove(frameTime);
            if (shifting || shiftingSpeed != 0)
                updateShift(frameTime);
            if (forwarding || forwardingSpeed != 0)
                updateForward(frameTime);
            if (turning)
                updateTurn(frameTime);
        }

        ///////////////////////////////////////////////////////////////////////////////
        // print itself
        ///////////////////////////////////////////////////////////////////////////////
        void printSelf()
        {
            //std::cout << "===== OrbitCamera =====\n"
            //          << "  Position: " << position << "\n"
            //          << "    Target: " << target << "\n"
            //          << "    Matrix:\n" << matrix << "\n"
            //          << std::endl;
        }

        // set position, target and transform matrix so camera looks at the target

        ///////////////////////////////////////////////////////////////////////////////
        // set transform matrix equivalent to gluLookAt()
        // 1. Mt: Translate scene to camera position inversely, (-x, -y, -z)
        // 2. Mr: Rotate scene inversely so camera looks at the scene
        // 3. Find matrix = Mr * Mt
        //       Mr               Mt
        // |r0  r4  r8  0|   |1  0  0 -x|   |r0  r4  r8  r0*-x + r4*-y + r8 *-z|
        // |r1  r5  r9  0| * |0  1  0 -y| = |r1  r5  r9  r1*-x + r5*-y + r9 *-z|
        // |r2  r6  r10 0|   |0  0  1 -z|   |r2  r6  r10 r2*-x + r6*-y + r10*-z|
        // |0   0   0   1|   |0  0  0  1|   |0   0   0   1                     |
        /////////////////////////////////////////////////////////////////////////////// 
        public void lookAt(Vector3 pos, Vector3 target)
        {
            // Store initial scene camera and target position
            if (this.initialPosition == Vector3.NegativeInfinity) this.initialPosition = pos;
            if (this.initialTarget == Vector3.NegativeInfinity) this.initialTarget = target;

            // remember the camera position & target position
            this.position = pos;
            this.target = target;
            //target.Z = 0f;

            if (refGeomRenderer != null) refGeomRenderer.SetRefTarget(target);

            // if pos and target are same, only translate camera to position without rotation
            if (position == target)
            {
                matrix = Matrix4.Identity;
                //matrix.setColumn(3, -position);
                matrix.Row3 = new Vector4(-position, 1);
                // rotation stuff
                matrixRotation = Matrix4.Identity;
                //angle.set(0, 0, 0);
                angle = Vector3.Zero;
                quaternion = new Quaternion(1, 0, 0, 0);
                return;
            }

            Vector3 left, up, forward;  // 3 axis of matrix for scene

            // first, compute the forward vector of rotation matrix
            // NOTE: the direction is reversed (target to camera pos) because of camera transform
            forward = position - target;
            this.distance = forward.Length;  // remember the distance
                                             // normalize
            forward /= this.distance;

            // compute temporal up vector based on the forward vector
            // watch out when look up/down at 90 degree
            // for example, forward vector is on the Y axis
            if (sgimath.fabs(forward.X) < EPSILON && sgimath.fabs(forward.Z) < EPSILON)
            {
                // forward vector is pointing +Y axis
                if (forward.Y > 0)
                {
                    up = new Vector3(0, 0, -1);
                }
                // forward vector is pointing -Y axis
                else
                {
                    up = new Vector3(0, 0, 1);
                }
            }
            // in general, up vector is straight up
            else
            {
                up = new Vector3(0, 1, 0);
            }

            // compute the left vector of rotation matrix
            //left = up.cross(forward);   // cross product
            left = Vector3.Cross(up, forward);
            left.Normalize();

            // re-calculate the orthonormal up vector
            //up = forward.cross(left);   // cross product
            up = Vector3.Cross(forward, left);

            // set inverse rotation matrix: M^-1 = M^T for Euclidean transform
            matrixRotation = Matrix4.Identity;
            //matrixRotation.setRow(0, left);
            matrixRotation.Column0 = new Vector4(left, 0);
            //matrixRotation.setRow(1, up);
            matrixRotation.Column1 = new Vector4(up, 0);
            //matrixRotation.setRow(2, forward);
            matrixRotation.Column2 = new Vector4(forward, 0);

            // copy it to matrix
            matrix = Matrix4.Identity;
            //matrix.setRow(0, left);
            matrix.Column0 = new Vector4(left, 0);
            //matrix.setRow(1, up);
            matrix.Column1 = new Vector4(up, 0);
            //matrix.setRow(2, forward);
            matrix.Column2 = new Vector4(forward, 0);

            // set translation part
            Vector3 trans = Vector3.Zero;
            //trans.X = matrix[0] * -position.X + matrix[4] * -position.Y + matrix[8] * -position.Z;
            trans.X = matrix.M11 * -position.X + matrix.M21 * -position.Y + matrix.M31 * -position.Z;
            //rans.Y = matrix[1] * -position.X + matrix[5] * -position.Y + matrix[9] * -position.Z;
            trans.Y = matrix.M12 * -position.X + matrix.M22 * -position.Y + matrix.M32 * -position.Z;
            //trans.Z = matrix[2] * -position.X + matrix[6] * -position.Y + matrix[10] * -position.Z;
            trans.Z = matrix.M13 * -position.X + matrix.M23 * -position.Y + matrix.M33 * -position.Z;

            //matrix.setColumn(3, trans);
            matrix.Row3 = new Vector4(trans, 1);

            // set Euler angles
            angle = matrixToAngle(matrixRotation);

            // set quaternion from angle
            Vector3 reversedAngle = new Vector3(angle.X, -angle.Y, angle.Z);
            quaternion = sgimath.getQuaternion(reversedAngle * DEG2RAD * 0.5f); // half angle

            //DEBUG
            //std::cout << matrixRotation << std::endl;
        }

        ///////////////////////////////////////////////////////////////////////////////
        // set transform matrix with target and camera's up vectors
        /////////////////////////////////////////////////////////////////////////////// 
        void lookAt(Vector3 pos, Vector3 target, Vector3 upDir)
        {
            // Store initial scene camera and target position
            if (this.initialPosition == Vector3.NegativeInfinity) this.initialPosition = pos;
            if (this.initialTarget == Vector3.NegativeInfinity) this.initialTarget = target;

            // remember the camera position & target position
            this.position = pos;
            this.target = target;

            // if pos and target are same, only translate camera to position without rotation
            if (position == target)
            {
                matrix = Matrix4.Identity;
                matrix = Matrix4.CreateTranslation(-position.X, -position.Y, -position.Z);
                // rotation stuff
                matrixRotation = Matrix4.Identity;
                //angle.set(0, 0, 0);
                angle = Vector3.Zero;
                quaternion = new Quaternion(1, 0, 0, 0);
                return;
            }

            Vector3 left, up, forward;          // 3 axis vectors for scene

            // compute the forward vector
            // NOTE: the direction is reversed (target to camera pos) because of camera transform
            forward = position - target;
            this.distance = forward.Length;  // remember the distance
                                             // normalize
            forward /= this.distance;

            // compute the left vector
            //left = upDir.cross(forward);        // cross product
            left = Vector3.Cross(upDir, forward);
            left.Normalize();

            // recompute the orthonormal up vector
            //up = forward.cross(left);           // cross product
            up = Vector3.Cross(forward, left);
            //up.normalize();

            // set inverse rotation matrix: M^-1 = M^T for Euclidean transform
            matrixRotation = Matrix4.Identity;
            //matrixRotation.setRow(0, left);
            matrixRotation.Column0 = new Vector4(left, 0);
            //matrixRotation.setRow(1, up);
            matrixRotation.Column1 = new Vector4(up, 0);
            //matrixRotation.setRow(2, forward);
            matrixRotation.Column2 = new Vector4(forward, 0);

            // copy it to matrix
            matrix = Matrix4.Identity;
            //matrix.setRow(0, left);
            matrix.Column0 = new Vector4(left, 0);
            //matrix.setRow(1, up);
            matrix.Column1 = new Vector4(up, 0);
            //matrix.setRow(2, forward);
            matrix.Column2 = new Vector4(forward, 0);

            // set translation
            Vector3 trans = new Vector3();
            //trans.X = matrix[0] * -position.X + matrix[4] * -position.Y + matrix[8] * -position.Z;
            trans.X = matrix.M11 * -position.X + matrix.M21 * -position.Y + matrix.M31 * -position.Z;
            //trans.Y = matrix[1] * -position.X + matrix[5] * -position.Y + matrix[9] * -position.Z;
            trans.Y = matrix.M12 * -position.X + matrix.M22 * -position.Y + matrix.M32 * -position.Z;
            //trans.Z = matrix[2] * -position.X + matrix[6] * -position.Y + matrix[10] * -position.Z;
            trans.Z = matrix.M13 * -position.X + matrix.M23 * -position.Y + matrix.M33 * -position.Z;
            //matrix.setColumn(3, trans);
            matrix.Row3 = new Vector4(trans, 1);

            // set Euler angles
            angle = matrixToAngle(matrixRotation);

            // set quaternion from angle
            Vector3 reversedAngle = new Vector3(angle.X, -angle.Y, angle.Z);
            quaternion = sgimath.getQuaternion(reversedAngle * DEG2RAD * 0.5f); // half angle

            //DEBUG
            //std::cout << matrixRotation << std::endl;
        }


        void lookAt(float px, float py, float pz, float tx, float ty, float tz)
        {
            lookAt(new Vector3(px, py, pz), new Vector3(tx, ty, tz));
        }

        void lookAt(float px, float py, float pz, float tx, float ty, float tz, float ux, float uy, float uz)
        {
            lookAt(new Vector3(px, py, pz), new Vector3(tx, ty, tz), new Vector3(ux, uy, uz));
        }

        // move the camera position to the destination
        // if duration(sec) is greater than 0, it will animate for the given duration
        // otherwise, it will set the position immediately
        // use moveForward() to move the camera forward/backward
        // NOTE: you must call update() before getting the delta movement per frame

        ///////////////////////////////////////////////////////////////////////////////
        // move the camera position with the given duration
        ///////////////////////////////////////////////////////////////////////////////

        void moveTo(Vector3 to, float duration = 0, AnimationMode mode = AnimationMode.EASE_OUT)
        {
            if (duration <= 0.0f)
            {
                setPosition(to);
            }
            else
            {
                movingFrom = position;
                movingTo = to;
                movingVector = movingTo - movingFrom;
                movingVector.Normalize();
                movingTime = 0;
                movingDuration = duration;
                movingMode = mode;
                moving = true;
            }
        }

        //////////////////////////////////////////////////////////////////////////////
        // zoom in/out the camera position with the given delta movement and duration(sec)
        // move forward or backward by a percent of the distance of current camera to target
        // note if percent is negative we move forward, if percent is positive we zoom backward
        // it actually moves the camera forward or backward.
        // positive delta means moving forward (decreasing distance)
        /////////////////////////////////////////////////////////////////////////////// 
        public void moveForwardPercent(float percent, float duration = 0, AnimationMode mode = AnimationMode.EASE_OUT)
        {
            if (duration <= 0.0f)
            {
                setDistance(distance + distance * percent);
            }
            else
            {
                forwardingFrom = distance;
                forwardingTo = distance + distance * percent;
                forwardingTime = 0;
                forwardingDuration = duration;
                forwardingMode = mode;
                forwarding = true;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        // zoom in/out the camera position with the given delta movement and duration(sec)
        // it actually moves the camera forward or backward.
        // positive delta means moving forward (decreasing distance)
        /////////////////////////////////////////////////////////////////////////////// 
        void moveForward(float delta, float duration = 0, AnimationMode mode = AnimationMode.EASE_OUT)
        {
            if (duration <= 0.0f)
            {
                setDistance(distance - delta);
            }
            else
            {
                forwardingFrom = distance;
                forwardingTo = distance - delta;
                forwardingTime = 0;
                forwardingDuration = duration;
                forwardingMode = mode;
                forwarding = true;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        // start accelerating to move forward
        // It takes maximum speed per sec and acceleration per squared sec.
        // positive speed means moving forward (decreasing distance).
        // acceleration should be always positive.
        /////////////////////////////////////////////////////////////////////////////// 
        public void startForward(float maxSpeed = 1.0f, float accel = 1.0f)
        {
            if (DEBUGFLAG) Console.WriteLine("Orbit Camera:: startForward");
            forwardingSpeed = 0;
            forwardingMaxSpeed = maxSpeed;
            forwardingAccel = accel;
            forwardingTime = 0;
            forwardingDuration = 0;
            forwarding = true;
        }

        public void stopForward()
        {
            if (DEBUGFLAG) Console.WriteLine("Orbit Camera:: stopForward");
            forwarding = false;
        }
        // pan the camera, shift both position and target point in same direction; left/right/up/down
        // use this function to offset the camera's rotation pivot

        ///////////////////////////////////////////////////////////////////////////////
        // pan the camera target left/right/up/down with the given duration
        // the camera position will be shifted after transform
        ///////////////////////////////////////////////////////////////////////////////
        public void shiftTo(Vector3 to, float duration = 0, AnimationMode mode = AnimationMode.EASE_OUT)
        {
            if (duration <= 0.0f)
            {
                setTarget(to);
            }
            else
            {
                shiftingFrom = target;
                shiftingTo = to;
                shiftingVector = shiftingTo - shiftingFrom;
                shiftingVector.Normalize();
                shiftingTime = 0;
                shiftingDuration = duration;
                shiftingMode = mode;
                shifting = true;
            }
        }
        ///////////////////////////////////////////////////////////////////////////////
        // shift the camera position and target left/right/up/down
        ///////////////////////////////////////////////////////////////////////////////
        public void shift(Vector2 delta, float duration = 0, AnimationMode mode = AnimationMode.EASE_OUT)
        {
            // get left & up vectors of camera
            //Vector3 cameraLeft = new Vector3(-matrix[0], -matrix[4], -matrix[8]);
            Vector3 cameraLeft = new Vector3(-matrix.M11, -matrix.M21, -matrix.M31);
            //Vector3 cameraUp = new Vector3(-matrix[1], -matrix[5], -matrix[9]);
            Vector3 cameraUp = new Vector3(-matrix.M12, -matrix.M22, -matrix.M32);

            // compute delta movement
            Vector3 deltaMovement = delta.X * cameraLeft;
            deltaMovement += -delta.Y * cameraUp;   // reverse up direction

            // find new target position
            Vector3 newTarget = target + deltaMovement;

            shiftTo(newTarget, duration, mode);
        }

        ///////////////////////////////////////////////////////////////////////////////
        // start accelerating to shift camera
        // It takes shift direction vector and acceleration per squared sec.
        // acceleration should be always positive.
        /////////////////////////////////////////////////////////////////////////////// 
        void startShift(Vector2 shiftVector, float accel = 1.0f)
        {
            // get left & up vectors of camera
            //Vector3 cameraLeft = new Vector3(-matrix[0], -matrix[4], -matrix[8]);
            Vector3 cameraLeft = new Vector3(-matrix.M11, -matrix.M21, -matrix.M31);
            //Vector3 cameraUp = new Vector3(-matrix[1], -matrix[5], -matrix[9]);
            Vector3 cameraUp = new Vector3(-matrix.M12, -matrix.M22, -matrix.M32);

            // compute new target vector
            Vector3 vector = shiftVector.X * cameraLeft;
            vector += -shiftVector.Y * cameraUp;   // reverse up direction

            shiftingMaxSpeed = shiftVector.Length;
            shiftingVector = vector;
            shiftingVector.Normalize();
            shiftingSpeed = 0;
            shiftingAccel = accel;
            shiftingTime = 0;
            shiftingDuration = 0;
            shifting = true;
        }


        internal void stopShift()
        {
            shifting = false;
        }

        // rotate the camera around the target point
        // You can use either quaternion or Euler angles
        ///////////////////////////////////////////////////////////////////////////////
        // rotate camera to the given angle with duration
        /////////////////////////////////////////////////////////////////////////////// 
        internal void rotateTo(Vector3 angle, float duration = 0.0f, AnimationMode mode = AnimationMode.EASE_OUT)
        {
            quaternionUsed = false;
            if (duration <= 0.0f)
            {
                setRotation(angle);
            }
            else
            {
                turningAngleFrom = this.angle;
                turningAngleTo = angle;
                turningTime = 0;
                turningDuration = duration;
                turningMode = mode;
                turning = true;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        // rotate camera to the given quaternion with duration
        /////////////////////////////////////////////////////////////////////////////// 
        public void rotateTo(Quaternion q, float duration = 0.0f, AnimationMode mode = AnimationMode.EASE_OUT)
        {
            quaternionUsed = true;
            if (duration <= 0.0f)
            {
                setRotation(q);
            }
            else
            {
                turningQuaternionFrom = this.quaternion;
                turningQuaternionTo = q;
                turningTime = 0;
                turningDuration = duration;
                turningMode = mode;
                turning = true;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        // rotate camera with delta angle
        // NOTE: delta angle must be negated already
        /////////////////////////////////////////////////////////////////////////////// 
        void rotate(Vector3 deltaAngle, float duration = 0.0f, AnimationMode mode = AnimationMode.EASE_OUT)
        {
            rotateTo(angle + deltaAngle, duration, mode);
        }


        // setters
        ///////////////////////////////////////////////////////////////////////////////
        // set position of camera, set transform matrix as well
        /////////////////////////////////////////////////////////////////////////////// 
        void setPosition(Vector3 v)
        {
            lookAt(v, target);
        }
        ///////////////////////////////////////////////////////////////////////////////
        // set target of camera, then rebuild matrix
        // rotation parts are not changed, but translation part must be recalculated
        // And, position is also shifted
        ///////////////////////////////////////////////////////////////////////////////
        void setPosition(float x, float y, float z) { setPosition(new Vector3(x, y, z)); }

        ///////////////////////////////////////////////////////////////////////////////
        // set target of camera, then rebuild matrix
        // rotation parts are not changed, but translation part must be recalculated
        // And, position is also shifted
        ///////////////////////////////////////////////////////////////////////////////
        void setTarget(Vector3 v)
        {
            target = v;

            // forward vector of camera
            //Vector3 forward = new Vector3(-matrix[2], -matrix[6], -matrix[10]);
            Vector3 forward = new Vector3(-matrix.M13, -matrix.M23, -matrix.M33);
            position = target - (distance * forward);
            computeMatrix();
        }
        void setTarget(float x, float y, float z) { setTarget(new Vector3(x, y, z)); }

        ///////////////////////////////////////////////////////////////////////////////
        // set distance of camera, then recompute camera position
        ///////////////////////////////////////////////////////////////////////////////
        void setDistance(float d)
        {
            distance = d;
            computeMatrix();
        }

        ///////////////////////////////////////////////////////////////////////////////
        // set transform matrix with rotation angles (degree)
        // NOTE: the angle is for camera, so yaw value must be negated for computation.
        //
        // The order of rotation is Roll.Yaw.Pitch (Rx*Ry*Rz)
        // Rx: rotation about X-axis, pitch
        // Ry: rotation about Y-axis, yaw(heading)
        // Rz: rotation about Z-axis, roll
        //    Rx           Ry          Rz
        // |1  0   0| | Cy  0 Sy| |Cz -Sz 0|   | CyCz        -CySz         Sy  |
        // |0 Cx -Sx|*|  0  1  0|*|Sz  Cz 0| = | SxSyCz+CxSz -SxSySz+CxCz -SxCy|
        // |0 Sx  Cx| |-Sy  0 Cy| | 0   0 1|   |-CxSyCz+SxSz  CxSySz+SxCz  CxCy|
        ///////////////////////////////////////////////////////////////////////////////

        void setRotation(Vector3 v)  // angles in degree
        {
            // remember angles
            // NOTE: assume all angles are already reversed for camera
            this.angle = angle;

            // remember quaternion value
            // NOTE: yaw must be negated again for quaternion
            Vector3 reversedAngle = new Vector3(angle.X, -angle.Y, angle.Z);
            this.quaternion = sgimath.getQuaternion(reversedAngle);

            // compute rotation matrix from angle
            matrixRotation = angleToMatrix(angle);

            // construct camera matrix
            computeMatrix();

            //DEBUG
            //std::cout << angle <<std::endl;
        }

        ///////////////////////////////////////////////////////////////////////////////
        // set rotation with new quaternion
        // NOTE: quaternion value is for matrix, so matrixToAngle() will reverse yaw.
        ///////////////////////////////////////////////////////////////////////////////
        void setRotation(float ax, float ay, float az) { setRotation(new Vector3(ax, ay, az)); }

        ///////////////////////////////////////////////////////////////////////////////
        // set rotation with new quaternion
        // NOTE: quaternion value is for matrix, so matrixToAngle() will reverse yaw.
        ///////////////////////////////////////////////////////////////////////////////
        void setRotation(Quaternion q)
        {
            // remember the current quaternion
            quaternion = q;

            // quaternion to matrix
            //matrixRotation = q.getMatrix();
            matrixRotation = sgimath.getMatrix(q);

            // construct matrix
            computeMatrix();

            // compute angle from matrix
            angle = matrixToAngle(matrixRotation);
        }

        // getters
        public Vector3 getPosition() { return position; }
        public Vector3 getTarget() { return target; }
        public Vector3 getAngle() { return angle; }
        public Matrix4 getMatrix() { return matrix; }
        public float getDistance() { return distance; }
        public Quaternion getQuaternion() { return quaternion; }

        // return camera's 3 axis vectors
        ///////////////////////////////////////////////////////////////////////////////
        // return left, up, forward axis
        ///////////////////////////////////////////////////////////////////////////////

        public Vector3 getLeftAxis()
        {
            //return new Vector3(-matrix[0], -matrix[4], -matrix[8]);
            return new Vector3(-matrix.M11, -matrix.M21, -matrix.M31);
        }

        public Vector3 getUpAxis()
        {
            //return new Vector3(matrix[1], matrix[5], matrix[9]);
            return new Vector3(matrix.M12, matrix.M22, matrix.M31);
        }
        public Vector3 getForwardAxis()
        {
            //return new Vector3(-matrix[2], -matrix[6], -matrix[10]);
            return new Vector3(-matrix.M13, -matrix.M23, -matrix.M33);
        }

        ///////////////////////////////////////////////////////////////////////////////
        // update position movement only
        ///////////////////////////////////////////////////////////////////////////////

        private void updateMove(float frameTime)
        {
            movingTime += frameTime;
            if (movingTime >= movingDuration)
            {
                setPosition(movingTo);
                moving = false;
            }
            else
            {
                Vector3 p = sgimath.interpolate(movingFrom, movingTo,
                                             movingTime / movingDuration, movingMode);
                setPosition(p);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        // update target movement only
        ///////////////////////////////////////////////////////////////////////////////
        private void updateShift(float frameTime)
        {
            shiftingTime += frameTime;

            // shift with duration
            if (shiftingDuration > 0)
            {
                if (shiftingTime >= shiftingDuration)
                {
                    setTarget(shiftingTo);
                    shifting = false;
                }
                else
                {
                    Vector3 p = sgimath.interpolate(shiftingFrom, shiftingTo,
                                                 shiftingTime / shiftingDuration, shiftingMode);
                    setTarget(p);
                }
            }
            // shift with acceleration
            else
            {
                shiftingSpeed = sgimath.accelerate(shifting, shiftingSpeed,
                                                shiftingMaxSpeed, shiftingAccel, frameTime);
                setTarget(target + (shiftingVector * shiftingSpeed * frameTime));
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        // update forward movement only
        ///////////////////////////////////////////////////////////////////////////////
        private void updateForward(float frameTime)
        {
            if (DEBUGFLAG) Console.WriteLine("updateForward");
            forwardingTime += frameTime;

            // move forward for duration
            if (forwardingDuration > 0)
            {
                if (forwardingTime >= forwardingDuration)
                {
                    setDistance(forwardingTo);
                    forwarding = false;
                }
                else
                {
                    float d = sgimath.interpolate(forwardingFrom, forwardingTo,
                                               forwardingTime / forwardingDuration, forwardingMode);
                    setDistance(d);
                }
            }

            // move forward with acceleration
            else
            {
                forwardingSpeed = sgimath.accelerate(forwarding, forwardingSpeed,
                                                  forwardingMaxSpeed, forwardingAccel, frameTime);
                setDistance(distance - forwardingSpeed * frameTime);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        // update rotation only
        ///////////////////////////////////////////////////////////////////////////////
        private void updateTurn(float frameTime)
        {
            turningTime += frameTime;
            if (turningTime >= turningDuration)
            {
                if (quaternionUsed)
                    setRotation(turningQuaternionTo);
                else
                    setRotation(turningAngleTo);
                turning = false;
            }
            else
            {
                if (quaternionUsed)
                {
                    Quaternion q = sgimath.slerp(turningQuaternionFrom, turningQuaternionTo,
                                              turningTime / turningDuration, turningMode);
                    setRotation(q);
                }
                else
                {
                    Vector3 p = sgimath.interpolate(turningAngleFrom, turningAngleTo,
                                                 turningTime / turningDuration, turningMode);
                    setRotation(p);
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        // construct camera matrix: M = Mt2 * Mr * Mt1
        // where Mt1: move scene to target (-x,-y,-z)
        //       Mr : rotate scene at the target point
        //       Mt2: move scene away from target with distance -d
        //
        //     Mt2             Mr               Mt1
        // |1  0  0  0|   |r0  r4  r8  0|   |1  0  0 -x|   |r0  r4  r8  r0*-x + r4*-y + r8*-z     |
        // |0  1  0  0| * |r1  r5  r9  0| * |0  1  0 -y| = |r1  r5  r9  r1*-x + r5*-y + r9*-z     |
        // |0  0  1 -d|   |r2  r6  r10 0|   |0  0  1 -z|   |r2  r6  r10 r2*-x + r6*-y + r10*-z - d|
        // |0  0  0  1|   |0   0   0   1|   |0  0  0  1|   |0   0   0   1                         |
        ///////////////////////////////////////////////////////////////////////////////

        private void computeMatrix()
        {
            // extract left/up/forward vectors from rotation matrix
            //Vector3 left = new Vector3(matrixRotation[0], matrixRotation[1], matrixRotation[2]);
            Vector3 left = new Vector3(matrixRotation.M11, matrixRotation.M12, matrixRotation.M13);
            //Vector3 up = new Vector3(matrixRotation[4], matrixRotation[5], matrixRotation[6]);
            Vector3 up = new Vector3(matrixRotation.M21, matrixRotation.M22, matrixRotation.M23);
            //Vector3 forward = new Vector3(matrixRotation[8], matrixRotation[9], matrixRotation[10]);
            Vector3 forward = new Vector3(matrixRotation.M31, matrixRotation.M32, matrixRotation.M33);

            // compute translation vector
            Vector3 trans = new Vector3();
            trans.X = left.X * -target.X + up.X * -target.Y + forward.X * -target.Z;
            trans.Y = left.Y * -target.X + up.Y * -target.Y + forward.Y * -target.Z;
            trans.Z = left.Z * -target.X + up.Z * -target.Y + forward.Z * -target.Z - distance;

            // construct matrix
            matrix = Matrix4.Identity;
            //matrix.setColumn(0, left);
            matrix.Row0 = new Vector4(left, 0);
            //matrix.setColumn(1, up);
            matrix.Row1 = new Vector4(up, 0);
            //matrix.setColumn(2, forward);
            matrix.Row2 = new Vector4(forward, 0);
            //matrix.setColumn(3, trans);
            matrix.Row3 = new Vector4(trans, 1);

            // re-compute camera position
            //forward.set(-matrix[2], -matrix[6], -matrix[10]);
            forward = new Vector3(-matrix.M13, -matrix.M23, -matrix.M33);
            position = target - (distance * forward);

            /*@@
            //DEBUG: equivalent to the above matrix computation
            matrix.identity();
            matrix.translate(-target.x, -target.Y, -target.Z); // Mt1: move scene to target point
            matrix = matrixRotation * matrix;                  // Mr : rotate scene at the target point
            matrix.translate(0, 0, -distance);                 // Mt2: move scene away from the target with distance

            // re-compute camera position
            // NOTE: camera's forward vector is the forward vector of inverse matrix
            Vector3 forward(-matrix[2], -matrix[6], -matrix[10]);
            position = target - (distance * forward);
            */
        }
    }
}