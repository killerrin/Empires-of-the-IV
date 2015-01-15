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
using EmpiresOfTheIV.Game.Enumerators;

namespace EmpiresOfTheIV.Game
{
    public class GameManager : IUpdatable
    {
        protected EmpiresOfTheIVGame m_game;
        public EmpiresOfTheIVGame Game { get { return m_game; } protected set { m_game = value; } }
        
        #region Fields/Properties
        #region Managers
        protected LoadingManager m_loadingManager;
        public LoadingManager LoadingManager { get { return m_loadingManager; } set { m_loadingManager = value; } }

        protected GameInputManager m_gameInputManager;
        public GameInputManager GameInputManager { get { return m_gameInputManager; } set { m_gameInputManager = value; } }

        protected StateManager m_stateManager;
        public StateManager StateManager { get { return m_stateManager; } set { m_stateManager = value; } }
        #endregion

        #region Menus

        #endregion
        #endregion

        List<ISelectableEntity> selectedEntities = new List<ISelectableEntity>();

        public GameManager(EmpiresOfTheIVGame game)
        {
            m_game = game;
        }

        /// <summary>
        /// Initializes All of the Game Managers
        /// </summary>
        public void Initialize()
        {
            // Load the State Manager
            m_stateManager = new StateManager(m_game, Color.Black);
            m_stateManager.OnExit += OnExitTriggered;

            // Load the Other Managers
            m_loadingManager = new LoadingManager(m_game);
            m_gameInputManager = new GameInputManager(m_game);
        }

        private void OnExitTriggered(object sender, EventArgs e)
        {
            // We can Exit the Game
            Debug.WriteLine("Exit");
            Windows.UI.Xaml.Application.Current.Exit();
        }

        public void LoadContent() { m_loadingManager.LoadContent(m_game.GraphicsDevice); }


        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        public void Update(GameTime gameTime)
        {
            // First, Update the StateManager
            m_stateManager.Update(gameTime);

            switch (m_stateManager.CurrentState) {
                case GameState.SplashScreen:            break;

                case GameState.MainMenu:                break;
                case GameState.Options:                 break;
                case GameState.Credits:                 break;
                case GameState.PlayGame:                break;

                case GameState.BluetoothMultiplayer:    break;
                case GameState.LanMultiplayer:          break;

                case GameState.GameLobby:               break;
                case GameState.EmpireSelection:         break;

                case GameState.InGame:                  break;
                case GameState.Paused:                  break;
                case GameState.GameOver:                break;
                default: break;
            }

            // Begin Regular Updates
            if (m_gameInputManager.rayPosOnTerrain.HasValue) {
                m_game.SceneManager.CurrentScene.SceneNode.GetChild(1).MoveToPosition(gameTime, m_gameInputManager.rayPosOnTerrain.Value);
            }

            float height = ((Terrain)m_game.SceneManager.CurrentScene.SceneNode.GetChild(0).GameObject).GetHeightAtPoint(m_game.SceneManager.CurrentScene.SceneNode.GetChild(1).Position);
            if (height != float.MaxValue) {
                Vector3 pos = m_game.SceneManager.CurrentScene.SceneNode.GetChild(1).Position;
                pos.Y = height;
                m_game.SceneManager.CurrentScene.SceneNode.GetChild(1).Position = pos;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            switch (m_stateManager.CurrentState) {
                case GameState.SplashScreen:            m_game.GraphicsDevice.Clear(Color.Black);   break;

                case GameState.MainMenu:                break;
                case GameState.Options:                 break;
                case GameState.Credits:                 break;
                case GameState.PlayGame:                break;

                case GameState.BluetoothMultiplayer:    break;
                case GameState.LanMultiplayer:          break;

                case GameState.GameLobby:               break;
                case GameState.EmpireSelection:         break;

                case GameState.InGame:                  break;
                case GameState.Paused:                  break;
                case GameState.GameOver:                break;
                default:                                break;
            }

            // Finally, Draw the StateManager over everything else
            m_stateManager.Draw(gameTime, spriteBatch, graphics);
        }
    }
}
