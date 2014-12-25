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

namespace EmpiresOfTheIV
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class EmpiresOfTheIVGame : AnarianGameEngine
    {
        protected GameManager m_gameManager;
        public GameManager GameManager { get { return m_gameManager; } protected set { m_gameManager = value; } }

        #region Content
        private AnimatedGameObject soldier = null;

        //Terrain
        Terrain m_terrain;
        #endregion


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

            // Subscribe to our Events
            // Pointer
            InputManager.PointerDown += InputManager_PointerDown;
            InputManager.PointerPressed += InputManager_PointerClicked;
            InputManager.PointerMoved += InputManager_PointerMoved;

            // Keybaord
            InputManager.Keyboard.KeyboardDown += Keyboard_KeyboardDown;
            InputManager.Keyboard.KeyboardPressed += Keyboard_KeyboardPressed;

            // Lastly we call PostInitialize to do MonoGame Initializations
            base.PostInitialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            m_gameManager.LoadContent();

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


            // Begin Normal Rendering
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

            // Draw the Rays
            if (currentRay.HasValue) {
                currentRay.Value.DrawRay(graphics, Color.Red, m_sceneManager.CurrentScene.Camera, Matrix.Identity);
            }

            m_terrain.Draw(gameTime, m_sceneManager.CurrentScene.Camera, graphics);
            soldier.Draw(gameTime, m_sceneManager.CurrentScene.Camera, graphics);
            
            // Lastly, Call the GameEngines PostDraw Method to let MonoGame know we are finished
            base.PostDraw(gameTime);
        }


        #region Input Events
        void InputManager_PointerDown(object sender, Anarian.Events.PointerPressedEventArgs e)
        {
            //Debug.WriteLine("{0}, Pressed", e.ToString());
        }

        Ray? currentRay;
        Vector3? rayPosOnTerrain;
        void InputManager_PointerClicked(object sender, Anarian.Events.PointerPressedEventArgs e)
        {
            //Debug.WriteLine("{0}, Pressed", e.ToString());

            if (e.Pointer == PointerPress.LeftMouseButton ||
                e.Pointer == PointerPress.Touch) {
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
            if (e.Pointer == PointerPress.MiddleMouseButton) {
                Debug.WriteLine("Middle Mouse Pressed");
                GamePage.PageFrame.Navigate(typeof(BlankPage));
                GamePage.PageFrame.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            if (e.Pointer == PointerPress.RightMouseButton) {
                Unit unit = m_sceneManager.CurrentScene.SceneNode.GetChild(0).GameObject as Unit;
                unit.AnimationState.AnimationPlayer.Paused = !unit.AnimationState.AnimationPlayer.Paused;
            }
        }

        void InputManager_PointerMoved(object sender, Anarian.Events.PointerMovedEventArgs e)
        {
            //Debug.WriteLine("{0}, Pressed", e.ToString());

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
