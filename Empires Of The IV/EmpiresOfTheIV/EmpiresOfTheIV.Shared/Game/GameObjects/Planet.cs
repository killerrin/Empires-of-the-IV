using Anarian.DataStructures;
using Anarian.DataStructures.Components;
using Anarian.DataStructures.Rendering;
using Anarian.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.GameObjects
{
    public class Planet : Sphere,
                          IUpdatable, IRenderable
    {
        #region Fields/Properties

        #endregion

        public Planet(GraphicsDevice graphicsDevice, Texture2D planetTexture, float radius)
            :base(graphicsDevice, planetTexture, radius)
        {

        }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
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
