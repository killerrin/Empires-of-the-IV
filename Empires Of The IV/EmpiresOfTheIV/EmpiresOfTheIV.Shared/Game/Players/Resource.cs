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

        public double RateOfAccumilation { get; set; }
        public double AccumilationModifier { get; set; }

        public double CurrentAmount { get; set; }
        public double MaxAmount { get; set; }

        public Resource(ResourceType resourceType)
        {
            ResourceType = resourceType;

            RateOfAccumilation = 1.0;
            AccumilationModifier = 1.0;
            CurrentAmount = 0.0;

            MaxAmount = double.MaxValue - 10.0;
        }

        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        public void Update(GameTime gameTime)
        {
            CurrentAmount += (RateOfAccumilation * AccumilationModifier) * gameTime.ElapsedGameTime.TotalMilliseconds;
            if (CurrentAmount >= MaxAmount) CurrentAmount = MaxAmount;
        }
    }
}
