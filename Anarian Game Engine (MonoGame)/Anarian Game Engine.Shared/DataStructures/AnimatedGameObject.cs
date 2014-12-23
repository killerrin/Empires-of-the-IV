using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Anarian.DataStructures.Animation;
using Anarian.DataStructures.Components;
using Anarian.Interfaces;
using Anarian.Helpers;
using Anarian.DataStructures.Animation.Aux;

namespace Anarian.DataStructures
{
    public class AnimatedGameObject : GameObject, IUpdatable, IRenderable
    {
        #region Fields/Properties
        protected AnimatedModel m_model;
        public AnimatedModel Model3D
        {
            get { return m_model; }
            set 
            { 
                m_model = value;
                CreateAnimationState();
            }
        }

        protected AnimationState m_animationState;
        public AnimationState AnimationState
        {
            get { return m_animationState; }
            set { m_animationState = value; }
        }
        public void CreateAnimationState() { if (m_model != null) m_animationState = new AnimationState(m_model); }
        #endregion

        public AnimatedGameObject()
            :base()
        {

        }

        public override bool CheckRayIntersection(Ray ray)
        {
            // Generate the bounding boxes
            m_boundingBoxes = new List<BoundingBox>();

            // Create the ModelTransforms
            Matrix[] modelTransforms = new Matrix[m_animationState.Bones.Count];
            Model3D.Model.CopyAbsoluteBoneTransformsTo(modelTransforms);

            // Check intersection
            foreach (ModelMesh mesh in Model3D.Model.Meshes) {
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

        #region Animation Helpers
        public AnimationPlayer PlayClip(AnimationClip clip) {
            return m_animationState.PlayClip(clip);
        }
        #endregion

        #region Update/Draw
        private void UpdateAnimation(GameTime gameTime)
        {
            m_animationState.Update(gameTime);
            m_model.Update(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (!m_active) return;
            
            // We first Update the Children
            base.Update(gameTime);
            
            // Now we update the Animation
            UpdateAnimation(gameTime);
        }

        public override void Draw(GameTime gameTime, Camera camera, GraphicsDeviceManager graphics)
        {
            if (!m_active) return;

            // We Draw the base here so that the Children get taken care of
            base.Draw(gameTime, camera, graphics);

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

            // Finally, we render This Object
            Model3D.Draw(graphics.GraphicsDevice, camera.View, camera.Projection, Transform.WorldMatrix, m_animationState);
        }
        #endregion
    }
}
