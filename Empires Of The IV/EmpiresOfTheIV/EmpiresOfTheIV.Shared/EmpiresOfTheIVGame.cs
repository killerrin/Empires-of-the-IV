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
using EmpiresOfTheIV.Game.Enumerators;

namespace EmpiresOfTheIV
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class EmpiresOfTheIVGame : AnarianGameEngine
    {
        public static EmpiresOfTheIVGame Current { get; private set; }

        #region Managers 
        protected StateManager m_stateManager;
        public StateManager StateManager { get { return m_stateManager; } set { m_stateManager = value; } }

        protected NetworkManager m_networkManager;
        public NetworkManager NetworkManager { get { return m_networkManager; } set { m_networkManager = value; } }
        #endregion

        public EmpiresOfTheIVGame()
            :base()
        {
            // Set the Default Content Project
            Content.RootDirectory = "Content";

            // Set the Current
            Current = this;
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

            // Load the State Manager
            m_stateManager = new StateManager(this);
            m_stateManager.OnExit += OnExitTriggered;
            
            m_networkManager = new NetworkManager(this);


            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

#if WINDOWS_APP
            this.IsMouseVisible = true;
#endif

            // Lastly we call PostInitialize to do MonoGame Initializations
            base.PostInitialize();
        }

        private void OnExitTriggered(object sender, EventArgs e)
        {
            // We can Exit the Game
            Debug.WriteLine("Exit");
            Windows.UI.Xaml.Application.Current.Exit();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent(Color.Black);

            // Load Special Data Once
            ResourceManager.LoadAsset(Content, typeof(Texture2D), "KillerrinStudiosLogo");
            ResourceManager.LoadAsset(Content, typeof(SpriteFont), "Fonts/EmpiresOfTheIVFont");

            // Load the Content
            m_stateManager.LoadStateManager(Color.Black);

            #region Unified Menu
            if (GameConsts.Loading.Menu_UnifiedLoaded != LoadingStatus.Loaded)
            {
                GameConsts.Loading.Menu_UnifiedLoaded = LoadingStatus.CurrentlyLoading;

                // Create Planets
                Debug.WriteLine("Loading Tyril");
                Texture2D tyrilTexture = Content.Load<Texture2D>("Tyril Texture");
                Planet tyril = new Planet(GraphicsDevice, tyrilTexture, MathHelper.ToRadians(-25.0f), Vector3.Zero, new Vector3(0.00020f, 0.0f, 0.0f), Vector3.Zero);
                tyril.Transform.Scale = new Vector3(10.0f);

                Debug.WriteLine("Loading Hope");
                Texture2D hopeTexture = Content.Load<Texture2D>("Hope Texture");
                Planet hope = new Planet(GraphicsDevice, hopeTexture, MathHelper.ToRadians(-20.0f), new Vector3(20.0f, 2.0f, 0.0f), new Vector3(0.00050f, 0.0f, 0.0f), new Vector3(0.00040f, 0.0f, 0.0f));
                hope.Transform.Scale = new Vector3(1.5f);

                Debug.WriteLine("Loading Yol");
                Texture2D yolTexture = Content.Load<Texture2D>("Yol Texture");
                Planet yol = new Planet(GraphicsDevice, yolTexture, MathHelper.ToRadians(-15.0f), new Vector3(2.0f, 20.0f, 0.0f), new Vector3(0.00040f, 0.0f, 0.0f), new Vector3(0.0f, 0.00040f, 0.0f));
                yol.Transform.Scale = new Vector3(1.2f);

                Debug.WriteLine("Loading Lura");
                //Texture2D luraTexture = m_game.Content.Load<Texture2D>("Lura Texture");
                //Planet lura = new Planet(graphics, luraTexture, MathHelper.ToRadians(30.0f), new Vector3(-30.0f, 30.0f, 2.0f), new Vector3(0.00020f, 0.0f, 0.0f), new Vector3(0.00020f, 0.00020f, 0.0f));
                //lura.Transform.Scale = new Vector3(2.0f);

                Debug.WriteLine("Adding Satellites");
                tyril.AddSatellite(hope);
                tyril.AddSatellite(yol);
                //tyril.AddSatellite(lura);

                SceneManager.CurrentScene.SceneNode.AddChild(tyril.Transform);

                GameConsts.Loading.Menu_UnifiedLoaded = LoadingStatus.Loaded;
            }
            #endregion

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

        bool m_doOnlyOnce = false;

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

            // First, Update the StateManager
            m_stateManager.Update(gameTime);

            // Update any required GameManager Data
            switch (m_stateManager.CurrentState)
            {
                case GameState.SplashScreen: break;

                case GameState.MainMenu: break;
                case GameState.Options: break;
                case GameState.Credits: break;

                case GameState.BluetoothMultiplayer: break;
                case GameState.LanMultiplayer: break;

                case GameState.GameLobby: break;
                case GameState.EmpireSelection: break;

                case GameState.InGame: break;
                case GameState.GameOver: break;
                default: break;
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

            if (!m_doOnlyOnce || !m_stateManager.Loaded)
            {
                base.PostDraw(gameTime);
            }

            // Call Draw on the Anarian Game Engine to render the Scene
            base.Draw(gameTime);

            switch (m_stateManager.CurrentState)
            {
                case GameState.SplashScreen: GraphicsDevice.Clear(Color.Black); break;

                case GameState.MainMenu: break;
                case GameState.Options: break;
                case GameState.Credits: break;

                case GameState.BluetoothMultiplayer: break;
                case GameState.LanMultiplayer: break;

                case GameState.GameLobby: break;
                case GameState.EmpireSelection: break;

                case GameState.InGame: break;
                case GameState.GameOver: break;
                default: break;
            }

            // Finally, Draw the StateManager over everything else
            m_stateManager.Draw(gameTime, spriteBatch, GraphicsDevice);

            // Lastly, Call the GameEngines PostDraw Method to let MonoGame know we are finished
            base.PostDraw(gameTime);
        }
    }
}
