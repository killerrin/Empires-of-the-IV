﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Anarian.Interfaces;
using Anarian.DataStructures.Components;

namespace Anarian.DataStructures
{
    public class Level : AnarianObject,
                         IScene, IUpdatable, IRenderable
    {
        private Camera m_camera;
        private Transform m_sceneNode;
        public Camera Camera
        {
            get { return m_camera; }
            set { m_camera = value; }
        }
        public Transform SceneNode
        {
            get { return m_sceneNode; }
            protected set { m_sceneNode = value; }
        }


        public Level(GraphicsDeviceManager graphics)
            :base()
        {
            // Create the Camera using the Graphics Device
            m_camera = new Camera();
            m_camera.AspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;

            // When Creating the Base SceneNode, we will set
            // its Scale to Zero so that SceneNodes which get
            // added as children aren't screwed up
            GameObject node = new GameObject();
            node.Transform.Scale = Vector3.Zero;
            m_sceneNode = node.Transform;
        }

        #region Interface Implimentation
        Camera IScene.Camera
        {
            get { return Camera; }
            set { Camera = value; }
        }

        Transform IScene.SceneNode
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
            m_sceneNode.GameObject.Update(gameTime);
        }

        /// <summary>
        /// Draws the Level
        /// </summary>
        /// <param name="gameTime">The GameTime</param>
        /// <param name="camera">Set Camera to null to use Main Camera</param>
        /// <param name="graphics">The GraphicsDeviceManager</param>
        public void Draw(GameTime gameTime, Camera camera, GraphicsDeviceManager graphics)
        {
            m_sceneNode.GameObject.Draw(gameTime, camera, graphics);
        }
    }
}
