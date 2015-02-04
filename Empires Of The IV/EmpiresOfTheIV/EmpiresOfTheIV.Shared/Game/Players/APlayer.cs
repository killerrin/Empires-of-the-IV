using Anarian.DataStructures;
using Anarian.Interfaces;
using EmpiresOfTheIV.Game.Enumerators;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Players
{
    public abstract class APlayer : IUpdatable, IRenderable
    {
        public Team TeamID { get; set; }
        public int PlayerID { get; protected set; }
        public string PlayerName { get; protected set; }

        public Economy Economy { get; protected set; }


        protected APlayer(Team team, int playerID, string playerName)
        {
            TeamID = team;
            PlayerID = playerID;
            PlayerName = playerName;

            Economy = new Economy(this);
        }

        #region Interface Implimentation
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
        #endregion

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera);

    }
}
