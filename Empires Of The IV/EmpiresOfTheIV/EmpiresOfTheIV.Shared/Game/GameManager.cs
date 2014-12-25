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

namespace EmpiresOfTheIV.Game
{
    public class GameManager
    {
        protected EmpiresOfTheIVGame m_game;
        public EmpiresOfTheIVGame Game { get { return m_game; } protected set { m_game = value; } }
        
        #region Fields/Properties
        protected LoadingManager m_loadingManager;
        public LoadingManager LoadingManager { get { return m_loadingManager; } set { m_loadingManager = value; } }
        #endregion

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
        }

        public void LoadContent() { m_loadingManager.LoadContent(); }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime)
        {

        }
    }
}
