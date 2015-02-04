using Anarian.DataStructures;
using Anarian.Interfaces;
using EmpiresOfTheIV.Game.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.GameModels
{
    public class GameModel1v1 : AGameModel
    {
        public Team Team1 { get; protected set; }
        public Team Team2 { get; protected set; }

        public GameModel1v1()
            :base()
        {
            Team1 = new Team(TeamID.TeamOne);
            Team2 = new Team(TeamID.TeamTwo);
        }

        public override void Update(GameTime gameTime)
        {
            Team1.Update(gameTime);
            Team2.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera)
        {
            Team1.Draw(gameTime, spriteBatch, graphics, camera);
            Team2.Draw(gameTime, spriteBatch, graphics, camera);
        }
    }
}
