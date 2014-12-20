using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Anarian.Interfaces;
using Anarian.Helpers;

namespace Anarian.DataStructures
{
    public class GameObject : IUpdatable, IRenderable
    {
        #region Fields/Properties
        bool    m_active;
        bool    m_visible;

        Transform m_transform;

        Model   m_model;
        List<BoundingBox> m_boundingBoxes;

        public bool Active
        {
            get { return m_active; }
            set { m_active = value; }
        }
        public bool Visible
        {
            get { return m_visible; }
            set { m_visible = value; }
        }

        public Transform Transform
        {
            get { return m_transform; }
            protected set { m_transform = value; }
        }

        public Model Model3D
        {
            get { return m_model; }
            set { m_model = value; }
        }
        #endregion

        public GameObject()
        {
            // Setup Defaults
            m_active    = true;
            m_visible   = true;

            // Setup the Transform
            m_transform = new Transform(this);

            // Setup Bounding Boxes
            m_boundingBoxes = new List<BoundingBox>();
        }

        public bool CheckRayIntersection(Ray ray)
        {
            // Generate the bounding boxes
            m_boundingBoxes = new List<BoundingBox>();
            
            // Create the ModelTransforms
            Matrix[] modelTransforms = new Matrix[Model3D.Bones.Count];
            Model3D.CopyAbsoluteBoneTransformsTo(modelTransforms);

            // Check intersection
            foreach (ModelMesh mesh in Model3D.Meshes) {
                //BoundingSphere boundingSphere = mesh.BoundingSphere.Transform(modelTransforms[mesh.ParentBone.Index] * WorldMatrix);
                BoundingBox boundingBox = mesh.GenerateBoundingBox(m_transform.WorldMatrix);
                m_boundingBoxes.Add(boundingBox);

                if (ray.Intersects(boundingBox) != null) return true;
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

            // Then do the Children
            foreach (var child in m_transform.GetChildren()) {
                child.GameObject.Update(gameTime);
            }

            // Update the Transform
            m_transform.Update(gameTime);
        }
        public void Draw(GameTime gameTime, Camera camera, GraphicsDeviceManager graphics)
        {
            if (!m_active) return;


            // Render the Children
            foreach (var child in m_transform.GetChildren()) {
                if (child != null) child.GameObject.Draw(gameTime, camera, graphics);
            }


            // Now that the children have been rendered...
            // We check if we are visible on the screen,
            // We check if we have a model,
            // Then we render it
            if (!m_visible) return;
            if (m_model == null) return;

            // Check Against Frustrum to cull out objects
            for (int i = 0; i < m_boundingBoxes.Count; i++) {
                if (!m_boundingBoxes[i].Intersects(camera.Frustum)) return;
            }

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
                    effect.World = transforms[mesh.ParentBone.Index] * 
                                   m_transform.WorldMatrix;
                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();

                //mesh.BoundingSphere.RenderBoundingSphere( graphics.GraphicsDevice, WorldMatrix, camera.View, camera.Projection, Color.Red);
                for (int i = 0; i < m_boundingBoxes.Count; i++) {
                    m_boundingBoxes[i].DrawBoundingBox(graphics, Color.Red, camera, Matrix.Identity);
                }
            }
        }
        #endregion
    }
}
