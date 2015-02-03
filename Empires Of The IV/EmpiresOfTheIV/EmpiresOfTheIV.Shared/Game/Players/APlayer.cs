using Anarian.Interfaces;
using EmpiresOfTheIV.Game.Enumerators;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Players
{
    public abstract class APlayer : IUpdatable
    {
        public Team TeamID { get; set; }
        public int PlayerID { get; protected set; }
        public string PlayerName { get; protected set; }

        protected APlayer(Team team, int playerID, string playerName)
        {
            TeamID = team;
            PlayerID = playerID;
            PlayerName = playerName;
        }

        #region Interface Implimentation
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        #endregion

        public abstract void Update(GameTime gameTime);

    }
}
