using Esri.ArcGISRuntime.ArcGISServices;
using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
	/// <summary>
	/// 
	/// </summary>
    /// <title>Lista de Subcapas</title>
    /// <category>Servicios Dinámicos</category>
	public sealed partial class SubLayerList : Page, INotifyPropertyChanged
    {
        private ArcGISDynamicMapServiceLayer dynamicServiceLayer;
        public SubLayerList()
        {
            this.InitializeComponent();
            mapView1.Map.InitialViewpoint = new Viewpoint(new Envelope(146667.7512488081, 20565.688878213987, 1804280.2926010732, 1984870.7250208315, SpatialReference.Create(21892)));
            mapView1.Loaded += mapView1_Loaded;
        }

        async void mapView1_Loaded(object sender, RoutedEventArgs e)
        {
            //iterate over all layers
            var taskResults = new List<Task<AllLayersServiceInfo>>();
            foreach (var layer in mapView1.Map.Layers)
            {
                if (layer is ArcGISDynamicMapServiceLayer)
                    taskResults.Add((layer as ArcGISDynamicMapServiceLayer).GetAllDetailsAsync());

                else if (layer is ArcGISTiledMapServiceLayer)
                    taskResults.Add((layer as ArcGISTiledMapServiceLayer).GetAllDetailsAsync());

            }

            var allLayers = await Task.WhenAll(taskResults);

            //show single node for tiledLayers



            //show sub-layer list for Dynamic Layers
            dynamicServiceLayer = mapView1.Map.Layers["DynamicLayerCalifornia"] as ArcGISDynamicMapServiceLayer;
            if (dynamicServiceLayer != null)
            {

                await dynamicServiceLayer.InitializeAsync();
                var dyn = dynamicServiceLayer.CreateDynamicLayerInfosFromLayerInfos();
                dynamicServiceLayer.VisibleLayers = GetDefaultVisibleLayers(dyn);

                DataContext = this;
                Layers = new ObservableCollection<DynamicLayerInfo>(dyn);

                var test = Layers.Select(x => new { Name = x.Name, Visibility = x.DefaultVisibility, Id = x.ID }).ToList();
            }
        }

        private ObservableCollection<DynamicLayerInfo> _layers = new ObservableCollection<DynamicLayerInfo>();
        public ObservableCollection<DynamicLayerInfo> Layers { get { return _layers; } set { _layers = value; NotifyPropertyChanged("Layers"); } }



        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox tickedCheckBox = sender as CheckBox;

            string serviceName = tickedCheckBox.Name;
            bool visible = (bool)tickedCheckBox.IsChecked;

            int layerIndex = (int)tickedCheckBox.Tag;

            //ArcGISDynamicMapServiceLayer dynamicServiceLayer = mapView1.Map.Layers[serviceName] as   ArcGISDynamicMapServiceLayer;

            List<int> visibleLayerList =
                dynamicServiceLayer.VisibleLayers != null
                ? dynamicServiceLayer.VisibleLayers.ToList() : new List<int>();

            if (visible)
            {
                if (!visibleLayerList.Contains(layerIndex))
                    visibleLayerList.Add(layerIndex);
            }
            else
            {
                if (visibleLayerList.Contains(layerIndex))
                    visibleLayerList.Remove(layerIndex);
            }

            if (visibleLayerList.Count == 0)
                visibleLayerList = new List<int>() { -1 };

            dynamicServiceLayer.VisibleLayers = new ObservableCollection<int>( visibleLayerList);
            mapView1.SetView(mapView1.Extent);
        }

        private ObservableCollection<int> GetDefaultVisibleLayers(IEnumerable<DynamicLayerInfo> layerInfos)
        {
            List<int> visibleLayerIDList = new List<int>();

            var layerInfoArray = layerInfos.ToArray();

            for (int index = 0; index < layerInfoArray.Count(); index++)
            {
                if (layerInfoArray[index].DefaultVisibility)
                    visibleLayerIDList.Add(index);
            }
            return new ObservableCollection<int>(visibleLayerIDList);
        }



        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
