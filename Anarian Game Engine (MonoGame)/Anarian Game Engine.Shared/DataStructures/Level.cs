using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Anarian.Interfaces;

namespace Anarian.DataStructures
{
    public class Level : IScene, IUpdatable, IRenderable
    {
        private Camera m_camera;
        private GameObject m_sceneNode;
        public Camera Camera
        {
            get { return m_camera; }
            set { m_camera = value; }
        }
        public GameObject SceneNode
        {
            get { return m_sceneNode; }
            protected set { m_sceneNode = value; }
        }


        public Level(GraphicsDeviceManager graphics)
        {
            // Create the Camera using the Graphics Device
            m_camera = new Camera();
            m_camera.AspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;

            // When Creating the Base SceneNode, we will set
            // its Scale to Zero so that SceneNodes which get
            // added as children aren't screwed up
            m_sceneNode = new GameObject();
            m_sceneNode.Transform.Scale = Vector3.Zero;
        }

        #region Interface Implimentation
        Camera IScene.Camera
        {
            get { return Camera; }
            set { Camera = value; }
        }

        GameObject IScene.SceneNode
        {
            get { return SceneNode; }
            set { }
        }
        
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }

        /// <summary>
        /// Draws the Level
        /// </summary>
        /// <param name="gameTime">The GameTime</param>
        /// <param name="camera">Set Camera to null to use Main Camera</param>
        /// <param name="graphics">The GraphicsDeviceManager</param>
        void IRenderable.Draw(GameTime gameTime, Camera camera, GraphicsDeviceManager graphics) { Draw(gameTime, camera, graphics); }
        #endregion

        /// <summary>
        /// Updates the Level
        /// </summary>
        /// <param name="gameTime">The GameTime</param>
        public void Update(GameTime gameTime)
        {

        }

        /// <summary>
        /// Draws the Level
        /// </summary>
        /// <param name="gameTime">The GameTime</param>
        /// <param name="camera">Set Camera to null to use Main Camera</param>
        /// <param name="graphics">The GraphicsDeviceManager</param>
        public void Draw(GameTime gameTime, Camera camera, GraphicsDeviceManager graphics)
        {

        }
    }
}
