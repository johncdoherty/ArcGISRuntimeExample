using Esri.ArcGISRuntime.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ArcGISRuntimeExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // TODO: Confirm that this shouldn't be a handler on the data context changed event
                if (this.DataContext != null)
                {

                    MapViewModel mvm = this.DataContext as MapViewModel;
                    await mvm?.InitializeMap(this.MapViewMain);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void MapViewMain_DrawStatusChanged(object sender, DrawStatusChangedEventArgs e)
        {
            if (this.ActivityIndicator != null)
            {
                //// Update the load status information - only necessary if not on UI thread
                //Dispatcher.Invoke(delegate ()
                //{
                // Show the activity indicator if the map is drawing
                if (e.Status == DrawStatus.InProgress)
                {
                    this.ActivityIndicator.IsEnabled = true;
                    this.ActivityIndicator.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    this.ActivityIndicator.IsEnabled = false;
                    this.ActivityIndicator.Visibility = System.Windows.Visibility.Collapsed;
                }
                //});
            }
        }

        // Map initialization logic is contained in MapViewModel.cs
    }

}
