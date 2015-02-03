using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game.Players
{
    public static class PlayerIDManager
    {
        public static uint CurrentID { get; set; }

        public static void Reset() { CurrentID = 0; }

        public static uint GetNewID()
        {
            uint IDToUse = CurrentID;
            IncrimentID();

            return IDToUse;
        }

        private static void IncrimentID()
        {
            if (CurrentID < uint.MaxValue)
                CurrentID++;
            else
                CurrentID = uint.MinValue;
        }
    }
}
