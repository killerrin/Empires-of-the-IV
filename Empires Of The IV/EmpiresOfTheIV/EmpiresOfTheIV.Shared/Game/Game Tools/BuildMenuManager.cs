using Anarian.DataStructures;
using Anarian.Interfaces;
using EmpiresOfTheIV.Game.GameObjects;
using EmpiresOfTheIV.Game.GameObjects.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Game_Tools
{
    public class BuildMenuManager : IUpdatable, IRenderable
    {
        bool Active;

        public SpriteFont SpriteFont;
        public Texture2D MenuTexture;

        public FactoryBase m_activeFactory;
        public BuildMenuType m_buildMenuType;

        #region GameObjects
        public Unit m_unit1;
        public Unit m_unit2;

        public Factory m_factory;
        #endregion

        public BuildMenuManager(SpriteFont font, Texture2D menuTexture)
        {
            Active = true;

            SpriteFont = font;
            MenuTexture = menuTexture;
        }

        public void Enable(FactoryBase factoryBase, BuildMenuType buildMenuType)
        {
            Active = true;
            m_activeFactory = factoryBase;
            m_buildMenuType = buildMenuType;
        }
        public void Disable()
        {
            Active = false;
            m_activeFactory = null;
            m_buildMenuType = BuildMenuType.None;
        }

        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        public void Update(GameTime gameTime)
        {
            if (!Active) return;
        }

        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
        {
            if (!Active) return;

            spriteBatch.Begin();

            spriteBatch.End();
        }
    }
}
