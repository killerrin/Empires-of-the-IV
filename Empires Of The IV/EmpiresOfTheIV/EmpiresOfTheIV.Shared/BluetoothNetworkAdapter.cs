using KillerrinStudiosToolkit;
using KillerrinStudiosToolkit.Enumerators;
using KillerrinStudiosToolkit.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Windows.Foundation;
using Windows.Networking.Proximity;

namespace EmpiresOfTheIV
{
    public class BluetoothNetworkAdapter
    {
        public readonly BluetoothConnectionHelper BluetoothConnectionHelper;

        public event TypedEventHandler<object, TriggeredConnectState> ConnectionStatusChanged;
        public event TypedEventHandler<object, ReceivedMessageEventArgs> MessageReceived;
        public event TypedEventHandler<object, IEnumerable<PeerInformation>> PeersFound;

        #region Helper Properties
        public bool ConnectCrossPlatform
        {
            get { return BluetoothConnectionHelper.ConnectCrossPlatform; }
            set { BluetoothConnectionHelper.ConnectCrossPlatform = value; }
        }
        #endregion

        public BluetoothNetworkAdapter()
        {
            BluetoothConnectionHelper = new BluetoothConnectionHelper(Consts.UniqueNetworkGUID.ToString());
            BluetoothConnectionHelper.ConnectCrossPlatform = true;

            // Connection Status Changed
            BluetoothConnectionHelper.ConnectionStatusChanged += BluetoothConnectionHelper_ConnectionStatusChanged;
            
            // Messages Recieved
            BluetoothConnectionHelper.MessageReceived += BluetoothConnectionHelper_MessageRecieved;
            
            // Peers Found
            BluetoothConnectionHelper.PeersFound += BluetoothConnectionHelper_PeersFound;
        }

        public void StartSearchBluetooth(string advertiseName = null) { BluetoothConnectionHelper.Start(ConnectMethod.Browse, advertiseName); }

        public void StartSearchNFC(string advertiseName = null) { BluetoothConnectionHelper.Start(ConnectMethod.Tap, advertiseName); }

        public void StopSearch() { BluetoothConnectionHelper.Reset(); }

        private void BluetoothConnectionHelper_ConnectionStatusChanged(object sender, TriggeredConnectState args)
        {
            Debug.WriteLine(args.ToString());

            try
            {
                if (ConnectionStatusChanged != null)
                    ConnectionStatusChanged(sender, args);
            }
            catch (Exception) { }
        }
        
        private void BluetoothConnectionHelper_MessageRecieved(object sender, ReceivedMessageEventArgs args)
        {
            try { 
                if (MessageReceived != null)
                    MessageReceived(sender, args);
            }
            catch (Exception) { }
        }

        private void BluetoothConnectionHelper_PeersFound(object sender, IEnumerable<PeerInformation> args)
        {
            foreach (var peer in args)
            {
                Debug.WriteLine("DisplayName: "+ peer.DisplayName.ToString() + " | " +
                                "ID: " + peer.Id.ToString());
            }

            try
            {
                if (PeersFound != null)
                    PeersFound(sender, args);
            }
            catch (Exception) { }
        }
    }
}
