using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Anarian.Interfaces;
using Anarian.Enumerators;
using Anarian.Helpers;

using Microsoft.Xna.Framework;

namespace Anarian.DataStructures.Components
{
    public class Transform : Component,
                             IEnumerable, IUpdatable, IMoveable
    {
        #region Fields/Properties
        #region Vectors
        Vector3 m_orbitalRotation;
        public Vector3 OrbitalRotation
        {
            get { return m_orbitalRotation; }
            set { m_orbitalRotation = value; }
        }

        Quaternion m_rotation;
        public Quaternion Rotation
        {
            get { return m_rotation; }
            private set { m_rotation = value; }
        }

        Vector3 m_scale;
        public Vector3 Scale
        {
            get { return m_scale; }
            set { m_scale = value; }
        }

        Vector3 m_lastPosition;
        Vector3 m_position;
        public Vector3 Position
        {
            get { return m_position; }
            set {
                m_lastPosition = m_position;
                m_position = value;
            }
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

        public Quaternion WorldRotation
        {
            get
            {
                Quaternion rot = m_rotation;
        
                if (m_parent != null) {
                    rot *= m_parent.WorldRotation;
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
        private Matrix m_worldMatrix = Matrix.Identity;
        public Matrix WorldMatrix
        {
            get { return m_worldMatrix; }
            protected set { m_worldMatrix = value; }
        }

        private Matrix m_scaleMatrix = Matrix.Identity;
        public Matrix ScaleMatrix
        {
            get { return m_scaleMatrix; }
            set { m_scaleMatrix = value; }
        }

        private Matrix m_rotationMatrix = Matrix.Identity;
        public Matrix RotationMatrix
        {
            get { return m_rotationMatrix; }
            set { m_rotationMatrix = value; }
        }

        private Matrix m_translationMatrix = Matrix.Identity;
        public Matrix TranslationMatrix
        {
            get { return m_translationMatrix; }
            set { m_translationMatrix = value; }
        }

        private Matrix m_orbitalRotationMatrix = Matrix.Identity;
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
            //m_rotationMatrix = Matrix.CreateFromQuaternion(WorldRotation);

            //Vector3 worldRot = WorldRotation;
            //Matrix rotX = Matrix.CreateRotationX(worldRotation.X);
            //Matrix rotY = Matrix.CreateRotationY(worldRotation.Y);
            //Matrix rotZ = Matrix.CreateRotationZ(worldRotation.Z);
            //m_rotationMatrix = rotX * rotY * rotZ;
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

        #region Directions
        Vector3 m_forward;

        /// <summary>
        /// Defaults to Vector3.Forward
        /// </summary>
        public Vector3 Forward
        {
            get { return m_forward; }
            set { m_forward = value; }
        }

        Vector3 m_up;

        /// <summary>
        /// Defaults to Vector3.Up
        /// </summary>
        public Vector3 Up
        {
            get { return m_up; }
            set { m_up = value; }
        }

        Vector3 m_right;
        public Vector3 Right 
        {
            get { return m_right; }
            set { m_right = value; }
        }

        public Vector3 Back { get { return -m_forward; } }
        public Vector3 Down { get { return -m_up; } }
        public Vector3 Left { get { return -m_right; } }
        #endregion
        #endregion

        public Transform(GameObject gameObject)
            :base(gameObject, ComponentTypes.Transform)
        {
            Reset();
        }

        public Transform(GameObject gameObject, Vector3 position, Vector3 scale, Quaternion rotation)
            : base(gameObject, ComponentTypes.Transform)
        {
            m_position = position;
            m_rotation = rotation;
            m_scale = scale;

            Setup();
        }

        public override void Reset()
        {
            base.Reset();

            // Basic Transforms
            m_position = Vector3.Zero;
            m_rotation = Quaternion.Identity;
            m_scale = Vector3.One;

            Setup();
        }

        private void Setup()
        {
            // Setup Transform Vectors
            m_orbitalRotation = Vector3.Zero;
            m_lastPosition = m_position;

            // Setup the Direction Vectors
            m_forward = -Vector3.Forward;
            m_up = Vector3.Up;
            m_right = Vector3.Normalize(Vector3.Cross(Forward, this.Up));

            // Reset all Matrices
            m_orbitalRotationMatrix = Matrix.Identity;
            m_rotationMatrix = Matrix.Identity;
            m_scaleMatrix = Matrix.Identity;
            m_translationMatrix = Matrix.Identity;
            m_worldMatrix = Matrix.Identity;

            // Create the Matricies
            CreateAllMatrices();

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

        public IEnumerator GetEnumerator()
        {
            foreach (var child in m_children) {
                // Return the current element and then on next function call 
                // resume from next element rather than starting all over again;
                yield return child;
            }
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            CreateAllMatrices();
        }

        #region Helper Methods
        /// <summary>
        /// A Direction Vector facing a specified point
        /// </summary>
        /// <param name="point">The point we wish to face</param>
        /// <returns>A normalized direction vector</returns>
        public Vector3 CreateDirectionVector(Vector3 point)
        {
            Vector3 direction = point - m_position;
            direction.Normalize();

            return direction;
        }
        #endregion

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

        public bool MoveToPosition(GameTime gameTime, Vector3 point, float boundry = 0.025f)
        {
            //Debug.WriteLine(Vector3.Distance(m_position, point));
            
            // Check if we even need to enter the method or not
            if (Vector3.Distance(m_position, point) <= boundry) return true;

            Vector3 direction = CreateDirectionVector(point);

            Vector3 speed = direction * 0.002f;
            Position += speed * gameTime.DeltaTime();

            // Rotate to the point
            RotateToFaceForward(gameTime);

            // Exit out of the method signaling whether we have arrived yet
            if (Vector3.Distance(m_position, point) <= boundry) return true;
            return false;
        }
        #endregion

        #region Rotations
        public void RotateToFaceForward(GameTime gameTime) { RotateToPoint(gameTime, m_lastPosition); }
        public void RotateToPoint(GameTime gameTime, Vector3 point)
        {
            // the new forward vector, so the avatar faces the target
            Vector3 newForward = -(Vector3.Normalize(m_position - point));

            // Set the Forwards
            Forward = newForward;
            Right = Vector3.Cross(Forward, Up);

            // Rotate the Matrix
            Matrix rotMatrix = Matrix.Identity;
            rotMatrix.Forward = Forward;
            rotMatrix.Right = Right;
            rotMatrix.Up = Up;

            //float angle = (float)Math.Atan2(-newForward.X, -newForward.Z);
            //Quaternion rotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), angle);
            //m_rotation.Y = rotation.Y;

            m_rotationMatrix = rotMatrix;
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

        public override string ToString()
        {
            return base.ToString() + 
                   "Position: " + WorldPosition + " " +
                   "Rotation: " + WorldRotation + " " +
                   "Scale: " + WorldScale + " " +
                   "OrbitalRotation: " + WorldOrbitalRotation + " ";
        }
    }
}
