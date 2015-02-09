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

        public bool IsConnected { get; private set; }

        public BluetoothConnectionHelper BluetoothConnectionHelper { get; protected set; }
        public LANHelper LanHelper { get; protected set; }

        public HostType HostSettings { get; set; }

        public NetworkStage CurrentNetworkStage { get; protected set; }
        public NetworkType NetworkType { get; protected set; }

        private object lockObject = new object();
        #endregion

        public NetworkManager(EmpiresOfTheIVGame game)
        {
            m_game = game;

            // Initialize us to None for later parsing
            HostSettings = HostType.Client;
            NetworkType = KillerrinStudiosToolkit.Enumerators.NetworkType.None;

            IsConnected = false;

            // Subscribe to myself to handle specific setup code
            OnMessageRecieved += NetworkManager_OnMessageRecieved;

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
        public event EventHandler OnFailedToConnect;
        public event EventHandler OnDisconnected;

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
        public async void StartListening(NetworkType networkType)
        {
            if (NetworkType != NetworkType.None) { Debug.WriteLine("Please Disconnect your connection first"); return; }

            HostSettings = HostType.Host;
            NetworkType = networkType;
            CurrentNetworkStage = NetworkStage.InLobby;

            if (NetworkType == NetworkType.LAN)
            {
                LanHelper.StartUDPListening(NetworkPort);
            }
            else if (NetworkType == NetworkType.Bluetooth)
            {

            }
        }

        /// <summary>
        /// Starts a connection with the specified settings
        /// </summary>
        /// <param name="networkType">The type of connection we wish to connect over</param>
        /// <param name="IP">The IP to connect to. Null if using Bluetooth</param>
        /// <param name="handshakeConnection">True if connecting to a host, False if responding to a connection request with a client</param>
        public async void Connect(NetworkType networkType, string IP, bool handshakeConnection = true)
        {
            if (NetworkType != NetworkType.None && IsConnected) { Debug.WriteLine("Please Disconnect your connection first"); return; }
            
            NetworkType = networkType;
            CurrentNetworkStage = NetworkStage.HandshakingConnection;
            
            if (NetworkType == NetworkType.LAN)
            {
                LanHelper.ConnectUDP(new NetworkConnectionEndpoint(IP, NetworkPort));
            }
            else if (NetworkType == NetworkType.Bluetooth)
            {

            }

            if (handshakeConnection)
            {
                SendMessage(StaticNetworkMessages.AttemptingToConnect);
            }
        }

        public void Disconnect()
        {
            if (NetworkType == NetworkType.LAN)
            {
                LanHelper.SendUDPCloseMessage();
                LanHelper.Reset();
            }
            else if (NetworkType == NetworkType.Bluetooth)
            {

            }

            NetworkType = NetworkType.None;
            CurrentNetworkStage = NetworkStage.None;
            IsConnected = false;

            if (OnDisconnected != null)
                OnDisconnected(null, null);
        }

        void LanHelper_OnTCPConnected(object sender, OnConnectedEventArgs e)
        {
            if (OnConnected != null)
                OnConnected(this, e);
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

        /// Used to get the initial handshake
        void NetworkManager_OnMessageRecieved(object sender, ReceivedMessageEventArgs e)
        {
            if (IsConnected) return;

            if (HostSettings == HostType.Client)
            {
                if (e.NetworkType == NetworkType.LAN)
                {
                    if (e.Message == StaticNetworkMessages.AttemptingToConnect)
                    {
                        Debug.WriteLine("Attempt to Connect Recieved: Sending Acknoledgement");
                        SendMessage(StaticNetworkMessages.Acknowledge);
                    }
                    else if (e.Message == StaticNetworkMessages.Acknowledge)
                    {
                        Debug.WriteLine("Connected To: " + e.NetworkConnectionEndpoint.Value.ToString());
                        IsConnected = true;
                    }
                }

                // Switch the Lobby
                CurrentNetworkStage = NetworkStage.InLobby;
            }
            else if (HostSettings == HostType.Host)
            {
                if (e.NetworkType == NetworkType.LAN)
                {
                    if (e.Message == StaticNetworkMessages.AttemptingToConnect)
                    {
                        Debug.WriteLine("Connection Request at: " + e.NetworkConnectionEndpoint.Value.ToString() + " - Attempting to Connect ... :");
                        Connect(NetworkType.LAN, e.NetworkConnectionEndpoint.Value.HostNameAsString, true);
                    }
                    else if (e.Message == StaticNetworkMessages.Acknowledge)
                    {
                        Debug.WriteLine("Connected to: " + e.NetworkConnectionEndpoint.Value.ToString());
                        SendMessage(StaticNetworkMessages.Acknowledge);
                        IsConnected = true;
                    }
                }
            }

            if (IsConnected && OnConnected != null)
                OnConnected(this, new OnConnectedEventArgs(e.NetworkConnectionEndpoint, e.NetworkType));
        }
    }
}
