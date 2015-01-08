using KillerrinStudiosToolkit.Converters;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace EmpiresOfTheIV
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SplashScreenPage : Page
    {
        private static Grid m_splashScreenGrid;

        public SplashScreenPage()
        {
            this.InitializeComponent();
        }

        public static void ToggleSplashScreen()
        {
            if (m_splashScreenGrid == null) return;

            bool currentVisibility = BooleanToVisibilityConverter.ConvertToBoolean(m_splashScreenGrid.Visibility);
            m_splashScreenGrid.Visibility = BooleanToVisibilityConverter.ConvertToVisibility(!currentVisibility);
        }

        private void SplashScreenGrid_Loaded(object sender, RoutedEventArgs e)
        {
            m_splashScreenGrid = (sender as Grid);
        }

    }
}
