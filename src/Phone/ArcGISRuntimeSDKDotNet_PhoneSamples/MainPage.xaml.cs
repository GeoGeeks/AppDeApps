﻿using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Portal;
using Esri.ArcGISRuntime.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ArcGISRuntimeSDKDotNet_PhoneSamples
{
    public sealed partial class MainPage : Page
    {
        private const string DEFAULT_SERVER_URL = "https://www.arcgis.com/sharing/rest";
        private const string DEFAULT_TOKEN_URL = "https://www.arcgis.com/sharing/generateToken";
        
        private SampleDataViewModel _sampleDataVM;
		private bool _hasDeployment;

        public MainPage()
        {
			// Define symbology path to Resources folder. This folder is included in the solution as a Content
            if (!ArcGISRuntimeEnvironment.IsInitialized)
            {
                ArcGISRuntimeEnvironment.SymbolsPath = @"arcgisruntime" + GetRuntimeVersionNumber() + @"\resources\symbols";
                Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.ClientId = "BfhTjgKdHlqHWcrp";
                try
                {
                    Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.Initialize();
                }
                catch (Exception ex)
                {
                    var _x = new MessageDialog("No se puede inicializar el Id de cliente. " + ex.Message).ShowAsync();
                }
            }
			this.InitializeComponent();

            if (IdentityManager.Current.Credentials.Count() > 0)
            {
                LoginGrid.Visibility = Visibility.Collapsed;
                CBar.Visibility = Visibility.Visible;
            }
                
			
			_sampleDataVM = new SampleDataViewModel();
			SampleDataPanel.DataContext = _sampleDataVM;

            DataContext = SampleDataSource.Current;
			CheckDeployment();
		}

		private string GetRuntimeVersionNumber()
		{
			// Get version number that is used in the deployment folder
			Assembly runtimeAssembly = typeof(ArcGISRuntimeEnvironment).GetTypeInfo().Assembly;

			var sdkVersion = string.Empty;
			var attr = CustomAttributeExtensions.GetCustomAttribute<AssemblyFileVersionAttribute>(runtimeAssembly);
			if (attr != null)
			{
				var version = attr.Version;
				string[] versions = attr.Version.Split(new[] { '.' });

				// Ensure that we only look maximum of 3 part version number ie. 10.2.4
				int partCount = 3;
				if (versions.Count() < 3)
					partCount = versions.Count();

				for (var i = 0; i < partCount; i++)
				{
					if (string.IsNullOrEmpty(sdkVersion))
						sdkVersion = versions[i];
					else
						sdkVersion += "." + versions[i];
				}
			}
			else
				throw new Exception("Cannot read version number from ArcGIS Runtime");

			return sdkVersion;
		}

		private async void CheckDeployment()
		{
			try
			{
				// Check that all folders are deployed - assuming that symbols folder contains all 
				// deployable dictionaries
				var appFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
				var runtimeFolder = await appFolder.GetFolderAsync("arcgisruntime" + GetRuntimeVersionNumber());
				var resourcesFolder = await runtimeFolder.GetFolderAsync("resources");
				var symbolsFolders = await resourcesFolder.GetFolderAsync("symbols");

				_hasDeployment = true;
        }
			catch (FileNotFoundException)
			{
				_hasDeployment = false;
			}
		}

        private async void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = (Sample)e.ClickedItem;

			// Check if sample needs symbols and if deployment is available with symbols
			if (item.RequiresSymbols && !_hasDeployment)
			{
					// Deployment folder is not found show sample not available page
					Frame.Navigate(typeof(SdkInstallNeededPage));
					return;
			}

			// Check if the app requires local data
			if (item.RequiresLocalData && !_sampleDataVM.HasData)
			{
				await new MessageDialog(
					"Este ejemplo require datos locales. Por favor descargue los datos.", "Se requieren datos")
					.ShowAsync();
				return;
			}

			GC.Collect();
			GC.WaitForPendingFinalizers();

            Frame.Navigate(item.Page);
        }

		private async void DownloadSampleDataButton_Click(object sender, RoutedEventArgs e)
		{
			await _sampleDataVM.DownloadLocalDataAsync();
		}
		
        public class SampleDataSource
        {
            private SampleDataSource()
            {
                var pages = from t in App.Current.GetType().GetTypeInfo().Assembly.ExportedTypes
                            where t.GetTypeInfo().IsSubclassOf(typeof(Page)) && t.FullName.Contains(".Samples.")
                            select t;

                Samples = (from p in pages
                           select new Sample()
                           {
                               Page = p,
                               Name = SplitCamelCasedWords(p.Name),
                               SampleFile = p.Name + ".xaml",
                               Category = "Misc",
							   RequiresLocalData = false
                           }).ToArray();

                //Update descriptions and category based on included XML Doc
                XDocument xdoc = null;
                try
                {
                    xdoc = XDocument.Load(new StreamReader(
                        this.GetType().GetTypeInfo().Assembly.GetManifestResourceStream("ArcGISRuntimeSDKDotNet_PhoneSamples.Assets.SampleDescriptions.xml")));
                    foreach (XElement member in xdoc.Descendants("member"))
                    {
                        try
                        {
                            string name = (string)member.Attribute("name");
                            if (name == null)
                                continue;
                            bool isType = name.StartsWith("T:", StringComparison.OrdinalIgnoreCase);
                            if (isType)
                            {
                                var match = (from s in Samples where name == "T:" + s.Page.FullName select s).FirstOrDefault();
                                if (match != null)
                                {
                                    var title = member.Descendants("title").FirstOrDefault();
                                    if (title != null && !string.IsNullOrWhiteSpace(title.Value))
                                        match.Name = title.Value.Trim();
                                    var summary = member.Descendants("summary").FirstOrDefault();
                                    if (summary != null && summary.Value is string)
                                        match.Description = summary.Value.Trim();
                                    var category = member.Descendants("category").FirstOrDefault();
                                    if (category != null && category.Value is string)
                                        match.Category = category.Value.Trim();
                                    var subcategory = member.Descendants("subcategory").FirstOrDefault();
                                    if (subcategory != null && category.Value is string)
                                        match.Subcategory = subcategory.Value.Trim();
									var localData = member.Descendants("localdata").FirstOrDefault();
									if (localData != null && localData.Value is string)
										match.RequiresLocalData = localData.Value.Trim().Equals(bool.TrueString, StringComparison.CurrentCultureIgnoreCase);

									// Get information if the sample needs symbols
									var requiresSymbols = member.Descendants("requiresSymbols").FirstOrDefault();
									if (requiresSymbols != null && requiresSymbols.Value is string)
									{
										var result = false;
										bool.TryParse(requiresSymbols.Value.Trim(), out result);
										match.RequiresSymbols = result;
									}
                                }
                            }
                        }
                        catch { } //ignore
                    }
                }
                catch { } //ignore
            }

            private static string SplitCamelCasedWords(string value)
            {
                var text = System.Text.RegularExpressions.Regex.Replace(value, "([a-z])([A-Z])", "$1 $2");
                return text.Replace("Arc GIS", "ArcGIS ");
            }

            public class SampleGroup
            {
                public SampleGroup(IEnumerable<Sample> samples)
                {
                    Items = samples;
                }
                public string Key { get; set; }

                public IEnumerable<Sample> Items { get; private set; }
            }

            public List<SampleGroup> SamplesByCategory
            {
                get
                {
                    List<SampleGroup> groups = new List<SampleGroup>();
                    List<string> groupOrder = new List<string>(new[] { "Cartografía", "Capas Cachadas", "Servicios Dinámicos", 
                        "Gráficos", "Geometría", "Tareas de Consulta", "Geocodificación", "Análisis de Redes", "Geoprocesamiento" });

                    var query = (from item in Samples
                                 orderby item.Category
                                 group item by item.Category into g
                                 select new { GroupName = g.Key, Items = g, GroupIndex = groupOrder.IndexOf(g.Key) })
                                    .OrderBy(g => g.GroupIndex < 0 ? int.MaxValue : g.GroupIndex);

                    foreach (var g in query)
                    {
                        groups.Add(new SampleGroup(g.Items.OrderBy(i => i.Subcategory).ThenBy(i => i.Name)) { Key = g.GroupName });
                    }

                     //Define order of Mapping samples
                    SampleGroup mappingSamplesGroup = groups.Where(i => i.Key == "Cartografía").First();
                    List<Sample> mappingSamples = new List<Sample>();
                    mappingSamples.Add(mappingSamplesGroup.Items.Where(i => i.Name == "Mapa Base").First());
                    mappingSamples.Add(mappingSamplesGroup.Items.Where(i => i.Name == "Propiedades").First());
                    mappingSamples.Add(mappingSamplesGroup.Items.Where(i => i.Name == "Rotación").First());
                    mappingSamples.Add(mappingSamplesGroup.Items.Where(i => i.Name == "Vista General").First());
                    mappingSamples.Add(mappingSamplesGroup.Items.Where(i => i.Name == "Lista de Capas").First());
                    mappingSamples.Add(mappingSamplesGroup.Items.Where(i => i.Name == "Ubicación").First());
                    mappingSamples.Add(mappingSamplesGroup.Items.Where(i => i.Name == "Leyenda Simple").First());
                    mappingSamples.Add(mappingSamplesGroup.Items.Where(i => i.Name == "Opciones de Interacción").First());
                    SampleGroup newMappingSamplesGroup = new SampleGroup(mappingSamples) { Key = mappingSamplesGroup.Key };
                    groups[groups.FindIndex(g => g.Key == mappingSamplesGroup.Key)] = newMappingSamplesGroup;

                    return groups;
                }
            }

            public IEnumerable<Sample> Samples { get; private set; }

            private static SampleDataSource m_Current;
            public static SampleDataSource Current
            {
                get
                {
                    if (m_Current == null)
                        m_Current = new SampleDataSource();
                    return m_Current;
                }
            }
        }

        public class Sample
        {
            public Type Page { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
            public string Subcategory { get; set; }
            public string Description { get; set; }
            public string SampleFile { get; set; }

			/// <summary>
			/// Defines if the sample needs symbol to work. 
			/// </summary>
			/// <remarks>This is used for samples that need something to being deployed like military symbology or S57 symbology.</remarks>
			public bool RequiresSymbols { get; set; }

			/// <summary>
			/// Defines if the sample needs local data to work.
			/// </summary>
			public bool RequiresLocalData { get; set; }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            //ResultsListBox.ItemsSource = null;
            progress.Visibility = Visibility.Visible;
            connectButton.Visibility = Visibility.Collapsed;
            try
            {
                var crd = await IdentityManager.Current.GenerateCredentialAsync(
                    DEFAULT_SERVER_URL, UserTextBox.Text, PasswordTextBox.Password);
                IdentityManager.Current.AddCredential(crd);

                //_portal = await ArcGISPortal.CreateAsync(new Uri(DEFAULT_SERVER_URL));
                LoginGrid.Visibility = Visibility.Collapsed;
                CBar.Visibility = Visibility.Visible;
            }
            catch
            {
                var _x = new MessageDialog("No se puede iniciar sesión. Por favor verifique los datos.").ShowAsync();
                progress.Visibility = Visibility.Collapsed;
                connectButton.Visibility = Visibility.Visible;
            }
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            IdentityManager.Current.RemoveCredential(IdentityManager.Current.FindCredential(DEFAULT_SERVER_URL));
            LoginGrid.Visibility = Visibility.Visible;
            progress.Visibility = Visibility.Collapsed;
            connectButton.Visibility = Visibility.Visible;
            CBar.Visibility = Visibility.Collapsed;
        }
    }

	// Converts a boolean to a SolidColorBrush (true = green, false = red)
	internal class BoolToGreenRedConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is bool)
				return new SolidColorBrush((bool)value ? Colors.Green : Colors.Red);
			else
				return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			var brush = value as SolidColorBrush;
			if (brush != null)
				return (brush.Color == Colors.Green);
			else
				return DependencyProperty.UnsetValue;
        }
    }
}

