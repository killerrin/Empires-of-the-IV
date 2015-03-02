using Anarian.DataStructures;
using Anarian.GUI;
using Anarian.Interfaces;
using EmpiresOfTheIV.Game.Enumerators;
using EmpiresOfTheIV.Game.GameObjects;
using EmpiresOfTheIV.Game.Menus.PageParameters;
using EmpiresOfTheIV.Game.Players;
using KillerrinStudiosToolkit.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Menus
{
    public class InGameMenu : GameMenu,
                              IUpdatable, IRenderable
    {
        private static bool m_loadedOnceAlready = false;
        #region Fields/Properties
        NetworkManager m_networkManager;
        PausedState m_pausedState;

        InGamePageParameter m_pageParameter;

        Team m_team1;
        Team m_team2;

        ChatManager m_chatManager;
        #endregion

        public InGameMenu(EmpiresOfTheIVGame game, object parameter)
            :base(game, parameter, GameState.InGame)
        {
            m_networkManager = m_game.GameManager.NetworkManager;
            m_pausedState = PausedState.Unpaused;

            m_networkManager.OnConnected += NetworkManager_OnConnected;
            m_networkManager.OnDisconnected += NetworkManager_OnDisconnected;
            m_networkManager.OnMessageRecieved += NetworkManager_OnMessageRecieved;

            Consts.Game.GameManager.StateManager.OnBackButtonPressed += StateManager_OnBackButtonPressed;
            Consts.Game.GameManager.StateManager.HandleBackButtonPressed = false;

            // Save the parameters
            m_pageParameter = (InGamePageParameter)parameter;
            m_team1 = m_pageParameter.team1;
            m_team2 = m_pageParameter.team2;

            m_chatManager = m_pageParameter.chatManager;
        }

        private void StateManager_OnBackButtonPressed(object sender, EventArgs e)
        {

        }

        public override void MenuLoaded()
        {
            base.MenuLoaded();

            if (!m_loadedOnceAlready)
            {

                m_loadedOnceAlready = true;
            }

            //if (NavigationSaveState == Anarian.Enumerators.NavigationSaveState.KeepSate)
            //{
            //}
        }

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

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera) { Draw(gameTime, spriteBatch, graphics); }
        #endregion

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            base.Draw(gameTime, spriteBatch, graphics);
        }
    }
}
