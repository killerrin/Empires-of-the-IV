using EmpiresOfTheIV.Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
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
    public sealed partial class MainMenuPage : Page
    {
        public MainMenuPage()
        {
            // Landscape and upside down landscape only
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape |
                                                         DisplayOrientations.LandscapeFlipped;

            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Consts.Game.StateManager.HandleBackButtonPressed = true;
            base.OnNavigatedTo(e);
        }

        private void SingleplayerButton_Click(object sender, RoutedEventArgs e)
        {
            PlatformMenuAdapter.MainMenu_SingleplayerButton_Click();
        }

        private void BluetoothMultiplayerButton_Click(object sender, RoutedEventArgs e)
        {
            PlatformMenuAdapter.MainMenu_BluetoothMultiplayerButton_Click();
        }

        private void LanMultiplayerButton_Click(object sender, RoutedEventArgs e)
        {
            PlatformMenuAdapter.MainMenu_LANMultiplayerButton_Click();
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            PlatformMenuAdapter.MainMenu_OptionsButton_Click();
        }

        private void CreditsButton_Click(object sender, RoutedEventArgs e)
        {
            PlatformMenuAdapter.MainMenu_CreditsButton_Click();
        }
    }
}
