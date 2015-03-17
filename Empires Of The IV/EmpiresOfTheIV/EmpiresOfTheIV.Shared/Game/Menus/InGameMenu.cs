using Anarian;
using Anarian.DataStructures;
using Anarian.DataStructures.Animation;
using Anarian.DataStructures.Animation.Aux;
using Anarian.DataStructures.Rendering;
using Anarian.DataStructures.ScreenEffects;
using Anarian.Enumerators;
using Anarian.Events;
using Anarian.GUI;
using Anarian.Helpers;
using Anarian.IDManagers;
using Anarian.Interfaces;
using EmpiresOfTheIV.Game.Enumerators;
using EmpiresOfTheIV.Game.GameObjects;
using EmpiresOfTheIV.Game.GameObjects.Factories;
using EmpiresOfTheIV.Game.Loading;
using EmpiresOfTheIV.Game.Menus.PageParameters;
using EmpiresOfTheIV.Game.Networking;
using EmpiresOfTheIV.Game.Players;
using KillerrinStudiosToolkit.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        SpriteFont m_empiresOfTheIVFont;
        SpriteFont m_empiresOfTheIVFontSmall;

        Texture2D m_currencyTexture; 
        Texture2D m_metalTexture;
        Texture2D m_energyTexture;
        Texture2D m_unitCapTexture;

        Rectangle screenRect = AnarianConsts.ScreenRectangle;
        Vector2 centerOfScreen;

        #region Loading
        private object loadinglockObject = new object();

        LoadingProgress m_currentLoadingProgress;
        Progress<LoadingProgress> m_loadingProgress;
        Task<LoadingStatus> m_loadingContentTask;
        Texture2D m_loadingMiniMap;
        #endregion

        #region Networking
        private object opponentloadedLockObject = new object();
        bool opponentFullyLoaded = false;

        Timer m_networkTimer;
        #endregion

        #region Page Parameters
        InGamePageParameter m_pageParameter;
        Player m_me;
        Team m_team1;
        Team m_team2;

        ChatManager m_chatManager;
        #endregion

        UniversalCamera m_gameCamera;
        int m_cameraMovementScreenBuffer = 30;

        List<Unit> m_activeUnits;
        List<Unit> m_inactiveUnits;
        Map m_map;
        #endregion

        #region Constructors and Messages
        public InGameMenu(EmpiresOfTheIVGame game, object parameter)
            :base(game, parameter, GameState.InGame)
        {
            m_networkManager = m_game.NetworkManager;
            m_networkTimer = new Timer(m_networkManager.ConnectionPreventTimeoutTick);
            m_networkTimer.Completed += m_networkTimer_Completed;

            m_pausedState = GamePausedState.WaitingForData;

            m_currentLoadingProgress = new LoadingProgress(0, "");
            m_loadingProgress = new Progress<LoadingProgress>();
            m_loadingProgress.ProgressChanged += m_loadingProgress_ProgressChanged;

            m_overlay = new Overlay(m_game.GraphicsDevice, Color.Black);
            m_overlay.FadePercentage = 0.75f;

            centerOfScreen = new Vector2(screenRect.Width / 2.0f, screenRect.Height / 2.0f);

            // Get basic Assets
            m_empiresOfTheIVFont = m_game.ResourceManager.GetAsset(typeof(SpriteFont), "EmpiresOfTheIVFont") as SpriteFont;
            m_empiresOfTheIVFontSmall = m_game.ResourceManager.GetAsset(typeof(SpriteFont), "EmpiresOfTheIVFont Small") as SpriteFont;

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

            #region Load the Minimap
            switch (m_pageParameter.MapName)
            {
                case MapName.RadientValley:
                    if (GameConsts.Loading.Map_RadientFlatlands == LoadingStatus.Loaded) {
                        m_loadingMiniMap = m_game.ResourceManager.GetAsset(typeof(Texture2D), "Radient Valley MiniMap") as Texture2D;
                    }
                    else {
                        m_loadingMiniMap = m_game.ResourceManager.LoadAsset(m_game.Content, typeof(Texture2D), "Textures/Maps/Radient Valley MiniMap") as Texture2D;
                    }
                    break;
                case MapName.Kalia:
                    if (GameConsts.Loading.Map_Kalia == LoadingStatus.Loaded) {
                        m_loadingMiniMap = m_game.ResourceManager.GetAsset(typeof(Texture2D), "Kalia MiniMap") as Texture2D;
                    }
                    else {
                        m_loadingMiniMap = m_game.ResourceManager.LoadAsset(m_game.Content, typeof(Texture2D), "Textures/Maps/Kalia MiniMap") as Texture2D;
                    }
                    break;
                default: m_loadingMiniMap = null; break;
            }
            #endregion

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

            // De-Subscribe to our Events
            // Pointer
            m_game.InputManager.PointerDown -= InputManager_PointerDown;
            m_game.InputManager.PointerPressed -= InputManager_PointerClicked;
            m_game.InputManager.PointerMoved -= InputManager_PointerMoved;

            // Keyboard
            m_game.InputManager.Keyboard.KeyboardDown -= Keyboard_KeyboardDown;
            m_game.InputManager.Keyboard.KeyboardPressed -= Keyboard_KeyboardPressed;
        }

        public override void SendMessage(object message)
        {
            base.SendMessage(message);
        }

        private void StateManager_OnBackButtonPressed(object sender, EventArgs e)
        {
            if (m_currentLoadingProgress.Progress < 100) { return; }
            return;
        }
        #endregion

        #region Loading
        private async Task<LoadingStatus> LoadContent(IProgress<LoadingProgress> progress, ContentManager Content, GraphicsDevice graphics)
        {
            if (progress != null) progress.Report(new LoadingProgress(0, "Initial Setup"));

            m_currencyTexture = m_game.ResourceManager.GetAsset(typeof(Texture2D), "Currency") as Texture2D;
            m_metalTexture = m_game.ResourceManager.GetAsset(typeof(Texture2D), "Metal") as Texture2D;
            m_energyTexture = m_game.ResourceManager.GetAsset(typeof(Texture2D), "Energy") as Texture2D;
            m_unitCapTexture = m_game.ResourceManager.GetAsset(typeof(Texture2D), "Unit Cap") as Texture2D;

            #region Setup Variables
            m_gameCamera = new UniversalCamera();
            m_gameCamera.AspectRatio = m_game.SceneManager.CurrentScene.Camera.AspectRatio;
            m_gameCamera.Speed = 0.8f;

            //Camera Position: {X:4.199995 Y:55.02913 Z:15.78831}, 
            m_gameCamera.DefaultCameraPosition = new Vector3(4.20f, 50.03f, 15.79f);

            //Camera Rotation: {M11:1 M12:0 M13:0 M14:0} {M21:0 M22:0.2419228 M23:-0.9702981 M24:0} {M31:0 M32:0.9702981 M33:0.2419228 M34:0} {M41:0 M42:0 M43:0 M44:1}
            m_gameCamera.DefaultCameraRotation = new Matrix(1, 0, 0, 0,
                                                 0, 0.242f, -0.97f, 0,
                                                 0, 0.97f, 0.24f, 0,
                                                 0, 0, 0, 1);

            m_gameCamera.ResetViewToDefaults();

            int totalUnitsInPool = (int)(m_pageParameter.maxUnitsPerPlayer * (m_team1.PlayerCount + m_team2.PlayerCount));
            m_activeUnits = new List<Unit>(totalUnitsInPool);
            m_inactiveUnits = new List<Unit>(totalUnitsInPool);

            IDManager unitIDManager = new IDManager();
            IDManager factoryBaseIDManager = new IDManager();
            #endregion

            if (progress != null) progress.Report(new LoadingProgress(20, "Loading Empires"));

            #region Load Empires
            #region Unanian Empire
            AnimatedModel unanianGroundSoldierTPose = null;
            AnimatedModel unanianGroundSoldierAnimation = null;
            AnimationClip unanianGroundSoldierWalkAnimClip = null;

            Model         unanianFactory = null;
            AnimatedModel unanianSpaceshipFighter = null;

            if (m_team1.IsPlayerEmpire(EmpireType.UnanianEmpire) || m_team2.IsPlayerEmpire(EmpireType.UnanianEmpire))
            {
                if (GameConsts.Loading.Empire_UnanianEmpireLoaded == LoadingStatus.Loaded)
                {
                    unanianGroundSoldierTPose = m_game.ResourceManager.GetAsset(typeof(AnimatedModel), UnitID.UnanianSoldier.ToString() + "|" + ModelType.AnimatedModel.ToString()) as AnimatedModel;
                    unanianGroundSoldierAnimation = m_game.ResourceManager.GetAsset(typeof(AnimatedModel), UnitID.UnanianSoldier.ToString() + "|" + ModelType.Animation.ToString()) as AnimatedModel;

                    unanianSpaceshipFighter = m_game.ResourceManager.GetAsset(typeof(AnimatedModel), UnitID.UnanianSpaceFighter.ToString() + "|" + ModelType.AnimatedModel.ToString()) as AnimatedModel;

                    unanianFactory = m_game.ResourceManager.GetAsset(typeof(Model), "Unanian Factory") as Model;
                }
                else
                {
                    GameConsts.Loading.Empire_UnanianEmpireLoaded = LoadingStatus.CurrentlyLoading;

                    unanianGroundSoldierTPose = m_game.ResourceManager.LoadAsset(Content, typeof(AnimatedModel), "Models/Units/Unanian Empire/t-pose_3", UnitID.UnanianSoldier.ToString() + "|" + ModelType.AnimatedModel.ToString()) as AnimatedModel;
                    unanianGroundSoldierAnimation = m_game.ResourceManager.LoadAsset(Content, typeof(AnimatedModel), "Models/Units/Unanian Empire/walk", UnitID.UnanianSoldier.ToString() + "|" + ModelType.Animation.ToString()) as AnimatedModel;

                    unanianSpaceshipFighter = m_game.ResourceManager.LoadAsset(Content, typeof(AnimatedModel), "Models/Units/Unanian Empire/Unanian Fighter", UnitID.UnanianSpaceFighter.ToString() + "|" + ModelType.AnimatedModel.ToString()) as AnimatedModel;

                    unanianFactory = m_game.ResourceManager.LoadAsset(Content, typeof(Model), "Models/Factories/Unanian Empire/Unanian Factory") as Model;
                    GameConsts.Loading.Empire_UnanianEmpireLoaded = LoadingStatus.Loaded;
                }

                unanianGroundSoldierWalkAnimClip = unanianGroundSoldierAnimation.Clips[0];
            }
            #endregion

            #region Crescanian Confederacy
            AnimatedModel crescanianGroundSoldierTPose = null;
            AnimatedModel crescanianGroundSoldierAnimation = null;
            AnimationClip crescanianGroundSoldierWalkAnimClip = null;

            Model         crescanianFactory = null;
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

            Model         kingdomOfEdolasFactory= null;
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

            if (progress != null) progress.Report(new LoadingProgress(40, "Loading Map"));

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
                case MapName.RadientValley:
                    #region Load/Get the Data
                    if (GameConsts.Loading.Map_RadientFlatlands == LoadingStatus.Loaded)
                    {
                        heightMap = m_game.ResourceManager.GetAsset(typeof(Texture2D), "Radient Valley HeightMap") as Texture2D;
                        mapTexture = m_game.ResourceManager.GetAsset(typeof(Texture2D), "Radient Valley Texture") as Texture2D;
                        mapParallax = m_game.ResourceManager.GetAsset(typeof(Texture2D), "Radient Valley Parallax") as Texture2D;

                        mapTerrain = m_game.PrefabManager.GetPrefab("Radient Valley Terrain") as Terrain;

                        factoryBaseModel = m_game.ResourceManager.GetAsset(typeof(Model), "Radient Valley FactoryBase") as Model;
                    }
                    else 
                    {
                        GameConsts.Loading.Map_RadientFlatlands = LoadingStatus.CurrentlyLoading;

                        heightMap = m_game.ResourceManager.LoadAsset(Content, typeof(Texture2D), "Textures/Maps/Radient Valley HeightMap") as Texture2D;
                        mapTexture = m_game.ResourceManager.LoadAsset(Content, typeof(Texture2D), "Textures/Maps/Radient Valley Texture") as Texture2D;
                        mapParallax = Color.Black.CreateTextureFromSolidColor(graphics, 1, 1); m_game.ResourceManager.AddAsset(mapParallax, "Radient Valley Parallax");
                        
                        mapTerrain = new Terrain(graphics, heightMap, mapTexture);
                        m_game.PrefabManager.AddPrefab(mapTerrain, "Radient Valley Terrain");

                        factoryBaseModel = m_game.ResourceManager.LoadAsset(Content, typeof(Model), "Models/Factories/Factory Base", "Radient Valley FactoryBase") as Model;

                        GameConsts.Loading.Map_RadientFlatlands = LoadingStatus.Loaded;
                    }
                    #endregion

                    if (progress != null) progress.Report(new LoadingProgress(50, "Creating Factories"));

                    #region Create Factories
                    factoryBases = new FactoryBase[2];

                    // Because the Meshes Bounding Sphere is messed up, we fix it here
                    foreach (ModelMesh mesh in factoryBaseModel.Meshes)
                    {
                        var bs = mesh.BoundingSphere;
                        bs.Radius = 230.0f;
                        mesh.BoundingSphere = bs;
                    }


                    // Factory 1
                    factoryBases[0] = new FactoryBase(factoryBaseIDManager.GetNewID());
                    factoryBases[0].Base = new StaticGameObject();
                    factoryBases[0].Base.Transform.Position = new Vector3(-70.0f, 0.0f, -10.0f);
                    factoryBases[0].Base.Transform.Scale = new Vector3(0.05f);
                    factoryBases[0].Base.Transform.Rotation = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(-90.0f), 0.0f, 0.0f);
                    factoryBases[0].Base.Transform.CreateAllMatrices();
                    factoryBases[0].Base.Model3D = factoryBaseModel;
                    factoryBases[0].Base.Active = true;
                    factoryBases[0].Base.CullDraw = false;
                    factoryBases[0].Base.RenderBounds = true;
                
                    // Factory 2
                    factoryBases[1] = new FactoryBase(factoryBaseIDManager.GetNewID());
                    factoryBases[1].Base = new StaticGameObject();
                    factoryBases[1].Base.Transform.Position = new Vector3(70.0f, 0.0f, 10.0f);
                    factoryBases[1].Base.Transform.Scale = new Vector3(0.05f);
                    factoryBases[1].Base.Transform.Rotation = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(90.0f), 0.0f, 0.0f);
                    factoryBases[1].Base.Transform.CreateAllMatrices();
                    factoryBases[1].Base.Model3D = factoryBaseModel;
                    factoryBases[1].Base.Active = true;
                    factoryBases[1].Base.CullDraw = false;
                    factoryBases[1].Base.RenderBounds = true;
                    #endregion

                    // Make the map
                    m_map = new Map(MapName.RadientValley, mapParallax, mapTerrain, factoryBases);
                    m_map.AddAvailableUnitType(UnitType.Soldier, UnitType.Vehicle, UnitType.Ship, UnitType.Air, UnitType.Space);

                    // Set GameCamera Starting Positions and Limits
                    var gameCameraPos = m_gameCamera.Position; 
                    if (m_team1.Exists(m_pageParameter.me.ID)) {
                        // Set Factory Owners
                        if (m_team2.PlayerCount > 0)
                            GameFactory.CreateFactoryOnFactoryBase(factoryBases[1], m_team2.Players[0]);
                        GameFactory.CreateFactoryOnFactoryBase(factoryBases[0], m_pageParameter.me);

                        switch (m_pageParameter.me.EmpireType)
                        {
                            case EmpireType.UnanianEmpire:                                break;
                            case EmpireType.CrescanianConfederation:                      break;
                            case EmpireType.TheKingdomOfEdolas:                           break;
                        }

                        // Set the Game Cameras Position
                        gameCameraPos.X = factoryBases[0].Base.Transform.Position.X;
                        gameCameraPos.Z = factoryBases[0].Base.Transform.Position.Z + 10;
                    }
                    else if (m_team2.Exists(m_pageParameter.me.ID)) {
                        // Set Factory Owners
                        if (m_team1.PlayerCount > 0)
                            GameFactory.CreateFactoryOnFactoryBase(factoryBases[0], m_team1.Players[0]);
                        GameFactory.CreateFactoryOnFactoryBase(factoryBases[1], m_pageParameter.me);

                        switch (m_pageParameter.me.EmpireType)
                        {
                            case EmpireType.UnanianEmpire: break;
                            case EmpireType.CrescanianConfederation: break;
                            case EmpireType.TheKingdomOfEdolas: break;
                        }

                        // Set the Game Cameras Position
                        gameCameraPos.X = factoryBases[1].Base.Transform.Position.X;
                        gameCameraPos.Z = factoryBases[1].Base.Transform.Position.Z + 10;                        
                    }
                    m_gameCamera.Position = gameCameraPos;
                    m_gameCamera.DefaultCameraPosition = m_gameCamera.Position;

                    // MathHelper.Clamp(gameCameraPosition.Y, 30.0f, 56.0f);
                    m_gameCamera.MinClamp = new Vector3(-92.60f, m_gameCamera.DefaultCameraPosition.Y - 10, -18.35f);
                    m_gameCamera.MaxClamp = new Vector3(85.80f, m_gameCamera.DefaultCameraPosition.Y + 10,  36.74f);

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

                    if (progress != null) progress.Report(new LoadingProgress(50, "Creating Factories"));

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

            if (progress != null) progress.Report(new LoadingProgress(60, "Setting up Units"));

            #region Create all the Units in the pool
            for (int i = 0; i < totalUnitsInPool; i++)
            {
                var unit = new Unit(unitIDManager.GetNewID(), UnitType.None);
                GameFactory.CreateUnit(unit, UnitID.UnanianSpaceFighter, new Vector3((float)Consts.random.NextDouble(), -(float)Consts.random.NextDouble(), -5.50f + (float)Consts.random.NextDouble()));                
                m_activeUnits.Add(unit);
            }
            #endregion

            if (progress != null) progress.Report(new LoadingProgress(80, "Preforming Technical Magic"));

            #region Subscribe to input Events
            // Subscribe to our Events
            // Pointer
            m_game.InputManager.PointerDown += InputManager_PointerDown;
            m_game.InputManager.PointerPressed += InputManager_PointerClicked;

            // Because Phone will have different input interactions, we will not subscribe to PointerMoved to save processing
            if (KillerrinStudiosToolkit.KillerrinApplicationData.OSType == KillerrinStudiosToolkit.Enumerators.ClientOSType.WindowsPhone81) { }
            else
            {
                m_game.InputManager.PointerMoved += InputManager_PointerMoved;
            }

            // Keyboard
            m_game.InputManager.Keyboard.KeyboardDown += Keyboard_KeyboardDown;
            m_game.InputManager.Keyboard.KeyboardPressed += Keyboard_KeyboardPressed;
            #endregion

            if (progress != null) progress.Report(new LoadingProgress(99, "Taking out Ninjas"));

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

            //await Task.Delay(TimeSpan.FromSeconds(0.5));
            if (m_pageParameter.GameConnectionType == GameConnectionType.Singleplayer)
                m_pausedState = GamePausedState.Unpaused;
            else
            {
                m_pausedState = GamePausedState.WaitingForData;
                if (m_networkManager.HostSettings == KillerrinStudiosToolkit.Enumerators.HostType.Client)
                {
                    SystemPacket sp = new SystemPacket(true, SystemPacketID.GameLoaded, "");
                    m_networkManager.SendMessage(sp.ThisToJson());
                }
            }

            // Send the final report and return loaded
            if (progress != null) progress.Report(new LoadingProgress(100, "Ready to Play!"));
            return LoadingStatus.Loaded;
        }
        void m_loadingProgress_ProgressChanged(object sender, LoadingProgress e)
        {
            Debug.WriteLine("m_loadingProgress_ProgressChanged: " + e.ToString());
            lock (loadinglockObject)
            {
                m_currentLoadingProgress = e;
            }
        }
        #endregion

        #region Networking
        void m_networkTimer_Completed(object sender, EventArgs e)
        {
            if (m_networkManager.HostSettings == KillerrinStudiosToolkit.Enumerators.HostType.Host)
            {
                Debug.WriteLine("Sending Connection Tick");

                // Send an ACK to keep the connection opened
                SystemPacket sp = new SystemPacket(true, SystemPacketID.ConnectionTick, "");
                m_networkManager.SendMessage(sp.ThisToJson());

            }

            // Reset the timer
            m_networkTimer.Reset();
        }


        private void NetworkManager_OnConnected(object sender, OnConnectedEventArgs e)
        {
        }

        private void NetworkManager_OnDisconnected(object sender, EventArgs e)
        {
        }

        private void NetworkManager_OnMessageRecieved(object sender, ReceivedMessageEventArgs e)
        {
            if (e.Message == Consts.Game.NetworkManager.LanHelper.ConnectionCloseMessage)
            {
                return;
            }

            // Get the regular object
            JObject jObject = null;
            EotIVPacket regularPacket = null;
            try
            {
                jObject = JObject.Parse(e.Message);
                regularPacket = JsonConvert.DeserializeObject<EotIVPacket>(jObject.ToString());
                Debug.WriteLine("Deserialized Packet");
            }
            catch (Exception) { return; }

            // Now parse the object to get the right derived class
            if (regularPacket.PacketType == PacketType.Chat)
            {
                Debug.WriteLine("Chat Packet Recieved");
                ChatPacket chatPacket = JsonConvert.DeserializeObject<ChatPacket>(jObject.ToString());

                try
                {
                    m_chatManager.AddMessage(chatPacket.Message);
                }
                catch (Exception) { }
            }
            else if (regularPacket.PacketType == PacketType.System)
            {
                Debug.WriteLine("System Packet Recieved");
                SystemPacket systemPacket = JsonConvert.DeserializeObject<SystemPacket>(jObject.ToString());

                if (systemPacket.ID == SystemPacketID.GameLoaded)
                {
                    lock(opponentloadedLockObject)
                    {
                        opponentFullyLoaded = true;
                    }
                }
                else if (systemPacket.ID == SystemPacketID.GameBegin)
                {
                    m_pausedState = GamePausedState.Unpaused;
                }
            }
        }
        #endregion

        #region Input
        #region Pointer
        public List<PointerPressedEventArgs> activeTouchPointers = new List<PointerPressedEventArgs>();
        bool middleMouseDown = false;
        void InputManager_PointerDown(object sender, Anarian.Events.PointerPressedEventArgs e)
        {
            if (m_pausedState != GamePausedState.Unpaused) return;
            //Debug.WriteLine("{0}, Pressed", e.ToString());

            if (e.Pointer == PointerPress.Touch)
            {
                activeTouchPointers.Add(e);
            }
            else if (e.Pointer == PointerPress.MiddleMouseButton)
            {
                middleMouseDown = true;

                var delta = e.DeltaPosition;
                delta.Normalize();
                float deltaBuffer = 0.20f;

                if (delta.X < 0 - deltaBuffer) {
                    m_gameCamera.Move(e.GameTime, m_gameCamera.CameraRotation.Right);
                }
                else if (delta.X > 0 + deltaBuffer) {
                    m_gameCamera.Move(e.GameTime, -m_gameCamera.CameraRotation.Right);
                }

                if (delta.Y < 0 - deltaBuffer) {
                    m_gameCamera.Move(e.GameTime, -m_gameCamera.CameraRotation.Up);
                }
                else if (delta.Y > 0 + deltaBuffer) {
                    m_gameCamera.Move(e.GameTime, m_gameCamera.CameraRotation.Up);
                }
            }
        }

        public Ray? currentRay;
        public Vector3? rayPosOnTerrain;
        void InputManager_PointerClicked(object sender, Anarian.Events.PointerPressedEventArgs e)
        {
            if (m_pausedState != GamePausedState.Unpaused) return;
            Debug.WriteLine("{0}, Pressed", e.ToString());

            if (e.Pointer == PointerPress.LeftMouseButton ||
                e.Pointer == PointerPress.Touch)
            {
                Ray ray = m_gameCamera.GetMouseRay(
                    e.Position,
                    m_game.Graphics.GraphicsDevice.Viewport
                    );

                bool intersects = m_activeUnits[unitIndex].CheckRayIntersection(ray);
                //Debug.WriteLine("Hit: {0}, Ray: {1}", intersects, ray.ToString());

                //currentRay = ray;

                // Get the point on the terrain
                //rayPosOnTerrain = m_map.Terrain.Intersects(ray);
            }

            if (e.Pointer == PointerPress.MiddleMouseButton)
            {
                middleMouseDown = false;
            }
        }


        PointerMovedEventArgs m_lastPointerMovedEventArgs = new PointerMovedEventArgs(new GameTime());
        void InputManager_PointerMoved(object sender, Anarian.Events.PointerMovedEventArgs e)
        {
            if (m_pausedState != GamePausedState.Unpaused) return;
            //Debug.WriteLine("{0}, Moved", e.ToString());

            m_lastPointerMovedEventArgs = e;

            if (e.InputType == InputType.Touch)
            {
            }
        }
        #endregion

        #region Keyboard
        void Keyboard_KeyboardDown(object sender, Anarian.Events.KeyboardPressedEventArgs e)
        {
            if (m_pausedState != GamePausedState.Unpaused) return;

            switch (e.KeyClicked)
            {
                    // Vertical
                case Keys.W: m_gameCamera.Move(e.GameTime, m_gameCamera.CameraRotation.Up); break;
                case Keys.S: m_gameCamera.Move(e.GameTime, -m_gameCamera.CameraRotation.Up); break;

                    // Horizontal
                case Keys.A: m_gameCamera.Move(e.GameTime, -m_gameCamera.CameraRotation.Right); break;
                case Keys.D: m_gameCamera.Move(e.GameTime, m_gameCamera.CameraRotation.Right); break;

                    // Zoom
                case Keys.Q: m_gameCamera.Move(e.GameTime, -m_gameCamera.CameraRotation.Forward); break;
                case Keys.E: m_gameCamera.Move(e.GameTime, m_gameCamera.CameraRotation.Forward); break;

                //case Keys.Up:   case Keys.NumPad8: m_gameCamera.Pitch = m_gameCamera.Pitch + MathHelper.ToRadians(2); break;
                //case Keys.Down: case Keys.NumPad2: m_gameCamera.Pitch = m_gameCamera.Pitch + MathHelper.ToRadians(-2); break;
                //
                //case Keys.Left:  case Keys.NumPad4: m_gameCamera.Yaw = m_gameCamera.Yaw + MathHelper.ToRadians(2); break;
                //case Keys.Right: case Keys.NumPad6: m_gameCamera.Yaw = m_gameCamera.Yaw + MathHelper.ToRadians(-2); break;
                //
                //case Keys.P: case Keys.NumPad1:
                //case Keys.NumPad7: m_gameCamera.Roll = m_gameCamera.Roll + MathHelper.ToRadians(2); break;
                //
                //case Keys.O: case Keys.NumPad3:
                //case Keys.NumPad9: m_gameCamera.Roll = m_gameCamera.Roll + MathHelper.ToRadians(-2); break;

                //case Keys.D0: case Keys.NumPad0: m_gameCamera.ResetCamera(); break;
                //case Keys.D5: case Keys.NumPad5: m_gameCamera.ResetRotations(); break;

                case Keys.LeftControl: Debug.WriteLine("Camera Position: {0}, \n Camera Rotation: {1}", m_gameCamera.Position, m_gameCamera.CameraRotation); break;
            }
        }
        void Keyboard_KeyboardPressed(object sender, Anarian.Events.KeyboardPressedEventArgs e)
        {
            if (m_pausedState != GamePausedState.Unpaused) return;

            switch (e.KeyClicked)
            {
                case Keys.Space: m_gameCamera.SwitchCameraMode(); break;
                case Keys.OemCloseBrackets:
                case Keys.Add: 
                    unitIndex++; 
                    currentRay = null;
                    rayPosOnTerrain = null;
                    break;
                case Keys.OemOpenBrackets:
                case Keys.Subtract:
                    unitIndex--; 
                    currentRay = null;
                    rayPosOnTerrain = null;
                    break;
            }

            if (unitIndex >= m_activeUnits.Count) unitIndex = 0;
            if (unitIndex <= -1) unitIndex = m_activeUnits.Count - 1;
        }

        public int unitIndex = 0;
        #endregion
        #endregion

        #region Update
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        public override void Update(GameTime gameTime)
        {
            m_networkTimer.Update(gameTime);

            // Check if the game is fully loaded
            if (m_currentLoadingProgress.Progress < 100) { base.Update(gameTime); return; }

            // Check if the game is paused
            switch (m_pausedState)
            {
                case GamePausedState.Paused: UpdatePaused(gameTime); base.Update(gameTime); return;
                case GamePausedState.WaitingForData: UpdateWaitingForData(gameTime); base.Update(gameTime); return;
            }

            #region First thing we do is Update the GameCamera
            var screenRect = AnarianConsts.ScreenRectangle;
            var previousCamY = m_gameCamera.Position.Y;

            #region Mouse Controls
            if (m_lastPointerMovedEventArgs.InputType == InputType.Mouse)
            {
                if (!middleMouseDown)
                {
                    var deltaPos = m_lastPointerMovedEventArgs.DeltaPosition;

                    if (m_lastPointerMovedEventArgs.Position.X <= (screenRect.X + m_cameraMovementScreenBuffer))
                    {
                        m_gameCamera.Move(gameTime, -m_gameCamera.CameraRotation.Right);
                    }
                    else if (m_lastPointerMovedEventArgs.Position.X >= (screenRect.Width - m_cameraMovementScreenBuffer))
                    {
                        m_gameCamera.Move(gameTime, m_gameCamera.CameraRotation.Right);
                    }

                    if (m_lastPointerMovedEventArgs.Position.Y <= (screenRect.Y + m_cameraMovementScreenBuffer))
                    {
                        m_gameCamera.Move(gameTime, m_gameCamera.CameraRotation.Up);
                    }
                    else if (m_lastPointerMovedEventArgs.Position.Y >= (screenRect.Height - m_cameraMovementScreenBuffer))
                    {
                        m_gameCamera.Move(gameTime, -m_gameCamera.CameraRotation.Up);
                    }

                    var camPos = m_gameCamera.Position;
                    camPos.Y = previousCamY;
                    m_gameCamera.Position = camPos;

                    var mouseWheelDelta = m_game.InputManager.Mouse.GetMouseWheelDelta();

                    if (mouseWheelDelta > 0)
                    {
                        m_gameCamera.Move(gameTime, m_gameCamera.CameraRotation.Forward * 2.0f);
                    }
                    else if (mouseWheelDelta < 0)
                    {
                        m_gameCamera.Move(gameTime, -m_gameCamera.CameraRotation.Forward * 2.0f);
                    }
                }
            }
            #endregion

            #region Touch Controls
            if (activeTouchPointers.Count == 2)
            {
                var movementBuffer = 0.2f;
                var deltaTouch = activeTouchPointers[0].DeltaPosition;
                deltaTouch.Normalize();

                if (deltaTouch.X < 0 - movementBuffer)
                {
                    m_gameCamera.Move(gameTime, -m_gameCamera.CameraRotation.Right);
                }
                else if (deltaTouch.X > 0 + movementBuffer)
                {
                    m_gameCamera.Move(gameTime, m_gameCamera.CameraRotation.Right);
                }

                if (deltaTouch.Y < 0 - movementBuffer)
                {
                    m_gameCamera.Move(gameTime, m_gameCamera.CameraRotation.Up);
                }
                else if (deltaTouch.Y > 0 + movementBuffer)
                {
                    m_gameCamera.Move(gameTime, -m_gameCamera.CameraRotation.Up);
                }
            }

            // Since we are done with the touch input, we can clear the pointers
            activeTouchPointers.Clear();
            #endregion

            m_gameCamera.Update(gameTime);
            #endregion

            // Then the Map
            m_map.Update(gameTime);

            // If it is not, we can proceed to update the game
            m_pageParameter.me.Update(gameTime);
            
            // Move the Unit
            if (rayPosOnTerrain.HasValue)
            {
                m_activeUnits[unitIndex].Transform.MoveToPosition(gameTime, (rayPosOnTerrain.Value + new Vector3(0.0f, m_activeUnits[unitIndex].HeightAboveTerrain, 0.0f)));
            }

            // Set all the active units to be on the terrain
            for (int i = 0; i < m_activeUnits.Count; i++)
            {
                float height = m_map.Terrain.GetHeightAtPoint(m_activeUnits[i].Transform.Position);
                if (height != float.MaxValue)
                {
                    Vector3 pos = m_activeUnits[i].Transform.Position;
                    pos.Y = height + m_activeUnits[i].HeightAboveTerrain;
                    m_activeUnits[i].Transform.Position = pos;
                }
            }

            // Update the Units
            foreach (var i in m_activeUnits)
            {
                i.Update(gameTime);
            }

            // Set the camera to chase the first unit if we want to
            m_gameCamera.WorldPositionToChase = m_activeUnits[unitIndex].Transform.WorldMatrix;// *Matrix.CreateTranslation(0, 2, 0);

            //-- Update the Menu
            base.Update(gameTime);
        }

        private void UpdatePaused(GameTime gameTime)
        {
            m_overlay.ApplyEffect(gameTime);
        }

        private void UpdateWaitingForData(GameTime gameTime)
        {
            m_overlay.ApplyEffect(gameTime);

            if (m_networkManager.HostSettings == KillerrinStudiosToolkit.Enumerators.HostType.Host)
            {
                lock (opponentloadedLockObject)
                {
                    // Send Ready to begin
                    if (opponentFullyLoaded)
                    {
                        SystemPacket sp = new SystemPacket(true, SystemPacketID.GameBegin, "");
                        m_networkManager.SendMessage(sp.ThisToJson());
                        m_pausedState = GamePausedState.Unpaused;
                    }
                }
            }
        }
        #endregion

        #region Draw
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics); }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            if (m_currentLoadingProgress.Progress < 100) { 
                DrawLoadingMenu(gameTime, spriteBatch, graphics);
                base.Draw(gameTime, spriteBatch, graphics); 
                return;
            }

            // Draw The Map
            m_map.Draw(gameTime, spriteBatch, graphics, m_gameCamera);
            foreach (var i in m_activeUnits)
            {
                i.Draw(gameTime, spriteBatch, graphics, m_gameCamera);
            }

            // Draw Player Economy
            int distanceBetweenElements = 160;
            int xOffset = (screenRect.Width) - distanceBetweenElements;
            int yOffset = 25;

            spriteBatch.Begin();
            spriteBatch.Draw(m_unitCapTexture, new Rectangle(xOffset, yOffset, 50, 50), Color.White);
            spriteBatch.DrawString(m_empiresOfTheIVFontSmall, m_pageParameter.me.Economy.UnitCap.CurrentAmountAsString, new Vector2(xOffset + 50, yOffset), Color.White);
            xOffset -= distanceBetweenElements;

            spriteBatch.Draw(m_energyTexture, new Rectangle(xOffset, yOffset, 50, 50), Color.White);
            spriteBatch.DrawString(m_empiresOfTheIVFontSmall, m_pageParameter.me.Economy.Energy.CurrentAmountAsString, new Vector2(xOffset + 50, yOffset), Color.White);
            xOffset -= distanceBetweenElements;

            spriteBatch.Draw(m_metalTexture, new Rectangle(xOffset, yOffset, 50, 50), Color.White);
            spriteBatch.DrawString(m_empiresOfTheIVFontSmall, m_pageParameter.me.Economy.Metal.CurrentAmountAsString, new Vector2(xOffset + 50, yOffset), Color.White);
            xOffset -= distanceBetweenElements;

            spriteBatch.Draw(m_currencyTexture, new Rectangle(xOffset, yOffset, 50, 50), Color.White);
            spriteBatch.DrawString(m_empiresOfTheIVFontSmall, m_pageParameter.me.Economy.Currency.CurrentAmountAsString, new Vector2(xOffset + 50, yOffset), Color.White);
            spriteBatch.End();

            switch (m_pausedState)
            {
                case GamePausedState.Paused:            DrawPaused(gameTime, spriteBatch, graphics);           break;                             
                case GamePausedState.WaitingForData:    DrawWaitingForData(gameTime, spriteBatch, graphics);   break;
            }

            base.Draw(gameTime, spriteBatch, graphics);
        }

        public void DrawLoadingMenu(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            //graphics.Clear(Color.Black);
            
            if (m_loadingMiniMap != null)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(m_loadingMiniMap,
                                new Rectangle(0, 0, AnarianConsts.ScreenRectangle.Width, (int)(AnarianConsts.ScreenRectangle.Height * 0.80)),
                                Color.White);
                spriteBatch.End();
            }

            // Loading Outline
            var outlineRect = new Rectangle(0,
                                      (int)(screenRect.Height * 0.75),
                                            screenRect.Width,
                                      (int)(screenRect.Height * 0.10)); 
            PrimitiveHelper2D.DrawRect(spriteBatch, Color.Wheat, outlineRect);

            // Loading Bar
            lock (loadinglockObject)
            {
                int distanceFromTopBottom = 5;
                int distanceFromLeftRight = 0;
                var loadingBar = new Rectangle(0 + distanceFromLeftRight,
                                               outlineRect.Y + distanceFromTopBottom,
                                               m_currentLoadingProgress.Progress * ((outlineRect.Width - (distanceFromLeftRight * 2)) / 100),
                                               outlineRect.Height - (distanceFromTopBottom * 2));
                PrimitiveHelper2D.DrawRect(spriteBatch, Color.ForestGreen, loadingBar);

                // Text
                Vector2 loadingSize = m_empiresOfTheIVFont.MeasureString("Loading");

                spriteBatch.Begin();
                spriteBatch.DrawString(m_empiresOfTheIVFont, "Loading", new Vector2(centerOfScreen.X - (loadingSize.X * 0.3f), screenRect.Height * 0.15f), Color.Wheat);

                spriteBatch.DrawString(m_empiresOfTheIVFont, m_currentLoadingProgress.Status, new Vector2(25, outlineRect.Y - 50), Color.Wheat);
                spriteBatch.DrawString(m_empiresOfTheIVFont, m_currentLoadingProgress.Progress + "%", new Vector2(outlineRect.Width - 100, outlineRect.Y - 50), Color.Wheat);
                spriteBatch.End();
            }
        }

        public void DrawPaused(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            m_overlay.Draw(gameTime, spriteBatch);
        }

        public void DrawWaitingForData(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            m_overlay.Draw(gameTime, spriteBatch);

            Vector2 waitingTextSize = m_empiresOfTheIVFont.MeasureString("Waiting for Data");

            spriteBatch.Begin();
            spriteBatch.DrawString(m_empiresOfTheIVFont, "Waiting for Data", new Vector2(centerOfScreen.X - (waitingTextSize.X * 0.3f), screenRect.Height * 0.15f), Color.Wheat);
            spriteBatch.End();
        }
        #endregion
    }
}
