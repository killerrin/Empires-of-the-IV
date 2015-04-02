using Anarian.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.GameObjects
{
    public class GUIButton : IUpdatable, IRenderable
    {
        public bool Active;
        public bool Visible;

        public Texture2D Texture { get; set; }
        public Rectangle Position { get; set; }
        public Color Colour { get; set; }

        public GUIButton(Texture2D texture, Vector2 position, Color color)
        {
            Texture = texture;
            Position = new Rectangle((int)position.X, (int)position.Y, Texture.Width, Texture.Height);
            Colour = color;

            Reset();
        }
        public GUIButton(Texture2D texture, Rectangle position, Color color)
        {
            Texture = texture;
            Position = position;
            Colour = color;

            Reset();
        }

        public void Reset()
        {
            Active = true;
            Visible = true;
        }

        public bool Intersects(Vector2 position)
        {
            return Position.Contains(position);
        }

        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        public void Update(GameTime gameTime)
        {
            if (!Active) return;
        }

        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
        {
            if (!Active) return;
            if (!Visible) return;

            spriteBatch.Begin();
            spriteBatch.Draw(Texture, Position, Colour);
            spriteBatch.End();
        }
}
}
