using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using KillerrinStudiosToolkit;

using Anarian;
using Anarian.DataStructures;
using Anarian.DataStructures.Animation;
using Anarian.DataStructures.Animation.Aux;
using Anarian.DataStructures.Input;
using Anarian.DataStructures.Rendering;
using Anarian.Enumerators;
using Anarian.Helpers;
using Anarian.Interfaces;
using EmpiresOfTheIV.Game;
using EmpiresOfTheIV.Game.GameObjects;
using Anarian.DataStructures.ScreenEffects;

namespace EmpiresOfTheIV
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class EmpiresOfTheIVGame : AnarianGameEngine
    {
        protected GameManager m_gameManager;
        public GameManager GameManager { get { return m_gameManager; } protected set { m_gameManager = value; } }

        public EmpiresOfTheIVGame()
            :base()
        {
            Content.RootDirectory = "Content";
            m_gameManager = new GameManager(this);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Initialize the Game Engine
            base.Initialize();

            // Initialize Game Specific Managers
            m_gameManager.Initialize();

            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

#if WINDOWS_APP
            this.IsMouseVisible = true;
#endif

            // Lastly we call PostInitialize to do MonoGame Initializations
            base.PostInitialize();
        }

        public Fade fadeEffect;

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            m_gameManager.LoadContent();

            fadeEffect = new Fade(GraphicsDevice, Color.Red);

            // Lastly we call PostLoadContent to do MonoGame LoadContent
            base.PostLoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Update the GameEngine
            base.PreUpdate(gameTime);

            // Update the SceneNodes
            base.Update(gameTime);

            // Preform normal Updates
            m_gameManager.Update(gameTime);

            fadeEffect.ApplyEffect(gameTime);

            // Lastly, we call PostUpdate on the GameEngine to let MonoGame know we are finished 
            base.PostUpdate(gameTime);
        }
        

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // First, call the PreDraw to prepare the screen for rendering
            base.PreDraw(gameTime);

            // Now we can begin our draw Here
            Texture2D logo = ResourceManager.Instance.GetAsset(typeof(Texture2D), "KillerrinStudiosLogo") as Texture2D;

            // Draw Texture
            spriteBatch.Begin();
            spriteBatch.Draw(logo, new Vector2(0.0f, 0.0f), Color.White);
            spriteBatch.End();

            // Call Draw on the Anarian Game Engine to render the Scene
            base.Draw(gameTime);

            // Preform Game Rendering
            m_gameManager.Draw(gameTime);

            fadeEffect.Draw(gameTime, spriteBatch);

            // Lastly, Call the GameEngines PostDraw Method to let MonoGame know we are finished
            base.PostDraw(gameTime);
        }
    }
}
