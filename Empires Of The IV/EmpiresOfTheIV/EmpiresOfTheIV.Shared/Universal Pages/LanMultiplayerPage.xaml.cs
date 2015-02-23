using EmpiresOfTheIV.Game.Enumerators;
using KillerrinStudiosToolkit;
using KillerrinStudiosToolkit.Enumerators;
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
            Consts.Game.GameManager.StateManager.OnBackButtonPressed += StateManager_OnBackButtonPressed;

            Consts.Game.GameManager.StateManager.HandleBackButtonPressed = true;

            // Set some variables
            m_attemptingToConnect = false;

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
            Consts.Game.GameManager.StateManager.OnBackButtonPressed -= StateManager_OnBackButtonPressed;
            base.OnNavigatedFrom(e);
        }

        void StateManager_OnBackButtonPressed(object sender, EventArgs e)
        {
            if (m_attemptingToConnect)
            {
                Consts.Game.GameManager.NetworkManager.Disconnect();
                m_attemptingToConnect = false;
            }
        }

        #region Host Events
        private async void HostButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Disable any controls here
            hostButton.IsEnabled = false;
            //connectButton.IsEnabled = false;
            //opponentsIPTextBox.IsEnabled = false;

            // Start Listening
            Consts.Game.GameManager.NetworkManager.StartListening(NetworkType.LAN);

            // Describe from Events
            Consts.Game.GameManager.NetworkManager.OnConnected -= NetworkManager_OnConnected;
            Consts.Game.GameManager.StateManager.OnBackButtonPressed -= StateManager_OnBackButtonPressed;

            // Tell the game we're off
            PlatformMenuAdapter.LanMultiplayerMenu_HostButton_Click();
        }
        #endregion


        #region Client Events
        private void ConnectButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //connectButton.IsEnabled = false;
            ConnectToHost();
        }

        private void IPAddressOnEnterEvent(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                Debug.WriteLine("Enter Pressed");
                XamlControlHelper.LoseFocusOnTextBox(opponentsIPTextBox);
                //opponentsIPTextBox.IsEnabled = false;

                ConnectToHost();
            }
        }
        #endregion

        async void ConnectToHost()
        {
            //if (m_attemptingToConnect) return;
            if (string.IsNullOrEmpty(opponentsIPTextBox.Text)) return;
            Debug.WriteLine("Attempting to connect to Host");

            try
            {

                m_attemptingToConnect = true;
                XamlControlHelper.ChangeProgressIndicator(progressRing, true);
                Debug.WriteLine("Progress Indicator Set");
                
                Consts.Game.GameManager.NetworkManager.Connect(NetworkType.LAN, opponentsIPTextBox.Text);
            }
            catch (Exception) { Debug.WriteLine("Exception"); }
        }

        void NetworkManager_OnConnected(object sender, KillerrinStudiosToolkit.Events.OnConnectedEventArgs e)
        {
            Debug.WriteLine("Connected");

            try {
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    m_attemptingToConnect = false;

                    // Disable any controls here
                    XamlControlHelper.ChangeProgressIndicator(progressRing, false);
                    hostButton.IsEnabled = false;
                    connectButton.IsEnabled = false;
                    opponentsIPTextBox.IsEnabled = false;

                    // Describe from Events
                    Consts.Game.GameManager.NetworkManager.OnConnected -= NetworkManager_OnConnected;
                    Consts.Game.GameManager.StateManager.OnBackButtonPressed -= StateManager_OnBackButtonPressed;

                    // Tell the game we're off
                    PlatformMenuAdapter.LanMultiplayerMenu_ConnectButton_Click();
                });
            }
            catch(Exception) { }
        }


        bool ipTextBoxCurrentlyFocused = false;
        private void IPTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("IPAddress TextBox GotFocus");
            ipTextBoxCurrentlyFocused = true;
            openKeyboardButton.Content = "Close Keyboard";
        }
        private void IPTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("IPAddress TextBox LostFocus");
            ipTextBoxCurrentlyFocused = false;

#if WINDOWS_PHONE_APP
            openKeyboardButton.Content = "Open Keyboard";
#endif
        }

        private void IPTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Handled) return;

            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                ConnectToHost();
                e.Handled = true;
            }
        }

        private void OpenKeyboard_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (ipTextBoxCurrentlyFocused)
                XamlControlHelper.LoseFocusOnTextBox(opponentsIPTextBox);
            else
                opponentsIPTextBox.Focus(FocusState.Programmatic);
        }
    }
}
