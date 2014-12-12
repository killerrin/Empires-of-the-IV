using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Anarian.GUI
{
    public class GuiObject
    {
        public bool Enabled { get; set; }
        public bool Visible { get; set; }

        public GuiObject()
        {
            Enabled = true;
            Visible = true;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!Enabled) return;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible) return;
        }

    }
}
