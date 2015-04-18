using Anarian.DataStructures;
using Anarian.DataStructures.ScreenEffects;
using Anarian.GUI;
using Anarian.Interfaces;
using EmpiresOfTheIV.Game.Enumerators;
using EmpiresOfTheIV.Game.GameObjects;
using EmpiresOfTheIV.Game.Networking;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EmpiresOfTheIV.Game.Menus
{
    public class GameLobbyMenu : GameMenu,
                                 IUpdatable, IRenderable
    {

        #region Fields/Properties
        GameConnectionType m_gameConnectionType;
        public GameConnectionType GameConnectionType { get { return m_gameConnectionType; } }

        Overlay m_overlay;
        Timer m_networkTimer;
        #endregion

        public GameLobbyMenu(EmpiresOfTheIVGame game, object parameter, GameConnectionType gameConnectionType)
            : base(game, parameter, GameState.GameLobby)
        {
            m_gameConnectionType = gameConnectionType;

            m_overlay = new Overlay(game.GraphicsDevice, Color.Black);
            m_overlay.FadePercentage = 0.5f;

            m_networkTimer = new Timer(m_game.NetworkManager.ConnectionPreventTimeoutTick);
            m_networkTimer.Completed += m_networkTimer_Completed;
        }

        public override void MenuLoaded()
        {
            Debug.WriteLine("GameLobbyMenu Loaded");

            base.MenuLoaded();

            if (NavigationSaveState == Anarian.Enumerators.NavigationSaveState.RecreateState) {
                m_overlay = new Overlay(m_game.GraphicsDevice, Color.Black);
                m_overlay.FadePercentage = 0.5f;
            }
        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics); }
        #endregion

        public override void Update(GameTime gameTime)
        {
            m_overlay.ApplyEffect(gameTime);
            m_networkTimer.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            base.Draw(gameTime, spriteBatch, graphics);
            m_overlay.Draw(gameTime, spriteBatch);
        }

        void m_networkTimer_Completed(object sender, EventArgs e)
        {
            if (m_game.NetworkManager.HostSettings == KillerrinStudiosToolkit.Enumerators.HostType.Host)
            {
                Debug.WriteLine("Sending Connection Tick");

                // Send an ACK to keep the connection opened
                SystemPacket sp = new SystemPacket(true, SystemPacketID.ConnectionTick, "");
                m_game.NetworkManager.SendMessage(sp.ThisToJson());

            }

            // Reset the timer
            m_networkTimer.Reset();
        }
    }
}
