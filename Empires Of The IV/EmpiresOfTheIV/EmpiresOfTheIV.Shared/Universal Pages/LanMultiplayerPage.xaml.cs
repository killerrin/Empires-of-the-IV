using KillerrinStudiosToolkit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
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
    public sealed partial class LanMultiplayerPage : Page
    {
        bool m_attemptingToConnect;
        public LanMultiplayerPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Consts.Game.GameManager.NetworkManager.OnConnected += NetworkManager_OnConnected;
            
            Consts.Game.GameManager.StateManager.HandleBackButtonPressed = false;
            Consts.Game.GameManager.StateManager.OnBackButtonPressed += StateManager_OnBackButtonPressed;
            
            // Set some variables
            try
            {
                myIP.Text = "Your IP: " + LANHelper.CurrentIPAddressAsString();
            }
            catch (Exception ex) { Debug.WriteLine(ex.PrintException("Showing IP")); }


            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Debug.WriteLine("Navigating From LAN Page");
            Consts.Game.GameManager.NetworkManager.OnConnected -= NetworkManager_OnConnected;

            Consts.Game.GameManager.StateManager.HandleBackButtonPressed = true;
            Consts.Game.GameManager.StateManager.OnBackButtonPressed -= StateManager_OnBackButtonPressed;
            base.OnNavigatedFrom(e);
        }

        #region Events
        void StateManager_OnBackButtonPressed(object sender, EventArgs e)
        {
            if (m_attemptingToConnect)
            {
                m_attemptingToConnect = false;
                Consts.Game.GameManager.NetworkManager.Disconnect();
                return;
            }
        }

        private void ConnectButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            connectButton.IsEnabled = false;
            ConnectToPlayer();
        }

        private void IPAddressOnEnterEvent(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Debug.WriteLine("Enter Pressed");
                XamlControlHelper.LoseFocusOnTextBox(opponentsIPTextBox);
                opponentsIPTextBox.IsEnabled = false;

                ConnectToPlayer();
            }
        }
        #endregion

        #region Connecting To Player
        public void ConnectToPlayer()
        {
            if (string.IsNullOrEmpty(opponentsIPTextBox.Text)) return;

            try
            {
                m_attemptingToConnect = true;
                XamlControlHelper.ChangeProgressIndicator(progressRing, true);
                Consts.Game.GameManager.NetworkManager.Connect(KillerrinStudiosToolkit.Enumerators.NetworkType.LAN, opponentsIPTextBox.Text);
            }
            catch (Exception) { }
        }


        private object lockobject = new object();
        bool hasConnected = false;
        void NetworkManager_OnConnected(object sender, KillerrinStudiosToolkit.Events.OnConnectedEventArgs e)
        {
            //if (Monitor.TryEnter(lockobject))
            //{
                try {
                    if (hasConnected) { return; }
                    hasConnected = true;

                    CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        m_attemptingToConnect = false;
                        XamlControlHelper.ChangeProgressIndicator(progressRing, false);
                        PlatformMenuAdapter.LanMultiplayerMenu_ConnectButton_Click();
                    });
                }
                finally {
                    //Monitor.Exit(lockobject);
                }
            //}
            //else {
            //    return;
            //}
        }

        #endregion
    }
}
