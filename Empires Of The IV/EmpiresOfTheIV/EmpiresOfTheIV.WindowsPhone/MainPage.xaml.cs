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

            // Before we create the game, we need to disable the status bar
            var statusBar = StatusBar.GetForCurrentView();
            statusBar.HideAsync();

            // Finally, Create the game. 
            if (Consts.Game == null)
                Consts.Game = XamlGame<EmpiresOfTheIVGame>.Create(launchArguments, Window.Current.CoreWindow, this);
        }
    }
}
