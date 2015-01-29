using KillerrinStudiosToolkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmpiresOfTheIV
{
    public static class Consts
    {
        public static EmpiresOfTheIVGame Game;

        public static BluetoothNetworkAdapter BluetoothNetworkAdapter = new BluetoothNetworkAdapter();
        public readonly Guid UniqueNetworkGUID = new Guid("20BB9225-AC4B-4ACD-9B12-126D8EBCF2D6");

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
