using Anarian.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Players
{
    public class Resource : IUpdatable
    {
        public ResourceType ResourceType { get; protected set; }

        public float RateOfAccumilation { get; set; }
        public float AccumilationModifier { get; set; }
        public float CurrentAmount { get; set; }

        public Resource(ResourceType resourceType)
        {
            ResourceType = resourceType;

            RateOfAccumilation = 1.0f;
            AccumilationModifier = 1.0f;
            CurrentAmount = 0.0f;
        }

        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        public void Update(GameTime gameTime)
        {
            CurrentAmount += ((RateOfAccumilation * AccumilationModifier) * (float)gameTime.ElapsedGameTime.TotalMilliseconds);
        }
    }
}
