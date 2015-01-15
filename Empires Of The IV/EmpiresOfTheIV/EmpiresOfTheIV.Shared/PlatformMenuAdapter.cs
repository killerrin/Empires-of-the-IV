using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using KillerrinStudiosToolkit;

using Anarian;
using Anarian.DataStructures;
using Anarian.DataStructures.Animation;
using Anarian.DataStructures.Animation.Aux;
using Anarian.DataStructures.Input;
using Anarian.DataStructures.Rendering;
using Anarian.Enumerators;
using Anarian.Helpers;
using Anarian.Interfaces;
using EmpiresOfTheIV.Game.GameObjects;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using EmpiresOfTheIV.Game.Enumerators;

namespace EmpiresOfTheIV
{
    public static class PlatformMenuAdapter
    {
        private static bool EarlyExitCheck()
        {
            if (MainPage.PageFrame == null) return true;

            if (Consts.Game == null) return true;
            if (Consts.Game.GameManager == null) return true;
            if (Consts.Game.GameManager.StateManager == null) return true;

            return false;
        }

        #region Launch and Activation
        public static void OnLaunched()
        {
            if (EarlyExitCheck()) return;
        }

        public static void OnSuspending()
        {
            if (EarlyExitCheck()) return;
        }

        public static void OnActivated()
        {
            if (EarlyExitCheck()) return;
        }
        #endregion

        #region Hardware Buttons
        public static void BackButtonPressed()
        {
            if (EarlyExitCheck()) return;
            Consts.Game.GameManager.StateManager.GoBack();
        }

        public static void HomeButtonPressed()
        {
            if (EarlyExitCheck()) return;
        }

        public static void SearchButtonPressed()
        {
            if (EarlyExitCheck()) return;
        }
        #endregion

        #region Main Menu
        public static void MainMenu_PlayButton_Click()
        {
            if (EarlyExitCheck()) return;
            Consts.Game.GameManager.StateManager.Navigate(GameState.PlayGame);
        }

        public static void MainMenu_OptionsButton_Click()
        {
            if (EarlyExitCheck()) return;
            Consts.Game.GameManager.StateManager.Navigate(GameState.Options);
        }

        public static void MainMenu_CreditsButton_Click()
        {
            if (EarlyExitCheck()) return;
            Consts.Game.GameManager.StateManager.Navigate(GameState.Credits);
        }
        #endregion

        #region PlayGame Menu
        public static void PlayGameMenu_SingleplayerButton_Click()
        {
            if (EarlyExitCheck()) return;
            Consts.Game.GameManager.StateManager.Navigate(GameState.Singleplayer);
        }

        public static void PlayGameMenu_BluetoothMultiplayerButton_Click()
        {
            if (EarlyExitCheck()) return;
            Consts.Game.GameManager.StateManager.Navigate(GameState.BluetoothMultiplayer);
        }

        public static void PlayGameMenu_LANMultiplayerButton_Click()
        {
            if (EarlyExitCheck()) return;
            Consts.Game.GameManager.StateManager.Navigate(GameState.LanMultiplayer);
        }
        #endregion
    }
}
