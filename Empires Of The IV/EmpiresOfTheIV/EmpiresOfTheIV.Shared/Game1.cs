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
using Anarian.Enumerators;
//using AnimationAux;
using Anarian.DataStructures.Input;
using Anarian.Helpers;
using Anarian.Interfaces;

namespace EmpiresOfTheIV
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : AnarianGameEngine
    {
        /// <summary>
        /// The animated model we are displaying
        /// </summary>
        private AnimatedModel model = null;

        /// <summary>
        /// This model is loaded solely for the dance animation
        /// </summary>
        private AnimatedModel dance = null;


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

#if WINDOWS_APP
            this.IsMouseVisible = true;
#endif

            m_inputManager.Mouse.MouseClicked += Mouse_MouseClicked;

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
            m_resourceManager.LoadModel(Content, "t-pose_3_t");
            
            // Create the Game Objects
            GameObject armyGuy = new GameObject();
            armyGuy.Model3D = m_resourceManager.GetModel("t-pose_3_t");
            armyGuy.Scale = new Vector3(0.007f);
            armyGuy.Position = new Vector3(0.0f, -0.5f, 0.0f);
            
            // Add to the Scene
            m_sceneManager.CurrentScene.SceneNode.AddChild(armyGuy);



            //// Load the Animated Model
            ////// Load the model we will display
            //model = new AnimatedModel("t-pose_3");
            //model.LoadContent(Content);
            ////// Load the model that has an animation clip it in
            //dance = new AnimatedModel("walk");
            //dance.LoadContent(Content);
            //
            ////System.Diagnostics.Debug.WriteLine(dance.Clips.Count);
            ////AnimationClip clip = dance.Clips[0];
            ////System.Diagnostics.Debug.WriteLine(dance.Clips[0].Name);
            ////
            ////// And play the clip
            ////AnimationPlayer player = model.PlayClip(clip);
            ////player.Looping = true;
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
            //m_sceneManager.CurrentScene.SceneNode.Position =
            //    new Vector3(0.5f, 0.5f, 0.5f);
            //m_sceneManager.CurrentScene.SceneNode.Rotation +=
            //    new Vector3(0.0f, (0.0025f) * (float)gameTime.ElapsedGameTime.TotalMilliseconds, 0.0f);
            //m_sceneManager.CurrentScene.SceneNode.OrbitalRotation +=
            //    new Vector3(MathHelper.ToRadians(1.0f), MathHelper.ToRadians(1.0f), MathHelper.ToRadians(1.0f));

            if (!set) {
                if (GamePage.CortanaMediaElement != null) {
                #if WINDOWS_PHONE_APP
                    set = true;
                    CortanaHelper.CortanaFeedback("Hello, World!", GamePage.CortanaMediaElement);
                #endif
                }
            }


            base.Update(gameTime);
        }

        void Mouse_MouseClicked(object sender, Anarian.Events.MouseClickedEventArgs e)
        {
            if (e.ButtonClicked == MouseButtonClick.LeftMouseButton) {
                Camera camera = m_sceneManager.CurrentScene.Camera;
                Ray ray = camera.GetMouseRay(
                    e.Position,
                    GraphicsDevice.Viewport
                    );

                Debug.WriteLine("Pos {0}, Dir {1}", ray.Position, ray.Direction);
                Vector3 position = m_sceneManager.CurrentScene.SceneNode.GetChild(0).Position;
                //position.X += ray.Position.X;
                //position.Y = ray.Position.Y;
                //m_sceneManager.CurrentScene.SceneNode.GetChild(0).Position = position;

                bool intersects = m_sceneManager.CurrentScene.SceneNode.GetChild(0).CheckRayIntersection(ray);
                Debug.WriteLine(intersects);
            } 
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

            //// Draw some Points
            //PrimitiveHelper2D.DrawPoints(spriteBatch, Color.Red, 20, new Vector2(200.0f, 200.0f));
            //PrimitiveHelper2D.DrawLines(spriteBatch, Color.Red, 4, new Vector2(0.0f, 400.0f), new Vector2(200.0f, 550.0f),
            //    new Vector2(400.0f, 550.0f), new Vector2(600.0f, 300.0f));
            //
            //PrimitiveHelper2D.DrawCircle(spriteBatch, Color.Red, 4, 25.0f, new Vector2(500.0f, 400.0f));
            //PrimitiveHelper2D.DrawArc(spriteBatch, Color.Red, 4, 120.0f, 25.0f, new Vector2(700.0f, 400.0f));
            //PrimitiveHelper2D.DrawArc(spriteBatch, Color.Red, 4, 120.0f, -25.0f, new Vector2(800.0f, 400.0f));
            //PrimitiveHelper2D.DrawSineWave(spriteBatch, Color.Red, 4, new Vector2(0.0f, 600.0f), 100.0f, 0.006f, GraphicsDevice.Viewport.Width, 0.0f);

            // Call Draw on the Anarian Game Engine to render the SceneGraph
            base.Draw(gameTime);


            // Lastly, Call the Monogame Draw Method
            base.PostDraw(gameTime);
        }
    }
}
