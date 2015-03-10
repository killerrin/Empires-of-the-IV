using Anarian;
using Anarian.DataStructures;
using Anarian.DataStructures.Animation;
using Anarian.DataStructures.Animation.Aux;
using Anarian.DataStructures.Rendering;
using Anarian.DataStructures.ScreenEffects;
using Anarian.Enumerators;
using Anarian.GUI;
using Anarian.Helpers;
using Anarian.IDManagers;
using Anarian.Interfaces;
using EmpiresOfTheIV.Game.Enumerators;
using EmpiresOfTheIV.Game.GameObjects;
using EmpiresOfTheIV.Game.GameObjects.Factories;
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

        Overlay m_overlay;

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

        #region Constructors and Messages
        public InGameMenu(EmpiresOfTheIVGame game, object parameter)
            :base(game, parameter, GameState.InGame)
        {
            m_networkManager = m_game.NetworkManager;
            m_pausedState = GamePausedState.WaitingForData;

            m_currentLoadingPercentage = 0;
            m_loadingProgress = new Progress<int>();
            m_loadingProgress.ProgressChanged += m_loadingProgress_ProgressChanged;

            m_overlay = new Overlay(m_game.GraphicsDevice, Color.Black);
            m_overlay.FadePercentage = 0.75f;

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

        public override void SendMessage(object message)
        {
            base.SendMessage(message);
        }
        #endregion

        private void StateManager_OnBackButtonPressed(object sender, EventArgs e)
        {
            // De-Subscribe to our Events
            // Pointer
            m_game.InputManager.PointerDown -= InputManager_PointerDown;
            m_game.InputManager.PointerPressed -= InputManager_PointerClicked;
            m_game.InputManager.PointerMoved -= InputManager_PointerMoved;
            
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
            m_gameCamera.Position = m_gameCamera.Position + new Vector3(0.0f, 15.0f, 0.0f);
            m_gameCamera.AspectRatio = m_game.SceneManager.CurrentScene.Camera.AspectRatio;

            int totalUnitsInPool = (int)(m_pageParameter.maxUnitsPerPlayer * (m_team1.PlayerCount + m_team2.PlayerCount));
            m_activeUnits = new List<Unit>(totalUnitsInPool);
            m_inactiveUnits = new List<Unit>(totalUnitsInPool);

            IDManager unitIDManager = new IDManager();
            IDManager factoryBaseIDManager = new IDManager();
            #endregion

            if (progress != null) progress.Report(20);

            #region Load Empires
            #region Unanian Empire
            AnimatedModel unanianGroundSoldierTPose = null;
            AnimatedModel unanianGroundSoldierAnimation = null;
            AnimationClip unanianGroundSoldierWalkAnimClip = null;

            AnimatedModel unanianSpaceshipFighter = null;

            if (m_team1.IsPlayerEmpire(EmpireType.UnanianEmpire) || m_team2.IsPlayerEmpire(EmpireType.UnanianEmpire))
            {
                if (GameConsts.Loading.Empire_UnanianEmpireLoaded == LoadingStatus.Loaded)
                {
                    unanianGroundSoldierTPose = m_game.ResourceManager.GetAsset(typeof(AnimatedModel), UnitID.UnanianSoldier.ToString() + "|" + ModelType.AnimatedModel.ToString()) as AnimatedModel;
                    unanianGroundSoldierAnimation = m_game.ResourceManager.GetAsset(typeof(AnimatedModel), UnitID.UnanianSoldier.ToString() + "|" + ModelType.Animation.ToString()) as AnimatedModel;
                }
                else
                {
                    GameConsts.Loading.Empire_UnanianEmpireLoaded = LoadingStatus.CurrentlyLoading;

                    unanianGroundSoldierTPose = m_game.ResourceManager.LoadAsset(Content, typeof(AnimatedModel), "t-pose_3", UnitID.UnanianSoldier.ToString() + "|" + ModelType.AnimatedModel.ToString()) as AnimatedModel;
                    unanianGroundSoldierAnimation = m_game.ResourceManager.LoadAsset(Content, typeof(AnimatedModel), "walk", UnitID.UnanianSoldier.ToString() + "|" + ModelType.Animation.ToString()) as AnimatedModel;

                    GameConsts.Loading.Empire_UnanianEmpireLoaded = LoadingStatus.Loaded;
                }

                unanianGroundSoldierWalkAnimClip = unanianGroundSoldierAnimation.Clips[0];
            }
            #endregion

            #region Crescanian Confederacy
            AnimatedModel crescanianGroundSoldierTPose = null;
            AnimatedModel crescanianGroundSoldierAnimation = null;
            AnimationClip crescanianGroundSoldierWalkAnimClip = null;

            AnimatedModel crescanianSpaceshipFighter = null;

            if (m_team1.IsPlayerEmpire(EmpireType.CrescanianConfederation) || m_team2.IsPlayerEmpire(EmpireType.CrescanianConfederation))
            {
                if (GameConsts.Loading.Empire_CrescanianConfederationLoaded == LoadingStatus.Loaded)
                {

                }
                else
                {
                    GameConsts.Loading.Empire_CrescanianConfederationLoaded = LoadingStatus.CurrentlyLoading;

                    GameConsts.Loading.Empire_CrescanianConfederationLoaded = LoadingStatus.Loaded;
                }
            }
            #endregion

            #region Kingdom of Edolas
            AnimatedModel kingdomOfEdolasGroundSoldierTPose = null;
            AnimatedModel kingdomOfEdolasGroundSoldierAnimation = null;
            AnimationClip kingdomOfEdolasGroundSoldierWalkAnimClip = null;

            AnimatedModel kingdomOfEdolasSpaceshipFighter = null;

            if (m_team1.IsPlayerEmpire(EmpireType.TheKingdomOfEdolas) || m_team2.IsPlayerEmpire(EmpireType.TheKingdomOfEdolas))
            {
                if (GameConsts.Loading.Empire_KingdomOfEdolasLoaded == LoadingStatus.Loaded)
                {

                }
                else
                {
                    GameConsts.Loading.Empire_KingdomOfEdolasLoaded = LoadingStatus.CurrentlyLoading;

                    GameConsts.Loading.Empire_KingdomOfEdolasLoaded = LoadingStatus.Loaded;
                }
            }
            #endregion
            #endregion

            if (progress != null) progress.Report(40);

            #region Load the Map
            Texture2D heightMap = null;
            Texture2D mapTexture = null;
            Texture2D mapParallax = null;
            Terrain mapTerrain = null;

            Model factoryBaseModel = null;
            FactoryBase[] factoryBases;

            switch (m_pageParameter.MapName)
            {
                #region Radient Flatlands
                case MapName.RadientFlatlands:
                    #region Load/Get the Data
                    if (GameConsts.Loading.Map_RadientFlatlands == LoadingStatus.Loaded)
                    {
                        heightMap = m_game.ResourceManager.GetAsset(typeof(Texture2D), "Radient Flatlands HeightMap") as Texture2D;
                        mapTexture = m_game.ResourceManager.GetAsset(typeof(Texture2D), "Radient Flatlands Texture") as Texture2D;
                        mapParallax = m_game.ResourceManager.GetAsset(typeof(Texture2D), "Radient Flatlands Parallax") as Texture2D;
                        mapTerrain = m_game.PrefabManager.GetPrefab("Radient Flatlands Terrain") as Terrain;

                        factoryBaseModel = m_game.ResourceManager.GetAsset(typeof(Model), "Radient Flatlands FactoryBase") as Model;
                    }
                    else 
                    {
                        GameConsts.Loading.Map_RadientFlatlands = LoadingStatus.CurrentlyLoading;

                        heightMap = m_game.ResourceManager.LoadAsset(Content, typeof(Texture2D), "Radient Flatlands HeightMap") as Texture2D;
                        mapTexture = m_game.ResourceManager.LoadAsset(Content, typeof(Texture2D), "Radient Flatlands Texture") as Texture2D;
                        mapParallax = Color.Black.CreateTextureFromSolidColor(graphics, 1, 1); m_game.ResourceManager.AddAsset(mapParallax, "Radient Flatlands Parallax");
                        
                        mapTerrain = new Terrain(graphics, heightMap, mapTexture);
                        m_game.PrefabManager.AddPrefab(mapTerrain, "Radient Flatlands Terrain");

                        factoryBaseModel = m_game.ResourceManager.LoadAsset(Content, typeof(Model), "Factory Base", "Radient Flatlands FactoryBase") as Model;

                        GameConsts.Loading.Map_RadientFlatlands = LoadingStatus.Loaded;
                    }
                    #endregion

                    if (progress != null) progress.Report(50);

                    #region Create Factories
                    factoryBases = new FactoryBase[2];
                    factoryBases[0] = new FactoryBase(factoryBaseIDManager.GetNewID());
                    factoryBases[0].Base = new StaticGameObject();
                    factoryBases[0].Base.Model3D = factoryBaseModel;
                    factoryBases[0].Base.Transform.Scale = new Vector3(0.05f);
                    factoryBases[0].Base.Active = true;
                    factoryBases[0].Base.CullDraw = false;
                    factoryBases[0].Base.RenderBounds = false;

                    factoryBases[1] = new FactoryBase(factoryBaseIDManager.GetNewID());
                    //factoryBases[1].Base

                    #endregion

                    // Make the map
                    m_map = new Map(MapName.RadientFlatlands, mapParallax, mapTerrain, factoryBases);
                    m_map.AddAvailableUnitType(UnitType.Soldier, UnitType.Vehicle, UnitType.Ship, UnitType.Air, UnitType.Space);
                    break;
                #endregion

                #region Kalia
                case MapName.Kalia:
                    #region Load/Get the Data
                    if (GameConsts.Loading.Map_Kalia == LoadingStatus.Loaded)
                    {
                        heightMap = m_game.ResourceManager.GetAsset(typeof(Texture2D), "Kalia HeightMap") as Texture2D;
                        mapTexture = m_game.ResourceManager.GetAsset(typeof(Texture2D), "Kalia Texture") as Texture2D;
                        mapParallax = m_game.ResourceManager.GetAsset(typeof(Texture2D), "Kalia Parallax") as Texture2D;

                        mapTerrain = mapTerrain = m_game.PrefabManager.GetPrefab("Kalia Terrain") as Terrain;

                        factoryBaseModel = m_game.ResourceManager.GetAsset(typeof(Model), "Kalia FactoryBase") as Model;
                    }
                    else
                    {
                        GameConsts.Loading.Map_Kalia = LoadingStatus.CurrentlyLoading;

                        mapTexture = Color.Black.CreateTextureFromSolidColor(graphics, 1, 1);  m_game.ResourceManager.AddAsset(mapParallax, "Kalia Texture");
                        mapParallax = Color.Black.CreateTextureFromSolidColor(graphics, 1, 1); m_game.ResourceManager.AddAsset(mapParallax, "Kalia Parallax");
                        
                        mapTerrain = Terrain.CreateFlatTerrain(graphics, 256, 256, mapTexture);
                        heightMap = mapTerrain.HeightData.HeightMap; m_game.ResourceManager.AddAsset(mapParallax, "Kalia HeightMap");

                        m_game.PrefabManager.AddPrefab(mapTerrain, "Kalia Terrain");

                        factoryBaseModel = m_game.ResourceManager.LoadAsset(Content, typeof(Model), "Kalia Factory Base", "Kalia FactoryBase") as Model;

                        GameConsts.Loading.Map_Kalia = LoadingStatus.Loaded;
                    }
#endregion

                    if (progress != null) progress.Report(50);

                    #region Create Factories
                    factoryBases = new FactoryBase[2];
                    #endregion

                    // Make the map
                    m_map = new Map(MapName.Kalia, mapParallax, mapTerrain, factoryBases);
                    m_map.AddAvailableUnitType(UnitType.Space);
                    break;
                #endregion
            }
            #endregion

            if (progress != null) progress.Report(60);

            #region Create all the Units in the pool
            for (int i = 0; i < totalUnitsInPool; i++)
            {
                var unit = new Unit(unitIDManager.GetNewID(), UnitType.None);
                unit.Model3D = unanianGroundSoldierTPose;
                unit.Transform.Scale = new Vector3(0.015f);
                unit.Transform.Position = new Vector3((float)Consts.random.NextDouble(), -(float)Consts.random.NextDouble(), -5.50f + (float)Consts.random.NextDouble());
                unit.CullDraw = false;
                unit.RenderBounds = false;

                unit.Active = true;
                unit.Health.Alive = true;

                AnimationPlayer animPlayer = unit.PlayClip(unanianGroundSoldierWalkAnimClip);
                animPlayer.Looping = true;
                
                m_inactiveUnits.Add(unit);
            }
            #endregion

            if (progress != null) progress.Report(80);

            #region Subscribe to input Events
            // Subscribe to our Events
            // Pointer
            m_game.InputManager.PointerDown += InputManager_PointerDown;
            m_game.InputManager.PointerPressed += InputManager_PointerClicked;
            m_game.InputManager.PointerMoved += InputManager_PointerMoved;

            // Keyboard
            m_game.InputManager.Keyboard.KeyboardDown += Keyboard_KeyboardDown;
            m_game.InputManager.Keyboard.KeyboardPressed += Keyboard_KeyboardPressed;
            #endregion

            if (m_pageParameter.GameConnectionType == GameConnectionType.Singleplayer)
                m_pausedState = GamePausedState.Unpaused;

            if (progress != null) progress.Report(99);

            #region Set Default Player Values
            foreach (var player in m_team1.Players)
            {
                //player
            }
            foreach (var player in m_team2.Players)
            {
                //player
            }
            #endregion

            await Task.Delay(TimeSpan.FromSeconds(0.5));

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
        #region Pointer
        void InputManager_PointerDown(object sender, Anarian.Events.PointerPressedEventArgs e)
        {
            //Debug.WriteLine("{0}, Pressed", e.ToString());
        }

        public Ray? currentRay;
        public Vector3? rayPosOnTerrain;
        void InputManager_PointerClicked(object sender, Anarian.Events.PointerPressedEventArgs e)
        {
            //Debug.WriteLine("{0}, Pressed", e.ToString());

            if (e.Pointer == PointerPress.LeftMouseButton ||
                e.Pointer == PointerPress.Touch)
            {
                UniversalCamera camera = m_gameCamera;
                Ray ray = camera.GetMouseRay(
                    e.Position,
                    m_game.Graphics.GraphicsDevice.Viewport
                    );

                bool intersects = m_inactiveUnits[0].CheckRayIntersection(ray);
                Debug.WriteLine("Hit: {0}, Ray: {1}", intersects, ray.ToString());

                currentRay = ray;

                // Get the point on the terrain
                rayPosOnTerrain = m_map.Terrain.Intersects(ray);
            }
            if (e.Pointer == PointerPress.MiddleMouseButton)
            {
                Debug.WriteLine("Middle Mouse Pressed");
            }
            if (e.Pointer == PointerPress.RightMouseButton)
            {
                m_inactiveUnits[0].AnimationState.AnimationPlayer.Paused = !m_inactiveUnits[0].AnimationState.AnimationPlayer.Paused;
            }
        }

        void InputManager_PointerMoved(object sender, Anarian.Events.PointerMovedEventArgs e)
        {
            //Debug.WriteLine("{0}, Pressed", e.ToString());

        }
        #endregion

        #region Keyboard
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

                case Keys.Up:   case Keys.NumPad8: cam.Pitch = uniCam.Pitch + MathHelper.ToRadians(2); break;
                case Keys.Down: case Keys.NumPad2: cam.Pitch = uniCam.Pitch + MathHelper.ToRadians(-2); break;

                case Keys.Left:  case Keys.NumPad4: cam.Yaw = uniCam.Yaw + MathHelper.ToRadians(2); break;
                case Keys.Right: case Keys.NumPad6: cam.Yaw = uniCam.Yaw + MathHelper.ToRadians(-2); break;

                case Keys.P: case Keys.NumPad1:
                case Keys.NumPad7: cam.Roll = uniCam.Roll + MathHelper.ToRadians(2); break;

                case Keys.O: case Keys.NumPad3:
                case Keys.NumPad9: cam.Roll = uniCam.Roll + MathHelper.ToRadians(-2); break;

                case Keys.D0: case Keys.NumPad0: uniCam.ResetCamera(); break;
                case Keys.D5: case Keys.NumPad5: uniCam.ResetRotations(); break;
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
        #endregion

        #region Update
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        public override void Update(GameTime gameTime)
        {
            if (m_currentLoadingPercentage < 100) { base.Update(gameTime); return; }
            // Since we passed the check, that means that the game is fully loaded

            // Update the Camera
            m_gameCamera.Update(null);

            m_map.Update(gameTime);
            foreach (var i in m_inactiveUnits)
            {
                i.Update(gameTime);
            }

            // Update everything else
            switch (m_pausedState)
            {
                case GamePausedState.Unpaused:
                    break;
                case GamePausedState.Paused:            m_overlay.ApplyEffect(gameTime); break;
                case GamePausedState.WaitingForData:    m_overlay.ApplyEffect(gameTime); break;
            }

            // Regular Updates
            if (rayPosOnTerrain.HasValue)
            {
                m_inactiveUnits[0].Transform.MoveToPosition(gameTime, rayPosOnTerrain.Value);
            }

            float height = m_map.Terrain.GetHeightAtPoint(m_inactiveUnits[0].Transform.Position);
            if (height != float.MaxValue)
            {
                Vector3 pos = m_inactiveUnits[0].Transform.Position;
                pos.Y = height;
                m_inactiveUnits[0].Transform.Position = pos;
            }
            
            m_gameCamera.WorldPositionToChase = m_inactiveUnits[0].Transform.WorldMatrix;

            //-- Update the Menu
            base.Update(gameTime);
        }
        #endregion

        #region Draw
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics); }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            if (m_currentLoadingPercentage < 100) { 
                DrawLoadingMenu(gameTime, spriteBatch, graphics);
                base.Draw(gameTime, spriteBatch, graphics); return;
            }

            // Draw The Map
            m_map.Draw(gameTime, spriteBatch, graphics, m_gameCamera);
            foreach (var i in m_inactiveUnits)
            {
                i.Draw(gameTime, spriteBatch, graphics, m_gameCamera);
            }

            switch (m_pausedState)
            {
                case GamePausedState.Unpaused:
                    break;
                case GamePausedState.Paused:            DrawPaused(gameTime, spriteBatch, graphics);           break;                             
                case GamePausedState.WaitingForData:    DrawWaitingForData(gameTime, spriteBatch, graphics);   break;
            }

            base.Draw(gameTime, spriteBatch, graphics);
        }

        public void DrawLoadingMenu(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            //graphics.Clear(Color.Black);
            var screenRect = AnarianConsts.ScreenRectangle;
            var centerOfScreen = new Vector2(screenRect.Width / 2.0f, screenRect.Height / 2.0f);
            
            spriteBatch.Begin();
            //spriteBatch.Draw(m_mapPreview, AnarianConsts.ScreenRectangle, Color.White);
            spriteBatch.End();

            // Loading Outline
            var outlineRect = new Rectangle(0,
                                      (int)(screenRect.Height * 0.75),
                                            screenRect.Width,
                                      (int)(screenRect.Height * 0.10)); 
            PrimitiveHelper2D.DrawRect(spriteBatch, Color.Wheat, outlineRect);

            // Loading Bar
            int distanceFromTopBottom = 5;
            int distanceFromLeftRight = 0;
            var loadingBar = new Rectangle(0                  + distanceFromLeftRight,
                                           outlineRect.Y      + distanceFromTopBottom,
                                           m_currentLoadingPercentage * ((outlineRect.Width - (distanceFromLeftRight * 2)) / 100) ,
                                           outlineRect.Height - (distanceFromTopBottom * 2));
            PrimitiveHelper2D.DrawRect(spriteBatch, Color.ForestGreen, loadingBar);

            // Text
            var spriteFont = m_game.ResourceManager.GetAsset(typeof(SpriteFont), "EmpiresOfTheIVFont") as SpriteFont;
            
            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, "Loading", new Vector2(centerOfScreen.X - 50, screenRect.Height * 0.15f), Color.Wheat);
            spriteBatch.DrawString(spriteFont, m_currentLoadingPercentage + "%", new Vector2(outlineRect.Width - 100, outlineRect.Y - 50), Color.Wheat);
            spriteBatch.End();
        }

        public void DrawPaused(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            m_overlay.Draw(gameTime, spriteBatch);
        }

        public void DrawWaitingForData(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            m_overlay.Draw(gameTime, spriteBatch);
        }
        #endregion
    }
}
