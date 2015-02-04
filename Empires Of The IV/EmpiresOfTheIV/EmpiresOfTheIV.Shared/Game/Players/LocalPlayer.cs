﻿using Anarian.DataStructures;
using EmpiresOfTheIV.Game.Enumerators;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Players
{
    public class LocalPlayer : APlayer
    {
        public LocalPlayer(Team team, int playerID, string playerName)
            : base(team, playerID, playerName)
        {

        }


        public override void Update(GameTime gameTime)
        {
            Economy.Update(gameTime);
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera)
        {

        }
    }
}
