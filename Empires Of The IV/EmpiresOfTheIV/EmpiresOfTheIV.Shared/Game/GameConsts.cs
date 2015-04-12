using Anarian.Enumerators;
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
        public static int MaxUnitsOnWindowsPhonePerPlayer = 20;
        public static int MaxUnitsOnWindowsPhone { get { return MaxUnitsOnWindowsPhonePerPlayer * 2; } }

        public static class Loading
        {
            // Menus
            public static LoadingStatus Menu_UnifiedLoaded                      = LoadingStatus.NotLoaded;

            // Empire Loading
            public static LoadingStatus Empire_UnanianEmpireLoaded              = LoadingStatus.NotLoaded;
            public static LoadingStatus Empire_KingdomOfEdolasLoaded            = LoadingStatus.NotLoaded;
            public static LoadingStatus Empire_CrescanianConfederationLoaded    = LoadingStatus.NotLoaded;

            // Loading
            public static LoadingStatus Map_RadientFlatlands                    = LoadingStatus.NotLoaded;
            public static LoadingStatus Map_Kalia                               = LoadingStatus.NotLoaded;
        }

        public static class Settings
        {
            // Camera Controls
            public static bool InverseCameraX = true;
            public static bool InverseCameraY = true;
        }
    }
}
