using Anarian;
using Anarian.DataStructures;
using Anarian.DataStructures.Components;
using Anarian.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EmpiresOfTheIV.Game.GameObjects.Factories
{
    public class Factory : Building
    {
        //List<FactoryPlot> m_factoryPlots;

        public Health Health { get { return GetComponent(typeof(Health)) as Health; } }
        private Texture2D blankTexture;
        public Point HealthBarOffset;

        public Factory()
            :base()
        {
            // Cash the refrences to textures
            blankTexture = ResourceManager.Instance.GetAsset(typeof(Texture2D), ResourceManager.EngineReservedAssetNames.blankTextureName) as Texture2D;
            HealthBarOffset = new Point(-100, 25);

            // Add Building Specific Components
            AddComponent(typeof(Health));
        }

        public override bool CheckRayIntersection(Ray ray)
        {
            return base.CheckRayIntersection(ray);
        }
        public override void Update(GameTime gameTime)
        {
            //Debug.WriteLine("Factory");
            base.Update(gameTime);

            Health.Update(gameTime);
        }
        public override bool Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera, bool creatingShadowMap = false)
        {
            bool result =  base.Draw(gameTime, spriteBatch, graphics, camera, creatingShadowMap);
            if (!result) return false;

            if (!creatingShadowMap)
                return DrawHealth(gameTime, spriteBatch, graphics, camera);
            return true;
        }

        public bool DrawHealth(GameTime gametime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
        {
            #region Draw the Health
            Vector3 screenPos3D = graphics.Viewport.Project(m_transform.WorldPosition, camera.Projection, camera.View, camera.World);
            Vector2 screenPos2D = new Vector2(screenPos3D.X, screenPos3D.Y);
            Rectangle healthRectOutline = new Rectangle((int)(screenPos2D.X - graphics.Viewport.X) + HealthBarOffset.X,
                                                        (int)(screenPos2D.Y - graphics.Viewport.Y) + HealthBarOffset.Y,
                                                        (int)Health.MaxHealth + 2,
                                                        5);
            Rectangle healthRect = new Rectangle(healthRectOutline.X + 1,
                                                 healthRectOutline.Y + 1,
                                                 (int)(MathHelper.Clamp(Health.CurrentHealth, 0, healthRectOutline.Width - 2)),
                                                 healthRectOutline.Height - 2);

            // Draw the Health rectangle
            spriteBatch.Begin();
            spriteBatch.Draw(blankTexture, healthRectOutline, Color.Black);
            spriteBatch.Draw(blankTexture, healthRect, Color.Red);
            spriteBatch.End();
            #endregion

            return true;
        }

        protected override void SetupEffects(Effect effect, GraphicsDevice graphics, ICamera camera, GameTime gameTime)
        {
            base.SetupEffects(effect, graphics, camera, gameTime);
            if (effect is BasicEffect)
            {
                BasicEffect e = (BasicEffect)effect;
            }
        }
    }
}
