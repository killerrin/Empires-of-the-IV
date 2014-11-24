using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Anarian.Interfaces;

namespace Anarian.DataStructures
{
    public class Level : IScene
    {
        private Camera m_camera;
        private GameObject m_sceneNode;

        public Level(GraphicsDeviceManager graphics)
        {
            // Create the Camera using the Graphics Device
            m_camera = new Camera();
            m_camera.AspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;

            // When Creating the Base SceneNode, we will set
            // its Scale to Zero so that SceneNodes which get
            // added as children aren't screwed up
            m_sceneNode = new GameObject();
            m_sceneNode.Scale = Vector3.Zero;
        }

        #region Interface Implimentation
        Camera IScene.GetCamera() { return Camera; }
        void IScene.SetCamera(Camera cam) { Camera = cam; }
        GameObject IScene.GetSceneNode() { return SceneNode; }
        #endregion

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
    }
}
