using EmpiresOfTheIV.Game.Enumerators;
using KillerrinStudiosToolkit;
using KillerrinStudiosToolkit.Datastructures;
using KillerrinStudiosToolkit.Enumerators;
using KillerrinStudiosToolkit.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.Proximity;

namespace EmpiresOfTheIV.Game
{
    public class NetworkManager
    {
        public static class StaticNetworkMessages
        {
            public static string AttemptingToConnect = "Attempting to Connect";
            public static string Ping = "Ping";
            public static string Acknowledge = "ACK";
        }

        protected EmpiresOfTheIVGame m_game;
        public EmpiresOfTheIVGame Game { get { return m_game; } protected set { m_game = value; } }

        #region Fields/Properties
        public readonly Guid UniqueNetworkGUID = new Guid("20BB9225-AC4B-4ACD-9B12-126D8EBCF2D6");
        public readonly string NetworkPort = "27135";

        public BluetoothConnectionHelper BluetoothConnectionHelper { get; protected set; }
        public LANHelper LanHelper { get; protected set; }

        public NetworkStage CurrentNetworkStage { get; protected set; }
        public NetworkType NetworkType { get; protected set; }

        private object lockObject = new object();
        #endregion

        public NetworkManager(EmpiresOfTheIVGame game)
        {
            m_game = game;

            // Initialize us to None for later parsing
            NetworkType = KillerrinStudiosToolkit.Enumerators.NetworkType.None;

            // Setup the Helpers
            LanHelper = new LANHelper(UniqueNetworkGUID);
            LanHelper.OnTCPConnected += LanHelper_OnTCPConnected;
            LanHelper.TCPMessageRecieved += LanHelper_TCPMessageRecieved;
            LanHelper.UDPMessageRecieved += LanHelper_UDPMessageRecieved;

            //BluetoothConnectionHelper = new BluetoothConnectionHelper(UniqueNetworkGUID.ToString());
            //BluetoothConnectionHelper.ConnectCrossPlatform = true;
            //BluetoothConnectionHelper.ConnectionStatusChanged += BluetoothConnectionHelper_ConnectionStatusChanged;
            //BluetoothConnectionHelper.MessageReceived += BluetoothConnectionHelper_MessageRecieved;
            //BluetoothConnectionHelper.PeersFound += BluetoothConnectionHelper_PeersFound;
        }

        #region Events
        public event OnConnectedEventHandler OnConnected;
        public event ReceivedMessageEventHandler OnMessageRecieved;
        public event TypedEventHandler<object, TriggeredConnectState> ConnectionStatusChanged;
        public event TypedEventHandler<object, IEnumerable<PeerInformation>> PeersFound;

        #region Other Events
        private void BluetoothConnectionHelper_ConnectionStatusChanged(object sender, TriggeredConnectState args)
        {

        }

        private void BluetoothConnectionHelper_PeersFound(object sender, IEnumerable<PeerInformation> args)
        {

        }
        #endregion

        #region Message Recieved
        private void BluetoothConnectionHelper_MessageRecieved(object sender, ReceivedMessageEventArgs args)
        {
            if (OnMessageRecieved != null)
                OnMessageRecieved(this, args);
        }
        
        void LanHelper_TCPMessageRecieved(object sender, ReceivedMessageEventArgs e)
        {
            if (OnMessageRecieved != null)
                OnMessageRecieved(this, e);
        }

        void LanHelper_UDPMessageRecieved(object sender, ReceivedMessageEventArgs e)
        {
            if (OnMessageRecieved != null)
                OnMessageRecieved(this, e); 
        }
        #endregion
        #endregion

        #region Connection
        /// <summary>
        /// Starts a connection with the specified settings
        /// </summary>
        /// <param name="networkType">The type of connection we wish to connect over</param>
        /// <param name="IP">The IP to connect to. Null if using Bluetooth</param>
        public async void Connect(NetworkType networkType, string IP)
        {
            if (NetworkType != NetworkType.None) { Debug.WriteLine("Please Disconnect your connection first"); return; }
            
            NetworkType = networkType;
            CurrentNetworkStage = NetworkStage.HandshakingConnection;

            // Subscribe to myself to handle specific setup code
            OnMessageRecieved += NetworkManager_OnMessageRecieved;
            
            if (NetworkType == NetworkType.LAN)
            {
                LanHelper.ConnectUDP(new NetworkConnectionEndpoint(IP, NetworkPort));
                BeginUDPHandshake();
            }
            else if (NetworkType == NetworkType.Bluetooth)
            {

            }
        }

        public void Disconnect()
        {
            if (NetworkType == NetworkType.None) return;

            if (NetworkType == NetworkType.LAN)
            {
                LanHelper.SendUDPCloseMessage();
                LanHelper.Reset();
                CurrentNetworkStage = NetworkStage.None;
            }
            else if (NetworkType == NetworkType.Bluetooth)
            {

            }
        }

        void LanHelper_OnTCPConnected(object sender, OnConnectedEventArgs e)
        {
            if (OnConnected != null)
                OnConnected(this, e);
        }

        async Task BeginUDPHandshake()
        {
            bool connectionFound = false;
            while (true)
            {
                if (!LanHelper.IsUDPConnected) continue;
                //lock (lockObject)
                //{
                    if (CurrentNetworkStage != NetworkStage.HandshakingConnection)
                    {
                        connectionFound = true;
                        break;
                    }
                //}
            
                SendMessage(StaticNetworkMessages.AttemptingToConnect);
                await Task.Delay(TimeSpan.FromSeconds(0.25));
            }

            if (connectionFound) { }
        }
        #endregion

        #region Sending
        public void SendMessage(string message)
        {
            if (NetworkType == NetworkType.None) return;

            if (NetworkType == NetworkType.LAN)
            {
                LanHelper.SendUDPMessage(message);
            }
            else if (NetworkType == NetworkType.Bluetooth)
            {

            }
        }
        #endregion

        // Used to get the initial handshake
        void NetworkManager_OnMessageRecieved(object sender, ReceivedMessageEventArgs e)
        {
            lock (lockObject)
            {
                if (e.Message != StaticNetworkMessages.AttemptingToConnect) return;
            
                if (e.NetworkType == NetworkType.LAN)
                {
                    // Double send it again to make sure
                    for (int i = 0; i < 5; i++)
                        SendMessage(StaticNetworkMessages.AttemptingToConnect);
                }
                else if (e.NetworkType == NetworkType.Bluetooth)
                {

                }

                // Switch the Lobby
                CurrentNetworkStage = NetworkStage.InLobby;

                // Describe so we no longer get events
                OnMessageRecieved -= NetworkManager_OnMessageRecieved;
            }

            if (OnConnected != null)
                OnConnected(this, new OnConnectedEventArgs(e.NetworkConnectionEndpoint, e.NetworkType));
        }
    }
}
