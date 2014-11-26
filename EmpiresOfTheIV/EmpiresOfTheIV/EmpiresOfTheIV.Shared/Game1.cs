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
using Anarian.Interfaces;
using Anarian.Helpers;

namespace EmpiresOfTheIV
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : AnarianGameEngine
    {
        public Game1()
            :base()
        {
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
            base.Initialize();

            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            // Load the Assets
            m_resourceManager.LoadTexture(Content, "KillerrinStudiosLogo");
            m_resourceManager.LoadModel(Content, "t-pose_3");

            // Create the Game Objects
            GameObject armyGuy = new GameObject();
            armyGuy.Model3D = m_resourceManager.GetModel("t-pose_3");
            armyGuy.Scale = new Vector3(0.007f);
            armyGuy.Position = new Vector3(0.0f, -0.5f, 0.0f);

            // Add to the Scene
            m_sceneManager.CurrentScene.GetSceneNode().AddChild(armyGuy);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        bool set = false;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            m_sceneManager.CurrentScene.GetSceneNode().Rotation += new Vector3(0.0f, (0.0025f) * (float)gameTime.ElapsedGameTime.TotalMilliseconds, 0.0f);

            if (!set) {
                set = true;
#if WINDOWS_PHONE_APP
                //CortanaHelper.CortanaFeedback("Hello, World!", GamePage.CortanaMediaElement);
#endif
            }

            MouseState mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && !set) {
                //set = true;
                //GamePage.PageFrame.Navigate(typeof(BlankPage));
            } 

            base.Update(gameTime);
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
            Texture2D logo = ResourceManager.Instance.GetTexture("KillerrinStudiosLogo");

            // Draw Texture
            spriteBatch.Begin();
            spriteBatch.Draw(logo, new Vector2(0.0f, 0.0f), Color.White);
            spriteBatch.End();

            // Draw some Points
            PrimitiveHelper2D.DrawPoints(spriteBatch, Color.Red, 20, new Vector2(200.0f, 200.0f));
            PrimitiveHelper2D.DrawLines(spriteBatch, Color.Red, 4, new Vector2(0.0f, 400.0f), new Vector2(200.0f, 550.0f),
                new Vector2(400.0f, 550.0f), new Vector2(600.0f, 300.0f));

            PrimitiveHelper2D.DrawCircle(spriteBatch, Color.Red, 4, 25.0f, new Vector2(500.0f, 400.0f));
            PrimitiveHelper2D.DrawArc(spriteBatch, Color.Red, 4, 120.0f, 25.0f, new Vector2(700.0f, 400.0f));
            PrimitiveHelper2D.DrawArc(spriteBatch, Color.Red, 4, 120.0f, -25.0f, new Vector2(800.0f, 400.0f));

            // Call Draw on the Anarian Game Engine to render the SceneGraph
            base.Draw(gameTime);

            PrimitiveHelper2D.DrawSineWave(spriteBatch, Color.Red, 4, new Vector2(0.0f, 600.0f), 100.0f, 0.006f, GraphicsDevice.Viewport.Width, 0.0f);

            // Lastly, Call the Monogame Draw Method
            base.PostDraw(gameTime);
        }
    }
}
