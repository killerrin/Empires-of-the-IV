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
    public class Player : IUpdatable, IRenderable
    {
        public int PlayerID { get; set; }
        public string PlayerName { get; set; }

        public PlayerType PlayerType { get; set; }

        //public Economy PlayerEconomy { get; protected set; }


        public Player(uint playerID, string playerName, PlayerType playerType)
        {
            PlayerID = Convert.ToInt32(playerID);
            PlayerName = playerName;

            //PlayerEconomy = new Economy(PlayerID);
            PlayerType = playerType;
        }

        public static Player HumanPlayer(uint playerID, string playerName)
        {
            return new Player(playerID, playerName, PlayerType.Human);
        }
        public static Player AIPlayer(uint playerID, string playerName)
        {
            return new Player(playerID, "AI" + playerName + playerID, PlayerType.AI);
        }

        #region Interface Implimentation
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
        #endregion

        public virtual void Update(GameTime gameTime) {
            //PlayerEconomy.Update(gameTime);
        }
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera)
        {

        }

    }
}
