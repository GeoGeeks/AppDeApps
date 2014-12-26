﻿using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ArcGISRuntimeSDKDotNet_PhoneSamples.Samples
{
    /// <summary>
    /// 
    /// </summary>
    /// <title>Etiqueta</title>
    /// <category>Geometría</category>
    public sealed partial class LabelPoints : Page
    {
        GraphicsLayer myGraphicsLayer;

        PictureMarkerSymbol pictureMarkerSymbol;
        Geometry inputGeom;
        public LabelPoints()
        {
            InitializeComponent();

            mapView1.Map.InitialViewpoint = new Viewpoint(new Envelope(-74.2311, 4.47791, -73.964, 4.8648, SpatialReference.Create(4326)));
            myGraphicsLayer = mapView1.Map.Layers["MyGraphicsLayer"] as GraphicsLayer;
            InitializePictureMarkerSymbol();

            mapView1.SpatialReferenceChanged += mapView1_SpatialReferenceChanged;
        }

        private async void InitializePictureMarkerSymbol()
        {
            try
            {
                var imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/x-24x24.png"));
                var imageSource = await imageFile.OpenReadAsync();
                pictureMarkerSymbol = LayoutRoot.Resources["MyPictureMarkerSymbol"] as PictureMarkerSymbol;
                await pictureMarkerSymbol.SetSourceAsync(imageSource);
            }
            catch (Exception ex)
            {
                var _x = new MessageDialog("Ha ocurrido un error : " + ex.Message, "Error en Etiqueta de Punto").ShowAsync();
            }
        }

        async void mapView1_SpatialReferenceChanged(object sender, EventArgs e)
        {
            mapView1.SpatialReferenceChanged -= mapView1_SpatialReferenceChanged;
            await DoLabelPoints();

        }
        private async Task DoLabelPoints()
        {
            try
            {
                if (mapView1.Editor.IsActive)
                    mapView1.Editor.Cancel.Execute(null);

                //Get the input polygon geometry from the user
                inputGeom = await mapView1.Editor.RequestShapeAsync(DrawShape.Polygon);

                if (inputGeom != null)
                {
                    //Add the polygon drawn by the user
                    var g = new Graphic
                    {
                        Geometry = inputGeom,
                    };
                    myGraphicsLayer.Graphics.Add(g);

                    //Get the label point for the input geometry
                    var labelPointGeom = GeometryEngine.LabelPoint(inputGeom);

                    if (labelPointGeom != null)
                    {
                        myGraphicsLayer.Graphics.Add(new Graphic { Geometry = labelPointGeom, Symbol = pictureMarkerSymbol });
                        ResetButton.IsEnabled = true;
                    }
                }

                await DoLabelPoints();
            }
            catch (TaskCanceledException) { }
            catch (Exception ex)
            {
                var _x = new MessageDialog("Ha ocurrido un error: " + ex.Message, "Error").ShowAsync();
            }
        }

        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            myGraphicsLayer.Graphics.Clear();
            ResetButton.IsEnabled = false;
            await DoLabelPoints();
        }
    }
}