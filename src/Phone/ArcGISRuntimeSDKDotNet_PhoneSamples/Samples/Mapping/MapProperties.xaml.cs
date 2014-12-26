using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using System;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace ArcGISRuntimeSDKDotNet_StoreSamples.Samples
{
	/// <summary>
	/// 
	/// </summary>
    /// <title>Propiedades</title>
    /// <category>Cartografía</category>
	public sealed partial class MapProperties : Page
    {
        public MapProperties()
        {
            this.InitializeComponent();
            //mapView1.Map.InitialViewpoint = new Viewpoint(new Envelope(-344448.8744537411, -1182136.4258723476, 3641336.505047027, 1471520.6499406511, SpatialReference.Create(21897)));
        }

        private void ShowHidePropertiesButton(object sender, RoutedEventArgs e)
        {
            PropertiesBorder.Visibility = (PropertiesBorder.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
        }
    }
    public class ProjectionConverter : IValueConverter
    {
        /// <summary>
        /// Converter for converting a geometry from one projection to another.
        /// The input geometry must have a spatial reference defined, and the converter parameter
        /// must but the Well-Known ID or the Well-Known text for the output spatial reference.
        /// </summary>
        /// <seealso cref="Esri.ArcGISRuntime.Geometry.GeometryEngine"/>

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Esri.ArcGISRuntime.Geometry.Geometry)
            {
                SpatialReference sref = null;
                if (parameter is SpatialReference)
                    sref = (SpatialReference)value;
                else if (parameter is int)
                {
                    sref = new SpatialReference((int)parameter);
                }
                else if (parameter is string)
                {
                    int wkid = -1;
                    if (int.TryParse((string)parameter, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out wkid))
                        sref = new SpatialReference(wkid);
                    else
                        sref = new SpatialReference((string)parameter);
                }
                if (sref != null)
                {
                    var geom = (Esri.ArcGISRuntime.Geometry.Geometry)value;
                    if (geom.Extent!=null&& !double.IsNaN(geom.Extent.XMin))
                        return GeometryEngine.Project(geom, sref);
                }
            }
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException">The requestUri was null.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

    }

}
