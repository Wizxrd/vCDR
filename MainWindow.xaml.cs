using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;
using vCDR.src;
using System.Text.RegularExpressions;

namespace vCDR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Mapbox mapbox;
        private DispatcherTimer zuluTimer;
        List<JObject> codedRoutes = new List<JObject>();
        JObject airports = new JObject();

        public MainWindow()
        {
            InitializeComponent();
            InitializeZuluClock();
            InitializeDatabase();
            InitializeMapView();
            OriginTextBox.Focus();
        }

        private void InitializeZuluClock()
        {
            zuluTimer = new DispatcherTimer();
            zuluTimer.Interval = TimeSpan.FromSeconds(1);
            zuluTimer.Tick += Timer_Tick;
            zuluTimer.Start(); // Start the timer
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTime utcNow = DateTime.UtcNow;
            Application.Current.Dispatcher.Invoke(() =>
            {
                TilteBarZuluTime.Text = DateTime.UtcNow.ToString("HH:mm:ss") + "Z";
                this.UpdateLayout();
            });
        }

        private void InitializeDatabase()
        {
            string codedRoutePath = Loader.LoadFile("Database", "codedswap_db.csv");
            codedRoutes = Parser.ParseDatabase(codedRoutePath);
            string airportsPath = File.ReadAllText(Loader.LoadFile("Database", "airports.json"));
            airports = JObject.Parse(airportsPath);
        }

        private void InitializeMapView()
        {
            string mapHtmlPath = Loader.LoadFile("Database", "map.html");
            MapWebView.Source = new Uri(mapHtmlPath);
            mapbox = new Mapbox(MapWebView);
        }

        private List<Route> ParseCodedRoutes(string originSearch, string destinationSearch, string fixSearch)
        {
            var matches = new List<Route>();

            foreach (JObject route in codedRoutes)
            {
                string origin = route["Orig"]?.ToString();
                string destination = route["Dest"]?.ToString();
                string depFix = route["DepFix"]?.ToString();
                string nav = route["NavEqp"]?.ToString();
                string dcntr = route["DCNTR"]?.ToString();
                string acntr = route["ACNTR"]?.ToString();
                string tcntrs = route["TCNTRs"]?.ToString();
                string codedRouteString = route["Route String"]?.ToString();
                codedRouteString = codedRouteString.Replace($"{origin} ", "").Replace($" {destination}", "");

                switch (nav)
                {
                    case "1":
                        nav = "BASIC";
                        break;
                    case "2":
                        nav = "RNAV";
                        break;
                    case "3":
                        nav = "Q";
                        break;
                    default:
                        break;
                }

                bool isOriginMatch = string.IsNullOrWhiteSpace(originSearch) || (origin.Contains(originSearch) || dcntr.Contains(originSearch));
                bool isDestinationMatch = string.IsNullOrWhiteSpace(destinationSearch) || (destination.Contains(destinationSearch) || acntr.Contains(destinationSearch));
                bool isFixMatch = string.IsNullOrWhiteSpace(fixSearch) || codedRouteString.Contains(fixSearch) || depFix.Contains(fixSearch);

                if (isOriginMatch && isDestinationMatch && isFixMatch)
                {
                    if (!string.IsNullOrEmpty(codedRouteString))
                    {
                        if (origin.StartsWith("K") || origin.StartsWith("C")) origin = origin.Substring(1);
                        if (destination.StartsWith("K") || destination.StartsWith("C")) destination = destination.Substring(1);
                        var match = new Route
                        {
                            Orig = origin,
                            Dest = destination,
                            DepartureFix = depFix,
                            Nav = nav,
                            Coordination = route["CoordReq"]?.ToString(),
                            CodedRoute = codedRouteString
                        };
                        matches.Add(match);
                    }
                }
            }

            matches = matches.OrderBy(m => m.Coordination != "N").ToList();

            return matches;
        }

        private (double Latitude, double Longitude)? GetAirportCoordinates(string originSearch)
        {
            var features = airports["features"] as JArray;

            if (features != null)
            {
                foreach (var feature in features)
                {
                    var textArray = feature["properties"]?["text"] as JArray;

                    if (textArray != null && textArray.Count > 0)
                    {
                        string icao = textArray[0].ToString();
                        if (icao == originSearch)
                        {
                            var coordinates = feature["geometry"]?["coordinates"] as JArray;

                            if (coordinates != null && coordinates.Count == 2)
                            {
                                double longitude = coordinates[0].ToObject<double>();
                                double latitude = coordinates[1].ToObject<double>();

                                return (latitude, longitude);
                            }
                        }
                    }
                }
            }

            return null;
        }

        private List<(double, double)> GetRouteCoordinates(string route)
        {
            List<(double, double)> coordinates = new List<(double, double)>();
            string[] waypoints = route.Split(' ');
            for (int i = 0; i < waypoints.Count(); i++)
            {
                var waypoint = waypoints[i];
                if (waypoint.Length == 3)
                {
                    if (Regex.IsMatch(waypoint, @"^[A-Za-z]\d+"))
                    {
                        MessageBox.Show($"AWY: {waypoint}");
                    }
                    else if (waypoint.Length > 0 && char.IsDigit(waypoint[waypoint.Length - 1]))
                    {
                        if (i == 0)
                        {
                            MessageBox.Show($"SID: {waypoint}");
                        }
                    }
                    else
                    {
                        var coordinate = Route.GetVOR(waypoint);
                        if (coordinate != null)
                        {
                            //MessageBox.Show($"VOR: {waypoint} | Lat: {coordinate.Value.Latitude} | Lon: {coordinate.Value.Longitude}");
                            coordinates.Add((coordinate.Value.Latitude, coordinate.Value.Longitude));
                        }
                    }
                }
                else if (waypoint.Length == 4)
                {
                    if (Regex.IsMatch(waypoint, @"^[A-Za-z]\d+"))
                    {
                        MessageBox.Show($"AWY: {waypoint}");
                    }
                    else if (waypoint.Length > 0 && char.IsDigit(waypoint[waypoint.Length - 1]))
                    {
                        if (i == 0)
                        {
                            MessageBox.Show($"SID: {waypoint}");
                        }
                        else if (i == waypoints.Count() - 1)
                        {
                            MessageBox.Show($"STAR: {waypoint}");
                        }
                    }
                }
                else if (waypoint.Length == 5)
                {
                    var coordinate = Route.GetFix(waypoint);
                    if (coordinate != null)
                    {
                        //MessageBox.Show($"FIX: {waypoint} | Lat: {coordinate.Value.Latitude} | Lon: {coordinate.Value.Longitude}");
                        coordinates.Add((coordinate.Value.Latitude, coordinate.Value.Longitude));
                    }
                }
                else if (waypoint.Length == 6 && Regex.IsMatch(waypoint, @"\d"))
                {
                    if (i == 0)
                    {
                        MessageBox.Show($"SID: {waypoint}");
                    }
                    else if (i == waypoints.Count() - 1)
                    {
                        MessageBox.Show($"STAR: {waypoint}");
                    }
                }
            }
            return coordinates;
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            string originSearch = OriginTextBox.Text.ToUpper();
            string destinationSearch = DestinationTextBox.Text.ToUpper();
            string fixSearch = FixTextBox.Text.ToUpper();

            if (!string.IsNullOrWhiteSpace(originSearch) || !string.IsNullOrWhiteSpace(destinationSearch) || !string.IsNullOrWhiteSpace(fixSearch))
            {
                List<Route> codedMatches = ParseCodedRoutes(originSearch, destinationSearch, fixSearch);
                RouteTable.ItemsSource = null;
                RouteTable.ItemsSource = codedMatches;

                OriginTextBox.Text = string.Empty;
                DestinationTextBox.Text = string.Empty;
                FixTextBox.Text = string.Empty;
                OriginTextBox.Focus();

                var originCoordinates = GetAirportCoordinates(originSearch);
                var destinationCoordinates = GetAirportCoordinates(destinationSearch);
                List<(double, double)> routeCoordinates = GetRouteCoordinates(
                    !string.IsNullOrEmpty(fixSearch)
                    ? fixSearch
                    : (codedMatches != null && codedMatches.Count > 0 ? codedMatches[0].CodedRoute : string.Empty)
                );
                if (originCoordinates != null)
                {
                    double originLat = originCoordinates.Value.Latitude;
                    double originLon = originCoordinates.Value.Longitude;

                    if (destinationCoordinates != null)
                    {
                        double destinationLat = destinationCoordinates.Value.Latitude;
                        double destinationLon = destinationCoordinates.Value.Longitude;

                        mapbox.AddLine(originLat, originLon, destinationLat, destinationLon);
                    }
                    else
                    {
                        mapbox.AddMarker(originLat, originLon);
                    }
                }
                else if (destinationCoordinates != null)
                {
                    double destinationLat = destinationCoordinates.Value.Latitude;
                    double destinationLon = destinationCoordinates.Value.Longitude;
                    mapbox.AddMarker(destinationLat, destinationLon);
                }
                else if (routeCoordinates.Count != 0)
                {
                }

                if (originCoordinates == null && destinationCoordinates == null && codedMatches.Count == 0)
                {
                    MessageBox.Show("No matching routes found.", "Invalid search");
                    RouteTable.ItemsSource = null;
                }
            }
            else
            {
                MessageBox.Show("Please input at least one search field.", "No search terms");
            }

            OriginTextBox.Focus();
        }

        private void MapButtonClick(object sender, RoutedEventArgs e)
        {
            if (MapWebView.Visibility == Visibility.Hidden)
            {
                DataBorder.Visibility = Visibility.Hidden;
                MapWebView.Visibility = Visibility.Visible;
                MapButton.Tag = "pack://application:,,,/Images/Grid.png";
            }
            else if (MapWebView.Visibility == Visibility.Visible)
            {
                DataBorder.Visibility = Visibility.Visible;
                MapWebView.Visibility = Visibility.Hidden;
                MapButton.Tag = "pack://application:,,,/Images/Map.png";
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                int caretPosition = textBox.CaretIndex;
                textBox.Text = textBox.Text.ToUpper();
                textBox.CaretIndex = caretPosition;
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchButtonClick(sender, e);
                this.Focus();
            }
        }

        private void HyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
                e.Handled = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
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
                MainWindowGrid.Margin = new Thickness(0);
                MaximizeButton.Tag = "pack://application:,,,/Images/Maximize.png";
                this.WindowState = WindowState.Normal;
            }
            else
            {
                MaximizeButton.ToolTip = "Restore";
                MainWindowGrid.Margin = new Thickness(8, 8, 8, 0);
                MaximizeButton.Tag = "pack://application:,,,/Images/MaximizeMinimize.png";
                this.WindowState = WindowState.Maximized;
            }
        }

        private void WindowStateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                MaximizeButton.ToolTip = "Restore";
                MainWindowGrid.Margin = new Thickness(8, 8, 8, 0);
                MaximizeButton.Tag = "pack://application:,,,/Images/MaximizeMinimize.png";
            }
            else
            {
                MaximizeButton.ToolTip = "Maximize";
                MainWindowGrid.Margin = new Thickness(0);
                MaximizeButton.Tag = "pack://application:,,,/Images/Maximize.png";
            }
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
    }
}
