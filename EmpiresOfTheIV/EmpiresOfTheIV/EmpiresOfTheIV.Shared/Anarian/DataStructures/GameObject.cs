using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Anarian.Interfaces;

namespace Anarian.DataStructures
{
    public class GameObject : IUpdatable, IRenderable
    {
        #region Properties
        bool    m_active;
        Model   m_model;

        public bool Active
        {
            get { return m_active; }
            set { m_active = value; }
        }
        public Model Model3D
        {
            get { return m_model; }
            set { m_model = value; }
        }
        #endregion

        #region Translations
        Vector3 m_rotation;
        public Vector3 Rotation
        {
            get { return m_rotation; }
            set { m_rotation = value; }
        }

        Vector3 m_scale;
        public Vector3 Scale {
            get { return m_scale; }
            set { m_scale = value; }
        }

        Vector3 m_position;
        public Vector3 Position {
            get { return m_position; }
            set { m_position = value; }
        }

        public Matrix WorldMatrix
        {
            get
            {
                Matrix scale = Matrix.CreateScale(WorldScale);

                Vector3 worldRotation = WorldRotation;
                Matrix rotX = Matrix.CreateRotationX(worldRotation.X);
                Matrix rotY = Matrix.CreateRotationY(worldRotation.Y);
                Matrix rotZ = Matrix.CreateRotationZ(worldRotation.Z);
                Matrix rotation = rotX * rotY * rotZ;

                Matrix translation = Matrix.CreateTranslation(WorldPosition);

                return scale * rotation * translation;
            }
            set { }
        }

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
            set { }
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
            set { }
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
            set { }
        }
        #endregion

        public GameObject()
        {
            m_parent    = null;
            m_active    = true;
            
            m_rotation  = Vector3.Zero;
            m_position  = Vector3.Zero;
            m_scale     = Vector3.One;

            m_children  = new List<GameObject>();
        }

        public bool CheckRayIntersection(Ray ray)
        {
            BoundingSphere boundingSphere;

            // Create the ModelTransforms
            Matrix[] modelTransforms = new Matrix[Model3D.Bones.Count];
            Model3D.CopyAbsoluteBoneTransformsTo(modelTransforms);

            // Check intersection
            foreach (ModelMesh mesh in Model3D.Meshes) {
                boundingSphere = mesh.BoundingSphere.Transform(modelTransforms[mesh.ParentBone.Index] * WorldMatrix);
                if (ray.Intersects(boundingSphere) != null) return true;
            }
            return false;
        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, Camera camera, GraphicsDeviceManager graphics) { Draw(gameTime, camera, graphics); }
        #endregion

        #region Update/Draw
        public void Update(GameTime gameTime)
        {
            if (!m_active) return;
            foreach (GameObject gO in m_children) {
                gO.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, Camera camera, GraphicsDeviceManager graphics)
        {
            if (!m_active) return;


            // Render the Children
            foreach (GameObject gO in m_children) {
                if (gO != null) gO.Draw(gameTime, camera, graphics);
            }


            // Now that the children have been rendered,
            // We check if we have a model, then we render it
            if (m_model == null) return;

            // Render This Object
            //Debug.WriteLine("Rendering Model Pos:{0}, Sca:{1}, Rot:{2}", WorldPosition, WorldScale, WorldRotation);
            
            // Since we are also using 2D, Reset the
            // Graphics Device to Render 3D Models properly
            GraphicsDevice graphicsDevice = graphics.GraphicsDevice;
            graphicsDevice.BlendState = BlendState.Opaque;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[m_model.Bones.Count];
            m_model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in m_model.Meshes) {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects) {
                    effect.EnableDefaultLighting();
                    //effect.LightingEnabled = false;// EnableDefaultLighting();
                    effect.DiffuseColor = new Vector3(1, 1, 1);
                    effect.PreferPerPixelLighting = true;
                    effect.World = transforms[mesh.ParentBone.Index]
                        * WorldMatrix;
                    //* Matrix.CreateScale(20f);
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }
        #endregion

        #region Parent/Children
        GameObject m_parent;
        GameObject Parent { 
            get { return m_parent; }
            set { m_parent = value; }
        }

        List<GameObject> m_children;
        public List<GameObject> GetChildren() { return m_children; }
        public void AddChild(GameObject child) {
            child.Parent = this;
            m_children.Add(child); 
        }
        public void RemoveChild(int index) { m_children.RemoveAt(index); }
        public GameObject GetChild(int index) { return m_children[index]; }
        #endregion
    }
}
