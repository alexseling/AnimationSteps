using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace AnimationSteps
{
    public class Camera
    {
        private GraphicsDeviceManager graphics;

        private Vector3 eye = new Vector3(1000, 1000, 1000);
        private Vector3 center = new Vector3(0, 0, 0);
        private Vector3 up = new Vector3(0, 1, 0);
        private float fov = MathHelper.ToRadians(35);
        private float znear = 10;
        private float zfar = 10000;

        public Vector3 Eye { get { return eye; } set { eye = value; ComputeView(); } }
        public Vector3 Center { get { return center; } set { center = value; ComputeView(); } }
        public Vector3 Up { get { return up; } set { up = value; ComputeView(); } }
        public float Fov { get { return fov; } set { fov = value; ComputeView(); } }
        public float Znear { get { return znear; } set { znear = value; ComputeView(); } }
        public float Zfar { get { return zfar; } set { zfar = value; ComputeView(); } }

        //private MouseState lastMouseState;

        private bool mousePitchYaw = true;
        private bool padPitchYaw = true;
        private bool mousePanTilt = true;

        public bool MousePitchYaw { get { return mousePitchYaw; } set { mousePitchYaw = value; } }
        public bool PadPitchYaw { get { return padPitchYaw; } set { padPitchYaw = value; } }
        public bool MousePanTilt { get { return mousePanTilt; } set { mousePanTilt = value; } }

        private Matrix view;
        private Matrix projection;

        public Matrix View { get { return view; } }
        public Matrix Projection { get { return projection; } }

        public Camera(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
        }

        public void Initialize()
        {
            ComputeView();
            ComputeProjection();
        }

        private void ComputeView()
        {
            view = Matrix.CreateLookAt(eye, center, up);
        }


        public void Pitch(float angle)
        {
            // Need a vector in the camera X direction
            Vector3 cameraZ = eye - center;
            Vector3 cameraX = Vector3.Cross(up, cameraZ);
            float len = cameraX.LengthSquared();
            if (len > 0)
                cameraX.Normalize();
            else
                cameraX = new Vector3(1, 0, 0);

            Matrix t1 = Matrix.CreateTranslation(-center);
            Matrix r = Matrix.CreateFromAxisAngle(cameraX, angle);
            Matrix t2 = Matrix.CreateTranslation(center);

            Matrix M = t1 * r * t2;
            eye = Vector3.Transform(eye, M);
            ComputeView();
        }

        public void Yaw(float angle)
        {
            Vector3 cameraZ = eye - center;
            Vector3 cameraX = Vector3.Cross(up, cameraZ);
            Vector3 cameraY = Vector3.Cross(cameraZ, cameraX);
            float len = cameraY.LengthSquared();
            if (len > 0)
                cameraY.Normalize();
            else
                cameraY = new Vector3(0, 1, 0);

            Matrix t1 = Matrix.CreateTranslation(-center);
            Matrix r = Matrix.CreateFromAxisAngle(cameraY, angle);
            Matrix t2 = Matrix.CreateTranslation(center);

            Matrix M = t1 * r * t2;
            eye = Vector3.Transform(eye, M);
            ComputeView();
        }

        public void Tilt(float angle)
        {
            // Need a vector in the camera X direction
            Vector3 cameraZ = eye - center;
            Vector3 cameraX = Vector3.Cross(up, cameraZ);
            float len = cameraX.LengthSquared();
            if (len > 0)
                cameraX.Normalize();
            else
                cameraX = new Vector3(1, 0, 0);

            Matrix t1 = Matrix.CreateTranslation(-eye);
            Matrix r = Matrix.CreateFromAxisAngle(cameraX, angle);
            Matrix t2 = Matrix.CreateTranslation(eye);

            Matrix M = t1 * r * t2;
            center = Vector3.Transform(center, M);
            ComputeView();
        }

        public void Pan(float angle)
        {
            Vector3 cameraZ = eye - center;
            Vector3 cameraX = Vector3.Cross(up, cameraZ);
            Vector3 cameraY = Vector3.Cross(cameraZ, cameraX);
            float len = cameraY.LengthSquared();
            if (len > 0)
                cameraY.Normalize();
            else
                cameraY = new Vector3(0, 1, 0);

            Matrix t1 = Matrix.CreateTranslation(-eye);
            Matrix r = Matrix.CreateFromAxisAngle(cameraY, angle);
            Matrix t2 = Matrix.CreateTranslation(eye);

            Matrix M = t1 * r * t2;
            center = Vector3.Transform(center, M);
            ComputeView();
        }

        public void Update(GameTime gameTime)
        {
            /*MouseState mouseState = Mouse.GetState();

            if (mousePitchYaw && mouseState.LeftButton == ButtonState.Pressed &&
                lastMouseState.LeftButton == ButtonState.Pressed)
            {
                float changeY = mouseState.Y - lastMouseState.Y;
                float changeX = mouseState.X - lastMouseState.X;
                Pitch(-changeY * 0.005f);
                Yaw(-changeX * 0.005f);
            }

            if (mousePanTilt && mouseState.RightButton == ButtonState.Pressed &&
                lastMouseState.RightButton == ButtonState.Pressed)
            {
                float changeY = mouseState.Y - lastMouseState.Y;
                float changeX = mouseState.X - lastMouseState.X;
                Tilt(-changeY * 0.0005f);
                Pan(-changeX * 0.0005f);
            }

            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            if (padPitchYaw)
            {
                Yaw(-gamePadState.ThumbSticks.Right.X * 0.05f);
                Pitch(gamePadState.ThumbSticks.Right.Y * 0.05f);
            }

            lastMouseState = mouseState;*/
        }

        private void ComputeProjection()
        {
            projection = Matrix.CreatePerspectiveFieldOfView(fov,
                graphics.GraphicsDevice.Viewport.AspectRatio, znear, zfar);
        }
    }
}
