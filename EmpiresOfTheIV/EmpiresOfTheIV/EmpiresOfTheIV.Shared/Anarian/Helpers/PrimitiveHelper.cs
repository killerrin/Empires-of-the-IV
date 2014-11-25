using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Anarian;
using Anarian.DataStructures;
using Anarian.Interfaces;

namespace Anarian.Helpers
{
    public static class PrimitiveHelper2D
    {
        private static Rectangle GetCenterOfPoint(int size, Vector2 position)
        {
            Rectangle rect = new Rectangle();
            rect.X = (int)(position.X - (size / 2.0f));
            rect.Y = (int)(position.Y - (size / 2.0f));
            rect.Width = size;
            rect.Height = size;
            return rect;
        }

        /// <summary>
        /// Draws Square Points on the screen at the specified Position, Size and Color
        /// </summary>
        /// <param name="spriteBatch">The Spritebatch</param>
        /// <param name="size">The Size of the point</param>
        /// <param name="color">The color of the point</param>
        /// <param name="param">Vector2s representing position on the screen</param>
        public static void DrawPoints(SpriteBatch spriteBatch, Color color, int size, params Vector2[] param)
        {
            Texture2D blankTex = ResourceManager.Instance.GetTexture("blankTexture_age");
            
            spriteBatch.Begin();
            for (int i = 0; i < param.Length; i++)
            {
                Rectangle rect = GetCenterOfPoint(size, param[i]);


                spriteBatch.Draw(blankTex, rect, color);
            }
            spriteBatch.End();
        }

        /// <summary>
        /// Draws Square Points on the screen at the specified Position, Size and Color
        /// </summary>
        /// <param name="spriteBatch">The Spritebatch</param>
        /// <param name="size">The Size of the point</param>
        /// <param name="color">The color of the point</param>
        /// <param name="param">Vector2s representing position on the screen</param>
        public static void DrawLines(SpriteBatch spriteBatch, Color color, int size, Vector2 p1, Vector2 p2, params Vector2[] param)
        {
            Texture2D blankTex = ResourceManager.Instance.GetTexture("blankTexture_age");

            //// Update the line positions to be in the center of their size
            //p1.X -= (size / 2.0f);
            //p1.Y -= (size / 2.0f);
            //p2.X -= (size / 2.0f);
            //p2.Y -= (size / 2.0f);

            // Draw the Line
            spriteBatch.Begin();

            // Draw all the points inbetween
            Rectangle r = new Rectangle((int)p1.X, (int)p1.Y, (int)(p2 - p1).Length() + size, size);
            Vector2 v = Vector2.Normalize(p1 - p2);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (p1.Y > p2.Y) angle = MathHelper.TwoPi - angle;
            spriteBatch.Draw(blankTex, r, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);

            spriteBatch.End();

            // Recursively Draw Lines until there are no more left
            if (param.Length > 0) {
                List<Vector2> leftoverParams = new List<Vector2>();
                for (int i = 1; i < param.Length; i++) {
                    leftoverParams.Add(param[i]);
                }

                DrawLines(spriteBatch, color, size, p2, param[0], leftoverParams.ToArray());
            }
        }
    }
}
