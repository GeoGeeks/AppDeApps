﻿using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
	/// <summary>
	/// 
	/// </summary>
    /// <title>Distancia</title>
    /// <category>Geometría</category>
	public sealed partial class Distance : Page
    {
        GraphicsLayer inputGraphicsLayer;
        Polyline inputPolylineGeometry;
        MapPoint inputPointGeometry;
        public Distance()
        {
            InitializeComponent();

            mapView1.Map.InitialViewpoint = new Viewpoint(new Envelope(-74.2311, 4.47791, -73.964, 4.8648, SpatialReference.Create(4326)));
            inputGraphicsLayer = mapView1.Map.Layers["MyGraphicsLayer"] as GraphicsLayer;

        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {

            inputGraphicsLayer.Graphics.Clear();

            //hide ui elements
            //InstructionsTextBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            StartButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ResultsTextBlock.Visibility = Visibility.Collapsed;

            //show ui elements
            //PolylineInstructionsTextBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;
            //PointInstructionsTextBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;


            //Get the user's input geometry and add it to the map
            inputPolylineGeometry = (await mapView1.Editor.RequestShapeAsync(DrawShape.Polyline)) as Polyline;
            inputGraphicsLayer.Graphics.Add(new Graphic { Geometry = inputPolylineGeometry });

            //PolylineInstructionsTextBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //PointInstructionsTextBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;


            inputPointGeometry = (await mapView1.Editor.RequestShapeAsync(DrawShape.Point)) as MapPoint;
            inputGraphicsLayer.Graphics.Add(new Graphic { Geometry = inputPointGeometry, Symbol = new SimpleMarkerSymbol { Color = Colors.Red, Size = 12 } });

            //Convert to WebMercator so that we can get the results in meters
            var wmPolyline = GeometryEngine.Project(inputPolylineGeometry, SpatialReferences.WebMercator);
            var wmPoint = GeometryEngine.Project(inputPointGeometry, SpatialReferences.WebMercator) as MapPoint;

            var result = GeometryEngine.NearestCoordinate(wmPolyline, wmPoint);



            var resultStr = string.Format("Distancia: {0} metros", result.Distance.ToString("n3"));
            ResultsTextBlock1.Text = resultStr;
            ResultsTextBlock.Visibility = Visibility.Visible;
            StartButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            ResetButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //PointInstructionsTextBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //PolylineInstructionsTextBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }



        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {

            inputGraphicsLayer.Graphics.Clear();

            //show ui elements
            StartButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            //InstructionsTextBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;

            //hide ui elements
            //PolylineInstructionsTextBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            //PointInstructionsTextBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ResetButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ResetButton.IsEnabled = false;


        }

    }
}
