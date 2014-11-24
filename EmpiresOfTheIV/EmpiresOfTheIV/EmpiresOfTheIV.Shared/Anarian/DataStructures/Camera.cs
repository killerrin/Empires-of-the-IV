using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Anarian.DataStructures
{
    public class Camera
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
        #endregion

        #region Matricies
        Matrix m_view;
        Matrix m_projection;
        public Matrix View { get { return m_view; } }
        public Matrix Projection { get { return m_projection; } }
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
        }

        public Camera(Vector3 eye, Vector3 lookat, Vector3 up,
                      float fov, float aspectRatio, float near, float far)
        {
            CreateViewMatrix(eye, lookat, up);
            CreateProjectionMatrix(fov, aspectRatio, near, far);
        }

        public void CreateViewMatrix(Vector3 eye, Vector3 lookat, Vector3 up)
        {
            m_eye = eye;
            m_lookAt = lookat;
            m_up = up;
            m_view = Matrix.CreateLookAt(m_eye, m_lookAt, m_up);
        }

        public void CreateProjectionMatrix(float fov, float aspectRatio, float near, float far)
        {
            m_fov = fov;
            m_aspectRatio = aspectRatio;
            m_zNear = near;
            m_zFar = far;
            m_projection = Matrix.CreatePerspectiveFieldOfView(m_fov, m_aspectRatio, m_zNear, m_zFar);
        }
    }

}
