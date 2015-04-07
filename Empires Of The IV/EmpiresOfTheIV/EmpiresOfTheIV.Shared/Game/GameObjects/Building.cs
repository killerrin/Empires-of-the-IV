using Anarian;
using Anarian.DataStructures;
using Anarian.DataStructures.Components;
using Anarian.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EmpiresOfTheIV.Game.GameObjects
{
    public class Building : StaticGameObject,
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

        public Building()
            :base()
        {
            Selectable = true;
            Selected = false;
        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics, camera); }

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
            //Debug.WriteLine("Building");
            base.Update(gameTime);
        }

        public virtual bool Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera, bool creatingShadowMap = false)
        {
            return base.Draw(gameTime, spriteBatch, graphics, camera);;
        }

        protected override void SetupEffects(Effect effect, GraphicsDevice graphics, ICamera camera, GameTime gameTime)
        {
            base.SetupEffects(effect, graphics, camera, gameTime);
        }
    }
}
