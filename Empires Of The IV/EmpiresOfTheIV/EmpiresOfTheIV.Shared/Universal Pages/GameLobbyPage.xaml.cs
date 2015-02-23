using EmpiresOfTheIV.Data_Models;
using EmpiresOfTheIV.Game;
using EmpiresOfTheIV.Game.Networking;
using KillerrinStudiosToolkit;
using KillerrinStudiosToolkit.Enumerators;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
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
        string pageparam = "";


        ChatManager chatManager;
        string username;

        public GameLobbyPage()
        {
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;
            Loaded += GameLobbyPage_Loaded;
            
            this.InitializeComponent();

            Debug.WriteLine("GameLobbyPage loaded");
        }

        void GameLobbyPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                myIP.Text = LANHelper.CurrentIPAddressAsString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.PrintException("Showing IP"));
            }

            // Get our username
            username = SystemInfoHelper.GetMachineName();
            if (string.IsNullOrEmpty(username)) { username = "Player" + Consts.random.Next(4285745); }

            // Create our Chat Manager
            chatManager = new ChatManager();
            chatLog.ItemsSource = chatManager.ChatMessages;;

            switch (pageparam)
            {
                case "Singleplayer": break;
                case "HostLan": break;
                case "ClientLan": SetClientAbilities(); break;
                default:
                    break;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Consts.Game.GameManager.NetworkManager.OnConnected += NetworkManager_OnConnected;
            Consts.Game.GameManager.NetworkManager.OnMessageRecieved += NetworkManager_OnMessageRecieved;

            Consts.Game.GameManager.StateManager.OnBackButtonPressed += StateManager_OnBackButtonPressed;
            Consts.Game.GameManager.StateManager.HandleBackButtonPressed = false;

            pageparam = e.Parameter as string;

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Consts.Game.GameManager.NetworkManager.OnConnected -= NetworkManager_OnConnected;
            Consts.Game.GameManager.NetworkManager.OnMessageRecieved -= NetworkManager_OnMessageRecieved;
            Consts.Game.GameManager.StateManager.OnBackButtonPressed -= StateManager_OnBackButtonPressed;

            base.OnNavigatedFrom(e);
        }

        private void StateManager_OnBackButtonPressed(object sender, EventArgs e)
        {
            
        }

        #region Networking
        /// <summary>
        /// Will only get fired if the player is the host
        /// </summary>
        private void NetworkManager_OnConnected(object sender, KillerrinStudiosToolkit.Events.OnConnectedEventArgs e)
        {
            Debug.WriteLine("Connected to: " + e.NetworkConnectionEndpoint.Value.ToString() + " over " + e.NetworkType.ToString());
        }
        void NetworkManager_OnMessageRecieved(object sender, KillerrinStudiosToolkit.Events.ReceivedMessageEventArgs e)
        {
            // Get the regular object
            JObject jObject = JObject.Parse(e.Message);
            EotIVPacket regularPacket = JsonConvert.DeserializeObject<EotIVPacket>(jObject.ToString());

            // Now parse the object to get the right derived class
            if (regularPacket.PacketType == PacketType.Chat)
            {
                Debug.WriteLine("Chat Packet Recieved");
                ChatPacket chatPacket = JsonConvert.DeserializeObject<ChatPacket>(jObject.ToString());
                chatManager.AddMessage(chatPacket.Message);

                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        chatLog.ItemsSource = chatManager.ChatMessages;
                    }
                );
            }
            else if (regularPacket.PacketType == PacketType.System)
            {
                Debug.WriteLine("System Packet Recieved");
                SystemPacket systemPacket = JsonConvert.DeserializeObject<SystemPacket>(jObject.ToString());

                if (systemPacket.ID == SystemPacketID.GameModeChanged) {
                    Debug.WriteLine("Changing Game Mode: " + systemPacket.Command);
                    gameModeSelector.SelectedIndex = Convert.ToInt32(systemPacket.Command);
                }
                else if (systemPacket.ID == SystemPacketID.MapChanged) {
                    Debug.WriteLine("Changing Map: " + systemPacket.Command);
                    mapSelector.SelectedIndex = Convert.ToInt32(systemPacket.Command);
                }
                else if (systemPacket.ID == SystemPacketID.RequestSetupData) {
                    Debug.WriteLine("Client Requesting Startup Data");
                    SendGameModeChanged();
                    SendMapChanged();
                }
            }
        }

        private void SendMapChanged()
        {
            Debug.WriteLine("Sending Map Changed");

            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (mapSelector.SelectedIndex == 0) return;

                    SystemPacket packet = new SystemPacket(true, SystemPacketID.MapChanged, mapSelector.SelectedIndex.ToString());
                    string packetSerialized = packet.ThisToJson();
                    Consts.Game.GameManager.NetworkManager.SendMessage(packetSerialized);
                }
            );
        }
        private void SendGameModeChanged()
        {
            Debug.WriteLine("Sending Game Mode Changed");

            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (gameModeSelector.SelectedIndex == 0) return;

                    SystemPacket packet = new SystemPacket(true, SystemPacketID.GameModeChanged, gameModeSelector.SelectedIndex.ToString());
                    string packetSerialized = packet.ThisToJson();
                    Consts.Game.GameManager.NetworkManager.SendMessage(packetSerialized);
                }
            );
        }
        #endregion

        private void SetClientAbilities()
        {
            gameModeSelector.IsEnabled = false;
            mapSelector.IsEnabled = false;
            gameStartButton.IsEnabled = false;

            // Now request startup data
            SystemPacket packet = new SystemPacket(true, SystemPacketID.RequestSetupData, "");
            string packetSerialized = packet.ThisToJson();
            Consts.Game.GameManager.NetworkManager.SendMessage(packetSerialized);
        }

        #region GameLobby Events
        private void JoinTeam1_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
        private void JoinTeam2_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
        

        private void Team1SelectionChanged(object sender, TappedRoutedEventArgs e)
        {
            //Frame.Navigate(typeof(EmpireSelectionPage));
        }
        private void Team2SelectionChanged(object sender, TappedRoutedEventArgs e)
        {

        }

        private void gameStartButton_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
        #endregion

        #region Game Mode Events
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

            if (Consts.Game.GameManager.NetworkManager.HostSettings == HostType.Host)
            {
                SendMapChanged();
            }
        }

        private void GameModeSelector_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (gameModeSelector == null) { return; }
            if (gameModeSelector.SelectedItem == null) { return; }

            switch (gameModeSelector.SelectedIndex)
            {
                case 0:
                    int index = Consts.random.Next(1, 2);
                    gameModeSelector.SelectedIndex = index;
                    return;
                case 1: //<sys:String>1v1</sys:String>
                    break;
                default:
                    break;
            }

            if (Consts.Game.GameManager.NetworkManager.HostSettings == HostType.Host)
            {
                SendGameModeChanged();
            }
        }
        #endregion

        #region Chat Events
        #region ChatLog
        private void ChatLog_Tapped(object sender, TappedRoutedEventArgs e)
        {
            XamlControlHelper.LoseFocusOnTextBox(chatLog);
            chatLog.SelectedItem = null;
        }
        private void ChatLog_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            XamlControlHelper.LoseFocusOnTextBox(chatLog);
            chatLog.SelectedItem = null;
        }
        #endregion

        #region ChatBox
        bool chatTextBoxCurrentlyFocused = false;
        private void ChatTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine("Chat TextBox GotFocus");
            chatTextBoxCurrentlyFocused = true;
            chatSendButton.Content = "Send";
        }
        private void ChatTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Chat TextBox LostFocus");
            chatTextBoxCurrentlyFocused = false;

#if WINDOWS_PHONE_APP
            chatSendButton.Content = "Open Keyboard";
#endif
        }
        private void ChatTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                string chatText = ("" + chatTextBox.Text);
                SendMessage(chatText);
            }
        }
        #endregion

        #region ChatButton
        private void ChatSendButton_Loaded(object sender, RoutedEventArgs e)
        {
#if WINDOWS_PHONE_APP
            chatSendButton.Content = "Open Keyboard";
#endif
        }

        private void ChatSendButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
#if WINDOWS_PHONE_APP
            if (!chatTextBoxCurrentlyFocused)
            {
                chatTextBox.Focus(FocusState.Programmatic);
                return;
            }
#endif
            string chatText = ("" + chatTextBox.Text);
            SendMessage(chatText);
        }
        #endregion

        private void SendMessage(string msg)
        {
            if (string.IsNullOrEmpty(msg)) return;
            Debug.WriteLine("SendMessage(" + msg + ")");

            // Send the Message
            ChatMessage chatMessage = chatManager.AddMessage(username, msg);
            ChatPacket packet = new ChatPacket(true, chatMessage);
            string packetSerialized = packet.ThisToJson();

            try
            {
                Consts.Game.GameManager.NetworkManager.SendMessage(packetSerialized);
            }
            catch (Exception) { }

            // Reset the ChatBox
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        XamlControlHelper.LoseFocusOnTextBox(chatTextBox);
                    }
                    catch (Exception) { }
            
                    chatTextBox.Text = "";
                    chatLog.ItemsSource = chatManager.ChatMessages;
                }
            );
        }
        #endregion
    }
}
