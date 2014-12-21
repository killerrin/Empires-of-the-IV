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
        protected AnimatedModel m_model;
        public AnimatedModel Model3D
        {
            get { return m_model; }
            set { m_model = value; }
        }

        protected AnimationPlayer m_animationPlayer;
        public AnimationPlayer AnimationPlayer
        {
            get { return m_animationPlayer; }
            set { m_animationPlayer = value; }
        }

        public AnimatedGameObject()
            :base()
        {

        }

        public override bool CheckRayIntersection(Ray ray)
        {
            // Generate the bounding boxes
            m_boundingBoxes = new List<BoundingBox>();

            // Create the ModelTransforms
            Matrix[] modelTransforms = new Matrix[Model3D.Bones.Count];
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
        /// <summary>
        /// Play an animation clip
        /// </summary>
        /// <param name="clip">The clip to play</param>
        /// <returns>The player that will play this clip</returns>
        public AnimationPlayer PlayClip(AnimationClip clip) { 
            m_animationPlayer = Model3D.PlayClip(clip);
            return m_animationPlayer;
        }
        #endregion

        #region Update/Draw
        private void UpdateAnimation(GameTime gameTime)
        {
            //Model3D.AnimationPlayer = m_animationPlayer;
            Model3D.Update(gameTime);
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

            // We Update the Animation here so that we can have several different
            // GameObjects Animating at different paces in their animations
            //UpdateAnimation(gameTime);

            // Finally, we render This Object
            Model3D.Draw(graphics.GraphicsDevice, camera.View, camera.Projection, Transform.WorldMatrix);
        }
        #endregion
    }
}
