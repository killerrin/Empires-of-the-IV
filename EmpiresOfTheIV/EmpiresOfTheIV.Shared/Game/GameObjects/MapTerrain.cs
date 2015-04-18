using Anarian.DataStructures.Rendering;
using Anarian.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.GameObjects
{
    public class MapTerrain : Terrain
    {

        public MapTerrain(GraphicsDevice graphics, Texture2D heightmap, Texture2D texture = null )
            :base(graphics, heightmap, texture)
        {
        }
        public MapTerrain(GraphicsDevice graphics, int width, int height, Color heightmapGenerationColor, Texture2D texture = null)
            :base(graphics, width, height, heightmapGenerationColor, texture)
        {
        }

        public static MapTerrain CreateFlatTerrain(GraphicsDevice graphics, int width, int height, Texture2D texture = null)
        {
            MapTerrain terrain = new MapTerrain(graphics, width, height, Color.White, texture);
            return terrain;
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public virtual bool Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera, bool creatingShadowMap = false)
        {
            return base.Draw(gameTime, spriteBatch, graphics, camera);
        }

        protected override void SetupEffects(Effect effect, GraphicsDevice graphics, ICamera camera, GameTime gameTime)
        {
            base.SetupEffects(effect, graphics, camera, gameTime);

            if (effect is BasicEffect)
            {
                BasicEffect e = (BasicEffect)effect;
                //e.LightingEnabled = false;
            }
        }
    }
}
