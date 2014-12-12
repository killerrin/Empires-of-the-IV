using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Anarian.GUI
{
    public class Label : Element
    {
        public string Text { get; set; }
        public SpriteFont Font { get; set; }
        public Vector2 StringSize { get; protected set; }

        public Label(SpriteFont font, string text, Vector2 position, Color color)
            :base(null, position, color, Vector2.One, Vector2.Zero )
        {
            Font = font;
            Text = text;

            StringSize = font.MeasureString(text);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Font == null || string.IsNullOrEmpty(Text)) return;

            spriteBatch.Begin();
            spriteBatch.DrawString(Font, Text, Position, Colour);
            spriteBatch.End();
        }
    }
}
