using KillerrinStudiosToolkit.Converters;
using MonoGame.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace EmpiresOfTheIV
{
    public partial class MainPage
    {
        #region Fields/Properties
        //readonly EmpiresOfTheIVGame m_game;

        public static Frame PageFrame { get; private set; }
        public static MediaElement CortanaMediaElement { get; private set; }
        #endregion

        #region Events
        private void PageFrame_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("PageFrame: Frame Loaded");
            PageFrame = (sender as Frame);
            PageFrame.Navigate(typeof(MainMenu));
        }

        private void CortanaElement_Loaded(object sender, RoutedEventArgs e)
        {
            CortanaMediaElement = (sender as MediaElement);
        }
        #endregion
    }
}
