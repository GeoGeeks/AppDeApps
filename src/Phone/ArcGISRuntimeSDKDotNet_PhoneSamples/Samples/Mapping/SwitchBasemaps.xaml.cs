using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
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

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
    /// <summary>
    /// 
    /// </summary>
    /// <title>Mapa Base</title>
    /// <category>Cartografía</category>
    public sealed partial class SwitchBasemaps : Page
    {
        public SwitchBasemaps()
        {
            this.InitializeComponent();
            mapView1.Map.InitialViewpoint = new Viewpoint(new Envelope(-344448.8744537411, -1182136.4258723476, 3641336.505047027, 1471520.6499406511, SpatialReference.Create(21897)));
        }

        private void SwitchBaseMapButton(object sender, RoutedEventArgs e)
        {
            if (mapView1 != null && mapView1.Map.Layers.Count > 0)
            {
                mapView1.Map.Layers.RemoveAt(0);
                mapView1.Map.Layers.Add(new ArcGISTiledMapServiceLayer()
                {
                    ServiceUri = (sender as AppBarButton).Tag as string
                });
            }
        }
    }
}
