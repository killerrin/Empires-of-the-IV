using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Anarian.Interfaces;
using Anarian.Enumerators;
using Anarian.Helpers;

using Microsoft.Xna.Framework;

namespace Anarian.DataStructures.Components
{
    public class Transform : Component,
                             IUpdatable, IMoveable
    {
        #region Vectors
        Vector3 m_orbitalRotation;
        public Vector3 OrbitalRotation
        {
            get { return m_orbitalRotation; }
            set { m_orbitalRotation = value; }
        }

        Vector3 m_rotation;
        public Vector3 Rotation
        {
            get { return m_rotation; }
            set { m_rotation = value; }
        }

        Vector3 m_scale;
        public Vector3 Scale
        {
            get { return m_scale; }
            set { m_scale = value; }
        }

        Vector3 m_position;
        public Vector3 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        #region WorldVectors
        public Vector3 WorldPosition
        {
            get
            {
                Vector3 pos = m_position;

                if (m_parent != null) {
                    pos += m_parent.WorldPosition;
                }
                return pos;
            }
        }

        public Vector3 WorldRotation
        {
            get
            {
                Vector3 rot = m_rotation;

                if (m_parent != null) {
                    rot += m_parent.WorldRotation;
                }
                return rot;
            }
        }

        public Vector3 WorldOrbitalRotation
        {
            get
            {
                Vector3 rot = m_orbitalRotation;

                if (m_parent != null) {
                    rot += m_parent.WorldOrbitalRotation;
                }
                return rot;
            }
        }

        public Vector3 WorldScale
        {
            get
            {
                Vector3 sca = m_scale;

                if (m_parent != null) {
                    sca += m_parent.WorldScale;
                }
                return sca;
            }
        }
        #endregion
        #endregion

        #region Matrices
        private Matrix m_worldMatrix;
        public Matrix WorldMatrix
        {
            get { return m_worldMatrix; }
            protected set { m_worldMatrix = value; }
        }

        private Matrix m_scaleMatrix;
        public Matrix ScaleMatrix
        {
            get { return m_scaleMatrix; }
            set { m_scaleMatrix = value; }
        }

        private Matrix m_rotationMatrix;
        public Matrix RotationMatrix
        {
            get { return m_rotationMatrix; }
            set { m_rotationMatrix = value; }
        }

        private Matrix m_translationMatrix;
        public Matrix TranslationMatrix
        {
            get { return m_translationMatrix; }
            set { m_translationMatrix = value; }
        }

        private Matrix m_orbitalRotationMatrix;
        public Matrix OrbitalRotationMatrix
        {
            get { return m_orbitalRotationMatrix; }
            set { m_orbitalRotationMatrix = value; }
        }

        #region Matrix Helpers
        protected void CreateScaleMatrix()
        {
            m_scaleMatrix = Matrix.CreateScale(WorldScale);
        }
        protected void CreateRotationMatrix()
        {
            Vector3 worldRotation = WorldRotation;
            Matrix rotX = Matrix.CreateRotationX(worldRotation.X);
            Matrix rotY = Matrix.CreateRotationY(worldRotation.Y);
            Matrix rotZ = Matrix.CreateRotationZ(worldRotation.Z);
            m_rotationMatrix = rotX * rotY * rotZ;
        }
        protected void CreateTranslationMatrix()
        {
            m_translationMatrix = Matrix.CreateTranslation(WorldPosition);
        }
        protected void CreateOrbitalRotationMatrix()
        {
            Vector3 worldOrbitalRotation = WorldOrbitalRotation;
            Matrix rotOX = Matrix.CreateRotationX(worldOrbitalRotation.X);
            Matrix rotOY = Matrix.CreateRotationY(worldOrbitalRotation.Y);
            Matrix rotOZ = Matrix.CreateRotationZ(worldOrbitalRotation.Z);
            m_orbitalRotationMatrix = rotOX * rotOY * rotOZ;
        }
        public void CreateWorldMatrix()
        {
            m_worldMatrix = m_scaleMatrix * m_rotationMatrix * m_translationMatrix * m_orbitalRotationMatrix;
        }

        public void CreateAllMatrices()
        {
            CreateRotationMatrix();
            CreateScaleMatrix();
            CreateTranslationMatrix();
            CreateOrbitalRotationMatrix();
            CreateWorldMatrix();
        }

        #endregion
        #endregion

        public Transform(GameObject gameObject)
            :base(gameObject, ComponentTypes.Transform)
        {
            m_position = Vector3.Zero;
            m_rotation = Vector3.Zero;
            m_scale = Vector3.One;

            Setup();
        }

        public Transform(GameObject gameObject, Vector3 position, Vector3 scale, Vector3 rotation)
            : base(gameObject, ComponentTypes.Transform)
        {
            m_position = position;
            m_rotation = scale;
            m_scale = rotation;

            Setup();
        }

        public override void Reset()
        {
            base.Reset();

            m_position = Vector3.Zero;
            m_rotation = Vector3.Zero;
            m_scale = Vector3.One;

            Setup();
        }

        private void Setup(GameObject gameObject = null)
        {
            m_orbitalRotation = Vector3.Zero;

            CreateRotationMatrix();
            CreateScaleMatrix();
            CreateTranslationMatrix();
            CreateOrbitalRotationMatrix();
            CreateWorldMatrix();

            // Setup Children
            m_parent = null;
            m_children = new List<Transform>();
        }

        #region Interfaces
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }

        void IMoveable.Move(GameTime gameTime, Vector3 movement) { Move(gameTime, movement); }
        void IMoveable.MoveVertical(GameTime gameTime, float amount) { MoveVertical(gameTime, amount); }
        void IMoveable.MoveHorizontal(GameTime gameTime, float amount) { MoveHorizontal(gameTime, amount); }
        void IMoveable.MoveForward(GameTime gameTime, float amount) { MoveForward(gameTime, amount); }
        void IMoveable.MoveToPosition(GameTime gameTime, Vector3 point) { MoveToPosition(gameTime, point); }
        #endregion

        public override void Update(GameTime gameTime)
        {
            CreateAllMatrices();
        }

        #region Movements
        public void Move(GameTime gameTime, Vector3 movement)
        {
            MoveHorizontal(gameTime, movement.X);
            MoveVertical(gameTime, movement.Y);
            MoveForward(gameTime, movement.Z);
        }

        public void MoveHorizontal(GameTime gameTime, float amount) { }
        public void MoveVertical(GameTime gameTime, float amount) { }
        public void MoveForward(GameTime gameTime, float amount) { }

        public void MoveToPosition(GameTime gameTime, Vector3 point)
        {
            Vector3 direction = point - m_position;
            direction.Normalize();

            Vector3 speed = direction * 0.002f;
            m_position += speed * gameTime.DeltaTime();

            // Rotate to the point
            RotateToPoint(gameTime, point);
        }
        #endregion

        #region Rotations
        public void RotateToPoint(GameTime gameTime, Vector3 point)
        {
            RotationMatrix = Matrix.CreateLookAt(Vector3.Zero, point, Vector3.Up);
            RotationMatrix = Matrix.Transpose(RotationMatrix);
        }
        #endregion

        #region Parent/Children
        Transform m_parent;
        Transform Parent
        {
            get { return m_parent; }
            set { m_parent = value; }
        }

        List<Transform> m_children;
        public List<Transform> GetChildren() { return m_children; }
        public void AddChild(Transform child)
        {
            child.Parent = this;
            m_children.Add(child);
        }
        public void RemoveChild(int index) { m_children.RemoveAt(index); }
        public Transform GetChild(int index) { return m_children[index]; }
        #endregion
    }
}
