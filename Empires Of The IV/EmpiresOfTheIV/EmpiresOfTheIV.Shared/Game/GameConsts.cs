using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV.Game
{
    public static class GameConsts
    {
        // PC Character Limit 125/Player - 250 Total
        public static int MaxUnitsOnWindowsPerPlayer = 125;
        public static int MaxUnitsOnWindows { get { return MaxUnitsOnWindowsPerPlayer * 2; } }

        // Phone Character Limit 25/Player - 50 Total
        public static int MaxUnitsOnWindowsPhonePerPlayer = 25;
        public static int MaxUnitsOnWindowsPhone { get { return MaxUnitsOnWindowsPhonePerPlayer * 2; } }
    }
}
