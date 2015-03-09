using Anarian;
using Anarian.DataStructures;
using Anarian.DataStructures.Animation;
using Anarian.DataStructures.Animation.Aux;
using Anarian.DataStructures.Rendering;
using Anarian.Enumerators;
using Anarian.GUI;
using Anarian.IDManagers;
using Anarian.Interfaces;
using EmpiresOfTheIV.Game.Enumerators;
using EmpiresOfTheIV.Game.GameObjects;
using EmpiresOfTheIV.Game.Menus.PageParameters;
using EmpiresOfTheIV.Game.Players;
using KillerrinStudiosToolkit.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace EmpiresOfTheIV.Game.Menus
{
    public class InGameMenu : GameMenu,
                              IUpdatable, IRenderable
    {
        private static bool m_loadedOnceAlready = false;
        #region Fields/Properties
        NetworkManager m_networkManager;
        GamePausedState m_pausedState;

        #region Loading
        private int m_currentLoadingPercentage;
        Progress<int> m_loadingProgress;
        Task<LoadingStatus> m_loadingContentTask;
        #endregion

        #region Page Parameters
        InGamePageParameter m_pageParameter;
        Player m_me;
        Team m_team1;
        Team m_team2;

        ChatManager m_chatManager;
        #endregion

        UniversalCamera m_gameCamera;
        List<Unit> m_activeUnits;
        List<Unit> m_inactiveUnits;
        Map m_map;
        #endregion

        public InGameMenu(EmpiresOfTheIVGame game, object parameter)
            :base(game, parameter, GameState.InGame)
        {
            m_networkManager = m_game.NetworkManager;
            m_pausedState = GamePausedState.Unpaused;

            m_currentLoadingPercentage = 0;
            m_loadingProgress = new Progress<int>();
            m_loadingProgress.ProgressChanged += m_loadingProgress_ProgressChanged;

            // Subscribe to Events
            m_networkManager.OnConnected += NetworkManager_OnConnected;
            m_networkManager.OnDisconnected += NetworkManager_OnDisconnected;
            m_networkManager.OnMessageRecieved += NetworkManager_OnMessageRecieved;

            Consts.Game.StateManager.OnBackButtonPressed += StateManager_OnBackButtonPressed;
            Consts.Game.StateManager.HandleBackButtonPressed = false;

            // Save the parameters
            m_pageParameter = (InGamePageParameter)parameter;
            m_me = m_pageParameter.me;           Debug.WriteLine("Me: " + m_me.ToString());
            m_team1 = m_pageParameter.team1;     Debug.WriteLine(m_team1.ToString());
            m_team2 = m_pageParameter.team2;     Debug.WriteLine(m_team2.ToString());

            m_chatManager = m_pageParameter.chatManager;

            // Begin the Asynchronous Loading
            m_loadingContentTask = Task.Run(() => LoadContent(m_loadingProgress, m_game.Content, m_game.GraphicsDevice));
        }

        public override void MenuLoaded()
        {
            base.MenuLoaded();

            if (!m_loadedOnceAlready)
            {
                m_loadedOnceAlready = true;
            }

            // Turn off the Unified Menu
            m_game.SceneManager.Active = false;

            //if (NavigationSaveState == Anarian.Enumerators.NavigationSaveState.KeepSate)
            //{
            //}
        }

        public override void MenuExited()
        {
            base.MenuExited();
        }

        private void StateManager_OnBackButtonPressed(object sender, EventArgs e)
        {
            // De-Subscribe to our Events
            // Pointer
            //m_game.InputManager.PointerDown -= InputManager_PointerDown;
            //m_game.InputManager.PointerPressed -= InputManager_PointerClicked;
            //m_game.InputManager.PointerMoved -= InputManager_PointerMoved;

            // Keyboard
            m_game.InputManager.Keyboard.KeyboardDown -= Keyboard_KeyboardDown;
            m_game.InputManager.Keyboard.KeyboardPressed -= Keyboard_KeyboardPressed;
        }

        #region Loading
        private async Task<LoadingStatus> LoadContent(IProgress<int> progress, ContentManager Content, GraphicsDevice graphics)
        {
            if (progress != null) progress.Report(0);

            #region Setup Variables
            m_gameCamera = new UniversalCamera();
            m_gameCamera.AspectRatio = m_game.SceneManager.CurrentScene.Camera.AspectRatio;

            int totalUnitsInPool = (int)(m_pageParameter.maxUnitsPerPlayer * (m_team1.PlayerCount + m_team2.PlayerCount));
            m_activeUnits = new List<Unit>(totalUnitsInPool);
            m_inactiveUnits = new List<Unit>(totalUnitsInPool);

            IDManager unitIDManager = new IDManager();
            #endregion

            if (progress != null) progress.Report(20);

            #region Load Assets
            AnimatedModel tPoseAnimModel = m_game.ResourceManager.LoadAsset(Content, typeof(AnimatedModel), "t-pose_3")  as AnimatedModel;
            AnimatedModel walkAnimModelAnim = m_game.ResourceManager.LoadAsset(Content, typeof(AnimatedModel), "walk")   as AnimatedModel;
            AnimationClip walkAnimClip = walkAnimModelAnim.Clips[0];

            Model factoryBaseModel = m_game.ResourceManager.LoadAsset(Content, typeof(Model), "Factory Base") as Model;
            #endregion

            if (progress != null) progress.Report(40);

            #region Load the Map
            // Make the Map
            Texture2D heightMap = null;
            Texture2D mapTexture = null;
            Terrain mapTerrain = null;

            switch (m_pageParameter.MapName)
            {
                case MapName.RadientFlatlands:
                    if (GameConsts.Loading.Map_RadientFlatlands != LoadingStatus.Loaded)
                    {
                        GameConsts.Loading.Map_RadientFlatlands = LoadingStatus.CurrentlyLoading;

                        heightMap = Content.Load<Texture2D>("Radient Flatlands HeightMap");
                        mapTexture = Content.Load<Texture2D>("Radient Flatlands Texture");
                        mapTerrain = new Terrain(graphics, heightMap, mapTexture);
                        m_map = new Map(MapName.RadientFlatlands, null, mapTerrain);

                        GameConsts.Loading.Map_RadientFlatlands = LoadingStatus.Loaded;
                    }
                    break;
                case MapName.Kalia:
                    if (GameConsts.Loading.Map_Kalia != LoadingStatus.Loaded)
                    {
                        GameConsts.Loading.Map_Kalia = LoadingStatus.CurrentlyLoading;

                        heightMap = Content.Load<Texture2D>("Radient Flatlands HeightMap");
                        mapTexture = Content.Load<Texture2D>("Radient Flatlands Texture");
                        mapTerrain = new Terrain(graphics, heightMap, mapTexture);
                        m_map = new Map(MapName.RadientFlatlands, null, mapTerrain);

                        GameConsts.Loading.Map_Kalia = LoadingStatus.Loaded;
                    }
                    break;
            }
            #endregion

            if (progress != null) progress.Report(60);

            #region Create all the Units in the pool
            for (int i = 0; i < totalUnitsInPool; i++)
            {
                var unit = new Unit(unitIDManager.GetNewID());
                unit.Model3D = tPoseAnimModel;
                unit.Transform.Scale = new Vector3(0.007f);
                unit.Transform.Position = new Vector3((float)Consts.random.NextDouble(), -(float)Consts.random.NextDouble(), -5.50f + (float)Consts.random.NextDouble());
                unit.CullDraw = false;
                unit.RenderBounds = false;

                unit.Active = true;
                unit.Health.Alive = true;

                AnimationPlayer animPlayer = unit.PlayClip(walkAnimClip);
                animPlayer.Looping = true;
                
                m_inactiveUnits.Add(unit);
            }
            #endregion

            if (progress != null) progress.Report(80);

            #region Subscribe to input Events
            // Subscribe to our Events
            // Pointer
            //m_game.InputManager.PointerDown += InputManager_PointerDown;
            //m_game.InputManager.PointerPressed += InputManager_PointerClicked;
            //m_game.InputManager.PointerMoved += InputManager_PointerMoved;

            // Keyboard
            m_game.InputManager.Keyboard.KeyboardDown += Keyboard_KeyboardDown;
            m_game.InputManager.Keyboard.KeyboardPressed += Keyboard_KeyboardPressed;
            #endregion

            // Send the final report and return loaded
            if (progress != null) progress.Report(100);
            return LoadingStatus.Loaded;
        }
        void m_loadingProgress_ProgressChanged(object sender, int e)
        {
            Debug.WriteLine("m_loadingProgress_ProgressChanged: " + e);
            m_currentLoadingPercentage = e;
        }
        #endregion

        #region Networking
        private void NetworkManager_OnConnected(object sender, OnConnectedEventArgs e)
        {
        }

        private void NetworkManager_OnDisconnected(object sender, EventArgs e)
        {
        }

        private void NetworkManager_OnMessageRecieved(object sender, ReceivedMessageEventArgs e)
        {
        }
        #endregion

        #region Input
        void Keyboard_KeyboardDown(object sender, Anarian.Events.KeyboardPressedEventArgs e)
        {
            //Debug.WriteLine("Keyboard: {0}, Held Down", e.KeyClicked.ToString());
            ICamera cam =            m_gameCamera; //m_game.SceneManager.CurrentScene.Camera;
            IMoveable camMoveable =  m_gameCamera; //m_game.SceneManager.CurrentScene.Camera as IMoveable;
            UniversalCamera uniCam = m_gameCamera; //m_game.SceneManager.CurrentScene.Camera as UniversalCamera;

            switch (e.KeyClicked)
            {
                case Keys.W: camMoveable.Move(e.GameTime, uniCam.CameraRotation.Forward); break;
                case Keys.S: camMoveable.Move(e.GameTime, -uniCam.CameraRotation.Forward); break;
                case Keys.A: camMoveable.Move(e.GameTime, -uniCam.CameraRotation.Right); break;
                case Keys.D: camMoveable.Move(e.GameTime, uniCam.CameraRotation.Right); break;
                case Keys.Q: camMoveable.Move(e.GameTime, -uniCam.CameraRotation.Up); break;
                case Keys.E: camMoveable.Move(e.GameTime, uniCam.CameraRotation.Up); break;

                case Keys.NumPad8: cam.Pitch = uniCam.Pitch + MathHelper.ToRadians(2); break;
                case Keys.NumPad2: cam.Pitch = uniCam.Pitch + MathHelper.ToRadians(-2); break;

                case Keys.NumPad4: cam.Yaw = uniCam.Yaw + MathHelper.ToRadians(2); break;
                case Keys.NumPad6: cam.Yaw = uniCam.Yaw + MathHelper.ToRadians(-2); break;

                case Keys.NumPad1:
                case Keys.NumPad7: cam.Roll = uniCam.Roll + MathHelper.ToRadians(2); break;

                case Keys.NumPad3:
                case Keys.NumPad9: cam.Roll = uniCam.Roll + MathHelper.ToRadians(-2); break;

                case Keys.NumPad0: uniCam.ResetCamera(); break;
                case Keys.NumPad5: uniCam.ResetRotations(); break;
            }
        }
        void Keyboard_KeyboardPressed(object sender, Anarian.Events.KeyboardPressedEventArgs e)
        {
            Debug.WriteLine("{0}, Pressed", e.KeyClicked.ToString());
            UniversalCamera uniCam = m_gameCamera;//m_game.SceneManager.CurrentScene.Camera as UniversalCamera;

            switch (e.KeyClicked)
            {
                case Keys.Space:
                    uniCam.SwitchCameraMode();
                    break;
            }
        }
        #endregion

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics); }
        #endregion

        public override void Update(GameTime gameTime)
        {
            if (m_currentLoadingPercentage < 100) { base.Update(gameTime); return; }
            // Since we passed the check, that means that the game is fully loaded

            // Update the Camera
            m_gameCamera.Update(gameTime);

            m_map.Update(gameTime);
            foreach (var i in m_inactiveUnits)
            {
                i.Update(gameTime);
            }

            // Update everything else
            switch (m_pausedState)
            {
                case GamePausedState.Paused:
                    break;
                case GamePausedState.Unpaused:
                    break;
                case GamePausedState.WaitingForData:
                    break;
                default:
                    break;
            }

            //// Regular Updates
            //if (m_game.GameInputManager.rayPosOnTerrain.HasValue)
            //{
            //    m_game.SceneManager.CurrentScene.SceneNode.GetChild(2).MoveToPosition(gameTime, m_game.GameInputManager.rayPosOnTerrain.Value);
            //}
            //
            //float height = ((Terrain)m_game.SceneManager.CurrentScene.SceneNode.GetChild(1).GameObject).GetHeightAtPoint(m_game.SceneManager.CurrentScene.SceneNode.GetChild(2).Position);
            //if (height != float.MaxValue)
            //{
            //    Vector3 pos = m_game.SceneManager.CurrentScene.SceneNode.GetChild(2).Position;
            //    pos.Y = height;
            //    m_game.SceneManager.CurrentScene.SceneNode.GetChild(2).Position = pos;
            //}
            //
            //((UniversalCamera)m_game.SceneManager.CurrentScene.Camera).WorldPositionToChase = m_game.SceneManager.CurrentScene.SceneNode.GetChild(2).WorldMatrix;

            //-- Update the Menu
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            if (m_currentLoadingPercentage < 100) { DrawLoadingMenu(gameTime, spriteBatch, graphics); base.Draw(gameTime, spriteBatch, graphics); return; }

            m_map.Draw(gameTime, spriteBatch, graphics, m_gameCamera);
            foreach (var i in m_inactiveUnits)
            {
                i.Draw(gameTime, spriteBatch, graphics, m_gameCamera);
            }

            switch (m_pausedState)
            {
                case GamePausedState.Paused:
                    break;
                case GamePausedState.Unpaused:
                    break;
                case GamePausedState.WaitingForData:
                    break;
                default:
                    break;
            }

            base.Draw(gameTime, spriteBatch, graphics);
        }

        public void DrawLoadingMenu(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            graphics.Clear(Color.Red);
        }
    }
}
