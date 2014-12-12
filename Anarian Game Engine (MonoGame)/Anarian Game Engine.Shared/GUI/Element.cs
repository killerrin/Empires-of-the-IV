using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Anarian.Enumerators;

namespace Anarian.GUI
{
    public class Element : GuiObject
    {
        public GuiObject m_parent;
        public GuiObject Parent { 
            get { return m_parent; }
            protected set { m_parent = value; }
        }

        protected GuiState m_guiState;

        #region Monogame SpriteBatch 
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Rectangle? SourceRectangle { get; set; }
        public Color Colour { get; set; }
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }
        public Vector2 Scale { get; set; }
        public SpriteEffects SpriteEffect { get; set; }
        public float Depth { get; set; }
        #endregion

        public Element(Texture2D _texture, Vector2 _position, Color _color, Vector2 _scale, Vector2 _origin,
            float _rotation = 0.0f,
            Rectangle? _sourceRectangle = null,
            SpriteEffects _spriteEffects = SpriteEffects.None,
            float _depth = 0.0f
            )
            :base()
        {
            Texture = _texture;
            Position = _position;
            SourceRectangle = _sourceRectangle;
            Colour = _color;
            Rotation = _rotation;
            Origin = _origin;
            Scale = _scale;
            SpriteEffect = _spriteEffects;
            Depth = _depth;

            m_guiState = GuiState.None;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled) return;

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible) return;
            if (Texture == null) return;

            spriteBatch.Begin();
            spriteBatch.Draw(Texture, Position, SourceRectangle, Colour, Rotation, Origin, Scale, SpriteEffect, Depth);
            spriteBatch.End();

            base.Draw(spriteBatch);
        }
    }
}
