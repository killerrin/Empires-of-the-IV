using EmpiresOfTheIV.Game.Enumerators;
using KillerrinStudiosToolkit.Converters;
using MonoGame.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace EmpiresOfTheIV
{
    public partial class MainPage
    {
        #region Fields/Properties
        public static MainPage Current { get; private set; }
        //readonly EmpiresOfTheIVGame m_game;

        public static Rectangle MonoGamePageDim { get; private set; }
        public static Frame PageFrame { get; private set; }
        public static MediaElement CortanaMediaElement { get; private set; }
        #endregion



        #region Events
        private void PageFrame_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("PageFrame: Frame Loaded");
            PageFrame = (sender as Frame);

            while (true) {
                if (Consts.Game == null) continue;
                if (Consts.Game.GameManager == null) continue;
                if (Consts.Game.GameManager.StateManager == null) continue;
            
                Consts.Game.GameManager.StateManager.Navigate(GameState.SplashScreen);
                break;
            }
        }

        private void CortanaElement_Loaded(object sender, RoutedEventArgs e)
        {
            CortanaMediaElement = (sender as MediaElement);
        }

        private void MonoGamePageDim_Loaded(object sender, RoutedEventArgs e)
        {
            MonoGamePageDim = (sender as Rectangle);
        }
        #endregion
    }
}
