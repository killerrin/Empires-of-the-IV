using Anarian;
using Anarian.DataStructures;
using Anarian.DataStructures.Components;
using Anarian.Interfaces;
using EmpiresOfTheIV.Game.Enumerators;
using EmpiresOfTheIV.Game.GameObjects.ParticleEmitters;
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

        public GameObjectLifeState LifeState;
        private Texture2D blankTexture;
        public Point HealthBarOffset;

        Timer DeathVisibilityTimer;
        public SmokePlumeParticleSystem SmokePlumeParticleEmitter;
        public ExplosionParticleSystem ExplosionParticleEmitter;

        public Factory()
            :base()
        {
            // Cash the refrences to textures
            blankTexture = ResourceManager.Instance.GetAsset(typeof(Texture2D), ResourceManager.EngineReservedAssetNames.blankTextureName) as Texture2D;
            HealthBarOffset = new Point(-100, 25);

            LifeState = GameObjectLifeState.Alive;

            DeathVisibilityTimer = new Timer(TimeSpan.FromSeconds(0.3));
            ExplosionParticleEmitter = new ExplosionParticleSystem(Vector2.Zero, 20, m_transform.WorldPosition);
            ExplosionParticleEmitter.OnNoActiveParticlesRemaining += ExplosionParticleEmitter_OnNoActiveParticlesRemaining;

            SmokePlumeParticleEmitter = new SmokePlumeParticleSystem(Vector2.Zero, 20, m_transform.WorldPosition);
            SmokePlumeParticleEmitter.Progress = Anarian.Enumerators.ProgressStatus.InProgress;

            // Add Building Specific Components
            AddComponent(typeof(Health));
        }

        public override bool CheckRayIntersection(Ray ray)
        {
            return base.CheckRayIntersection(ray);
        }

        #region Event Handlers
        void ExplosionParticleEmitter_OnNoActiveParticlesRemaining(object sender, Anarian.Events.AnarianEventArgs e)
        {
            LifeState = GameObjectLifeState.Dead;
            ExplosionParticleEmitter.OnNoActiveParticlesRemaining -= ExplosionParticleEmitter_OnNoActiveParticlesRemaining;
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            if (LifeState == GameObjectLifeState.Dying)
            {
                DeathVisibilityTimer.Update(gameTime);

                ExplosionParticleEmitter.WorldPosition = m_transform.WorldPosition;
                ExplosionParticleEmitter.Update(gameTime);
            }

            base.Update(gameTime);

            if (Health.CurrentHealth <= Health.MaxHealth * 0.7)
                SmokePlumeParticleEmitter.EmissionSettings.Active = true;
            else 
                SmokePlumeParticleEmitter.EmissionSettings.Active = false;

            SmokePlumeParticleEmitter.WorldPosition = m_transform.WorldPosition + new Vector3(0.0f, 0.0f, -5.0f);
            SmokePlumeParticleEmitter.Update(gameTime);
            
            //Health.Update(gameTime);
        }
        public override bool Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera, bool creatingShadowMap = false)
        {
            if (DeathVisibilityTimer.Progress != Anarian.Enumerators.ProgressStatus.Completed)
            {
                bool result = base.Draw(gameTime, spriteBatch, graphics, camera, creatingShadowMap);
                if (!result) return false;
            }

            if (!creatingShadowMap)
                return DrawEffects(gameTime, spriteBatch, graphics, camera);
            return true;
        }

        public bool DrawEffects(GameTime gametime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
        {
            if (LifeState == GameObjectLifeState.Dead) return false;

            SmokePlumeParticleEmitter.Draw(gametime, spriteBatch, graphics, camera);

            if (LifeState == GameObjectLifeState.Alive)
            {
                #region Draw the Health
                Vector2 screenPos2D = camera.ProjectToScreenCoordinates(m_transform.WorldPosition, graphics.Viewport);
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
            }

            if (LifeState == GameObjectLifeState.Dying)
            {
                ExplosionParticleEmitter.Draw(gametime, spriteBatch, graphics, camera);
            }

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
