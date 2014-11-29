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
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace EmpiresOfTheIV
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : SwapChainBackgroundPanel
    {
        readonly Game1 m_game;

        private static Frame m_pageFrame;
        public static Frame PageFrame
        {
            get { return m_pageFrame; }
            private set { m_pageFrame = value; }
        }

        private static MediaElement m_cortanaElement;
        public static MediaElement CortanaMediaElement
        {
            get { return m_cortanaElement; }
            private set { m_cortanaElement = value; }
        }


        public GamePage(string launchArguments)
        {
            this.InitializeComponent();

            // Create the game.
            m_game = XamlGame<Game1>.Create(launchArguments, Window.Current.CoreWindow, this);
        }

        private void PageFrame_Loaded(object sender, RoutedEventArgs e)
        {
            PageFrame = (sender as Frame);
        }

        private void cortanaElement_Loaded(object sender, RoutedEventArgs e)
        {
            CortanaMediaElement = (sender as MediaElement);
        }
    }
}
