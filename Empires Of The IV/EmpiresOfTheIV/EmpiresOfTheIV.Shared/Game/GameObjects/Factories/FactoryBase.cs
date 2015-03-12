using Anarian.DataStructures;
using Anarian.Interfaces;
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

        public StaticGameObject Base;
        public Factory Factory;

        #region Properties
        public bool IsFactoryOnBase { get { return (Factory != null); } }
        #endregion

        public FactoryBase(uint id)
            :base()
        {
            FactoryBaseID = id;
        }

        public FactoryBaseRayIntersection CheckRayIntersection(Ray ray)
        {
            
            bool factoryIntersection = false;
            bool factoryBaseIntersection = false;

            // Check the collision off of the Factory
            if (Factory != null)
            {
                factoryIntersection = Factory.CheckRayIntersection(ray);
                factoryBaseIntersection = Base.CheckRayIntersection(ray);

                if (factoryIntersection || factoryBaseIntersection) return FactoryBaseRayIntersection.Factory;
                return FactoryBaseRayIntersection.None;
            }
                
            // There is no factory on the base
            // Check the collison off of the Factory Base
            if (Base != null)
            {
                factoryBaseIntersection = Base.CheckRayIntersection(ray);
                if (factoryBaseIntersection) return FactoryBaseRayIntersection.FactoryBase;
            }

            // No collision detected
            return FactoryBaseRayIntersection.None; 
        }
        public void Update(GameTime gameTime)
        {
            if (Base != null)
                Base.Update(gameTime);

            if (Factory != null)
                Factory.Update(gameTime);
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, ICamera camera)
        {
            if (Base != null)
                Base.Draw(gameTime, spriteBatch, graphics, camera);

            if (Factory != null)
                Factory.Draw(gameTime, spriteBatch, graphics, camera);
        }
    }
}
