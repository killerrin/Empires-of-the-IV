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
        protected EmpiresOfTheIVGame m_game;
        public EmpiresOfTheIVGame Game { get { return m_game; } protected set { m_game = value; } }

        protected GameState m_gameState;
        public GameState State { get { return m_gameState; } protected set { m_gameState = value; } }

        protected Level m_level;
        public Level Level { get { return m_level; } protected set { m_level = value; } }
        #endregion

        public GameMenu(EmpiresOfTheIVGame game, object pageParameter, GameState gameState)
            : base(pageParameter)
        {
            m_game = game;
            m_gameState = gameState;
            
            m_level = new Level(m_game.GraphicsDevice);
        }

        public virtual void MenuLoaded()
        {
            base.MenuLoaded();

            m_level.LevelLoaded();
        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, ICamera camera) { Draw(gameTime, spriteBatch, graphicsDevice); }
        #endregion

        public virtual void Update(GameTime gameTime)
        {
            // Update the 3D Level
            m_level.Update(gameTime);

            // Update the 2D Menu
            base.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics)
        {
            // Draw the 3D Level
            m_level.Draw(gameTime, spriteBatch, graphics);

            // Draw the 2D Menu
            base.Draw(gameTime, spriteBatch, graphics);
        }
    }
}
