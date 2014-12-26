using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Windows.UI.Xaml.Controls;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
	/// <summary>
	/// 
	/// </summary>
    /// <title>Capa WMS</title>
    /// <category>Servicios Dinámicos</category>
	public sealed partial class WmsLayerSimple : Page
    {
        public WmsLayerSimple()
        {
            this.InitializeComponent();
            MyMapView.Map.InitialViewpoint = new Viewpoint(new Envelope(-344448.8744537411, -1182136.4258723476, 3641336.505047027, 1471520.6499406511, SpatialReference.Create(21897)));
        }
    }
}
