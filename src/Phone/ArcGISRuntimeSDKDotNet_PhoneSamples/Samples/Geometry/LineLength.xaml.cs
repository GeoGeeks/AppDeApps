﻿using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Query;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
	/// <summary>
	/// 
	/// </summary>
    /// <title>Longitud de Línea</title>
    /// <category>Geometría</category>
	public sealed partial class LineLength : Page
    {
        GraphicsLayer myGraphicsLayer;
        Geometry inputGeom;
        public LineLength()
        {
            InitializeComponent();

            mapView1.Map.InitialViewpoint = new Viewpoint(new Envelope(-74.2311, 4.47791, -73.964, 4.8648, SpatialReference.Create(4326)));
            myGraphicsLayer = mapView1.Map.Layers["MyGraphicsLayer"] as GraphicsLayer;
        }

        private async Task DoGeodesicLength()
        {
            ResetButton.IsEnabled = false;

            try
            {
                if (mapView1.Editor.IsActive)
                    mapView1.Editor.Cancel.Execute(null);

                //Get the input polygon geometry from the user
                inputGeom = await mapView1.Editor.RequestShapeAsync(DrawShape.Polyline);

                if (inputGeom != null)
                {
                    //Add the polygon drawn by the user
                    var g = new Graphic
                    {
                        Geometry = inputGeom,
                    };
                    myGraphicsLayer.Graphics.Add(g);


                    //Get the label point for the input geometry
                    var length = GeometryEngine.GeodesicLength(inputGeom);
                    LineLengthTextBlock1.Text = length.ToString("N2") + " m";
                    LineLengthTextBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
            }
            catch (Exception)
            {

            }
            ResetButton.IsEnabled = true;

        }

        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            LineLengthTextBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            myGraphicsLayer.Graphics.Clear();
            await DoGeodesicLength();

        }


        private async void mapView1_LayerLoaded(object sender, LayerLoadedEventArgs e)
        {
            if (e.Layer.ID == "MyGraphicsLayer")
            {
                await DoGeodesicLength();
            }
        }


    }
}
