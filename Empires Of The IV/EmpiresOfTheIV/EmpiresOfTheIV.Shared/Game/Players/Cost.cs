using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Players
{
    public struct Cost
    {
        public float CurrencyCost { get; set; }
        public float MetalCost { get; set; }
        public float EnergyCost { get; set; }

        public Cost(float currencyCost, float metalCost, float energyCost)
        {
            CurrencyCost = currencyCost;
            MetalCost = metalCost;
            EnergyCost = energyCost;
        }
    }
}
