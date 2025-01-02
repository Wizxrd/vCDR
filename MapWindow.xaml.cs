using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using vCDR.src;

namespace vCDR
{
    /// <summary>
    /// Interaction logic for MapWindow.xaml
    /// </summary>
    public partial class MapWindow : Window
    {
        public MapWindow()
        {
            InitializeComponent();
            string mapHtmlPath = Path.Combine("D:\\vCDR\\Database", "map.html");
            MapWebView.Source = new Uri(mapHtmlPath);

            MapWebView.Loaded += async (sender, e) =>
            {   
                //string geoJsonFilePath = Path.Combine("D:\\vCDR\\Database", "boundaries.geojson");
                //var features = await LoadGeoJsonFeatures(geoJsonFilePath);
                //await AddLinesToMap(features);
            };
        }

        public async Task<List<List<double[]>>> LoadGeoJsonFeatures(string filePath)
        {
            var features = new List<List<double[]>>();

            try
            {
                string geoJson = File.ReadAllText(filePath);
                var geoJsonObject = JsonConvert.DeserializeObject<GeoJson>(geoJson);
                foreach (var feature in geoJsonObject.Features)
                {
                    if (feature.Geometry.Type == "LineString" && feature.Geometry.Coordinates != null)
                    {
                        var coordinates = new List<double[]>();

                        foreach (var coord in feature.Geometry.Coordinates)
                        {
                            double longitude = coord[0];
                            double latitude = coord[1];
                            coordinates.Add(new double[] { longitude, latitude });
                        }

                        features.Add(coordinates);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading GeoJSON: {ex.Message}");
            }
            return features;
        }

        public class GeoJson
        {
            public string Type { get; set; }
            public List<GeoJsonFeature> Features { get; set; }
        }

        public class GeoJsonFeature
        {
            public string Type { get; set; }
            public GeoJsonGeometry Geometry { get; set; }
        }

        public class GeoJsonGeometry
        {
            public string Type { get; set; }

            // Change this to List<List<double>> to List<double[]> if needed
            public List<double[]> Coordinates { get; set; }
        }


        private void TitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void MinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                MaximizeButton.ToolTip = "Maximize";
                MapWindowGrid.Margin = new Thickness(0);
                MaximizeButton.Tag = "pack://application:,,,/Images/Maximize.png";
                this.WindowState = WindowState.Normal;
            }
            else
            {
                MaximizeButton.ToolTip = "Restore";
                MapWindowGrid.Margin = new Thickness(8, 8, 8, 0);
                MaximizeButton.Tag = "pack://application:,,,/Images/MaximizeMinimize.png";
                this.WindowState = WindowState.Maximized;
            }
        }

        private void WindowStateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                MaximizeButton.ToolTip = "Restore";
                MapWindowGrid.Margin = new Thickness(8, 8, 8, 0);
                MaximizeButton.Tag = "pack://application:,,,/Images/MaximizeMinimize.png";
            }
            else
            {
                MaximizeButton.ToolTip = "Maximize";
                MapWindowGrid.Margin = new Thickness(0);
                MaximizeButton.Tag = "pack://application:,,,/Images/Maximize.png";
            }
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
    }
}
