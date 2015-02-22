using EmpiresOfTheIV.Data_Models;
using KillerrinStudiosToolkit;
using KillerrinStudiosToolkit.Enumerators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Media.Imaging;
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

            try
            {
                myIP.Text = LANHelper.CurrentIPAddressAsString();
            }
            catch (Exception ex) { Debug.WriteLine(ex.PrintException("Showing IP")); }


            ObservableCollection<ChatMessage> items = new ObservableCollection<ChatMessage>();
            items.Add(new ChatMessage(new Uri("http://asset3.neogaf.com/forum/image.php?u=483446&dateline=1417594199", UriKind.Absolute), "killer rin", "HelloHelloHelloHelloHelloHello HelloHelloHelloHelloHello HelloHelloHelloHello HelloHelloHello HelloHelloHelloHello Hello HelloHelloHello HelloHello HelloHelloHelloHelloHelloHelloHello HelloHelloHelloHelloHelloHelloHello HelloHelloHelloHelloHelloHello", DateTime.UtcNow));
            items.Add(new ChatMessage(new Uri("http://asset3.neogaf.com/forum/image.php?u=483446&dateline=1417594199", UriKind.Absolute), "killer rin", "HelloHello HelloHelloHelloHello HelloHello HelloHello HelloHello HelloHelloHelloHello HelloHello HelloHello HelloHello HelloHelloHelloHello HelloHello HelloHello HelloHello HelloHelloHelloHello HelloHello HelloHello HelloHello HelloHelloHelloHello HelloHello HelloHello HelloHello HelloHelloHelloHello HelloHello HelloHello HelloHello HelloHelloHelloHello HelloHello HelloHello ", DateTime.UtcNow));
            items.Add(new ChatMessage(new Uri("http://asset3.neogaf.com/forum/image.php?u=483446&dateline=1417594199", UriKind.Absolute), "killer rin", "Hello", DateTime.UtcNow));
            items.Add(new ChatMessage(new Uri("http://asset3.neogaf.com/forum/image.php?u=483446&dateline=1417594199", UriKind.Absolute), "killer rin", "Hello", DateTime.UtcNow));
            items.Add(new ChatMessage(new Uri("http://asset3.neogaf.com/forum/image.php?u=483446&dateline=1417594199", UriKind.Absolute), "killer rin", "Hello", DateTime.UtcNow));
            items.Add(new ChatMessage(new Uri("http://asset3.neogaf.com/forum/image.php?u=483446&dateline=1417594199", UriKind.Absolute), "killer rin", "Hello", DateTime.UtcNow));
            items.Add(new ChatMessage(new Uri("http://asset3.neogaf.com/forum/image.php?u=483446&dateline=1417594199", UriKind.Absolute), "killer rin", "Hello", DateTime.UtcNow));
            items.Add(new ChatMessage(new Uri("http://asset3.neogaf.com/forum/image.php?u=483446&dateline=1417594199", UriKind.Absolute), "killer rin", "Hello", DateTime.UtcNow));
            items.Add(new ChatMessage(new Uri("http://asset3.neogaf.com/forum/image.php?u=483446&dateline=1417594199", UriKind.Absolute), "killer rin", "Hello", DateTime.UtcNow));
            chatLog.ItemsSource = items;

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

        private void MapSelector_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (mapSelector == null) { return; }
            if (mapSelector.SelectedItem == null) { return; }

            switch (mapSelector.SelectedIndex)
            {
                case 0:
                    int index = Consts.random.Next(1, 2);
                    mapSelector.SelectedIndex = index;
                    return;
                case 1: //<sys:String>Flatlands</sys:String>
                    mapImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Map Images/Radient Flatlands MiniMap.png", UriKind.Absolute));
                    mapSize.Text = "512x512";
                    mapLimit.Text = "2 Players";
                    mapDescription.Text = "Located on the planet of Radient Garden stands a stretch of land surrounded by mountains known only as the Flatlands";
                    break;
                default:
                    break;
            }
        }

        private void GameModeSelector_Changed(object sender, SelectionChangedEventArgs e)
        {

        }

        private void JoinTeam1_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void JoinTeam2_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void ChatLog_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        #region Chat Events

        #endregion
    }
}
