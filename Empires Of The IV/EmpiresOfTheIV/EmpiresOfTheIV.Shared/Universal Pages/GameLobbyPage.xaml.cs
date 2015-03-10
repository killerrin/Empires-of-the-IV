using Anarian.IDManagers;
using EmpiresOfTheIV.Data_Models;
using EmpiresOfTheIV.Game;
using EmpiresOfTheIV.Game.Enumerators;
using EmpiresOfTheIV.Game.Menus.PageParameters;
using EmpiresOfTheIV.Game.Networking;
using EmpiresOfTheIV.Game.Players;
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
        GameConnectionType pageparam;

        ChatManager chatManager;

        IDManager playerIDManager;
        Team team1;
        Team team2;

        string username;    // My Username
        uint playerID;      // My Player ID

        int totalMaps = 2;
        int totalGameModes = 2;

        bool gameStarting;
        bool enableChangingTeams = false;

        public GameLobbyPage()
        {
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;
            Loaded += GameLobbyPage_Loaded;
            
            this.InitializeComponent();

            Debug.WriteLine("GameLobbyPage loaded");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Consts.Game.NetworkManager.OnConnected += NetworkManager_OnConnected;
            Consts.Game.NetworkManager.OnDisconnected += NetworkManager_OnDisconnected;
            Consts.Game.NetworkManager.OnMessageRecieved += NetworkManager_OnMessageRecieved;

            Consts.Game.StateManager.OnBackButtonPressed += StateManager_OnBackButtonPressed;
            Consts.Game.StateManager.HandleBackButtonPressed = false;

            GameConnectionType? connType = e.Parameter as GameConnectionType?;
            pageparam = connType.Value;

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Debug.WriteLine("Unsubscribed from GameLobbyPage");
            Consts.Game.NetworkManager.OnConnected -= NetworkManager_OnConnected;
            Consts.Game.NetworkManager.OnDisconnected -= NetworkManager_OnDisconnected;
            Consts.Game.NetworkManager.OnMessageRecieved -= NetworkManager_OnMessageRecieved;

            Consts.Game.StateManager.OnBackButtonPressed -= StateManager_OnBackButtonPressed;

            base.OnNavigatedFrom(e);
        }

        private void StateManager_OnBackButtonPressed(object sender, EventArgs e)
        {
            
        }

        void GameLobbyPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Get the IP Address
            try { myIP.Text = LANHelper.CurrentIPAddressAsString(); }
            catch (Exception) { }

            // Get our Username
            username = SystemInfoHelper.GetMachineName();
            if (string.IsNullOrEmpty(username)) { username = "Player" + Consts.random.Next(42845); }
            myUsername.Text = username;

            gameStarting = false;

            // Create our Chat Manager
            chatManager = new ChatManager();
            chatLog.ItemsSource = chatManager.ChatMessages; ;

            if (enableChangingTeams)
            {
                joinTeam1Button.IsEnabled = true;
                joinTeam2Button.IsEnabled = true;
            }

            // Create our Teams
            team1 = new Team(TeamID.TeamOne);
            team2 = new Team(TeamID.TeamTwo);

            playerIDManager = new IDManager();

            // Set the Host/Client UI Abilities
            SetAbilities();
        }

        private void maxUnitSlider_Loaded(object sender, RoutedEventArgs e)
        {
            // Set the Maximum on the slider for that specific platform
#if WINDOWS_PHONE_APP
            maxUnitSlider.Maximum = GameConsts.MaxUnitsOnWindowsPhonePerPlayer;
#elif WINDOWS_APP
            maxUnitSlider.Maximum = GameConsts.MaxUnitsOnWindowsPerPlayer;
#endif
            
            // Update the Header
            maxUnitSlider.Header = "Max Units Per Player: " + maxUnitSlider.Value;
        }

        private void SetAbilities()
        {
            switch (pageparam)
            {
                case GameConnectionType.Singleplayer:    SetHostAbilities();     break;
                case GameConnectionType.LANHost:         SetHostAbilities();     break;
                case GameConnectionType.LANClient: SetClientAbilities(); break;
                default:
                    break;
            }

            // Get off of the starting value of "random"
            try
            {
                if (gameModeSelector.SelectedIndex == 0)
                {
                    gameModeSelector.SelectedItem = null;
                    gameModeSelector.SelectedIndex = 1;
                }

                if (mapSelector.SelectedIndex == 0)
                {
                    mapSelector.SelectedItem = null;
                    mapSelector.SelectedIndex = 1;
                }
            }
            catch (Exception) { }
        }

        private void SetHostAbilities()
        {
            gameModeSelector.IsEnabled = true;
            mapSelector.IsEnabled = true;
            gameStartButton.IsEnabled = true;

            // Add the Host to its team
            team1.AddToTeam(PlayerType.Human, playerIDManager.GetNewID(), username);
            playerID = team1[0].ID;

            team1ListBox.ItemsSource = team1.Players;
            team2ListBox.ItemsSource = team2.Players;
        }

        private void SetClientAbilities()
        {
            gameModeSelector.IsEnabled = false;
            mapSelector.IsEnabled = false;
            gameStartButton.IsEnabled = false;
            maxUnitSlider.IsEnabled = false;

            // Now request startup data
            string sendData = (int)KillerrinApplicationData.OSType + "|" +
                              username;

            SystemPacket packet = new SystemPacket(true, SystemPacketID.RequestSetupData, sendData);
            string packetSerialized = packet.ThisToJson();
            Consts.Game.NetworkManager.SendMessage(packetSerialized);
        }

        #region Networking
        /// <summary>
        /// Will only get fired if the player is the host
        /// </summary>
        private void NetworkManager_OnConnected(object sender, KillerrinStudiosToolkit.Events.OnConnectedEventArgs e)
        {
            Debug.WriteLine("Connected to: " + e.NetworkConnectionEndpoint.Value.ToString() + " over " + e.NetworkType.ToString());
        }
        void NetworkManager_OnDisconnected(object sender, EventArgs e)
        {
        
        }

        void NetworkManager_OnMessageRecieved(object sender, KillerrinStudiosToolkit.Events.ReceivedMessageEventArgs e)
        {
            if (e.Message == Consts.Game.NetworkManager.LanHelper.ConnectionCloseMessage)
            {
                return;
            }

            // Get the regular object
            JObject jObject = JObject.Parse(e.Message);
            EotIVPacket regularPacket = JsonConvert.DeserializeObject<EotIVPacket>(jObject.ToString());
            Debug.WriteLine("Deserialized Packet");

            // Now parse the object to get the right derived class
            if (regularPacket.PacketType == PacketType.Chat)
            {
                Debug.WriteLine("Chat Packet Recieved");
                ChatPacket chatPacket = JsonConvert.DeserializeObject<ChatPacket>(jObject.ToString());

                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        chatManager.AddMessage(chatPacket.Message);
                        chatLog.ItemsSource = chatManager.ChatMessages;
                    }
                );
            }
            else if (regularPacket.PacketType == PacketType.System)
            {
                Debug.WriteLine("System Packet Recieved");
                SystemPacket systemPacket = JsonConvert.DeserializeObject<SystemPacket>(jObject.ToString());

                if (systemPacket.ID == SystemPacketID.RequestSetupData) {
                    Debug.WriteLine("Client Requesting Startup Data: " + systemPacket.Command);

                    string[] splitCommands = systemPacket.Command.Split('|');

                    // Send the GameMode and Map
                    SendGameModeChanged();
                    SendMapChanged();

                    // Send the Max Units
                    CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            // If the host is a Windows Phone, we dont really need to do the check as our maximum will always be the Windows Phone Maximum
                            if (KillerrinApplicationData.OSType == ClientOSType.WindowsPhone81)
                            {
                                maxUnitSlider.Maximum = GameConsts.MaxUnitsOnWindowsPhonePerPlayer;
                            }
                            else // We set our maximum based off of what our opponent is capable of
                            {
                                int ostype = Convert.ToInt32(splitCommands[0]);
                                ClientOSType opponentOSType = (ClientOSType)ostype;
                                Debug.WriteLine("Opponent Device Type: " + opponentOSType.ToString());
                                switch (opponentOSType)
                                {
                                    case ClientOSType.Windows81: maxUnitSlider.Maximum = GameConsts.MaxUnitsOnWindowsPerPlayer; break;
                                    case ClientOSType.WindowsPhone81: maxUnitSlider.Maximum = GameConsts.MaxUnitsOnWindowsPhonePerPlayer; break;
                                    case ClientOSType.Windows10: break;
                                    case ClientOSType.Other: break;
                                    default: break;
                                }
                            }
                            SendMaxUnitsChanged();
                        }
                    );

                    // Create and Send the Teams
                    CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            int ostype = Convert.ToInt32(splitCommands[0]);
                            ClientOSType opponentOSType = (ClientOSType)ostype;
                            uint playerID = playerIDManager.GetNewID();

                            if (team1.PlayerCount > team2.PlayerCount) {
                                Debug.WriteLine("Player added to Team2");
                                team2.AddToTeam(PlayerType.Human, playerID, splitCommands[1]);
                                team2.GetPlayer(playerID).OperatingSystem = opponentOSType; 
                            }
                            else { //if (team1.PlayerCount == team2.PlayerCount) 
                                Debug.WriteLine("Player added to Team1");
                                team1.AddToTeam(PlayerType.Human, playerID, splitCommands[1]);
                                team1.GetPlayer(playerID).OperatingSystem = opponentOSType; 
                            }

                            team1ListBox.ItemsSource = null;
                            team1ListBox.ItemsSource = team1.Players;

                            team2ListBox.ItemsSource = null;
                            team2ListBox.ItemsSource = team2.Players;


                            SendTeamsChanged();
                        }
                    );
                }

                else if (systemPacket.ID == SystemPacketID.GameModeChanged)
                {
                    Debug.WriteLine("Host Changed the Game Mode: " + systemPacket.Command);
                    CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            gameModeSelector.SelectedIndex = Convert.ToInt32(systemPacket.Command);
                        }
                    );
                }
                else if (systemPacket.ID == SystemPacketID.UnitMaxChanged)
                {
                    Debug.WriteLine("Host Changed the Unit Max: " + systemPacket.Command);

                    CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            string[] splitCommands = systemPacket.Command.Split('|');
                            maxUnitSlider.Value = Convert.ToDouble(splitCommands[0]);
                            maxUnitSlider.Maximum = Convert.ToDouble(splitCommands[1]);
                        }
                    );
                }
                else if (systemPacket.ID == SystemPacketID.MapChanged)
                {
                    Debug.WriteLine("Host Changed the Map: " + systemPacket.Command);
                    CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            mapSelector.SelectedIndex = Convert.ToInt32(systemPacket.Command);
                        }
                    );
                }
                else if (systemPacket.ID == SystemPacketID.TeamsChanged)
                {
                    Debug.WriteLine("Host Changed the Teams: " + systemPacket.Command);
                    string[] splitCommands = systemPacket.Command.Split('|');

                    string team1string = splitCommands[0];//.Substring(1, splitCommands[0].Length - 2);
                    string team2string = splitCommands[1];//.Substring(1, splitCommands[1].Length - 2);
                    Debug.WriteLine(team1string);
                    Debug.WriteLine(team2string);

                    JObject team1JObject = JObject.Parse(team1string);
                    JObject team2JObject = JObject.Parse(team2string);
                    //Debug.WriteLine("Teams JObject Parsed");

                    team1 = JsonConvert.DeserializeObject<Team>(team1JObject.ToString());   Debug.WriteLine("Team 1 Deserialized");
                    team2 = JsonConvert.DeserializeObject<Team>(team2JObject.ToString());   Debug.WriteLine("Team 2 Deserialized");

                    if (team1.Exists(username)) { playerID = team1.GetPlayer(username).ID; }
                    if (team2.Exists(username)) { playerID = team2.GetPlayer(username).ID; }

                    CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            team1ListBox.ItemsSource = null;
                            team1ListBox.ItemsSource = team1.Players;

                            team2ListBox.ItemsSource = null;
                            team2ListBox.ItemsSource = team2.Players;
                        }
                    );
                }
                else if (systemPacket.ID == SystemPacketID.JoinTeam1)
                {
                    Debug.WriteLine("A Player Joined Team 1");

                    string[] splitCommands = systemPacket.Command.Split('|');
                    string opponentUsername = splitCommands[0];
                    uint opponentPlayerID = Convert.ToUInt32(splitCommands[1]);

                    Player me = team2.GetPlayer(opponentPlayerID);
                    team2.RemovePlayer(opponentPlayerID);
                    team1.AddToTeam(me);

                    team1ListBox.ItemsSource = null;
                    team1ListBox.ItemsSource = team1.Players;

                    team2ListBox.ItemsSource = null;
                    team2ListBox.ItemsSource = team2.Players;
                    
                    SendTeamsChanged();
                }
                else if (systemPacket.ID == SystemPacketID.JoinTeam2)
                {
                    Debug.WriteLine("A Player Joined Team 2");

                    string[] splitCommands = systemPacket.Command.Split('|');
                    string opponentUsername = splitCommands[0];
                    uint opponentPlayerID = Convert.ToUInt32(splitCommands[1]);

                    Player me = team1.GetPlayer(opponentPlayerID);
                    team1.RemovePlayer(opponentPlayerID);
                    team2.AddToTeam(me);

                    team1ListBox.ItemsSource = null;
                    team1ListBox.ItemsSource = team1.Players;

                    team2ListBox.ItemsSource = null;
                    team2ListBox.ItemsSource = team2.Players;

                    SendTeamsChanged();
                }
                else if (systemPacket.ID == SystemPacketID.GameStart)
                {
                    if (Consts.Game.NetworkManager.HostSettings == HostType.Host)
                    {
                        Debug.WriteLine("Clients Responded to GameStart. Starting Game");
                        SendTransitionAndGameLoad();
                        StartGame();
                    }
                    else
                    {
                        if (systemPacket.Command == true.ToString())
                        {
                            Debug.WriteLine("Host Started the Game");
                            gameStartButton.Content = "Cancel";
                            gameStarting = true;

                            SendStartGame();
                        }
                        else if (systemPacket.Command == false.ToString())
                        {
                            Debug.WriteLine("The Host Cancelled the Game Start");
                            gameStartButton.Content = "Start";
                            gameStarting = false;
                        }
                    }
                }
                else if (systemPacket.ID == SystemPacketID.TransitionAndGameLoad)
                {
                    StartGame();
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
                    Consts.Game.NetworkManager.SendMessage(packetSerialized);
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
                    Consts.Game.NetworkManager.SendMessage(packetSerialized);
                }
            );
        }

        private void SendMaxUnitsChanged()
        {
            Debug.WriteLine("Sending Unit Max Changed");

            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    string sendData = maxUnitSlider.Value.ToString() + "|" +
                                      maxUnitSlider.Maximum.ToString();

                    SystemPacket packet = new SystemPacket(true, SystemPacketID.UnitMaxChanged, sendData);
                    string packetSerialized = packet.ThisToJson();
                    Consts.Game.NetworkManager.SendMessage(packetSerialized);
                }
            );
        }

        private void SendJoinTeam1()
        {
            Debug.WriteLine("Sending Join Team 1");

            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    string sendData = username + "|" +
                                      playerID;

                    SystemPacket packet = new SystemPacket(true, SystemPacketID.JoinTeam1, sendData);
                    string packetSerialized = packet.ThisToJson();
                    Consts.Game.NetworkManager.SendMessage(packetSerialized);
                }
            );
        }

        private void SendJoinTeam2()
        {
            Debug.WriteLine("Sending Join Team 2");

            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    string sendData = username + "|" +
                                      playerID;

                    SystemPacket packet = new SystemPacket(true, SystemPacketID.JoinTeam2, sendData);
                    string packetSerialized = packet.ThisToJson();
                    Consts.Game.NetworkManager.SendMessage(packetSerialized);
                }
            );
        }

        private void SendTeamsChanged()
        {
            Debug.WriteLine("Sending Teams Changed");

            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    string sendData = team1.ThisToJson() + "|" + team2.ThisToJson();

                    SystemPacket packet = new SystemPacket(true, SystemPacketID.TeamsChanged, sendData);
                    string packetSerialized = packet.ThisToJson();
                    Consts.Game.NetworkManager.SendMessage(packetSerialized);
                }
            );

        }

        private void SendStartGame()
        {
            Debug.WriteLine("Sending Game Start");

            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    // Since we are the host, we do a final sync
                    if (Consts.Game.NetworkManager.HostSettings == HostType.Host)
                    {
                        SendGameModeChanged();
                        SendMaxUnitsChanged();
                        SendMapChanged();
                        SendTeamsChanged();
                    }

                    SystemPacket packet = new SystemPacket(true, SystemPacketID.GameStart, true.ToString());
                    string packetSerialized = packet.ThisToJson();
                    Consts.Game.NetworkManager.SendMessage(packetSerialized);
                }
            );
        }

        private void SendCancelStartGame()
        {
            Debug.WriteLine("Sending Game Start");

            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    SystemPacket packet = new SystemPacket(true, SystemPacketID.GameStart, false.ToString());
                    string packetSerialized = packet.ThisToJson();
                    Consts.Game.NetworkManager.SendMessage(packetSerialized);
                }
            );
        }

        private void SendTransitionAndGameLoad()
        {
            Debug.WriteLine("Sending Game Start");

            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                SystemPacket packet = new SystemPacket(true, SystemPacketID.TransitionAndGameLoad, true.ToString());
                string packetSerialized = packet.ThisToJson();
                Consts.Game.NetworkManager.SendMessage(packetSerialized);
            }
            );
        }
        #endregion

        public void StartGame()
        {
            // Create the page parameter and send us off
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        InGamePageParameter pageParameter = new InGamePageParameter();

                        pageParameter.GameConnectionType = pageparam;

                        switch (gameModeSelector.SelectedIndex)
                        {
                            case 0:
                            case 1:
                            default: pageParameter.GameType = GameMode.OneVSOne;           break;
                        }

                        switch (mapSelector.SelectedIndex)
                        {
                            case 0:
                            case 1:
                            default: pageParameter.MapName = MapName.RadientFlatlands;     break;
                        }

                        pageParameter.maxUnitsPerPlayer = maxUnitSlider.Value;

                        Player me = null;
                        if (team1.Exists(username))
                            me = team1.GetPlayer(username);
                        if (team2.Exists(username))
                            me = team2.GetPlayer(username);
                        pageParameter.me = me;
                        
                        pageParameter.team1 = team1;
                        pageParameter.team2 = team2;

                        pageParameter.chatManager = chatManager;
                        
                        PlatformMenuAdapter.GameLobbyMenu_StartGame_Click(pageParameter);
                    }
                );
        }

        #region GameLobby Events
        private void JoinTeam1_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!enableChangingTeams) return;

            Debug.WriteLine("JoinTeam1 Tapped");
            if (team1.Exists(playerID)) return;

            if (Consts.Game.NetworkManager.HostSettings == HostType.Client &&
                pageparam != GameConnectionType.Singleplayer)
            {
                SendJoinTeam1();
            }
            else
            {
                Player me = team2.GetPlayer(playerID);
                team2.RemovePlayer(playerID);
                team1.AddToTeam(me);

                team1ListBox.ItemsSource = null;
                team1ListBox.ItemsSource = team1.Players;

                team2ListBox.ItemsSource = null;
                team2ListBox.ItemsSource = team2.Players;

                if (Consts.Game.NetworkManager.HostSettings == HostType.Host)
                {
                    SendTeamsChanged();
                }
            }
        }
        private void JoinTeam2_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!enableChangingTeams) return;

            Debug.WriteLine("JoinTeam2 Tapped");
            if (team2.Exists(playerID)) return;
            
            if (Consts.Game.NetworkManager.HostSettings == HostType.Client && 
                pageparam != GameConnectionType.Singleplayer)
            {
                SendJoinTeam2();
            }
            else
            {
                Player me = team1.GetPlayer(playerID);
                team1.RemovePlayer(playerID);
                team2.AddToTeam(me);

                team1ListBox.ItemsSource = null;
                team1ListBox.ItemsSource = team1.Players;

                team2ListBox.ItemsSource = null;
                team2ListBox.ItemsSource = team2.Players;

                if (Consts.Game.NetworkManager.HostSettings == HostType.Host)
                {
                    SendTeamsChanged();
                }
            }
        }
        

        private void Team1SelectionChanged(object sender, TappedRoutedEventArgs e)
        {
            //Frame.Navigate(typeof(EmpireSelectionPage));

            XamlControlHelper.LoseFocusOnTextBox(team1ListBox);
            team1ListBox.SelectedItem = null;
        }
        private void Team2SelectionChanged(object sender, TappedRoutedEventArgs e)
        {
            //Frame.Navigate(typeof(EmpireSelectionPage));

            XamlControlHelper.LoseFocusOnTextBox(team2ListBox);
            team2ListBox.SelectedItem = null;
        }

        private void gameStartButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (gameStarting)
            {
                gameStartButton.Content = "Start";
                
                SetAbilities();
                gameStarting = false;

                if (Consts.Game.NetworkManager.HostSettings == HostType.Host)
                {
                    SendCancelStartGame();
                }
                return;
            }
            
            bool returnEarly = false;
            
            // If the Game Mode or Map is set to random, set them now
            if (gameModeSelector.SelectedIndex == 0)
            {
                gameModeSelector.SelectedItem = null;
                gameModeSelector.SelectedIndex = 0;
                returnEarly = true;
            }

            if (mapSelector.SelectedIndex == 0)
            {
                mapSelector.SelectedItem = null;
                mapSelector.SelectedIndex = 0;
                returnEarly = true;
            }

            if (returnEarly) { return; }

            // Set the controls
            gameStartButton.Content = "Cancel";
            
            mapSelector.IsEnabled = false;
            gameModeSelector.IsEnabled = false;
            gameStarting = true;

            if (pageparam == GameConnectionType.Singleplayer)
            {
                StartGame();
                return;
            }

            // If we are good to start, send the start command
            if (Consts.Game.NetworkManager.HostSettings == HostType.Host)
            {
                SendStartGame();
            }
        }
        #endregion

        #region Game Mode Events
        private void GameModeSelector_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (gameModeSelector == null) { return; }
            if (gameModeSelector.SelectedItem == null) { return; }

            switch (gameModeSelector.SelectedIndex)
            {
                case 0:
                    int index = Consts.random.Next(1, totalGameModes);
                    gameModeSelector.SelectedIndex = index;
                    return;
                case 1: //<sys:String>1v1</sys:String>
                    break;
                default:
                    break;
            }

            if (Consts.Game.NetworkManager.HostSettings == HostType.Host)
            {
                SendGameModeChanged();
            }
        }

        private void MaxUnitSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (maxUnitSlider == null) return;
            maxUnitSlider.Header = "Max Units Per Player: " + maxUnitSlider.Value;

            if (Consts.Game.NetworkManager.HostSettings == HostType.Host)
            {
                SendMaxUnitsChanged();
            }
        }
        
        private void MapSelector_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (mapSelector == null) { return; }
            if (mapSelector.SelectedItem == null) { return; }

            switch (mapSelector.SelectedIndex)
            {
                case 0:
                    int index = Consts.random.Next(1, totalMaps);
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

            if (Consts.Game.NetworkManager.HostSettings == HostType.Host)
            {
                SendMapChanged();
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
            if (e.Handled) return;

//#if WINDOWS_PHONE_APP
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                string chatText = ("" + chatTextBox.Text);
                SendMessage(chatText);
                e.Handled = true;
            }
//#endif

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
            if (string.IsNullOrWhiteSpace(msg)) return;

            Debug.WriteLine("SendMessage(" + msg + ")");

            // Send the Message
            ChatMessage chatMessage = chatManager.AddMessage(username, msg);
            ChatPacket packet = new ChatPacket(true, chatMessage);
            string packetSerialized = packet.ThisToJson();

            try
            {
                Consts.Game.NetworkManager.SendMessage(packetSerialized);
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
