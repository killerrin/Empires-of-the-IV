using System;
using System.Collections.Generic;
using System.Text;
using Anarian.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Anarian.DataStructures;

namespace Anarian
{
    public class SceneManager : IUpdatable, IRenderable
    {
        #region Singleton
        static SceneManager m_instance;
        public static SceneManager Instance
        {
            get
            {
                if (m_instance == null) m_instance = new SceneManager();
                return m_instance;
            }
            set { }
        }
        #endregion

        private SceneManager()
        {
            CurrentScene = null;
        }

        public IScene CurrentScene { get; set; }

        #region Interfaces
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, Camera camera, GraphicsDeviceManager graphics) { Draw(gameTime, camera, graphics); }
        #endregion

        public void Update(GameTime gameTime)
        {
            if (CurrentScene != null) {
                CurrentScene.SceneNode.GameObject.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, Camera camera, GraphicsDeviceManager graphics)
        {
            if (CurrentScene != null) {
                CurrentScene.SceneNode.GameObject.Draw(
                    gameTime,
                    camera,
                    graphics);
            }
        }

    }
}
