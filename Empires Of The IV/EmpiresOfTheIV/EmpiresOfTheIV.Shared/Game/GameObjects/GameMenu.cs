using Anarian.DataStructures;
using Anarian.GUI;
using Anarian.Interfaces;
using EmpiresOfTheIV.Game.Enumerators;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.GameObjects
{
    public class GameMenu : Menu,
                            IUpdatable, IRenderable
    {
        #region Fields/Properties
        GameState m_gameState;
        public GameState State { get { return m_gameState; } protected set { m_gameState = value; } }
        #endregion

        public GameMenu(GameState gameState)
            :base()
        {
            m_gameState = gameState;
        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Camera camera) { Draw(gameTime, spriteBatch, graphicsDevice); }
        #endregion

        public void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            base.Draw(gameTime, spriteBatch, graphics);
        }
    }
}
