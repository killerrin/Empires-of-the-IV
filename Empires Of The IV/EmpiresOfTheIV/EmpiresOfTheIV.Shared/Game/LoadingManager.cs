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
    public class LoadingManager
    {
        protected EmpiresOfTheIVGame m_game;
        public EmpiresOfTheIVGame Game { get { return m_game; } protected set { m_game = value; } }

        #region Fields/Properties
        bool m_currentlyLoading;
        public bool CurrentlyLoading { get { return m_currentlyLoading; } protected set { m_currentlyLoading = value; } }

        float m_loadingPercentage;
        public float LoadingPercentage { get { return m_loadingPercentage; } protected set { m_loadingPercentage = value; } }

        private object m_loadingLock = new object();
        #endregion

        public LoadingManager(EmpiresOfTheIVGame game)
        {
            m_game = game;

            ResetLoad();
        }

        public void ResetLoad()
        {
            m_loadingPercentage = 0.0f;
            m_currentlyLoading = true;
        }

        public void LoadContent()
        {

        }
    }
}
