using Anarian.DataStructures;
using EmpiresOfTheIV.Game.Enumerators;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Players
{
    public class AIPlayer : APlayer
    {
        public AIPlayer(Team team, int playerID, string playerName)
            :base(team, playerID, "AI" + playerName + playerID)
        {

        }


        public override void Update(GameTime gameTime)
        {

        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera)
        {

        }
    }
}
