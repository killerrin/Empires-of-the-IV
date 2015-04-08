using Anarian.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Game_Tools
{
    public class SelectionManager :IRenderable
    {
        public Texture2D SelectionTexture;

        public Vector2? StartingPosition;
        public Vector2? EndingPosition;

        public Rectangle PreviousSelection;


        public Point MinimumWidthHeightToIssueCommand;
        public Rectangle MinimumSelection
        {
            get
            {
                if (StartingPosition.HasValue)
                {
                    return new Rectangle((int)(StartingPosition.Value.X) - MinimumWidthHeightToIssueCommand.X,
                                         (int)(StartingPosition.Value.Y) - MinimumWidthHeightToIssueCommand.Y,
                                         MinimumWidthHeightToIssueCommand.X * 2,
                                         MinimumWidthHeightToIssueCommand.Y * 2);
                }

                return Rectangle.Empty;
            }
        }


        public bool HasSelection
        {
            get { return (StartingPosition.HasValue && EndingPosition.HasValue); }
        }

        public SelectionManager()
        {
            SelectionTexture = null;
            MinimumWidthHeightToIssueCommand = new Point(25, 25);

            Deselect();
        }

        public void Deselect()
        {
            PreviousSelection = GetSelection();

            StartingPosition = null;
            EndingPosition = null;
        }

        public Rectangle GetSelection()
        {
            if (!StartingPosition.HasValue ||
                !EndingPosition.HasValue)
            {
                return Rectangle.Empty;
            }

            // To stop from selecting all units in a single click
            // We simply set the EndingPosition to not be the same
            // as the StartingPosition
            if (StartingPosition.Value == EndingPosition.Value)
                EndingPosition = StartingPosition + new Vector2(1.0f, 1.0f);

            return new Rectangle((int)(StartingPosition.Value.X),
                                 (int)(StartingPosition.Value.Y),
                                 (int)(EndingPosition.Value.X - StartingPosition.Value.X),
                                 (int)(EndingPosition.Value.Y - StartingPosition.Value.Y));
        }

        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
        {
            if (SelectionTexture == null || !HasSelection) 
                return;

            spriteBatch.Begin();
            spriteBatch.Draw(SelectionTexture, GetSelection(), Color.Black);
            spriteBatch.End();
        }
   
    }
}
