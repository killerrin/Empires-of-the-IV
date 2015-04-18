using EmpiresOfTheIV.Game.Menus.PageParameters;
using KillerrinStudiosToolkit.Helpers;
using KillerrinStudiosToolkit;
using EmpiresOfTheIV.Game.Players;
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
using System.Diagnostics;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace EmpiresOfTheIV
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GameOverPage : Page
    {
        public GameOverPageParameter PageParameter;

        public GameOverPage()
        {
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;
            this.InitializeComponent();
            Loaded += GameOverPage_Loaded;
        }

        void GameOverPage_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("GameOverPage Loaded");
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PageParameter = ((GameOverPageParameter)e.Parameter);
            base.OnNavigatedTo(e);

            EmpiresOfTheIVGame.Instance.StateManager.HandleBackButtonPressed = true;
            EmpiresOfTheIVGame.Instance.NetworkManager.Disconnect(false);
        }

        private void okButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PlatformMenuAdapter.GameOverMenu_OK_Click();
        }

        private void winningTeam_Loaded(object sender, RoutedEventArgs e)
        {
            if (PageParameter.team1.Winner && PageParameter.team2.Winner)
            {
                winningTeam.Text = "DRAW";
            }
            else if (PageParameter.team1.Winner)
            {
                winningTeam.Text = (PageParameter.team1.TeamID.ToString().AddSpacesToSentence() + " Won").ToUpper();
            }
            else if (PageParameter.team2.Winner)
            {
                winningTeam.Text = (PageParameter.team2.TeamID.ToString().AddSpacesToSentence() + " Won").ToUpper();
            }
        }
    }
}
