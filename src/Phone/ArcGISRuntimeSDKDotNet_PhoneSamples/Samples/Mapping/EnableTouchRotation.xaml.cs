using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
	/// <summary>
	/// 
	/// </summary>
    /// <title>Rotación</title>
    /// <category>Cartografía</category>
	public sealed partial class EnableTouchRotation : Page
	{
		public EnableTouchRotation()
		{
			this.InitializeComponent();
            mapView1.Map.InitialViewpoint = new Viewpoint(new Envelope(-344448.8744537411, -1182136.4258723476, 3641336.505047027, 1471520.6499406511, SpatialReference.Create(21897)));
            mapView1.InteractionOptions.RotationOptions.IsEnabled = true;
		}

		private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
		{
			if(mapView1 == null)
				return;
            var isOn = (sender as AppBarToggleButton).IsChecked;
            mapView1.InteractionOptions.RotationOptions.IsEnabled = (bool)isOn ? false : true;            
		}
	}
}
