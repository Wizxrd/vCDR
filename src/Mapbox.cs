using Microsoft.Web.WebView2.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace vCDR.src
{
    public class Mapbox
    {
        private WebView2 webView;
        public Mapbox(WebView2 webView)
        {
            this.webView = webView;
        }

        public async Task RemoveMarker()
        {
            string sourceId = "marker-source";
            string layerId = "marker-layer";
            string script = $@"
            try {{
                // Remove the existing source or layer if it exists
                if (map.getSource('{sourceId}')) {{
                    map.removeSource('{sourceId}');
                }}
                if (map.getLayer('{layerId}')) {{
                    map.removeLayer('{layerId}');
                }}
            }} catch (error) {{
                console.error('Error removing marker:', error);
            }}
            ";
            await webView.ExecuteScriptAsync(script);
        }

        private async Task RemoveLine()
        {
            string lineLayerId = "route-line-layer";
            string lineSourceId = "route-line-source";
            string markerLayerId = "marker-layer";
            string markerSourceId = "marker-source";

            string script = $@"
            if (map.getSource('{lineSourceId}')) {{
                map.removeSource('{lineSourceId}');
                map.removeLayer('{lineLayerId}');
            }}

            if (map.getSource('{markerSourceId}')) {{
                map.removeSource('{markerSourceId}');
                map.removeLayer('{markerLayerId}');
            }}";
            await webView.ExecuteScriptAsync(script);
        }

        public async void AddMarker(double latitude, double longitude)
        {
            await RemoveMarker();
            await RemoveLine();
            var markerGeoJson = $@"
            {{
                'type': 'FeatureCollection',
                'features': [
                    {{
                        'type': 'Feature',
                        'geometry': {{
                            'type': 'Point',
                            'coordinates': [{longitude}, {latitude}]
                        }},
                        'properties': {{
                            'icon': 'marker'
                        }}
                    }}
                ]
            }}";

            string sourceId = "marker-source";
            string layerId = "marker-layer";

            string script = $@"
            if (map.getSource('{sourceId}')) {{
                map.removeSource('{sourceId}');
            }}
            if (map.getLayer('{layerId}')) {{
                map.removeLayer('{layerId}');
            }}
            map.addSource('{sourceId}', {{
                'type': 'geojson',
                'data': {markerGeoJson}
            }});

            map.addLayer({{
                'id': '{layerId}',
                'type': 'symbol',
                'source': '{sourceId}',
                'layout': {{
                    'icon-image': 'marker', // Use a stock Mapbox icon
                    'icon-size': 2
                }}
            }});

            map.flyTo({{
                center: [{longitude}, {latitude}],
                zoom: 8,
                essential: true // Ensures the flyTo happens instantly
            }});";
            await webView.ExecuteScriptAsync(script);
        }

        public async void AddLine(double originLat, double originLon, double destinationLat, double destinationLon)
        {
            await RemoveMarker();
            await RemoveLine();
            var lineGeoJson = $@"
            {{
                'type': 'FeatureCollection',
                'features': [
                    {{
                        'type': 'Feature',
                        'geometry': {{
                            'type': 'LineString',
                            'coordinates': [
                                [{originLon}, {originLat}],
                                [{destinationLon}, {destinationLat}]
                            ]
                        }},
                        'properties': {{}}  // Optional properties for the line (e.g., color, width)
                    }}
                ]
            }}";

            var markerGeoJson = $@"
            {{
                'type': 'FeatureCollection',
                'features': [
                    {{
                        'type': 'Feature',
                        'geometry': {{
                            'type': 'Point',
                            'coordinates': [{originLon}, {originLat}]
                        }},
                        'properties': {{
                            'icon': 'marker'
                        }}
                    }},
                    {{
                        'type': 'Feature',
                        'geometry': {{
                            'type': 'Point',
                            'coordinates': [{destinationLon}, {destinationLat}]
                        }},
                        'properties': {{
                            'icon': 'marker'
                        }}
                    }}
                ]
            }}";

            string lineLayerId = "route-line-layer";
            string lineSourceId = "route-line-source";
            string markerLayerId = "marker-layer";
            string markerSourceId = "marker-source";

            string script = $@"
            if (map.getSource('{lineSourceId}')) {{
                map.removeSource('{lineSourceId}');
                map.removeLayer('{lineLayerId}');
            }}

            if (map.getSource('{markerSourceId}')) {{
                map.removeSource('{markerSourceId}');
                map.removeLayer('{markerLayerId}');
            }}

            // Add the line connecting origin and destination
            map.addSource('{lineSourceId}', {{
                'type': 'geojson',
                'data': {lineGeoJson}
            }});

            map.addLayer({{
                'id': '{lineLayerId}',
                'type': 'line',
                'source': '{lineSourceId}',
                'layout': {{
                    'line-join': 'round',
                    'line-cap': 'round'
                }},
                'paint': {{
                    'line-color': '#c8cb0f',
                    'line-width': 2
                }}
            }});

            // Add the markers for origin and destination
            map.addSource('{markerSourceId}', {{
                'type': 'geojson',
                'data': {markerGeoJson}
            }});

            map.addLayer({{
                'id': '{markerLayerId}',
                'type': 'symbol',
                'source': '{markerSourceId}',
                'layout': {{
                    'icon-image': 'marker', // Use a stock Mapbox icon
                    'icon-size': 2
                }}
            }});

            var centerLat = ({originLat} + {destinationLat}) / 2;
            var centerLon = ({originLon} + {destinationLon}) / 2;

            // Calculate the bounding box to fit both points
            var latDiff = Math.abs({originLat} - {destinationLat});
            var lonDiff = Math.abs({originLon} - {destinationLon});
            var padding = 0.1; // Adjust the padding to your preference

            // Fly to the center of the map based on the midpoint
            var center = [centerLon, centerLat];
            map.flyTo({{
                center: center,
                zoom: 4,  // This can be adjusted or calculated dynamically if needed
                essential: true
            }});

            // Adjust the zoom level to fit both markers
            map.fitBounds([[
                Math.min({originLon}, {destinationLon}) - padding,
                Math.min({originLat}, {destinationLat}) - padding
            ], [
                Math.max({originLon}, {destinationLon}) + padding,
                Math.max({originLat}, {destinationLat}) + padding
            ]], {{
                padding: 20,  // Adjust padding around the bounds
                maxZoom: 10    // Max zoom level to avoid zooming in too much
            }});
            ";
            await webView.ExecuteScriptAsync(script);
        }

    }
}
