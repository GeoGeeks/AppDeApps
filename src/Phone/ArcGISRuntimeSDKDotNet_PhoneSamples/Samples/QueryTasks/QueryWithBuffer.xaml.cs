using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Query;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using System.Linq;
using Esri.ArcGISRuntime.Controls;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
	/// <summary>
	/// 
	/// </summary>
    /// <title>Consulta con Buffer</title>
    /// <category>Tareas de Consulta</category>
	public sealed partial class QueryWithBuffer : Page
    {
        public QueryWithBuffer()
        {
            this.InitializeComponent();

            mapView1.Map.InitialViewpoint = new Viewpoint(new Envelope(605145.655397665, 10515511.3400704, 605887.368366165, 10516337.1944652, SpatialReference.Create(32718)));
            InitializePMS();
        }

        private async void InitializePMS()
        {
            try
            {
                var imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/i_pushpin.png"));
                var imageSource = await imageFile.OpenReadAsync();
                var pms = LayoutRoot.Resources["DefaultMarkerSymbol"] as PictureMarkerSymbol;
                await pms.SetSourceAsync(imageSource);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

         private async void mapView1_Tapped_1(object sender, Esri.ArcGISRuntime.Controls.MapViewInputEventArgs e)
        {
            var mp = e.Location;
            Graphic g = new Graphic() { Geometry = mp };
            var graphicsLayer = mapView1.Map.Layers["MyGraphicsLayer"] as GraphicsLayer;
            graphicsLayer.Graphics.Add(g);

            var bufferResult = GeometryEngine.Buffer(mp, 100);
            var bufferLayer = mapView1.Map.Layers["BufferLayer"] as GraphicsLayer;
            bufferLayer.Graphics.Add(new Graphic() { Geometry = bufferResult });


            var queryTask = new QueryTask(new Uri("http://services.arcgis.com/8DAUcrpQcpyLMznu/arcgis/rest/services/Manzanas/FeatureServer/0"));
            var query = new Query("1=1")
            {
                ReturnGeometry = true,
                OutSpatialReference = mapView1.SpatialReference,
                Geometry = bufferResult
            };
            query.OutFields.Add("OWNERNME1");

            try
            {
                var queryResult = await queryTask.ExecuteAsync(query);
                if (queryResult != null && queryResult.FeatureSet != null)
                {
                    var resultLayer = mapView1.Map.Layers["MyResultsGraphicsLayer"] as GraphicsLayer;
                    resultLayer.Graphics.AddRange(queryResult.FeatureSet.Features.OfType<Graphic>());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        
    }
}