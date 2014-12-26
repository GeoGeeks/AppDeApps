using Esri.ArcGISRuntime.Layers;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ArcGISRuntimeSDKDotNet_PhoneSamples.Samples
{
    /// <summary>
    ///
    /// </summary>
    /// <title>Capa Cachada Local</title>
    /// <category>Capas Cachadas</category>
	/// <localdata>true</localdata>
    public partial class ArcGISLocalTiledLayerSample : Page
    {
        public ArcGISLocalTiledLayerSample()
        {
            InitializeComponent();

			MyMapView.Loaded += MyMapView_Loaded;
        }

        private async void MyMapView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var tpk = await GetSingleFileAsync(@"basemaps\campus.tpk");
                var layer = new ArcGISLocalTiledLayer(tpk) { ID = "local_basemap" };
                MyMapView.Map.Layers.Add(layer);
            }
            catch (Exception ex)
            {
                var _x = new MessageDialog(ex.Message, "Error").ShowAsync();
            }
        }

        /// <summary>
        /// Get a single file from the user
        /// </summary>
        /// <remarks>Copies the selected file to App Local Data before returning</remarks>
        internal static async Task<StorageFile> GetSingleFileAsync(string name)
        {
            // Attempt to open LocalFolder target
			try
			{
				return await ApplicationData.Current.LocalFolder.GetFileAsync(name);
			}
			catch (FileNotFoundException)
			{
				throw new Exception("Paquete local no encontrado. Descargue los datos desde la página inicial.");
			}
        }
    }
}
