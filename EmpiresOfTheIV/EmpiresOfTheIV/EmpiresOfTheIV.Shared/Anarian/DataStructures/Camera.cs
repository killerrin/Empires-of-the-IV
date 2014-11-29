using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Anarian.Interfaces;

namespace Anarian.DataStructures
{
    public class Camera : IMoveable
    {
        #region View
        Vector3 m_eye;
        Vector3 m_up;
        Vector3 m_lookAt;

        public Vector3 Eye
        {
            get { return m_eye; }
            set { CreateViewMatrix(value, m_lookAt, m_up); }
        }
        public Vector3 LookAt
        {
            get { return m_lookAt; }
            set { CreateViewMatrix(m_eye, value, m_up); }
        }
        public Vector3 Up
        {
            get { return m_up; }
            set { CreateViewMatrix(m_eye, m_lookAt, value); }
        }

        public void CreateViewMatrix(Vector3 eye, Vector3 lookat, Vector3 up)
        {
            m_eye = eye;
            m_lookAt = lookat;
            m_up = up;
            m_view = Matrix.CreateLookAt(m_eye, m_lookAt, m_up);

            // Calculate the YawPitch from the new settings
            CalculateYawPitch();
        }

        float m_yaw;
        float m_pitch;
        public float Yaw
        {
            get { return m_yaw; }
            set { m_yaw = value; }
        }
        public float Pitch
        {
            get { return m_pitch; }
            set { m_pitch = value; }
        }

        public void CalculateYawPitch()
        {
            Vector3 dir = m_lookAt - m_eye;
            dir.Normalize();
            Vector3 m = dir; m.Y = m_eye.Y;

            // Calculate Yaw
            m_yaw = (float)Math.Atan2(dir.X, dir.Z);

            // Calculate Pitch
            float len = (new Vector2(m.X, m.Z)).Length();
            m_pitch = (float)Math.Atan2(dir.Y, len);
        }
        #endregion

        #region Projection
        float m_fov;
        float m_aspectRatio;
        float m_zNear;
        float m_zFar;

        public float FoV
        {
            get { return m_fov; }
            set { CreateProjectionMatrix(value, m_aspectRatio, m_zNear, m_zFar); }
        }
        public float AspectRatio
        {
            get { return m_aspectRatio; }
            set { CreateProjectionMatrix(m_fov, value, m_zNear, m_zFar); }
        }
        public float Near
        {
            get { return m_zNear; }
            set { CreateProjectionMatrix(m_fov, m_aspectRatio, value, m_zFar); }
        }
        public float Far
        {
            get { return m_zFar; }
            set { CreateProjectionMatrix(m_fov, m_aspectRatio, m_zNear, value); }
        }

        public void CreateProjectionMatrix(float fov, float aspectRatio, float near, float far)
        {
            m_fov = fov;
            m_aspectRatio = aspectRatio;
            m_zNear = near;
            m_zFar = far;
            m_projection = Matrix.CreatePerspectiveFieldOfView(m_fov, m_aspectRatio, m_zNear, m_zFar);
        }
        #endregion

        #region Matricies
        Matrix m_view;
        Matrix m_projection;
        Matrix m_world;

        public Matrix View { get { return m_view; } }
        public Matrix Projection { get { return m_projection; } }
        public Matrix World { get { return m_world; } }
        #endregion

        #region Helper Properties
        public BoundingFrustum GetFrustum { get { return new BoundingFrustum(View * Projection); } }

        Vector3 Direction { get { return Vector3.Normalize(m_lookAt - m_eye); } }
        public Vector3 Right { get { return Vector3.Normalize(Vector3.Cross(this.Direction, this.Up)); } }
        #endregion

        public Camera()
        {
            CreateViewMatrix(
                new Vector3(0.0f, 0.7f, 1.5f),      // eye
                new Vector3(0.0f, 0.0f, 0.0f),      // look at
                new Vector3(0.0f, 1.0f, 0.0f)       // up
                );
            CreateProjectionMatrix(
                MathHelper.Pi * 70.0f / 180.0f,     // fov
                1.45f,                              // aspect ratio
                0.001f,                             // near
                1000.0f                             // far
                );
            m_world = Matrix.Identity;
        }
        public Camera(Vector3 eye, Vector3 lookat, Vector3 up,
                      float fov, float aspectRatio, float near, float far)
        {
            CreateViewMatrix(eye, lookat, up);
            CreateProjectionMatrix(fov, aspectRatio, near, far);
            m_world = Matrix.Identity;
        }


        public Ray GetMouseRay(Vector2 mousePosition, Viewport viewport)
        {
            Vector3 nearPoint = new Vector3(mousePosition, 0);
            Vector3 farPoint = new Vector3(mousePosition, 1);

            // Unproject these points into world space
            nearPoint = viewport.Unproject(nearPoint, Projection, View, World);
            farPoint = viewport.Unproject(farPoint, Projection, View, World);

            // Create the Ray and Return
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();

            return new Ray(nearPoint, direction);
        }

        public BoundingFrustum UnprojectRectangle(Rectangle source, Viewport viewport)
        {
            //http://forums.create.msdn.com/forums/p/6690/35401.aspx , by "The Friggm"
            // Many many thanks to him...

            // Point in screen space of the center of the region selected
            Vector2 regionCenterScreen = new Vector2(source.Center.X, source.Center.Y);

            // Generate the projection matrix for the screen region
            Matrix regionProjMatrix = Projection;

            // Calculate the region dimensions in the projection matrix. M11 is inverse of width, M22 is inverse of height.
            regionProjMatrix.M11 /= ((float)source.Width / (float)viewport.Width);
            regionProjMatrix.M22 /= ((float)source.Height / (float)viewport.Height);

            // Calculate the region center in the projection matrix. M31 is horizonatal center.
            regionProjMatrix.M31 = (regionCenterScreen.X - (viewport.Width / 2f)) / ((float)source.Width / 2f);

            // M32 is vertical center. Notice that the screen has low Y on top, projection has low Y on bottom.
            regionProjMatrix.M32 = -(regionCenterScreen.Y - (viewport.Height / 2f)) / ((float)source.Height / 2f);

            return new BoundingFrustum(View * regionProjMatrix);
        }

        #region Interface Implimentation
        void IMoveable.Move(GameTime gameTime, Vector3 movement)
        {
            MoveHorizontal(gameTime, movement.X);
            MoveVertical(gameTime, movement.Y);
            MoveForward(gameTime, movement.Z);
        }

        void IMoveable.MoveHorizontal(GameTime gameTime, float amount)
        {
            MoveHorizontal(gameTime, amount);
        }
        void IMoveable.MoveVertical(GameTime gameTime, float amount)
        {
            MoveVertical(gameTime, amount);
        }
        void IMoveable.MoveForward(GameTime gameTime, float amount)
        {
            MoveForward(gameTime, amount);
        }
        #endregion

        #region Camera Movement
        public void Move(GameTime gameTime, Vector3 movement)
        {
            MoveHorizontal(gameTime, movement.X);
            MoveVertical(gameTime, movement.Y);
            MoveForward(gameTime, movement.Z);
        }

        public void MoveHorizontal(GameTime gameTime, float amount)
        {
            m_eye += amount * this.Right;
            m_lookAt += amount * this.Right;
            CreateViewMatrix(m_eye, m_lookAt, m_up);
        }
        public void MoveVertical(GameTime gameTime, float amount)
        {
            m_eye += amount * this.Up;
            m_lookAt += amount * this.Up;
            CreateViewMatrix(m_eye, m_lookAt, m_up); 
        }
        public void MoveForward(GameTime gameTime, float amount)
        {
            Vector3 temp = m_eye;
            m_eye += amount * this.Direction;
            m_lookAt += amount * this.Direction;
            CreateViewMatrix(m_eye, m_lookAt, m_up);
        }

        public void AddYaw(float angle)
        {
            m_yaw += angle;
            Vector3 dir = this.Direction;
            dir = Vector3.Transform(dir, Matrix.CreateFromAxisAngle(this.Up, angle));

            m_lookAt = m_eye + Vector3.Distance(m_lookAt, m_eye) * dir;
            CreateViewMatrix(m_eye, m_lookAt, m_up);
        }

        public void AddPitch(float angle)
        {
            if (Math.Abs(m_pitch + angle) >= MathHelper.ToRadians(80)) return;
            m_pitch += angle;
            Vector3 dir = this.Direction;
            dir = Vector3.Transform(dir, Matrix.CreateFromAxisAngle(this.Right, angle));

            m_lookAt = m_eye + Vector3.Distance(m_lookAt, m_eye) * dir;
            CreateViewMatrix(m_eye, m_lookAt, m_up);
        }

        public void Levitate(float amount)
        {
            m_eye.Y += amount;
            m_lookAt.Y += amount;
            CreateViewMatrix(m_eye, m_lookAt, m_up);
        }
        #endregion
    }

}
