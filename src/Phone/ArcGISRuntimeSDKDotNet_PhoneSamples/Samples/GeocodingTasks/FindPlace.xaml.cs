using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using System;
using System.Linq;
using System.Threading;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ArcGISRuntimeSDKDotNet_PhoneSamples.Samples
{
	/// <summary>
	///
	/// </summary>
	/// <title>Encontrar Lugar</title>
	/// <category>Geocodificación</category>
	public sealed partial class FindPlace : Page
	{
		private const string OnlineLocatorUrl = "http://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer";

		private GraphicsOverlay _addressOverlay;
		private OnlineLocatorTask _locatorTask;

		/// <summary>Construct find place sample control</summary>
		public FindPlace()
		{
			InitializeComponent();
            MyMapView.Map.InitialViewpoint = new Viewpoint(new Envelope(-74.2311, 4.47791, -73.964, 4.8648, SpatialReference.Create(4326)));

			_addressOverlay = MyMapView.GraphicsOverlays[0]; ;

			_locatorTask = new OnlineLocatorTask(new Uri(OnlineLocatorUrl));
			_locatorTask.AutoNormalize = true;

			listResults.ItemsSource = _addressOverlay.Graphics;

			SetSimpleRendererSymbols();
		}

		// Setup the pin graphic and graphics overlay renderer
		private async void SetSimpleRendererSymbols()
		{
			try
			{
				var markerSymbol = new PictureMarkerSymbol() { Width = 48, Height = 48, YOffset = 24 };
				await markerSymbol.SetSourceAsync(new Uri("ms-appx:///Assets/location.png"));
				var renderer = new SimpleRenderer() { Symbol = markerSymbol };

				_addressOverlay.Renderer = renderer;
			}
			catch (Exception ex)
			{
				var _x = new MessageDialog("Selection Error: " + ex.Message, "Find Place Sample").ShowAsync();
			}
		}

		// Find matching places, create graphics and add them to the UI
		private async void FindButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				progress.Visibility = Visibility.Visible;
				listResults.Visibility = Visibility.Collapsed;
				_addressOverlay.Graphics.Clear();

				var param = new OnlineLocatorFindParameters(SearchTextBox.Text)
				{
					SearchExtent = MyMapView.Extent,
					Location = MyMapView.Extent.GetCenter(),
					MaxLocations = 5,
					OutSpatialReference = MyMapView.SpatialReference,
					OutFields = new string[] { "Place_addr" }
				};

				var candidateResults = await _locatorTask.FindAsync(param, CancellationToken.None);

				if (candidateResults == null || candidateResults.Count == 0)
					throw new Exception("No se encontraron candidatos en la extensión actual.");

				foreach (var candidate in candidateResults)
					AddGraphicFromLocatorCandidate(candidate);

				var extent = GeometryEngine.Union(_addressOverlay.Graphics.Select(g => g.Geometry)).Extent.Expand(1.1);
				await MyMapView.SetViewAsync(extent);

				listResults.Visibility = Visibility.Visible;
			}
			catch (AggregateException ex)
			{
				var innermostExceptions = ex.Flatten().InnerExceptions;
				if (innermostExceptions != null && innermostExceptions.Count > 0)
				{
					var _x = new MessageDialog(string.Join(" > ", innermostExceptions.Select(i => i.Message).ToArray()), "Error").ShowAsync();
				}
				else
				{
					var _x = new MessageDialog(ex.Message, "Error").ShowAsync();
				}
			}
			catch (Exception ex)
			{
				var _x = new MessageDialog(ex.Message, "Error").ShowAsync();
			}
			finally
			{
				progress.Visibility = Visibility.Collapsed;
			}
		}

		private void AddGraphicFromLocatorCandidate(LocatorFindResult candidate)
		{
			var location = GeometryEngine.Project(candidate.Feature.Geometry, SpatialReferences.Wgs84) as MapPoint;

			var graphic = new Graphic(location);
			graphic.Attributes["Name"] = candidate.Name;
			graphic.Attributes["Address"] = candidate.Feature.Attributes["Place_addr"];
			graphic.Attributes["LocationDisplay"] = string.Format("{0:0.000}, {1:0.000}", location.X, location.Y);

			_addressOverlay.Graphics.Add(graphic);
		}

		private void listResults_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
		{
			_addressOverlay.ClearSelection();

			if (e.AddedItems != null)
			{
				foreach (var graphic in e.AddedItems.OfType<Graphic>())
				{
					graphic.IsSelected = true;
				}
			}
		}
	}
}
