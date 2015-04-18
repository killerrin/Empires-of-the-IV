using Anarian.DataStructures;
using Anarian.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.GameModels
{
    public abstract class AGameModel : IUpdatable, IRenderable
    {
        public ChatManager GameChatManager { get; protected set; }

        protected AGameModel()
        {
            GameChatManager = new ChatManager();
        }


        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        public abstract void Update(GameTime gameTime);

        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera);

    }
}
