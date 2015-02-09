using KillerrinStudiosToolkit.Enumerators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace EmpiresOfTheIV
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GameLobbyPage : Page
    {
        public GameLobbyPage()
        {
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;
            this.InitializeComponent();
            Debug.WriteLine("GameLobbyPage loaded");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Consts.Game.GameManager.NetworkManager.OnConnected += NetworkManager_OnConnected;
            Consts.Game.GameManager.NetworkManager.OnMessageRecieved += NetworkManager_OnMessageRecieved;
            Consts.Game.GameManager.StateManager.OnBackButtonPressed += StateManager_OnBackButtonPressed;
            Consts.Game.GameManager.StateManager.HandleBackButtonPressed = false;

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Consts.Game.GameManager.NetworkManager.OnConnected -= NetworkManager_OnConnected;
            Consts.Game.GameManager.StateManager.OnBackButtonPressed -= StateManager_OnBackButtonPressed;
            base.OnNavigatedFrom(e);
        }

        private void StateManager_OnBackButtonPressed(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Will only get fired if the player is the host
        /// </summary>
        private void NetworkManager_OnConnected(object sender, KillerrinStudiosToolkit.Events.OnConnectedEventArgs e)
        {
            Debug.WriteLine("Connected to: " + e.NetworkConnectionEndpoint.Value.ToString() + " over " + e.NetworkType.ToString());
        }
        void NetworkManager_OnMessageRecieved(object sender, KillerrinStudiosToolkit.Events.ReceivedMessageEventArgs e)
        {
        }

        #region GameLobby Events
        private void gameStartButton_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void Team1SelectionChanged(object sender, TappedRoutedEventArgs e)
        {
            //Frame.Navigate(typeof(EmpireSelectionPage));
        }
        private void Team2SelectionChanged(object sender, TappedRoutedEventArgs e)
        {

        }
        #endregion

        #region Chat Events

        #endregion
    }
}
