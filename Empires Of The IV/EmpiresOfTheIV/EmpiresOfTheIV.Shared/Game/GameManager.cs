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
    public class GameManager
    {
        protected EmpiresOfTheIVGame m_game;
        public EmpiresOfTheIVGame Game { get { return m_game; } protected set { m_game = value; } }
        
        #region Fields/Properties
        #region Managers
        protected LoadingManager m_loadingManager;
        public LoadingManager LoadingManager { get { return m_loadingManager; } set { m_loadingManager = value; } }

        protected GameInputManager m_gameInputManager;
        public GameInputManager GameInputManager { get { return m_gameInputManager; } set { m_gameInputManager = value; } }
        #endregion

        protected GameState m_gameState;
        public GameState GameState { get { return m_gameState; } protected set { m_gameState = value; } }
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
            m_loadingManager = new LoadingManager(m_game);
            m_gameInputManager = new GameInputManager(m_game);
        }

        public void LoadContent() { m_loadingManager.LoadContent(); }

        public void Update(GameTime gameTime)
        {
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

        public void Draw(GameTime gameTime)
        {
            // Draw the Rays
            if (m_gameInputManager.currentRay.HasValue) {
                m_gameInputManager.currentRay.Value.DrawRay(m_game.Graphics, Color.Red, m_game.SceneManager.CurrentScene.Camera, Matrix.Identity);
            }
        }
    }
}
