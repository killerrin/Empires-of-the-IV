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
            // LAN
            LanHelper = new LANHelper(UniqueNetworkGUID);
            LanHelper.OnTCPConnected += LanHelper_OnTCPConnected;
            LanHelper.OnTCPDisconnected += LanHelper_OnTCPDisconnected;

            LanHelper.TCPMessageRecieved += LanHelper_TCPMessageRecieved;
            LanHelper.UDPMessageRecieved += LanHelper_UDPMessageRecieved;

            // Bluetooth
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

        #region Proximity Events
        private void BluetoothConnectionHelper_ConnectionStatusChanged(object sender, TriggeredConnectState args)
        {

        }

        private void BluetoothConnectionHelper_PeersFound(object sender, IEnumerable<PeerInformation> args)
        {

        }
        #endregion

        #region Message Recieved Events
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

            Debug.WriteLine("Starting Server");

            HostSettings = HostType.Host;
            NetworkType = networkType;
            CurrentNetworkStage = NetworkStage.InLobby;

            if (NetworkType == NetworkType.LAN)
            {
                LanHelper.StartTCPServer(new NetworkConnectionEndpoint("localhost", NetworkPort));
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
        public async void Connect(NetworkType networkType, string IP)
        {
            if (NetworkType != NetworkType.None && IsConnected) { Debug.WriteLine("Please Disconnect your connection first"); return; }

            Debug.WriteLine("Connecting to Server");

            try
            {
                HostSettings = HostType.Client;
                NetworkType = networkType;
                CurrentNetworkStage = NetworkStage.HandshakingConnection;

                if (NetworkType == NetworkType.LAN)
                {
                    LanHelper.ConnectTCP(new NetworkConnectionEndpoint(IP, NetworkPort));
                }
                else if (NetworkType == NetworkType.Bluetooth)
                {

                }
            }
            catch(Exception)
            {
                HostSettings = HostType.Client;
                NetworkType = NetworkType.NFC;
                CurrentNetworkStage = NetworkStage.None;
            }
        }

        public void Disconnect(bool callDisconnectedEvent = true)
        {
            if (NetworkType == NetworkType.LAN)
            {
                LanHelper.SendTCPCloseMessage();
                LanHelper.Reset();
            }
            else if (NetworkType == NetworkType.Bluetooth)
            {

            }

            HostSettings = HostType.Client;
            NetworkType = NetworkType.None;
            CurrentNetworkStage = NetworkStage.None;
            IsConnected = false;

            if (callDisconnectedEvent)
                LanHelper_OnTCPDisconnected(this, null);
        }

        void LanHelper_OnTCPConnected(object sender, OnConnectedEventArgs e)
        {
            IsConnected = true;

            if (OnConnected != null)
                OnConnected(this, e);
        }

        void LanHelper_OnTCPDisconnected(object sender, EventArgs e)
        {
            Disconnect(false);

            if (OnDisconnected != null)
                OnDisconnected(this, null);
        }
        #endregion

        #region Sending
        public void SendMessage(string message)
        {
            if (NetworkType == NetworkType.None) return;

            if (NetworkType == NetworkType.LAN)
            {
                LanHelper.SendTCPMessage(message);
            }
            else if (NetworkType == NetworkType.Bluetooth)
            {

            }
        }
        #endregion

        void NetworkManager_OnMessageRecieved(object sender, ReceivedMessageEventArgs e)
        {

        }
    }
}
