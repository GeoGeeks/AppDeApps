using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Tasks.Query;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
	/// <summary>
	/// 
	/// </summary>
    /// <title>Identificar</title>
    /// <category>Tareas de Consulta</category>
	public sealed partial class Identify : Page
    {
        public Identify()
        {
            this.InitializeComponent();
            mapView1.Map.InitialViewpoint = new Viewpoint(new Envelope(-344448.8744537411, -1182136.4258723476, 3641336.505047027, 1471520.6499406511, SpatialReference.Create(21897)));
        }

        private async void mapView1_Tapped_1(object sender, Esri.ArcGISRuntime.Controls.MapViewInputEventArgs e)
        {
            await RunIdentify(e.Location);
        }

        private async Task RunIdentify(MapPoint mp)
        {
            IdentifyParameters identifyParams = new IdentifyParameters(mp, mapView1.Extent, 2, (int)mapView1.ActualHeight, (int)mapView1.ActualWidth)
            {
                LayerOption = LayerOption.Visible,
                SpatialReference = mapView1.SpatialReference,
            };

            IdentifyTask identifyTask = new IdentifyTask(new Uri("http://54.187.22.10:6080/arcgis/rest/services/" +
                "Resultados_presidenciales/MapServer"));
            progress.IsActive = true;

            try
            {
                TitleComboBox.ItemsSource = null;
                ResultsGrid.ItemsSource = null;
                var result = await identifyTask.ExecuteAsync(identifyParams);

                GraphicsLayer graphicsLayer = mapView1.Map.Layers["MyGraphicsLayer"] as GraphicsLayer;
                graphicsLayer.Graphics.Clear();
                graphicsLayer.Graphics.Add(new Graphic() { Geometry = mp });

                var _dataItems = new List<DataItem>();
                if (result != null && result.Results != null && result.Results.Count > 0)
                {
                    foreach (var r in result.Results)
                    {
                        Feature feature = r.Feature;
                        string title = r.Value.ToString() + " (" + r.LayerName + ")";
                        _dataItems.Add(new DataItem()
                        {
                            Title = title,
                            Data = feature.Attributes
                        });
                    }
                }
                TitleComboBox.ItemsSource = _dataItems;
                if (_dataItems.Count > 0)
                    TitleComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                progress.IsActive = false;
            }
        }

        private void TitleComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            int index = TitleComboBox.SelectedIndex;
            var _dataItems = TitleComboBox.ItemsSource as IList<DataItem>;
            if (index > -1)
                ResultsGrid.ItemsSource = _dataItems[index].Data;
        }

        private void KeyLoaded(object sender, object e)
        {
            TextBlock textBlock = (TextBlock)sender;
            dynamic dyn = textBlock.DataContext;
            textBlock.Text = dyn.Key;
        }

        private void ValueLoaded(object sender, object e)
        {
            TextBlock textBlock = (TextBlock)sender;
            dynamic dyn = textBlock.DataContext;
            textBlock.Text = Convert.ToString(dyn.Value, CultureInfo.InvariantCulture);
        }

        private void AppBarToggleButton_Click(object sender, RoutedEventArgs e)
        {
            PropertiesBorder.Visibility = (PropertiesBorder.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }

       

    }

    public class DataItem : INotifyPropertyChanged
    {
        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged("Title");
                }
            }
        }

        private IDictionary<string, object> data;
        public IDictionary<string, object> Data
        {
            get { return data; }
            set
            {
                if (data != value)
                {
                    data = value;
                    OnPropertyChanged("Data");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}