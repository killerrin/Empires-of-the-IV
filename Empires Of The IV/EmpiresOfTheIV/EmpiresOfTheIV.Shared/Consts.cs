using KillerrinStudiosToolkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV
{
    public static class Consts
    {
        public static EmpiresOfTheIVGame Game;
        public static string LaunchArguments;

        /// <summary>
        /// Used as an early exit check to determine if the Game is setup and ready to be interacted with
        /// </summary>
        /// <returns>True if ready, false if not</returns>
        public static bool EarlyExitCheck()
        {
            if (MainPage.PageFrame == null) return true;

            if (Consts.Game == null) return true;
            if (Consts.Game.GameManager == null) return true;
            if (Consts.Game.GameManager.StateManager == null) return true;

            return false;
        }
    }
}
