using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Layers;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using Esri.ArcGISRuntime.Controls;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
	/// <summary>
	/// 
	/// </summary>
    /// <title>Modificación en Código</title>
    /// <category>Servicios Dinámicos</category>
	public sealed partial class DynamicLayersInCode : Page
    {
        public DynamicLayersInCode()
        {
            this.InitializeComponent();
			//mapView1.Map.InitialViewpoint = new Viewpoint(new Envelope(-3548912, -1847469, 2472012, 1742990, SpatialReference.Create(102009)));
        }

        private void ApplyRangeValueClick(object sender, RoutedEventArgs e)
        {
            ClassBreaksRenderer newClassBreaksRenderer = new ClassBreaksRenderer();
            newClassBreaksRenderer.Field = "Educacion";
            var Infos = new ClassBreakInfoCollection();
            Infos.Add(new ClassBreakInfo()
            {

                Minimum = 0,
                Maximum = 5,
                Symbol = new SimpleFillSymbol()
                {
                    Color = Color.FromArgb(255, 0, 255, 0)
                }
            });

            Infos.Add(new ClassBreakInfo()
            {
                Maximum = 10,
                Symbol = new SimpleFillSymbol()
                {
                    Color = Color.FromArgb(255, 100, 255, 100)
                }
            });

            Infos.Add(new ClassBreakInfo()
            {
                Maximum = 20,
                Symbol = new SimpleFillSymbol()
                {
                    Color = Color.FromArgb(255, 0, 255, 200)
                }
            });

            Infos.Add(new ClassBreakInfo()
            {
                Maximum = 30,
                Symbol = new SimpleFillSymbol()
                {
                    Color = Color.FromArgb(255, 0, 255, 255)
                }
            });

            Infos.Add(new ClassBreakInfo()
            {
                Maximum = 41,
                Symbol = new SimpleFillSymbol()
                {
                    Color = Color.FromArgb(255, 0, 0, 255)
                }
            });
            newClassBreaksRenderer.Infos = Infos;
            var layer = mapView1.Map.Layers["Col"] as ArcGISDynamicMapServiceLayer;
            LayerDrawingOptionCollection layerDrawingOptionCollection = new LayerDrawingOptionCollection()
            {
                new LayerDrawingOptions()
            {
                LayerID = 3,
                Renderer = newClassBreaksRenderer
            }
            };

            layer.LayerDrawingOptions = layerDrawingOptionCollection;

            layer.VisibleLayers = new ObservableCollection<int> { 3 };
        }

        private void ApplyUniqueValueClick(object sender, RoutedEventArgs e)
        {

            UniqueValueRenderer uvr = new UniqueValueRenderer();
            uvr.Fields = new ObservableCollection<string>(new string[] { "NOMBRE_DPT" });
            uvr.DefaultSymbol = new SimpleFillSymbol() { Color = Colors.Gray };

            uvr.Infos.Add(new UniqueValueInfo { Values = new ObservableCollection<object>(new object[] { "Región Atlántico" }), Symbol = new SimpleFillSymbol() { Color = Colors.Purple, Outline = new SimpleLineSymbol() { Color = Colors.Transparent } } });
            uvr.Infos.Add(new UniqueValueInfo { Values = new ObservableCollection<object>(new object[] { "Región Andina"}), Symbol = new SimpleFillSymbol() { Color = Colors.Green, Outline = new SimpleLineSymbol() { Color = Colors.Transparent } } });
            uvr.Infos.Add(new UniqueValueInfo { Values = new ObservableCollection<object>(new object[] { "Región Amazonía"}), Symbol = new SimpleFillSymbol() { Color = Colors.Cyan, Outline = new SimpleLineSymbol() { Color = Colors.Transparent } } });
            uvr.Infos.Add(new UniqueValueInfo { Values = new ObservableCollection<object>(new object[] { "Región Pacífica"}), Symbol = new SimpleFillSymbol() { Color = Colors.Yellow, Outline = new SimpleLineSymbol() { Color = Colors.Transparent } } });
            uvr.Infos.Add(new UniqueValueInfo { Values = new ObservableCollection<object>(new object[] { "Región Orinoquía"}), Symbol = new SimpleFillSymbol() { Color = Colors.Blue, Outline = new SimpleLineSymbol() { Color = Colors.Transparent } } });
            uvr.Infos.Add(new UniqueValueInfo { Values = new ObservableCollection<object>(new object[] { "Archipiélago San Andrés Providencia y Santa Catalina" }), Symbol = new SimpleFillSymbol() { Color = Colors.Red, Outline = new SimpleLineSymbol() { Color = Colors.Transparent } } });
            
            var layer = mapView1.Map.Layers["Col"] as ArcGISDynamicMapServiceLayer;
            LayerDrawingOptionCollection layerDrawingOptionCollection = new LayerDrawingOptionCollection()
            {
                new LayerDrawingOptions()
            {
                LayerID = 2,
                Renderer = uvr
            }
            };

            layer.LayerDrawingOptions = layerDrawingOptionCollection;

            layer.VisibleLayers = new ObservableCollection<int> { 2 };
        }

        private void ChangeLayerOrderClick(object sender, RoutedEventArgs e)
        {
            var layer = mapView1.Map.Layers["Col"] as ArcGISDynamicMapServiceLayer;
            layer.LayerDrawingOptions.Clear();
            layer.DynamicLayerInfos = layer.CreateDynamicLayerInfosFromLayerInfos();
            var aDynamicLayerInfo = layer.DynamicLayerInfos[0];
            layer.DynamicLayerInfos.RemoveAt(0);
            layer.DynamicLayerInfos.Add(aDynamicLayerInfo);
        }
        /*
        private void AddLayerClick(object sender, RoutedEventArgs e)
        {
            var layer = mapView1.Map.Layers["Col"] as ArcGISDynamicMapServiceLayer;
            layer.LayerDrawingOptions.Clear();
            layer.DynamicLayerInfos = layer.CreateDynamicLayerInfosFromLayerInfos();
            layer.DynamicLayerInfos.Insert(0, new DynamicLayerInfo()
            {
                ID = 4,
                Source = new LayerDataSource()
                {
                    DataSource = new TableDataSource()
                    {
                        WorkspaceID = "MyDatabaseWorkspaceIDSSR2",
                        DataSourceName = "ss6.gdb.Lakes"
                    }
                }
            });
            layer.LayerDrawingOptions.Add(new LayerDrawingOptions()
            {
                LayerID = 4,
                Renderer = new SimpleRenderer()
            {
                Symbol = new SimpleFillSymbol()
                {
                    Color = Color.FromArgb((int)255, (int)0, (int)0, (int)255)
                }
            }
            });

            layer.VisibleLayers.Add(3);
            layer.VisibleLayers.Add(4);
        }*/
    }
}