using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Anarian.Enumerators;

namespace Anarian.GUI
{
    public class Button : Element
    {
        public Texture2D DownTexture { get; set; }
        public Texture2D HoverTexture { get; set; }
        public Texture2D DisabledTexture { get; set; }

        public Label Label { get; set; }


        public Button(Texture2D _texture, Vector2 _position, Color _color, Vector2 _scale, Vector2 _origin,
            float _rotation = 0.0f,
            Rectangle? _sourceRectangle = null,
            SpriteEffects _spriteEffects = SpriteEffects.None,
            float _depth = 0.0f
            )
            : base(_texture, _position, _color, _scale, _origin, _rotation, _sourceRectangle, _spriteEffects, _depth)
        {
            m_guiState = GuiState.None;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw the Element
            base.Draw(spriteBatch);

            // Draw the Label
            Label.Draw(spriteBatch);
        }


    }
}
