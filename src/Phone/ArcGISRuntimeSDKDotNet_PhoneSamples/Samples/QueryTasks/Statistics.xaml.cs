using Esri.ArcGISRuntime.Tasks.Query;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
	/// <summary>
	/// 
	/// </summary>
    /// <title>Estadísticas</title>
    /// <category>Tareas de Consulta</category>
	public sealed partial class Statistics : Page
    {
        public Statistics()
        {
            this.InitializeComponent();

            RunQuery();
        }

        private async void RunQuery()
        {
            QueryTask queryTask =
                new QueryTask(new Uri("http://54.187.22.10:6080/arcgis/rest/services/Win_Phone_y_Runtime/Poblaci%C3%B3n_dane/MapServer/0"));

            Query query = new Query("1=1")
             {
                 GroupByFieldsForStatistics = new List<string> { "NOMBRE_DPT" },
                 OutStatistics = new List<OutStatistic> { 
                    new OutStatistic(){
                        OnStatisticField = "Poblacion",
                        OutStatisticFieldName = "subregionpopulation",
                        StatisticType = StatisticType.Sum
                    },
                    new OutStatistic(){
                        OnStatisticField = "NOMBRE_DPT",
                        OutStatisticFieldName = "numberofstates",
                        StatisticType = StatisticType.Count
                    }
                 }
             }; 
            try
            {
                var result = await queryTask.ExecuteAsync(query);
                if (result.FeatureSet.Features != null && result.FeatureSet.Features.Count > 0)
                {
                    ResultGrid.ItemsSource = result.FeatureSet.Features;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message); 
            }
        }
    }

    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // No format provided.
            if (parameter == null)
            {
                return value;
            }


            return String.Format((String)parameter, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }

}