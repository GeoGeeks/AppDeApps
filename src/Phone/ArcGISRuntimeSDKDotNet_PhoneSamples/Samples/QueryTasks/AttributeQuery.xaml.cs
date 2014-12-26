using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Query;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
	/// <summary>
	/// 
	/// </summary>
    /// <title>Consulta de Atributos</title>
	/// <category>Tareas de Consulta</category>

	public sealed partial class AttributeQuery : Page
    {
        public AttributeQuery()
        {
            this.InitializeComponent();
            mapView1.Map.InitialViewpoint = new Viewpoint(new Envelope(-344448.8744537411, -1182136.4258723476, 3641336.505047027, 1471520.6499406511, SpatialReference.Create(21897)));
            InitializeComboBox();
        }

        private async void InitializeComboBox()
        {
            QueryTask queryTask = new QueryTask(new Uri("http://54.187.22.10:6080/arcgis/rest/services/Resultados_presidenciales/MapServer/0"));


            Query query = new Query("1=1")
            {
                ReturnGeometry = false,
            };
            query.OutFields.Add("presidenciales.DBO.DEPARTAMENTO.NOMBRE");

            try
            {
                var result = await queryTask.ExecuteAsync(query);
                QueryComboBox.ItemsSource = result.FeatureSet.Features.OrderBy(x => x.Attributes["presidenciales.DBO.DEPARTAMENTO.NOMBRE"]);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private async void QueryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await GetAttributes();
        }

        private async Task GetAttributes()
        {
            QueryTask queryTask = new QueryTask(new Uri("http://54.187.22.10:6080/arcgis/rest/services/Resultados_presidenciales/MapServer/0"));

            var qryText = (string)(QueryComboBox.SelectedItem as Graphic).Attributes["presidenciales.DBO.DEPARTAMENTO.NOMBRE"];
            Query query = new Query(qryText)
            {
                OutFields = OutFields.All,
                ReturnGeometry = true,
                OutSpatialReference = mapView1.SpatialReference
            };
            try
            {
                ResultsGrid.ItemsSource = null;
                progress.IsActive = true;
                var result = await queryTask.ExecuteAsync(query);
                var featureSet = result.FeatureSet;
                // If an item has been selected            
                GraphicsLayer graphicsLayer = mapView1.Map.Layers["MyGraphicsLayer"] as GraphicsLayer;
                graphicsLayer.Graphics.Clear();

                if (featureSet != null && featureSet.Features.Count > 0)
                {
                    var symbol = LayoutRoot.Resources["DefaultFillSymbol"] as Esri.ArcGISRuntime.Symbology.Symbol;
                    var g = featureSet.Features[0];
                    graphicsLayer.Graphics.Add(g as Graphic);
                    var selectedFeatureExtent = g.Geometry.Extent;
                    Envelope displayExtent = selectedFeatureExtent.Expand(1.3);
                    mapView1.SetView(displayExtent);
                    ResultsGrid.ItemsSource = g.Attributes;
                }
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
}