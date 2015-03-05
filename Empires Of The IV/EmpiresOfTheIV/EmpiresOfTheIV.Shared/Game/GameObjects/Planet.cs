using Anarian.DataStructures;
using Anarian.DataStructures.Components;
using Anarian.DataStructures.Rendering;
using Anarian.Interfaces;
using Anarian.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.GameObjects
{
    public class Planet : Sphere,
                          IUpdatable, IRenderable
    {
        #region Fields/Properties
        public Planet OrbitingBody;
        List<Planet> m_satellites;

        public float   m_planetTilt;
        public Vector3 m_initialOrbit;
        public Vector3 m_rotationalVelocity;
        public Vector3 m_orbitalVelocity;

        float m_wobbleAngle;
        float m_orbitAngle;
        Vector3 m_currentTilt;
        Vector3 m_currentRotation;
        Vector3 m_currentOrbit;
        #endregion

        public Planet(GraphicsDevice graphicsDevice, Texture2D planetTexture,
            float planetTilt, Vector3 initialOrbit, Vector3 rotationalVelocity, Vector3 orbitalVelocity)
            :base(graphicsDevice, planetTexture, 10, 10, 1.0f)
        {
            OrbitingBody = null;
            m_satellites = new List<Planet>();

            m_planetTilt = planetTilt;
            m_initialOrbit = initialOrbit;
            m_rotationalVelocity = rotationalVelocity;
            m_orbitalVelocity = orbitalVelocity;

            m_wobbleAngle = 0.0f;
            m_orbitAngle = 0.0f;

            Transform.Position = m_initialOrbit;
            m_currentRotation = Vector3.Zero;
            m_currentOrbit = Vector3.Zero;
            m_currentTilt = new Vector3(0.0f, m_planetTilt, 0.0f);
        }

        public void AddSatellite(Planet sat) { sat.OrbitingBody = this; m_satellites.Add(sat); }
        public Planet GetSatellite(int index) { return m_satellites[index]; }

        #region Interface Implimentations
        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        void IRenderable.Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera) { Draw(gameTime, spriteBatch, graphics, camera); }
        #endregion

        public override void Update(GameTime gameTime)
        {
            // Do the Tilt
            m_currentTilt = new Vector3(
                0.0f, 
                (m_planetTilt * 2.0f) * (float)Math.Cos((double)MathHelper.ToRadians(m_wobbleAngle)),
                (m_planetTilt * 2.0f) * (float)Math.Sin((double)MathHelper.ToRadians(m_wobbleAngle))
            );
            m_wobbleAngle = (m_wobbleAngle + 0.01f) % 360.0f;
            Transform.Rotation = Quaternion.CreateFromYawPitchRoll(m_currentTilt.X, m_currentTilt.Y, m_currentTilt.Z);

            // Rotate the Planet
            m_currentRotation += (m_rotationalVelocity * gameTime.DeltaTime());
            Transform.Rotation *= Quaternion.CreateFromYawPitchRoll(m_currentRotation.X, m_currentRotation.Y, m_currentRotation.Z);
            Transform.RotationMatrix = Matrix.CreateFromQuaternion(Transform.Rotation);

            // Finally the Orbit
            if (OrbitingBody != null)
            {

            }

            // Now that we have updated our transform, we can update our base
            base.Update(gameTime);

            foreach (var sat in m_satellites)
            {
                sat.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
        {
            base.Draw(gameTime, spriteBatch, graphics, camera);


            foreach (var sat in m_satellites)
            {
                sat.Draw(gameTime, spriteBatch, graphics, camera);
            }
        }

        protected override void SetupEffects(Effect effect, GraphicsDevice graphics, ICamera camera, GameTime gameTime)
        {
            base.SetupEffects(effect, graphics, camera, gameTime);
        }
    }
}
