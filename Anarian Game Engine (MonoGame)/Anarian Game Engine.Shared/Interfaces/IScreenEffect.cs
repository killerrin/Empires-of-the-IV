﻿using Anarian.DataStructures;
using Anarian.Enumerators;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anarian.Interfaces
{
    public interface IScreenEffect : IRenderable
    {
        ProgressStatus Progress { get; set; }

        void PreformEffect(GameTime gameTime);

        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
}
