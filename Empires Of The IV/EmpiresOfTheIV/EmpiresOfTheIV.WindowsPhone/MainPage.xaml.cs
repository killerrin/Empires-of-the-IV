using System;
using System.Collections.Generic;
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
using MonoGame.Framework;
using Windows.UI.ViewManagement;
using Windows.Phone.UI.Input;
using EmpiresOfTheIV.Game;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace EmpiresOfTheIV
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : SwapChainBackgroundPanel
    {
        public MainPage(string launchArguments)
        {
            this.InitializeComponent();

            // Since this is Phone, we need to subscribe to the BackButton
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            
            // Before we create the game, we need to disable the status bar
            var statusBar = StatusBar.GetForCurrentView();
            statusBar.HideAsync();
            
            // Save the Launch Arguments
            Consts.LaunchArguments = launchArguments;

            // Save the Current
            Current = this;

            // Set the Loaded Event
            Loaded += MainPage_Loaded;

            // Create the Game!
            CreateGame();
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            e.Handled = true;
            PlatformMenuAdapter.BackButtonPressed();
        }
    }
}
