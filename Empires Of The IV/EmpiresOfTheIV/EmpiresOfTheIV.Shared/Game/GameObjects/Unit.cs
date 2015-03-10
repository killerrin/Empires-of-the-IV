using Anarian.DataStructures;
using Anarian.DataStructures.Components;
using Anarian.Interfaces;
using EmpiresOfTheIV.Game.Enumerators;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.GameObjects
{
    public class Unit : AnimatedGameObject,
                        IUpdatable, IRenderable, ISelectableEntity
    {
        #region Fields/Properties
        public uint UnitID { get; protected set; }
        public UnitType UnitType;

        public bool Selectable { get; set; }
        public bool Selected { get; set; }

        public Health Health { get { return GetComponent(typeof(Health)) as Health; } }
        public Mana Mana { get { return GetComponent(typeof(Mana)) as Mana; } }
        #endregion

        public Unit(uint unitID, UnitType unitType)
            : base()
        {
            UnitID = unitID;
            UnitType = unitType;

            // Setup base Selection rules
            Selectable = true;
            Selected = false;

            // Add Unit Specific Components
            AddComponent(typeof(Health));
            AddComponent(typeof(Mana));
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
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
        {
            base.Draw(gameTime, spriteBatch, graphics, camera);
        }

        protected override void SetupEffects(Effect effect, GraphicsDevice graphics, ICamera camera, GameTime gameTime)
        {
            base.SetupEffects(effect, graphics, camera, gameTime);
        }
    }
}
