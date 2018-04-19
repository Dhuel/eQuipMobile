using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace eQuipMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SitesPage : ContentPage
    {
        private SQLiteConnection _connection;
        private static AssetDetailNames Names_ = JsonConvert.DeserializeObject<AssetDetailNames>(Application.Current.Properties["DesignInformation"].ToString());
        public SitesPage(SQLiteConnection connection_)
        {
            MessagingCenter.Subscribe<string>(this, "Datawedge", (sender) => {
                SearchBar_.Text = sender;
            });
            InitializeComponent();
            BarcodeImage.Source = ImageSource.FromResource("eQuipMobile.Images.Camera_icon.png");
            _connection = connection_;
            sitesListView.ItemsSource = GetSites();
        }

        private void sitesListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            sitesListView.SelectedItem = null;
        }

        private async void sitesListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var site = e.Item as SitesClass;
            await Navigation.PushAsync(new LocationPage(_connection, site, Names_));
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            sitesListView.ItemsSource = GetSites(e.NewTextValue);
        }
        List<SitesClass> GetSites(string searchText = null)
        {
            //Return List od SitesClass
            var sites = SitesClass.DbToSite(Database.Sites.GetTableData(_connection), _connection, Names_);
            if (String.IsNullOrWhiteSpace(searchText))
                return sites;
            return sites.Where(c => (c.SiteName.Contains(searchText)|| c.SiteCode.Contains(searchText))).ToList();
        }
        private void BarcodeIconTapped(object sender, EventArgs e)
        {
            BarcodeScannerPage();
        }
        private async void BarcodeScannerPage()
        {
            try
            {
                //setup options
                var frontCamera = Database.Settings.GetTableData(_connection).FrontCamera;
                var options = new MobileBarcodeScanningOptions
                {
                    AutoRotate = false,
                    UseFrontCameraIfAvailable = frontCamera,
                    TryHarder = true,
                };

                //add options and customize page
                var scanPage = new ZXingScannerPage(options)
                {
                    DefaultOverlayTopText = "Align the barcode within the frame",
                    DefaultOverlayBottomText = string.Empty,
                    DefaultOverlayShowFlashButton = true
                };
                scanPage.Title = "Scanning";
                string _result = "";
                scanPage.OnScanResult += (result) =>
                {
                    // Stop scanning
                    scanPage.IsScanning = false;

                    // Pop the page and show the result
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopAsync();
                        _result = result.Text;
                        SearchBar_.Text = _result;
                    });
                };

                // Navigate to our scanner page
                await Navigation.PushAsync(scanPage);
            }
            catch (Exception exc)
            {
                DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                await DisplayAlert("Error", exc.Message, "OK");
            }
        }
    }
}