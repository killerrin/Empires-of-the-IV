using Anarian.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.GameObjects.Factories
{
    public class FactoryBase : Building
    {
        public Factory Factory;
        public bool IsFactoryOnBase { get { return (Factory != null); } }

        public FactoryBase()
            :base()
        {

        }

        public override bool CheckRayIntersection(Ray ray)
        {
            bool thisIntersection = base.CheckRayIntersection(ray);

            bool factoryIntersection = false;
            if (Factory != null)
                factoryIntersection = Factory.CheckRayIntersection(ray);

            if (thisIntersection || factoryIntersection) return true;
            return false; 
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Factory != null)
                Factory.Update(gameTime);
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphics, Camera camera)
        {
            base.Draw(gameTime, spriteBatch, graphics, camera);

            if (Factory != null)
                Factory.Draw(gameTime, spriteBatch, graphics, camera);
        }
    }
}
