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
        public uint ID { get; set; }
        public string Name { get; set; }
        public PlayerType PlayerType { get; set; }

        public EmpireType EmpireType { get; set; }

        public Economy Economy { get; protected set; }

        public Player(uint playerID, string playerName, PlayerType playerType)
        {
            ID = playerID;
            Name = playerName;
            PlayerType = playerType;

            EmpireType = EmpireType.UnanianEmpire;

            Economy = new Economy(ID);
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
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
        #endregion

        public override string ToString()
        {
            return String.Format("ID: {0} | Name: {1} | PlayerType: {2}", ID.ToString(), Name, PlayerType.ToString());
        }

        public virtual void Update(GameTime gameTime) {
            Economy.Update(gameTime);
        }
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
        {

        }

    }
}
