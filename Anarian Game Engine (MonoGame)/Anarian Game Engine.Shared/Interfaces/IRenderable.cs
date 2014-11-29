using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Anarian.DataStructures;

namespace Anarian.Interfaces
{
    public interface IRenderable
    {
        void Draw(GameTime gameTime, Camera camera, GraphicsDeviceManager graphics);
    }
}
