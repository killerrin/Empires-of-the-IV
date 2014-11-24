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

namespace EmpiresOfTheIV
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SceneManager m_sceneManager;
        ResourceManager m_resourceManager;

        public Game1()
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

            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
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

            // Load the Assets
            m_resourceManager.LoadTexture(Content, "KillerrinStudiosLogo");
            m_resourceManager.LoadModel(Content, "t-pose_3");

            // Create the Game Objects
            GameObject armyGuy = new GameObject();
            armyGuy.Model3D = m_resourceManager.GetModel("t-pose_3");
            armyGuy.Scale = new Vector3(0.007f);
            armyGuy.Position = new Vector3(0.0f, -0.5f, 0.0f);

            // Create the Scene
            m_sceneManager.CurrentScene = new Level(graphics);
            m_sceneManager.CurrentScene.GetSceneNode().AddChild(armyGuy);
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
                    m_sceneManager.CurrentScene.GetSceneNode().Rotation += new Vector3(0.0f, (0.0025f) * (float)gameTime.ElapsedGameTime.TotalMilliseconds, 0.0f);


                    // Finally, Update the Base Scene node
                    m_sceneManager.CurrentScene.GetSceneNode().Update(gameTime);
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Texture2D logo = ResourceManager.Instance.GetTexture("KillerrinStudiosLogo");

            // Draw Texture
            spriteBatch.Begin();
            spriteBatch.Draw(logo, new Vector2(0.0f, 0.0f), Color.White);
            spriteBatch.End();

            // Model
            if (m_sceneManager != null) {
                if (m_sceneManager.CurrentScene != null) {
                    m_sceneManager.CurrentScene.GetSceneNode().Draw(
                        gameTime,
                        m_sceneManager.CurrentScene.GetCamera(),
                        graphics);
                }
            }

            base.Draw(gameTime);
        }
    }
}
