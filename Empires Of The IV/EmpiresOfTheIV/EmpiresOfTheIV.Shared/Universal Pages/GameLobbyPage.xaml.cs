using System;
using System.Collections.Generic;
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
    public sealed partial class GameLobbyPage : Page
    {
        ListBox team1ListBox;
        ListBox team2ListBox;

        public GameLobbyPage()
        {
            //this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;
            this.InitializeComponent();
            Debug.WriteLine("GameLobbyPage loaded");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Consts.Game.GameManager.StateManager.HandleBackButtonPressed = false;
            base.OnNavigatedTo(e);
        }

        #region GameLobby Events
        #region Loaded Events
        private void Team1ListBoxLoaded(object sender, RoutedEventArgs e)
        {
            //team1ListBox = (sender as ListBox);
        }

        private void Team2ListBoxLoaded(object sender, RoutedEventArgs e)
        {
            //team2ListBox = (sender as ListBox);
        }
        #endregion

        private void gameStartButton_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        #endregion

        private void Team1SelectionChanged(object sender, TappedRoutedEventArgs e)
        {
            //Frame.Navigate(typeof(EmpireSelectionPage));
        }
        private void Team2SelectionChanged(object sender, TappedRoutedEventArgs e)
        {

        }
        #region Chat Events

        #endregion
    }
}
