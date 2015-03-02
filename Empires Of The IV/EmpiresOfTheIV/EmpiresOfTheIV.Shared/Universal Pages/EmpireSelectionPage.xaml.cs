using KillerrinStudiosToolkit.Data_Models;
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
    public sealed partial class EmpireSelectionPage : Page
    {
        ObservableCollection<NameImageDescription> m_empires;

        public EmpireSelectionPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            m_empires = new ObservableCollection<NameImageDescription>();
            m_empires.Add(new NameImageDescription("Unanian Empire", new Uri("ms-appx:///Assets/Empire Flags/Unanian Flag.png", UriKind.Absolute), "UNANIANDESCRIPTION"));
            m_empires.Add(new NameImageDescription("Crescanian Confederacy", new Uri("ms-appx:///Assets/Empire Flags/Crescanian Confederation Flag.png", UriKind.Absolute), "CRESCANIANDESCRIPTION"));
            m_empires.Add(new NameImageDescription("Kingdom of Edolas", new Uri("ms-appx:///Assets/Empire Flags/The Kingdom Of Edolas Flag.png", UriKind.Absolute), "EDOLASDESCRIPTION"));
            empireSelection_GridView.ItemsSource = m_empires;
        }

        private void EmpireSelection_GridView_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void EmpireSelection_GridView_Clicked(object sender, ItemClickEventArgs e)
        {

        }
    }
}
