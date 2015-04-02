using Anarian.DataStructures;
using Anarian.Interfaces;
using Anarian.Helpers;
using EmpiresOfTheIV.Game.Enumerators;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.GameObjects.Factories
{
    public class FactoryBase : AnarianObject, IUpdatable, IRenderable
    {
        
        public uint FactoryBaseID { get; private set; }
        public uint PlayerID;

        public StaticGameObject Base;
        public Factory Factory;

        public BoundingSphere Bounds;

        #region Properties
        public bool IsFactoryOnBase { get { return (Factory != null); } }
        #endregion

        public FactoryBase(uint id, BoundingSphere bounds)
            :base()
        {
            FactoryBaseID = id;
            PlayerID = uint.MaxValue;

            Bounds = bounds;
        }

        public bool HasOwner
        {
            get
            {
                if (PlayerID == uint.MaxValue) return false;
                return true;
            }
        }

        public FactoryBaseRayIntersection CheckRayIntersection(Ray ray)
        {

            float? result = Bounds.Intersects(ray);
            if (result.HasValue)
            {
                if (HasOwner)
                    return FactoryBaseRayIntersection.Factory;
                else
                    return FactoryBaseRayIntersection.FactoryBase;
            }

            // No collision detected
            return FactoryBaseRayIntersection.None; 
        }

        public FactoryBaseRayIntersection CheckSphereIntersection(BoundingSphere sphere)
        {
            bool result = Bounds.Intersects(sphere);
            if (result)
            {
                if (HasOwner)
                    return FactoryBaseRayIntersection.Factory;
                else
                    return FactoryBaseRayIntersection.FactoryBase;
            }

            // No collision detected
            return FactoryBaseRayIntersection.None;
        }

        public void Update(GameTime gameTime)
        {
            if (Base != null)
            {
                Base.Update(gameTime);
                Bounds.Center = Base.Transform.WorldPosition;
            }

            if (Factory != null)
                Factory.Update(gameTime);
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
        {
            if (Base != null)
                Base.Draw(gameTime, spriteBatch, graphics, camera);

            if (Factory != null)
                Factory.Draw(gameTime, spriteBatch, graphics, camera);

            //Bounds.RenderBoundingSphere(graphics, camera.World, camera.View, camera.Projection, Color.Red);
        }
    }
}
