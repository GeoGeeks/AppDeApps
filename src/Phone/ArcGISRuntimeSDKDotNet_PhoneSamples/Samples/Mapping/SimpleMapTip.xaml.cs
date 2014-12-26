using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Windows.UI.Xaml.Controls;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
	/// <summary>
	/// 
	/// </summary>
    /// <title>Leyenda Simple</title>
    /// <category>Cartografía</category>
	public sealed partial class SimpleMapTip : Page
    {
        public SimpleMapTip()
        {
            this.InitializeComponent();
            mapView1.Map.InitialViewpoint = new Viewpoint(new Envelope(-344448.8744537411, -1182136.4258723476, 3641336.505047027, 1471520.6499406511, SpatialReference.Create(21897)));
            ((Grid)mapView1.Overlays.Items[0]).DataContext = new MapPoint(-74.049968, 4.673008, SpatialReferences.Wgs84);			
        }
    }
}
