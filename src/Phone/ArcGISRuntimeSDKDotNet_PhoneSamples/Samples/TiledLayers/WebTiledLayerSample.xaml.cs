﻿using Esri.ArcGISRuntime.Layers;
using System.Collections.Generic;
using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
    /// <summary>
    /// Demonstrates adding a Web tiled layer to a Map in XAML, and changing layer properties in code behind.
    /// Also demonstrates including Attribution as per Terms of Use for the various layers. 
    /// </summary>
    /// <title>Capa Web Cachada</title>
    /// <category>Capas Cachadas</category>
    public sealed partial class WebTiledLayerSample : Page
    {
        public WebTiledLayerSample()
        {
            this.InitializeComponent();
            string[] ABCD = new string[] { "a", "b", "c", "d" };
            string[] MQ_SUBDOMAINS = new string[] { "otile1", "otile2", "otile3", "otile4", };
            string[] ABC = new string[] { "a", "b", "c" };

            List<WebTiledLayerComboItem> items = new List<WebTiledLayerComboItem>()
            {
                new WebTiledLayerComboItem("MapQuest",
                    "http://{subDomain}.mqcdn.com/tiles/1.0.0/map/{level}/{col}/{row}.jpg",
                    "MapQuest",
                    MQ_SUBDOMAINS),

                 new WebTiledLayerComboItem("OpenCycleMap",
                    "http://{subDomain}.tile.opencyclemap.org/cycle/{level}/{col}/{row}.png",
                    "OpenCycleMap",
                    ABC),

                new WebTiledLayerComboItem("Dark de MapBox",
                    "http://{subDomain}.tiles.mapbox.com/v3/examples.map-cnkhv76j/{level}/{col}/{row}.png",
                    "Dark de MapBox",
                    ABCD),

                new WebTiledLayerComboItem("Terreno de MapBox",
                    "http://{subDomain}.tiles.mapbox.com/v3/mapbox.mapbox-warden/{level}/{col}/{row}.png",
                    "Terreno de MapBox",
                    ABCD),   
     
                new WebTiledLayerComboItem("Toner de Stamen",
                    "http://{subDomain}.tile.stamen.com/toner/{level}/{col}/{row}.png",
                    "Toner de Stamen",
                    ABCD),               
                new WebTiledLayerComboItem("Watercolor de Stamen",
                    "http://{subDomain}.tile.stamen.com/watercolor/{level}/{col}/{row}.jpg",
                    "Watercolor de Stamen",
                    ABCD)
               
            };

            webTiledLayerComboBox1.ItemsSource = items;
            webTiledLayerComboBox1.DisplayMemberPath = "Name";
            MyMapView.Loaded += MyMapView_Loaded;
        }

        private void MyMapView_Loaded(object sender, RoutedEventArgs e)
        {
            webTiledLayerComboBox1.SelectedIndex = 0;
        }

        private class WebTiledLayerComboItem
        {
            public WebTiledLayerComboItem(string name, string urlTemplate, string copyrightText, string[] subDomains)
            {
                this.UrlTemplate = urlTemplate;
                this.CopyrightText = copyrightText;
                this.SubDomains = subDomains;
                this.Name = name;
            }

            public string Name { get; private set; }
            public string UrlTemplate { get; private set; }
            public string CopyrightText { get; private set; }
            public string[] SubDomains { get; private set; }
        }

        private void webTiledLayerComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WebTiledLayerComboItem selectedItem = (sender as ComboBox).SelectedItem as WebTiledLayerComboItem;
            if (selectedItem != null)
            {
                if (MyMapView.Map.Layers["MyWebTiledLayer"] != null)
                    MyMapView.Map.Layers.Remove("MyWebTiledLayer");

                var myWebTiledLayer = new WebTiledLayer { ID = "MyWebTiledLayer" };
                myWebTiledLayer.CopyrightText = selectedItem.CopyrightText;
                myWebTiledLayer.TemplateUri = selectedItem.UrlTemplate;
                myWebTiledLayer.SubDomains = selectedItem.SubDomains;

                switch (selectedItem.Name)
                {
                    case "MapQuest":
                        Attribution.ContentTemplate = Attribution.Resources["MapQuestAttribution"] as DataTemplate;
                        break;
                    case "Toner de Stamen":
                        Attribution.ContentTemplate = Attribution.Resources["StamenTonerAttribution"] as DataTemplate;
                        break;
                    case "Watercolor de Stamen":
                        Attribution.ContentTemplate = Attribution.Resources["StamenOtherAttribution"] as DataTemplate;
                        break;
                    case "OpenCycleMap":
                        Attribution.ContentTemplate = Attribution.Resources["OpenCycleMapAttribution"] as DataTemplate;
                        break;
                    case "Dark de MapBox":
                        Attribution.ContentTemplate = Attribution.Resources["MapboxAttribution"] as DataTemplate;
                        break;
                    case "Terreno de MapBox":
                        Attribution.ContentTemplate = Attribution.Resources["MapboxAttribution"] as DataTemplate;
                        break;
                    default:
                        Attribution.Visibility = Visibility.Collapsed;
                        break;
                }
                MyMapView.Map.Layers.Add(myWebTiledLayer);
            }
        }

        private async void Hyperlink_Click(Windows.UI.Xaml.Documents.Hyperlink sender, Windows.UI.Xaml.Documents.HyperlinkClickEventArgs args)
        {
            // Navigate to specified Uri
            await Launcher.LaunchUriAsync(sender.NavigateUri);
        }
    }
}
