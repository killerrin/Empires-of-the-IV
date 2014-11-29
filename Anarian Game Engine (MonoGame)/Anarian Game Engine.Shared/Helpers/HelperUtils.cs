using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using Anarian;
using Anarian.DataStructures;

namespace Anarian.Helpers
{
    public static class HelperUtils
    {
        public static Vector2 GetViewportCenter(Viewport viewport)
        {
            return new Vector2(viewport.X + viewport.Width / 2, viewport.Y + viewport.Height / 2);
        }

        public static Rectangle GetViewportRectangle(Viewport viewport)
        {
            return new Rectangle(0, 0, viewport.Width, viewport.Height);
        }
    }
}
