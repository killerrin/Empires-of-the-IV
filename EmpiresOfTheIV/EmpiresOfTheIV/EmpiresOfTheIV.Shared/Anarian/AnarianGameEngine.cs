using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Anarian;
using Anarian.DataStructures;
using Anarian.Interfaces;
using Anarian.Helpers;

namespace Anarian
{
    public class AnarianGameEngine : Game
    {
        protected GraphicsDeviceManager graphics;
        protected SpriteBatch spriteBatch;

        protected SceneManager m_sceneManager;
        protected ResourceManager m_resourceManager;

        protected Color m_backgroundColor;

        public AnarianGameEngine()
            :base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            m_sceneManager = SceneManager.Instance;
            m_resourceManager = ResourceManager.Instance;
         
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Set the Background Color
            m_backgroundColor = Color.CornflowerBlue;

            // Create the Scene
            m_sceneManager.CurrentScene = new Level(graphics);

            // Create Textures which will be needed in the engine
            // Blank Texture
            Texture2D blankTexture = new Texture2D(GraphicsDevice, 1, 1);
            blankTexture.SetData(new Color[] { Color.White });
            m_resourceManager.AddTexture(blankTexture, "blankTexture_age");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (m_sceneManager != null) {
                if (m_sceneManager.CurrentScene != null) {
                    m_sceneManager.CurrentScene.GetSceneNode().Update(gameTime);
                }
            }

            base.Update(gameTime);
        }

        protected virtual void PreDraw(GameTime gameTime)
        {
            GraphicsDevice.Clear(m_backgroundColor);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Root Scene Node
            if (m_sceneManager != null) {
                if (m_sceneManager.CurrentScene != null) {
                    m_sceneManager.CurrentScene.GetSceneNode().Draw(
                        gameTime,
                        m_sceneManager.CurrentScene.GetCamera(),
                        graphics);
                }
            }
        }

        protected virtual void PostDraw(GameTime gameTime)
        {
            // Since we don't call MonoGame.Draw on the Draw method,
            // We call it here so that the screen will render
            base.Draw(gameTime);
        }
    }
}
