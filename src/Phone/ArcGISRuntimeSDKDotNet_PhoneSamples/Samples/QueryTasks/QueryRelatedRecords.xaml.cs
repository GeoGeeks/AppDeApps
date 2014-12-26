using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Tasks.Query;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace ArcGISRuntimeSDKDotNet_PhoneSamples.Samples
{
    /// <summary>
    /// 
    /// </summary>
    /// <title>Registros Relacionados</title>
    /// <category>Tareas de Consulta</category>
    public sealed partial class QueryRelatedRecords : Page
    {
        public QueryRelatedRecords()
        {
            this.InitializeComponent();
            mapView1.Map.InitialViewpoint = new Viewpoint(new Envelope(-83, -8, -60, 23, SpatialReferences.Wgs84));
        }

        private async Task RunQuery(Geometry geometry)
        {
            var l = mapView1.Map.Layers["GraphicsWellsLayer"] as GraphicsLayer;
            l.Graphics.Clear();
            QueryTask queryTask =
                new QueryTask(new Uri("http://54.187.22.10:6080/arcgis/rest/services/Resultados_presidenciales/MapServer/0"));

            Query query = new Query("1=1")
            {
                Geometry = geometry,
                ReturnGeometry = true,
                //OutSpatialReference = mapView1.SpatialReference,
                OutFields = OutFields.All
            };
            try
            {
                var result = await queryTask.ExecuteAsync(query);
                if (result.FeatureSet.Features != null && result.FeatureSet.Features.Count > 0)
                {
                    ResultsGrid.ItemsSource = result.FeatureSet.Features;
                    l.Graphics.AddRange(from g in result.FeatureSet.Features.OfType<Graphic>() select g);
                }
            }
            catch (Exception)
            {
            }
        }

        private async void mapView1_Tapped_1(object sender, Esri.ArcGISRuntime.Controls.MapViewInputEventArgs e)
        {
            await RunQuery(Expand(mapView1.Extent, e.Location, 0.01));
        }

        private Envelope Expand(Envelope mapExtent, MapPoint point, double pct)
        {
            return new Envelope(
                point.X - mapExtent.Width * (pct / 2), point.Y - mapExtent.Height * (pct / 2),
                point.X + mapExtent.Width * (pct / 2), point.Y + mapExtent.Height * (pct / 2),
                mapExtent.SpatialReference);
        }

        private async void ResultsGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                await RunRelationshipQuery(from item in e.AddedItems select Convert.ToInt64((item as Graphic).Attributes["OBJECTID"], CultureInfo.InvariantCulture));
            }
        }

        private async Task RunRelationshipQuery(IEnumerable<long> objectIds)
        {
            QueryTask queryTask =
               new QueryTask(new Uri("http://54.187.22.10:6080/arcgis/rest/services/Resultados_presidenciales/MapServer/0"));

            //Relationship query
            RelationshipParameters parameters = new RelationshipParameters(new List<long>(objectIds), 3)
            {
                OutSpatialReference = mapView1.SpatialReference
            };
            parameters.OutFields.AddRange(new string[] { "ESRI_OID, COD_DEPTO, DEPTO" });
            var result = await queryTask.ExecuteRelationshipQueryAsync(parameters);
            RelationshipResultsGrid.ItemsSource = result.RelatedRecordGroups.FirstOrDefault().Value;
        }


    }
}