using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Security;
using Esri.ArcGISRuntime.Tasks.NetworkAnalyst;
using System;
using System.Globalization;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace ArcGISRuntimeSDKDotNet_PhoneSamples.Samples
{
    /// <summary>
    /// 
    /// </summary>
    /// <title>Ruteo</title>
    /// <category>Análisis de Redes</category>
    public sealed partial class Routing : Page
    {
        public Routing()
        {
            this.InitializeComponent();
            mapView1.Map.InitialViewpoint = new Viewpoint(new Envelope(-74.2311, 4.47791, -73.964, 4.8648, SpatialReference.Create(4326)));
        }

        private async void mapView1_Tapped(object sender, Esri.ArcGISRuntime.Controls.MapViewInputEventArgs e)
        {
            var mp = e.Location;
            Graphic stop = new Graphic() { Geometry = mp };
            var stopsGraphicsLayer = mapView1.Map.Layers["MyStopsGraphicsLayer"] as GraphicsLayer;
            stopsGraphicsLayer.Graphics.Add(stop);

            if (stopsGraphicsLayer.Graphics.Count > 1)
            {
                try
                {
                    var routeTask = new OnlineRouteTask(
                        new Uri("http://54.187.22.10:6080/arcgis/rest/services/RuteoBogota/NAServer/Route"));
                    var routeParams = await routeTask.GetDefaultParametersAsync();

                    routeParams.SetStops(stopsGraphicsLayer.Graphics);
                    routeParams.UseTimeWindows = false;
                    routeParams.OutSpatialReference = mapView1.SpatialReference;
                    routeParams.DirectionsLengthUnit = LinearUnits.Miles;
                    routeParams.DirectionsLanguage = new CultureInfo("es-CO"); // CultureInfo.CurrentCulture;

                    var result = await routeTask.SolveAsync(routeParams);
                    if (result != null)
                    {
                        if (result.Routes != null && result.Routes.Count > 0)
                        {
                            var firstRoute = result.Routes.FirstOrDefault();
                            var direction = firstRoute.RouteDirections.FirstOrDefault();

                            if (direction != null)
                            {
                                int totalMins = 0;
                                foreach (RouteDirection dir in firstRoute.RouteDirections)
                                    totalMins = totalMins + dir.Time.Minutes;

                                await new MessageDialog(string.Format("{0:N2} minutos", totalMins)).ShowAsync();
                            }

                            var routeLayer = mapView1.Map.Layers["MyRouteGraphicsLayer"] as GraphicsLayer;
                            routeLayer.Graphics.Add(firstRoute.RouteFeature as Graphic);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var _x = new MessageDialog(ex.Message, "Error").ShowAsync();
                }
            }
        }
    }
}