using Anarian;
using Anarian.Interfaces;
using Anarian.Particles;
using Anarian.Particles.Particle2D;
using Anarian.Particles.Particle2D.Modifiers;
using Anarian.Particles.Particle2D.Modifiers.Emission;
using Anarian.Particles.Particle2D.Modifiers.Lifespan;
using Anarian.Particles.Particle2D.Modifiers.Modifiers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EmpiresOfTheIV.Game.GameObjects.ParticleEmitters
{
    public class BulletParticleSystem : ParticleEmitter2D
    {
        public Vector3 WorldPosition;
        public Vector3 TargetWorldPosition;

        public MoveToPositionParticleModifier MoveToPositionModifier;
        public DistanceParticleLifespan DistanceLifespan { get { return (DistanceParticleLifespan)ParticleLifespan; } }

        public BulletParticleSystem(Vector2 position, uint maxNumberOfParticles, Vector3 worldPosition)
            : base(maxNumberOfParticles, new OneTimeParticleEmitter(), new DistanceParticleLifespan(Vector2.Zero))
        {                     
            Position = position;
            WorldPosition = worldPosition;
            TargetWorldPosition = Vector3.Zero;

            OnEmission += BulletParticleSystem_OnEmission;
            OnNoActiveParticlesRemaining += BulletParticleSystem_OnNoActiveParticlesRemaining;

            // Add one Time assets
            ParticleTextures.Add(new TextureColorPair(ResourceManager.Instance.GetAsset(typeof(Texture2D), ParticleNames.StandardParticleEffect.ToString()) as Texture2D,
                                                      Color.DarkSlateGray)
                                );

            //ParticleModifiersPostUpdate.Add(new OpacityLifespanParticleModifier());
            ParticleModifiersPostUpdate.Add(ScaleLifespanParticleModifier.SimpleScale(0.025f));

            MoveToPositionModifier = new MoveToPositionParticleModifier(Vector2.Zero, 0.9f);
            ParticleModifiersPostUpdate.Add(MoveToPositionModifier);
        }

        #region Event Handlers
        void BulletParticleSystem_OnNoActiveParticlesRemaining(object sender, Anarian.Events.AnarianEventArgs e)
        {
            Debug.WriteLine("Bullet Killed");
            OnNoActiveParticlesRemaining -= BulletParticleSystem_OnNoActiveParticlesRemaining;
        }

        void BulletParticleSystem_OnEmission(object sender, Anarian.Events.AnarianEventArgs e)
        {
            OnEmission -= BulletParticleSystem_OnEmission;
        }
        #endregion

        public override void Reset()
        {
            base.Reset();
        }

        protected override void InitializeConstants()
        {
            // Add the rest of the constraints
            minInitialSpeed = 0;
            maxInitialSpeed = 0;

            // we don't want the particles to accelerate at all, aside from what we
            // do in our overriden InitializeParticle.
            minAcceleration = 0;
            maxAcceleration = 0;

            minScale = .5f;
            maxScale = 1.0f;

            // rotate slowly, we want a fairly relaxed effect
            minRotationSpeed = -MathHelper.PiOver4 / 2.0f;
            maxRotationSpeed = MathHelper.PiOver4 / 2.0f;

            blendState = BlendState.AlphaBlend;
        }

        /// <summary>
        /// InitializeParticle is overridden to add the appearance of wind.
        /// </summary>
        /// <param name="gameTime">The gameTime</param>
        /// <param name="particle">the particle to set up</param>
        protected override void InitializeParticle(GameTime gameTime, Particle2D particle)
        {
            base.InitializeParticle(gameTime, particle);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
        {
            //ProjectedWorldPosition = camera.ProjectToScreenCoordinates(WorldPosition, graphics.Viewport);

            Vector2 newProjectedTarget = camera.ProjectToScreenCoordinates(TargetWorldPosition, graphics.Viewport);
            MoveToPositionModifier.TargetPosition = newProjectedTarget;
            DistanceLifespan.TargetPosition = newProjectedTarget;

            base.Draw(gameTime, spriteBatch, graphics, camera);
        }
    }
}
