﻿using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
	/// <summary>
	/// 
	/// </summary>
    /// <title>Extender</title>
    /// <category>Servicios Dinámicos</category>
	public sealed partial class Stretch : Page
    {
        public Stretch()
        {
            this.InitializeComponent();
            mapView1.Map.InitialViewpoint =
                new Viewpoint(new Envelope(-13483041, 5707158, -13300798, 5776035, SpatialReferences.WebMercator));
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ArcGISImageServiceLayer imageLayer = mapView1.Map.Layers["ImageServiceLayer"] as ArcGISImageServiceLayer;
                RenderingRule renderingRule = new RenderingRule();
                renderingRule.VariableName = "Raster";

                Dictionary<string, object> rasterParams = new Dictionary<string, object>();

                string[] strArray = (BandIDsTextBox.Text + "," + BandIDsTextBox1.Text + "," + BandIDsTextBox2.Text).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (strArray.Length == 1 || strArray.Length == 2 || strArray.Length > 3)
                {
                    ValidBandIdsTextBlock.Visibility = Visibility.Visible;
                    return;
                }
                else
                    ValidBandIdsTextBlock.Visibility = Visibility.Collapsed;

                renderingRule.RasterFunctionName = "Stretch";
                renderingRule.VariableName = "Raster";

                int stretchType = 0;
                if (StandardDevCheckBox.IsChecked.Value)
                    stretchType = 3;
                else if (HistogramCheckBox.IsChecked.Value)
                    stretchType = 4;
                else if (MinMaxCheckBox.IsChecked.Value)
                    stretchType = 5;

                rasterParams.Add("StretchType", stretchType);
                rasterParams.Add("NumberOfStandardDeviations",
                    string.IsNullOrEmpty(StnDevTextBox.Text) ? 1 : double.Parse(StnDevTextBox.Text));

                double[][] statistics = new double[3][] { 
                    new double[4] { 0.2, 222.46, 99.35, 1.64 }, 
                    new double[4] { 5.56, 100.345, 45.4, 3.96 }, 
                    new double[4] { 0, 352.37, 172.284, 2 } };
                rasterParams.Add("Statistics", statistics);

                double[] gamma = new double[] { 1.25, 2, 3.95 };
                rasterParams.Add("Gamma", gamma);


                int[] numArray = new int[strArray.Length];
                for (int i = 0; i < strArray.Length; i++)
                {
                    numArray[i] = int.Parse(strArray[i]);
                }

                imageLayer.BandIds = numArray.Length < 1 ? null : numArray;


                renderingRule.RasterFunctionArguments = rasterParams;
                imageLayer.RenderingRule = renderingRule;
            }
            catch (Exception)
            {
            }
        }

        private void ShowHideStretchOptions(object sender, RoutedEventArgs e)
        {
            StretchOptions.Visibility = (StretchOptions.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }
    }
}
