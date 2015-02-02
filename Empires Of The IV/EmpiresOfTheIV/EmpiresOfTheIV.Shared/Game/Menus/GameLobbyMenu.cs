using Anarian.DataStructures;
using Anarian.DataStructures.ScreenEffects;
using Anarian.GUI;
using Anarian.Interfaces;
using EmpiresOfTheIV.Game.Enumerators;
using EmpiresOfTheIV.Game.GameObjects;
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
        #endregion

        public GameLobbyMenu(EmpiresOfTheIVGame game, GameConnectionType gameConnectionType)
            : base(game, GameState.GameLobby)
        {
            m_gameConnectionType = gameConnectionType;

            m_overlay = new Overlay(game.GraphicsDevice, Color.Black);
            m_overlay.FadePercentage = 0.5f;
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
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera) { Draw(gameTime, spriteBatch, graphics); }
        #endregion

        public override void Update(GameTime gameTime)
        {
            m_overlay.ApplyEffect(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            base.Draw(gameTime, spriteBatch, graphics);
            m_overlay.Draw(gameTime, spriteBatch);
        }
    }
}
