using EmpiresOfTheIV.Data_Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class BluetoothMultiplayerPage : Page
    {
        public BluetoothMultiplayerPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ObservableCollection<NameDescription> devices = new ObservableCollection<NameDescription>();
            devices.Add(new NameDescription("Lumia", ""));
            devices.Add(new NameDescription("Surface", "192.168.0.1"));
            connectionListBox.ItemsSource = devices;

            base.OnNavigatedTo(e);
        }

        private void BeginSearchButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BluetoothSettingsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ConnectionListBox_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }
    }
}
