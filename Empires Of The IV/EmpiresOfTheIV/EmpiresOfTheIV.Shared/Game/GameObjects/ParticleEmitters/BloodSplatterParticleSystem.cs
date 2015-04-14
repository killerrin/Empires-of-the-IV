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
    public class BloodSplatterParticleSystem : ParticleEmitter2D
    {
        public Vector3 WorldPosition;

        public BloodSplatterParticleSystem(Vector2 position, uint maxNumberOfParticles, Vector3 worldposition)
            : base(maxNumberOfParticles, new OneTimeParticleEmitter(), new TimebasedParticleLifespan(0.5f, 1.0f))
        {                     
            Position = position;
            WorldPosition = worldposition;

            //OnEmission += ExplosionParticleSystem_OnEmission;
            OnNoActiveParticlesRemaining += ExplosionParticleSystem_OnNoActiveParticlesRemaining;

            // Add one Time assets
            //ParticleTextures.Add(ResourceManager.Instance.GetAsset(typeof(Texture2D), ParticleNames.SmokeParticleEffect.ToString()) as Texture2D);
            ParticleTextures.Add(ResourceManager.Instance.GetAsset(typeof(Texture2D), ParticleNames.ExplosionParticleEffect.ToString()) as Texture2D);

            ParticleModifiersPostUpdate.Add(new OpacityLifespanParticleModifier());
            ParticleModifiersPostUpdate.Add(new ScaleLifespanParticleModifier(0.75f, 0.25f));
        }

        public override void Reset()
        {
            base.Reset();
        }

        #region Event Handlers
        void ExplosionParticleSystem_OnEmission(object sender, Anarian.Events.AnarianEventArgs e)
        {
        }
        void ExplosionParticleSystem_OnNoActiveParticlesRemaining(object sender, Anarian.Events.AnarianEventArgs e)
        {
            EmissionSettings.Active = false;
            OnNoActiveParticlesRemaining -= ExplosionParticleSystem_OnNoActiveParticlesRemaining;
        }
        #endregion

        protected override void InitializeConstants()
        {
            // high initial speed with lots of variance.  make the values closer
            // together to have more consistently circular explosions.
            minInitialSpeed = 40;
            maxInitialSpeed = 500;

            // doesn't matter what these values are set to, acceleration is tweaked in
            // the override of InitializeParticle.
            minAcceleration = 0;
            maxAcceleration = 0;

            minScale = .3f;
            maxScale = 1.0f;

            minRotationSpeed = -MathHelper.PiOver4;
            maxRotationSpeed = MathHelper.PiOver4;

            // additive blending is very good at creating fiery effects.
            blendState = BlendState.Additive;
        }

        /// <summary>
        /// InitializeParticle is overridden to add the appearance of wind.
        /// </summary>
        /// <param name="gameTime">The gameTime</param>
        /// <param name="particle">the particle to set up</param>
        protected override void InitializeParticle(GameTime gameTime, Particle2D particle)
        {
            base.InitializeParticle(gameTime, particle);

            // The base works fine except for acceleration. Explosions move outwards,
            // then slow down and stop because of air resistance. Let's change
            // acceleration so that when the particle is at max lifetime, the velocity
            // will be zero.

            // We'll use the equation vt = v0 + (a0 * t). (If you're not familar with
            // this, it's one of the basic kinematics equations for constant
            // acceleration, and basically says:
            // velocity at time t = initial velocity + acceleration * t)
            // We'll solve the equation for a0, using t = p.Lifetime and vt = 0.
            particle.Acceleration = -particle.Velocity / particle.MaxLifespan;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
        {
            ProjectedWorldPosition = camera.ProjectToScreenCoordinates(WorldPosition, graphics.Viewport);
            base.Draw(gameTime, spriteBatch, graphics, camera);
        }
    }
}
