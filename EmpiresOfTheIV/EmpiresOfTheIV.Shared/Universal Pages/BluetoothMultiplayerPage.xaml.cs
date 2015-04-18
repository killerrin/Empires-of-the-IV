using KillerrinStudiosToolkit;
using KillerrinStudiosToolkit.Enumerators;
using KillerrinStudiosToolkit.Data_Models;
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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace EmpiresOfTheIV
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BluetoothMultiplayerPage : Page
    {
        public BluetoothMultiplayerPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Consts.Game.NetworkManager.ConnectionStatusChanged += BluetoothConnectionHelper_ConnectionStatusChanged;
            Consts.Game.NetworkManager.OnMessageRecieved += BluetoothConnectionHelper_MessageRecieved;
            Consts.Game.NetworkManager.PeersFound += BluetoothConnectionHelper_PeersFound;
            
            ObservableCollection<NameDescription> devices = new ObservableCollection<NameDescription>();
            devices.Add(new NameDescription("Lumia", ""));
            devices.Add(new NameDescription("Surface", "192.168.0.1"));
            connectionListBox.ItemsSource = devices;
            
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //Consts.Game.GameManager.NetworkManager.ConnectionStatusChanged -= BluetoothConnectionHelper_ConnectionStatusChanged;
            //Consts.Game.GameManager.NetworkManager.OnMessageRecieved -= BluetoothConnectionHelper_MessageRecieved;
            //Consts.Game.GameManager.NetworkManager.PeersFound -= BluetoothConnectionHelper_PeersFound;
            base.OnNavigatedFrom(e);
        }

        #region Control Events
        private void BeginSearchButton_Click(object sender, RoutedEventArgs e)
        {
            //NetworkConnectionState currentConnectionState = Consts.BluetoothNetworkAdapter.BluetoothConnectionHelper.NetworkConnectionStatus;
            //if (currentConnectionState == NetworkConnectionState.NotSearching)
            //{
            //    Consts.BluetoothNetworkAdapter.StartSearchBluetooth();
            //    try { beginSearchButton.Content = "Stop Searching"; }
            //    catch (Exception) { }
            //}
            //else if (currentConnectionState == NetworkConnectionState.Searching)
            //{
            //    Consts.BluetoothNetworkAdapter.StopSearch();
            //    try { beginSearchButton.Content = "Begin Searching"; }
            //    catch (Exception) { }
            //}
        }

        private void BluetoothSettingsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ConnectionListBox_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
        #endregion

        #region Bluetooth Events
        void BluetoothConnectionHelper_MessageRecieved(object sender, KillerrinStudiosToolkit.Events.ReceivedMessageEventArgs args)
        {
        }

        void BluetoothConnectionHelper_ConnectionStatusChanged(object sender, Windows.Networking.Proximity.TriggeredConnectState args)
        {
        }

        void BluetoothConnectionHelper_PeersFound(object sender, IEnumerable<Windows.Networking.Proximity.PeerInformation> args)
        {
            //Peers.Clear();
            //args.ForEach(Peers.Add);
            //if (Peers.Count > 0)
            //{
            //    SelectedPeer = Peers.First();
            //}
            //else
            //{
            //    ConnectMessages.Add("No contacts found");
            //    Reset();
            //}
        }
        #endregion
    }
}
