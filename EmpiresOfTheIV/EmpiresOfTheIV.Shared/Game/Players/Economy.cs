﻿using Anarian.Interfaces;
using EmpiresOfTheIV.Game.Players;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Players
{
    public class Economy : IUpdatable
    {
        //public Player Owner { get; protected set; }
        public uint PlayerID { get; protected set; }
        
        public Resource Currency { get; protected set; }
        public Resource Metal { get; protected set; }
        public Resource Energy { get; protected set; }

        public Resource UnitCap { get; protected set; }

        public Economy(uint playerID)//APlayer player)
        {
            //Player = player;
            PlayerID = playerID;

            Currency = new Resource(ResourceType.Currency);
            Metal = new Resource(ResourceType.Metal);
            Energy = new Resource(ResourceType.Energy);

            UnitCap = new Resource(ResourceType.Unit);
        }

        public bool CanAfford(Cost cost)
        {
            return (Currency.CanAfford(cost.CurrencyCost)   &&
                   Metal.CanAfford(cost.MetalCost)      &&
                   Energy.CanAfford(cost.EnergyCost)     &&
                   UnitCap.CanAfford(cost.UnitCost)
                   );
        }

        public void AddCost(Cost cost)
        {
            Currency.AddAmount(cost.CurrencyCost);
            Metal.AddAmount(cost.MetalCost);
            Energy.AddAmount(cost.EnergyCost);
            UnitCap.AddAmount(cost.UnitCost);
        }

        public bool SubtractCost(Cost cost)
        {
            if (!CanAfford(cost)) return false;

            Currency.SubtractAmount(cost.CurrencyCost);
            Metal.SubtractAmount(cost.MetalCost);
            Energy.SubtractAmount(cost.EnergyCost);
            UnitCap.SubtractAmount(cost.UnitCost);
            return true;
        }


        void IUpdatable.Update(GameTime gameTime) { Update(gameTime); }
        public void Update(GameTime gameTime)
        {
            Currency.Update(gameTime);
            Metal.Update(gameTime);
            Energy.Update(gameTime);
        }
    }
}
