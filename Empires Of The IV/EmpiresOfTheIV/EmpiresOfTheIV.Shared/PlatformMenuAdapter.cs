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
using KillerrinStudiosToolkit.Events;
using EmpiresOfTheIV.Game.Menus.PageParameters;

namespace EmpiresOfTheIV
{
    public static class PlatformMenuAdapter
    {
        #region Launch and Activation
        public static void OnLaunched()
        {
            if (Consts.EarlyExitCheck()) return;
        }

        public static void OnSuspending()
        {
            if (Consts.EarlyExitCheck()) return;
        }

        public static void OnActivated()
        {
            if (Consts.EarlyExitCheck()) return;
        }
        #endregion

        public static event EventHandler OnBackButtonPressed;

        #region Hardware Buttons
        public static void BackButtonPressed()
        {
            if (OnBackButtonPressed != null)
                OnBackButtonPressed(null, null);
        }

        public static void HomeButtonPressed()
        {
            if (Consts.EarlyExitCheck()) return;
        }

        public static void SearchButtonPressed()
        {
            if (Consts.EarlyExitCheck()) return;
        }
        #endregion

        #region Main Menu
        public static void MainMenu_SingleplayerButton_Click()
        {
            if (Consts.EarlyExitCheck()) return;
            Consts.Game.StateManager.Navigate(GameState.Singleplayer, GameConnectionType.Singleplayer);
        }

        public static void MainMenu_BluetoothMultiplayerButton_Click()
        {
            if (Consts.EarlyExitCheck()) return;
            Consts.Game.StateManager.Navigate(GameState.BluetoothMultiplayer);
        }

        public static void MainMenu_LANMultiplayerButton_Click()
        {
            if (Consts.EarlyExitCheck()) return;
            Consts.Game.StateManager.Navigate(GameState.LanMultiplayer);
        }

        public static void MainMenu_OptionsButton_Click()
        {
            if (Consts.EarlyExitCheck()) return;
            Consts.Game.StateManager.Navigate(GameState.Options);
        }

        public static void MainMenu_CreditsButton_Click()
        {
            if (Consts.EarlyExitCheck()) return;
            Consts.Game.StateManager.Navigate(GameState.Credits);
        }
        #endregion

        #region Multiplayer Menus
        public static void LanMultiplayerMenu_HostButton_Click()
        {
            if (Consts.EarlyExitCheck()) return;

            Consts.Game.StateManager.RemovePreviousOnCompleted = true;
            Consts.Game.StateManager.Navigate(GameState.GameLobby, GameConnectionType.LANHost);
        }

        public static void LanMultiplayerMenu_ConnectButton_Click()
        {
            if (Consts.EarlyExitCheck()) return;

            Consts.Game.StateManager.RemovePreviousOnCompleted = true;
            Consts.Game.StateManager.Navigate(GameState.GameLobby, GameConnectionType.LANClient);
        }
        #endregion

        #region Game Lobby Menus
        public static void GameLobbyMenu_StartGame_Click(InGamePageParameter parameter)
        {
            if (Consts.EarlyExitCheck()) return;

            Consts.Game.StateManager.RemovePreviousOnCompleted = true;
            Consts.Game.StateManager.Navigate(GameState.InGame, parameter);
        }        
        #endregion

        #region GameOverMenu
        public static void GameOverMenu_OK_Click()
        {
            if (Consts.EarlyExitCheck()) return;

            Consts.Game.StateManager.RemovePreviousOnCompleted = true;
            Consts.Game.StateManager.Navigate(GameState.MainMenu);
        }   
        #endregion
    }
}
