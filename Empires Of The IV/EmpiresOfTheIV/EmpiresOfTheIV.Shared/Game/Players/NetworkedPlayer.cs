using EmpiresOfTheIV.Game.Enumerators;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Players
{
    public class NetworkedPlayer : APlayer
    {
        public NetworkedPlayer(Team team, int playerID, string playerName)
            : base(team, playerID, playerName)
        {
        }


        public override void Update(GameTime gameTime)
        {

        }
    }
}
