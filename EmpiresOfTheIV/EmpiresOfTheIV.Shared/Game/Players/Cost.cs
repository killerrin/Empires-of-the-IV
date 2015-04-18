using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Players
{
    public struct Cost
    {
        public double CurrencyCost;
        public double MetalCost;
        public double EnergyCost;
        public double UnitCost;

        public Cost(double currencyCost, double metalCost, double energyCost, double unitCost)
        {
            CurrencyCost = currencyCost;
            MetalCost = metalCost;
            EnergyCost = energyCost;

            UnitCost = unitCost;
        }

        public Cost OnlyEconomyCost() { return Cost.FromEconomyCost(CurrencyCost, MetalCost, EnergyCost); }
        public Cost OnlyUnitCost() { return Cost.FromUnitCost(UnitCost); }

        public static Cost Zero { get { return new Cost(0.0, 0.0, 0.0, 0.0); } }
        public static Cost FromEconomyCost(double currencyCost, double metalCost, double energyCost) { return new Cost(currencyCost, metalCost, energyCost, 0.0); }
        public static Cost FromUnitCost(double unitCost) { return new Cost(0.0, 0.0, 0.0, unitCost); }
    }
}
