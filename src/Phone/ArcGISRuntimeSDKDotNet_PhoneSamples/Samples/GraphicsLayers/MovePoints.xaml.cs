using System.Linq;
using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Layers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using System;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Controls;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
	/// <summary>
	/// 
	/// </summary>
    /// <title>Mover Puntos</title>
    /// <category>Gráficos</category>
	public sealed partial class MovePoints : Page
    {
		public MovePoints()
        {
            this.InitializeComponent();
            mapView1.Map.InitialViewpoint = new Viewpoint(new Envelope(-344448.8744537411, -1182136.4258723476, 3641336.505047027, 1471520.6499406511, SpatialReference.Create(21897)));
			LoadData();
        }

		private void LoadData()
		{
			//Add some random points for editing
			Random r = new Random();
			var graphicsLayer = mapView1.Map.Layers["MyGraphicsLayer"] as GraphicsLayer;
            for (int i = 0; i < 20; i++)
			{
				graphicsLayer.Graphics.Add(
                    new Graphic(new MapPoint(r.NextDouble() * 3641336.505047027, r.NextDouble() * 1471520.6499406511, SpatialReference.Create(21897))));

			}
		}

		private Graphic graphicBeingEdited;

        private async void mapView1_MapViewTapped(object sender, Esri.ArcGISRuntime.Controls.MapViewInputEventArgs e)
        {
			var graphicsLayer = mapView1.Map.Layers["MyGraphicsLayer"] as GraphicsLayer;
			var editGraphicsLayer = mapView1.Map.Layers["EditGraphicsLayer"] as GraphicsLayer;
			if (graphicBeingEdited == null)
			{
				var hit = await graphicsLayer.HitTestAsync(sender as ViewBase, e.Position);
				if (hit != null)
				{
					graphicBeingEdited = hit;
					//highlight the active graphic
					graphicBeingEdited.IsSelected = true;
					//Create a temporary we can move around without 'disturbing' the original feature until commit
					Graphic g = new Graphic();
					g.Symbol = hit.Symbol ?? graphicsLayer.Renderer.GetSymbol(hit);
					g.Geometry = hit.Geometry;
					editGraphicsLayer.Graphics.Add(g);
				}
			}
			else //Commit and clean up
			{
				graphicBeingEdited.Geometry = e.Location;
				graphicBeingEdited.IsSelected = false;
				graphicBeingEdited = null;
				editGraphicsLayer.Graphics.Clear();
			}
        }

		private void mapView1_PointerMoved(object sender, PointerRoutedEventArgs e)
		{
			if (graphicBeingEdited != null)
			{
				var editGraphicsLayer = mapView1.Map.Layers["EditGraphicsLayer"] as GraphicsLayer;
				MapView mapview = (MapView)sender;
				var location = mapview.ScreenToLocation(e.GetCurrentPoint(mapview).Position);
				editGraphicsLayer.Graphics[0].Geometry = location;
			}
		}
    }
}
