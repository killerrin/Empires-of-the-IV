﻿using Anarian.DataStructures;
using Anarian.GUI;
using Anarian.Interfaces;
using EmpiresOfTheIV.Game.Enumerators;
using EmpiresOfTheIV.Game.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Menus
{
    public class GameOverMenu : GameMenu,
                                IUpdatable, IRenderable
    {

        #region Fields/Properties

        #endregion

        public GameOverMenu(EmpiresOfTheIVGame game, object parameter)
            :base(game, parameter, GameState.GameOver)
        {
        }

        public override void MenuLoaded()
        {
            base.MenuLoaded();

            // Turn off the Unified Menu
            m_game.StateManager.HandleBackButtonPressed = true;
            m_game.SceneManager.Active = true;
        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics); }
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
