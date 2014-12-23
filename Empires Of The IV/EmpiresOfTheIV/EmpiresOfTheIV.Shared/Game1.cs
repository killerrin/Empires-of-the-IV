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
//using AnimationAux;
using Anarian.Enumerators;
using Anarian.DataStructures.Input;
using Anarian.DataStructures.Rendering;
using Anarian.Helpers;
using Anarian.Interfaces;
using EmpiresOfTheIV.GameObjects;

namespace EmpiresOfTheIV
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : AnarianGameEngine
    {
        private AnimatedGameObject soldier = null;

        //Terrain
        Terrain m_terrain;

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

            // Subscribe to our Events
            // Mouse
            m_inputManager.Mouse.MouseDown += Mouse_MouseDown;
            m_inputManager.Mouse.MouseClicked += Mouse_MouseClicked;
            m_inputManager.Mouse.MouseMoved += Mouse_MouseMoved;
            // Keybaord
            m_inputManager.Keyboard.KeyboardDown += Keyboard_KeyboardDown;
            m_inputManager.Keyboard.KeyboardPressed += Keyboard_KeyboardPressed;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            // Load the Assets
            m_resourceManager.LoadAsset(Content, typeof(Texture2D), "KillerrinStudiosLogo");
            m_resourceManager.LoadAsset(Content, typeof(AnimatedModel), "t-pose_3");

            // Create the Game Objects
            Unit armyGuy = new Unit();
            armyGuy.Model3D = m_resourceManager.GetAsset(typeof(AnimatedModel), "t-pose_3") as AnimatedModel;
            armyGuy.Transform.Scale = new Vector3(0.007f);
            armyGuy.Transform.Position = new Vector3(0.2f, -0.5f, 0.50f);
            
            // Add to the Scene
            m_sceneManager.CurrentScene.SceneNode.AddChild(armyGuy.Transform);
            
            // Load the Terrain
            Texture2D heightMap = Content.Load<Texture2D>("heightmap");
            Texture2D grassTexture = Content.Load<Texture2D>("grassTexture");
            m_terrain = new Terrain(graphics, heightMap, grassTexture);

            // Load the model we will display
            soldier = new AnimatedGameObject();
            soldier.Model3D = m_resourceManager.GetAsset(typeof(AnimatedModel), "t-pose_3") as AnimatedModel;

            // Load the model that has an animation clip it in
            AnimatedModel walk = CustomContentLoader.LoadAnimatedModel(Content, "walk");
            AnimationClip clip = walk.Clips[0];
            
            AnimationPlayer player = soldier.PlayClip(clip);
            player.Looping = true;

            AnimationPlayer armyGuyPlayer = armyGuy.PlayClip(clip);
            armyGuyPlayer.Looping = true;
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
            //m_sceneManager.CurrentScene.SceneNode.GetChild(0).Rotation *= Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), gameTime.DeltaTime());
            soldier.Update(gameTime);

            if (rayPosOnTerrain.HasValue) {
                m_sceneManager.CurrentScene.SceneNode.GetChild(0).MoveToPosition(gameTime, rayPosOnTerrain.Value);
            }

            float height = m_terrain.GetHeightAtPoint(m_sceneManager.CurrentScene.SceneNode.GetChild(0).Position);
            if (height != float.MaxValue) {
                Vector3 pos = m_sceneManager.CurrentScene.SceneNode.GetChild(0).Position;
                pos.Y = height;
                m_sceneManager.CurrentScene.SceneNode.GetChild(0).Position = pos;
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
            Texture2D logo = ResourceManager.Instance.GetAsset(typeof(Texture2D), "KillerrinStudiosLogo") as Texture2D;

            // Draw Texture
            spriteBatch.Begin();
            spriteBatch.Draw(logo, new Vector2(0.0f, 0.0f), Color.White);
            spriteBatch.End();

            // Call Draw on the Anarian Game Engine to render the SceneGraph
            base.Draw(gameTime);

            // Draw the Rays
            if (currentRay.HasValue) {
                currentRay.Value.DrawRay(graphics, Color.Red, m_sceneManager.CurrentScene.Camera, Matrix.Identity);
            }

            m_terrain.Draw(gameTime, m_sceneManager.CurrentScene.Camera, graphics);
            soldier.Draw(gameTime, m_sceneManager.CurrentScene.Camera, graphics);
            
            // Lastly, Call the Monogame Draw Method
            base.PostDraw(gameTime);
        }


        #region Input Events
        void Mouse_MouseDown(object sender, Anarian.Events.MouseClickedEventArgs e)
        {
            if (e.ButtonClicked == MouseButtonClick.RightMouseButton) {

            }
        }

        Ray? currentRay;
        Vector3? rayPosOnTerrain;
        void Mouse_MouseClicked(object sender, Anarian.Events.MouseClickedEventArgs e)
        {
            if (e.ButtonClicked == MouseButtonClick.LeftMouseButton) {
                Camera camera = m_sceneManager.CurrentScene.Camera;
                Ray ray = camera.GetMouseRay(
                    e.Position,
                    GraphicsDevice.Viewport
                    );

                bool intersects = m_sceneManager.CurrentScene.SceneNode.GetChild(0).GameObject.CheckRayIntersection(ray);
                Debug.WriteLine("Hit: {0}, Ray: {1}", intersects, ray.ToString());

                currentRay = ray;

                // Get the point on the terrain
                rayPosOnTerrain = m_terrain.Intersects(ray);
            }            
            if (e.ButtonClicked == MouseButtonClick.MiddleMouseButton) {
                Debug.WriteLine("Middle Mouse Pressed");
                GamePage.PageFrame.Navigate(typeof(BlankPage));
                GamePage.PageFrame.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            if (e.ButtonClicked == MouseButtonClick.RightMouseButton) {
                Unit unit = m_sceneManager.CurrentScene.SceneNode.GetChild(0).GameObject as Unit;
                unit.AnimationState.AnimationPlayer.Paused = !unit.AnimationState.AnimationPlayer.Paused;
            }
        }

        void Mouse_MouseMoved(object sender, Anarian.Events.MouseMovedEventArgs e)
        {
        }

        void Keyboard_KeyboardDown(object sender, Anarian.Events.KeyboardPressedEventArgs e)
        {
            //Debug.WriteLine("Keyboard: {0}, Held Down", e.KeyClicked.ToString());
            Camera cam = m_sceneManager.CurrentScene.Camera;
            
            switch (e.KeyClicked) {
                case Keys.W:
                    cam.MoveForward(2.0f);
                    break;
                case Keys.S:
                    cam.MoveForward(-2.0f);
                    break;
                case Keys.A:
                    cam.MoveHorizontal(-2.0f);
                    break;
                case Keys.D:
                    cam.MoveHorizontal(2.0f);
                    break;
                case Keys.Q:
                    cam.MoveVertical(-2.0f);
                    break;
                case Keys.E:
                    cam.MoveVertical(2.0f);
                    break;

                case Keys.O:
                    cam.MoveDepth(-2.0f);
                    break;
                case Keys.L:
                    cam.MoveDepth(2.0f);
                    break;

                case Keys.Up:
                    cam.AddPitch(MathHelper.ToRadians(2));
                    break;
                case Keys.Down:
                    cam.AddPitch(MathHelper.ToRadians(-2));
                    break;
                case Keys.Left:
                    cam.AddYaw(MathHelper.ToRadians(2));
                    break;
                case Keys.Right:
                    cam.AddYaw(MathHelper.ToRadians(-2));
                    break;
            }
        }
        void Keyboard_KeyboardPressed(object sender, Anarian.Events.KeyboardPressedEventArgs e)
        {
            //Debug.WriteLine("{0}, Pressed", e.KeyClicked.ToString());
        }
        #endregion
    }
}
