using Anarian.DataStructures;
using Anarian.DataStructures.Components;
using Anarian.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.GameObjects
{
    public class Planet : StaticGameObject,
                            IUpdatable, IRenderable, ISelectableEntity
    {
        #region Fields/Properties
        bool m_selectable;
        public bool Selectable
        {
            get { return m_selectable; }
            set { m_selectable = value; }
        }

        bool m_selected;
        public bool Selected
        {
            get { return m_selected; }
            set { m_selected = value; }
        }
        #endregion

        public Planet()
            :base()
        {
            Selectable = true;
            Selected = false;
        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera) { Draw(gameTime, spriteBatch, graphics, camera); }

        bool ISelectableEntity.Selectable
        {
            get { return Selectable; }
            set { Selectable = value; }
        }

        bool ISelectableEntity.Selected
        {
            get { return Selected; }
            set { Selected = value; }
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera)
        {
            base.Draw(gameTime, spriteBatch, graphics, camera);
        }
    }
}
