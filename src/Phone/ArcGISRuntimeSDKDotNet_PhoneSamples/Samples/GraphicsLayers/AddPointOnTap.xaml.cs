using System.Linq;
using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Layers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
	/// <summary>
	/// 
	/// </summary>
    /// <title>Agregar Punto</title>
    /// <category>Gráficos</category>
	public sealed partial class AddPointOnTap : Page
    {
        public AddPointOnTap()
        {
            this.InitializeComponent();
            mapView1.Map.InitialViewpoint = new Viewpoint(new Envelope(-344448.8744537411, -1182136.4258723476, 3641336.505047027, 1471520.6499406511, SpatialReference.Create(21897)));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var layer = mapView1.Map.Layers.OfType<GraphicsLayer>().First();
            layer.Graphics.Clear();
        }

        private void mapView1_MapViewTapped(object sender, Esri.ArcGISRuntime.Controls.MapViewInputEventArgs e)
        {
            // Convert screen point to map point
            var mapPoint = mapView1.ScreenToLocation(e.Position);

            // Create graphic
            Graphic g = new Graphic() { Geometry = mapPoint };

            // Get layer and add point to it
            var graphicsLayer = mapView1.Map.Layers["MyGraphicsLayer"] as GraphicsLayer;
            graphicsLayer.Graphics.Add(g);
        }
    }
}
