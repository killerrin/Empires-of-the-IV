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

        public Point MinimumWidthHeightToIssueCommand;
        public Point CurrentWidthHeight;

        public Rectangle PreviousSelection;
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

        public bool HasSelection { get { return (StartingPosition.HasValue && EndingPosition.HasValue); } }

        public SelectionManager()
        {
            SelectionTexture = null;
            MinimumWidthHeightToIssueCommand = new Point(10, 10);

            Deselect();
        }

        public void Deselect()
        {
            PreviousSelection = GetSelection();

            CurrentWidthHeight = Point.Zero;
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

            Vector2 startingPos = new Vector2();
            Vector2 endingPos = new Vector2();
            if (StartingPosition.Value.X < EndingPosition.Value.X)
            {
                startingPos.X = StartingPosition.Value.X;
                endingPos.X = EndingPosition.Value.X;
            }
            else
            {
                startingPos.X = EndingPosition.Value.X;
                endingPos.X = StartingPosition.Value.X;
            }

            if (StartingPosition.Value.Y < EndingPosition.Value.Y)
            {
                startingPos.Y = StartingPosition.Value.Y;
                endingPos.Y = EndingPosition.Value.Y;
            }
            else
            {
                startingPos.Y = EndingPosition.Value.Y;
                endingPos.Y = StartingPosition.Value.Y;
            }


            CurrentWidthHeight = new Point((int)(endingPos.X - startingPos.X),
                                           (int)(endingPos.Y - startingPos.Y));

            return new Rectangle((int)(startingPos.X),
                                 (int)(startingPos.Y),
                                 CurrentWidthHeight.X,
                                 CurrentWidthHeight.Y);
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
